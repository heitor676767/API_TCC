using ApiTCC.Data;
using ApiTCC.Models;
using API_TCC.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ApiTCC.DTOs;

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
        public async Task<IActionResult> GetDisponiveis(
            [FromQuery] string? area = null,
            [FromQuery] double? lat = null,
            [FromQuery] double? lng = null,
            [FromQuery] double raioKm = 2

            )
        {
            try
            {
                if (lat.HasValue != lng.HasValue)
                    return BadRequest("Informe lat e lng juntos");

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

                if (lat.HasValue && lng.HasValue)
                {
                    // Haversine calculado em memória: pro volume de um TCC é simples e evita
                    // depender de como o EF traduz funções trigonométricas pro SQL Server.
                    petwalkers = petwalkers
                        .Where(p => p.Latitude.HasValue && p.Longitude.HasValue)
                        .Select(p =>
                        {
                            p.DistanciaKm = Math.Round(
                                DistanciaEmKm(lat.Value, lng.Value, (double)p.Latitude!.Value, (double)p.Longitude!.Value), 2);
                            return p;
                        })
                        .Where(p => p.DistanciaKm <= raioKm)
                        .OrderBy(p => p.DistanciaKm)
                        .ToList();
                }

                return Ok(petwalkers);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        private static double GrausParaRadianos(double graus) => graus * Math.PI / 180.0;

        // Formula de Haversine: nao sei explicar ainda mas calcula distancia em km entre dois pontos (lat/lng em graus)
        private static double DistanciaEmKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double raioTerraKm = 6371.0;
            double dLat = GrausParaRadianos(lat2 - lat1);
            double dLng = GrausParaRadianos(lng2 - lng1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(GrausParaRadianos(lat1)) * Math.Cos(GrausParaRadianos(lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng/2) ;
            return raioTerraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
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
                        Latitude = p.Latitude,        
                        Longitude = p.Longitude,        
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
        // Caso de uso: um usuário cadastrado como "Dono" decide virar petwalker depois.
        // IMPORTANTE: isso TROCA o tipo da conta (não existe mais "Ambos") — o usuário
        // deixa de ter a role "Dono" e passa a ser só "Petwalker".
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

                // Não existe mais "Ambos": virar petwalker agora troca o tipo por completo,
                // o usuário deixa de ser Dono (perde a role Dono no próximo login).
                usuario.TipoUsuario = "Petwalker";
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

        // PUT/Petwalker/Localizacao
        // O petwalker logado define onde ele atende (ponto base que aparece no mapa)
        [Authorize(Roles = "Petwalker")]
        [HttpPut("Localizacao")]
        public async Task<IActionResult> AtualizarLocalizacao(AtualizarLocalizacaoDto dto)
        {
            try
            {
                if (CpfLogado == null)
                    return Unauthorized();

                PetwalkerPerfil? perfil = await _context.TB_PETWALKER_PERFIL.FirstOrDefaultAsync(p => p.Cpf == CpfLogado);

                if (perfil == null)
                    return NotFound("Perfil de petwalker nao encontrado para este usuario!");

                perfil.Latitude = dto.Latitude;
                perfil.Longitude = dto.Longitude;
                await _context.SaveChangesAsync();

                return Ok(new { perfil.Latitude, perfil.Longitude });
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message + " - " + ex.InnerException);
            }
        }
    }
}
