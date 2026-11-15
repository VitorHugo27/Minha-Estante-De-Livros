namespace MinhaEstante.Application.Dtos;

public record EstatisticasResponse(
    int TotalLivros,
    int QueroLer,
    int Lendo,
    int Lidos,
    int Abandonados,
    int Fisicos,
    int Virtuais,
    int PaginasLidas,
    double? MediaAvaliacao,
    int? MediaDiasPorLivro,
    int LidosSemData,
    IReadOnlyList<LeiturasDoAno> LidosPorAno,
    IReadOnlyList<LivrosPorNota> Notas,
    IReadOnlyList<LivrosPorGenero> GenerosMaisLidos);

public record LeiturasDoAno(int Ano, int Paginas, IReadOnlyList<LivroDoAno> Livros);

public record LivroDoAno(string Titulo, int? Paginas);

public record LivrosPorNota(int Nota, int Livros);

public record LivrosPorGenero(string Genero, int Livros);
