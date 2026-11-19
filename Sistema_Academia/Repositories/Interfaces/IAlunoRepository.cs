
using Sistema_Academia.Models;

namespace Sistema_Academia.Repositories.Interfaces
{
    public interface IAlunoRepository
    {
        Task<List<Aluno>> ListarAsync();

        Task<Aluno?> ObterPorIdAsync(int id);

        Task AdicionarAsync(Aluno aluno);

        Task<bool> AtualizarAsync(Aluno aluno);

        Task<bool> ExisteCpfAsync(string cpf, int? ignorarAlunoId = null);

        Task<bool> DesativarAsync(int id);

        Task<bool> ExcluirDefinitivamenteAsync(int id);
    }
}
