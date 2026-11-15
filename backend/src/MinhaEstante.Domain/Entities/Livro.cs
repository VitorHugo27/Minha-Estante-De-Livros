using MinhaEstante.Domain.Enums;
using MinhaEstante.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace MinhaEstante.Domain.Entities;

public class Livro
{
    public Livro(string titulo, string autor, FormatoLivro formato,
              string? isbn = null, string? genero = null, int? totalPaginas = null,
              string? capaUrl = null)
    {
        AtualizarDados(titulo, autor, formato, isbn, genero, totalPaginas, capaUrl);

        Id = Guid.NewGuid();
        Status = StatusLeitura.QueroLer;
        DataCadastro = DateTime.UtcNow;
    }
    private Livro()
    {
        Titulo = null!;
        Autor = null!;
    }

    [MemberNotNull(nameof(Titulo), nameof(Autor))]
    public void AtualizarDados(string titulo, string autor, FormatoLivro formato,
                               string? isbn, string? genero, int? totalPaginas,
                               string? capaUrl)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("O título é obrigatório.");
        if (string.IsNullOrWhiteSpace(autor))
            throw new DomainException("O autor é obrigatório.");
        if (!Enum.IsDefined(formato))
            throw new DomainException("Formato inválido.");
        if (totalPaginas is <= 0)
            throw new DomainException("O total de páginas deve ser maior que zero.");
        if (!LinkValido(capaUrl))
            throw new DomainException("O link da capa deve ser um endereço completo, começando com http:// ou https://.");

        Titulo = titulo.Trim();
        Autor = autor.Trim();
        Formato = formato;
        Isbn = Normalizar(isbn);
        Genero = Normalizar(genero);
        TotalPaginas = totalPaginas;
        CapaUrl = Normalizar(capaUrl);
    }

    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string Autor { get; private set; }
    public FormatoLivro Formato { get; private set; }
    public string? Isbn { get; private set; }
    public string? Genero { get; private set; }
    public int? TotalPaginas { get; private set; }
    public string? CapaUrl { get; private set; }
    public StatusLeitura Status { get; private set; }
    public DateOnly? DataInicioLeitura { get; private set; }
    public DateOnly? DataFimLeitura { get; private set; }
    public int? Avaliacao { get; private set; }
    public string? Anotacoes { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public void IniciarLeitura(DateOnly data)
    {
        if (Status is not (StatusLeitura.QueroLer or StatusLeitura.Abandonado))
            throw new DomainException("Só é possível iniciar um livro que você quer ler ou que abandonou.");
        Status = StatusLeitura.Lendo;
        DataInicioLeitura = data;
    }

    public void ConcluirLeitura(DateOnly data)
    {
        if (Status != StatusLeitura.Lendo)
            throw new DomainException("Só é possível concluir um livro que está sendo lido.");
        if (data < DataInicioLeitura)
            throw new DomainException("A conclusão não pode ser anterior ao início da leitura.");

        Status = StatusLeitura.Lido;
        DataFimLeitura = data;
    }

    public void Abandonar()
    {
        if (Status != StatusLeitura.Lendo)
            throw new DomainException("Só é possível abandonar um livro que está sendo lido.");

        Status = StatusLeitura.Abandonado;
    }

    public void Avaliar(int nota)
    {
        if (Status != StatusLeitura.Lido)
            throw new DomainException("Só é possível avaliar um livro que você já leu.");
        if (nota is < 1 or > 5)
            throw new DomainException("A avaliação deve ser uma nota de 1 a 5.");

        Avaliacao = nota;
    }

    public void RegistrarLeituraAnterior(DateOnly? dataInicio, DateOnly? dataFim)
    {
        if (Status != StatusLeitura.QueroLer)
            throw new DomainException("Só é possível registrar uma leitura anterior de um livro em 'Quero ler'.");

        if (dataInicio.HasValue && dataFim.HasValue && dataFim < dataInicio)
            throw new DomainException("A conclusão não pode ser anterior ao início da leitura.");

        Status = StatusLeitura.Lido;
        DataInicioLeitura = dataInicio;
        DataFimLeitura = dataFim;
    }

    public void AtualizarAnotacoes(string? anotacoes)
    {
        Anotacoes = Normalizar(anotacoes);
    }

    private static string? Normalizar(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

    private static bool LinkValido(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return true;

        return Uri.TryCreate(link.Trim(), UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
