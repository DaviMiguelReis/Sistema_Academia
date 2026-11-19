using Sistema_Academia.Models;

namespace Sistema_Academia.Repositories.Interfaces
{
    public interface IPagamentoRepository
    {
        Task<List<Pagamento>> ListarAsync();
        Task<List<Pagamento>> PesquisarAsync(string? termo);
        Task<Pagamento?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Pagamento pagamento);
        Task<bool> AtualizarAsync(Pagamento pagamento);
        Task<bool> ExcluirDefinitivamenteAsync(int id);
    }
}
