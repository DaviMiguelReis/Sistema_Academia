
using System.ComponentModel.DataAnnotations;

namespace Sistema_Academia.Models
{
    public class Aluno
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do aluno.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o CPF do aluno.")]
        [StringLength(11, MinimumLength = 11,
            ErrorMessage = "Digite os 11 números do CPF, sem pontos ou traço.")]
        [Display(Name = "CPF")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Informe o e-mail do aluno.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o telefone do aluno.")]
        [RegularExpression(
            @"^(?:[0-9]{10,11}|\([0-9]{2}\)\s?[0-9]{4,5}-[0-9]{4})$",
            ErrorMessage = "Informe um telefone com DDD e 8 ou 9 dígitos.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o Email do aluno.")]
        [StringLength(200,
            ErrorMessage = "O endereço deve ter no máximo 200 caracteres.")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Data de cadastro")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        // Relacionamento: um aluno pode possuir várias matrículas.
        public ICollection<Matricula> Matriculas { get; set; }
            = new List<Matricula>();
    }
}
