using FluentResults;
using eAgenda.Dominio.Modulos.ModuloContato;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Aplicacao.Compartilhado;

namespace eAgenda.Aplicacao.Modulos.ModuloContato;

public class ServicoContato : ServicoBase<Contato>
{
    private readonly IRepositorioContato repositorioContato;
    private readonly IRepositorioCompromisso repositorioCompromisso;

    public ServicoContato(
        IRepositorioContato repositorioContato,
        IRepositorioCompromisso repositorioCompromisso
    )
    {
        this.repositorioContato = repositorioContato;
        this.repositorioCompromisso = repositorioCompromisso;
    }

    public Result<Guid> Cadastrar(CadastrarContatoDto dto)
    {
        if (ExisteContatoComMesmoEmail(dto.Email))
            return Falha<Guid>(nameof(dto.Email), "Já existe um contato com este email.");

        if (ExisteContatoComMesmoTelefone(dto.Telefone))
            return Falha<Guid>(nameof(dto.Telefone), "Já existe um contato com este telefone.");

        Contato novoContato = new Contato(
            dto.Nome,
            dto.Email,
            dto.Telefone,
            dto.Cargo,
            dto.Empresa
        );

        Result resultadoValidacao = ValidarEntidade(novoContato);

        if (resultadoValidacao.IsFailed)
            return Result.Fail<Guid>(resultadoValidacao.Errors);

        repositorioContato.Cadastrar(novoContato);

        return Result.Ok(novoContato.Id);
    }

    public Result Editar(EditarContatoDto dto)
    {
        if (ExisteContatoComMesmoEmail(dto.Email, dto.Id))
            return Falha(nameof(dto.Email), "Já existe um contato com este email.");

        if (ExisteContatoComMesmoTelefone(dto.Telefone, dto.Id))
            return Falha(nameof(dto.Telefone), "Já existe um contato com este telefone.");

        Contato contatoAtualizado = new Contato(
            dto.Nome,
            dto.Email,
            dto.Telefone,
            dto.Cargo,
            dto.Empresa
        );

        Result resultadoValidacao = ValidarEntidade(contatoAtualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioContato.Editar(dto.Id, contatoAtualizado);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Contato não encontrado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Contato? contato = repositorioContato.SelecionarPorId(id);

        if (contato == null)
            return Falha(string.Empty, "Contato não encontrado.");

        if (PossuiCompromissosVinculados(id))
            return Falha(string.Empty, "Não é possível excluir este contato, pois ele possui compromissos vinculados.");

        repositorioContato.Excluir(id);

        return Result.Ok();
    }

    public List<ListarContatosDto> SelecionarTodos()
    {
        return repositorioContato
            .SelecionarTodos()
            .Select(c => new ListarContatosDto(c.Id, c.Nome, c.Email, c.Telefone, c.Cargo, c.Empresa))
            .ToList();
    }

    public Result<DetalhesContatoDto> SelecionarPorId(Guid id)
    {
        Contato? contato = repositorioContato.SelecionarPorId(id);

        if (contato == null)
            return Result.Fail("Contato não encontrado.");

        return Result.Ok(new DetalhesContatoDto(
            contato.Id,
            contato.Nome,
            contato.Email,
            contato.Telefone,
            contato.Cargo,
            contato.Empresa
        ));
    }

    private bool ExisteContatoComMesmoEmail(string email, Guid? idIgnorado = null)
    {
        string emailNormalizado = NormalizarEmail(email);

        return repositorioContato
            .SelecionarTodos()
            .Any(c =>
                c.Id != idIgnorado &&
                NormalizarEmail(c.Email) == emailNormalizado
            );
    }

    private bool ExisteContatoComMesmoTelefone(string telefone, Guid? idIgnorado = null)
    {
        string telefoneNormalizado = NormalizarTelefone(telefone);

        return repositorioContato
            .SelecionarTodos()
            .Any(c =>
                c.Id != idIgnorado &&
                NormalizarTelefone(c.Telefone) == telefoneNormalizado
            );
    }

    private bool PossuiCompromissosVinculados(Guid contatoId)
    {
        return repositorioCompromisso
            .SelecionarTodos()
            .Any(c => c.Contato?.Id == contatoId);
    }

    private static string NormalizarEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static string NormalizarTelefone(string telefone)
    {
        return new string(telefone.Where(char.IsDigit).ToArray());
    }
}
