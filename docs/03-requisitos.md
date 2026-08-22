# Requisitos

## Requisitos funcionais

### RF01 — Autenticação

O sistema deve implementar autenticação própria e permitir registro e login sem senha usando o e-mail e um código de confirmação enviado por e-mail.

No registro, o cliente deve informar nome completo, telefone e e-mail. O e-mail será usado como identificador de acesso.

O código de confirmação deve:

- conter exatamente seis dígitos;
- expirar cinco minutos após sua emissão;
- permitir no máximo três tentativas de validação;
- ser válido para um único uso;
- ser armazenado de forma segura, sem persistência do valor em texto puro.

Após três tentativas incorretas, novas validações e emissões de código para o usuário devem ficar bloqueadas por três minutos. Encerrado o bloqueio, um novo código poderá ser solicitado.

Após uma validação bem-sucedida, o sistema deve confirmar o e-mail no primeiro acesso ou autenticar o usuário nos acessos seguintes e emitir a credencial de sessão correspondente.

### RF02 — Autorização

O sistema deve controlar acesso pelas funções Administrador, Funcionário e Cliente, complementadas por regras de propriedade do recurso.

### RF03 — Funcionários

O administrador deve poder cadastrar, consultar, alterar, ativar e desativar funcionários.

### RF04 — Autogestão do funcionário

O funcionário deve poder consultar e alterar os campos autorizados de seu próprio perfil.

Um funcionário deve poder criar um agendamento para si próprio, desde que outro funcionário seja o profissional responsável pelo atendimento.

### RF05 — Serviços

O administrador deve poder cadastrar serviços com nome, descrição, preço, duração e estado ativo ou inativo.

### RF06 — Habilitação profissional

O administrador deve poder definir quais serviços cada funcionário executa.

### RF07 — Horários de trabalho

O administrador deve poder definir um ou mais intervalos de trabalho por funcionário e dia da semana.

Exemplo:

- segunda-feira: `09:00–12:00`;
- segunda-feira: `13:00–18:00`.

### RF08 — Consulta de disponibilidade

O sistema deve calcular horários disponíveis considerando:

- horário de trabalho;
- duração total dos serviços;
- agendamentos já confirmados;
- solicitações que estejam sendo processadas conforme a estratégia de concorrência;
- horários passados.

### RF09 — Solicitação de agendamento

O cliente ou funcionário autorizado deve poder solicitar um agendamento informando profissional, serviços e horário de início.

### RF10 — Processamento de agendamento

O sistema deve confirmar somente solicitações que respeitem todas as regras de disponibilidade e concorrência.

### RF11 — Estado do agendamento

O sistema deve inicialmente reconhecer os seguintes estados:

- `Pending`: solicitação recebida e ainda não decidida;
- `Scheduled`: agendamento confirmado;
- `Cancelled`: agendamento cancelado.

Estados operacionais como concluído e não compareceu poderão ser acrescentados posteriormente.

### RF12 — Rejeição por conflito

Uma solicitação que perder a disputa por um horário não deve criar um agendamento confirmado nem um agendamento com estado “conflitado”.

O sistema deve:

- marcar a solicitação como rejeitada, quando houver uma entidade separada de solicitação;
- ou retornar imediatamente uma resposta de conflito, quando o processamento for síncrono;
- registrar o motivo;
- produzir uma notificação de resultado quando o fluxo for assíncrono.

### RF13 — Consulta de agendamentos

- Administradores e funcionários autorizados devem consultar a agenda completa.
- Clientes devem consultar somente os próprios agendamentos.

### RF14 — Alteração

Funcionários autorizados devem poder alterar agendamentos. Clientes devem poder alterar somente os próprios, submetendo o novo intervalo às mesmas validações de uma criação.

### RF15 — Cancelamento

O cancelamento deve preservar o histórico e liberar o intervalo ocupado.

### RF16 — Snapshot do serviço

O sistema deve armazenar no item do agendamento o nome relevante, preço e duração vigentes no momento da confirmação.

### RF17 — Intervalo confirmado

O sistema deve armazenar o início e o fim calculado do agendamento confirmado.

### RF18 — Outbox

O agendamento e os eventos correspondentes devem ser gravados na mesma transação do PostgreSQL.

### RF19 — Mensageria

O sistema deve usar RabbitMQ para comunicação assíncrona entre o produtor e os processadores definidos.

### RF20 — Documentação da API

Os endpoints, formatos, autenticação, respostas e erros devem ser documentados por OpenAPI/Swagger.

## Requisitos não funcionais

### RNF01 — Consistência

O sistema não deve confirmar dois agendamentos ativos sobrepostos para o mesmo funcionário.

### RNF02 — Concorrência

A regra de não sobreposição deve continuar verdadeira sob requisições simultâneas e múltiplas instâncias da API ou do consumidor.

### RNF03 — Idempotência

O reenvio da mesma solicitação ou mensagem não deve criar agendamentos ou efeitos duplicados.

### RNF04 — Segurança

O sistema não deve armazenar senhas. Códigos de confirmação devem ser aleatórios, temporários, persistidos somente de forma protegida e nunca registrados em logs. A emissão e a validação devem possuir limitação de frequência. Dados privados não devem aparecer em logs e todo acesso deve respeitar as permissões documentadas.

### RNF05 — Auditabilidade

Criações, confirmações, rejeições, alterações e cancelamentos devem ser rastreáveis.

### RNF06 — Datas e horas

Instantes persistidos devem adotar uma convenção única. A decisão recomendada é armazenar UTC e converter para o fuso configurado da aplicação.

### RNF07 — Confiabilidade de mensagens

Consumidores devem tolerar mensagens repetidas e falhas transitórias.

### RNF08 — Testabilidade

Regras de domínio, persistência, concorrência, autorização e mensageria devem ser testáveis de forma automatizada.

### RNF09 — Observabilidade

O sistema deve produzir logs estruturados e identificadores de correlação para acompanhar requisições, solicitações e eventos.

