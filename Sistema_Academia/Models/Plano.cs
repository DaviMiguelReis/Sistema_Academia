
using System.ComponentModel.DataAnnotations;

namespace Sistema_Academia.Models
{
    public class Plano
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do plano.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o valor do plano.")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "O valor deve ser maior que zero.",
        ParseLimitsInInvariantCulture = true)]
        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Informe a duração do plano.")]
        [Range(1, 120, ErrorMessage = "A duração deve ser entre 1 e 120 meses.")]
        [Display(Name = "Duração (meses)")]
        public int DuracaoMeses { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        // Relacionamento: um plano pode estar associado a várias matrículas.
        public ICollection<Matricula> Matriculas { get; set; }
            = new List<Matricula>();
    }
}
