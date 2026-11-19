
namespace Sistema_Academia.Models
{
    public class MatriculaModalidade
    {
        // Chaves estrangeiras que formam a chave primária composta.
        public int MatriculaId { get; set; }

        public int ModalidadeId { get; set; }

        // Propriedades de navegação
        public Matricula Matricula { get; set; } = null!;

        public Modalidade Modalidade { get; set; } = null!;
    }
}
