using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.Fakes;

public class LivroRepositoryFake : ILivroRepository
{
    private readonly List<Livro> livros = [];

    // Quantas vezes o serviço pediu para salvar uma alteração.
    public int TotalDeAtualizacoes { get; private set; }

    public Task<Livro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(livros.FirstOrDefault(livro => livro.Id == id));
    }

    public Task<IReadOnlyList<Livro>> ListarAsync(
        StatusLeitura? status, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Livro> resultado = livros
            .Where(livro => status == null || livro.Status == status)
            .OrderBy(livro => livro.Titulo)
            .ToList();

        return Task.FromResult(resultado);
    }

    public Task AdicionarAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        livros.Add(livro);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        TotalDeAtualizacoes++;
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        livros.Remove(livro);
        return Task.CompletedTask;
    }

}
