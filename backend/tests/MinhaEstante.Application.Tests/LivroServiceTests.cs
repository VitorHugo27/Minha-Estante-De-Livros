using MinhaEstante.Application.Dtos;
using MinhaEstante.Application.Exceptions;
using MinhaEstante.Application.Services;
using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Domain.Exceptions;

namespace MinhaEstante.Application.Tests;

public class LivroServiceTests
{
    private readonly LivroRepositoryFake repository = new();
    private readonly LivroService service;

    public LivroServiceTests()
    {
        service = new LivroService(repository);
    }

    private async Task<Livro> CriarLivroLidoAsync()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico);
        livro.RegistrarLeituraAnterior(null, new DateOnly(2019, 3, 20));
        await repository.AdicionarAsync(livro);
        return livro;
    }

    [Fact]
    public async Task AvaliarAsync_LivroLido_SalvaANota()
    {
        var livro = await CriarLivroLidoAsync();

        var resposta = await service.AvaliarAsync(livro.Id, 4);

        Assert.Equal(4, resposta.Avaliacao);
        Assert.Equal(1, repository.TotalDeAtualizacoes);
    }

    [Fact]
    public async Task AvaliarAsync_NotaForaDoIntervalo_NaoSalva()
    {
        var livro = await CriarLivroLidoAsync();

        await Assert.ThrowsAsync<DomainException>(() => service.AvaliarAsync(livro.Id, 6));
        Assert.Equal(0, repository.TotalDeAtualizacoes);
    }

    [Fact]
    public async Task AvaliarAsync_LivroInexistente_LancaNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => service.AvaliarAsync(Guid.NewGuid(), 4));
    }

    [Fact]
    public async Task RemoverAsync_LivroInexistente_LancaNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => service.RemoverAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CadastrarAsync_LidoComAvaliacao_SalvaOLivroAvaliado()
    {
        var request = new CadastrarLivroRequest(
            "Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico,
            Isbn: null, Genero: null, TotalPaginas: null,
            Status: StatusLeitura.Lido,
            DataInicioLeitura: null, DataFimLeitura: new DateOnly(2019, 3, 20),
            Avaliacao: 5);

        var resposta = await service.CadastrarAsync(request);

        Assert.Equal(StatusLeitura.Lido, resposta.Status);
        Assert.Equal(5, resposta.Avaliacao);
        Assert.NotNull(await repository.ObterPorIdAsync(resposta.Id));
    }

    [Fact]
    public async Task CadastrarAsync_LendoComAvaliacao_NaoSalva()
    {
        var request = new CadastrarLivroRequest(
            "Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico,
            Isbn: null, Genero: null, TotalPaginas: null,
            Status: StatusLeitura.Lendo,
            DataInicioLeitura: null, DataFimLeitura: null,
            Avaliacao: 5);

        await Assert.ThrowsAsync<DomainException>(() => service.CadastrarAsync(request));
        Assert.Empty(await repository.ListarAsync(null));
    }
}
