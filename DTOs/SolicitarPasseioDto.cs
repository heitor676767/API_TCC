using System;

namespace API_TCC.DTOs
{
    // Body enviado pelo Dono ao solicitar um passeio
    public class SolicitarPasseioDto
    {
        public string Rga { get; set; } = string.Empty;        // pet que vai passear
        public string CpfPetwalker { get; set; } = string.Empty;
        public DateTime DataPass { get; set; }
        public int Duracao { get; set; }                        // em minutos

        // Local de encontro/partida do passeio
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }
}
