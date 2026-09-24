using ApiTCC.Data;
using ApiTCC.Models;
using API_TCC.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_TCC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PetwalkerController : ControllerBase
    {
        private readonly DataContext _context;

        public PetwalkerController(DataContext context)
        {
            _context = context;
        }

        // Pega o CPF do usuário logado a partir do token (setado em UsuariosController.CriarToken).
        // É assim que os endpoints "do petwalker logado" sabem em qual PetwalkerPerfil mexer,
        // sem precisar (e sem confiar) no app mandar o CPF por fora.
        private string? CpfLogado => User.FindFirstValue("Cpf");

        // GET /Petwalker/Disponiveis
        // Lista petwalkers disponíveis para exibir no mapa. Hoje filtra por AreaAtendimento
        // (texto livre) porque ainda não existem colunas de Latitude/Longitude no
        // PetwalkerPerfil — ver observação abaixo sobre isso.
        [AllowAnonymous]
        [HttpGet("Disponiveis")]
        public async Task<IActionResult> GetDisponiveis([FromQuery] string? area = null)
        {
            try
            {
                var query = _context.TB_PETWALKER_PERFIL
                    .Include(p => p.Usuario)
                    .Include(p => p.Avaliacoes)
                    .Where(p => p.Disponibilidade);

                if (!string.IsNullOrWhiteSpace(area))
                    query = query.Where(p => p.AreaAtendimento.ToLower().Contains(area.ToLower()));

                List<PetwalkerDto> petwalkers = await query
                    .Select(p => new PetwalkerDto
                    {
                        Cpf = p.Cpf,
                        Nome = p.Usuario.Nome,
                        Foto = p.Usuario.Foto,
                        Disponibilidade = p.Disponibilidade,
                        AreaAtendimento = p.AreaAtendimento,
                        QuantidadeAvaliacoes = p.Avaliacoes.Count,
                        NotaMedia = p.Avaliacoes.Any() ? p.Avaliacoes.Average(a => a.Nota) : 0
                    })
                    .ToListAsync();

                return Ok(petwalkers);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // GET /Petwalker/{cpf}
        // Detalhe de um petwalker específico (tela de perfil ao clicar no pin do mapa).
        [AllowAnonymous]
        [HttpGet("{cpf}")]
        public async Task<IActionResult> GetPorCpf(string cpf)
        {
            try
            {
                PetwalkerDetalheDto? petwalker = await _context.TB_PETWALKER_PERFIL
                    .Include(p => p.Usuario)
                    .Include(p => p.Avaliacoes)
                    .Where(p => p.Cpf == cpf)
                    .Select(p => new PetwalkerDetalheDto
                    {
                        Cpf = p.Cpf,
                        Nome = p.Usuario.Nome,
                        Foto = p.Usuario.Foto,
                        Disponibilidade = p.Disponibilidade,
                        AreaAtendimento = p.AreaAtendimento,
                        QuantidadeAvaliacoes = p.Avaliacoes.Count,
                        NotaMedia = p.Avaliacoes.Any() ? p.Avaliacoes.Average(a => a.Nota) : 0,
                        Avaliacoes = p.Avaliacoes.Select(a => new AvaliacaoDto
                        {
                            Id = a.Id,
                            Comentario = a.Comentario,
                            Nota = a.Nota,
                            DataPublicacao = a.DataPublicacao
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (petwalker == null)
                    return NotFound("Petwalker não encontrado.");

                return Ok(petwalker);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // PUT /Petwalker/Disponibilidade
        // Só quem tem a role "Petwalker" no token consegue chamar isso — é a API
        // barrando um "Dono" de tentar se marcar como disponível pra passear.
        [Authorize(Roles = "Petwalker")]
        [HttpPut("Disponibilidade")]
        public async Task<IActionResult> AtualizarDisponibilidade(AtualizarDisponibilidadeDto dto)
        {
            try
            {
                if (CpfLogado == null)
                    return Unauthorized();

                PetwalkerPerfil? perfil = await _context.TB_PETWALKER_PERFIL
                    .FirstOrDefaultAsync(p => p.Cpf == CpfLogado);

                if (perfil == null)
                    return NotFound("Perfil de petwalker não encontrado para este usuário.");

                perfil.Disponibilidade = dto.Disponibilidade;
                await _context.SaveChangesAsync();

                return Ok(perfil.Disponibilidade);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // POST /Petwalker/TornarPetwalker
        // Caso de uso: um usuário cadastrado só como "Dono" decide virar petwalker depois.
        // Atualiza TipoUsuario e cria o PetwalkerPerfil sem precisar recadastrar a conta.
        // Obs: como TipoUsuario muda, o app precisa pedir login de novo pra pegar um
        // token novo com a role "Petwalker" incluída.
        [Authorize]
        [HttpPost("TornarPetwalker")]
        public async Task<IActionResult> TornarPetwalker([FromBody] string areaAtendimento)
        {
            try
            {
                string? cpf = User.FindFirstValue("Cpf");
                if (cpf == null)
                    return Unauthorized();

                Usuario? usuario = await _context.TB_USUARIOS
                    .Include(u => u.PetwalkerPerfil)
                    .FirstOrDefaultAsync(u => u.Cpf == cpf);

                if (usuario == null)
                    return NotFound("Usuário não encontrado.");

                if (usuario.PetwalkerPerfil != null)
                    return BadRequest("Usuário já possui perfil de petwalker.");

                usuario.TipoUsuario = usuario.TipoUsuario == "Dono" ? "Ambos" : "Petwalker";
                usuario.PetwalkerPerfil = new PetwalkerPerfil
                {
                    Cpf = usuario.Cpf,
                    AreaAtendimento = string.IsNullOrWhiteSpace(areaAtendimento) ? usuario.Cep : areaAtendimento,
                    Disponibilidade = false
                };

                await _context.SaveChangesAsync();

                return Ok("Perfil de petwalker criado. Faça login novamente para atualizar seu token.");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }
    }
}
