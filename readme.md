# 💰 Controle de Gastos Residenciais

Sistema para gerenciamento de gastos residenciais, com cadastro de pessoas, categorias e transações financeiras.

---

## 🚀 Como rodar

Pré-requisito: ter o **Docker** e o **Docker Compose** instalados.

```bash
docker compose up --build
```

Aguarde o build finalizar e acesse:

| Interface | URL |
|-----------|-----|
| Front-end | http://localhost:3000 |
| Swagger (API) | http://localhost:5285/index.html |

---

## 🧩 Funcionalidades

- **Pessoas** — cadastro completo (criar, editar, deletar, listar). Ao deletar uma pessoa, todas as suas transações são removidas automaticamente.
- **Categorias** — cadastro com finalidade: Despesa, Receita ou Ambas.
- **Transações** — vinculadas a uma pessoa e categoria. Menores de 18 anos só podem registrar despesas. A categoria é filtrada automaticamente pelo tipo da transação.
- **Totais por Pessoa** — resumo de receitas, despesas e saldo individual, com total geral ao final.
- **Totais por Categoria** — mesma visão agrupada por categoria.

---

## 🛠️ Tecnologias

**Back-end:** C# · .NET · Web API  
**Front-end:** React · TypeScript · Tailwind CSS · Zod  
**Infra:** Docker · Docker Compose · Nginx

---

## 📁 Estrutura

```
/
├── Backend/        # Web API em .NET
├── Frontend/       # React + TypeScript
└── docker-compose.yml
```