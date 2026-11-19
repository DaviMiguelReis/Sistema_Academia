
using Sistema_Academia.Models;

namespace Sistema_Academia.Repositories.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<List<Matricula>> ListarAsync();

        Task<Matricula?> ObterPorIdAsync(int id);

        Task AdicionarAsync(Matricula matricula);

        Task<bool> AtualizarAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds);

        Task<bool> ExcluirDefinitivamenteAsync(int id);
    }
}
