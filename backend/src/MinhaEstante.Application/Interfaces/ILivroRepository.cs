using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Application.Interfaces;

public interface ILivroRepository
{
    Task<Livro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Livro>> ListarAsync(StatusLeitura? status, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Livro livro, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Livro livro, CancellationToken cancellationToken = default);
    Task RemoverAsync(Livro livro, CancellationToken cancellationToken= default);
}
