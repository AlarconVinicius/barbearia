# Sistema de Agendamento para Barbearia

Sistema de agendamento para barbearia desenvolvido em .NET.

O repositório contém a estrutura inicial da API, do Worker, do domínio e da infraestrutura, além do ambiente local com PostgreSQL e RabbitMQ.

## Documentos

1. [Visão do produto](docs/01-visao-do-produto.md)
2. [Atores e permissões](docs/02-atores-e-permissoes.md)
3. [Requisitos](docs/03-requisitos.md)
4. [Regras de negócio](docs/04-regras-de-negocio.md)
5. [Casos de uso](docs/05-casos-de-uso.md)
6. [Decisões de arquitetura](docs/06-decisoes-de-arquitetura.md)
7. [Entidades e propriedades](docs/07-entidades-e-propriedades.md)
8. [Ambiente local](docs/08-ambiente-local.md)

## Início rápido

Pré-requisitos:

- .NET SDK 10;
- Docker Desktop com Docker Compose v2.

No PowerShell, a partir da raiz do repositório:

```powershell
Copy-Item .\src\docker\.env.example .\src\docker\.env
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml up -d
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml ps
dotnet build .\src\backend\BarberFlow.slnx
```

O PostgreSQL ficará disponível em `localhost:5432`, o RabbitMQ em `localhost:5672` e sua interface de administração em [http://localhost:15672](http://localhost:15672).

Consulte [Ambiente local](docs/08-ambiente-local.md) para configuração da aplicação, diagnóstico, logs e encerramento dos serviços.

## Estado atual

- Escopo de produto inicial definido.
- Atores e matriz de permissões definidos.
- Requisitos funcionais e não funcionais iniciais definidos.
- Casos de uso principais descritos.
- Estratégia de concorrência e uso do RabbitMQ registrada como decisão proposta.
- Modelo inicial de entidades, propriedades e relacionamentos documentado.
- Solution inicial com API, Worker, Domain e Infrastructure criada.
- PostgreSQL e RabbitMQ configurados para desenvolvimento local.
- CI/CD, AWS e envio real de notificações foram adiados para etapas futuras.

