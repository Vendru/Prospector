# Prospector — Plataforma Analítica de Prospectos QB

Aplicação web fullstack em **Blazor (.NET 8)** para catalogar, filtrar e analisar
estatísticas e scout grades de Quarterbacks do College Football para classes de Draft futuras.

## Stack Técnica

| Camada | Tecnologia |
|--------|-----------|
| Frontend | Blazor WebAssembly (Auto render mode) |
| Backend | ASP.NET Core 8 + Minimal API |
| Banco de dados | SQLite + Entity Framework Core 8 |
| Autenticação | ASP.NET Core Identity + Cookie Auth |
| UI | MudBlazor 6 |
| Gráficos | Blazor.ApexCharts |

## Pré-requisitos

1. **.NET 8 SDK** — [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Um editor como **Visual Studio 2022 (17.8+)** ou **VS Code** com extensão C#

## Como Executar

```bash
# 1. Entrar no diretório do projeto servidor
cd Prospector.Web

# 2. Restaurar dependências e executar
dotnet run
```

Acesse `http://localhost:5000` no navegador.

O banco de dados SQLite (`prospector.db`) é criado e populado automaticamente
na primeira execução com **10 prospects QB** e **2 usuários demo**.

## Integração com College Football Data API (opcional)

A importação de estatísticas reais de QBs utiliza a [College Football Data API](https://collegefootballdata.com) (gratuita).

Para habilitá-la:

1. Registre-se em [collegefootballdata.com](https://collegefootballdata.com) e obtenha sua API key gratuita
2. Copie o arquivo de exemplo:
   ```bash
   cp Prospector.Web/appsettings.Development.json.example Prospector.Web/appsettings.Development.json
   ```
3. Substitua `SUA_CHAVE_AQUI` pela sua key no arquivo criado

Sem essa configuração o app funciona normalmente — os 10 prospects do seed já incluem stats. A integração é necessária apenas para importar novos jogadores.

## Usuários Demo

| Email | Senha | Perfil |
|-------|-------|--------|
| `admin@prospector.com` | `Admin@123` | Administrador |
| `scout@prospector.com` | `Scout@123` | Analista |

## Funcionalidades

### Dashboard
- KPIs: total de prospects, notas médias, classes de draft
- Gráfico de distribuição por classe de draft (donut)
- Top 5 por QBR (barras horizontais)
- Top 5 por EPA (barras verticais)
- Lista dos top prospects por nota geral com badges coloridos

### Lista de Prospectos
- Busca por nome/escola com debounce
- Filtros por: Classe de Draft, Conferência, Round Projetado
- Ordenação por: Nota Geral, Teto, Nome, Escola, Classe
- Cards com estatísticas da última temporada e QBR progress bar
- Seleção para comparação (até 4 jogadores)

### Detalhe do Prospecto
- Header com jersey number, badges de notas, comparação NFL
- Bio do prospecto
- **Aba Estatísticas**: tabela com stats de 2-3 temporadas + gráfico de evolução (QBR e Completion%)
- **Aba Métricas Avançadas**: EPA, CPOE, On-Target%, Pressure Rate, Sack Rate, Red Zone %
- **Aba Scout Grades**: radar chart + barras individuais por habilidade + pontos fortes/fracos
- **Aba Medidas Físicas**: 40 jardas, salto vertical, broad jump, cone drill, hand size, arm length

### Comparação de Prospectos
- Autocomplete para buscar e adicionar até 4 prospectos
- Cards side-by-side com grades de cada prospecto
- Gráfico Ceiling vs Floor (barras agrupadas + linha de Overall)
- Radar chart de Scout Grades sobreposto (até 4 jogadores)
- Tabela comparativa com destaque do melhor em cada métrica

## Arquitetura do Projeto

```
Prospector.sln
├── Prospector.Web/               # Servidor (SSR + host do WASM)
│   ├── Components/               # App.razor, Routes.razor, MainLayout
│   ├── Data/                     # ApplicationDbContext, SeedData
│   ├── Endpoints/                # Minimal API (Player, Auth, Dashboard)
│   ├── Models/                   # Entidades EF Core (Player, QBSeasonStats...)
│   └── Services/                 # PlayerDbService (IPlayerService)
│
└── Prospector.Web.Client/        # Cliente WASM
    ├── Components/
    │   ├── Pages/                # Dashboard, PlayerList, PlayerDetail, Compare, Login, Register
    │   └── Shared/               # PlayerCard, GradeRadarChart, StatCompareChart...
    ├── Models/                   # DTOs (PlayerListDto, DashboardDto...)
    └── Services/                 # PlayerApiService, CookieAuthStateProvider
```
