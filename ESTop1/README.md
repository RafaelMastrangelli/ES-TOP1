# ESTop1 - MVP SaaS

Biblioteca de jogadores profissionais de CS2 - MVP em 7 dias.

## Estrutura da Solução

```
ESTop1.sln
src/
  ESTop1.Api/               // ASP.NET Core Web API
  ESTop1.Domain/            // Entidades e enums (PT-BR)
  ESTop1.Infrastructure/    // EF Core: DbContext, Migrations, Seed
```

## Tecnologias

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core + SQLite
- Swagger/OpenAPI

## Setup Inicial

### 1. Instalar EF Core Tools

```bash
dotnet tool install --global dotnet-ef
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Executar Migrations

```bash
dotnet ef migrations add Inicial --project ESTop1.Infrastructure --startup-project ESTop1.Api
dotnet ef database update --project ESTop1.Infrastructure --startup-project ESTop1.Api
```

### 4. Executar a Aplicação

```bash
dotnet run --project ESTop1.Api
```

Acesse: https://localhost:7000/swagger

## Endpoints Principais

### Jogadores (`/api/jogadores`)

- `GET /api/jogadores` - Lista com filtros e paginação
- `GET /api/jogadores/{id}` - Detalhes do jogador
- `POST /api/jogadores` - Criar jogador
- `PUT /api/jogadores/{id}/visibilidade` - Alterar visibilidade

**Filtros disponíveis:**
- `q` - Busca por apelido
- `pais` - Filtrar por país
- `status` - Status do jogador (Profissional, Aposentado, Amador)
- `disp` - Disponibilidade (EmTime, Livre, Teste)
- `funcao` - Função (Entry, Suporte, Awp, Igl, Lurker)
- `maxIdade` - Idade máxima
- `ordenar` - rating_desc, valor_desc, apelido_asc
- `page` - Página (padrão: 1)
- `pageSize` - Tamanho da página (padrão: 20)

### Times (`/api/times`)

- `GET /api/times` - Lista times
- `GET /api/times/{id}` - Detalhes do time
- `POST /api/times` - Criar time

### Inscrições (`/api/inscricoes`) - Opcional

- `POST /api/inscricoes` - Criar inscrição de aspirante
- `POST /api/inscricoes/{id}/pagar` - Simular pagamento
- `POST /api/inscricoes/{id}/aprovar` - Aprovar inscrição

## Cálculo de Valor de Mercado

Fórmula implementada:
```
valor = 50_000 * (rating^1.5) * (1 + kd/10) * (1 + partidas/500)
```

Valor limitado entre R$ 10.000 e R$ 5.000.000.

## Roadmap

### Próxima Etapa
- [ ] JWT Authentication (roles: Admin, Time, Jogador)
- [ ] Endpoints protegidos de criação/visibilidade
- [ ] Importar estatísticas (CSV/JSON)
- [ ] Cálculo de valor de mercado em batch

### Plano SaaS
- [ ] Free browsing vs. plano Time
- [ ] Plano Jogador
- [ ] Sistema de assinaturas

## Dados de Seed

A aplicação já vem com dados de exemplo:
- 1 Time: "Bauru Stars"
- 2 Jogadores: "rafaa" (EmTime) e "coldzera" (Livre)
- Estatísticas calculadas automaticamente
