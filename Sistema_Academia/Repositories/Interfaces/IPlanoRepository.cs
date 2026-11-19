
using Sistema_Academia.Models;

namespace Sistema_Academia.Repositories.Interfaces
{
    public interface IPlanoRepository
    {
        Task<List<Plano>> ListarAsync();

        Task<Plano?> ObterPorIdAsync(int id);

        Task AdicionarAsync(Plano plano);

        Task<bool> AtualizarAsync(Plano plano);

        Task<bool> ExisteMatriculasAsync(int planoId);

        Task<bool> DesativarAsync(int id);

        Task<bool> ExcluirDefinitivamenteAsync(int id);
    }
}