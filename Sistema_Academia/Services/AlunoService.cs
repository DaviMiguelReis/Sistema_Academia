
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Sistema_Academia.Models;
using Sistema_Academia.Repositories.Interfaces;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public Task<List<Aluno>> ListarAsync()
        {
            return _alunoRepository.ListarAsync();
        }

        public Task<Aluno?> ObterPorIdAsync(int id)
        {
            return _alunoRepository.ObterPorIdAsync(id);
        }

        public async Task<List<Aluno>> PesquisarPorNomeAsync(string? nome)
        {
            var alunos = await ListarAsync();

            if (string.IsNullOrWhiteSpace(nome))
                return alunos;

            return alunos
                .Where(a => a.Nome.Contains(
                    nome.Trim(),
                    StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        public List<Aluno> Ordenar(
            IEnumerable<Aluno> alunos,
            string? ordenarPor)
        {
            return ordenarPor?.ToLowerInvariant() switch
            {
                "nome" => alunos.OrderBy(a => a.Nome).ToList(),
                "datacadastro" => alunos.OrderBy(a => a.DataCadastro).ToList(),
                "datanascimento" => alunos.OrderBy(a => a.DataNascimento).ToList(),
                _ => alunos.ToList()
            };
        }

        public async Task AdicionarAsync(Aluno aluno)
        {
            ArgumentNullException.ThrowIfNull(aluno);

            PrepararDados(aluno);
            ValidarAluno(aluno);

            if (await _alunoRepository.ExisteCpfAsync(aluno.CPF))
                throw new ValidationException("Já existe um aluno cadastrado com este CPF.");

            if (aluno.DataCadastro == default)
                aluno.DataCadastro = DateTime.Now;

            await _alunoRepository.AdicionarAsync(aluno);
        }

        public async Task<bool> AtualizarAsync(Aluno aluno)
        {
            ArgumentNullException.ThrowIfNull(aluno);

            if (aluno.Id <= 0)
                throw new ValidationException("Identificação do aluno inválida.");

            PrepararDados(aluno);
            ValidarAluno(aluno);

            if (await _alunoRepository.ExisteCpfAsync(aluno.CPF, aluno.Id))
                throw new ValidationException("Já existe outro aluno cadastrado com este CPF.");

            return await _alunoRepository.AtualizarAsync(aluno);
        }

        public Task<bool> RemoverAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Identificação do aluno inválida.");

            return _alunoRepository.ExcluirDefinitivamenteAsync(id);
        }

        private static void PrepararDados(Aluno aluno)
        {
            aluno.Nome = aluno.Nome?.Trim() ?? string.Empty;
            aluno.CPF = Regex.Replace(aluno.CPF ?? string.Empty, @"[^0-9]", "");
            aluno.Email = aluno.Email?.Trim() ?? string.Empty;
            aluno.Telefone = Regex.Replace(aluno.Telefone ?? string.Empty, @"[^0-9]", "");
            aluno.Endereco = aluno.Endereco?.Trim() ?? string.Empty;
        }

        private static void ValidarAluno(Aluno aluno)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(aluno);

            if (!Validator.TryValidateObject(
                aluno,
                contexto,
                resultados,
                validateAllProperties: true))
            {
                var erros = string.Join(" ", resultados.Select(r => r.ErrorMessage));
                throw new ValidationException(erros);
            }

            // Projeto acadêmico: o campo CPF funciona como identificador de teste.
            // Validamos apenas o formato (11 dígitos), sem conferir os dígitos verificadores.
            if (aluno.CPF.Length != 11 || aluno.CPF.Any(c => c < '0' || c > '9'))
                throw new ValidationException("Informe exatamente 11 números no campo CPF.");

            if (aluno.DataNascimento.Date > DateTime.Today)
                throw new ValidationException("A data de nascimento não pode ser futura.");

            if (aluno.DataNascimento == default)
                throw new ValidationException("Informe uma data de nascimento válida.");
        }


    }
}
