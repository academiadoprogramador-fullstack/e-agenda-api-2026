using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.Modulos.ModuloTarefa;

public class Tarefa : EntidadeBase<Tarefa>
{
    public string Titulo { get; set; } = string.Empty;
    public PrioridadeTarefa Prioridade { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Today;
    public DateTime? DataConclusao { get; set; }
    public bool Concluida { get; set; }
    public int PercentualConcluido { get; set; }
    public List<ItemTarefa> Itens { get; set; } = [];

    public Tarefa()
    {
    }

    public Tarefa(string titulo, PrioridadeTarefa prioridade) : this()
    {
        Titulo = titulo;
        Prioridade = prioridade;
        DataCriacao = DateTime.Today;
        RecalcularConclusao();
    }

    public void AdicionarItem(ItemTarefa item)
    {
        Itens.Add(item);
        RecalcularConclusao();
    }

    public bool RemoverItem(Guid itemId)
    {
        ItemTarefa? item = Itens.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
            return false;

        Itens.Remove(item);
        RecalcularConclusao();

        return true;
    }

    public bool AlterarConclusaoItem(Guid itemId, bool concluido)
    {
        ItemTarefa? item = Itens.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
            return false;

        item.Concluido = concluido;
        RecalcularConclusao();

        return true;
    }

    public bool AlterarConclusaoManual(bool concluida)
    {
        if (Itens.Count > 0)
            return false;

        Concluida = concluida;
        PercentualConcluido = concluida ? 100 : 0;
        DataConclusao = concluida ? DateTime.Today : null;

        return true;
    }

    public void RecalcularConclusao()
    {
        if (Itens.Count == 0)
        {
            PercentualConcluido = 0;
            Concluida = false;
            DataConclusao = null;

            return;
        }

        int itensConcluidos = Itens.Count(i => i.Concluido);

        PercentualConcluido = (int)Math.Round((decimal)itensConcluidos / Itens.Count * 100);
        Concluida = PercentualConcluido == 100;
        DataConclusao = Concluida ? DataConclusao ?? DateTime.Today : null;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Length < 2 || Titulo.Length > 100)
            erros.Add("O campo \"Título\" deve conter entre 2 e 100 caracteres.");

        if (!Enum.IsDefined(Prioridade))
            erros.Add("O campo \"Prioridade\" deve ser preenchido.");

        if (DataCriacao == default)
            erros.Add("O campo \"Data de Criação\" deve ser preenchido.");

        foreach (ItemTarefa item in Itens)
            erros.AddRange(item.Validar());

        return erros;
    }

    public override void Atualizar(Tarefa entidadeAtualizada)
    {
        Titulo = entidadeAtualizada.Titulo;
        Prioridade = entidadeAtualizada.Prioridade;
        DataCriacao = entidadeAtualizada.DataCriacao;
        DataConclusao = entidadeAtualizada.DataConclusao;
        Concluida = entidadeAtualizada.Concluida;
        PercentualConcluido = entidadeAtualizada.PercentualConcluido;
        Itens = entidadeAtualizada.Itens;
    }
}
