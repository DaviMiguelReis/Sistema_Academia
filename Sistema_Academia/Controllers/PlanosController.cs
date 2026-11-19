
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sistema_Academia.Models;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Controllers
{
    public class PlanosController : Controller
    {
        private readonly IPlanoService _planoService;

        public PlanosController(IPlanoService planoService)
        {
            _planoService = planoService;
        }

        public async Task<IActionResult> Index()
        {
            var planos = await _planoService.ListarAsync();

            return View(planos);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var plano = await _planoService.ObterPorIdAsync(id);

            if (plano is null)
                return NotFound();

            return View(plano);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View(new Plano());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Plano model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _planoService.AdicionarAsync(model);

                TempData["Mensagem"] = "Plano cadastrado com sucesso!";

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
            var plano = await _planoService.ObterPorIdAsync(id);

            if (plano is null)
                return NotFound();

            return View(plano);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Plano model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var atualizado = await _planoService.AtualizarAsync(model);

                if (!atualizado)
                    return NotFound();

                TempData["Mensagem"] = "Plano atualizado com sucesso!";

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
            var plano = await _planoService.ObterPorIdAsync(id);

            if (plano is null)
                return NotFound();

            return View(plano);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            var removido = await _planoService.RemoverAsync(id);

            if (!removido)
                return NotFound();

            TempData["Mensagem"] = "Plano excluído definitivamente!";

            return RedirectToAction(nameof(Index));
        }
    }
}
