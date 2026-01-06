# 🎮 FGC – Games Micro Service

Repositório oficial do **FIAP Cloud Games (FGC)**, API backend desenvolvida em **.NET 8** como parte do **Tech Challenge FIAP – Fase 1** da FIAP.

## 📦 Visão Geral

O **FGC** simula uma **loja virtual de jogos digitais** com recursos completos de autenticação, catálogo, promoções e bibliotecas de jogos por usuário.

### Funcionalidades:

 
- 🎮 Cadastro, listagem, compra e recomendação de jogos. 

## ⚙️ Tecnologias Utilizadas

- [.NET 8 (C#)](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- [JWT Bearer Authentication](https://jwt.io/)
---

## 🚀 Como Executar o Projeto

### 1️⃣ Clonar o Repositório

```bash
git clone https://github.com/VitorDietrich-Coder/games-service.git

cd ../games-service/Games.Microservice.API

dotnet restore
```
## 🌐 Configuração de Host

#### 👤 Autenticação no SQL Server

Alterar no Arquivo AppsettingsDevelop.json

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=DB_Games;User Id=seu_usuario;Password=sua_senha_segura;TrustServerCertificate=True;"
}

Para rodar o projeto execute:
dotnet run --project ./src/FGC.Api

## Dados inseridos:

#### 👤 Usuários

adminnew@fiapgames.com (Admin)

usernew@fiapgames.com (Usuário comum)

####  🎮 Jogos

4 títulos com nome, categoria e preço

## 🔐 Credenciais de Acesso

####   👤 Usuário Comum

Email: usernew@fiapgames.com

Senha: 1GamesTeste@

####  👑 Usuário Administrador

Email: adminnew@fiapgames.com

Senha: 1GamesAdmin@

##  📄 Documentação

 

##  🐳 Rodando a Aplicação com docker
Rode no console:

```bash
cd ./games-service

docker compose up -d
```

Com esse comando irá subir a API juntamente com o grafana e o prometheus, 
tendo em vista que deixei um docker compose para empacotar e subir mais facil localmente.

docker-compose.yml

Após isso teremos os seguintes serviços:

API: http://localhost:8080

Grafana: http://localhost:3000
(usuário/padrão: admin / admin)

Prometheus: http://localhost:9090/query



