using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Rodada
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public ICollection<Partida> Partidas { get; set; } = new List<Partida>();
    }
}