using FluentResults;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using eAgenda.Aplicacao.Compartilhado;

namespace eAgenda.Aplicacao.Modulos.ModuloCompromisso;

public class ServicoCompromisso : ServicoBase<Compromisso>
{
    private readonly IRepositorioCompromisso repositorioCompromisso;
    private readonly IRepositorioContato repositorioContato;

    public ServicoCompromisso(
        IRepositorioCompromisso repositorioCompromisso,
        IRepositorioContato repositorioContato
    )
    {
        this.repositorioCompromisso = repositorioCompromisso;
        this.repositorioContato = repositorioContato;
    }

    public Result<Guid> Cadastrar(CadastrarCompromissoDto dto)
    {
        Result<Contato?> resultadoContato = SelecionarContatoOpcional(dto.ContatoId);

        if (resultadoContato.IsFailed)
            return Result.Fail<Guid>(resultadoContato.Errors);

        Compromisso novoCompromisso = new Compromisso(
            dto.Assunto,
            dto.DataOcorrencia,
            dto.HoraInicio,
            dto.HoraTermino,
            dto.Tipo,
            dto.Local,
            dto.Link,
            resultadoContato.Value
        );

        Result resultadoValidacao = ValidarEntidade(novoCompromisso);

        if (resultadoValidacao.IsFailed)
            return Result.Fail<Guid>(resultadoValidacao.Errors);

        if (ExisteConflitoDeHorario(novoCompromisso))
            return Falha<Guid>(string.Empty, "Já existe um compromisso cadastrado neste intervalo de horário.");

        repositorioCompromisso.Cadastrar(novoCompromisso);

        return Result.Ok(novoCompromisso.Id);
    }

    public Result Editar(EditarCompromissoDto dto)
    {
        Result<Contato?> resultadoContato = SelecionarContatoOpcional(dto.ContatoId);

        if (resultadoContato.IsFailed)
            return resultadoContato.ToResult();

        Compromisso compromissoAtualizado = new Compromisso(
            dto.Assunto,
            dto.DataOcorrencia,
            dto.HoraInicio,
            dto.HoraTermino,
            dto.Tipo,
            dto.Local,
            dto.Link,
            resultadoContato.Value
        );

        Result resultadoValidacao = ValidarEntidade(compromissoAtualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        if (ExisteConflitoDeHorario(compromissoAtualizado, dto.Id))
            return Falha(string.Empty, "Já existe um compromisso cadastrado neste intervalo de horário.");

        bool conseguiuEditar = repositorioCompromisso.Editar(dto.Id, compromissoAtualizado);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Compromisso não encontrado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Compromisso? compromisso = repositorioCompromisso.SelecionarPorId(id);

        if (compromisso == null)
            return Falha(string.Empty, "Compromisso não encontrado.");

        repositorioCompromisso.Excluir(id);

        return Result.Ok();
    }

    public List<ListarCompromissosDto> SelecionarTodos()
    {
        return repositorioCompromisso
            .SelecionarTodos()
            .Select(c => new ListarCompromissosDto(
                c.Id,
                c.Assunto,
                c.DataOcorrencia,
                c.HoraInicio,
                c.HoraTermino,
                c.Tipo,
                c.Local,
                c.Link,
                c.Contato?.Id,
                c.Contato?.Nome
            ))
            .ToList();
    }

    public Result<DetalhesCompromissoDto> SelecionarPorId(Guid id)
    {
        Compromisso? compromisso = repositorioCompromisso.SelecionarPorId(id);

        if (compromisso == null)
            return Result.Fail("Compromisso não encontrado.");

        return Result.Ok(new DetalhesCompromissoDto(
            compromisso.Id,
            compromisso.Assunto,
            compromisso.DataOcorrencia,
            compromisso.HoraInicio,
            compromisso.HoraTermino,
            compromisso.Tipo,
            compromisso.Local,
            compromisso.Link,
            compromisso.Contato?.Id,
            compromisso.Contato?.Nome
        ));
    }

    public List<OpcaoContatoDto> SelecionarContatos()
    {
        return repositorioContato
            .SelecionarTodos()
            .Select(c => new OpcaoContatoDto(c.Id, c.Nome))
            .ToList();
    }

    private Result<Contato?> SelecionarContatoOpcional(Guid? contatoId)
    {
        if (contatoId == null || contatoId == Guid.Empty)
            return Result.Ok<Contato?>(null);

        Contato? contato = repositorioContato.SelecionarPorId(contatoId.Value);

        if (contato == null)
            return Result.Fail<Contato?>(new Error("Selecione um contato válido.").WithMetadata("Campo", nameof(CadastrarCompromissoDto.ContatoId)));

        return Result.Ok<Contato?>(contato);
    }

    private bool ExisteConflitoDeHorario(Compromisso compromisso, Guid? idIgnorado = null)
    {
        return repositorioCompromisso
            .SelecionarTodos()
            .Any(c =>
                c.Id != idIgnorado &&
                c.DataOcorrencia.Date == compromisso.DataOcorrencia.Date &&
                compromisso.HoraInicio < c.HoraTermino &&
                compromisso.HoraTermino > c.HoraInicio
            );
    }
}
