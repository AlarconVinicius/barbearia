# Casos de uso

## UC01 — Cadastrar funcionário

**Ator:** Administrador.

**Pré-condição:** administrador autenticado.

### Fluxo principal

1. Administrador informa os dados do funcionário.
2. Administrador seleciona os serviços que ele executa.
3. Sistema valida os dados e a unicidade necessária.
4. Sistema cria o perfil profissional.
5. Sistema registra a operação.

### Alternativas

- Dados inválidos: sistema rejeita e informa os campos incorretos.
- Identidade já utilizada: sistema não cria duplicidade.

## UC02 — Configurar horário de trabalho

**Ator:** Administrador.

### Fluxo principal

1. Administrador seleciona o funcionário.
2. Seleciona o dia da semana.
3. Informa um ou mais intervalos.
4. Sistema valida início, fim e sobreposições.
5. Sistema salva a configuração.

### Exemplo

Para segunda-feira:

- `09:00–12:00`;
- `13:00–18:00`.

## UC03 — Consultar disponibilidade

**Ator:** Cliente ou Funcionário.

### Fluxo principal

1. Ator seleciona serviços e funcionário.
2. Informa uma data.
3. Sistema calcula a duração total.
4. Sistema consulta os intervalos de trabalho.
5. Sistema remove intervalos ocupados ou inválidos.
6. Sistema retorna as opções disponíveis.

### Observação

Um horário exibido não está reservado. A disponibilidade deve ser validada novamente durante a confirmação.

## UC04 — Solicitar agendamento

**Ator:** Cliente ou Funcionário.

### Pré-condições

- ator autenticado;
- funcionário e serviços ativos;
- funcionário habilitado para os serviços.

### Fluxo principal assíncrono proposto

1. Ator envia serviços, funcionário, início e chave de idempotência.
2. Sistema determina o cliente a partir da identidade ou do contexto autorizado do funcionário.
3. Sistema valida os dados básicos.
4. Sistema cria uma solicitação `Pending` e sua mensagem Outbox na mesma transação.
5. Dispatcher publica a solicitação no RabbitMQ.
6. Consumidor processa a solicitação.
7. Consumidor calcula início, duração e fim.
8. Consumidor valida o horário de trabalho e conflitos.
9. PostgreSQL aplica a proteção final contra sobreposição.
10. Consumidor cria ou atualiza o agendamento para `Scheduled` e grava o evento de confirmação na Outbox.
11. Sistema disponibiliza o resultado ao solicitante.

### Alternativa — Conflito

1. A solicitação é processada depois de outra que ocupou o intervalo.
2. Sistema não cria um agendamento confirmado.
3. Solicitação recebe resultado `Rejected` com motivo `ScheduleConflict`.
4. Evento de rejeição é gravado na Outbox.
5. Cliente é informado de que o horário não está mais disponível.

### Alternativa — Repetição

Se a chave de idempotência já existir, o sistema retorna o estado conhecido da solicitação original.

## UC05 — Consultar meus agendamentos

**Ator:** Cliente.

1. Cliente solicita seus agendamentos.
2. Sistema usa a identidade autenticada para determinar o cliente.
3. Sistema retorna somente recursos pertencentes a ele.

## UC06 — Alterar agendamento próprio

**Ator:** Cliente.

1. Cliente seleciona um agendamento próprio.
2. Informa o novo horário ou os novos serviços permitidos.
3. Sistema verifica propriedade e estado.
4. Sistema executa novamente as validações de disponibilidade.
5. Sistema confirma a alteração ou informa conflito.
6. Sistema registra evento e auditoria.

## UC07 — Manejar agendamento

**Ator:** Funcionário ou Administrador.

1. Ator consulta a agenda.
2. Seleciona um agendamento.
3. Cria, altera ou cancela conforme a operação.
4. Sistema verifica a role e a regra do recurso.
5. Sistema valida e registra a operação.

## UC08 — Cancelar agendamento

**Ator:** Cliente proprietário, Funcionário ou Administrador.

1. Ator solicita o cancelamento.
2. Sistema verifica permissão e estado atual.
3. Sistema altera o estado para `Cancelled`.
4. Sistema registra responsável, data e motivo.
5. Sistema grava evento Outbox.
6. O intervalo deixa de bloquear a agenda.

## UC09 — Publicar mensagens da Outbox

**Ator:** Dispatcher.

1. Dispatcher busca mensagens ainda não publicadas.
2. Publica cada mensagem no RabbitMQ.
3. Registra sucesso após confirmação do broker.
4. Em falha transitória, mantém a mensagem disponível para nova tentativa.

## UC10 — Consumir evento idempotentemente

**Ator:** Worker ou consumidor.

1. Consumidor recebe evento com identificador único.
2. Verifica se ele já foi processado.
3. Se já processado, confirma a mensagem sem repetir o efeito.
4. Caso contrário, processa e registra o identificador.
5. Confirma a mensagem no broker.

