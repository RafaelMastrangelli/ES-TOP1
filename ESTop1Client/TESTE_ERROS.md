# Teste de Erros de Login - Frontend

## Como Testar os Erros Específicos

### 1. Acesse o Frontend
- URL: http://localhost:5173 (ou a porta que aparecer no terminal)
- Vá para a página de Login

### 2. Teste os Cenários de Erro

#### ✅ Email vazio
- Deixe o campo email vazio
- Digite qualquer senha
- Clique em "Entrar"
- **Resultado esperado**: "Email é obrigatório"

#### ✅ Senha vazia  
- Digite um email válido
- Deixe o campo senha vazio
- Clique em "Entrar"
- **Resultado esperado**: "Senha é obrigatória"

#### ✅ Email inválido
- Digite: "email-invalido" (sem @)
- Digite qualquer senha
- Clique em "Entrar"
- **Resultado esperado**: "Formato de email inválido"

#### ✅ Credenciais incorretas
- Digite: "teste@exemplo.com"
- Digite: "senha_errada"
- Clique em "Entrar"
- **Resultado esperado**: "Email ou senha incorretos"

#### ✅ Conta desativada
- Use credenciais de uma conta desativada
- **Resultado esperado**: "Conta desativada. Entre em contato com o suporte."

### 3. Verificar no Console do Navegador

Abra o DevTools (F12) e vá para a aba Console para ver:
- Requisições HTTP sendo feitas
- Respostas de erro do backend
- Logs de debug

### 4. Verificar na Aba Network

Na aba Network do DevTools:
- Veja as requisições POST para `/api/auth/login`
- Verifique o status code (400, 401, 500)
- Veja o corpo da resposta de erro

## Estrutura da Resposta de Erro

O backend agora retorna erros no formato:

```json
{
  "message": "Mensagem específica do erro",
  "errorCode": "CODIGO_DO_ERRO", 
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

## Códigos de Erro Disponíveis

- `EMAIL_REQUIRED` - Email é obrigatório
- `PASSWORD_REQUIRED` - Senha é obrigatória
- `INVALID_EMAIL_FORMAT` - Formato de email inválido
- `INVALID_CREDENTIALS` - Email ou senha incorretos
- `ACCOUNT_DISABLED` - Conta desativada
- `INTERNAL_ERROR` - Erro interno do servidor

## Status HTTP

- `400 Bad Request` - Dados inválidos
- `401 Unauthorized` - Credenciais incorretas/conta desativada  
- `500 Internal Server Error` - Erros internos
