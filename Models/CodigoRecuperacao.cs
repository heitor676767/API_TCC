namespace ApiTCC.Models
{
    public class CodigoRecuperacao
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime DataExpiracao { get; set; }
        public bool Usado { get; set; } = false;
    }
}
