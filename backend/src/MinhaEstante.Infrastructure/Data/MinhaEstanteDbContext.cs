using Microsoft.EntityFrameworkCore;
using MinhaEstante.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Infrastructure.Data;

public class MinhaEstanteDbContext(DbContextOptions<MinhaEstanteDbContext> options)
    : DbContext(options)
{
    public DbSet<Livro> Livros => Set<Livro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MinhaEstanteDbContext).Assembly);
    }
}
