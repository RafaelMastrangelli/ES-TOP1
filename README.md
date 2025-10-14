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

## ⚙️ Configuração

### OpenAI API
Para usar a busca inteligente, configure a chave da API do OpenAI:

1. **Variável de ambiente** (recomendado):
   ```bash
   export OPENAI_API_KEY="sua-chave-aqui"
   ```

2. **Arquivo de configuração**:
   Edite `ESTop1/appsettings.json` e adicione sua chave:
   ```json
   {
     "OpenAI": {
       "ApiKey": "sua-chave-aqui"
     }
   }
   ```

## 🔧 Funcionalidades

### ✅ Implementadas
- **Autenticação** (Login/Cadastro)
- **Gestão de Jogadores** (CRUD completo)
- **Gestão de Times** (CRUD completo)
- **Sistema de Inscrições** para aspirantes
- **Integração FACEIT API** (busca de jogadores)
- **Integração OpenAI** (busca inteligente por IA)
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

#### OpenAI Integration
- `GET /api/integracoes/openai/buscar-jogadores?consulta={texto}` - Busca inteligente
- `GET /api/integracoes/openai/sugerir-filtros?descricao={texto}` - Sugerir filtros

### 🤖 Busca Inteligente com IA

A nova funcionalidade permite buscar jogadores usando linguagem natural:

**Exemplos de consultas:**
- "melhor AWP brasileiro"
- "jogador entry agressivo jovem"
- "IGL experiente disponível"
- "suporte brasileiro com alta rating"
- "jogador lurker aposentado"

**Como usar:**
1. Acesse a página de Jogadores
2. Use o campo "Buscar com IA" 
3. Digite sua consulta em linguagem natural
4. A IA analisará todos os jogadores e retornará os mais relevantes


## 📝 Licença

Este projeto é de uso pessoal.

---

**Desenvolvido com ❤️ para a comunidade de CS2**
