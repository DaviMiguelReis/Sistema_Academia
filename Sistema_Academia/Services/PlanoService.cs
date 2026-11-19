
using System.ComponentModel.DataAnnotations;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Services
{
    public class PlanoService : IPlanoService
    {
        private readonly IPlanoRepository _planoRepository;

        public PlanoService(IPlanoRepository planoRepository)
        {
            _planoRepository = planoRepository;
        }

        public Task<List<Plano>> ListarAsync()
        {
            return _planoRepository.ListarAsync();
        }

        public Task<Plano?> ObterPorIdAsync(int id)
        {
            return _planoRepository.ObterPorIdAsync(id);
        }

        public async Task<List<Plano>> PesquisarPorNomeAsync(string? nome)
        {
            var planos = await ListarAsync();

            if (string.IsNullOrWhiteSpace(nome))
                return planos;

            nome = nome.Trim();

            return planos
                .Where(p => p.Nome.Contains(
                    nome,
                    StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        public List<Plano> Ordenar(
            IEnumerable<Plano> planos,
            string? ordenarPor)
        {
            return ordenarPor?.ToLowerInvariant() switch
            {
                "nome" => planos.OrderBy(p => p.Nome).ToList(),
                "valor" => planos.OrderBy(p => p.Valor).ToList(),
                "duracao" => planos.OrderBy(p => p.DuracaoMeses).ToList(),
                _ => planos.ToList()
            };
        }

        public async Task AdicionarAsync(Plano plano)
        {
            ArgumentNullException.ThrowIfNull(plano);

            PrepararDados(plano);
            ValidarPlano(plano);

            await _planoRepository.AdicionarAsync(plano);
        }

        public async Task<bool> AtualizarAsync(Plano plano)
        {
            ArgumentNullException.ThrowIfNull(plano);

            if (plano.Id <= 0)
                throw new ValidationException("Identificação do plano inválida.");

            PrepararDados(plano);
            ValidarPlano(plano);

            return await _planoRepository.AtualizarAsync(plano);
        }

        public Task<bool> RemoverAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Identificação do plano inválida.");

            return _planoRepository.ExcluirDefinitivamenteAsync(id);
        }

        private static void PrepararDados(Plano plano)
        {
            plano.Nome = plano.Nome?.Trim() ?? string.Empty;
            plano.Descricao = plano.Descricao?.Trim() ?? string.Empty;
        }

        private static void ValidarPlano(Plano plano)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(plano);

            if (!Validator.TryValidateObject(
                plano,
                contexto,
                resultados,
                validateAllProperties: true))
            {
                var erros = string.Join(
                    " ",
                    resultados.Select(r => r.ErrorMessage));

                throw new ValidationException(erros);
            }

            if (plano.Valor <= 0)
                throw new ValidationException("O valor do plano deve ser maior que zero.");

            if (plano.DuracaoMeses <= 0)
                throw new ValidationException("A duração do plano deve ser maior que zero.");
        }
    }
}
