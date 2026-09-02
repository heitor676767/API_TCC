using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTCC.Models;

namespace API_TCC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetController
    {
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