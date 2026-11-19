
using System.ComponentModel.DataAnnotations;

namespace Sistema_Academia.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a data da matrícula.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data da matrícula")]
        public DateTime DataMatricula { get; set; }

        [Required(ErrorMessage = "Informe a data de início.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de início")]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de término")]
        public DateTime? DataFim { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }


        [Required(ErrorMessage = "Informe o Valor Contratado.")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "O valor deve ser maior que zero.",
        ParseLimitsInInvariantCulture = true)]
        [Display(Name = "ValorContratado")]
        [DataType(DataType.Currency)]
        public decimal ValorContratado { get; set; }

        [Required(ErrorMessage = "Informe o status da matrícula.")]
        [StringLength(30, ErrorMessage = "O status deve ter no máximo 30 caracteres.")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        // Chaves estrangeiras
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um aluno.")]
        [Display(Name = "Aluno")]
        public int AlunoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione um plano.")]
        [Display(Name = "Plano")]
        public int PlanoId { get; set; }

        // Propriedades de navegação
        public Aluno? Aluno { get; set; }

        public Plano? Plano { get; set; }

        // Relacionamento: uma matrícula pode possuir várias modalidades.
        public ICollection<MatriculaModalidade> MatriculaModalidades { get; set; }
            = new List<MatriculaModalidade>();

        // Relacionamento: uma matrícula pode possuir vários pagamentos.
        public ICollection<Pagamento> Pagamentos { get; set; }
            = new List<Pagamento>();
    }
}
