using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.Modulos.ModuloTarefa;

public class ItemTarefa : EntidadeBase<ItemTarefa>
{
    public string Titulo { get; set; } = string.Empty;
    public bool Concluido { get; set; }

    public ItemTarefa()
    {
    }

    public ItemTarefa(string titulo) : this()
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

    public override void Atualizar(ItemTarefa entidadeAtualizada)
    {
        Titulo = entidadeAtualizada.Titulo;
        Concluido = entidadeAtualizada.Concluido;
    }
}
