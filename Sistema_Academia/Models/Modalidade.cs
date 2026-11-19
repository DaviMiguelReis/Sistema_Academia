
using System.ComponentModel.DataAnnotations;

namespace Sistema_Academia.Models
{
    public class Modalidade
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome da modalidade.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome do professor.")]
        [StringLength(150, ErrorMessage = "O nome do professor deve ter no máximo 150 caracteres.")]
        [Display(Name = "Professor")]
        public string Professor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a capacidade máxima.")]
        [Range(1, 1000, ErrorMessage = "A capacidade máxima deve ser maior que zero.")]
        [Display(Name = "Capacidade máxima")]
        public int CapacidadeMaxima { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        // Relacionamento: uma modalidade pode estar associada a várias matrículas.
        public ICollection<MatriculaModalidade> MatriculaModalidades { get; set; }
            = new List<MatriculaModalidade>();
    }
}
