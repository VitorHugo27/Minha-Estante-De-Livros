using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaEstante.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Infrastructure.Data.Configurations;

public class Configurations : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.ToTable("Livros");

        builder.HasKey(livro => livro.Id);
        builder.Property(livro => livro.Id).ValueGeneratedNever();

        builder.Property(livro => livro.Titulo).HasMaxLength(200);
        builder.Property(livro => livro.Autor).HasMaxLength(150);
        builder.Property(livro => livro.Isbn).HasMaxLength(20);
        builder.Property(livro => livro.Genero).HasMaxLength(50);
        builder.Property(livro => livro.Anotacoes).HasMaxLength(4000);
        builder.Property(livro => livro.CapaUrl).HasMaxLength(1000);
        builder.Property(livro => livro.DataCadastro).HasConversion(
            data => data,
            data => DateTime.SpecifyKind(data, DateTimeKind.Utc));
    }
}
