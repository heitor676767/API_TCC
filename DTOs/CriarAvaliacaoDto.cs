using System.ComponentModel.DataAnnotations;

namespace ApiTCC.DTOs
{
    public class CriarAvaliacaoDto
    {
        [Required]
        public int IdPasseio { get; set; }

        [Range(1, 5)]
        public int Nota { get; set; }

        [StringLength(250)]
        public string? Comentario { get; set; }
    }
}
