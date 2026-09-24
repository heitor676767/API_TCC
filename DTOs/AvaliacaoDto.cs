using System;

namespace API_TCC.DTOs
{
    public class AvaliacaoDto
    {
        public int Id { get; set; }
        public string? Comentario { get; set; }
        public int Nota { get; set; }
        public DateTime DataPublicacao { get; set; }
    }
}
