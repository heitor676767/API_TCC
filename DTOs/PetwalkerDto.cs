namespace API_TCC.DTOs
{
    // Dados de um petwalker para exibir no mapa / lista de resultados do app
    public class PetwalkerDto
    {
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Foto { get; set; }
        public bool Disponibilidade { get; set; }
        public string AreaAtendimento { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public double? DistanciaKm { get; set; }

        // Média das avaliações (0 se ainda não tiver nenhuma)
        public double NotaMedia { get; set; }
        public int QuantidadeAvaliacoes { get; set; }
    }
}
