using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eAgenda.Testes.Unidade.Dominio;

[TestClass]
public sealed class TarefaTests
{
    [TestMethod]
    public void Deve_calcular_percentual_e_conclusao_a_partir_dos_itens()
    {
        // Arrange
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta);
        ItemTarefa primeiroItem = new("Criar controllers");
        ItemTarefa segundoItem = new("Documentar rotas");

        tarefa.AdicionarItem(primeiroItem);
        tarefa.AdicionarItem(segundoItem);

        // Act
        tarefa.AlterarConclusaoItem(primeiroItem.Id, true);

        // Assert
        Assert.AreEqual(50, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);

        // Act
        tarefa.AlterarConclusaoItem(segundoItem.Id, true);

        // Assert
        Assert.AreEqual(100, tarefa.PercentualConcluido);
        Assert.IsTrue(tarefa.Concluida);
        Assert.IsNotNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void Nao_deve_permitir_conclusao_manual_com_itens()
    {
        // Arrange
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Normal);
        tarefa.AdicionarItem(new ItemTarefa("Criar controllers"));

        // Act
        bool conseguiuConcluir = tarefa.AlterarConclusaoManual(true);

        // Assert
        Assert.IsFalse(conseguiuConcluir);
        Assert.IsFalse(tarefa.Concluida);
    }
}
