# EcoTrack.Tests
Projeto de testes xUnit para o EcoTrack.api (.NET 8).

## Como usar
1) Mova esta pasta **EcoTrack.Tests** para a raiz do seu repositório (ao lado de `EcoTrack.api/` e `EcoTrack.sln`).
2) Adicione o projeto de testes à solution:
   ```bash
   dotnet sln EcoTrack.sln add EcoTrack.Tests/EcoTrack.Tests.csproj
   ```
3) Restaure e execute os testes:
   ```bash
   dotnet restore
   dotnet test
   ```

Se o caminho do `ProjectReference` no `.csproj` não bater com a sua estrutura, ajuste-o.
