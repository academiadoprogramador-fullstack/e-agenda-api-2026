using eAgenda.Dominio.Modulos.ModuloContato;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infra.Compartilhado.Orm.Config;

public sealed class ContatoConfiguration : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.ToTable("TBContato");

        builder.HasKey(c => c.Id)
            .HasName("PK_TBContato");

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Telefone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Cargo)
            .HasMaxLength(100);

        builder.Property(c => c.Empresa)
            .HasMaxLength(100);

        // Índice de exclusividade
        builder.HasIndex(c => new { c.UsuarioId, c.Email })
            .IsUnique()
            .HasDatabaseName("UQ_TBContato_Email");

        builder.HasIndex(c => new { c.UsuarioId, c.Telefone })
            .IsUnique()
            .HasDatabaseName("UQ_TBContato_Telefone");
    }
}
