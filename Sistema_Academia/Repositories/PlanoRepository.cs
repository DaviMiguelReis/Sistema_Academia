
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Data;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;

namespace Sistema_Academia.Repositories
{
    public class PlanoRepository : IPlanoRepository
    {
        private readonly AcademiaContext _context;

        public PlanoRepository(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Plano>> ListarAsync()
        {
            return await _context.Planos
                .AsNoTracking()
                .Include(p => p.Matriculas)
                .AsSplitQuery()
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Plano?> ObterPorIdAsync(int id)
        {
            return await _context.Planos
                .AsNoTracking()
                .Include(p => p.Matriculas)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AdicionarAsync(Plano plano)
        {
            await _context.Planos.AddAsync(plano);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AtualizarAsync(Plano plano)
        {
            var existente = await _context.Planos
                .FirstOrDefaultAsync(p => p.Id == plano.Id);

            if (existente is null)
                return false;

            existente.Nome = plano.Nome;
            existente.Descricao = plano.Descricao;
            existente.Valor = plano.Valor;
            existente.DuracaoMeses = plano.DuracaoMeses;
            existente.Ativo = plano.Ativo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteMatriculasAsync(int planoId)
        {
            return await _context.Matriculas
                .AsNoTracking()
                .AnyAsync(m => m.PlanoId == planoId);
        }

        public async Task<bool> DesativarAsync(int id)
        {
            var plano = await _context.Planos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plano is null)
                return false;

            plano.Ativo = false;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ExcluirDefinitivamenteAsync(int id)
        {
            var plano = await _context.Planos.FirstOrDefaultAsync(p => p.Id == id);
            if (plano is null) return false;
            if (await _context.Matriculas.AnyAsync(m => m.PlanoId == id)) return false;
            _context.Planos.Remove(plano);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
