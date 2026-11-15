using MinhaEstante.Application.Services;
using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests;

public class EstatisticasServiceTests
{
    private readonly LivroRepositoryFake repository = new();
    private readonly EstatisticasService service;

    public EstatisticasServiceTests()
    {
        service = new EstatisticasService(repository);
    }

    private async Task AdicionarLidoAsync(string titulo, DateOnly? inicio, DateOnly? fim,
        int? paginas = null, int? nota = null, string? genero = null)
    {
        var livro = new Livro(titulo, "Autor", FormatoLivro.Fisico, genero: genero, totalPaginas: paginas);
        livro.RegistrarLeituraAnterior(inicio, fim);

        if (nota.HasValue)
            livro.Avaliar(nota.Value);

        await repository.AdicionarAsync(livro);
    }

    [Fact]
    public async Task ObterAsync_EstanteVazia_RetornaZerosSemMedias()
    {
        var estatisticas = await service.ObterAsync();

        Assert.Equal(0, estatisticas.TotalLivros);
        Assert.Null(estatisticas.MediaAvaliacao);
        Assert.Null(estatisticas.MediaDiasPorLivro);
        Assert.Empty(estatisticas.LidosPorAno);
        Assert.Equal(5, estatisticas.Notas.Count);
    }

    [Fact]
    public async Task ObterAsync_LivrosLidos_CalculaTotaisEMedias()
    {
        await AdicionarLidoAsync("A", new DateOnly(2025, 3, 1), new DateOnly(2025, 3, 10), paginas: 200, nota: 4, genero: "Fantasia");
        await AdicionarLidoAsync("B", null, new DateOnly(2026, 1, 5), paginas: 300, nota: 5, genero: "fantasia");
        await AdicionarLidoAsync("C", new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 2), paginas: 100);
        await repository.AdicionarAsync(new Livro("D", "Autor", FormatoLivro.Virtual));

        var estatisticas = await service.ObterAsync();

        Assert.Equal(4, estatisticas.TotalLivros);
        Assert.Equal(3, estatisticas.Lidos);
        Assert.Equal(1, estatisticas.QueroLer);
        Assert.Equal(600, estatisticas.PaginasLidas);
        Assert.Equal(4.5, estatisticas.MediaAvaliacao);
        Assert.Equal(6, estatisticas.MediaDiasPorLivro);
        Assert.Equal(2, Assert.Single(estatisticas.GenerosMaisLidos).Livros);
    }

    [Fact]
    public async Task ObterAsync_LivrosLidos_AgrupaPorAnoDoMaisRecente()
    {
        await AdicionarLidoAsync("A", null, new DateOnly(2025, 3, 10), paginas: 200);
        await AdicionarLidoAsync("B", null, new DateOnly(2026, 1, 5), paginas: 300);
        await AdicionarLidoAsync("C", null, new DateOnly(2026, 2, 2), paginas: 100);
        await AdicionarLidoAsync("D", null, null);

        var estatisticas = await service.ObterAsync();

        Assert.Equal(1, estatisticas.LidosSemData);
        Assert.Collection(estatisticas.LidosPorAno,
            ano =>
            {
                Assert.Equal(2026, ano.Ano);
                Assert.Equal(400, ano.Paginas);
                Assert.Equal("B", ano.Livros[0].Titulo);
                Assert.Equal("C", ano.Livros[1].Titulo);
            },
            ano =>
            {
                Assert.Equal(2025, ano.Ano);
                Assert.Equal(200, ano.Paginas);
            });
    }
}
