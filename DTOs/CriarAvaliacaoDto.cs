using System.ComponentModel.DataAnnotations;

namespace ApiTCC.DTOs
{
    public class CriarAvaliacaoDto
    {
        [Required]
        public string Rga { get; set; } = string.Empty;

        [Required]
        public string CpfPetwalker { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Nota { get; set; }

        public string? Comentario { get; set; }
    }
}
