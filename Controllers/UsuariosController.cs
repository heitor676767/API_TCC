using ApiTCC.Data;
using ApiTCC.Models;
using ApiTCC.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using API_TCC.DTOs;

namespace ApiTCC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        public readonly IConfiguration _configuration;

        public UsuariosController(DataContext context, IConfiguration configuration) 
        { 
            _context = context; 
            _configuration = configuration;
        }

        private async Task<bool> UsuarioExistente(string username)
        {
            if (await _context.TB_USUARIOS.AnyAsync(x => x.Nome.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        // Traduz o campo TipoUsuario ('Dono','Petwalker','Ambos') em uma ou mais roles do JWT.
        // É essa lista de roles que permite usar [Authorize(Roles = "Petwalker")] nos endpoints
        // e garantir a diferenciação de papéis na API, não só na interface do app.
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
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuario user)
        {
            try
            {
                if (await UsuarioExistente(user.Cpf))
                    throw new System.Exception("CPF ja existente");

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
                    .FirstOrDefaultAsync(x => x.Nome.ToLower().Equals(credenciais.Nome.ToLower()));

                if (usuario == null)
                    throw new System.Exception("Usuário não encontrado");
                else if (!Criptografia.VerificarPasswordHash(credenciais.PasswordString, usuario.PasswordHash, usuario.PasswordSalt))
                    throw new System.Exception("Senha incorreta.");
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

        [AllowAnonymous]//testando
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

    }
}
