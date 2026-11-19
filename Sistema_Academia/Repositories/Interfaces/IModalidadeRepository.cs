
using Sistema_Academia.Models;

namespace Sistema_Academia.Repositories.Interfaces
{
    public interface IModalidadeRepository
    {
        Task<List<Modalidade>> ListarAsync();

        Task<Modalidade?> ObterPorIdAsync(int id);

        Task AdicionarAsync(Modalidade modalidade);

        Task<bool> AtualizarAsync(Modalidade modalidade);

        Task<bool> ExisteMatriculasAsync(int modalidadeId);

        Task<bool> DesativarAsync(int id);

        Task<bool> ExcluirDefinitivamenteAsync(int id);
    }
}