// ponte entre C# e o banco de dados, permitindo que o Entity Framework Core interaja com o banco de dados

// "using" utilizado para adicionar dependências
using Microsoft.EntityFrameworkCore;
using AutenticacaoApi.Models;

// AppDbContext que herda DbContext extendendo o comportamento padrão EF Core
// DbContext é a classe base que representa uma sessão com o banco de dados, permitindo que você consulte e salve dados (ORM)

public class AppDbContext : DbContext
{
    // Construtor que recebe as opções de configuração do banco de dados e as passa para a classe base DbContext
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Ele vai procurar a tabela chamada Usuarios e vai guardar os dados do tipo Usuario
    // DbSet<T> representa uma tabela no banco
    // O EF Core vai usar essa propriedade pra criar a tabela e gerar queries
    public DbSet<Usuario> Usuarios { get; set; }
}


// Qualquer alteração no BD coluna, tabela... rodar o comando "dotnet ef migrations add "nome" e depois "dotnet ef database update"
