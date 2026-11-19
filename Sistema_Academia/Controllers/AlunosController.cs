
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sistema_Academia.Models;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Controllers
{
    public class AlunosController : Controller
    {
        private readonly IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        public async Task<IActionResult> Index(
            string? pesquisa,
            string? ordenarPor)
        {
            var alunos = await _alunoService.PesquisarPorNomeAsync(pesquisa);

            alunos = _alunoService.Ordenar(alunos, ordenarPor);

            return View(alunos);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var aluno = await _alunoService.ObterPorIdAsync(id);

            if (aluno is null)
                return NotFound();

            return View(aluno);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View(new Aluno());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Aluno model)
        {
            model.CPF = System.Text.RegularExpressions.Regex.Replace(
                model.CPF ?? string.Empty, @"[^0-9]", "");
            // A validação do modelo deve contar apenas os dígitos, não a máscara visual.
            model.Telefone = System.Text.RegularExpressions.Regex.Replace(
                model.Telefone ?? string.Empty, @"[^0-9]", "");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _alunoService.AdicionarAsync(model);

                TempData["Mensagem"] = "Aluno cadastrado com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var aluno = await _alunoService.ObterPorIdAsync(id);

            if (aluno is null)
                return NotFound();

            return View(aluno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Aluno model)
        {
            model.CPF = System.Text.RegularExpressions.Regex.Replace(
                model.CPF ?? string.Empty, @"[^0-9]", "");
            // A validação do modelo deve contar apenas os dígitos, não a máscara visual.
            model.Telefone = System.Text.RegularExpressions.Regex.Replace(
                model.Telefone ?? string.Empty, @"[^0-9]", "");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var atualizado = await _alunoService.AtualizarAsync(model);

                if (!atualizado)
                    return NotFound();

                TempData["Mensagem"] = "Aluno atualizado com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int id)
        {
            var aluno = await _alunoService.ObterPorIdAsync(id);

            if (aluno is null)
                return NotFound();

            return View(aluno);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var removido = await _alunoService.RemoverAsync(id);

            if (!removido)
            {
                TempData["Erro"] = "Não foi possível excluir o aluno. Verifique se ele ainda existe e tente novamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensagem"] = "Aluno excluído definitivamente!";

            return RedirectToAction(nameof(Index));
        }
    }
}
