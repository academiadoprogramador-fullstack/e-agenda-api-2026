using eAgenda.Aplicacao.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eAgenda.UnitTests;

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
        var resultado = servico.Cadastrar(new("Casa"));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Cadastrar(It.Is<Categoria>(c => c.Titulo == "Casa")), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_titulo_duplicado_ignorando_caixa_e_espacos()
    {
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([new Categoria("Casa")]);

        var resultado = servico.Cadastrar(new(" CASA "));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe uma categoria com este título.", resultado.Errors.Single().Message);
        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_titulo_invalido()
    {
        var resultado = servico.Cadastrar(new("A"));

        Assert.IsTrue(resultado.IsFailed);
        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void Editar_deve_atualizar_categoria()
    {
        Guid id = Guid.CreateVersion7();
        repositorioCategoria.Setup(r => r.Editar(id, It.IsAny<Categoria>())).Returns(true);

        var resultado = servico.Editar(new(id, "Casa atualizada"));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Editar(id, It.Is<Categoria>(c => c.Titulo == "Casa atualizada")), Times.Once);
    }

    [TestMethod]
    public void Editar_deve_retornar_falha_quando_categoria_nao_for_encontrada()
    {
        Guid id = Guid.CreateVersion7();
        repositorioCategoria.Setup(r => r.Editar(id, It.IsAny<Categoria>())).Returns(false);

        var resultado = servico.Editar(new(id, "Casa atualizada"));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Categoria não encontrada.", resultado.Errors.Single().Message);
    }

    [TestMethod]
    public void Excluir_deve_bloquear_categoria_com_despesas_vinculadas()
    {
        Guid id = Guid.CreateVersion7();
        Categoria categoria = new("Casa") { Id = id };
        Despesa despesa = new("Conta", DateTime.Today, 10, FormaPagamento.AVista, [categoria]);
        repositorioCategoria.Setup(r => r.SelecionarPorId(id)).Returns(categoria);
        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns([despesa]);

        var resultado = servico.Excluir(id);

        Assert.IsTrue(resultado.IsFailed);
        repositorioCategoria.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Excluir_deve_persistir_categoria_sem_despesas()
    {
        Guid id = Guid.CreateVersion7();
        Categoria categoria = new("Casa") { Id = id };
        repositorioCategoria.Setup(r => r.SelecionarPorId(id)).Returns(categoria);

        var resultado = servico.Excluir(id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Excluir(id), Times.Once);
    }

    [TestMethod]
    public void SelecionarTodos_deve_mapear_categorias()
    {
        Categoria categoria = new("Casa");
        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([categoria]);

        var resultado = servico.SelecionarTodos();

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(categoria.Id, resultado[0].Id);
        Assert.AreEqual("Casa", resultado[0].Titulo);
    }
}
