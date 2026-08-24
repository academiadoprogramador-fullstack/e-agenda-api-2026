using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infra.Compartilhado.Orm.Config;

public sealed class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("TBTarefa");

        builder.HasKey(t => t.Id)
            .HasName("PK_TBTarefa");

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Titulo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Prioridade)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.DataCriacao)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(t => t.DataConclusao)
            .HasColumnType("date");

        builder.Property(t => t.Concluida)
            .IsRequired();

        builder.Property(t => t.PercentualConcluido)
            .IsRequired();

        builder.HasMany(t => t.Itens)
            .WithOne()
            .HasForeignKey("TarefaId")
            .HasConstraintName("FK_TBItemTarefa_TBTarefa")
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
