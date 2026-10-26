using ApiTCC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ApiTCC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SessoesController : ControllerBase
    {
        private readonly DataContext _context;

        public SessoesController(DataContext context) => _context = context;

        private string? Cpf => User.FindFirstValue("Cpf");

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            string? sidAtual = User.FindFirstValue("sid");

            var sessoes = await _context.TB_SESSOES_USUARIO
                .Where(s => s.CpfUsuario == Cpf
                         && s.DataRevogacao == null
                         && s.DataExpiracao > DateTime.UtcNow)
                .OrderByDescending(s => s.UltimoAcesso)
                .Select(s => new
                {
                    s.Id,
                    s.NomeDispositivo,
                    s.UltimoAcesso,
                    Atual = s.Id.ToString() == sidAtual
                })
                .ToListAsync();

            return Ok(sessoes);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Revogar(Guid id)
        {
            var sessao = await _context.TB_SESSOES_USUARIO
                .FirstOrDefaultAsync(s => s.Id == id && s.CpfUsuario == Cpf);

            if (sessao is null) return NotFound();

            sessao.DataRevogacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> RevogarTodas()
        {
            await _context.TB_SESSOES_USUARIO
                .Where(s => s.CpfUsuario == Cpf && s.DataRevogacao == null)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.DataRevogacao, DateTime.UtcNow));

            return NoContent();
        }
    }

}

