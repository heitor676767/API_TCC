using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTCC.Models
{
    public class Usuario
    {
        public string Cpf { get; set; }

        public string Nome { get; set; }

        public string Cep { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string TipoUsuario { get; set; }

        public string? StatusUser { get; set; }

        public string Telefone { get; set; }

        public string Genero { get; set; } = "Nao informado";

        public string? Foto { get; set; }

        public DateTime UltimoLogin { get; set; } = DateTime.Now;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Navegação 1:1 (pode ser null se o usuário for só Dono)
        public PetwalkerPerfil? PetwalkerPerfil { get; set; }

        // Navegação 1:N (pets que esse usuário é dono)
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
