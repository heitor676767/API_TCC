using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLibertadoresHAS.Models
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

        public enum Porte { Grande, Medio, Pequeno }

        public enum Sexo { Macho, Femea }

        public string CpfDono { get; set; } = string.Empty; 

    }
}
