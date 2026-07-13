# 🎮 ESTop1 - Plataforma de E-Sports

Uma plataforma completa para gerenciamento de jogadores e times de Counter-Strike 2, com sistema de assinaturas, tipos de usuário e integração com APIs externas.

## 🚀 Tecnologias

### Backend (.NET 9)
- **ASP.NET Core Web API**
- **Entity Framework Core** com SQLite
- **Clean Architecture** (Domain, Infrastructure, API)
- **Swagger/OpenAPI** para documentação
- **JWT Authentication** com sistema de tipos de usuário
- **Sistema de Assinaturas** com planos pagos
- **Integração FACEIT API**
- **Integração OpenAI API** para busca inteligente

### Frontend (React + TypeScript)
- **React 18** com TypeScript
- **Vite** para build e desenvolvimento
- **Tailwind CSS** para estilização
- **React Router** para navegação
- **Shadcn/ui** para componentes
- **Sistema de Proteção de Rotas** baseado em autenticação
- **Interface Responsiva** com layouts diferenciados

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
**URL**: `http://localhost:5280` (API em `/api`)

### Frontend
```bash
cd ESTop1Client
npm install
npm run dev
```
**URL**: `http://localhost:5173`

## ⚙️ Configuração

### Variáveis de Ambiente Obrigatórias

#### Backend
```bash
# JWT Configuration
JWT_KEY="sua-chave-jwt-super-secreta-para-producao"
JWT_ISSUER="ESTop1"
JWT_AUDIENCE="ESTop1Client"

# OpenAI API (para busca inteligente)
OPENAI_API_KEY="sua-chave-openai"

# FACEIT API (opcional)
FACEIT_TOKEN="seu-token-faceit"

# Database
CONNECTION_STRING="Data Source=estop1.db"
```

#### Frontend
```bash
# API Base URL
VITE_API_BASE_URL="http://localhost:5280/api"  # Desenvolvimento
# VITE_API_BASE_URL="https://api.estop1.com"  # Produção
```

### Configuração de Desenvolvimento

1. **Backend**: Edite `ESTop1/appsettings.Development.json`
2. **Frontend**: Crie arquivo `.env` na pasta `ESTop1Client`

### Configuração de Produção

1. **Backend**: Use `ESTop1/appsettings.Production.json`
2. **Frontend**: Configure variáveis de ambiente no servidor
3. **CORS**: Atualize domínios permitidos
4. **SSL**: Configure certificados HTTPS

## 👥 Sistema de Tipos de Usuário

O ESTop1 diferencia entre três tipos de usuários com funcionalidades específicas:

### 🎮 Jogador
- **Perfil próprio** (sempre acessível)
- **Estatísticas próprias** (sempre acessível)
- **Buscar times** (requer assinatura)
- **Aplicar para vagas** (requer assinatura)
- **Plano gratuito** inicial

### 🏢 Organização
- **Gerenciar jogadores** (requer assinatura)
- **Buscar jogadores** (requer assinatura)
- **Estatísticas avançadas** (requer assinatura)
- **API de integração** (requer assinatura)
- **Criar e gerenciar times**

### 👑 Administrador
- **Acesso total** a todos os recursos
- **Gerenciamento do sistema**
- **Não pode ser registrado** via API pública

## 💳 Sistema de Assinaturas

### Planos Disponíveis
- **Gratuito**: Acesso limitado a jogadores estabelecidos
- **Mensal**: R$ 500,00 - Acesso completo por 30 dias
- **Trimestral**: R$ 1.350,00 - Acesso completo por 90 dias

### Recursos por Plano
- **Gratuito**: Apenas jogadores estabelecidos (sem usuário vinculado)
- **Pago**: Todos os jogadores + Busca por IA + Estatísticas avançadas

## 🔧 Funcionalidades

### ✅ Implementadas
- **Autenticação JWT** com tipos de usuário
- **Sistema de Assinaturas** com planos pagos
- **Gestão de Jogadores** (CRUD completo)
- **Gestão de Times** (CRUD completo)
- **Sistema de Inscrições** para aspirantes
- **Proteção de Rotas** baseada em autenticação
- **Integração FACEIT API** (busca de jogadores)
- **Integração OpenAI** (busca inteligente por IA)
- **Interface Responsiva** (Mobile/Desktop)
- **Filtros e Paginação**
- **Sistema de Autorização** por tipo de usuário

### 🔌 APIs Disponíveis

#### Autenticação
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/registro` - Registro de usuário (Jogador/Organização)
- `GET /api/auth/tipos-usuario` - Obter tipos de usuário disponíveis
- `GET /api/auth/me` - Informações do usuário logado

#### Jogadores
- `GET /api/jogadores` - Listar jogadores (requer assinatura)
- `POST /api/jogadores` - Criar jogador (Organização + assinatura)
- `GET /api/jogadores/{id}` - Detalhes do jogador (requer assinatura)
- `PUT /api/jogadores/{id}` - Atualizar jogador (Organização + assinatura)

#### Times
- `GET /api/times` - Listar times (requer assinatura)
- `POST /api/times` - Criar time (Organização + assinatura)
- `GET /api/times/{id}` - Detalhes do time (requer assinatura)
- `PUT /api/times/{id}` - Atualizar time (Organização + assinatura)

#### Assinaturas
- `GET /api/assinaturas` - Listar planos disponíveis
- `POST /api/times/{id}/assinar` - Assinar plano (Organização)
- `GET /api/assinaturas/me` - Minha assinatura atual

#### Inscrições
- `POST /api/inscricoes` - Nova inscrição (público)

#### FACEIT Integration
- `GET /api/integracoes/faceit/jogador/{nickname}` - Buscar jogador
- `GET /api/integracoes/faceit/estatisticas/{playerId}` - Estatísticas
- `GET /api/integracoes/faceit/partidas/{playerId}` - Histórico de partidas

#### OpenAI Integration (Requer Plano Pago)
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

## 🔐 Sistema de Proteção de Rotas

### Rotas Públicas
- `/` - Página principal
- `/login` - Página de login
- `/selecionar-tipo` - Seleção de tipo de usuário
- `/cadastro` - Cadastro (com tipo selecionado)
- `/inscricao` - Inscrição de aspirantes
- `/inscricao-sucesso` - Confirmação de inscrição

### Rotas Protegidas
- `/jogadores` - Lista de jogadores
- `/jogadores/:id` - Detalhes do jogador
- `/times` - Lista de times
- `/times/:id` - Detalhes do time
- `/assinaturas` - Página de assinaturas
- `/pagamento` - Página de pagamento
- `/pagamento-sucesso` - Confirmação de pagamento

### Fluxo de Autenticação
1. **Usuário não autenticado** acessa rota protegida
2. **Redirecionamento automático** para `/login`
3. **Após login**, redirecionamento para página original
4. **Verificação contínua** de token JWT
5. **Logout** limpa estado e redireciona para home

### Layouts Diferenciados
- **PublicLayout**: Para páginas públicas (header simples)
- **ProtectedLayout**: Para páginas protegidas (navbar + conteúdo)

### Documentação Adicional
- 👥 [USER_TYPES_SYSTEM.md](ESTop1/USER_TYPES_SYSTEM.md) - Sistema de tipos de usuário
- 🔐 [ROUTE_PROTECTION_SYSTEM.md](ESTop1Client/ROUTE_PROTECTION_SYSTEM.md) - Proteção de rotas
- 📝 [OPENAI_SETUP.md](ESTop1/ESTop1/OPENAI_SETUP.md) - Configuração da busca por IA

## 📝 Licença

Este projeto é de uso pessoal.

---
**Desenvolvido com ❤️ para a comunidade de CS2**