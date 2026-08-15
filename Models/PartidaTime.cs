using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class PartidaTime
    {
        public int PartidaId { get; set; }

        public int TimeId { get; set; }

        public int Gols { get; set; }

        public Partida? Partida { get; set; }

        public Time? Time { get; set; }
    }
}