using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Application.Dtos;

public record LivroResponse(
    Guid Id,
    string Titulo,
    string Autor,
    FormatoLivro Formato,
    string? Isbn,
    string? Genero,
    int? TotalPaginas,
    string? CapaUrl,
    StatusLeitura Status,
    DateOnly? DataInicioLeitura,
    DateOnly? DataFimLeitura,
    int? Avaliacao,
    string? Anotacoes,
    DateTime DataCadastro)
{
    public static LivroResponse DeEntidade(Livro livro) => new(
        livro.Id, livro.Titulo, livro.Autor, livro.Formato,
        livro.Isbn, livro.Genero, livro.TotalPaginas, livro.CapaUrl, livro.Status,
        livro.DataInicioLeitura, livro.DataFimLeitura,
        livro.Avaliacao, livro.Anotacoes, livro.DataCadastro);
}