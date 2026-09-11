using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TCC.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Email { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public string? StatusUser { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string? Foto { get; set; }
        public DateTime UltimoLogin { get; set; }
        public DateTime DataCadastro { get; set; }
        public List<PetDto> Pets { get; set; } = new List<PetDto>();

    }
}