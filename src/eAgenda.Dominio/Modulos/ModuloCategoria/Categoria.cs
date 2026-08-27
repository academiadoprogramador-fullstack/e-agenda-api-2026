using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Compartilhado.Identity;
using eAgenda.Dominio.Modulos.ModuloDespesa;

namespace eAgenda.Dominio.Modulos.ModuloCategoria;

public class Categoria : EntidadeBase<Categoria>, IEntidadeDeUsuario
{
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public List<Despesa> Despesas { get; set; } = new List<Despesa>();

    public Categoria()
    {
    }

    public Categoria(string titulo) : this()
    {
        Titulo = titulo;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Length < 2 || Titulo.Length > 100)
            erros.Add(new(nameof(Titulo), "O campo \"Título\" deve conter entre 2 e 100 caracteres."));

        return erros;
    }

    public override void Atualizar(Categoria entidadeAtualizada)
    {
        Titulo = entidadeAtualizada.Titulo;
    }
}
