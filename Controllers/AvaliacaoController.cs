using API_TCC.DTOs;
using ApiTCC.Data;
using ApiTCC.DTOs;
using ApiTCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiTCC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AvaliacaoController : ControllerBase
    {
        private readonly DataContext _context;

        public AvaliacaoController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar(CriarAvaliacaoDto dto)
        {
            try
            {
                Passeio? passeio = await _context.TB_PASSEIOS.FirstOrDefaultAsync(p => p.IdPasseio == dto.IdPasseio);

                if (passeio == null)
                    return NotFound("Passeio nao encontrado");
                if (passeio.StatusPass != StatusPasseio.Finalizado)
                    return BadRequest($"Só é possível avaliar um passeio finalizado (status atual: '{passeio.StatusPass}').");

                bool jaAvaliado = await _context.TB_AVALIACOES.AnyAsync(a => a.IdPasseio == dto.IdPasseio);
                if (jaAvaliado)
                    return BadRequest("Essse passeio já foi avaliado.");
                

                var avaliacao = new Avaliacao
                {
                    IdPasseio = passeio.IdPasseio,
                    Rga = passeio.Rga,
                    CpfPetwalker = passeio.CpfPetwalker,
                    Nota = dto.Nota,
                    Comentario = dto.Comentario,
                    DataPublicacao = DateTime.Now
                };

                await _context.TB_AVALIACOES.AddAsync(avaliacao);
                await _context.SaveChangesAsync();

                return Ok(avaliacao.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }


        [HttpGet("GetByPetwalker")]
        public async Task<IActionResult> GetByPetwalker(string cpfPetwalker)
        {
            try
            {
                List<AvaliacaoDto> avaliacoes = await _context.TB_AVALIACOES
                    .Where(a => a.CpfPetwalker == cpfPetwalker)
                    .Select(a => new AvaliacaoDto
                    {
                        Id = a.Id,
                        Comentario = a.Comentario,
                        Nota = a.Nota,
                        DataPublicacao = a.DataPublicacao
                    })
                    .ToListAsync();

                return Ok(avaliacoes);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }


        [HttpGet("GetByPet")]
        public async Task<IActionResult> GetByPet(string rga)
        {
                try
                {
                    List<AvaliacaoDto> avaliacoes = await _context.TB_AVALIACOES
                        .Where(a => a.Rga == rga)
                        .Select(a => new AvaliacaoDto
                        {
                            Id = a.Id,
                            Comentario = a.Comentario,
                            Nota = a.Nota,
                            DataPublicacao = a.DataPublicacao
                        })
                        .ToListAsync();

                    return Ok(avaliacoes);
                }
                catch (System.Exception ex)
                {
                    return BadRequest(ex.Message + " _ " + ex.InnerException);
                }
         }


}    }
