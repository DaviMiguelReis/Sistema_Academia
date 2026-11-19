using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Models;
using Sistema_Academia.Services.Interfaces;

namespace Sistema_Academia.Controllers
{
    public class PagamentosController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IMatriculaService _matriculaService;
        private readonly ILogger<PagamentosController> _logger;

        public PagamentosController(
            IPagamentoService pagamentoService,
            IMatriculaService matriculaService,
            ILogger<PagamentosController> logger)
        {
            _pagamentoService = pagamentoService;
            _matriculaService = matriculaService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? pesquisa)
        {
            var pagamentos = string.IsNullOrWhiteSpace(pesquisa)
                ? await _pagamentoService.ListarAsync()
                : await _pagamentoService.PesquisarAsync(pesquisa);

            ViewBag.Pesquisa = pesquisa;
            return View(pagamentos);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            if (id <= 0)
                return BadRequest();

            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento is null)
                return NotFound();

            return View(pagamento);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            await CarregarMatriculas();
            return View(new Pagamento
            {
                Status = "Pendente",
                DataVencimento = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Pagamento model)
        {
            if (!ModelState.IsValid)
            {
                await CarregarMatriculas(model.MatriculaId);
                return View(model);
            }

            try
            {
                await _pagamentoService.AdicionarAsync(model);
                TempData["Mensagem"] = "Pagamento cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CarregarMatriculas(model.MatriculaId);
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao salvar pagamento da matrícula {MatriculaId}.", model.MatriculaId);
                ModelState.AddModelError(string.Empty,
                    "O banco de dados não conseguiu salvar o pagamento. Confira se a matrícula ainda existe e tente novamente.");
                await CarregarMatriculas(model.MatriculaId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cadastrar pagamento.");
                ModelState.AddModelError(string.Empty,
                    "Ocorreu um erro ao cadastrar o pagamento. Consulte o log do sistema para identificar a causa.");
                await CarregarMatriculas(model.MatriculaId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (id <= 0)
                return BadRequest();

            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento is null)
                return NotFound();

            await CarregarMatriculas(pagamento.MatriculaId);
            return View(pagamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Pagamento model)
        {
            if (!ModelState.IsValid)
            {
                await CarregarMatriculas(model.MatriculaId);
                return View(model);
            }

            try
            {
                var atualizado = await _pagamentoService.AtualizarAsync(model);

                if (!atualizado)
                    return NotFound();

                TempData["Mensagem"] = "Pagamento atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CarregarMatriculas(model.MatriculaId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int id)
        {
            if (id <= 0)
                return BadRequest();

            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento is null)
                return NotFound();

            if (pagamento.Status.Equals("Pago", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Erro"] = "Não é possível cancelar um pagamento já realizado.";
                return RedirectToAction(nameof(Detalhes), new { id });
            }

            return View(pagamento);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            try
            {
                var removido = await _pagamentoService.RemoverAsync(id);

                if (!removido)
                {
                    TempData["Erro"] = "Não foi possível excluir o pagamento. Pagamentos pagos são preservados no histórico financeiro.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Mensagem"] = "Pagamento excluído definitivamente!";
            }
            catch (ValidationException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarMatriculas(int? matriculaSelecionada = null)
        {
            var matriculas = await _matriculaService.ListarAsync();

            ViewBag.Matriculas = matriculas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"Matrícula #{m.Id} - {m.Aluno?.Nome ?? "Aluno não identificado"}",
                Selected = m.Id == matriculaSelecionada
            }).ToList();
        }
    }
}
