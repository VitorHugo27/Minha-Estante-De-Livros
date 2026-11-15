using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaEstante.Api.Requests;
using MinhaEstante.Application.Dtos;
using MinhaEstante.Application.Services;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Api.Controllers
{
    [ApiController]
    [Route("api/livros")]
    public class LivrosController(LivroService livroService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<LivroResponse>>> Listar(
            [FromQuery] StatusLeitura? status, CancellationToken cancellationToken)
        {
            var livros = await livroService.ListarAsync(status, cancellationToken);
            return Ok(livros);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> ObterPorId(
            Guid id, CancellationToken cancellationToken)
        {
            var livro = await livroService.ObterPorIdAsync(id, cancellationToken);
            return Ok(livro);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LivroResponse>> Cadastrar(
            CadastrarLivroRequest request, CancellationToken cancellationToken)
        {
            var livro = await livroService.CadastrarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObterPorId), new { id = livro.Id }, livro);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> AtualizarDados(
            Guid id, AtualizarLivroRequest request, CancellationToken cancellationToken)
        {
            var livro = await livroService.AtualizarDadosAsync(id, request, cancellationToken);
            return Ok(livro);
        }

        [HttpPut("{id:guid}/anotacoes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> AtualizarAnotacoes(
            Guid id, AnotacoesRequest request, CancellationToken cancellationToken)
        {
            var livro = await livroService.AtualizarAnotacoesAsync(
                id, request.Anotacoes, cancellationToken);
            return Ok(livro);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
        {
            await livroService.RemoverAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/iniciar-leitura")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> IniciarLeitura(
            Guid id, DataLeituraRequest request, CancellationToken cancellationToken)
        {
            var data = request.Data ?? DateOnly.FromDateTime(DateTime.Today);
            var livro = await livroService.IniciarLeituraAsync(id, data, cancellationToken);
            return Ok(livro);
        }

        [HttpPost("{id:guid}/concluir-leitura")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> ConcluirLeitura(
            Guid id, DataLeituraRequest request, CancellationToken cancellationToken)
        {
            var data = request.Data ?? DateOnly.FromDateTime(DateTime.Today);
            var livro = await livroService.ConcluirLeituraAsync(id, data, cancellationToken);
            return Ok(livro);
        }

        [HttpPost("{id:guid}/abandonar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> Abandonar(
            Guid id, CancellationToken cancellationToken)
        {
            var livro = await livroService.AbandonarAsync(id, cancellationToken);
            return Ok(livro);
        }

        [HttpPut("{id:guid}/avaliacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LivroResponse>> Avaliar(
            Guid id, AvaliacaoRequest request, CancellationToken cancellationToken)
        {
            var livro = await livroService.AvaliarAsync(id, request.Nota, cancellationToken);
            return Ok(livro);
        }
    }
}