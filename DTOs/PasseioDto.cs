using System;

namespace API_TCC.DTOs
{
    public class PasseioDto
    {
        public int IdPasseio { get; set; }
        public string StatusPass { get; set; } = string.Empty;
        public DateTime DataPass { get; set; }
        public int Duracao { get; set; }

        public string PetRga { get; set; } = string.Empty;
        public string PetNome { get; set; } = string.Empty;

        public string DonoCpf { get; set; } = string.Empty;
        public string DonoNome { get; set; } = string.Empty;

        public string PetwalkerCpf { get; set; } = string.Empty;
        public string PetwalkerNome { get; set; } = string.Empty;

        public LocalizacaoDto? Localizacao { get; set; }
    }
}
