// "using" utilizado para adicionar dependências


using Microsoft.EntityFrameworkCore;
using AutenticacaoApi.Models;

// AppDbContext que herda DbContext extendendo o comportamento padrão EF Core
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
}


// Qualquer alteração no BD coluna, tabela... rodar o comando "dotnet ef migrations add "nome" e depois "dotnet ef database update"