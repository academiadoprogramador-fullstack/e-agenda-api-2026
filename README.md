# e-Agenda

## Projeto

Desenvolvido durante o curso Fullstack da [Academia do Programador](https://www.academiadoprogramador.net) 2026

## Funcionalidades

### 1. Módulo de Contatos

Requisitos Funcionais

- O sistema deve permitir a inserção de novos contatos
- O sistema deve permitir a edição de contatos já cadastrados
- O sistema deve permitir excluir contatos já cadastrados
- O sistema deve permitir visualizar contatos cadastrados

Regras de Negócio

Campos obrigatórios:

- Nome (2-100 caracteres)
- Email (formato válido)
- Telefone (formato validado: (XX) XXXX-XXXX ou (XX) XXXXX-XXXX)
- Cargo (opcional)
- Empresa (opcional)

- Não pode haver contatos com o mesmo email e/ou telefone.
- Não permitir excluir um contato caso tenha compromissos vinculados

### 2. Módulo de Compromissos

Requisitos Funcionais

- O sistema deve permitir a inserção de novos compromissos
- O sistema deve permitir a edição de compromissos já cadastrados
- O sistema deve permitir excluir compromissos já cadastrados
- O sistema deve permitir visualizar compromissos cadastrados

Regras de Negócio

Campos obrigatórios:

- Assunto (2-100 caracteres)
- Data de Ocorrência
- Hora de Início
- Hora de Término
- Tipo de Compromisso (Remoto ou Presencial)
- Local (caso presencial)
- Link (caso remoto)
- Contato (opcional)

- Não pode haver conflito de horários entre compromissos

### 3. Módulo de Categorias

Requisitos Funcionais

- O sistema deve permitir cadastrar novas categorias
- O sistema deve permitir editar categorias existentes
- O sistema deve permitir excluir categorias
- O sistema deve permitir visualizar todas as categorias
- O sistema deve permitir visualizar todas as despesas pertencentes a uma categoria
  específica

Regras de Negócio

Campos obrigatórios:

- Título (2-100 caracteres)
- Despesas (cadastradas posteriormente)
- Não pode haver categorias com mesmo título
- Não deve permitir excluir categorias relacionadas a despesas.

### 4. Módulo de Despesas

Requisitos Funcionais

- O sistema deve permitir cadastrar novas despesas
- O sistema deve permitir editar despesas existentes
- O sistema deve permitir excluir despesas
- O sistema deve permitir visualizar todas as despesas

Regras de Negócio

Campos obrigatórios:

- Descrição (2-100 caracteres)
- Data de Ocorrência (opcional, data de cadastro por padrão)
- Valor (R$)
- Forma de Pagamento (À Vista, Crédito ou Débito),
- Categorias (1 ou mais categorias)

### 5. Módulo de Tarefas

Requisitos Funcionais

- O sistema deve permitir cadastrar novas tarefas
- O sistema deve permitir editar tarefas existentes
- O sistema deve permitir excluir tarefas
- O sistema deve permitir visualizar todas as tarefas, as tarefas pendentes e as
  tarefas concluídas
- O sistema deve permitir visualizar as tarefas agrupadas por prioridade

Regras de Negócio

Campos obrigatórios:

- Título (2-100 caracteres)
- Prioridade (Baixa, Normal, Alta)
- Data de Criação
- Data de Conclusão
- Status de Conclusão
- Percentual Concluído,
- Itens da Tarefa (opcionais)

### 5.1 Itens de Tarefas

Requisitos Funcionais

- O sistema deve permitir adicionar ou remover itens em uma determinada tarefa
- O sistema deve permitir concluir itens de tarefas, alterando o percentual (%) de
  conclusão da tarefa.

Regras de Negócio:

Campos obrigatórios:

- Título (2-100 caracteres)
- Status de Conclusão
- Tarefa

---

## Como utilizar

1. Clone o repositório ou baixe o código fonte.
2. Abra o terminal ou o prompt de comando e navegue até a pasta raiz
3. Utilize o comando abaixo para restaurar as dependências do projeto.

   ```bash
   dotnet restore
   ```

4. Para executar o projeto compilando em tempo real

 ```bash
   dotnet run --project src/eAgenda.WebApp
   ```

## WebApi

Para executar a API, configure a connection string `ConnectionStrings:SqlServerEF` no ambiente
utilizado e execute:

```bash
dotnet run --project src/eAgenda.WebApi
```

Em desenvolvimento, as migrations são aplicadas automaticamente. O documento OpenAPI fica
disponível em `/openapi/v1.json`.

### Rotas principais

```text
GET|POST                 /api/contatos
GET|PUT|DELETE           /api/contatos/{id}
GET|POST                 /api/compromissos
GET|PUT|DELETE           /api/compromissos/{id}
GET|POST                 /api/categorias
GET|PUT|DELETE           /api/categorias/{id}
GET|POST                 /api/despesas
GET|PUT|DELETE           /api/despesas/{id}
GET|POST                 /api/tarefas
GET|PUT|DELETE           /api/tarefas/{id}
POST                    /api/tarefas/{tarefaId}/itens
PATCH                   /api/tarefas/{tarefaId}/conclusao
PATCH                   /api/tarefas/{tarefaId}/itens/{itemId}/conclusao
DELETE                  /api/tarefas/{tarefaId}/itens/{itemId}
```

As respostas de erro seguem o formato `ProblemDetails`. A API não possui autenticação nesta
primeira versão.

## Requisitos

- .NET 10.0 SDK
