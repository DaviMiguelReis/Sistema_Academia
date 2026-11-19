
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Data;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;

namespace Sistema_Academia.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly AcademiaContext _context;

        public AlunoRepository(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Aluno>> ListarAsync()
        {
            return await _context.Alunos
                .AsNoTracking()
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<Aluno?> ObterPorIdAsync(int id)
        {
            return await _context.Alunos
                .AsNoTracking()
                .Include(a => a.Matriculas)
                    .ThenInclude(m => m.Plano)
                .Include(a => a.Matriculas)
                    .ThenInclude(m => m.Pagamentos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AdicionarAsync(Aluno aluno)
        {
            await _context.Alunos.AddAsync(aluno);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AtualizarAsync(Aluno aluno)
        {
            var existente = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == aluno.Id);

            if (existente is null)
                return false;

            existente.Nome = aluno.Nome;
            existente.CPF = aluno.CPF;
            existente.Email = aluno.Email;
            existente.Telefone = aluno.Telefone;
            existente.Endereco = aluno.Endereco;
            existente.DataNascimento = aluno.DataNascimento;
            //existente.DataCadastro = aluno.DataCadastro;
            existente.Ativo = aluno.Ativo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteCpfAsync(
            string cpf,
            int? ignorarAlunoId = null)
        {
            return await _context.Alunos
                .AsNoTracking()
                .AnyAsync(a =>
                    a.CPF == cpf &&
                    (!ignorarAlunoId.HasValue ||
                     a.Id != ignorarAlunoId.Value));
        }

        public async Task<bool> DesativarAsync(int id)
        {
            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno is null)
                return false;

            aluno.Ativo = false;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ExcluirDefinitivamenteAsync(int id)
        {
            var aluno = await _context.Alunos
                .Include(a => a.Matriculas)
                    .ThenInclude(m => m.Pagamentos)
                .Include(a => a.Matriculas)
                    .ThenInclude(m => m.MatriculaModalidades)
                .AsSplitQuery()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno is null)
                return false;

            // Exclui primeiro os registros dependentes para respeitar as FKs
            // configuradas com DeleteBehavior.Restrict.
            var matriculas = aluno.Matriculas.ToList();
            var pagamentos = matriculas.SelectMany(m => m.Pagamentos).ToList();
            var vinculosModalidades = matriculas.SelectMany(m => m.MatriculaModalidades).ToList();

            _context.Pagamentos.RemoveRange(pagamentos);
            _context.MatriculaModalidades.RemoveRange(vinculosModalidades);
            _context.Matriculas.RemoveRange(matriculas);
            _context.Alunos.Remove(aluno);

            // Um único SaveChanges mantém a exclusão consistente em transação.
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
