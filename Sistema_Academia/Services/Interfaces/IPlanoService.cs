using Sistema_Academia.Models;

namespace Sistema_Academia.Services.Interfaces
{
    public interface IPlanoService
    {
        Task<List<Plano>> ListarAsync();
        Task<Plano?> ObterPorIdAsync(int id);
        Task<List<Plano>> PesquisarPorNomeAsync(string? nome);
        List<Plano> Ordenar(IEnumerable<Plano> planos, string? ordenarPor);
        Task AdicionarAsync(Plano plano);
        Task<bool> AtualizarAsync(Plano plano);
        Task<bool> RemoverAsync(int id);
    }
}