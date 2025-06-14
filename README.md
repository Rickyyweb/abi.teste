
# abi.teste
Projeto de Avaliação ABI: .NET com EF Core e PostgreSQL
=======
# Ambev Developer Evaluation

Este README descreve como executar a aplicação via Docker, bem como boas práticas de Git Flow e convenções de commit semântico.

---

## Sumário

1. [Requisitos](#requisitos)  
2. [Estrutura do Projeto](#estrutura-do-projeto)  
3. [Executando a Aplicação via Docker](#executando-a-aplicação-via-docker)  
   - [1. Clonar o Repositório](#1-clonar-o-repositório)  
   - [2. Ajustar Variáveis de Ambiente (opcional)](#2-ajustar-variáveis-de-ambiente-opcional)  
   - [3. Subir Containers com Docker Compose](#3-subir-containers-com-docker-compose)  
   - [4. Verificar Logs e Testar Endpoints](#4-verificar-logs-e-testar-endpoints)  
4. [Git Flow](#git-flow)  
   - [Branches Principais](#branches-principais)  
5. [Boas Práticas Gerais](#boas-práticas-gerais)  

---

## Requisitos

Antes de executar a aplicação via Docker, verifique se você possui instalado em sua máquina:

- [Docker](https://docs.docker.com/get-docker/) (versão mínima recomendada: 20.10+)  
- [Docker Compose](https://docs.docker.com/compose/install/) (versão mínima recomendada: 1.27+)  
- [Git](https://git-scm.com/downloads)  

---

## Estrutura do Projeto

```
Ambev.DeveloperEvaluation
├── Adapters
│   └── Driven
│   ├    └── Infrastructure
│   │        └── Ambev.DeveloperEvaluation.ORM              # Persistence (EF Core + PostgreSQL)
├   └Drivers
│   └── WebApi
│       └── Ambev.DeveloperEvaluation.WebApi               # ASP.NET Core Web API
├── Core
│   ├── Application
│   │   └── Ambev.DeveloperEvaluation.Application          # Handlers, Commands, Queries, Services
│   └── Domain
│       └── Ambev.DeveloperEvaluation.Domain               # Entidades, Especificações, Validações, Eventos de Domínio
├── Crosscutting
│   ├── Common
│   │   └── Ambev.DeveloperEvaluation.Common               # Utilitários compartilhados (PaginatedList, ApiResponse, etc.)
│   └── IoC
│       └── Ambev.DeveloperEvaluation.IoC                  # Configuração de DI (MediatR, AutoMapper, DbContext, etc.)
├── Tests
│   ├── Functional
│   │   └── Ambev.DeveloperEvaluation.Functional           # Testes Funcionais (WebApplicationFactory, InMemory, Helpers)
│   ├── Integration
│   │   └── Ambev.DeveloperEvaluation.Integration          # Testes de Integração (Testcontainers + PostgreSQL real)
│   └── Unit
│       └── Ambev.DeveloperEvaluation.Unit                 # Testes Unitários (xUnit + FluentAssertions + NSubstitute)
├── docker-compose.yml                                      # Orquestra WebApi + PostgreSQL
└── README.md                                               # Documentação (já comentada anteriormente)
```

- **Adapters/Driven/Infrastructure/Ambev.DeveloperEvaluation.ORM**: Projeto de persistência usando Entity Framework Core (PostgreSQL).  
- **Drivers/WebApi/Ambev.DeveloperEvaluation.WebApi**: Projeto ASP.NET Core Web API (controllers, endpoints etc.).  
- **Core/Application/Ambev.DeveloperEvaluation.Application**: Contém comandos, handlers e regras de aplicação.  
- **Core/Domain/Ambev.DeveloperEvaluation.Domain**: Entidades de domínio, especificações e validações.  
- **Crosscutting/Common/Ambev.DeveloperEvaluation.Common**: Componentes comuns e utilitários compartilhados.  
- **Crosscutting/IoC/Ambev.DeveloperEvaluation.IoC**: Configuração de Injeção de Dependências.  
- **Tests/Functional**: Testes funcionais (WebApplicationFactory), incluindo helper ProgramStub e ProjectPathHelper.  
- **Tests/Integration**: Testes de integração (Testcontainers).  
- **Tests/Unit**: Testes unitários (xUnit + FluentAssertions).  
- **docker-compose.yml**: Orquestra a execução da WebAPI e do container PostgreSQL.  
- **README.md**: Documentação deste projeto (este arquivo).

---

## Executando a Aplicação via Docker

### 1. Clonar o Repositório

```bash
git clone https://github.com/Rickyyweb/abi.teste.git
cd Ambev.DeveloperEvaluation
```

### 2. Ajustar Variáveis de Ambiente (opcional)

O `docker-compose.yml` já define as variáveis de ambiente necessárias para rodar localmente em modo de desenvolvimento. Caso queira alterar alguma string de conexão ou porta, edite o arquivo `docker-compose.yml` na raiz do projeto. Exemplo de trecho relevante:

```yaml
services:
  webapi:
    container_name: ambev_developer_evaluation_webapi
    build:
      context: .
      dockerfile: src/Ambev.DeveloperEvaluation.WebApi/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=database;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n;Trust Server Certificate=True
      - ASPNETCORE_URLS=http://+:8080
    depends_on:
      database:
        condition: service_healthy
    ports:
      - "8080:8080"

  database:
    container_name: ambev_developer_evaluation_database
    image: postgres:13
    environment:
      POSTGRES_DB: developer_evaluation
      POSTGRES_USER: developer
      POSTGRES_PASSWORD: ev@luAt10n
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U developer -d developer_evaluation"]
      interval: 5s
      timeout: 5s
      retries: 5
    restart: unless-stopped
```

- `ConnectionStrings__DefaultConnection` dentro de `webapi.environment` sobrescreve o `appsettings.json`.  
- A aplicação WebAPI ficará exposta em `http://localhost:8080`.  

### 3. Subir Containers com Docker Compose

No terminal, execute:

```bash
docker-compose up --build
```

- A flag `--build` força a reconstrução da imagem do WebAPI toda vez que algo mudar no código.  
- O Docker Compose irá:
  1. Subir um container `database` com PostgreSQL (versão 13).  
  2. Subir um container `webapi` que expõe a API em `http://localhost:8080`.  
  3. Rodar as migrations automaticamente (via `Program.cs`) ao iniciar o WebAPI.  

Aguarde até ver no log algo parecido com:

```
webapi    | info: Microsoft.Hosting.Lifetime[0]
webapi    |       Now listening on: http://[::]:8080
webapi    | info: Microsoft.Hosting.Lifetime[0]
webapi    |       Application started. Press Ctrl+C to shut down.
database  | 2023-XX-XX 12:00:00.000 UTC [1] LOG:  database system is ready to accept connections
```

```

```

### 4. Verificar Logs e Testar Endpoints

1. **Logs**:  
   - Em outro terminal, rode `docker-compose logs -f webapi` para ver em tempo real as requisições.  
   - Rode `docker-compose logs -f database` para ver a inicialização do PostgreSQL e healthchecks.

2. **Testar Endpoints** (via `curl`, `Postman` ou browser):

   - **Criar Venda** (POST):
     ```bash
		 curl -X 'POST' \
		  'http://localhost:8080/api/Sales' \
		  -H 'accept: text/plain' \
		  -H 'Content-Type: application/json' \
		  -d '{
		  "idempotencyKey": "abc123-qualquer-coisa",
		  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
		  "items": [
			{
			  "productId": "6f1e4f02-1a35-4b2d-9fdd-3a7c5ab1ae2b",
			  "quantity": 13
			},
			{
			  "productId": "b254a3d4-7c76-4e14-af08-2dcf928d3b10",
			  "quantity": 2
			}
		  ]
		}'
     ```

   - **Obter Venda** (GET):
     ```bash
     curl http://localhost:8080/api/Sales/1c107255-2f1d-45ee-975e-433a1db0f441
     ```
     Exemplo de resposta (HTTP 200):
     ```json
     {
		  "success": true,
		  "message": "string",
		  "errors": [
			{
			  "error": "string",
			  "detail": "string"
			}
		  ],
		  "data": {
			"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
			"subtotal": 0,
			"discount": 0,
			"totalAmount": 0,
			"items": [
			  {
				"productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
				"productName": "string",
				"productPrice": 0,
				"quantity": 0,
				"unitPrice": 0,
				"totalPrice": 0
			  }
			]
		  }
	  }
     ```
	 
	 - **Atualizar Venda** (PUT):
     ```bash
		 curl -X 'PUT' \
		  'http://localhost:8080/api/Sales/1c107255-2f1d-45ee-975e-433a1db0f441' \
		  -H 'accept: text/plain' \
		  -H 'Content-Type: application/json' \
		  -d '{
		  "idempotencyKey": "dwd2312",
		  "items": [
			{
			  "productId": "b254a3d4-7c76-4e14-af08-2dcf928d3b10",
			  "quantity": 20
			},
			{
			  "productId": "6f1e4f02-1a35-4b2d-9fdd-3a7c5ab1ae2b",
			  "quantity": 10
			}
		  ]
		}'
     ```
	 
	 - **Deletar Venda** (DELETE):
     ```bash
		 curl http://localhost:8080/api/Sales/1c107255-2f1d-45ee-975e-433a1db0f441
     ```

3. **Encerrar os Containers**:
   ```bash
   docker-compose down
   ```
   - O comando `down` encerra e remove os containers, lançando mão do nome `container_name` definido no `docker-compose.yml`.

---

## Git Flow

Para garantir organização e controle de versões, padrão **Git Flow**:

### Branches Principais

- `main`: Código em produção (ou pronto para produção). Não faça commits diretos; use pull requests.  
- `develop`: Código para a próxima release. Todas as features aprovadas são mescladas aqui.


## Boas Práticas Gerais

1. **Clean Code**  
   - Nomes descritivos e coesos.  
   - Funções/métodos curtos (máx. 20–30 linhas).  
   - Evite duplicação (DRY).

2. **SOLID**  
   - Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion.

3. **YAGNI**  
   Implemente apenas o que foi requisitado.

4. **Object Calisthenics** (opcional)  
   - Linhas com no máximo um ponto (`.`).  
   - Máx. 10 linhas por método, 50 linhas por classe.

5. **Logs com Serilog**  
   - Use `ILogger<T>`.  
   - Logue eventos importantes (criação, operações críticas, erros).

6. **Testes Automatizados**  
   - **Unitários** (xUnit + FluentAssertions + NSubstitute).  
   - **Integração** (Testcontainers).

---

© 2023 Ambev Developer Evaluation
