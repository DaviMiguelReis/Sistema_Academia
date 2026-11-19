using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Data;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;

namespace Sistema_Academia.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly AcademiaContext _context;

        public PagamentoRepository(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Pagamento>> ListarAsync()
        {
            return await ConsultaBase()
                .OrderBy(p => p.DataVencimento)
                .ThenBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<Pagamento>> PesquisarAsync(string? termo)
        {
            var consulta = ConsultaBase();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                termo = termo.Trim();

                consulta = consulta.Where(p =>
                    p.Status.Contains(termo) ||
                    p.FormaPagamento.Contains(termo) ||
                    (p.Matricula != null &&
                     p.Matricula.Aluno != null &&
                     p.Matricula.Aluno.Nome.Contains(termo)));
            }

            return await consulta
                .OrderBy(p => p.DataVencimento)
                .ThenBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Pagamento?> ObterPorIdAsync(int id)
        {
            return await ConsultaBase()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AdicionarAsync(Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AtualizarAsync(Pagamento pagamento)
        {
            var existente = await _context.Pagamentos
                .FirstOrDefaultAsync(p => p.Id == pagamento.Id);

            if (existente is null)
                return false;

            existente.Valor = pagamento.Valor;
            existente.DataPagamento = pagamento.DataPagamento;
            existente.DataVencimento = pagamento.DataVencimento;
            existente.FormaPagamento = pagamento.FormaPagamento;
            existente.Observacao = pagamento.Observacao;
            existente.Status = pagamento.Status;
            existente.MatriculaId = pagamento.MatriculaId;

            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<Pagamento> ConsultaBase()
        {
            return _context.Pagamentos
                .AsNoTracking()
                .Include(p => p.Matricula)
                    .ThenInclude(m => m.Aluno);
        }
        public async Task<bool> ExcluirDefinitivamenteAsync(int id)
        {
            var pagamento = await _context.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
            if (pagamento is null) return false;
            if (string.Equals(pagamento.Status, "Pago", StringComparison.OrdinalIgnoreCase)) return false;
            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
