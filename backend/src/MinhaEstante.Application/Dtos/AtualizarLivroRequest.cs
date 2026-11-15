using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Dtos;

public record AtualizarLivroRequest(
    string Titulo,
    string Autor,
    FormatoLivro Formato,
    string? Isbn,
    string? Genero,
    int? TotalPaginas,
    string? CapaUrl);
