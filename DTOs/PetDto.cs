using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiTCC.Models.Enums;

namespace API_TCC.DTOs
{
    public class PetDto
{
    [Required]
    public string Rga { get; set; } = string.Empty;

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Especie { get; set; } = string.Empty;

    [Required]
    public string Raca { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
    public int Peso { get; set; }
    public Porte Porte { get; set; }
    public Sexo Sexo { get; set; }

    [Required]
    public string CpfDono { get; set; } = string.Empty;
}

}