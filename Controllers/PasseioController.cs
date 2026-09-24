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
    public class PasseioController : ControllerBase
    {
        private readonly DataContext _context;

        public PasseioController(DataContext context)
        {
            _context = context;
        }

        private string? CpfLogado => User.FindFirstValue("Cpf");

        private IQueryable<Passeio> QueryComIncludes()
        {
            return _context.TB_PASSEIOS
                .Include(p => p.Pet).ThenInclude(pet => pet.Dono)
                .Include(p => p.PetwalkerPerfil).ThenInclude(pw => pw.Usuario)
                .Include(p => p.LocalizacaoPasseio);
        }

        private static PasseioDto ParaDto(Passeio p)
        {
            return new PasseioDto
            {
                IdPasseio = p.IdPasseio,
                StatusPass = p.StatusPass,
                DataPass = p.DataPass,
                Duracao = p.Duracao,
                PetRga = p.Pet.Rga,
                PetNome = p.Pet.Nome,
                DonoCpf = p.Pet.CpfDono,
                DonoNome = p.Pet.Dono?.Nome ?? string.Empty,
                PetwalkerCpf = p.CpfPetwalker,
                PetwalkerNome = p.PetwalkerPerfil?.Usuario?.Nome ?? string.Empty,
                Localizacao = p.LocalizacaoPasseio == null ? null : new LocalizacaoDto
                {
                    Latitude = p.LocalizacaoPasseio.Latitude,
                    Longitude = p.LocalizacaoPasseio.Longitude,
                    Cep = p.LocalizacaoPasseio.Cep,
                    Numero = p.LocalizacaoPasseio.Numero
                }
            };
        }

        // POST /Passeio/Solicitar
        // Só um Dono pode solicitar, e só pra um pet que realmente seja dele
        // (não confiamos em nada que o app mande além do que está no token + no banco).
        [Authorize(Roles = "Dono")]
        [HttpPost("Solicitar")]
        public async Task<IActionResult> Solicitar(SolicitarPasseioDto dto)
        {
            try
            {
                if (CpfLogado == null)
                    return Unauthorized();

                Pet? pet = await _context.TB_PETS.FirstOrDefaultAsync(x => x.Rga == dto.Rga);
                if (pet == null)
                    return NotFound("Pet não encontrado.");

                if (pet.CpfDono != CpfLogado)
                    return Forbid("Esse pet não pertence ao usuário logado.");

                PetwalkerPerfil? petwalker = await _context.TB_PETWALKER_PERFIL
                    .FirstOrDefaultAsync(x => x.Cpf == dto.CpfPetwalker);
                if (petwalker == null)
                    return NotFound("Petwalker não encontrado.");

                if (!petwalker.Disponibilidade)
                    return BadRequest("Esse petwalker não está disponível no momento.");

                var passeio = new Passeio
                {
                    StatusPass = StatusPasseio.Solicitado,
                    DataPass = dto.DataPass,
                    Duracao = dto.Duracao,
                    Rga = dto.Rga,
                    CpfPetwalker = dto.CpfPetwalker,
                    LocalizacaoPasseio = new LocalizacaoPasseio
                    {
                        Latitude = dto.Latitude,
                        Longitude = dto.Longitude,
                        Cep = dto.Cep,
                        Numero = dto.Numero
                    }
                };

                await _context.TB_PASSEIOS.AddAsync(passeio);
                await _context.SaveChangesAsync();

                Passeio criado = await QueryComIncludes().FirstAsync(x => x.IdPasseio == passeio.IdPasseio);
                return Ok(ParaDto(criado));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // Retorna passeios diferentes dependendo do papel: Dono vê os que pediu,
        // Petwalker vê os que aceitou/foi designado. Quem é "Ambos" vê os dois conjuntos.
        [HttpGet("Meus")]
        public async Task<IActionResult> Meus()
        {
            try
            {
                if (CpfLogado == null)
                    return Unauthorized();

                List<Passeio> passeios = await QueryComIncludes()
                    .Where(p => p.Pet.CpfDono == CpfLogado || p.CpfPetwalker == CpfLogado)
                    .OrderByDescending(p => p.DataPass)
                    .ToListAsync();

                return Ok(passeios.Select(ParaDto));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // Só o dono do pet ou o petwalker daquele passeio específico podem ver o detalhe.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                if (CpfLogado == null)
                    return Unauthorized();

                Passeio? passeio = await QueryComIncludes().FirstOrDefaultAsync(p => p.IdPasseio == id);
                if (passeio == null)
                    return NotFound("Passeio não encontrado.");

                if (passeio.Pet.CpfDono != CpfLogado && passeio.CpfPetwalker != CpfLogado)
                    return Forbid();

                return Ok(ParaDto(passeio));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // PUT /Passeio/{id}/Aceitar
        [Authorize(Roles = "Petwalker")]
        [HttpPut("{id}/Aceitar")]
        public async Task<IActionResult> Aceitar(int id)
        {
            Passeio? passeio = await _context.TB_PASSEIOS.FirstOrDefaultAsync(p => p.IdPasseio == id);
            if (passeio == null) return NotFound("Passeio não encontrado.");
            if (passeio.CpfPetwalker != CpfLogado) return Forbid();
            if (passeio.StatusPass != StatusPasseio.Solicitado)
                return BadRequest($"Não é possível aceitar um passeio no status '{passeio.StatusPass}'.");

            passeio.StatusPass = StatusPasseio.Aceito;
            await _context.SaveChangesAsync();
            return Ok(passeio.StatusPass);
        }

        // PUT /Passeio/{id}/Recusar
        [Authorize(Roles = "Petwalker")]
        [HttpPut("{id}/Recusar")]
        public async Task<IActionResult> Recusar(int id)
        {
            Passeio? passeio = await _context.TB_PASSEIOS.FirstOrDefaultAsync(p => p.IdPasseio == id);
            if (passeio == null) return NotFound("Passeio não encontrado.");
            if (passeio.CpfPetwalker != CpfLogado) return Forbid();
            if (passeio.StatusPass != StatusPasseio.Solicitado)
                return BadRequest($"Não é possível recusar um passeio no status '{passeio.StatusPass}'.");

            passeio.StatusPass = StatusPasseio.Recusado;
            await _context.SaveChangesAsync();
            return Ok(passeio.StatusPass);
        }

        // PUT /Passeio/{id}/Iniciar
        [Authorize(Roles = "Petwalker")]
        [HttpPut("{id}/Iniciar")]
        public async Task<IActionResult> Iniciar(int id)
        {
            Passeio? passeio = await _context.TB_PASSEIOS.FirstOrDefaultAsync(p => p.IdPasseio == id);
            if (passeio == null) return NotFound("Passeio não encontrado.");
            if (passeio.CpfPetwalker != CpfLogado) return Forbid();
            if (passeio.StatusPass != StatusPasseio.Aceito)
                return BadRequest($"Não é possível iniciar um passeio no status '{passeio.StatusPass}'.");

            passeio.StatusPass = StatusPasseio.EmAndamento;
            await _context.SaveChangesAsync();
            return Ok(passeio.StatusPass);
        }

        // PUT /Passeio/{id}/Finalizar
        // Ao finalizar, já cria a Transacao pendente de pagamento referente ao passeio.
        [Authorize(Roles = "Petwalker")]
        [HttpPut("{id}/Finalizar")]
        public async Task<IActionResult> Finalizar(int id, [FromQuery] decimal valor)
        {
            try
            {
                Passeio? passeio = await _context.TB_PASSEIOS.FirstOrDefaultAsync(p => p.IdPasseio == id);
                if (passeio == null) return NotFound("Passeio não encontrado.");
                if (passeio.CpfPetwalker != CpfLogado) return Forbid();
                if (passeio.StatusPass != StatusPasseio.EmAndamento)
                    return BadRequest($"Não é possível finalizar um passeio no status '{passeio.StatusPass}'.");

                passeio.StatusPass = StatusPasseio.Finalizado;

                await _context.TB_TRANSACOES.AddAsync(new Transacao
                {
                    MtdPgmt = "Nao definido",
                    StatusPgmt = "Pendente",
                    Valor = valor,
                    DataPgmt = DateOnly.FromDateTime(DateTime.Now),
                    IdPasseio = passeio.IdPasseio
                });

                await _context.SaveChangesAsync();
                return Ok(passeio.StatusPass);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }

        // PUT /Passeio/{id}/Cancelar
        // Só o Dono que pediu pode cancelar, e só antes do passeio começar.
        [Authorize(Roles = "Dono")]
        [HttpPut("{id}/Cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            Passeio? passeio = await _context.TB_PASSEIOS
                .Include(p => p.Pet)
                .FirstOrDefaultAsync(p => p.IdPasseio == id);

            if (passeio == null) return NotFound("Passeio não encontrado.");
            if (passeio.Pet.CpfDono != CpfLogado) return Forbid();
            if (passeio.StatusPass != StatusPasseio.Solicitado && passeio.StatusPass != StatusPasseio.Aceito)
                return BadRequest($"Não é possível cancelar um passeio no status '{passeio.StatusPass}'.");

            passeio.StatusPass = StatusPasseio.Cancelado;
            await _context.SaveChangesAsync();
            return Ok(passeio.StatusPass);
        }
    }
}
