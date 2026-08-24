using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAgenda.Infra.Compartilhado.Orm.Config;

public sealed class ItemTarefaConfiguration : IEntityTypeConfiguration<ItemTarefa>
{
    public void Configure(EntityTypeBuilder<ItemTarefa> builder)
    {
        builder.ToTable("TBItemTarefa");

        builder.HasKey(i => i.Id)
            .HasName("PK_TBItemTarefa");

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Titulo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.Concluido)
            .IsRequired();

        builder.Property<Guid>("TarefaId")
            .IsRequired();
    }
}
