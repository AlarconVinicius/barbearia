# Entidades e propriedades

Este documento descreve o modelo de domínio inicial. Os nomes estão em inglês para manter alinhamento com o código C# e com as decisões de arquitetura.

Tipos e tamanhos exatos de colunas serão confirmados durante a implementação das configurações do Entity Framework Core.

## Visão geral

| Entidade | Responsabilidade |
|---|---|
| `User` | Identidade e dados de clientes, funcionários e administradores |
| `AuthenticationCode` | Código temporário de confirmação de e-mail ou login |
| `UserRole` | Associação entre usuário e papel de autorização |
| `Service` | Serviço oferecido pela barbearia |
| `EmployeeService` | Habilitação de um funcionário para executar um serviço |
| `WorkingInterval` | Intervalo semanal de trabalho de um funcionário |
| `AppointmentRequest` | Tentativa idempotente de criar ou alterar um agendamento |
| `Appointment` | Reserva confirmada na agenda |
| `AppointmentItem` | Snapshot de um serviço incluído no agendamento |
| `OutboxMessage` | Evento pendente de publicação |
| `InboxMessage` | Registro de mensagem já consumida |
| `AuditEntry` | Registro de operações relevantes para auditoria |

## Propriedades comuns

As entidades de negócio usarão identificadores `Guid` e, quando aplicável, as seguintes propriedades:

| Propriedade | Tipo | Descrição |
|---|---|---|
| `Id` | `Guid` | Identificador único |
| `CreatedAtUtc` | `DateTimeOffset` | Instante de criação em UTC |
| `UpdatedAtUtc` | `DateTimeOffset?` | Instante da última alteração em UTC |

## User

Representa a identidade autenticável e os dados comuns de clientes, funcionários e administradores. O e-mail será usado como identificador de acesso e não haverá senha.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador da identidade |
| `FullName` | `string` | Sim | Nome completo |
| `Email` | `string` | Sim | E-mail normalizado e único |
| `PhoneNumber` | `string` | Sim | Telefone de contato informado no registro |
| `Cpf` | `string?` | Condicional | CPF normalizado com 11 dígitos; obrigatório para `Employee` |
| `IsActive` | `bool` | Sim | Indica se o usuário pode acessar o sistema |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data de criação |
| `UpdatedAtUtc` | `DateTimeOffset?` | Não | Data da última alteração |

Os papéis iniciais serão `Administrator`, `Employee` e `Customer`. A autenticação, a confirmação do e-mail e a emissão da credencial de acesso serão implementadas pela aplicação.

`Cpf` será nulo para clientes que não o informarem. Antes de atribuir o papel `Employee`, a aplicação deverá exigir e validar o CPF. O banco aplicará unicidade somente aos CPFs preenchidos, preferencialmente por índice único parcial.

## UserRole

Permite que um usuário acumule mais de um papel de autorização.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `UserId` | `Guid` | Sim | Referência a `User` |
| `Role` | `UserRoleType` | Sim | Papel atribuído ao usuário |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data da atribuição |

A chave será composta por `UserId` e `Role`, impedindo papéis duplicados. Um usuário pode acumular papéis, embora `Employee` já seja suficiente para que ele também apareça como cliente em um agendamento.

## AuthenticationCode

Representa um código temporário usado para confirmar o e-mail ou autenticar o usuário.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador do código |
| `UserId` | `Guid` | Sim | Referência ao usuário ao qual o código pertence |
| `Purpose` | `AuthenticationCodePurpose` | Sim | Confirmação de e-mail ou login |
| `CodeHash` | `string` | Sim | Representação protegida do código; nunca o valor em texto puro |
| `ExpiresAtUtc` | `DateTimeOffset` | Sim | Expiração, cinco minutos após a emissão |
| `AttemptCount` | `int` | Sim | Tentativas realizadas, limitada a três |
| `UsedAtUtc` | `DateTimeOffset?` | Não | Instante do uso bem-sucedido |
| `InvalidatedAtUtc` | `DateTimeOffset?` | Não | Instante de invalidação por substituição ou excesso de tentativas |
| `LockedUntilUtc` | `DateTimeOffset?` | Não | Fim do bloqueio de três minutos provocado pelo excesso de tentativas |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Instante da emissão |

O valor gerado conterá exatamente seis dígitos. Um código só é válido quando ainda não foi utilizado ou invalidado, não expirou e possui menos de três tentativas. A terceira tentativa incorreta invalida o código e impede novas validações e emissões para o usuário por três minutos. Encerrado o bloqueio, um novo código pode ser solicitado. A emissão de um novo código invalida os anteriores ainda ativos para o mesmo usuário e propósito.

## Service

Representa um serviço oferecido pela barbearia.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador do serviço |
| `Name` | `string` | Sim | Nome público |
| `Description` | `string?` | Não | Descrição do serviço |
| `Price` | `decimal` | Sim | Preço atual, maior ou igual a zero |
| `DurationMinutes` | `int` | Sim | Duração atual em minutos, maior que zero |
| `IsActive` | `bool` | Sim | Indica se pode ser selecionado |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data de criação |
| `UpdatedAtUtc` | `DateTimeOffset?` | Não | Data da última alteração |

Serviços com histórico não serão excluídos fisicamente. Alterações posteriores não modificam os snapshots existentes em `AppointmentItem`.

## EmployeeService

Entidade associativa que informa quais serviços um funcionário executa.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `EmployeeId` | `Guid` | Sim | Referência a um `User` com papel `Employee` |
| `ServiceId` | `Guid` | Sim | Referência a `Service` |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data da habilitação |

A chave será composta por `EmployeeId` e `ServiceId`, impedindo associações duplicadas.

## WorkingInterval

Representa um intervalo recorrente de trabalho em determinado dia da semana.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador do intervalo |
| `EmployeeId` | `Guid` | Sim | Usuário com papel `Employee` proprietário da jornada |
| `DayOfWeek` | `DayOfWeek` | Sim | Dia da semana |
| `StartsAt` | `TimeOnly` | Sim | Horário local de início |
| `EndsAt` | `TimeOnly` | Sim | Horário local de término |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data de criação |
| `UpdatedAtUtc` | `DateTimeOffset?` | Não | Data da última alteração |

Deve valer `StartsAt < EndsAt`. Intervalos do mesmo funcionário e dia não podem se sobrepor. Jornadas que atravessam a meia-noite não fazem parte da versão inicial.

## AppointmentRequest

Representa uma tentativa recebida para processamento assíncrono. Esta entidade será adotada se a proposta de fluxo assíncrono for confirmada.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador da solicitação |
| `IdempotencyKey` | `string` | Sim | Chave enviada pelo cliente da API |
| `RequestedByUserId` | `Guid` | Sim | Usuário que realizou a operação |
| `CustomerId` | `Guid` | Sim | Usuário cliente; pode possuir papel `Customer` ou `Employee` |
| `EmployeeId` | `Guid` | Sim | Usuário profissional com papel `Employee` |
| `RequestedStartsAtUtc` | `DateTimeOffset` | Sim | Início solicitado em UTC |
| `Type` | `AppointmentRequestType` | Sim | Criação ou alteração |
| `Status` | `AppointmentRequestStatus` | Sim | Estado do processamento |
| `RejectionReason` | `AppointmentRequestRejectionReason?` | Não | Motivo estruturado da rejeição |
| `RejectionDetails` | `string?` | Não | Explicação segura para diagnóstico |
| `AppointmentId` | `Guid?` | Não | Agendamento criado ou alterado quando aceita |
| `ProcessedAtUtc` | `DateTimeOffset?` | Não | Instante da decisão |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data de recebimento |
| `UpdatedAtUtc` | `DateTimeOffset?` | Não | Data da última alteração |

Os serviços pedidos serão armazenados em uma coleção persistida associada à solicitação, com pelo menos `AppointmentRequestId` e `ServiceId`. A modelagem física dessa coleção será fechada junto com o fluxo de alteração.

A combinação entre usuário solicitante e `IdempotencyKey` deve ser única. Repetições retornam o resultado já conhecido.

## Appointment

Representa uma reserva efetivamente confirmada.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador do agendamento |
| `CustomerId` | `Guid` | Sim | Referência ao `User` atendido; papel `Customer` ou `Employee` |
| `EmployeeId` | `Guid` | Sim | Referência ao `User` profissional; papel `Employee` |
| `StartsAtUtc` | `DateTimeOffset` | Sim | Início confirmado em UTC |
| `EndsAtUtc` | `DateTimeOffset` | Sim | Fim confirmado em UTC |
| `Status` | `AppointmentStatus` | Sim | `Scheduled` ou `Cancelled` |
| `CreatedByUserId` | `Guid` | Sim | Usuário responsável pela criação |
| `CancelledAtUtc` | `DateTimeOffset?` | Não | Instante do cancelamento |
| `CancelledByUserId` | `Guid?` | Não | Responsável pelo cancelamento |
| `CancellationReason` | `string?` | Não | Motivo informado |
| `CreatedAtUtc` | `DateTimeOffset` | Sim | Data de criação |
| `UpdatedAtUtc` | `DateTimeOffset?` | Não | Data da última alteração |

Deve valer `StartsAtUtc < EndsAtUtc` e `CustomerId != EmployeeId`. Somente agendamentos `Scheduled` bloqueiam a agenda. O PostgreSQL deverá impedir intervalos sobrepostos para o mesmo `EmployeeId`, preferencialmente com uma restrição de exclusão parcial. A aplicação validará os papéis dos dois usuários, pois uma chave estrangeira isolada não garante essa regra.

## AppointmentItem

Preserva os dados comerciais dos serviços no momento da confirmação.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador do item |
| `AppointmentId` | `Guid` | Sim | Referência ao agendamento |
| `ServiceId` | `Guid` | Sim | Referência histórica ao serviço original |
| `ServiceName` | `string` | Sim | Snapshot do nome |
| `UnitPrice` | `decimal` | Sim | Snapshot do preço |
| `DurationMinutes` | `int` | Sim | Snapshot da duração |

A soma de `DurationMinutes` deve corresponder à diferença entre `StartsAtUtc` e `EndsAtUtc`. O preço total pode ser calculado pela soma de `UnitPrice`.

## OutboxMessage

Armazena eventos na mesma transação das mudanças de negócio.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador único do evento |
| `OccurredAtUtc` | `DateTimeOffset` | Sim | Instante em que o evento ocorreu |
| `Type` | `string` | Sim | Nome versionado do tipo de evento |
| `Payload` | `string` | Sim | Conteúdo JSON serializado |
| `CorrelationId` | `string?` | Não | Identificador de correlação |
| `ProcessedAtUtc` | `DateTimeOffset?` | Não | Instante da publicação confirmada |
| `AttemptCount` | `int` | Sim | Quantidade de tentativas |
| `LastAttemptAtUtc` | `DateTimeOffset?` | Não | Última tentativa de publicação |
| `LastError` | `string?` | Não | Resumo seguro do último erro |

Uma mensagem permanece pendente enquanto `ProcessedAtUtc` for nulo. O dispatcher pode publicá-la mais de uma vez.

## InboxMessage

Garante idempotência no consumo de mensagens.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `MessageId` | `Guid` | Sim | Identificador da mensagem recebida |
| `Consumer` | `string` | Sim | Nome do consumidor |
| `ProcessedAtUtc` | `DateTimeOffset` | Sim | Instante do processamento |

A chave composta por `MessageId` e `Consumer` impede que o mesmo consumidor repita o efeito da mensagem.

## AuditEntry

Registra operações relevantes sem armazenar senhas, tokens ou outros segredos.

| Propriedade | Tipo | Obrigatória | Descrição |
|---|---|---:|---|
| `Id` | `Guid` | Sim | Identificador do registro |
| `OccurredAtUtc` | `DateTimeOffset` | Sim | Instante da ação |
| `UserId` | `Guid?` | Não | Usuário responsável, quando conhecido |
| `Action` | `string` | Sim | Ação executada |
| `EntityType` | `string` | Sim | Tipo do recurso afetado |
| `EntityId` | `string` | Sim | Identificador do recurso |
| `CorrelationId` | `string?` | Não | Correlação com requisição ou mensagem |
| `Data` | `string?` | Não | Metadados JSON permitidos para auditoria |

## Enumerações

### AppointmentStatus

- `Scheduled`: reserva confirmada e ocupando a agenda;
- `Cancelled`: reserva cancelada e sem bloqueio de horário.

### AppointmentRequestStatus

- `Pending`: aguardando processamento;
- `Accepted`: solicitação aplicada com sucesso;
- `Rejected`: solicitação recusada.

### AppointmentRequestType

- `Create`: criação de novo agendamento;
- `Reschedule`: alteração de profissional, serviços ou horário.

### AppointmentRequestRejectionReason

Valores iniciais propostos:

- `ScheduleConflict`;
- `OutsideWorkingHours`;
- `EmployeeInactive`;
- `ServiceInactive`;
- `EmployeeNotQualified`;
- `InvalidStartTime`;
- `AppointmentNotChangeable`;
- `Unauthorized`.

### AuthenticationCodePurpose

- `EmailConfirmation`: confirmação do e-mail após o registro;
- `Login`: autenticação de usuário com e-mail já confirmado.

### UserRoleType

- `Administrator`: acesso administrativo;
- `Employee`: operação profissional da agenda;
- `Customer`: operações do cliente sobre os próprios recursos.

## Relacionamentos

- `User` possui um ou mais papéis por meio de `UserRole`;
- `User` possui vários `AuthenticationCode` ao longo do tempo;
- um `User` com papel `Employee` possui vários `WorkingInterval`;
- um `User` com papel `Employee` executa vários `Service` por meio de `EmployeeService`;
- `Appointment.CustomerId` e `Appointment.EmployeeId` referenciam `User`, mas devem ser diferentes;
- `Appointment` possui um ou mais `AppointmentItem`;
- `AppointmentRequest` referencia o solicitante, o cliente, o funcionário e, após aceitação, um `Appointment`;
- `OutboxMessage`, `InboxMessage` e `AuditEntry` são entidades de suporte e não fazem parte do agregado de agendamento.

## Decisões ainda pendentes

- confirmar se `AppointmentRequest` será usada em criação e alteração;
- modelar definitivamente os serviços de uma solicitação;
- definir como uma alteração assíncrona preserva a reserva atual até a confirmação da nova;
- definir limites de tamanho das propriedades textuais;
- definir a precisão de valores monetários;
- definir política de retenção de Outbox, Inbox e auditoria;
- confirmar o identificador de fuso da aplicação e o tratamento de horários locais inválidos ou ambíguos;
- definir se a auditoria será uma entidade própria, eventos de domínio ou uma combinação dos dois.
