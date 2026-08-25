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

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Descricao) || Descricao.Length < 2 || Descricao.Length > 100)
            erros.Add(new(nameof(Descricao), "O campo \"Descrição\" deve conter entre 2 e 100 caracteres."));

        if (DataOcorrencia == default)
            erros.Add(new(nameof(DataOcorrencia), "O campo \"Data de Ocorrência\" deve ser preenchido."));

        if (Valor <= 0)
            erros.Add(new(nameof(Valor), "O campo \"Valor\" deve ser maior que zero."));

        if (!Enum.IsDefined(FormaPagamento))
            erros.Add(new(nameof(FormaPagamento), "O campo \"Forma de Pagamento\" deve ser preenchido."));

        if (Categorias.Count == 0)
            erros.Add(new(nameof(Categorias), "Selecione ao menos uma categoria."));

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
