using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloDespesa;

namespace eAgenda.Dominio.Modulos.ModuloCategoria;

public class Categoria : EntidadeBase<Categoria>
{
    public string Titulo { get; set; } = string.Empty;
    public List<Despesa> Despesas { get; set; } = new List<Despesa>();

    public Categoria()
    {
    }

    public Categoria(string titulo) : this()
    {
        Titulo = titulo;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Length < 2 || Titulo.Length > 100)
            erros.Add("O campo \"Título\" deve conter entre 2 e 100 caracteres.");

        return erros;
    }

    public override void Atualizar(Categoria entidadeAtualizada)
    {
        Titulo = entidadeAtualizada.Titulo;
    }
}
