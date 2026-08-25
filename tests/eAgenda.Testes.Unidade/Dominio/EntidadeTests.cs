using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eAgenda.Testes.Unidade.Dominio;

[TestClass]
public sealed class EntidadeTests
{
    [TestMethod]
    public void Contato_deve_rejeitar_email_e_telefone_invalidos()
    {
        // Arrange
        Contato contato = new("A", "email-invalido", "000", null, null);

        // Act
        IReadOnlyList<ErroValidacao> erros = contato.Validar();

        // Assert
        Assert.IsTrue(erros.Any(erro => erro.Campo == nameof(Contato.Nome)));
        Assert.IsTrue(erros.Any(erro => erro.Campo == nameof(Contato.Email)));
        Assert.IsTrue(erros.Any(erro => erro.Campo == nameof(Contato.Telefone)));
    }

    [TestMethod]
    public void Compromisso_presencial_deve_exigir_local()
    {
        // Arrange
        Compromisso compromisso = new(
            "Reunião",
            DateTime.Today,
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            null,
            null,
            null);

        // Act
        IReadOnlyList<ErroValidacao> erros = compromisso.Validar();

        // Assert
        Assert.IsTrue(erros.Any(erro =>
            erro.Campo == nameof(Compromisso.Local) && erro.Mensagem.Contains("Local")));
    }
}
