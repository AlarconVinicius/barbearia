# Regras de negócio

## Serviços e duração

- **RN01:** um agendamento deve possuir pelo menos um serviço.
- **RN02:** todos os serviços devem estar ativos.
- **RN03:** o funcionário escolhido deve estar ativo e habilitado para todos os serviços.
- **RN04:** a duração total será a soma das durações dos serviços selecionados.
- **RN05:** no momento da confirmação, preço, duração e dados necessários do serviço devem ser copiados para os itens do agendamento.
- **RN06:** o fim deve ser calculado somando a duração total ao início e deve ser persistido.

## Horário de trabalho

- **RN07:** um agendamento deve caber integralmente dentro de um único intervalo de trabalho.
- **RN08:** um agendamento não pode atravessar o intervalo entre duas jornadas. Por exemplo, um atendimento `11:30–12:30` não cabe na jornada `09:00–12:00` e `13:00–18:00`.
- **RN09:** intervalos de trabalho de um mesmo funcionário e dia não podem se sobrepor.
- **RN10:** o sistema não deve oferecer horários no passado.

## Conflito

- **RN11:** dois agendamentos `Scheduled` do mesmo funcionário não podem ocupar intervalos sobrepostos.
- **RN12:** intervalos adjacentes são permitidos. Um agendamento pode começar exatamente quando outro termina.
- **RN13:** existe sobreposição quando `novoInicio < fimExistente` e `novoFim > inicioExistente`.
- **RN14:** agendamentos `Cancelled` não bloqueiam horários.
- **RN15:** a prevenção deve existir no banco de dados, além da validação feita pela aplicação.
- **RN16:** somente uma tentativa concorrente pelo mesmo intervalo pode ser confirmada.

## Estados

- **RN17:** `Pending` representa uma solicitação aguardando decisão e não uma confirmação de horário.
- **RN18:** somente `Scheduled` ocupa definitivamente a agenda.
- **RN19:** `Cancelled` é terminal na versão inicial.
- **RN20:** conflito é resultado da tentativa, não estado de um agendamento confirmado.
- **RN21:** uma solicitação rejeitada deve informar um motivo adequado, como `ScheduleConflict`.

## Permissões

- **RN22:** somente o administrador gerencia outros funcionários.
- **RN23:** um funcionário pode alterar somente os próprios campos autorizados.
- **RN24:** funcionários autorizados podem manejar agendamentos.
- **RN25:** um cliente pode consultar, alterar ou cancelar somente seus próprios agendamentos.
- **RN26:** um cliente não pode criar um agendamento em nome de outro cliente.

## Alteração e cancelamento

- **RN27:** toda alteração de profissional, serviços ou horário deve repetir as validações de uma criação.
- **RN28:** ao validar uma alteração, o próprio agendamento deve ser desconsiderado na busca por conflito.
- **RN29:** o cancelamento deve ser lógico e registrar data, responsável e motivo quando informado.

## Mensagens e transações

- **RN30:** a mudança de estado e a mensagem Outbox correspondente devem ser persistidas na mesma transação.
- **RN31:** a publicação no RabbitMQ pode ocorrer mais de uma vez; consumidores devem ser idempotentes.
- **RN32:** uma chave de idempotência repetida deve devolver o resultado já conhecido, sem criar uma nova solicitação.

## Cadastro e autenticação

- **RN33:** o registro de cliente exige nome completo, CPF, telefone e e-mail.
- **RN34:** o e-mail normalizado deve ser único e será o identificador usado no login.
- **RN35:** o login não utilizará senha; um código de confirmação será enviado ao e-mail informado.
- **RN36:** o código deve conter exatamente seis dígitos e expirar cinco minutos após sua emissão.
- **RN37:** o código deve ser invalidado após uma validação bem-sucedida e não pode ser reutilizado.
- **RN38:** cada código permite no máximo três tentativas de validação. Ao atingir o limite, ele deve ser invalidado e novas validações e emissões para o usuário devem ser bloqueadas por três minutos.
- **RN39:** a emissão de um novo código deve invalidar qualquer código anterior ainda ativo para o mesmo usuário e propósito.
- **RN40:** o código não pode ser persistido nem registrado em logs em texto puro.
- **RN41:** respostas de solicitação de código não devem revelar desnecessariamente se um e-mail está cadastrado.
- **RN42:** solicitações e tentativas de validação devem possuir limitação de frequência por e-mail e origem da requisição.
- **RN43:** o primeiro código validado após o registro confirma o endereço de e-mail; códigos posteriores autenticam o usuário.
- **RN44:** encerrados os três minutos de bloqueio, o usuário pode solicitar um novo código; o código que atingiu o limite de tentativas permanece inválido.
- **RN45:** usuários clientes, funcionários e administradores serão armazenados na entidade única `User`, e suas capacidades serão determinadas por `UserRole`.
- **RN46:** `EmployeeId` deve referenciar um usuário ativo com papel `Employee`.
- **RN47:** `CustomerId` deve referenciar um usuário ativo com papel `Customer` ou `Employee`.
- **RN48:** um usuário com papel `Customer`, mas sem papel `Employee`, não pode ser o profissional de um agendamento.
- **RN49:** um usuário com papel `Employee` pode ser o cliente ou o profissional de um agendamento, mas `CustomerId` e `EmployeeId` não podem ter o mesmo valor no mesmo agendamento.
- **RN50:** `Cpf` é obrigatório e único para todos os usuários.

