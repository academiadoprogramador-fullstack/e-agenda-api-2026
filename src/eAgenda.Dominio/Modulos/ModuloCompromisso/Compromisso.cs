using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloContato;

namespace eAgenda.Dominio.Modulos.ModuloCompromisso;

public class Compromisso : EntidadeBase<Compromisso>
{
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; } = DateTime.Today;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraTermino { get; set; }
    public TipoCompromisso Tipo { get; set; }
    public string? Local { get; set; }
    public string? Link { get; set; }
    public Contato? Contato { get; set; }

    public Compromisso()
    {
    }

    public Compromisso(
        string assunto,
        DateTime dataOcorrencia,
        TimeSpan horaInicio,
        TimeSpan horaTermino,
        TipoCompromisso tipo,
        string? local,
        string? link,
        Contato? contato
    ) : this()
    {
        Assunto = assunto;
        DataOcorrencia = dataOcorrencia.Date;
        HoraInicio = horaInicio;
        HoraTermino = horaTermino;
        Tipo = tipo;
        Local = local;
        Link = link;
        Contato = contato;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Assunto) || Assunto.Length < 2 || Assunto.Length > 100)
            erros.Add("O campo \"Assunto\" deve conter entre 2 e 100 caracteres.");

        if (DataOcorrencia == default)
            erros.Add("O campo \"Data de Ocorrência\" deve ser preenchido.");

        if (HoraInicio == default)
            erros.Add("O campo \"Hora de Início\" deve ser preenchido.");

        if (HoraTermino == default)
            erros.Add("O campo \"Hora de Término\" deve ser preenchido.");

        if (HoraTermino <= HoraInicio)
            erros.Add("A hora de término deve ser posterior à hora de início.");

        if (!Enum.IsDefined(Tipo))
            erros.Add("O campo \"Tipo de Compromisso\" deve ser preenchido.");

        if (Tipo == TipoCompromisso.Presencial && string.IsNullOrWhiteSpace(Local))
            erros.Add("O campo \"Local\" deve ser preenchido para compromissos presenciais.");

        if (Tipo == TipoCompromisso.Remoto && string.IsNullOrWhiteSpace(Link))
            erros.Add("O campo \"Link\" deve ser preenchido para compromissos remotos.");

        if (!string.IsNullOrWhiteSpace(Local) && Local.Length > 255)
            erros.Add("O campo \"Local\" deve conter no máximo 255 caracteres.");

        if (!string.IsNullOrWhiteSpace(Link) && Link.Length > 500)
            erros.Add("O campo \"Link\" deve conter no máximo 500 caracteres.");

        return erros;
    }

    public override void Atualizar(Compromisso entidadeAtualizada)
    {
        Assunto = entidadeAtualizada.Assunto;
        DataOcorrencia = entidadeAtualizada.DataOcorrencia;
        HoraInicio = entidadeAtualizada.HoraInicio;
        HoraTermino = entidadeAtualizada.HoraTermino;
        Tipo = entidadeAtualizada.Tipo;
        Local = entidadeAtualizada.Local;
        Link = entidadeAtualizada.Link;
        Contato = entidadeAtualizada.Contato;
    }
}
