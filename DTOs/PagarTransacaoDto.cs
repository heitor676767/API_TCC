using System.ComponentModel.DataAnnotations;

namespace ApiTCC.DTOs
{
    public class PagarTransacaoDto
    {
        [Required(ErrorMessage = "Informe o método de pagamento.")]
        [StringLength(50)]
        public string MtdPgmt { get; set; } = string.Empty; // ex: "Pix", "Cartao"
    }
}
