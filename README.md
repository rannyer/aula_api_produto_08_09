# API de Produtos — projeto base

.NET 8 + PostgreSQL, uma entidade (`Produto`), CRUD completo em camadas.

Estado atual: **sem paginação e sem tratamento de exceção** — os dois temas a serem construídos em aula.

## Rodar

Com o PostgreSQL rodando em `localhost:5432` (usuário `postgres`, senha `postgres`):

```bash
dotnet restore
dotnet run
```

O `EnsureCreated` cria o banco `produtosdb` e o seeder insere 47 produtos. Swagger em `http://localhost:5099/swagger`.

Se as credenciais forem outras, ajuste `ConnectionStrings:Postgres` em `appsettings.json`.

## Camadas

```
Domain/Produto.cs                 entidade
Data/AppDbContext.cs              mapeamento Fluent API
Data/DbSeeder.cs                  47 registros
DTOs/ProdutoDtos.cs               Request e Response
Repositories/ProdutoRepository.cs acesso a dados
Services/ProdutoService.cs        regras
Controllers/ProdutosController.cs endpoints
```

`GET /api/produtos` devolve os 47 registros de uma vez. Erros de "não encontrado" são resolvidos com `NotFound()` dentro do controller.

## O que está preparado para a aula

**Paginação.** O `ExisteNomeAsync` já está no repositório mas não é usado por ninguém — está ali para quando entrar a validação de duplicidade. O `ListarAsync` retorna `List<Produto>` com `OrderBy(p => p.Id)` fixo: os pontos de mudança são a assinatura do repositório, um envelope `PagedResult<T>` novo em `DTOs/`, e o `[FromQuery]` no controller.

Gancho para abrir o assunto: `GET /api/produtos` com os 47 registros, depois aumentar o seeder para 50 mil e repetir a chamada.

**Tratamento de exceção.** Hoje cada endpoint checa `null` e devolve `NotFound()` por conta própria, e nada mais é tratado. Três falhas já reproduzíveis com o projeto como está, todas em `ProdutosApi.http`:

- criar dois produtos com o mesmo nome → 500 cru, porque existe índice `UNIQUE` em `nome` e ninguém captura o `DbUpdateException`;
- parar o serviço do PostgreSQL e chamar a listagem → stack trace de conexão vazando para o cliente;
- trocar `DateTime.UtcNow` por `DateTime.Now` no `CriarAsync` → estoura, porque a coluna é `timestamptz`.

O caminho depois disso: pasta `Exceptions/` com as exceções de domínio, middleware global convertendo para `ProblemDetails`, e os `if (produto is null) return NotFound();` do controller saindo em favor de um `throw` no service.

## Migrations

O projeto sobe com `EnsureCreated()`. Para trocar por migrations:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add Inicial
dotnet ef database update
```

E em `Program.cs`, `EnsureCreatedAsync()` vira `MigrateAsync()`.
