using ApiTCC.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Rga { get; set; } = string.Empty; 
        public string Descricao { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Especie { get; set; } = string.Empty;

        public string? Foto { get; set; } 

        public string Raca { get; set; } = string.Empty;

        public int Peso { get; set; }

        public Porte Porte { get; set; }

        public Sexo Sexo { get; set; }

        public string CpfDono { get; set; } = string.Empty;

        public Usuario Dono { get; set; } = null!;
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
        public ICollection<Passeio> Passeios { get; set; } = new List<Passeio>();

    }
}
