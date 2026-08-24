using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloCategoria;

namespace eAgenda.Dominio.Modulos.ModuloDespesa;

public class Despesa : EntidadeBase<Despesa>
{
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; } = DateTime.Today;
    public decimal Valor { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public List<Categoria> Categorias { get; set; } = [];

    public Despesa()
    {
    }

    public Despesa(
        string descricao,
        DateTime dataOcorrencia,
        decimal valor,
        FormaPagamento formaPagamento,
        List<Categoria> categorias
    ) : this()
    {
        Descricao = descricao;
        DataOcorrencia = dataOcorrencia.Date;
        Valor = valor;
        FormaPagamento = formaPagamento;
        Categorias = categorias;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Descricao) || Descricao.Length < 2 || Descricao.Length > 100)
            erros.Add("O campo \"Descrição\" deve conter entre 2 e 100 caracteres.");

        if (DataOcorrencia == default)
            erros.Add("O campo \"Data de Ocorrência\" deve ser preenchido.");

        if (Valor <= 0)
            erros.Add("O campo \"Valor\" deve ser maior que zero.");

        if (!Enum.IsDefined(FormaPagamento))
            erros.Add("O campo \"Forma de Pagamento\" deve ser preenchido.");

        if (Categorias.Count == 0)
            erros.Add("Selecione ao menos uma categoria.");

        return erros;
    }

    public override void Atualizar(Despesa entidadeAtualizada)
    {
        Descricao = entidadeAtualizada.Descricao;
        DataOcorrencia = entidadeAtualizada.DataOcorrencia;
        Valor = entidadeAtualizada.Valor;
        FormaPagamento = entidadeAtualizada.FormaPagamento;
        Categorias = entidadeAtualizada.Categorias;
    }
}
