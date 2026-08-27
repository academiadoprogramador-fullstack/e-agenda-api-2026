using eAgenda.Dominio.Modulos.ModuloCompromisso;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infra.Compartilhado.Orm.Config;

public sealed class CompromissoConfiguration : IEntityTypeConfiguration<Compromisso>
{
    public void Configure(EntityTypeBuilder<Compromisso> builder)
    {
        builder.ToTable("TBCompromisso");

        builder.HasKey(c => c.Id)
            .HasName("PK_TBCompromisso");

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Assunto)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DataOcorrencia)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.HoraInicio)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(c => c.HoraTermino)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(c => c.Tipo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.Local)
            .HasMaxLength(255);

        builder.Property(c => c.Link)
            .HasMaxLength(500);

        builder.HasOne(c => c.Contato)
            .WithMany()
            .HasForeignKey("ContatoId")
            .HasConstraintName("FK_TBCompromisso_TBContato")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
