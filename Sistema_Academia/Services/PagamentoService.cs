using System.ComponentModel.DataAnnotations;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Services
{
    public class PagamentoService : IPagamentoService
    {
        private static readonly string[] StatusPermitidos =
        {
            "Pendente", "Pago", "Atrasado"
        };

        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMatriculaRepository _matriculaRepository;

        public PagamentoService(
            IPagamentoRepository pagamentoRepository,
            IMatriculaRepository matriculaRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _matriculaRepository = matriculaRepository;
        }

        public Task<List<Pagamento>> ListarAsync()
        {
            return _pagamentoRepository.ListarAsync();
        }

        public Task<List<Pagamento>> PesquisarAsync(string? termo)
        {
            return _pagamentoRepository.PesquisarAsync(termo);
        }

        public async Task<Pagamento?> ObterPorIdAsync(int id)
        {
            ValidarIdentificador(id);
            return await _pagamentoRepository.ObterPorIdAsync(id);
        }

        public async Task AdicionarAsync(Pagamento pagamento)
        {
            ArgumentNullException.ThrowIfNull(pagamento);

            PrepararDados(pagamento);
            ValidarPagamento(pagamento);
            await ValidarMatriculaAsync(pagamento.MatriculaId);

            await _pagamentoRepository.AdicionarAsync(pagamento);
        }

        public async Task<bool> AtualizarAsync(Pagamento pagamento)
        {
            ArgumentNullException.ThrowIfNull(pagamento);
            ValidarIdentificador(pagamento.Id);

            var existente = await _pagamentoRepository.ObterPorIdAsync(pagamento.Id);

            if (existente is null)
                return false;

            if (existente.Status.Equals(
                "Cancelado", StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Não é possível alterar um pagamento cancelado.");
            }

            PrepararDados(pagamento);
            ValidarPagamento(pagamento);
            await ValidarMatriculaAsync(pagamento.MatriculaId);

            return await _pagamentoRepository.AtualizarAsync(pagamento);
        }

        public async Task<bool> RemoverAsync(int id)
        {
            ValidarIdentificador(id);
            var pagamento = await _pagamentoRepository.ObterPorIdAsync(id);
            if (pagamento is null) return false;
            if (pagamento.Status.Equals("Pago", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Pagamentos já realizados não podem ser excluídos para preservar o histórico financeiro.");
            return await _pagamentoRepository.ExcluirDefinitivamenteAsync(id);
        }

        private static void ValidarIdentificador(int id)
        {
            if (id <= 0)
                throw new ValidationException(
                    "Identificação do pagamento inválida.");
        }

        private static void PrepararDados(Pagamento pagamento)
        {
            pagamento.FormaPagamento =
                pagamento.FormaPagamento?.Trim() ?? string.Empty;

            pagamento.Status =
                pagamento.Status?.Trim() ?? string.Empty;

            pagamento.Observacao =
                pagamento.Observacao?.Trim() ?? string.Empty;
        }

        private static void ValidarPagamento(Pagamento pagamento)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(pagamento);

            if (!Validator.TryValidateObject(
                pagamento, contexto, resultados, validateAllProperties: true))
            {
                var erros = string.Join(
                    " ", resultados.Select(r => r.ErrorMessage));

                throw new ValidationException(erros);
            }

            if (pagamento.Valor <= 0)
                throw new ValidationException(
                    "O valor do pagamento deve ser maior que zero.");

            if (pagamento.DataVencimento == default)
                throw new ValidationException(
                    "Informe uma data de vencimento válida.");

            if (pagamento.DataPagamento.HasValue)
            {
                if (pagamento.DataPagamento.Value == default)
                    throw new ValidationException(
                        "Informe uma data de pagamento válida.");

                if (pagamento.DataPagamento.Value.Date > DateTime.Today)
                    throw new ValidationException(
                        "A data de pagamento não pode ser futura.");
            }

            if (!StatusPermitidos.Contains(
                pagamento.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Status inválido. Utilize Pendente, Pago ou Atrasado. O cancelamento deve ser realizado pela ação específica.");
            }

            var estaPago = pagamento.Status.Equals(
                "Pago", StringComparison.OrdinalIgnoreCase);

            if (estaPago && !pagamento.DataPagamento.HasValue)
            {
                throw new ValidationException(
                    "Informe a data em que o pagamento foi realizado.");
            }

            if (!estaPago && pagamento.DataPagamento.HasValue)
            {
                throw new ValidationException(
                    "Somente pagamentos com status Pago podem possuir data de pagamento.");
            }

        }

        private async Task ValidarMatriculaAsync(int matriculaId)
        {
            if (matriculaId <= 0)
                throw new ValidationException(
                    "Selecione uma matrícula válida.");

            var matricula = await _matriculaRepository.ObterPorIdAsync(matriculaId);

            if (matricula is null)
                throw new ValidationException(
                    "A matrícula informada não existe.");
        }
    }
}
