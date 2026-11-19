
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Data;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;

namespace Sistema_Academia.Repositories
{
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly AcademiaContext _context;

        public MatriculaRepository(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Matricula>> ListarAsync()
        {
            return await _context.Matriculas
                .AsNoTracking()
                .Include(m => m.Aluno)
                .Include(m => m.Plano)
                .Include(m => m.Pagamentos)
                .Include(m => m.MatriculaModalidades)
                    .ThenInclude(mm => mm.Modalidade)
                .AsSplitQuery()
                .OrderBy(m => m.Id)
                .ToListAsync();
        }

        public async Task<Matricula?> ObterPorIdAsync(int id)
        {
            return await _context.Matriculas
                .AsNoTracking()
                .Include(m => m.Aluno)
                .Include(m => m.Plano)
                .Include(m => m.Pagamentos)
                .Include(m => m.MatriculaModalidades)
                    .ThenInclude(mm => mm.Modalidade)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AdicionarAsync(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AtualizarAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds)
        {
            ArgumentNullException.ThrowIfNull(modalidadeIds);

            var existente = await _context.Matriculas
                .Include(m => m.MatriculaModalidades)
                .FirstOrDefaultAsync(m => m.Id == matricula.Id);

            if (existente is null)
                return false;

            // Atualiza os dados principais da matrícula.
            existente.DataInicio = matricula.DataInicio;
            existente.DataFim = matricula.DataFim;
            existente.DataMatricula = matricula.DataMatricula;
            existente.Ativo = matricula.Ativo;
            existente.ValorContratado = matricula.ValorContratado;
            existente.Status = matricula.Status;
            existente.AlunoId = matricula.AlunoId;
            existente.PlanoId = matricula.PlanoId;

            // Identifica as modalidades selecionadas.
            var modalidadesSelecionadas = modalidadeIds.ToHashSet();

            // Identifica os vínculos que já existem.
            var modalidadesAtuais = existente.MatriculaModalidades
                .Select(mm => mm.ModalidadeId)
                .ToHashSet();

            // Remove os vínculos que foram desmarcados.
            var vinculosRemover = existente.MatriculaModalidades
                .Where(mm => !modalidadesSelecionadas.Contains(mm.ModalidadeId))
                .ToList();

            _context.MatriculaModalidades.RemoveRange(vinculosRemover);

            // Adiciona somente os novos vínculos.
            var novasModalidades = modalidadesSelecionadas
                .Except(modalidadesAtuais);

            foreach (var modalidadeId in novasModalidades)
            {
                existente.MatriculaModalidades.Add(
                    new MatriculaModalidade
                    {
                        MatriculaId = existente.Id,
                        ModalidadeId = modalidadeId
                    });
            }

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ExcluirDefinitivamenteAsync(int id)
        {
            var matricula = await _context.Matriculas
                .Include(m => m.Pagamentos)
                .Include(m => m.MatriculaModalidades)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matricula is null)
                return false;

            // Remove pagamentos e vínculos antes da matrícula para respeitar as FKs.
            _context.Pagamentos.RemoveRange(matricula.Pagamentos);
            _context.MatriculaModalidades.RemoveRange(matricula.MatriculaModalidades);
            _context.Matriculas.Remove(matricula);

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
