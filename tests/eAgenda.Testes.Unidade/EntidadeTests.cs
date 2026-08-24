using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eAgenda.UnitTests;

[TestClass]
public sealed class EntidadeTests
{
    [TestMethod]
    public void Contato_deve_rejeitar_email_e_telefone_invalidos()
    {
        Contato contato = new("A", "email-invalido", "000", null, null);

        List<string> erros = contato.Validar();

        Assert.IsTrue(erros.Any(erro => erro.Contains("Nome")));
        Assert.IsTrue(erros.Any(erro => erro.Contains("E-mail")));
        Assert.IsTrue(erros.Any(erro => erro.Contains("Telefone")));
    }

    [TestMethod]
    public void Compromisso_presencial_deve_exigir_local()
    {
        Compromisso compromisso = new(
            "Reunião",
            DateTime.Today,
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            null,
            null,
            null);

        Assert.IsTrue(compromisso.Validar().Any(erro => erro.Contains("Local")));
    }
}
