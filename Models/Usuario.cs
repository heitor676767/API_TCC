using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLibertadoresHAS.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string senha { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string? StatusUser { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public string Genero { get; set; } = "Nao informado";
        public string? Foto { get; set; }
        public DateTime UltimoLogin { get; set; } = DateTime.Now;
        public DateTime DataCadastro { get; set; } = DateTime.Now;


    }
}
