using eAgenda.Aplicacao.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eAgenda.Testes.Unidade.Aplicacao;

[TestClass]
public sealed class ServicoCompromissoTests
{
    private Mock<IRepositorioCompromisso> repositorioCompromisso = null!;
    private Mock<IRepositorioContato> repositorioContato = null!;
    private ServicoCompromisso servico = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        repositorioContato = new Mock<IRepositorioContato>();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);

        servico = new ServicoCompromisso(repositorioCompromisso.Object, repositorioContato.Object);
    }

    private static CadastrarCompromissoDto NovoDto(
        Guid? contatoId = null,
        TimeSpan? inicio = null,
        TimeSpan? termino = null) =>
        new(
            "Reuniao semanal",
            DateTime.Today,
            inicio ?? new(9, 0, 0),
            termino ?? new(10, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.test",
            contatoId);

    [TestMethod]
    public void Cadastrar_deve_persistir_compromisso_sem_contato()
    {
        // Arrange
        CadastrarCompromissoDto dto = NovoDto();

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.Is<Compromisso>(c =>
            c.Assunto == "Reuniao semanal" && c.Tipo == TipoCompromisso.Remoto && c.Contato == null)), Times.Once);
        repositorioContato.Verify(r => r.SelecionarPorId(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_resolver_contato_informado()
    {
        // Arrange
        Guid contatoId = Guid.CreateVersion7();
        Contato contato = new(
            "Maria Silva",
            "maria@email.com",
            "(11) 99999-9999",
            null,
            null)
        {
            Id = contatoId
        };
        repositorioContato.Setup(r => r.SelecionarPorId(contatoId)).Returns(contato);
        CadastrarCompromissoDto dto = NovoDto(contatoId);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(r => r.Cadastrar(It.Is<Compromisso>(c => c.Contato == contato)), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_contato_inexistente()
    {
        // Arrange
        Guid contatoId = Guid.CreateVersion7();
        CadastrarCompromissoDto dto = NovoDto(contatoId);

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Selecione um contato válido.", resultado.Errors.Single().Message);
        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_rejeitar_conflito_de_horario_no_mesmo_dia()
    {
        // Arrange
        Compromisso existente = new(
            "Outra reuniao",
            DateTime.Today,
            new(9, 30, 0),
            new(11, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.test",
            null);
        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([existente]);
        CadastrarCompromissoDto dto = NovoDto();

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Já existe um compromisso cadastrado neste intervalo de horário.",
            resultado.Errors.Single().Message);
        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_deve_permitir_compromissos_em_dias_diferentes()
    {
        // Arrange
        Compromisso existente = new(
            "Outra reuniao",
            DateTime.Today.AddDays(1),
            new(9, 0, 0),
            new(11, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.test",
            null);
        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([existente]);
        CadastrarCompromissoDto dto = NovoDto();

        // Act
        var resultado = servico.Cadastrar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Editar_deve_ignorar_o_proprio_compromisso_na_validacao_de_conflito()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        Compromisso existente = new(
            "Reuniao semanal",
            DateTime.Today,
            new(9, 0, 0),
            new(10, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.test",
            null)
        {
            Id = id
        };
        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([existente]);
        repositorioCompromisso.Setup(r => r.Editar(id, It.IsAny<Compromisso>())).Returns(true);

        EditarCompromissoDto dto = new(
            id,
            "Reuniao atualizada",
            DateTime.Today,
            new(9, 0, 0),
            new(10, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.test",
            null);

        // Act
        var resultado = servico.Editar(dto);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(
            r => r.Editar(id, It.Is<Compromisso>(c => c.Assunto == "Reuniao atualizada")),
            Times.Once);
    }

    [TestMethod]
    public void Editar_deve_retornar_falha_quando_compromisso_nao_for_encontrado()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        repositorioCompromisso.Setup(r => r.Editar(id, It.IsAny<Compromisso>())).Returns(false);

        EditarCompromissoDto dto = new(
            id,
            "Reuniao atualizada",
            DateTime.Today,
            new(9, 0, 0),
            new(10, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.test",
            null);

        // Act
        var resultado = servico.Editar(dto);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Compromisso não encontrado.", resultado.Errors.Single().Message);
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
        Assert.AreEqual("Compromisso não encontrado.", resultado.Errors.Single().Message);
        repositorioCompromisso.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarContatos_deve_retornar_opcoes_mapeadas()
    {
        // Arrange
        Contato contato = new(
            "Maria Silva",
            "maria@email.com",
            "(11) 99999-9999",
            null,
            null);
        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([contato]);

        // Act
        var resultado = servico.SelecionarContatos();

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(contato.Id, resultado[0].Id);
        Assert.AreEqual(contato.Nome, resultado[0].Nome);
    }
}
