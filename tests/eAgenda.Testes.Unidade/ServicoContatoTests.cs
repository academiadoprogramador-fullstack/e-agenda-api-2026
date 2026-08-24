using eAgenda.Aplicacao.Modulos.ModuloContato;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eAgenda.UnitTests;

[TestClass]
public sealed class ServicoContatoTests
{
    private Mock<IRepositorioContato> repositorioContato = null!;
    private Mock<IRepositorioCompromisso> repositorioCompromisso = null!;
    private ServicoContato servico = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioContato = new Mock<IRepositorioContato>();
        repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);
        servico = new ServicoContato(repositorioContato.Object, repositorioCompromisso.Object);
    }

    [TestMethod]
    public void Cadastrar_deve_persistir_contato_valido()
    {
        CadastrarContatoDto dto = new("Maria Silva", "maria@email.com", "(11) 99999-9999", "Analista", "Empresa");

        var resultado = servico.Cadastrar(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, resultado.Value);
        repositorioContato.Verify(r => r.Cadastrar(It.Is<Contato>(c =>
            c.Nome == dto.Nome && c.Email == dto.Email && c.Telefone == dto.Telefone &&
            c.Cargo == dto.Cargo && c.Empresa == dto.Empresa)), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_email_duplicado_ignorando_caixa_e_espacos()
    {
        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([
            new Contato("Joao Silva", "joao@email.com", "(11) 9999-9999", null, null)
        ]);

        var resultado = servico.Cadastrar(new("Maria Silva", " JOAO@EMAIL.COM ", "(11) 98888-8888", null, null));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe um contato com este email.", resultado.Errors.Single().Message);
        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_telefone_duplicado_ignorando_formatacao()
    {
        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([
            new Contato("Joao Silva", "joao@email.com", "(11) 99999-9999", null, null)
        ]);

        var resultado = servico.Cadastrar(new("Maria Silva", "maria@email.com", "11999999999", null, null));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe um contato com este telefone.", resultado.Errors.Single().Message);
        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_dados_invalidos_sem_persistir()
    {
        var resultado = servico.Cadastrar(new("A", "invalido", "000", null, null));

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message.Contains("Nome")));
        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Editar_deve_atualizar_contato_valido()
    {
        Guid id = Guid.CreateVersion7();
        repositorioContato.Setup(r => r.Editar(id, It.IsAny<Contato>())).Returns(true);

        var resultado = servico.Editar(new(id, "Maria Souza", "maria.souza@email.com", "(11) 99999-9999", null, "Nova Empresa"));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContato.Verify(r => r.Editar(id, It.Is<Contato>(c =>
            c.Nome == "Maria Souza" && c.Email == "maria.souza@email.com" && c.Empresa == "Nova Empresa")), Times.Once);
    }

    [TestMethod]
    public void Editar_deve_retornar_falha_quando_contato_nao_for_encontrado()
    {
        Guid id = Guid.CreateVersion7();
        repositorioContato.Setup(r => r.Editar(id, It.IsAny<Contato>())).Returns(false);

        var resultado = servico.Editar(new(id, "Maria Souza", "maria@email.com", "(11) 99999-9999", null, null));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Contato não encontrado.", resultado.Errors.Single().Message);
    }

    [TestMethod]
    public void Excluir_deve_bloquear_contato_com_compromissos_vinculados()
    {
        Guid id = Guid.CreateVersion7();
        Contato contato = new("Maria Silva", "maria@email.com", "(11) 99999-9999", null, null) { Id = id };
        Compromisso compromisso = new("Reuniao", DateTime.Today, new(9, 0, 0), new(10, 0, 0), TipoCompromisso.Remoto, null, "https://meet.test", contato);
        repositorioContato.Setup(r => r.SelecionarPorId(id)).Returns(contato);
        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([compromisso]);

        var resultado = servico.Excluir(id);

        Assert.IsTrue(resultado.IsFailed);
        repositorioContato.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Excluir_deve_persistir_quando_contato_nao_tem_vinculos()
    {
        Guid id = Guid.CreateVersion7();
        Contato contato = new("Maria Silva", "maria@email.com", "(11) 99999-9999", null, null) { Id = id };
        repositorioContato.Setup(r => r.SelecionarPorId(id)).Returns(contato);

        var resultado = servico.Excluir(id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContato.Verify(r => r.Excluir(id), Times.Once);
    }

    [TestMethod]
    public void SelecionarPorId_deve_mapear_detalhes_do_contato()
    {
        Guid id = Guid.CreateVersion7();
        Contato contato = new("Maria Silva", "maria@email.com", "(11) 99999-9999", "Analista", "Empresa") { Id = id };
        repositorioContato.Setup(r => r.SelecionarPorId(id)).Returns(contato);

        var resultado = servico.SelecionarPorId(id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(id, resultado.Value.Id);
        Assert.AreEqual("Maria Silva", resultado.Value.Nome);
        Assert.AreEqual("Analista", resultado.Value.Cargo);
    }
}
