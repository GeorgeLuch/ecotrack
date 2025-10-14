# EcoTrack API — CI/CD, Docker e Orquestração

> API .NET 8 containerizada, orquestrada com Docker Compose e entregue por CI/CD no GitHub Actions.  
> Imagem pública no **GHCR** e deploy automático em **Staging (5001)** e **Produção (5000)**.  
> Ambiente **Dev** roda localmente na **5002**.

[![CI (.NET)](https://github.com/GeorgeLuch/ecotrack/actions/workflows/ci-dotnet.yml/badge.svg?branch=main)](https://github.com/GeorgeLuch/ecotrack/actions/workflows/ci-dotnet.yml)
[![Docker Publish (GHCR)](https://github.com/GeorgeLuch/ecotrack/actions/workflows/docker-publish.yml/badge.svg?branch=main)](https://github.com/GeorgeLuch/ecotrack/actions/workflows/docker-publish.yml)
[![Deploy (self-hosted)](https://github.com/GeorgeLuch/ecotrack/actions/workflows/deploy-selfhosted.yml/badge.svg?branch=main)](https://github.com/GeorgeLuch/ecotrack/actions/workflows/deploy-selfhosted.yml)

---

## 0) Equipe e título

**Projeto:** EcoTrack API — Pipeline CI/CD, Containerização e Orquestração  
**Integrantes:** _preencha com os nomes_  
**Professor/Disciplina:** _preencha_

---

## ✅ Pré-requisitos

- **Docker Desktop** (Windows com WSL2) ou Docker Engine.
- Portas livres no host: **5000**, **5001**, **5002** (API) e **1433** (SQL Server).
- Imagem pública: **`ghcr.io/georgeluch/ecotrack`**.
- (Deploy) Runner self-hosted aberto em `C:\actions-runner\run.cmd`.

---

## 🚀 Como executar localmente com Docker

> Execute os comandos **na raiz do repositório**.

### Opção A — Docker Compose (Dev com SQL em container)
```bash
# Sobe API (5002) + SQL (1433)
docker compose up -d

# Ver status dos serviços
docker compose ps

# Logs da API (Ctrl+C para sair)
docker compose logs -f api
```
Acesse: **http://localhost:5002/swagger**

> Se o SQL ficar *unhealthy*: confirme `SA_PASSWORD`, libere espaço no WSL2/Docker e reinicie o Docker Desktop.

### Opção B — Somente a API usando SQL externo (docker run)
> Conecta a API em um SQL Server rodando no host (porta 1433).
```bash
docker run -d --name ecotrack-api-dev -p 5002:8080 ^
  -e ASPNETCORE_ENVIRONMENT=Development ^
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=EcoTrack_Dev;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=False;TrustServerCertificate=True;" ^
  ghcr.io/georgeluch/ecotrack:latest
```

**Parar/limpar (quando precisar)**
```bash
docker compose down          # para e remove containers
docker compose down -v       # idem + apaga volumes (zera banco)
docker rm -f ecotrack-api-dev
```

---

## 🔁 Pipeline CI/CD (GitHub Actions)

**Workflows:**
- **CI (.NET)** — `/.github/workflows/ci-dotnet.yml`  
  _checkout → setup-dotnet 8 → `dotnet restore` → `dotnet build -c Release` → `dotnet test`_
- **Docker Publish (GHCR)** — `/.github/workflows/docker-publish.yml`  
  _login no GHCR → build → push **:latest** (push na `main`) e **:v*** (quando há tag)_
- **Deploy (self-hosted)** — `/.github/workflows/deploy-selfhosted.yml`  
  - **Staging (5001):** dispara em **push** na `main`, puxa `ghcr.io/georgeluch/ecotrack:latest` e sobe `docker-compose.deploy.staging.yml`  
    → **http://localhost:5001/swagger**
  - **Produção (5000):** dispara em **tag** `v*` (ex.: `v1.0.1`), puxa a tag e sobe `docker-compose.deploy.yml` usando `IMAGE_TAG`  
    → **http://localhost:5000/swagger**

**Como disparar manualmente:**
```bash
# Staging
git push

# Produção (crie e envie uma tag v*)
git tag v1.0.1
git push origin v1.0.1
```
> **Importante:** manter `C:\actions-runner\run.cmd` aberto para o job de deploy executar no self-hosted runner.

---

## 🐳 Containerização

**Imagem pública:** `ghcr.io/georgeluch/ecotrack`  
A aplicação escuta **8080** dentro do container (mapeada para 5000/5001/5002 no host).

**Dockerfile (multi-stage .NET 8):**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore && dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EcoTrack.api.dll"]
```

**Estratégias adotadas:**
- **Multi-stage build** (imagem final menor/rápida).
- **Binding explícito** `ASPNETCORE_URLS=http://+:8080` (garante que a API responda no container).
- **EXPOSE 8080** + mapeamentos de porta no host.
- **Compose por ambiente** (Dev/Staging/Prod) para isolar configurações.

---

## 🧩 Orquestração (Docker Compose)

Arquivos na raiz do repo:
- `docker-compose.yml` — **Dev** (API 5002 + SQL 1433)  
- `docker-compose.deploy.staging.yml` — **Staging** (API 5001, usa **:latest**)  
- `docker-compose.deploy.yml` — **Produção** (API 5000, usa **IMAGE_TAG**)  

**Comandos úteis:**
```bash
# Dev (local)
docker compose up -d
docker compose ps

# Staging (imagem :latest do GHCR)
docker compose -f docker-compose.deploy.staging.yml up -d
docker compose -f docker-compose.deploy.staging.yml ps

# Produção (usa tag)
set IMAGE_TAG=v1.0.1   # no Windows CMD
docker compose -f docker-compose.deploy.yml up -d
docker compose -f docker-compose.deploy.yml ps
```

**Tabela de ambientes e portas**

| Ambiente | Como sobe                                                       | URL Swagger                    | Imagem usada                                   |
|----------|-----------------------------------------------------------------|--------------------------------|------------------------------------------------|
| **Dev**  | `docker compose up -d`                                          | http://localhost:5002/swagger  | `ecotrackapi-api` (build local)                |
| **Stg**  | `docker compose -f docker-compose.deploy.staging.yml up -d`     | http://localhost:5001/swagger  | `ghcr.io/georgeluch/ecotrack:latest`           |
| **Prod** | `set IMAGE_TAG=v1.0.1 && docker compose -f docker-compose.deploy.yml up -d` | http://localhost:5000/swagger  | `ghcr.io/georgeluch/ecotrack:v1.0.1`           |

---

## 🖼️ Prints do funcionamento

> **Onde colocar as prints?**  
> Crie a pasta **`docs/`** na **raiz do repositório** e salve as imagens com os nomes abaixo.  
> Os links já estão prontos — basta colar as imagens e commitar.

```
docs/
├─ print_01_ci_ok.png                # Actions → CI (.NET) concluído (verde)
├─ print_02_publish_ok.png           # Actions → Docker Publish (GHCR) concluído
├─ print_03_deploy_staging_ok.png    # Actions → Deploy (staging) concluído
├─ print_04_deploy_prod_ok.png       # Actions → Deploy (prod) concluído
├─ print_05_swagger_prod.png         # Navegador em http://localhost:5000/swagger
├─ print_06_swagger_staging.png      # Navegador em http://localhost:5001/swagger
├─ print_07_swagger_dev.png          # Navegador em http://localhost:5002/swagger
├─ print_08_prod_image.png           # docker inspect mostrando imagem/tag em prod
├─ print_09_sql_health.png           # docker compose ps com SQL "healthy"
└─ print_10_runner.png               # janela do runner self-hosted (Listening for Jobs)
```

**Placeholders prontos no README (exibição):**
![CI (.NET) OK](docs/print_01_ci_ok.png)
![Publish GHCR OK](docs/print_02_publish_ok.png)
![Deploy Staging OK](docs/print_03_deploy_staging_ok.png)
![Deploy Produção OK](docs/print_04_deploy_prod_ok.png)

![Swagger Prod](docs/print_05_swagger_prod.png)
![Swagger Staging](docs/print_06_swagger_staging.png)
![Swagger Dev](docs/print_07_swagger_dev.png)

![Imagem/Tag Produção](docs/print_08_prod_image.png)
![SQL Healthy](docs/print_09_sql_health.png)
![Runner Ativo](docs/print_10_runner.png)

---

## 🛠️ Tecnologias utilizadas

- **.NET 8** (ASP.NET Core Web API, EF Core)  
- **SQL Server** (mcr.microsoft.com/mssql/server:2022-latest)  
- **Docker & Docker Compose**  
- **GitHub Actions** (CI, Publish, Deploy)  
- **GitHub Container Registry (GHCR)**  

---

## 🔎 Validações rápidas (comandos úteis)

```bash
# Containers e portas
docker ps --format "table {{.Names}}	{{.Image}}	{{.Ports}}	{{.Status}}"

# Saúde do SQL (staging)
docker compose -f docker-compose.deploy.staging.yml ps

# Swagger (deve responder 200 no index.html)
curl -I http://localhost:5000/swagger/index.html
curl -I http://localhost:5001/swagger/index.html
curl -I http://localhost:5002/swagger/index.html

# Conferir imagem/tag de produção
docker inspect -f "{{.Config.Image}} @ {{.Image}}" ecotrack-api-prod
```

---

## 👥 Autores (exemplo)

- **Seu Nome** (@seuuser) — CI/CD e Docker  
- **Colega 1** — Orquestração / Banco  
- **Colega 2** — API / Testes

> Ajuste conforme a equipe.

---

## 📁 Estrutura (resumo)

```
EcoTrack.api/
├─ .github/workflows/
│  ├─ ci-dotnet.yml
│  ├─ docker-publish.yml
│  └─ deploy-selfhosted.yml
├─ EcoTrack.api/              # código da API (.NET 8)
│  ├─ Dockerfile
│  └─ Program.cs
├─ docker-compose.yml
├─ docker-compose.deploy.yml
├─ docker-compose.deploy.staging.yml
└─ README.md
```

---

## 🧯 Troubleshooting

- **404 no Swagger** → confirme `UseSwagger()`/`UseSwaggerUI()` e `ASPNETCORE_URLS=http://+:8080`.  
- **Porta em uso (5000/5001/5002)** → `docker ps`, pare o serviço conflitante ou remapeie.  
- **SQL unhealthy** → senha `SA_PASSWORD`, healthcheck, espaço em disco e reinício do Docker Desktop/WSL2.  
- **GHCR privado/sem login** → pacote está público; se privatizar, use `docker login ghcr.io` (PAT com `read:packages`).  
- **Runner parado** → mantenha `C:\actions-runner\run.cmd` aberto; GitHub → Settings → Actions → Runners.

---

## 📝 Licença
Uso acadêmico/educacional.
