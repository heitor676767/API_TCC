using System.Collections.Generic;

namespace API_TCC.DTOs
{
    public class PetwalkerDetalheDto : PetwalkerDto
    {
        public List<AvaliacaoDto> Avaliacoes { get; set; } = new List<AvaliacaoDto>();
    }
}
