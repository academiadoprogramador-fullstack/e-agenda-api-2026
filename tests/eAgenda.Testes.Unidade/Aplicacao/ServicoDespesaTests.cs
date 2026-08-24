using eAgenda.Aplicacao.Modulos.ModuloDespesa;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eAgenda.Testes.Unidade.Aplicacao;

[TestClass]
public sealed class ServicoDespesaTests
{
    private Mock<IRepositorioDespesa> repositorioDespesa = null!;
    private Mock<IRepositorioCategoria> repositorioCategoria = null!;
    private ServicoDespesa servico = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioDespesa = new Mock<IRepositorioDespesa>();
        repositorioCategoria = new Mock<IRepositorioCategoria>();
        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([]);
        servico = new ServicoDespesa(repositorioDespesa.Object, repositorioCategoria.Object);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_despesa_sem_categorias()
    {
        // Arrange
        CadastrarDespesaDto dto = new(
            "Conta de luz",
            null,
            100,
            FormaPagamento.AVista,
            []);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Selecione ao menos uma categoria.", resultado.Errors.Single().Message);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_categoria_inexistente()
    {
        // Arrange
        CadastrarDespesaDto dto = new(
            "Conta de luz",
            null,
            100,
            FormaPagamento.AVista,
            [Guid.CreateVersion7()]);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Selecione apenas categorias válidas.", resultado.Errors.Single().Message);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_remover_ids_de_categoria_vazios_e_repetidos()
    {
        // Arrange
        Categoria categoria = new("Moradia");
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([categoria]);
        CadastrarDespesaDto dto = new(
            "Conta de luz",
            null,
            100,
            FormaPagamento.AVista,
            [Guid.Empty, categoria.Id, categoria.Id]);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioDespesa.Verify(r => r.Cadastrar(It.Is<Despesa>(d =>
            d.Categorias.Count == 1 && d.Categorias[0].Id == categoria.Id &&
            d.DataOcorrencia == DateTime.Today)), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_dados_invalidos_sem_persistir()
    {
        // Arrange
        Categoria categoria = new("Moradia");
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([categoria]);
        CadastrarDespesaDto dto = new(
            "A",
            null,
            0,
            FormaPagamento.AVista,
            [categoria.Id]);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Descrição")));
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Editar_deve_retornar_falha_quando_despesa_nao_for_encontrada()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        Categoria categoria = new("Moradia");
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([categoria]);
        repositorioDespesa.Setup(r => r.Editar(id, It.IsAny<Despesa>())).Returns(false);
        EditarDespesaDto dto = new(
            id,
            "Conta de luz",
            DateTime.Today,
            100,
            FormaPagamento.Credito,
            [categoria.Id]);

        // Act
        var resultado = servico.Editar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Despesa não encontrada.", resultado.Errors.Single().Message);
    }

    [TestMethod]
    public void Excluir_deve_verificar_existencia_antes_de_excluir()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        var resultado = servico.Excluir(id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Despesa não encontrada.", resultado.Errors.Single().Message);
        repositorioDespesa.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarTodos_por_categoria_deve_usar_predicado_do_repositorio()
    {
        // Arrange
        Categoria categoriaAlvo = new("Moradia");
        Categoria outraCategoria = new("Lazer");
        Despesa despesaAlvo = new(
            "Aluguel",
            DateTime.Today,
            1000,
            FormaPagamento.AVista,
            [categoriaAlvo]);
        Despesa outraDespesa = new(
            "Cinema",
            DateTime.Today,
            50,
            FormaPagamento.Credito,
            [outraCategoria]);
        repositorioDespesa.Setup(r => r.Filtrar(It.IsAny<Func<Despesa, bool>>()))
            .Returns((Func<Despesa, bool> filtro) => new[] { despesaAlvo, outraDespesa }.Where(filtro).ToList());

        // Act
        var resultado = servico.SelecionarTodos(categoriaAlvo.Id);

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(despesaAlvo.Id, resultado[0].Id);
        repositorioDespesa.Verify(r => r.Filtrar(It.IsAny<Func<Despesa, bool>>()), Times.Once);
    }

    [TestMethod]
    public void SelecionarTodos_sem_categoria_deve_listar_todas_as_despesas()
    {
        // Arrange
        Despesa despesa = new(
            "Aluguel",
            DateTime.Today,
            1000,
            FormaPagamento.AVista,
            [new Categoria("Moradia")]);
        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns([despesa]);

        // Act
        var resultado = servico.SelecionarTodos(Guid.Empty);

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(despesa.Descricao, resultado[0].Descricao);
        repositorioDespesa.Verify(r => r.Filtrar(It.IsAny<Func<Despesa, bool>>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarCategorias_deve_retornar_opcoes_mapeadas()
    {
        // Arrange
        Categoria categoria = new("Moradia");
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([categoria]);

        // Act
        var resultado = servico.SelecionarCategorias();

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(categoria.Id, resultado[0].Id);
        Assert.AreEqual("Moradia", resultado[0].Titulo);
    }
}
