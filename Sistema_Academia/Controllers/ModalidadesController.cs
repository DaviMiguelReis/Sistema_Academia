
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sistema_Academia.Models;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Controllers
{
    public class ModalidadesController : Controller
    {
        private readonly IModalidadeService _modalidadeService;

        public ModalidadesController(IModalidadeService modalidadeService)
        {
            _modalidadeService = modalidadeService;
        }

        public async Task<IActionResult> Index(
            string? pesquisa,
            string? ordenarPor)
        {
            var modalidades = await _modalidadeService.PesquisarAsync(pesquisa);

            modalidades = _modalidadeService.Ordenar(modalidades, ordenarPor);

            return View(modalidades);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var modalidade = await _modalidadeService.ObterPorIdAsync(id);

            if (modalidade is null)
                return NotFound();

            return View(modalidade);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View(new Modalidade());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Modalidade model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _modalidadeService.AdicionarAsync(model);

                TempData["Mensagem"] = "Modalidade cadastrada com sucesso!";

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
            var modalidade = await _modalidadeService.ObterPorIdAsync(id);

            if (modalidade is null)
                return NotFound();

            return View(modalidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Modalidade model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var atualizado = await _modalidadeService.AtualizarAsync(model);

                if (!atualizado)
                    return NotFound();

                TempData["Mensagem"] = "Modalidade atualizada com sucesso!";

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
            var modalidade = await _modalidadeService.ObterPorIdAsync(id);

            if (modalidade is null)
                return NotFound();

            return View(modalidade);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var removido = await _modalidadeService.RemoverAsync(id);

            if (!removido)
                return NotFound();

            TempData["Mensagem"] = "Modalidade excluída definitivamente!";

            return RedirectToAction(nameof(Index));
        }
    }
}
