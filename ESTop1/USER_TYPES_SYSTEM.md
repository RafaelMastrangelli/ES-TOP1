# Sistema de Tipos de Usuário - ESTop1

## Visão Geral

O sistema ESTop1 agora diferencia entre dois tipos principais de usuários com regras de acesso específicas:

1. **Jogador** - Usuário individual que busca oportunidades
2. **Organização** - Time/Organização que gerencia jogadores e busca talentos
3. **Admin** - Administrador do sistema (acesso total)

## Tipos de Usuário

### 1. Jogador (`TipoUsuario.Jogador`)
- **Descrição**: Jogador individual que pode buscar times e aplicar para vagas
- **Recursos Disponíveis**:
  - Perfil próprio (sempre permitido)
  - Estatísticas próprias (sempre permitido)
  - Buscar times (requer assinatura)
  - Aplicar vagas (requer assinatura)
- **Assinatura**: Começa com plano gratuito
- **Limitações**: Não pode gerenciar outros jogadores

### 2. Organização (`TipoUsuario.Organizacao`)
- **Descrição**: Organização ou time que pode gerenciar jogadores e buscar talentos
- **Recursos Disponíveis**:
  - Gerenciar jogadores (requer assinatura)
  - Buscar jogadores (requer assinatura)
  - Estatísticas avançadas (requer assinatura)
  - API de integração (requer assinatura)
- **Assinatura**: Começa com plano gratuito mas com limitações
- **Privilégios**: Pode criar e gerenciar jogadores

### 3. Admin (`TipoUsuario.Admin`)
- **Descrição**: Administrador do sistema
- **Recursos Disponíveis**: Acesso total a todos os recursos
- **Privilégios**: Não pode ser registrado via API, apenas criado diretamente no banco

## Sistema de Autorização

### Atributos de Autorização

#### `[AuthorizeUserType(TipoUsuario.Jogador, TipoUsuario.Admin)]`
- Autoriza apenas jogadores e administradores

#### `[AuthorizeOrganizacao]`
- Autoriza apenas organizações e administradores

#### `[AuthorizeJogador]`
- Autoriza apenas jogadores e administradores

#### `[AuthorizeAdmin]`
- Autoriza apenas administradores

### Middleware de Assinatura Atualizado

O middleware `AssinaturaMiddleware` agora considera o tipo de usuário nas regras de acesso:

```csharp
// Regras específicas por tipo de usuário
switch (tipoUsuario)
{
    case TipoUsuario.Organizacao:
        // Organizações podem acessar recursos de gerenciamento
        if (recurso == "gerenciar_jogadores" || recurso == "buscar_jogadores" || recurso == "estatisticas")
            return await assinaturaService.VerificarAcessoAsync(userId, recurso);
        break;

    case TipoUsuario.Jogador:
        // Jogadores têm acesso limitado
        if (recurso == "perfil_proprio" || recurso == "estatisticas_proprias")
            return true; // Sempre permitido
        if (recurso == "buscar_times" || recurso == "aplicar_vagas")
            return await assinaturaService.VerificarAcessoAsync(userId, recurso);
        break;
}
```

## Endpoints Atualizados

### AuthController
- `POST /api/auth/registro` - Agora valida o tipo de usuário
- `GET /api/auth/tipos-usuario` - Novo endpoint para obter tipos disponíveis

### JogadoresController
- `GET /api/jogadores` - Requer autorização e assinatura para "buscar_jogadores"
- `GET /api/jogadores/{id}` - Requer autorização e assinatura para "estatisticas"
- `POST /api/jogadores` - Requer autorização de organização e assinatura para "gerenciar_jogadores"
- `POST /api/jogadores/atualizar-fotos` - Requer autorização de admin

### TimesController
- `GET /api/times` - Requer autorização e assinatura para "buscar_times"
- `GET /api/times/{id}` - Requer autorização e assinatura para "buscar_times"
- `POST /api/times` - Requer autorização de organização e assinatura para "gerenciar_jogadores"

## Recursos por Tipo de Usuário

### Recursos de Jogador
- `perfil_proprio` - Sempre permitido
- `estatisticas_proprias` - Sempre permitido
- `buscar_times` - Requer assinatura
- `aplicar_vagas` - Requer assinatura

### Recursos de Organização
- `gerenciar_jogadores` - Requer assinatura
- `buscar_jogadores` - Requer assinatura
- `estatisticas` - Requer assinatura
- `api_integracao` - Requer assinatura

## Fluxo de Registro

1. **Validação**: O sistema valida se o tipo de usuário é válido
2. **Criação**: Usuário é criado com o tipo especificado
3. **Assinatura**: Assinatura gratuita é criada automaticamente
4. **Token**: Token JWT inclui o tipo de usuário

## Exemplo de Uso

### Registro de Jogador
```json
POST /api/auth/registro
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "123456",
  "tipo": "Jogador"
}
```

### Registro de Organização
```json
POST /api/auth/registro
{
  "nome": "Team Liquid",
  "email": "contato@teamliquid.com",
  "senha": "123456",
  "tipo": "Organizacao"
}
```

### Obter Tipos Disponíveis
```json
GET /api/auth/tipos-usuario
```

## Segurança

- **Validação de Tipo**: Tipos inválidos são rejeitados
- **Autorização por Recurso**: Cada endpoint verifica o tipo de usuário
- **Assinatura**: Recursos premium requerem assinatura ativa
- **Admin**: Não pode ser registrado via API pública

## Migração de Dados

O sistema foi atualizado para usar `TipoUsuario.Organizacao` em vez de `TipoUsuario.Time` para maior clareza. A migração foi aplicada automaticamente ao banco de dados.
