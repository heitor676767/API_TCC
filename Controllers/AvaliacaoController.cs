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
                bool petExiste = await _context.TB_PETS.AnyAsync(p => p.Rga == dto.Rga);
                if (!petExiste)
                    return NotFound("Pet não encontrado.");

                bool petwalkerExiste = await _context.TB_PETWALKER_PERFIL.AnyAsync(p => p.Cpf == dto.CpfPetwalker);
                if (!petwalkerExiste)
                    return NotFound("Petwalker não encontrado.");

                var avaliacao = new Avaliacao
                {
                    Rga = dto.Rga,
                    CpfPetwalker = dto.CpfPetwalker,
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
