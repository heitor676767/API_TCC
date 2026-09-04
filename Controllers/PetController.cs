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
using Microsoft.AspNetCore.Authorization;
using API_TCC.DTOs;

namespace API_TCC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PetController : ControllerBase
    {
        private readonly DataContext _context;
        public readonly IConfiguration _configuration;

        public PetController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        private async Task<bool> PetExistente(string rga)
        {
            if (await _context.TB_PETS.AnyAsync(x => x.Rga.ToLower() == rga.ToLower()))
            {
                return true;
            }
            return false;
        }
        [AllowAnonymous]
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarPet(PetDto dto)
        {
            try
            {
                if (await PetExistente(dto.Rga))
                    throw new System.Exception("Animal ja cadastrado");

                var novoPet = new Pet
                {
                    Rga = dto.Rga,
                    Nome = dto.Nome,
                    Especie = dto.Especie,
                    Raca = dto.Raca,
                    Descricao = dto.Descricao,
                    Peso = dto.Peso,
                    Porte = dto.Porte,
                    Sexo = dto.Sexo,
                    CpfDono = dto.CpfDono,
                };
                await _context.TB_PETS.AddAsync(novoPet);
                await _context.SaveChangesAsync();

                return Ok(novoPet.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }
        
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetPets()
        {
            try
            {
                List<Pet> pets = await _context.TB_PETS.ToListAsync();
                return Ok(pets);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }

        }
    }
}