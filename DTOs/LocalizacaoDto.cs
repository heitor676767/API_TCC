namespace API_TCC.DTOs
{
    public class LocalizacaoDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }
}
