using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Posicao
    {
         public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public ICollection<Jogador> Jogadores { get; set; } = new List<Jogador>();
    }
}