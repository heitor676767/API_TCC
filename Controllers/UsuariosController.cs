using API_TCC.DTOs;
using ApiTCC.Data;
using ApiTCC.DTOs;
using ApiTCC.Models;
using ApiTCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiTCC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        public readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public UsuariosController(DataContext context, IConfiguration configuration, EmailService emailService) 
        { 
            _context = context; 
            _configuration = configuration;
            _emailService = emailService;
        }

        private async Task<bool> EmailExistente(string email)
        {
            return await _context.TB_USUARIOS.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }
        private async Task<bool> CpfExistente(string cpf)
        {
            return await _context.TB_USUARIOS.AnyAsync(x => x.Cpf == cpf);
        }
        private async Task<bool> TelefoneExistente(string telefone)
        {
            return await _context.TB_USUARIOS.AnyAsync(x => x.Telefone == telefone);
        }

        private static List<Claim> ObterRoleClaims(Usuario usuario)
        {
            var roles = new List<Claim>();

            if (usuario.TipoUsuario == "Dono" || usuario.TipoUsuario == "Ambos")
                roles.Add(new Claim(ClaimTypes.Role, "Dono"));

            if (usuario.TipoUsuario == "Petwalker" || usuario.TipoUsuario == "Ambos")
                roles.Add(new Claim(ClaimTypes.Role, "Petwalker"));

            return roles;
        }

        private string CriarToken(Usuario usuario)
{
        List<Claim> claims = new List<Claim>
        {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("Cpf", usuario.Cpf)
            };
            claims.AddRange(ObterRoleClaims(usuario));
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_configuration.GetSection("ConfiguracaoToken:Chave").Value));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [AllowAnonymous]
        [HttpGet("VerificarEmail")]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            bool existe = await EmailExistente(email);
            return Ok(existe);
        }

        [AllowAnonymous]
        [HttpGet("VerificarCpf")]
        public async Task<IActionResult> VerificarCpf(string cpf)
        {
            bool existe = await CpfExistente(cpf);
            return Ok(existe);
        }

        [AllowAnonymous]
        [HttpGet("VerificarTelefone")]
        public async Task<IActionResult> VerificarTelefone(string telefone)
        {
            return Ok(await TelefoneExistente(telefone));
        }

        [AllowAnonymous]
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuario user)
        {
            try
            {
                if (!ValidadorCpf.EhValido(user.Cpf))
                    throw new System.Exception("CPF inválido");

                if (await CpfExistente(user.Cpf))
                    throw new System.Exception("CPF já cadastrado");

                if (await EmailExistente(user.Email))
                    throw new System.Exception("E-mail já cadastrado");

                if (await TelefoneExistente(user.Telefone))
                    throw new System.Exception("Telefone já cadastrado");

                Criptografia.CriarPasswordHash(user.PasswordString, out byte[] hash, out byte[] salt);
                user.PasswordString = string.Empty;
                user.PasswordHash = hash;
                user.PasswordSalt = salt;

                // Diferenciação Dono x Petwalker acontece AQUI, na API: se o TipoUsuario
                // indicar que ele também é petwalker, garantimos a criação do perfil
                // correspondente (TB_PETWALKER_PERFIL) já no cadastro.
                if (user.TipoUsuario == "Petwalker" || user.TipoUsuario == "Ambos")
                {
                    user.PetwalkerPerfil ??= new PetwalkerPerfil();
                    user.PetwalkerPerfil.Cpf = user.Cpf;

                    // Se o app ainda não envia AreaAtendimento num campo próprio,
                    // usamos o CEP como valor provisório para não violar o NOT NULL.
                    if (string.IsNullOrWhiteSpace(user.PetwalkerPerfil.AreaAtendimento))
                        user.PetwalkerPerfil.AreaAtendimento = user.Cep;
                }
                else
                {
                    // Se for só Dono, garante que nenhum perfil de petwalker seja criado por engano.
                    user.PetwalkerPerfil = null;
                }

                await _context.TB_USUARIOS.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok(user.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        [AllowAnonymous]
        [HttpPost("Autenticar")]
        public async Task<IActionResult> AutenticarUsuario(Usuario credenciais)
        {
            try
            {
                Usuario? usuario = await _context.TB_USUARIOS
                    .Include(x => x.PetwalkerPerfil) // inclui o perfil pra o app já saber, no login, se esse usuário é petwalker
                    .FirstOrDefaultAsync(x => x.Cpf == credenciais.Cpf);

                if (usuario == null)
                    throw new System.Exception("Usuário ou senha incorretos");
                else if (!Criptografia.VerificarPasswordHash(credenciais.PasswordString, usuario.PasswordHash, usuario.PasswordSalt))
                    throw new System.Exception("Usuário ou senha incorretos");
                else
                {
                    usuario.PasswordString = string.Empty;
                    usuario.PasswordHash = null;
                    usuario.PasswordSalt = null;
                    usuario.Token = CriarToken(usuario);

                    return Ok(usuario);
                }

            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }

        }

        [HttpPut]
        public async Task<IActionResult> AtualizarUsuario(AtualizarUsuarioDto dto)
        {
            try
            {
                string? cpfLogado = User.FindFirstValue("Cpf");
                if (cpfLogado == null)
                    return Unauthorized();

                Usuario? usuario = await _context.TB_USUARIOS.FirstOrDefaultAsync(u => u.Cpf == cpfLogado);

                if (usuario == null)
                    return NotFound("Usuário não encontrado");

                bool telefonedeOutroUsuario = await _context.TB_USUARIOS.AnyAsync(u => u.Telefone == dto.Telefone && u.Cpf != cpfLogado);
                if (telefonedeOutroUsuario)
                    return BadRequest("Esse telefone já está em uso por outro usuário.");

                usuario.Nome = dto.Nome;
                usuario.Telefone = dto.Telefone;
                usuario.Cep = dto.Cep;
                usuario.Genero = dto.Genero ?? usuario.Genero;
                usuario.Foto = dto.Foto ?? usuario.Foto;

                await _context.SaveChangesAsync();

                return Ok(new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Cpf = usuario.Cpf,
                    Email = usuario.Email,
                    Telefone = usuario.Telefone,
                    Cep = usuario.Cep
                });
            }
            catch(DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("would be truncated"))
                    return BadRequest("Um dos campos enviaos excede o tamanho máximo permitido.");
                return BadRequest("Não foi possível atualizar o usuário. Confira os dados enviados.");
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message + " - " + ex.InnerException);
            }
        }

        [AllowAnonymous]//testando
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                List<UsuarioDto> usuarios = await _context.TB_USUARIOS
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Cpf = u.Cpf,
                        Nome = u.Nome,
                        Cep = u.Cep,
                        Email = u.Email,
                        TipoUsuario = u.TipoUsuario,
                        StatusUser = u.StatusUser,
                        Telefone = u.Telefone,
                        Genero = u.Genero,
                        Foto = u.Foto,
                        UltimoLogin = u.UltimoLogin,
                        DataCadastro = u.DataCadastro,
                        Pets = u.Pets.Select(p => new PetDto
                        {
                            Rga = p.Rga,
                            Nome = p.Nome,
                            Especie = p.Especie,
                            Raca = p.Raca,
                            Descricao = p.Descricao,
                            Peso = p.Peso,
                            Porte = p.Porte,
                            Sexo = p.Sexo,
                            CpfDono = p.CpfDono
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }

        }

        [AllowAnonymous]
        [HttpPost("EsqueciSenha")]
        public async Task<IActionResult> EsqueciSenha(EsqueciSenhaDto dto)
        {
            try
            {
                Usuario? usuario = await _context.TB_USUARIOS
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

                if (usuario == null)
                    return Ok(); // não revela se o e-mail existe ou não, por segurança

                string codigo = new Random().Next(100000, 999999).ToString();

                var recuperacao = new CodigoRecuperacao
                {
                    Email = dto.Email,
                    Codigo = codigo,
                    DataExpiracao = DateTime.Now.AddMinutes(15),
                    Usado = false
                };

                await _context.TB_CODIGOS_RECUPERACAO.AddAsync(recuperacao);
                await _context.SaveChangesAsync();

                await _emailService.EnviarCodigoAsync(dto.Email, codigo);

                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("RedefinirSenha")]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaDto dto)
        {
            try
            {
                var recuperacao = await _context.TB_CODIGOS_RECUPERACAO
                    .Where(c => c.Email.ToLower() == dto.Email.ToLower()
                             && c.Codigo == dto.Codigo
                             && !c.Usado
                             && c.DataExpiracao > DateTime.Now)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();

                if (recuperacao == null)
                    throw new System.Exception("Código inválido ou expirado");

                Usuario? usuario = await _context.TB_USUARIOS
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

                if (usuario == null)
                    throw new System.Exception("Usuário não encontrado");

                Criptografia.CriarPasswordHash(dto.NovaSenha, out byte[] hash, out byte[] salt);
                usuario.PasswordHash = hash;
                usuario.PasswordSalt = salt;

                recuperacao.Usado = true;

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
    
}
