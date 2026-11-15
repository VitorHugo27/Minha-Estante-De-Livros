using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Domain.Exceptions;

namespace MinhaEstante.Domain.Tests;

public class LivroTests
{
    private static Livro CriarLivro() =>
       new("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico);

    [Fact]
    public void Construtor_DadosValidos_CriaLivroEmQueroLer()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico);

        Assert.Equal("Dom Casmurro", livro.Titulo);
        Assert.Equal(StatusLeitura.QueroLer, livro.Status);
    }

    [Fact]
    public void IniciarLeitura_LivroEmQueroLer_PassaParaLendo()
    {
        var livro = CriarLivro();
        var data = new DateOnly(2026, 10, 11);

        livro.IniciarLeitura(data);

        Assert.Equal(StatusLeitura.Lendo, livro.Status);
        Assert.Equal(data, livro.DataInicioLeitura);
    }

    [Fact]
    public void Abandonar_LivroEmQueroLer_LancaDomainException()
    {
        var livro = CriarLivro();

        Assert.Throws<DomainException>(() => livro.Abandonar());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Avaliar_NotaForaDoIntervalo_LancaDomainException(int nota)
    {
        var livro = CriarLivro();
        livro.IniciarLeitura(new DateOnly(2026, 9, 1));
        livro.ConcluirLeitura(new DateOnly(2026, 10, 1));

        Assert.Throws<DomainException>(() => livro.Avaliar(nota));
    }

    [Fact]
    public void RegistrarLeituraAnterior_SemDataDeInicio_RegistraComoLido()
    {
        var livro = CriarLivro();
        var dataFim = new DateOnly(2019, 3, 20);

        livro.RegistrarLeituraAnterior(null, dataFim);

        Assert.Equal(StatusLeitura.Lido, livro.Status);
        Assert.Null(livro.DataInicioLeitura);
        Assert.Equal(dataFim, livro.DataFimLeitura);
    }

    [Fact]
    public void RegistrarLeituraAnterior_FimAntesDoInicio_LancaDomainException()
    {
        var livro = CriarLivro();
        var dataInicio = new DateOnly(2019, 3, 20);
        var dataFim = new DateOnly(2019, 3, 1);

        Assert.Throws<DomainException>(() => livro.RegistrarLeituraAnterior(dataInicio, dataFim));
    }

    [Fact]
    public void RegistrarLeituraAnterior_InicioEFimNoMesmoDia_RegistraComoLido()
    {
        var livro = CriarLivro();
        var data = new DateOnly(2019, 3, 20);

        livro.RegistrarLeituraAnterior(data, data);
        Assert.Equal(StatusLeitura.Lido, livro.Status);
    }

    [Fact]
    public void Construtor_LinkDaCapaValido_GuardaOLink()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico,
            capaUrl: "https://exemplo.com/capas/dom-casmurro.jpg");

        Assert.Equal("https://exemplo.com/capas/dom-casmurro.jpg", livro.CapaUrl);
    }

    [Theory]
    [InlineData("capa.jpg")]
    [InlineData("ftp://exemplo.com/capa.jpg")]
    public void Construtor_LinkDaCapaInvalido_LancaDomainException(string capaUrl)
    {
        Assert.Throws<DomainException>(() =>
            new Livro("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico, capaUrl: capaUrl));
    }

    [Fact]
    public void AtualizarDados_LinkDaCapaEmBranco_RemoveACapa()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico,
            capaUrl: "https://exemplo.com/capas/dom-casmurro.jpg");

        livro.AtualizarDados("Dom Casmurro", "Machado de Assis", FormatoLivro.Fisico,
            null, null, null, capaUrl: "  ");

        Assert.Null(livro.CapaUrl);
    }
}
