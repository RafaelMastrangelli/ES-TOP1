# Sistema de Proteção de Rotas - ESTop1 Frontend

## Visão Geral

O sistema de proteção de rotas garante que usuários não autenticados só tenham acesso às páginas públicas (principal, entrar, cadastrar e inscrever-se), enquanto usuários autenticados têm acesso às funcionalidades completas do sistema.

## Componentes Implementados

### 1. AuthGuard (`/src/components/AuthGuard.tsx`)

Componente principal que controla o acesso às rotas baseado no status de autenticação e tipo de usuário.

#### Funcionalidades:
- **Verificação de Autenticação**: Redireciona usuários não autenticados para login
- **Verificação de Tipo de Usuário**: Controla acesso baseado no tipo (Jogador, Organização, Admin)
- **Loading State**: Mostra spinner durante verificação de autenticação
- **Redirecionamento Inteligente**: Salva a URL original para redirecionar após login

#### Componentes Especializados:
- `ProtectedRoute`: Para rotas que requerem autenticação
- `PublicOnlyRoute`: Para rotas apenas para usuários não autenticados
- `OrganizationRoute`: Para rotas específicas de organizações
- `PlayerRoute`: Para rotas específicas de jogadores

### 2. PublicLayout (`/src/components/PublicLayout.tsx`)

Layout para páginas públicas com:
- **Header Simples**: Logo, navegação básica e botões de login/cadastro
- **Menu de Usuário**: Para usuários autenticados (dropdown com informações)
- **Footer Completo**: Links e informações da empresa
- **Responsivo**: Adapta-se a diferentes tamanhos de tela

### 3. ProtectedLayout (`/src/components/ProtectedLayout.tsx`)

Layout para páginas protegidas com:
- **Sidebar de Navegação**: Menu lateral com todas as funcionalidades
- **Top Bar**: Barra superior com informações do usuário
- **Menu Mobile**: Sidebar responsiva para dispositivos móveis
- **Navegação Contextual**: Destaque da página atual

## Estrutura de Rotas

### Páginas Públicas (Acessíveis sem autenticação)
```
/ - Página principal
/login - Página de login
/cadastro - Página de cadastro
/inscricao - Página de inscrição
/inscricao-sucesso - Confirmação de inscrição
```

### Páginas Protegidas (Requerem autenticação)
```
/jogadores - Lista de jogadores
/jogadores/:id - Detalhes do jogador
/times - Lista de times
/times/:id - Detalhes do time
/assinaturas - Página de assinaturas
/pagamento - Página de pagamento
/pagamento-sucesso - Confirmação de pagamento
```

## Fluxo de Autenticação

### 1. Usuário Não Autenticado
1. Acessa página protegida
2. `AuthGuard` detecta falta de autenticação
3. Redireciona para `/login` salvando URL original
4. Após login, redireciona para URL original

### 2. Usuário Autenticado
1. Acessa qualquer página
2. `AuthGuard` verifica token válido
3. Permite acesso se autorizado
4. Mostra layout apropriado baseado no tipo de usuário

### 3. Logout
1. Remove token do localStorage
2. Atualiza estado de autenticação
3. Redireciona para página principal
4. Mostra layout público

## Hook useAuth Atualizado

### Novas Funcionalidades:
- **Redirecionamento Automático**: Após login/registro/logout
- **Preservação de URL**: Salva página original para redirecionamento
- **Estado de Loading**: Gerencia estado de carregamento durante verificações

### Métodos:
```typescript
const { 
  isAuthenticated, 
  user, 
  assinatura, 
  isLoading, 
  login, 
  register, 
  logout 
} = useAuth();
```

## Proteção por Tipo de Usuário

### Jogadores
- Acesso a: Jogadores, Times, Assinaturas
- Layout: ProtectedLayout com navegação completa

### Organizações
- Acesso a: Jogadores, Times, Assinaturas
- Layout: ProtectedLayout com navegação completa
- Funcionalidades especiais: Gerenciamento de jogadores

### Administradores
- Acesso total a todas as funcionalidades
- Layout: ProtectedLayout com navegação completa

## Estados de Loading

### Durante Verificação de Autenticação
```tsx
<div className="min-h-screen flex items-center justify-center">
  <div className="flex flex-col items-center gap-4">
    <Loader2 className="h-8 w-8 animate-spin text-primary" />
    <p className="text-muted-foreground">Verificando autenticação...</p>
  </div>
</div>
```

### Acesso Negado
```tsx
<div className="min-h-screen flex items-center justify-center">
  <div className="text-center">
    <h1 className="text-2xl font-bold text-destructive mb-2">
      Acesso Negado
    </h1>
    <p className="text-muted-foreground mb-4">
      Você não tem permissão para acessar esta página.
    </p>
  </div>
</div>
```

## Responsividade

### Desktop
- Sidebar fixa para páginas protegidas
- Header completo com todas as informações
- Navegação horizontal no header público

### Mobile
- Menu hambúrguer para páginas protegidas
- Sidebar deslizante
- Navegação simplificada no header público

## Segurança

### Proteções Implementadas:
1. **Verificação de Token**: Valida token JWT em cada requisição
2. **Redirecionamento Forçado**: Impede acesso direto via URL
3. **Limpeza de Estado**: Remove dados sensíveis no logout
4. **Validação de Tipo**: Verifica tipo de usuário para recursos específicos

### Comportamento:
- Usuários não autenticados são sempre redirecionados para login
- Usuários autenticados tentando acessar páginas públicas são redirecionados para home
- Tokens inválidos/expirados são limpos automaticamente
- Estado de autenticação é verificado a cada carregamento da página

## Exemplo de Uso

### Rota Protegida
```tsx
<Route path="/jogadores" element={
  <ProtectedRoute>
    <ProtectedLayout>
      <Jogadores />
    </ProtectedLayout>
  </ProtectedRoute>
} />
```

### Rota Pública
```tsx
<Route path="/login" element={
  <PublicOnlyRoute>
    <PublicLayout>
      <Login />
    </PublicLayout>
  </PublicOnlyRoute>
} />
```

### Rota com Restrição de Tipo
```tsx
<Route path="/admin" element={
  <OrganizationRoute>
    <ProtectedLayout>
      <AdminPanel />
    </ProtectedLayout>
  </OrganizationRoute>
} />
```

## Benefícios

1. **Segurança**: Proteção robusta contra acesso não autorizado
2. **UX**: Redirecionamento inteligente e preservação de contexto
3. **Flexibilidade**: Sistema modular e extensível
4. **Responsividade**: Adaptação a diferentes dispositivos
5. **Manutenibilidade**: Código organizado e reutilizável
