using eAgenda.Dominio.Modulos.ModuloCategoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infra.Compartilhado.Orm.Config;

public sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("TBCategoria");

        builder.HasKey(c => c.Id)
            .HasName("PK_TBCategoria");

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Titulo)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.Titulo)
            .IsUnique()
            .HasDatabaseName("UQ_TBCategoria_Titulo");
    }
}
