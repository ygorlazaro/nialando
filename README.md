# Nia Lando: Glavo kaj Sigelo

Grand strategy por turnos, single-player, inspirado en Europa Universalis IV,
numa Europa fictícia do século XV. Um turno = 1 mês (configurável).

**Idioma:** Esperanto no título, subtítulo e termos de jogo. Textos de UI e docs em pt-BR.
**Código:** identificadores em inglês.

## Estado atual

Fase R0 em andamento — saneamento e migração para Godot 4 + C#.

## Prerrequisitos

- [.NET SDK](https://dotnet.microsoft.com/) (>= 8.0; 10.0 também funciona)
- [Godot 4.7.2 .NET (edição mono)](https://godotengine.org/) — **não** use a edição padrão, sem C#.
  O Godot instalado exige `net8.0` como `TargetFramework` para projetos C#. Bibliotecas `.NET` comuns
  usam `net8.0` também, para que o projeto Godot possa referenciá-las.

## Estrutura

```
NiaLando.sln
├── src/NiaLando.Domain/       # tipos puros (IDs, enums, estado serializável)
├── src/NiaLando.Simulation/   # motor determinístico: PRNG, mapa, turno, IA. Sem Godot, sem I/O
├── src/NiaLando.Application/  # fachada GameSession: casos de uso, saves, view models somente leitura
├── src/NiaLando.Cli/          # modo headless: N turnos só com IA + relatório JSON
├── NiaLando.Game/             # projeto Godot (Godot.NET.Sdk): apresentação (UI, mapa, câmera, áudio)
├── tests/                     # xUnit + FluentAssertions
└── doc/adr/
```

Domain, Simulation, Application e Cli são bibliotecas .NET comuns, sem nenhuma referência a Godot.
O projeto Godot referencia Application (e, por ela, o resto). Isso mantém tudo testável sem abrir a engine.
Godot é só a camada de apresentação: nós Godot são views finas; não contêm regra de jogo.

## Comandos

### Build e testes (.NET)

```bash
dotnet build NiaLando.sln -warnaserror
dotnet test NiaLando.sln
```

### Build do projeto Godot (headless)

```bash
godot --headless --path NiaLando.Game --build-solutions --quit
```

### Modo headless (simulação IA-only)

```bash
dotnet run --project src/NiaLando.Cli -- --seed 1234 --turns 200 --out report.json
```

### Rodar o jogo

```bash
godot --path NiaLando.Game
```

## ADRs

- [ADR-0001](doc/adr/ADR-0001.md) — Estado, saves e versionamento do gerador.
- [ADR-0002](doc/adr/ADR-0002.md) — i18n (mensagens como dados estruturados).
- [ADR-0003](doc/adr/ADR-0003.md) — Godot como camada de apresentação e simulação independente.
- [ADR-0004](doc/adr/ADR-0004.md) — Determinismo entre plataformas (sem trigonometria na Simulation, golden hashes).
## Fases completadas
- R0-F9: Arquitetura Godot+C#, PRNG determinístico, gerador de mapa, setup, turnos, economia/guerra/diplomacia/povos/naval
- Modo headless: `dotnet run --project src/NiaLando.Cli -- --seed 1234 --turns 200 --out report.json`

