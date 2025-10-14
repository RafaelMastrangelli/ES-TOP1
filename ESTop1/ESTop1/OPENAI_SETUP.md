# 🤖 Configuração da API OpenAI

## Como Configurar sua Chave da API

### 1. Obter a Chave da API
1. Acesse: https://platform.openai.com/account/api-keys
2. Faça login na sua conta OpenAI
3. Clique em "Create new secret key"
4. Copie a chave (começa com `sk-`)

### 2. Configurar nos User Secrets
Execute o comando abaixo substituindo `SUA_CHAVE_AQUI` pela sua chave real:

```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-sua-chave-real-aqui"
```

### 3. Verificar Configuração
```bash
dotnet user-secrets list
```

### 4. Executar a API
```bash
dotnet run
```

## ✅ Vantagens dos User Secrets

- ✅ **Seguro**: Chaves não ficam no código
- ✅ **Local**: Apenas no seu ambiente
- ✅ **Fácil**: Comando simples para configurar
- ✅ **Padrão**: Prática recomendada do .NET

## 🔧 Comandos Úteis

```bash
# Listar todos os secrets
dotnet user-secrets list

# Remover um secret
dotnet user-secrets remove "OpenAI:ApiKey"

# Limpar todos os secrets
dotnet user-secrets clear
```

## 🚀 Testando a Funcionalidade

1. Execute a API: `dotnet run`
2. Acesse: http://localhost:5059/swagger
3. Teste o endpoint: `/api/integracoes/openai/buscar-jogadores`
4. Use no frontend: Campo "Buscar com IA"
