using MinhaEstante.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Application.Dtos;

public record CadastrarLivroRequest(
    string Titulo,
    string Autor,
    FormatoLivro Formato,
    string? Isbn,
    string? Genero,
    int? TotalPaginas,
    StatusLeitura Status,
    DateOnly? DataInicioLeitura,
    DateOnly? DataFimLeitura,
    int? Avaliacao,
    string? CapaUrl = null);
