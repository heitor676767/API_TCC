using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Jogador
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int Numero { get; set; }

        public DateTime DataNascimento { get; set; }

        public int TimeId { get; set; }

        public int PosicaoId { get; set; }

        public Time? Time { get; set; }

        public Posicao? Posicao { get; set; }
    }
}