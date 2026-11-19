using Sistema_Academia.Models;

namespace Sistema_Academia.Services.Interfaces
{
    public interface IPagamentoService
    {
        Task<List<Pagamento>> ListarAsync();
        Task<List<Pagamento>> PesquisarAsync(string? termo);
        Task<Pagamento?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Pagamento pagamento);
        Task<bool> AtualizarAsync(Pagamento pagamento);
        Task<bool> RemoverAsync(int id);
    }
}
