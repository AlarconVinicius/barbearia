# Visão do produto

## Nome provisório

Sistema de Agendamento para Barbearia.

## Contexto

A barbearia precisa organizar os atendimentos de seus profissionais e permitir que clientes solicitem horários sem criar reservas sobrepostas.

O sistema atenderá inicialmente **uma única barbearia**. Não haverá isolamento multiempresa ou modelo SaaS nesta versão.

## Problema

Agendamentos controlados manualmente podem produzir:

- dois clientes reservados com o mesmo profissional no mesmo horário;
- reservas fora do horário de trabalho;
- dificuldade para clientes acompanharem seus próprios horários;
- alterações e cancelamentos sem controle de permissão;
- perda de notificações quando algum serviço externo estiver indisponível.

## Objetivo

Disponibilizar uma API confiável para cadastrar serviços e funcionários, consultar horários e criar, consultar, alterar e cancelar agendamentos, garantindo que um profissional não tenha atendimentos simultâneos.

## Escopo inicial

- cadastro e autenticação de usuários;
- administração de funcionários;
- cadastro de serviços, preços e durações;
- configuração de horários de trabalho dos funcionários;
- consulta de disponibilidade;
- solicitação e confirmação de agendamentos;
- consulta, alteração e cancelamento conforme as permissões do usuário;
- prevenção de conflitos sob requisições concorrentes;
- registro confiável de eventos por meio de Outbox;
- documentação da API com OpenAPI/Swagger;
- persistência em PostgreSQL com EF Core;
- comunicação assíncrona com RabbitMQ;
- testes automatizados.

## Horário de trabalho

O modelo inicial permitirá mais de um intervalo no mesmo dia. Assim, uma jornada como `09:00–12:00` e `13:00–18:00` poderá representar o intervalo de almoço sem uma entidade específica de pausa.

Não serão tratados inicialmente:

- feriados nacionais, estaduais ou municipais;
- férias;
- folgas excepcionais;
- regras recorrentes complexas;
- bloqueios avulsos de agenda.

Esses recursos poderão ser adicionados posteriormente sem alterar o objetivo central.

## Fora do escopo inicial

- múltiplas barbearias ou unidades;
- pagamentos;
- controle de estoque;
- comissões;
- programa de fidelidade;
- aplicativo móvel;
- integração com calendários externos;
- envio real de e-mail, SMS ou WhatsApp;
- infraestrutura AWS;
- pipeline completo de entrega contínua.

## Critérios de sucesso do primeiro marco

O primeiro marco será considerado concluído quando:

1. um administrador conseguir cadastrar funcionário, serviços e horários de trabalho;
2. um cliente conseguir consultar horários válidos;
3. um cliente conseguir solicitar um agendamento;
4. somente uma de várias solicitações simultâneas para o mesmo profissional e intervalo for confirmada;
5. as demais solicitações receberem uma resposta clara de indisponibilidade;
6. clientes acessarem somente os próprios agendamentos;
7. funcionários manejarem os agendamentos conforme suas permissões.

