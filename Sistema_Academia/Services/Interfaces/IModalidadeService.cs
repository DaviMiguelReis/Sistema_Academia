using Sistema_Academia.Models;

namespace Sistema_Academia.Services.Interfaces
{
    public interface IModalidadeService
    {
        Task<List<Modalidade>> ListarAsync();
        Task<Modalidade?> ObterPorIdAsync(int id);
        Task<List<Modalidade>> PesquisarAsync(string? termo);
        List<Modalidade> Ordenar(
            IEnumerable<Modalidade> modalidades,
            string? ordenarPor);
        Task AdicionarAsync(Modalidade modalidade);
        Task<bool> AtualizarAsync(Modalidade modalidade);
        Task<bool> RemoverAsync(int id);
    }
}