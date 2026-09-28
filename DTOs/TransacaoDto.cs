namespace ApiTCC.DTOs
{
    public class TransacaoDto
    {
        public int IdTransacao { get; set; }
        public int IdPasseio { get; set; }
        public string MtdPgmt { get; set; } = string.Empty;
        public string StatusPgmt { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateOnly DataPgmt { get; set; }
    }
}
