using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ApiLibertadoresHAS.Models
{
    public class Passeio
    {
        public int IdPasseio { get; set; } // int, chave primária, identity

        public string StatusPass { get; set; }

        public DateTime DataPass { get; set; } // date

        public int Duracao { get; set; }

        public string Rga { get; set; } // char(7), FK -> Pet.Rga

        public string CpfPetwalker { get; set; } // char(11), FK -> PetwalkerPerfil.Cpf

    }
}
