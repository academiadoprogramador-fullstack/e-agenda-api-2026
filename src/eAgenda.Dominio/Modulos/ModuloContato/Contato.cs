using System.Text.RegularExpressions;
using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.Modulos.ModuloContato;

public class Contato : EntidadeBase<Contato>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? Empresa { get; set; }

    public Contato()
    {
    }

    public Contato(
        string nome,
        string email,
        string telefone,
        string? cargo,
        string? empresa
    ) : this()
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
        Cargo = cargo;
        Empresa = empresa;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 2 || Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            erros.Add("O campo \"E-mail\" deve conter um endereço de e-mail válido.");

        if (!Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
            erros.Add("O campo \"Telefone\" deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.");

        if (!string.IsNullOrWhiteSpace(Cargo) && Cargo.Length > 100)
            erros.Add("O campo \"Cargo\" deve conter no máximo 100 caracteres.");

        if (!string.IsNullOrWhiteSpace(Empresa) && Empresa.Length > 100)
            erros.Add("O campo \"Empresa\" deve conter no máximo 100 caracteres.");

        return erros;
    }

    public override void Atualizar(Contato entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Email = entidadeAtualizada.Email;
        Telefone = entidadeAtualizada.Telefone;
        Cargo = entidadeAtualizada.Cargo;
        Empresa = entidadeAtualizada.Empresa;
    }
}
