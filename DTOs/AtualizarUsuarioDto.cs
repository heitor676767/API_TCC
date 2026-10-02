using System.ComponentModel.DataAnnotations;

namespace ApiTCC.DTOs
{
    public class AtualizarUsuarioDto
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cep é obrigatório.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Cep deve ter exatamente 8 caracteres.")]
        public string Cep { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Genero { get; set; }

        [StringLength(100)]
        public string? Foto { get; set; }
    }
}
