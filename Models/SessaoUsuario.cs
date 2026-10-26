namespace ApiTCC.Models
{
    public class SessaoUsuario
    {
        public Guid Id { get; set; }
        public string CpfUsuario { get; set; } = default!;
        public string RefreshTokenHash { get; set; } = default!;
        public string NomeDispositivo { get; set; } = default!;
        public string? EnderecoIp { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimoAcesso { get; set; }
        public DateTime DataExpiracao { get; set; }
        public DateTime? DataRevogacao { get; set; }

        public Usuario Usuario { get; set; } = default!;
    }
}
