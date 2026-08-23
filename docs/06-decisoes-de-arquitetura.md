# Decisões de arquitetura

## DA01 — Arquitetura inicial

**Estado:** proposta aceita para planejamento.

Será utilizado um monólito modular ASP.NET Core para a API, acompanhado de um Worker quando houver processamento assíncrono.

A solução inicial será organizada em quatro projetos de produção:

- `BarberFlow.Api`: camada de apresentação HTTP e ponto de composição da API;
- `BarberFlow.Domain`: entidades, regras, casos de uso, abstrações e contratos internos de mensagens;
- `BarberFlow.Infrastructure`: implementações de persistência, autenticação, mensageria, e-mail e demais integrações;
- `BarberFlow.Worker`: consumidores e tarefas em segundo plano, além do ponto de composição do processamento assíncrono.

Não haverá um projeto `Shared` inicialmente. API e Worker compartilharão os contratos internos por meio de `BarberFlow.Domain`. Caso esses contratos precisem ser distribuídos para sistemas externos ou versionados separadamente, essa decisão será reavaliada.

Tecnologias planejadas:

- .NET;
- ASP.NET Core;
- autenticação e autorização próprias sobre ASP.NET Core;
- PostgreSQL;
- Entity Framework Core;
- RabbitMQ;
- OpenAPI/Swagger;
- Docker;
- testes automatizados.

CI/CD e AWS serão detalhados em uma fase posterior.

## DA02 — Uma única barbearia

**Estado:** aceita.

Não será implementado multi-tenancy. Entidades não precisarão carregar `TenantId` ou `BarbershopId` para isolamento entre empresas.

Essa decisão simplifica autorização, índices, consultas e regras de unicidade, mas torna uma futura conversão para SaaS uma mudança relevante de arquitetura e banco de dados.

## DA03 — RabbitMQ e conflitos de agenda

**Estado:** proposta a validar com protótipo e teste de concorrência.

RabbitMQ poderá serializar solicitações de agendamento quando todas forem encaminhadas para uma única sequência lógica de processamento. Entretanto, ele **não será a única garantia** contra conflitos.

### Motivos

- múltiplos consumidores podem processar mensagens simultaneamente;
- filas particionadas ou futuras mudanças de topologia podem quebrar a ordem global;
- mensagens podem ser repetidas;
- funcionários podem alterar ou cancelar agendamentos por outros fluxos;
- a indisponibilidade do broker não deve comprometer a integridade do banco;
- a regra pertence aos dados e deve sobreviver a qualquer caminho de escrita.

### Decisão proposta

Usar duas camadas complementares:

1. RabbitMQ para enfileirar e desacoplar o processamento da solicitação;
2. PostgreSQL como autoridade final, impedindo sobreposição entre agendamentos confirmados.

Uma validação de disponibilidade também ocorrerá na aplicação para produzir mensagens compreensíveis.

### Proteção no PostgreSQL

A implementação deverá escolher e testar uma destas abordagens:

- restrição de exclusão por funcionário e intervalo de tempo para agendamentos ativos, preferencialmente;
- ou transação com bloqueio por funcionário e validação serializada.

A escolha final será registrada antes da implementação da persistência de agendamentos.

## DA04 — Solicitação versus agendamento

**Estado:** proposta.

Como o fluxo pode ser assíncrono, recomenda-se distinguir:

- `AppointmentRequest`: tentativa recebida, com estados `Pending`, `Accepted` ou `Rejected`;
- `Appointment`: reserva de fato, com estados `Scheduled` ou `Cancelled` inicialmente.

Isso evita usar `Pending` como se o horário já estivesse reservado e evita criar agendamentos “conflitados”.

### Alternativa simplificada

Caso o endpoint permaneça síncrono, a entidade de solicitação poderá ser omitida. Nesse caso, a API tenta confirmar na própria requisição e retorna conflito sem criar registro de agendamento.

## DA05 — Estado de conflito

**Estado:** aceita.

`Conflict` não será estado de agendamento. Um conflito significa que a tentativa falhou.

- No fluxo assíncrono, a solicitação fica `Rejected` com motivo `ScheduleConflict`.
- No fluxo síncrono, a API retorna uma resposta de conflito, sem criar o agendamento.
- Quando necessário, auditoria preserva a tentativa e seu resultado.

## DA06 — Persistência do horário final

**Estado:** aceita.

O agendamento confirmado armazenará `StartsAt` e `EndsAt`.

Os itens armazenarão snapshots de duração e preço. O fim será calculado durante criação ou alteração e persistido para:

- consultar disponibilidade com eficiência;
- criar índices e restrições de intervalo;
- preservar o histórico;
- evitar que uma alteração posterior no serviço mude agendamentos existentes.

## DA07 — Horário de trabalho simples

**Estado:** aceita.

Cada funcionário terá zero ou mais intervalos por dia da semana. Uma pausa de almoço será representada por dois intervalos de trabalho.

Não haverá inicialmente folgas, feriados ou exceções de calendário.

## DA08 — Outbox

**Estado:** aceita.

Toda mudança que exija publicação de evento gravará a mensagem Outbox na mesma transação da mudança de negócio.

Um dispatcher publicará mensagens pendentes no RabbitMQ. Como podem existir republicações, os consumidores deverão ser idempotentes.

## DA09 — Notificações

**Estado:** adiada.

O canal real de notificação será decidido depois. Durante as primeiras fases, eventos e tentativas poderão ser observados por logs estruturados ou por um adaptador falso de desenvolvimento.

## DA10 — Entrega e infraestrutura

**Estado:** adiada.

Os detalhes de GitHub Actions, Docker para produção, ECR, ECS, RDS e infraestrutura como código serão definidos depois que o núcleo de agenda e concorrência estiver validado.

## DA11 — Testes especializados

**Estado:** planejada para a fase de implementação.

Testes de PostgreSQL, concorrência e RabbitMQ serão detalhados junto à implementação correspondente. O critério mínimo de concorrência será provar que várias tentativas simultâneas pelo mesmo intervalo produzem apenas uma confirmação.

## DA12 — Autenticação sem senha

**Estado:** aceita.

O sistema utilizará autenticação passwordless por e-mail. O usuário informará o e-mail e receberá um código numérico de seis dígitos, válido por cinco minutos, para uso único e com no máximo três tentativas de validação. Ao atingir o limite, o código será invalidado e o usuário ficará impedido de validar ou solicitar outro código por três minutos.

A aplicação será responsável pela identidade, papéis, confirmação do e-mail e emissão da credencial de acesso. O ASP.NET Core Identity não será utilizado e não haverá cadastro, validação ou recuperação de senha.

O código será persistido somente de forma protegida. A implementação também aplicará invalidação de códigos anteriores, limitação de frequência e respostas que reduzam o risco de enumeração de usuários.

O formato e o ciclo de vida da credencial emitida após a validação do código serão definidos antes da implementação da camada de autenticação.

## DA13 — Usuário único e autorização por papel

**Estado:** aceita.

Clientes, funcionários e administradores serão armazenados em uma única entidade `User`. A associação `UserRole` determinará as operações permitidas para cada usuário, eliminando as tabelas separadas `ApplicationUser`, `Customer` e `Employee`.

Um usuário com papel `Employee` poderá ser profissional em alguns agendamentos e cliente em outros, sem precisar receber também o papel `Customer`. Entretanto, ele não poderá ser simultaneamente cliente e profissional no mesmo agendamento. Um usuário que possua somente o papel `Customer` nunca poderá ocupar o campo de profissional.

O CPF será uma propriedade opcional de `User` no banco de dados, mas será obrigatório pela regra de domínio sempre que o usuário possuir o papel `Employee`. A unicidade será aplicada aos CPFs preenchidos.

## DA14 — Estrutura da solution e dependências

**Estado:** aceita.

A solution seguirá uma versão simplificada de Clean Architecture com três responsabilidades principais e dois pontos de entrada:

- Presentation, representada por `BarberFlow.Api`;
- Domain, representada por `BarberFlow.Domain`;
- Infrastructure, representada por `BarberFlow.Infrastructure`;
- processamento assíncrono, representado por `BarberFlow.Worker`.

### Direção das dependências

`BarberFlow.Domain` não referencia nenhum outro projeto da solution.

`BarberFlow.Infrastructure` referencia `BarberFlow.Domain` para implementar as abstrações definidas pelo núcleo.

`BarberFlow.Api` referencia `BarberFlow.Domain` para executar casos de uso e `BarberFlow.Infrastructure` para registrar as implementações no container de injeção de dependência.

`BarberFlow.Worker` referencia `BarberFlow.Domain` para executar casos de uso e consumir contratos internos, além de `BarberFlow.Infrastructure` para registrar persistência e mensageria.

```text
BarberFlow.Api ───────────> BarberFlow.Domain
       └─────────────────> BarberFlow.Infrastructure ──> BarberFlow.Domain

BarberFlow.Worker ────────> BarberFlow.Domain
       └─────────────────> BarberFlow.Infrastructure ──> BarberFlow.Domain
```

O domínio não conhecerá ASP.NET Core, Entity Framework Core, PostgreSQL ou RabbitMQ. As abstrações necessárias serão declaradas no domínio e implementadas na infraestrutura.

### Organização física inicial

```text
src/backend/
├── BarberFlow.slnx
├── core/
│   └── BarberFlow.Domain/
├── infrastructure/
│   └── BarberFlow.Infrastructure/
└── services/
    ├── BarberFlow.Api/
    └── BarberFlow.Worker/
```

Dentro da API, os endpoints serão agrupados por funcionalidade. No domínio, entidades, abstrações, casos de uso e mensagens serão separados por responsabilidade. Na infraestrutura ficarão persistência, autenticação, mensageria, e-mail e implementações técnicas. O Worker conterá consumidores, jobs e sua configuração de host.

