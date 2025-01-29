# 🔗 YARP Gateway com JWT e Redirecionamento de Requisições

## Obs.: Projeto de portfólio

## 📌 Visão Geral
Este projeto é um **gateway HTTP** utilizando o **YARP (Yet Another Reverse Proxy)**, responsável por redirecionar requisições para outras APIs, além de fornecer uma rota para **autenticação JWT**, gerando tokens de acesso para as APIs protegidas.

## 🚀 Tecnologias Utilizadas
- **ASP.NET Core**
- **YARP (Yet Another Reverse Proxy)**
- **JWT (JSON Web Token)** para autenticação
- **BCrypt** para validação de senhas

## 🔐 Autenticação e Identidade
O sistema utiliza **JWT** para autenticação, permitindo:
- **Gerar tokens JWT** com tempo de expiração configurável.
- Assinatura dos tokens com HMAC-SHA256.
- Reivindicações (**Claims**) customizadas, incluindo usuário e roles.

Embora o projeto não use o **ASP.NET Identity** completo, ele implementa recursos essenciais como:
- Hash de senha usando **BCrypt**.
- Validação de roles para controle de acesso.

## 📄 Endpoints

### 🔐 Login e Geração de JWT
```http
POST /auth/user/login
```
**Requer autenticação:** ❌ Não

#### 📥 Corpo da Requisição
```json
{
  "email": "usuario@example.com",
  "password": "senha123"
}
```

#### 📤 Resposta de Sucesso
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2025-01-29T10:00:00Z"
}
```

#### 📤 Resposta de Erro
```json
{
  "error": "Credenciais inválidas."
}
```

---

### 🔍 Endpoint Protegido (Exemplo)
```http
GET /auth/user
```
**Requer autenticação:** ✅ Sim (Role: Admin)

#### 📤 Resposta de Sucesso
```json
"Hello World Admin!"
```

---

## 🛠 Redirecionamento com YARP
O YARP é configurado para redirecionar requisições de acordo com rotas personalizadas. Exemplos:

### 🔄 Rota de Redirecionamento
```http
GET /products
```
Esta rota pode ser redirecionada para uma API de destino, como:
```
https://api.dominio-interno.com/api/products
```

### 📋 Configuração do YARP (appsettings.json)
```json
{
  "ReverseProxy": {
    "Routes": {
      "ProductsRoute": {
        "ClusterId": "ProductsCluster",
        "Match": {
          "Path": "/products"
        }
      }
    },
    "Clusters": {
      "ProductsCluster": {
        "Destinations": {
          "ProductsApi": {
            "Address": "https://api.dominio-interno.com/api/products"
          }
        }
      }
    }
  }
}
```

---

## 🛠 Como Rodar o Projeto
1. Clone o repositório:
   ```sh
   git clone https://github.com/seu-usuario/seu-repositorio.git
   ```
2. Instale as dependências:
   ```sh
   dotnet restore
   ```
3. Configure o **appsettings.json** com:
   - Segredo do JWT: `JwtToken:Secret`
   - Configuração de rotas do YARP.
4. Execute a API:
   ```sh
   dotnet run
   ```

## 🏆 Contribuição
Sinta-se à vontade para abrir issues ou enviar PRs para melhorias! 😃

