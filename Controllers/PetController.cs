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

namespace API_TCC.Controllers
{
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

        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarPet(Pet pet)
        {
            try
            {
                if (await PetExistente(pet.Rga))
                    throw new System.Exception("Animal ja cadastrao");
                await _context.TB_PETS.AddAsync(pet);
                await _context.SaveChangesAsync();

                return Ok(pet.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message + " _ " + ex.InnerException);
            }
        }
    }
}