# Casos de uso

## UC00 — Registrar cliente

**Ator:** Visitante.

### Fluxo principal

1. Visitante informa nome completo, telefone e e-mail.
2. Sistema valida e normaliza os dados.
3. Sistema cria `User` e atribui o papel `Customer`.
4. Sistema gera um código de seis dígitos, armazena somente sua representação protegida e define a expiração para cinco minutos.
5. Sistema envia o código ao e-mail informado.
6. Visitante informa o código recebido.
7. Sistema valida prazo, quantidade de tentativas e correspondência do código.
8. Sistema marca o código como utilizado, confirma o e-mail e emite a credencial de acesso.

### Alternativas

- Código incorreto: sistema registra uma tentativa e informa que o código é inválido.
- Terceira tentativa incorreta: sistema invalida o código e bloqueia novas validações e emissões para o usuário por três minutos.
- Código expirado ou utilizado: sistema rejeita a validação e exige um novo código.
- Novo código solicitado: sistema invalida qualquer código anterior ainda ativo para o mesmo propósito.
- Bloqueio encerrado: após três minutos, o visitante pode solicitar um novo código.

## UC00A — Entrar com código por e-mail

**Ator:** Usuário cadastrado.

1. Usuário informa seu e-mail.
2. Sistema aplica as limitações de frequência e, quando permitido, gera um código de seis dígitos válido por cinco minutos.
3. Sistema armazena somente a representação protegida do código e o envia por e-mail.
4. Usuário informa o código recebido.
5. Sistema permite até três tentativas dentro do prazo de validade.
6. Em caso de sucesso, sistema invalida o código e emite a credencial de acesso.

Após a terceira tentativa incorreta, o código é invalidado e novas validações e emissões ficam bloqueadas por três minutos. Depois desse período, o usuário pode solicitar um novo código.

As respostas não devem permitir enumeração desnecessária de e-mails cadastrados.

## UC01 — Cadastrar funcionário

**Ator:** Administrador.

**Pré-condição:** administrador autenticado.

### Fluxo principal

1. Administrador informa os dados do funcionário.
2. Administrador seleciona os serviços que ele executa.
3. Sistema valida os dados e a unicidade necessária.
4. Sistema cria ou atualiza o usuário e atribui o papel `Employee`.
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

### Alternativa — Funcionário agenda para si

1. Funcionário escolhe criar um agendamento para si próprio.
2. Sistema usa o próprio `User` do funcionário como `CustomerId`.
3. Sistema exige que o profissional selecionado seja outro usuário com papel `Employee`.
4. Sistema rejeita a solicitação se `CustomerId` e `EmployeeId` forem iguais.
5. As mesmas validações de disponibilidade e concorrência do fluxo comum são aplicadas.

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

