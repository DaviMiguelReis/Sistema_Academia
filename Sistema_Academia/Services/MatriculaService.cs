
using System.ComponentModel.DataAnnotations;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPlanoRepository _planoRepository;
        private readonly IModalidadeRepository _modalidadeRepository;

        public MatriculaService(
            IMatriculaRepository matriculaRepository,
            IAlunoRepository alunoRepository,
            IPlanoRepository planoRepository,
            IModalidadeRepository modalidadeRepository)
        {
            _matriculaRepository = matriculaRepository;
            _alunoRepository = alunoRepository;
            _planoRepository = planoRepository;
            _modalidadeRepository = modalidadeRepository;
        }

        public Task<List<Matricula>> ListarAsync()
        {
            return _matriculaRepository.ListarAsync();
        }

        public async Task<Matricula?> ObterPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException(
                    "Identificação da matrícula inválida.");

            return await _matriculaRepository.ObterPorIdAsync(id);
        }

        public async Task<List<Matricula>> PesquisarAsync(string? termo)
        {
            var matriculas = await ListarAsync();

            if (string.IsNullOrWhiteSpace(termo))
                return matriculas;

            termo = termo.Trim();

            return matriculas
                .Where(m =>
                    (m.Aluno != null &&
                     m.Aluno.Nome.Contains(
                         termo,
                         StringComparison.CurrentCultureIgnoreCase)) ||
                    m.Status.Contains(
                        termo,
                        StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        public List<Matricula> Ordenar(
            IEnumerable<Matricula> matriculas,
            string? ordenarPor)
        {
            ArgumentNullException.ThrowIfNull(matriculas);

            return ordenarPor?.ToLowerInvariant() switch
            {
                "datamatricula" => matriculas
                    .OrderBy(m => m.DataMatricula)
                    .ToList(),

                "datainicio" => matriculas
                    .OrderBy(m => m.DataInicio)
                    .ToList(),

                "valor" => matriculas
                    .OrderBy(m => m.ValorContratado)
                    .ToList(),

                "status" => matriculas
                    .OrderBy(m => m.Status)
                    .ToList(),

                _ => matriculas.ToList()
            };
        }

        // Mantém compatibilidade com chamadas antigas.
        public Task AdicionarAsync(Matricula matricula)
        {
            ArgumentNullException.ThrowIfNull(matricula);

            var modalidadeIds = matricula.MatriculaModalidades
                .Select(mm => mm.ModalidadeId)
                .Distinct()
                .ToList();

            return AdicionarAsync(matricula, modalidadeIds);
        }

        public async Task AdicionarAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds)
        {
            ArgumentNullException.ThrowIfNull(matricula);
            ArgumentNullException.ThrowIfNull(modalidadeIds);

            if (matricula.DataMatricula == default)
                matricula.DataMatricula = DateTime.Today;

            PrepararDados(matricula);
            ValidarMatricula(matricula);

            var modalidades = modalidadeIds
                .Distinct()
                .ToList();

            await ValidarRelacionamentosAsync(
                matricula,
                modalidades);

            matricula.MatriculaModalidades = modalidades
                .Select(id => new MatriculaModalidade
                {
                    ModalidadeId = id,
                    Matricula = matricula
                })
                .ToList();

            await _matriculaRepository.AdicionarAsync(matricula);
        }

        // Mantém compatibilidade com chamadas antigas.
        public Task<bool> AtualizarAsync(Matricula matricula)
        {
            ArgumentNullException.ThrowIfNull(matricula);

            var modalidadeIds = matricula.MatriculaModalidades
                .Select(mm => mm.ModalidadeId)
                .Distinct()
                .ToList();

            return AtualizarAsync(matricula, modalidadeIds);
        }

        public async Task<bool> AtualizarAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds)
        {
            ArgumentNullException.ThrowIfNull(matricula);
            ArgumentNullException.ThrowIfNull(modalidadeIds);

            if (matricula.Id <= 0)
                throw new ValidationException(
                    "Identificação da matrícula inválida.");

            var existente = await _matriculaRepository
                .ObterPorIdAsync(matricula.Id);

            if (existente is null)
                return false;

            // A data original de matrícula não deve ser alterada.
            matricula.DataMatricula = existente.DataMatricula;

            PrepararDados(matricula);
            ValidarMatricula(matricula);

            var modalidades = modalidadeIds
                .Distinct()
                .ToList();

            await ValidarRelacionamentosAsync(
                matricula,
                modalidades,
                matricula.Id);

            return await _matriculaRepository.AtualizarAsync(
                matricula,
                modalidades);
        }

        public async Task<bool> RemoverAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Identificação da matrícula inválida.");

            return await _matriculaRepository.ExcluirDefinitivamenteAsync(id);
        }

        private static void PrepararDados(Matricula matricula)
        {
            matricula.Status = matricula.Status?.Trim() ?? string.Empty;
        }

        private static void ValidarMatricula(Matricula matricula)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(matricula);

            if (!Validator.TryValidateObject(
                matricula,
                contexto,
                resultados,
                validateAllProperties: true))
            {
                var erros = string.Join(
                    " ",
                    resultados.Select(r => r.ErrorMessage));

                throw new ValidationException(erros);
            }

            if (matricula.DataMatricula == default)
                throw new ValidationException(
                    "Informe uma data de matrícula válida.");

            if (matricula.DataMatricula.Date > DateTime.Today)
                throw new ValidationException(
                    "A data da matrícula não pode ser futura.");

            if (matricula.DataInicio == default)
                throw new ValidationException(
                    "Informe uma data de início válida.");

            if (matricula.DataFim.HasValue &&
                matricula.DataFim.Value.Date < matricula.DataInicio.Date)
            {
                throw new ValidationException(
                    "A data de término não pode ser anterior à data de início.");
            }

            if (matricula.ValorContratado <= 0)
                throw new ValidationException(
                    "O valor contratado deve ser maior que zero.");
        }

        private async Task ValidarRelacionamentosAsync(
            Matricula matricula,
            IReadOnlyCollection<int> modalidadeIds,
            int? ignorarMatriculaId = null)
        {
            if (matricula.AlunoId <= 0)
                throw new ValidationException(
                    "Selecione um aluno válido.");

            var aluno = await _alunoRepository
                .ObterPorIdAsync(matricula.AlunoId);

            if (aluno is null)
                throw new ValidationException(
                    "O aluno informado não existe.");

            if (!aluno.Ativo)
                throw new ValidationException(
                    "Não é possível matricular um aluno inativo.");

            if (matricula.PlanoId <= 0)
                throw new ValidationException(
                    "Selecione um plano válido.");

            var plano = await _planoRepository
                .ObterPorIdAsync(matricula.PlanoId);

            if (plano is null)
                throw new ValidationException(
                    "O plano informado não existe.");

            if (!plano.Ativo)
                throw new ValidationException(
                    "Não é possível utilizar um plano inativo.");

            foreach (var modalidadeId in modalidadeIds)
            {
                if (modalidadeId <= 0)
                    throw new ValidationException(
                        "Uma das modalidades selecionadas é inválida.");

                var modalidade = await _modalidadeRepository
                    .ObterPorIdAsync(modalidadeId);

                if (modalidade is null)
                    throw new ValidationException(
                        $"A modalidade de identificação {modalidadeId} não existe.");

                if (!modalidade.Ativo)
                    throw new ValidationException(
                        $"A modalidade {modalidade.Nome} está inativa.");

                var matriculasAtivas = modalidade.MatriculaModalidades
                    .Count(mm =>
                        mm.Matricula != null &&
                        mm.Matricula.Ativo &&
                        mm.Matricula.Id != ignorarMatriculaId);

                if (matriculasAtivas >= modalidade.CapacidadeMaxima)
                    throw new ValidationException(
                        $"A modalidade {modalidade.Nome} atingiu sua capacidade máxima.");
            }
        }
    }
}
