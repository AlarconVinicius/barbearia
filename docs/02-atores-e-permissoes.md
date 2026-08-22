# Atores e permissões

## Administrador

Responsável pela configuração e gestão da barbearia.

Pode:

- cadastrar, consultar, alterar, ativar e desativar funcionários;
- cadastrar, consultar, alterar, ativar e desativar serviços;
- configurar os horários de trabalho dos funcionários;
- consultar todos os agendamentos;
- criar, alterar e cancelar qualquer agendamento;
- consultar usuários e atribuir funções administrativas permitidas.

## Funcionário

Representa um profissional ou atendente autorizado a operar a agenda.

Pode:

- consultar os agendamentos da barbearia;
- criar agendamentos para clientes;
- alterar e cancelar agendamentos;
- consultar e alterar o próprio perfil;
- alterar os próprios dados profissionais que forem explicitamente permitidos.

Não pode:

- cadastrar ou excluir outros funcionários;
- atribuir funções;
- alterar permissões;
- alterar configurações administrativas de outros funcionários.

### Ponto a detalhar

Antes da implementação, os campos que o funcionário pode alterar no próprio perfil deverão ser listados. A recomendação inicial é permitir dados pessoais e de apresentação, mas reservar serviços executados, status e permissões ao administrador.

## Cliente

Pode:

- criar e manter a própria conta;
- consultar serviços, funcionários e disponibilidade;
- solicitar um agendamento para si;
- consultar somente os próprios agendamentos;
- alterar ou cancelar somente os próprios agendamentos, respeitando as regras vigentes;
- alterar o próprio perfil.

Não pode:

- consultar dados ou agendamentos de outro cliente;
- administrar funcionários ou serviços;
- alterar o estado operacional de atendimentos de terceiros.

## Sistema de processamento

Responsável por processar solicitações e eventos assíncronos.

Pode:

- consumir solicitações pendentes;
- confirmar ou rejeitar uma solicitação de agendamento;
- publicar eventos de resultado;
- processar mensagens da Outbox;
- registrar falhas e novas tentativas.

## Matriz resumida

| Operação | Administrador | Funcionário | Cliente |
|---|---:|---:|---:|
| Gerenciar funcionários | Sim | Não | Não |
| Alterar o próprio perfil | Sim | Sim | Sim |
| Gerenciar serviços | Sim | Não | Não |
| Configurar horários de trabalho | Sim | Não, inicialmente | Não |
| Ver todos os agendamentos | Sim | Sim | Não |
| Ver os próprios agendamentos | Sim | Sim | Sim |
| Criar agendamento | Sim | Sim | Sim, para si |
| Alterar agendamento | Sim | Sim | Somente o próprio |
| Cancelar agendamento | Sim | Sim | Somente o próprio |

## Princípios de autorização

- Autenticação não implica autorização.
- O identificador do cliente será obtido da identidade autenticada, e não aceito livremente no corpo da requisição.
- Toda consulta de cliente deverá limitar o resultado ao usuário autenticado, salvo quando realizada por funcionário autorizado.
- Funções gerais serão representadas por roles, mas regras como “somente o próprio agendamento” serão verificadas sobre o recurso acessado.
- Exclusões relevantes deverão ser lógicas quando houver histórico associado.

