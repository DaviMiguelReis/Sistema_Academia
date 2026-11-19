
using System.ComponentModel.DataAnnotations;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Services
{
    public class ModalidadeService : IModalidadeService
    {
        private readonly IModalidadeRepository _modalidadeRepository;

        public ModalidadeService(IModalidadeRepository modalidadeRepository)
        {
            _modalidadeRepository = modalidadeRepository;
        }

        public Task<List<Modalidade>> ListarAsync()
        {
            return _modalidadeRepository.ListarAsync();
        }

        public async Task<Modalidade?> ObterPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Identificação da modalidade inválida.");

            return await _modalidadeRepository.ObterPorIdAsync(id);
        }

        public async Task<List<Modalidade>> PesquisarAsync(string? termo)
        {
            var modalidades = await ListarAsync();

            if (string.IsNullOrWhiteSpace(termo))
                return modalidades;

            termo = termo.Trim();

            return modalidades
                .Where(m =>
                    m.Nome.Contains(
                        termo,
                        StringComparison.CurrentCultureIgnoreCase) ||
                    m.Professor.Contains(
                        termo,
                        StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        public List<Modalidade> Ordenar(
            IEnumerable<Modalidade> modalidades,
            string? ordenarPor)
        {
            ArgumentNullException.ThrowIfNull(modalidades);

            return ordenarPor?.ToLowerInvariant() switch
            {
                "nome" => modalidades.OrderBy(m => m.Nome).ToList(),
                "professor" => modalidades.OrderBy(m => m.Professor).ToList(),
                "capacidade" => modalidades
                    .OrderBy(m => m.CapacidadeMaxima)
                    .ToList(),
                _ => modalidades.ToList()
            };
        }

        public async Task AdicionarAsync(Modalidade modalidade)
        {
            ArgumentNullException.ThrowIfNull(modalidade);

            PrepararDados(modalidade);
            ValidarModalidade(modalidade);

            await _modalidadeRepository.AdicionarAsync(modalidade);
        }

        public async Task<bool> AtualizarAsync(Modalidade modalidade)
        {
            ArgumentNullException.ThrowIfNull(modalidade);

            if (modalidade.Id <= 0)
                throw new ValidationException("Identificação da modalidade inválida.");

            PrepararDados(modalidade);
            ValidarModalidade(modalidade);

            return await _modalidadeRepository.AtualizarAsync(modalidade);
        }

        public Task<bool> RemoverAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Identificação da modalidade inválida.");

            return _modalidadeRepository.ExcluirDefinitivamenteAsync(id);
        }

        private static void PrepararDados(Modalidade modalidade)
        {
            modalidade.Nome = modalidade.Nome?.Trim() ?? string.Empty;
            modalidade.Descricao = modalidade.Descricao?.Trim() ?? string.Empty;
            modalidade.Professor = modalidade.Professor?.Trim() ?? string.Empty;
        }

        private static void ValidarModalidade(Modalidade modalidade)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(modalidade);

            if (!Validator.TryValidateObject(
                modalidade,
                contexto,
                resultados,
                validateAllProperties: true))
            {
                var erros = string.Join(
                    " ",
                    resultados.Select(r => r.ErrorMessage));

                throw new ValidationException(erros);
            }

            if (modalidade.CapacidadeMaxima <= 0)
                throw new ValidationException(
                    "A capacidade máxima deve ser maior que zero.");
        }
    }
}
