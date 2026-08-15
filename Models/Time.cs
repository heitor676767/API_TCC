using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Time
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Pais { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public int AnoFundacao { get; set; }

        public string? Escudo { get; set; }
        public int? TitulosLibertadores { get; set; }

        public ICollection<Jogador> Jogadores { get; set; } = new List<Jogador>();

        public ICollection<PartidaTime> PartidaTimes { get; set; } = new List<PartidaTime>();
    }
}