using eAgenda.Aplicacao.Modulos.ModuloTarefa;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eAgenda.Testes.Unidade.Aplicacao;

[TestClass]
public sealed class ServicoTarefaTests
{
    private Mock<IRepositorioTarefa> repositorioTarefa = null!;
    private ServicoTarefa servico = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioTarefa = new Mock<IRepositorioTarefa>();
        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns([]);
        servico = new ServicoTarefa(repositorioTarefa.Object);
    }

    [TestMethod]
    public void Cadastrar_deve_persistir_tarefa_valida()
    {
        // Arrange
        CadastrarTarefaDto dto = new("Publicar API", PrioridadeTarefa.Alta);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioTarefa.Verify(r => r.Cadastrar(It.Is<Tarefa>(t =>
            t.Titulo == "Publicar API" && t.Prioridade == PrioridadeTarefa.Alta &&
            t.PercentualConcluido == 0 && !t.Concluida)), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_titulo_invalido_sem_persistir()
    {
        // Arrange
        CadastrarTarefaDto dto = new("A", PrioridadeTarefa.Normal);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        repositorioTarefa.Verify(r => r.Cadastrar(It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void Editar_deve_alterar_titulo_e_prioridade_da_tarefa()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        Tarefa tarefa = new("Titulo antigo", PrioridadeTarefa.Baixa) { Id = id };
        repositorioTarefa.Setup(r => r.SelecionarPorId(id)).Returns(tarefa);
        repositorioTarefa.Setup(r => r.Editar(id, It.IsAny<Tarefa>())).Returns(true);
        EditarTarefaDto dto = new(id, "Titulo novo", PrioridadeTarefa.Alta);

        // Act
        var resultado = servico.Editar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual("Titulo novo", tarefa.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefa.Prioridade);
        repositorioTarefa.Verify(r => r.Editar(id, It.Is<Tarefa>(t =>
            t.Titulo == "Titulo novo" && t.Prioridade == PrioridadeTarefa.Alta)), Times.Once);
    }

    [TestMethod]
    public void AdicionarItem_deve_atualizar_tarefa_e_percentual()
    {
        // Arrange
        Guid tarefaId = Guid.CreateVersion7();
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta) { Id = tarefaId };
        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaId)).Returns(tarefa);
        AdicionarItemTarefaDto dto = new(tarefaId, "Criar controllers");

        // Act
        var resultado = servico.AdicionarItem(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(1, tarefa.Itens.Count);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
        repositorioTarefa.Verify(r => r.Editar(tarefaId, It.Is<Tarefa>(t =>
            t.Itens.Count == 1 && t.Itens[0].Titulo == "Criar controllers")), Times.Once);
    }

    [TestMethod]
    public void AdicionarItem_deve_rejeitar_item_invalido_sem_persistir()
    {
        // Arrange
        Guid tarefaId = Guid.CreateVersion7();
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta) { Id = tarefaId };
        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaId)).Returns(tarefa);
        AdicionarItemTarefaDto dto = new(tarefaId, "A");

        // Act
        var resultado = servico.AdicionarItem(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(0, tarefa.Itens.Count);
        repositorioTarefa.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void AlterarConclusaoItem_deve_recalcular_percentual()
    {
        // Arrange
        Guid tarefaId = Guid.CreateVersion7();
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta) { Id = tarefaId };
        ItemTarefa item = new("Criar controllers");
        tarefa.AdicionarItem(item);
        tarefa.AdicionarItem(new("Documentar rotas"));
        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaId)).Returns(tarefa);
        AlterarConclusaoItemTarefaDto dto = new(tarefaId, item.Id, true);

        // Act
        var resultado = servico.AlterarConclusaoItem(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(50, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
        repositorioTarefa.Verify(r => r.Editar(tarefaId, tarefa), Times.Once);
    }

    [TestMethod]
    public void AlterarConclusao_deve_rejeitar_tarefa_com_itens()
    {
        // Arrange
        Guid tarefaId = Guid.CreateVersion7();
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta) { Id = tarefaId };
        tarefa.AdicionarItem(new("Criar controllers"));
        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaId)).Returns(tarefa);
        AlterarConclusaoTarefaDto dto = new(tarefaId, true);

        // Act
        var resultado = servico.AlterarConclusao(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        repositorioTarefa.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void AlterarConclusao_deve_concluir_tarefa_sem_itens()
    {
        // Arrange
        Guid tarefaId = Guid.CreateVersion7();
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta) { Id = tarefaId };
        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaId)).Returns(tarefa);
        AlterarConclusaoTarefaDto dto = new(tarefaId, true);

        // Act
        var resultado = servico.AlterarConclusao(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(tarefa.Concluida);
        Assert.AreEqual(100, tarefa.PercentualConcluido);
        repositorioTarefa.Verify(r => r.Editar(tarefaId, tarefa), Times.Once);
    }

    [TestMethod]
    public void RemoverItem_deve_persistir_tarefa_sem_item_removido()
    {
        // Arrange
        Guid tarefaId = Guid.CreateVersion7();
        Tarefa tarefa = new("Publicar API", PrioridadeTarefa.Alta) { Id = tarefaId };
        ItemTarefa item = new("Criar controllers");
        tarefa.AdicionarItem(item);
        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaId)).Returns(tarefa);
        RemoverItemTarefaDto dto = new(tarefaId, item.Id);

        // Act
        var resultado = servico.RemoverItem(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(0, tarefa.Itens.Count);
        repositorioTarefa.Verify(r => r.Editar(tarefaId, tarefa), Times.Once);
    }

    [TestMethod]
    public void SelecionarTodos_deve_filtrar_tarefas_pendentes_e_concluidas()
    {
        // Arrange
        Tarefa pendente = new("Pendente", PrioridadeTarefa.Normal);
        Tarefa concluida = new("Concluida", PrioridadeTarefa.Normal);
        concluida.AlterarConclusaoManual(true);
        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns([pendente, concluida]);

        // Act
        var pendentes = servico.SelecionarTodos("Pendentes");
        var concluidas = servico.SelecionarTodos("Concluidas");

        // Assert
        Assert.AreEqual(1, pendentes.Count);
        Assert.AreEqual("Pendente", pendentes[0].Titulo);
        Assert.AreEqual(1, concluidas.Count);
        Assert.AreEqual("Concluida", concluidas[0].Titulo);
    }
}
