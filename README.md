# 🎮 ESTop1 - Plataforma de E-Sports

Uma plataforma completa para gerenciamento de jogadores e times de Counter-Strike 2, com integração FACEIT API.

## 🚀 Tecnologias

### Backend (.NET 9)
- **ASP.NET Core Web API**
- **Entity Framework Core** com SQLite
- **Clean Architecture** (Domain, Infrastructure, API)
- **Swagger/OpenAPI** para documentação
- **Integração FACEIT API**

### Frontend (React + TypeScript)
- **React 18** com TypeScript
- **Vite** para build e desenvolvimento
- **Tailwind CSS** para estilização
- **React Router** para navegação
- **Shadcn/ui** para componentes

## 📁 Estrutura do Projeto

```
estop1/
├── ESTop1/                    # Backend (.NET)
│   ├── ESTop1/               # API Layer
│   ├── ESTop1.Domain/        # Domain Layer
│   └── ESTop1.Infrastructure/ # Infrastructure Layer
├── ESTop1Client/             # Frontend (React)
└── README.md
```

## 🛠️ Como Executar

### Backend
```bash
cd ESTop1/ESTop1
dotnet restore
dotnet run
```
**URL**: `https://localhost:7193`

### Frontend
```bash
cd ESTop1Client
npm install
npm run dev
```
**URL**: `http://localhost:5173`

## 🔧 Funcionalidades

### ✅ Implementadas
- **Autenticação** (Login/Cadastro)
- **Gestão de Jogadores** (CRUD completo)
- **Gestão de Times** (CRUD completo)
- **Sistema de Inscrições** para aspirantes
- **Integração FACEIT API** (busca de jogadores)
- **Interface Responsiva** (Mobile/Desktop)
- **Filtros e Paginação**

### 🔌 APIs Disponíveis

#### Jogadores
- `GET /api/jogadores` - Listar jogadores
- `POST /api/jogadores` - Criar jogador
- `GET /api/jogadores/{id}` - Detalhes do jogador

#### Times
- `GET /api/times` - Listar times
- `POST /api/times` - Criar time
- `GET /api/times/{id}` - Detalhes do time

#### Inscrições
- `POST /api/inscricoes` - Nova inscrição

#### FACEIT Integration
- `GET /api/integracoes/faceit/jogador/{nickname}` - Buscar jogador
- `GET /api/integracoes/faceit/estatisticas/{playerId}` - Estatísticas
- `GET /api/integracoes/faceit/partidas/{playerId}` - Histórico de partidas

## 🎯 Próximos Passos

- [ ] Deploy em produção
- [ ] Sistema de notificações
- [ ] Chat entre jogadores
- [ ] Sistema de torneios
- [ ] Dashboard administrativo

## 📝 Licença

Este projeto é de uso pessoal/educacional.

---

**Desenvolvido com ❤️ para a comunidade de CS2**
