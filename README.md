# Authentication & Authorization

## Sobre o Projeto

Este projeto foi criado para servir como um exemplo de como implementar autenticação e autorização em ASP.NET Core utilizando JWT. Ao invés de armazenar os tokens JWT no LocalStorage ou SessionStorage, este projeto opta por armazená-los em cookies, proporcionando uma camada adicional de segurança contra potenciais ataques XSS.

## Tecnologias Usadas

- ASP.NET Core 7.0.400
- JWT (JSON Web Tokens)
- BCrypt.Net-Next - Para hashing seguro de senhas

## Endpoints

### Grupo: Auth

- `POST /api/Auth/Register`:
  - Descrição: Endpoint para registrar um novo usuário.
  - Payload:
    - `username`: Nome do usuário (string).
    - `password`: Senha do usuário (string).

- `POST /api/Auth/RegisterAdmin`:
  - Descrição: Endpoint para registrar um novo administrador.
  - Payload:
    - `username`: Nome do usuário (string).
    - `password`: Senha do usuário (string).

- `POST /api/Auth/Login`:
  - Descrição: Endpoint para autenticar um usuário (utiliza cookies).
  - Payload:
    - `username`: Nome do usuário (string).
    - `password`: Senha do usuário (string).

- `POST /api/Auth/LoginApi`:
  - Descrição: Endpoint para autenticar um usuário via API (não utiliza cookies).
  - Payload:
    - `username`: Nome do usuário (string).
    - `password`: Senha do usuário (string).

- `GET /api/Auth/Users`:
  - Permissões: `Admin`
  - Descrição: Endpoint para obter uma lista de usuários `Default`.

### Grupo: WeatherForecast

- `GET /api/WeatherForecast`:
  - Permissões: `Admin`, `Default`
  - Descrição: Endpoint para obter uma previsão do tempo.
  - Resposta:
    - `date`: Data da previsão (string).
    - `temperatureC`: Temperatura em Celsius (int).
    - `temperatureF`: Temperatura em Fahrenheit (int, somente leitura).
    - `summary`: Sumário da previsão (string, opcional).
