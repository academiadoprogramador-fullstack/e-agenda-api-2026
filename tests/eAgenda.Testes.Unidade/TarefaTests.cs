using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eAgenda.UnitTests;

[TestClass]
public sealed class TarefaTests
{
    [TestMethod]
    public void Deve_calcular_percentual_e_conclusao_a_partir_dos_itens()
    {
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta);
        ItemTarefa primeiroItem = new("Criar controllers");
        ItemTarefa segundoItem = new("Documentar rotas");

        tarefa.AdicionarItem(primeiroItem);
        tarefa.AdicionarItem(segundoItem);
        tarefa.AlterarConclusaoItem(primeiroItem.Id, true);

        Assert.AreEqual(50, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);

        tarefa.AlterarConclusaoItem(segundoItem.Id, true);

        Assert.AreEqual(100, tarefa.PercentualConcluido);
        Assert.IsTrue(tarefa.Concluida);
        Assert.IsNotNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void Nao_deve_permitir_conclusao_manual_com_itens()
    {
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Normal);
        tarefa.AdicionarItem(new ItemTarefa("Criar controllers"));

        Assert.IsFalse(tarefa.AlterarConclusaoManual(true));
        Assert.IsFalse(tarefa.Concluida);
    }
}
