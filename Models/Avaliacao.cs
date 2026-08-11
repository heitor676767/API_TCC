using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ApiLibertadoresHAS.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }

        public string? Comentario { get; set; } // opcional (aceita null)

        public int Nota { get; set; }

        public DateTime DataPublicacao { get; set; } = DateTime.Now; // default getdate()

        public string Rga { get; set; } = string.Empty; // char(7), FK -> Pet.Rga

        public string CpfPetwalker { get; set; } = string.Empty; // char(11), FK -> PetwalkerPerfil.Cpf
        //Navegacao
        public Pet Pet { get; set; } = null!;
        public PetwalkerPerfil PetwalkerPerfil { get; set; } = null!;


    }
}
