

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Sistema_Academia.Models
{
    public class Pagamento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o Valor.")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "O valor deve ser maior que zero.",
        ParseLimitsInInvariantCulture = true)]
        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data do pagamento")]
        public DateTime? DataPagamento { get; set; }

        [Required(ErrorMessage = "Informe a data de vencimento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de vencimento")]
        public DateTime DataVencimento { get; set; }

        [Required(ErrorMessage = "Informe a forma de pagamento.")]
        [StringLength(50, ErrorMessage = "A forma de pagamento deve ter no máximo 50 caracteres.")]
        [Display(Name = "Forma de pagamento")]
        public string FormaPagamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o status do pagamento.")]
        [StringLength(30, ErrorMessage = "O status deve ter no máximo 30 caracteres.")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
        [Display(Name = "Observação")]
        public string? Observacao { get; set; } = string.Empty;

        // Chave estrangeira
        [Required(ErrorMessage = "Selecione uma matrícula.")]
        [Display(Name = "Matrícula")]
        public int MatriculaId { get; set; }

        // Relacionamento: um pagamento pertence a uma matrícula.
        [ValidateNever]
        public Matricula Matricula { get; set; } = null!;
    }
}
