# Decisões de arquitetura

## DA01 — Arquitetura inicial

**Estado:** proposta aceita para planejamento.

Será utilizado um monólito modular ASP.NET Core para a API, acompanhado de um Worker quando houver processamento assíncrono.

Tecnologias planejadas:

- .NET;
- ASP.NET Core;
- ASP.NET Core Identity;
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

