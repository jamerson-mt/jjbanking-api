# 🏦 JJ Banking API
> **"A espinha dorsal financeira para suas aplicações modernas."**

![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/Tests-xUnit%20%26%20FluentAssertions-green?style=for-the-badge)

A **JJ Banking API** é um motor de serviços financeiros de alta performance desenvolvido para ser o "Core Banking" definitivo para desenvolvedores. Construída com o que há de mais moderno no ecossistema **.NET 10**, esta API oferece uma infraestrutura robusta, escalável e pronta para uso em projetos de Front-end, Mobile ou sistemas de gestão financeira.

---

## 📖 Índice
- [Visão Geral](#-visão-geral)
- [Diferenciais Estratégicos](#-diferenciais-estratégicos)
- [Stack Tecnológica](#-stack-tecnológica)
- [Arquitetura e Qualidade](#-arquitetura-e-qualidade)
- [Guia de Endpoints](#-guia-de-endpoints)
- [Setup Instantâneo](#-setup-instantâneo-com-docker)

---

## 🌟 Visão Geral

A proposta da **JJ Banking API** é eliminar a necessidade de mocks estáticos e oferecer um backend financeiro persistente e documentado para a comunidade. Seja para validar um fluxo de carteira digital em React ou para estudar padrões de concorrência em transações bancárias, a JJ API entrega uma infraestrutura pronta para integrar.

### 🎯 Para quem é este projeto?
* **Devs Frontend/Mobile:** Consuma uma API real via Swagger e veja seus dados persistidos em um banco PostgreSQL.
* **Devs Backend:** Explore as novas funcionalidades do **C# 14** e a robustez do **Entity Framework Core 10**.
* **Open Source:** Use como base para criar novas funcionalidades e aprimorar seus conhecimentos em arquitetura de sistemas.

---

## 💎 Diferenciais Estratégicos

1. **Persistência Industrial:** Utiliza **PostgreSQL**, garantindo integridade transacional e suporte a grandes volumes de dados via EF Core 10.
2. **Invariantes de Domínio:** Regras de negócio protegidas (saldo insuficiente, transações negativas, validações de CPF) diretamente nas Entidades.
3. **Segurança de Dados:** Uso rigoroso de **DTOs (Records)** para garantir que a API nunca exponha detalhes internos das entidades de banco.
4. **Developer Experience (DX):** Foco total no "Clonou, Rodou" via Docker Compose e documentação automática.

---

## 🏗️ Arquitetura e Qualidade

O projeto segue os princípios da **Clean Architecture**, garantindo que a lógica de negócio seja independente de frameworks externos:

### Camadas do Projeto:
* **`JJBanking.Domain`**: O coração do projeto. Contém as Entidades (`Account`, `Transaction`), Enums e as Regras de Negócio.
* **`JJBanking.Infra`**: Camada de persistência. Gerencia o Contexto do Entity Framework, Repositórios e as Migrations.
* **`JJBanking.API`**: Porta de entrada. Gerencia Controllers, DTOs de Request/Response e Injeção de Dependência.

### 🛡️ Qualidade de Software (Testes Automatizados):
Para garantir a confiabilidade bancária, o projeto conta com:
* **Testes Unitários (xUnit):** Validação de regras isoladas no domínio (ex: impedir saques negativos).
* **Testes de Integração:** Validação do fluxo completo (API -> Service -> Banco) usando `WebApplicationFactory` e banco de dados real em memória.
* **FluentAssertions:** Escrita de testes semânticos e de fácil manutenção.

---
## 🛠️ Guia de Endpoints (v1)

A API utiliza **Swagger/OpenAPI** para documentação. Ao rodar o projeto, acesse `http://localhost:5000/swagger` para testar os endpoints interativamente.

### Rota do Swagger: 


### 👤 Gerenciamento de Contas (`/api/accounts`)

| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `POST` | `/api/accounts` | Cria uma nova conta bancária. |
| `GET` | `/api/accounts/{id}` | Recupera detalhes e saldo atual. |

**Exemplo de Payload (POST):**
```json
{
  "owner": "Jamerson Silva",
  "cpf": "12345678901",
  "initialDeposit": 500.00
}

```

---

## 🚀 Setup Instantâneo com Docker

Esqueça configurações manuais. Com o Docker, você sobe toda a stack com apenas um comando:

1. **Clone o repositório:**
   ```bash
   git clone [https://github.com/jamerson-mt/jj-banking-api.git](https://github.com/jamerson-mt/jj-banking-api.git)
   cd jj-banking-api