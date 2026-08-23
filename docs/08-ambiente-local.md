# Ambiente local

Este documento descreve como executar e diagnosticar as dependências locais do BarberFlow.

## Pré-requisitos

- .NET SDK 10;
- Docker Desktop em execução;
- Docker Compose v2;
- portas `5432`, `5672` e `15672` disponíveis.

Verifique as instalações:

```powershell
dotnet --version
docker --version
docker compose version
```

## Serviços

| Serviço | Imagem | Porta | Finalidade |
|---|---|---:|---|
| PostgreSQL | `postgres:16` | `5432` | Persistência da aplicação |
| RabbitMQ | `rabbitmq:4-management` | `5672` | Mensageria da API e do Worker |
| RabbitMQ Management | `rabbitmq:4-management` | `15672` | Interface administrativa do broker |

Os dados são mantidos nos volumes Docker `postgres_data` e `rabbitmq_data`.

## Configurar variáveis do Docker Compose

O arquivo `src/docker/.env.example` contém valores exclusivamente destinados ao desenvolvimento local. Copie-o para `.env`:

```powershell
Copy-Item .\src\docker\.env.example .\src\docker\.env
```

O arquivo criado fica ignorado pelo Git. Ajuste seus valores quando necessário:

```dotenv
POSTGRES_USER=admin
POSTGRES_PASSWORD=admin
POSTGRES_DB=barberflow
RABBITMQ_DEFAULT_USER=admin
RABBITMQ_DEFAULT_PASS=admin
```

Esses valores não devem ser utilizados em produção.

## Validar o Compose

A partir da raiz do repositório:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml config
```

O comando deve exibir a configuração resolvida sem erros de validação.

## Iniciar os serviços

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml up -d
```

O parâmetro `-d` mantém os containers em segundo plano.

Consulte o estado:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml ps
```

Os containers `barberflow-postgres` e `barberflow-rabbitmq` devem alcançar o estado `healthy`. Durante a inicialização, eles podem permanecer temporariamente como `starting`.

## Acessar os serviços

### PostgreSQL

```text
Host: localhost
Port: 5432
Database: valor de POSTGRES_DB
Username: valor de POSTGRES_USER
Password: valor de POSTGRES_PASSWORD
```

### RabbitMQ

```text
Host: localhost
AMQP port: 5672
Management URL: http://localhost:15672
Username: valor de RABBITMQ_DEFAULT_USER
Password: valor de RABBITMQ_DEFAULT_PASS
Virtual host: /
```

## Configurar a API e o Worker

API e Worker utilizam as mesmas chaves de configuração:

```text
ConnectionStrings:PostgreSql
RabbitMq:HostName
RabbitMq:Port
RabbitMq:VirtualHost
RabbitMq:UserName
RabbitMq:Password
```

Os arquivos `appsettings.Development.json` possuem endereços locais e valores `replace_me` no lugar das credenciais. Não substitua esses valores por segredos reais em arquivos versionados.

Use variáveis de ambiente com dois sublinhados para representar a hierarquia da configuração do .NET. Exemplo no PowerShell:

```powershell
$env:ConnectionStrings__PostgreSql = "Host=localhost;Port=5432;Database=barberflow;Username=admin;Password=admin"
$env:RabbitMq__HostName = "localhost"
$env:RabbitMq__Port = "5672"
$env:RabbitMq__VirtualHost = "/"
$env:RabbitMq__UserName = "admin"
$env:RabbitMq__Password = "admin"
```

As variáveis valem somente para o processo atual do PowerShell e os processos iniciados por ele. Configure-as em cada terminal usado para iniciar API ou Worker.

## Compilar a solution

```powershell
dotnet build .\src\backend\BarberFlow.slnx
```

## Executar API e Worker

Em terminais separados, com as variáveis de ambiente configuradas:

```powershell
dotnet run --project .\src\backend\services\BarberFlow.Api\BarberFlow.Api.csproj
```

```powershell
dotnet run --project .\src\backend\services\BarberFlow.Worker\BarberFlow.Worker.csproj
```

Nesta fase inicial, os projetos apenas inicializam seus respectivos hosts. As conexões efetivas serão registradas durante a implementação da persistência e da mensageria.

## Consultar logs

Todos os serviços:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml logs --follow
```

Somente PostgreSQL:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml logs --follow postgres
```

Somente RabbitMQ:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml logs --follow rabbitmq
```

Use `Ctrl+C` para sair da visualização sem parar os containers.

## Verificar a saúde manualmente

PostgreSQL:

```powershell
docker exec barberflow-postgres pg_isready
```

RabbitMQ:

```powershell
docker exec barberflow-rabbitmq rabbitmq-diagnostics -q ping
```

Resultados esperados:

```text
PostgreSQL: accepting connections
RabbitMQ: Ping succeeded
```

## Parar e remover os containers

Parar sem remover:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml stop
```

Remover os containers e a rede, preservando os volumes:

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml down
```

Para iniciar novamente, execute `docker compose ... up -d`.

## Apagar os dados locais

O comando abaixo remove os containers e também os volumes do PostgreSQL e RabbitMQ. Todos os dados locais serão apagados e não poderão ser recuperados pelos containers.

```powershell
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml down --volumes
```

Use essa opção somente quando precisar recriar o ambiente do zero.

## Diagnóstico

### Porta já utilizada

Se uma porta estiver ocupada, identifique o processo no Windows:

```powershell
Get-NetTCPConnection -LocalPort 5432,5672,15672 -ErrorAction SilentlyContinue
```

Encerre ou reconfigure o serviço conflitante antes de iniciar o Compose.

### Container com estado unhealthy

Consulte o estado detalhado e os logs:

```powershell
docker inspect barberflow-postgres
docker inspect barberflow-rabbitmq
docker compose --env-file .\src\docker\.env -f .\src\docker\docker-compose.yml logs
```

### Variável ausente

Se `docker compose config` informar que uma variável não foi definida, confirme que `src/docker/.env` existe e que o argumento `--env-file` aponta para esse arquivo.

### Credenciais diferentes após alterar o .env

PostgreSQL e RabbitMQ inicializam credenciais quando seus volumes são criados. Alterar apenas o `.env` não modifica automaticamente usuários já existentes.

Para um ambiente descartável, remova os volumes conforme a seção anterior e inicie novamente. Não faça isso se precisar preservar os dados locais.
