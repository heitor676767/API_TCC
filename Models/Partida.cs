using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Partida
    {
         public int Id { get; set; }

    public DateTime DataHora { get; set; }

    //public string Rodada { get; set; } = string.Empty;

    public int EstadioId { get; set; }

    public Estadio? Estadio { get; set; }

    public ICollection<PartidaTime> PartidaTimes { get; set; } = new List<PartidaTime>();
    public int RodadaId { get; set; }
    public Rodada? Rodada { get; set; }
    }
}