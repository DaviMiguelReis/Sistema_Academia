
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Models;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Controllers
{
    public class MatriculasController : Controller
    {
        private readonly IMatriculaService _matriculaService;
        private readonly IAlunoService _alunoService;
        private readonly IPlanoService _planoService;
        private readonly IModalidadeService _modalidadeService;

        public MatriculasController(
            IMatriculaService matriculaService,
            IAlunoService alunoService,
            IPlanoService planoService,
            IModalidadeService modalidadeService)
        {
            _matriculaService = matriculaService;
            _alunoService = alunoService;
            _planoService = planoService;
            _modalidadeService = modalidadeService;
        }

        public async Task<IActionResult> Index(
            string? pesquisa,
            string? ordenarPor)
        {
            var matriculas =
                await _matriculaService.PesquisarAsync(pesquisa);

            matriculas =
                _matriculaService.Ordenar(matriculas, ordenarPor);

            return View(matriculas);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var matricula =
                await _matriculaService.ObterPorIdAsync(id);

            if (matricula is null)
                return NotFound();

            return View(matricula);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            await CarregarListas();

            var model = new Matricula
            {
                DataMatricula = DateTime.Today,
                DataInicio = DateTime.Today,
                Ativo = true,
                Status = "Ativa"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(
            Matricula model,
            List<int>? modalidadeIds)
        {
            var idsSelecionados = modalidadeIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            // A data da matrícula é definida pelo sistema, não pelo formulário.
            model.DataMatricula = DateTime.Today;
            ModelState.Remove(nameof(Matricula.DataMatricula));

            if (!ModelState.IsValid)
            {
                await CarregarListas(
                    model.AlunoId,
                    model.PlanoId,
                    idsSelecionados);

                return View(model);
            }

            try
            {
                await _matriculaService.AdicionarAsync(
                    model,
                    idsSelecionados);

                TempData["Mensagem"] =
                    "Matrícula cadastrada com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível salvar a matrícula no banco de dados. Confira se o aluno, o plano e as modalidades ainda existem e tente novamente.");
            }

            await CarregarListas(
                model.AlunoId,
                model.PlanoId,
                idsSelecionados);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var matricula =
                await _matriculaService.ObterPorIdAsync(id);

            if (matricula is null)
                return NotFound();

            var idsSelecionados = matricula.MatriculaModalidades
                .Select(mm => mm.ModalidadeId)
                .ToList();

            await CarregarListas(
                matricula.AlunoId,
                matricula.PlanoId,
                idsSelecionados);

            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            Matricula model,
            List<int>? modalidadeIds)
        {
            var idsSelecionados = modalidadeIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            ModelState.Remove(nameof(Matricula.DataMatricula));

            if (!ModelState.IsValid)
            {
                await CarregarListas(
                    model.AlunoId,
                    model.PlanoId,
                    idsSelecionados);

                return View(model);
            }

            try
            {
                var atualizado =
                    await _matriculaService.AtualizarAsync(
                        model,
                        idsSelecionados);

                if (!atualizado)
                    return NotFound();

                TempData["Mensagem"] =
                    "Matrícula atualizada com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar a matrícula no banco de dados. Confira os dados selecionados e tente novamente.");
            }

            await CarregarListas(
                model.AlunoId,
                model.PlanoId,
                idsSelecionados);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int id)
        {
            var matricula =
                await _matriculaService.ObterPorIdAsync(id);

            if (matricula is null)
                return NotFound();

            return View(matricula);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var removido =
                await _matriculaService.RemoverAsync(id);

            if (!removido)
            {
                TempData["Erro"] = "Não foi possível excluir a matrícula. Atualize a página e tente novamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensagem"] =
                "Matrícula excluída definitivamente!";

            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarListas(
            int? alunoSelecionado = null,
            int? planoSelecionado = null,
            IEnumerable<int>? modalidadesSelecionadas = null)
        {
            var alunos = await _alunoService.ListarAsync();
            var planos = await _planoService.ListarAsync();
            var modalidades = await _modalidadeService.ListarAsync();

            var idsSelecionados = modalidadesSelecionadas?
                .Distinct()
                .ToList() ?? new List<int>();

            ViewBag.Alunos = new SelectList(
                alunos.Where(a => a.Ativo || a.Id == alunoSelecionado),
                "Id",
                "Nome",
                alunoSelecionado);

            ViewBag.Planos = new SelectList(
                planos.Where(p => p.Ativo || p.Id == planoSelecionado),
                "Id",
                "Nome",
                planoSelecionado);

            var modalidadesDisponiveis = modalidades
                .Where(m => m.Ativo || idsSelecionados.Contains(m.Id))
                .ToList();

            ViewBag.Modalidades = new MultiSelectList(
                modalidadesDisponiveis,
                "Id",
                "Nome",
                idsSelecionados);

            ViewBag.ModalidadesSelecionadas = idsSelecionados;
        }
    }
}
