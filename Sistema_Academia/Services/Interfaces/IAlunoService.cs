
using Sistema_Academia.Models;

namespace Sistema_Academia.Services.Interfaces
{
    public interface IAlunoService
    {
        Task<List<Aluno>> ListarAsync();

        Task<Aluno?> ObterPorIdAsync(int id);

        Task<List<Aluno>> PesquisarPorNomeAsync(string? nome);

        List<Aluno> Ordenar(
            IEnumerable<Aluno> alunos,
            string? ordenarPor);

        Task AdicionarAsync(Aluno aluno);

        Task<bool> AtualizarAsync(Aluno aluno);

        Task<bool> RemoverAsync(int id);
    }
}
