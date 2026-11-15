using MinhaEstante.Application.Dtos;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Services;

public class EstatisticasService(ILivroRepository repository)
{
    public async Task<EstatisticasResponse> ObterAsync(CancellationToken cancellationToken = default)
    {
        var livros = await repository.ListarAsync(null, cancellationToken);
        var lidos = livros.Where(livro => livro.Status == StatusLeitura.Lido).ToList();

        var notas = lidos
            .Where(livro => livro.Avaliacao.HasValue)
            .Select(livro => livro.Avaliacao!.Value)
            .ToList();

        var duracoes = lidos
            .Where(livro => livro.DataInicioLeitura.HasValue && livro.DataFimLeitura.HasValue)
            .Select(livro => livro.DataFimLeitura!.Value.DayNumber - livro.DataInicioLeitura!.Value.DayNumber + 1)
            .ToList();

        var lidosPorAno = lidos
            .Where(livro => livro.DataFimLeitura.HasValue)
            .GroupBy(livro => livro.DataFimLeitura!.Value.Year)
            .OrderByDescending(ano => ano.Key)
            .Select(ano => new LeiturasDoAno(
                ano.Key,
                ano.Sum(livro => livro.TotalPaginas ?? 0),
                ano.OrderBy(livro => livro.DataFimLeitura)
                    .Select(livro => new LivroDoAno(livro.Titulo, livro.TotalPaginas))
                    .ToList()))
            .ToList();

        var livrosPorNota = Enumerable.Range(1, 5)
            .Select(nota => new LivrosPorNota(nota, notas.Count(n => n == nota)))
            .ToList();

        var generosMaisLidos = lidos
            .Where(livro => livro.Genero is not null)
            .GroupBy(livro => livro.Genero!, StringComparer.OrdinalIgnoreCase)
            .Select(genero => new LivrosPorGenero(genero.Key, genero.Count()))
            .OrderByDescending(genero => genero.Livros)
            .ThenBy(genero => genero.Genero)
            .Take(5)
            .ToList();

        return new EstatisticasResponse(
            TotalLivros: livros.Count,
            QueroLer: livros.Count(livro => livro.Status == StatusLeitura.QueroLer),
            Lendo: livros.Count(livro => livro.Status == StatusLeitura.Lendo),
            Lidos: lidos.Count,
            Abandonados: livros.Count(livro => livro.Status == StatusLeitura.Abandonado),
            Fisicos: livros.Count(livro => livro.Formato == FormatoLivro.Fisico),
            Virtuais: livros.Count(livro => livro.Formato == FormatoLivro.Virtual),
            PaginasLidas: lidos.Sum(livro => livro.TotalPaginas ?? 0),
            MediaAvaliacao: notas.Count > 0 ? (double?)Math.Round(notas.Average(), 1) : null,
            MediaDiasPorLivro: duracoes.Count > 0 ? (int?)Math.Round(duracoes.Average()) : null,
            LidosSemData: lidos.Count(livro => !livro.DataFimLeitura.HasValue),
            LidosPorAno: lidosPorAno,
            Notas: livrosPorNota,
            GenerosMaisLidos: generosMaisLidos);
    }
}
