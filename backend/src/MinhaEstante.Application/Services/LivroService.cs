using MinhaEstante.Application.Dtos;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Domain.Exceptions;
using MinhaEstante.Application.Exceptions;
using System.Security.Cryptography;

namespace MinhaEstante.Application.Services;

public class LivroService(ILivroRepository repository)
{
    public async Task<LivroResponse> CadastrarAsync(
        CadastrarLivroRequest request, CancellationToken cancellationToken = default)
    {
        var livro = new Livro(request.Titulo, request.Autor, request.Formato,
            request.Isbn, request.Genero, request.TotalPaginas, request.CapaUrl);

        switch (request.Status)
        {
            case StatusLeitura.QueroLer:
                break;
            case StatusLeitura.Lendo:
                livro.IniciarLeitura(request.DataInicioLeitura ?? DateOnly.FromDateTime(DateTime.Today));
                break;
            case StatusLeitura.Lido:
                livro.RegistrarLeituraAnterior(request.DataInicioLeitura, request.DataFimLeitura);
                break;
            default:
                throw new DomainException("Um livro só pode ser cadastrado como Quero ler, Lendo ou Lido.");
        }

        if (request.Avaliacao.HasValue)
            livro.Avaliar(request.Avaliacao.Value);

        await repository.AdicionarAsync(livro, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }

    public async Task<LivroResponse> ObterPorIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }

    public async Task<IReadOnlyList<LivroResponse>> ListarAsync(
        StatusLeitura? status, CancellationToken cancellationToken = default)
    {
        var livros = await repository.ListarAsync(status, cancellationToken);
        return livros.Select(livro => LivroResponse.DeEntidade(livro)).ToList();
    }

    private async Task<Livro> ObterLivroAsync(Guid id, CancellationToken cancellationToken)
    {
        var livro = await repository.ObterPorIdAsync(id, cancellationToken);

        if (livro is null)
            throw new NotFoundException($"Livro {id} não encontrado.");
        return livro;
    }

    public async Task<LivroResponse> IniciarLeituraAsync(
        Guid id, DateOnly data, CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        livro.IniciarLeitura(data);
        await repository.AtualizarAsync(livro, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }

    public async Task<LivroResponse> ConcluirLeituraAsync(
        Guid id, DateOnly data, CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        livro.ConcluirLeitura(data);
        await repository.AtualizarAsync(livro, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }

    public async Task<LivroResponse> AbandonarAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var livros = await ObterLivroAsync(id, cancellationToken);
        livros.Abandonar();
        await repository.AtualizarAsync(livros, cancellationToken);
        return LivroResponse.DeEntidade(livros);
    }

    public async Task<LivroResponse> AvaliarAsync(
        Guid id, int nota, CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        livro.Avaliar(nota);
        await repository.AtualizarAsync(livro, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }


    public async Task<LivroResponse> AtualizarDadosAsync(
    Guid id, AtualizarLivroRequest request, CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        livro.AtualizarDados(request.Titulo, request.Autor, request.Formato,
                             request.Isbn, request.Genero, request.TotalPaginas, request.CapaUrl);
        await repository.AtualizarAsync(livro, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }

    public async Task<LivroResponse> AtualizarAnotacoesAsync(
    Guid id, string? anotacoes, CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        livro.AtualizarAnotacoes(anotacoes);
        await repository.AtualizarAsync(livro, cancellationToken);
        return LivroResponse.DeEntidade(livro);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var livro = await ObterLivroAsync(id, cancellationToken);
        await repository.RemoverAsync(livro, cancellationToken);
    }
}
