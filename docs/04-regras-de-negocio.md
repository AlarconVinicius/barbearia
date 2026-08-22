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

