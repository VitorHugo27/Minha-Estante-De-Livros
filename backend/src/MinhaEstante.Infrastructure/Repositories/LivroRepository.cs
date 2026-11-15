using Microsoft.EntityFrameworkCore;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Infrastructure.Repositories;

public class LivroRepository(MinhaEstanteDbContext context) : ILivroRepository
{
    public async Task<Livro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Livros.FirstOrDefaultAsync(livro => livro.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Livro>> ListarAsync(StatusLeitura? status, CancellationToken cancellationToken = default)
    {
        var consulta = context.Livros.AsNoTracking();

        if (status.HasValue)
            consulta = consulta.Where(livro => livro.Status == status.Value);

        return await consulta.OrderBy(livro => livro.Titulo).ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        context.Livros.Add(livro);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        context.Livros.Update(livro);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        context.Livros.Remove(livro);
        await context.SaveChangesAsync(cancellationToken);
    }
}
