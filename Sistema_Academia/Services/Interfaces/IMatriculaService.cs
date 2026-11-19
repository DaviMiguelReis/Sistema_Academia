
using Sistema_Academia.Models;

namespace Sistema_Academia.Services.Interfaces
{
    public interface IMatriculaService
    {
        Task<List<Matricula>> ListarAsync();

        Task<Matricula?> ObterPorIdAsync(int id);

        Task<List<Matricula>> PesquisarAsync(string? termo);

        List<Matricula> Ordenar(
            IEnumerable<Matricula> matriculas,
            string? ordenarPor);

        Task AdicionarAsync(Matricula matricula);

        Task AdicionarAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds);

        Task<bool> AtualizarAsync(Matricula matricula);

        Task<bool> AtualizarAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds);

        Task<bool> RemoverAsync(int id);
    }
}
