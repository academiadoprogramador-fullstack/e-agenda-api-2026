using eAgenda.Aplicacao.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eAgenda.Testes.Unidade.Aplicacao;

[TestClass]
public sealed class ServicoCategoriaTests
{
    private Mock<IRepositorioCategoria> repositorioCategoria = null!;
    private Mock<IRepositorioDespesa> repositorioDespesa = null!;
    private ServicoCategoria servico = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioCategoria = new Mock<IRepositorioCategoria>();
        repositorioDespesa = new Mock<IRepositorioDespesa>();
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns([]);
        servico = new ServicoCategoria(repositorioCategoria.Object, repositorioDespesa.Object);
    }

    [TestMethod]
    public void Cadastrar_deve_persistir_categoria_valida()
    {
        // Arrange
        CadastrarCategoriaDto dto = new("Casa");

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Cadastrar(It.Is<Categoria>(c => c.Titulo == "Casa")), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_titulo_duplicado_ignorando_caixa_e_espacos()
    {
        // Arrange
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([new Categoria("Casa")]);
        CadastrarCategoriaDto dto = new(" CASA ");

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe uma categoria com este título.", resultado.Errors.Single().Message);
        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_titulo_invalido()
    {
        // Arrange
        CadastrarCategoriaDto dto = new("A");

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void Editar_deve_atualizar_categoria()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        repositorioCategoria.Setup(r => r.Editar(id, It.IsAny<Categoria>())).Returns(true);
        EditarCategoriaDto dto = new(id, "Casa atualizada");

        // Act
        var resultado = servico.Editar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(
            r => r.Editar(id, It.Is<Categoria>(c => c.Titulo == "Casa atualizada")),
            Times.Once);
    }

    [TestMethod]
    public void Editar_deve_retornar_falha_quando_categoria_nao_for_encontrada()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        repositorioCategoria.Setup(r => r.Editar(id, It.IsAny<Categoria>())).Returns(false);
        EditarCategoriaDto dto = new(id, "Casa atualizada");

        // Act
        var resultado = servico.Editar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Categoria não encontrada.", resultado.Errors.Single().Message);
    }

    [TestMethod]
    public void Excluir_deve_bloquear_categoria_com_despesas_vinculadas()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        Categoria categoria = new("Casa") { Id = id };
        Despesa despesa = new(
            "Conta",
            DateTime.Today,
            10,
            FormaPagamento.AVista,
            [categoria]);
        repositorioCategoria.Setup(r => r.SelecionarPorId(id)).Returns(categoria);
        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns([despesa]);

        // Act
        var resultado = servico.Excluir(id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        repositorioCategoria.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Excluir_deve_persistir_categoria_sem_despesas()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        Categoria categoria = new("Casa") { Id = id };
        repositorioCategoria.Setup(r => r.SelecionarPorId(id)).Returns(categoria);

        // Act
        var resultado = servico.Excluir(id);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Excluir(id), Times.Once);
    }

    [TestMethod]
    public void SelecionarTodos_deve_mapear_categorias()
    {
        // Arrange
        Categoria categoria = new("Casa");
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([categoria]);

        // Act
        var resultado = servico.SelecionarTodos();

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(categoria.Id, resultado[0].Id);
        Assert.AreEqual("Casa", resultado[0].Titulo);
    }
}
