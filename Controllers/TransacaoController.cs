using ApiTCC.Data;
using ApiTCC.DTOs;
using ApiTCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiTCC.Controllers
{
    // Pagamento simulado: não há gateway real, só o registro/atualização do status.
    // Sem exigir login por enquanto, pra teste
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class TransacaoController : ControllerBase
    {
        private const string Pendente = "Pendente";
        private const string Pago = "Pago";

        private readonly DataContext _context;

        public TransacaoController(DataContext context)
        {
            _context = context;
        }

        private static TransacaoDto ParaDto(Transacao t) => new TransacaoDto
        {
            IdTransacao = t.IdTransacao,
            IdPasseio = t.IdPasseio,
            MtdPgmt = t.MtdPgmt,
            StatusPgmt = t.StatusPgmt,
            Valor = t.Valor,
            DataPgmt = t.DataPgmt
        };

        [HttpGet("DoPasseio/{idPasseio}")]
        public async Task<IActionResult> GetDoPasseio(int idPasseio)
        {
            try
            {
                List<Transacao> transacoes = await _context.TB_TRANSACOES.Where(t => t.IdPasseio == idPasseio).ToListAsync();
                
                return Ok(transacoes.Select(ParaDto));
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message + " - " + ex.InnerException);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                Transacao? transacao = await _context.TB_TRANSACOES.FirstOrDefaultAsync(t => t.IdTransacao == id);

                if (transacao == null)
                    return NotFound("transacao nao encontrada!");

                return Ok(ParaDto(transacao));
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message + " - " + ex.InnerException);
            }
        }

        [HttpPut("{id}/Pagar")]
        public async Task<IActionResult> Pagar(int id, PagarTransacaoDto dto)
        {
            try
            {
                Transacao? transacao = await _context.TB_TRANSACOES.FirstOrDefaultAsync(t => t.IdTransacao == id);

                if (transacao == null)
                    return NotFound("Transacao nao encontrada!");
                if (transacao.StatusPgmt != Pendente)
                    return BadRequest($"Não é possível pagar uma transação com status '{transacao.StatusPgmt}'.");

                transacao.MtdPgmt = dto.MtdPgmt;
                transacao.StatusPgmt = Pago;
                transacao.DataPgmt = DateOnly.FromDateTime(DateTime.Now);

                await _context.SaveChangesAsync();
                return Ok(ParaDto(transacao));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

    }
}
