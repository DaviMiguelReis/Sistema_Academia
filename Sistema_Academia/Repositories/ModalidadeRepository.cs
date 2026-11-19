
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Data;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;

namespace Sistema_Academia.Repositories
{
    public class ModalidadeRepository : IModalidadeRepository
    {
        private readonly AcademiaContext _context;

        public ModalidadeRepository(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Modalidade>> ListarAsync()
        {
            return await _context.Modalidades
                .AsNoTracking()
                .OrderBy(m => m.Nome)
                .ToListAsync();
        }

        public async Task<Modalidade?> ObterPorIdAsync(int id)
        {
            return await _context.Modalidades
                .AsNoTracking()
                .Include(m => m.MatriculaModalidades)
                    .ThenInclude(mm => mm.Matricula)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AdicionarAsync(Modalidade modalidade)
        {
            await _context.Modalidades.AddAsync(modalidade);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AtualizarAsync(Modalidade modalidade)
        {
            var existente = await _context.Modalidades
                .FirstOrDefaultAsync(m => m.Id == modalidade.Id);

            if (existente is null)
                return false;

            existente.Nome = modalidade.Nome;
            existente.Descricao = modalidade.Descricao;
            existente.Professor = modalidade.Professor;
            existente.Ativo = modalidade.Ativo;
            existente.CapacidadeMaxima = modalidade.CapacidadeMaxima;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteMatriculasAsync(int modalidadeId)
        {
            return await _context.MatriculaModalidades
                .AsNoTracking()
                .AnyAsync(mm => mm.ModalidadeId == modalidadeId);
        }

        public async Task<bool> DesativarAsync(int id)
        {
            var modalidade = await _context.Modalidades
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modalidade is null)
                return false;

            modalidade.Ativo = false;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ExcluirDefinitivamenteAsync(int id)
        {
            var modalidade = await _context.Modalidades.FirstOrDefaultAsync(m => m.Id == id);
            if (modalidade is null) return false;
            if (await _context.MatriculaModalidades.AnyAsync(mm => mm.ModalidadeId == id)) return false;
            _context.Modalidades.Remove(modalidade);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
