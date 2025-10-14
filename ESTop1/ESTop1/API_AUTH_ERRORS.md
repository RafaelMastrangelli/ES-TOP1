# Documentação de Erros de Autenticação

## Endpoint: POST /api/auth/login

### Cenários de Erro e Respostas

#### 1. Email vazio ou nulo
**Request:**
```json
{
  "email": "",
  "senha": "123456"
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Email é obrigatório",
  "details": null,
  "errorCode": "EMAIL_REQUIRED",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 2. Senha vazia ou nula
**Request:**
```json
{
  "email": "usuario@exemplo.com",
  "senha": ""
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Senha é obrigatória",
  "details": null,
  "errorCode": "PASSWORD_REQUIRED",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 3. Formato de email inválido
**Request:**
```json
{
  "email": "email-invalido",
  "senha": "123456"
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Formato de email inválido",
  "details": null,
  "errorCode": "INVALID_EMAIL_FORMAT",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 4. Credenciais incorretas
**Request:**
```json
{
  "email": "usuario@exemplo.com",
  "senha": "senha_errada"
}
```

**Response (401 Unauthorized):**
```json
{
  "message": "Email ou senha incorretos",
  "details": null,
  "errorCode": "INVALID_CREDENTIALS",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 5. Conta desativada
**Response (401 Unauthorized):**
```json
{
  "message": "Conta desativada. Entre em contato com o suporte.",
  "details": null,
  "errorCode": "ACCOUNT_DISABLED",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 6. Erro interno do servidor
**Response (500 Internal Server Error):**
```json
{
  "message": "Erro interno do servidor. Tente novamente mais tarde.",
  "details": "Detalhes técnicos do erro (apenas em desenvolvimento)",
  "errorCode": "INTERNAL_ERROR",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

## Endpoint: POST /api/auth/registro

### Cenários de Erro e Respostas

#### 1. Nome vazio
**Response (400 Bad Request):**
```json
{
  "message": "Nome é obrigatório",
  "details": null,
  "errorCode": "NAME_REQUIRED",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 2. Email já existe
**Response (400 Bad Request):**
```json
{
  "message": "Email já está em uso",
  "details": null,
  "errorCode": "EMAIL_ALREADY_EXISTS",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

#### 3. Senha muito curta
**Response (400 Bad Request):**
```json
{
  "message": "Senha deve ter pelo menos 6 caracteres",
  "details": null,
  "errorCode": "PASSWORD_TOO_SHORT",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

## Códigos de Erro Disponíveis

- `EMAIL_REQUIRED`: Email é obrigatório
- `PASSWORD_REQUIRED`: Senha é obrigatória
- `NAME_REQUIRED`: Nome é obrigatório
- `INVALID_EMAIL_FORMAT`: Formato de email inválido
- `INVALID_CREDENTIALS`: Email ou senha incorretos
- `ACCOUNT_DISABLED`: Conta desativada
- `EMAIL_ALREADY_EXISTS`: Email já está em uso
- `PASSWORD_TOO_SHORT`: Senha muito curta
- `VALIDATION_ERROR`: Erro de validação
- `AUTH_ERROR`: Erro de autenticação
- `INTERNAL_ERROR`: Erro interno do servidor

## Como Testar

### Usando cURL

```bash
# Teste de email vazio
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "", "senha": "123456"}'

# Teste de credenciais incorretas
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "teste@exemplo.com", "senha": "senha_errada"}'
```

### Usando Postman

1. Configure o método como POST
2. URL: `http://localhost:5000/api/auth/login`
3. Headers: `Content-Type: application/json`
4. Body (raw JSON): Use os exemplos acima
