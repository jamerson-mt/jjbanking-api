# 🏦 JJ Banking API
> **"A espinha dorsal financeira robusta para suas aplicações modernas."**

![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Identity](https://img.shields.io/badge/Security-ASP.NET_Core_Identity-ff69b4?style=for-the-badge)

A **JJ Banking API** é um motor de serviços financeiros de alta performance. Construída com **.NET 10**, esta API oferece uma infraestrutura de "Core Banking" real, com autenticação segura, geração automática de contas e persistência transacional.

---

## 📖 Índice
- [Visão Geral](#-visão-geral)
- [Diferenciais Estratégicos](#-diferenciais-estratégicos)
- [Arquitetura e Qualidade](#-arquitetura-e-qualidade)
- [Guia de Integração (Mobile/Front)](#-guia-de-integração-mobilefront)
- [Setup Instantâneo](#-setup-instantâneo-com-docker)

---

## 🌟 Visão Geral

A proposta da **JJ Banking API** é oferecer um backend financeiro persistente e documentado. Diferente de mocks estáticos, aqui os dados são reais: ao registrar um usuário, o sistema gera automaticamente um **Número de Conta Único** e gerencia o saldo de forma segura.

### 🎯 Para quem é este projeto?
* **Devs React Native / Mobile:** Consuma uma API real com documentação rica (XML Comments) e exemplos prontos no Swagger.
* **Devs Backend:** Explore Clean Architecture, Identity Framework e EF Core 10 com PostgreSQL.

---

## 💎 Diferenciais Estratégicos

1. **Identity & Security:** Autenticação completa via ASP.NET Core Identity (Gestão de usuários, senhas e claims).
2. **Geração Automática de Conta:** Ao se cadastrar, o usuário recebe instantaneamente um número de conta (Ex: `4829-2`) validado contra duplicidade.
3. **Documentação de Elite:** Swagger configurado com comentários XML e exemplos de DTOs, facilitando a vida do time de Frontend.
4. **Resiliência Transacional:** Uso de Database Transactions para garantir que a criação do usuário e da conta ocorram juntas ou falhem juntas.

---

## 🏗️ Arquitetura e Qualidade

O projeto segue os princípios da **Clean Architecture**, garantindo total separação de responsabilidades:

* **`JJBanking.Domain`**: Entidades, Interfaces, DTOs (Data Transfer Objects) e as regras de ouro do banco.
* **`JJBanking.Infra`**: Persistência com PostgreSQL, IdentityDbContext e implementação dos Services.
* **`JJBanking.API`**: Controllers documentados e orquestração de Injeção de Dependência.

---

## 🛠️ Guia de Integração (Mobile/Front)

A API utiliza **Swagger/OpenAPI** turbinado com comentários XML. Acesse para ver os exemplos de JSON:

👉 [http://localhost:5000/swagger](http://localhost:5000/swagger)

### 🔐 Autenticação (`/api/Auth`)
* `POST /api/Auth/register` - Cria usuário e gera o número da conta.
* `POST /api/Auth/login` - Autentica e retorna o Token de acesso.

### 💸 Operações Bancárias (`/api/Transaction`)
* `POST /api/Transaction/deposit` - Realiza aporte financeiro via número da conta.
* `POST /api/Transaction/withdraw` - Realiza saque com validação de saldo em tempo real.

---

## 🚀 Setup Instantâneo com Docker

1. **Clone o repositório:**
   ```bash
   git clone [https://github.com/jamerson-mt/jjbanking-api.git](https://github.com/jamerson-mt/jjbanking-api.git)
   cd jjbanking-api