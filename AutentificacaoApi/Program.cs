// Configura serviços (injeção de dependência, banco, Swagger, etc)
// Define o pipeline de requisições (middleware: HTTPS, rotas, autenticação...).
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Cria o builder para a aplicação
// O builder é responsável por configurar os serviços e o pipeline da aplicação
var builder = WebApplication.CreateBuilder(args);

// regista os controllers da aplicação para que o ASP.NET Core saiba como lidar com requisições HTTP
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes("sua-chave-super-secreta-muito-forte-123!@#");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Regista o DbContext da aplicação para que o ASP.NET Core saiba como lidar com o banco de dados
// a string de conexão vem do arquivo appsettings.json
// o DbContext é responsável por mapear as entidades do banco de dados para as classes da aplicação
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona o Swagger para gerar a documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Depois de registrar os serviços, agora monta o app real
var app = builder.Build();

// Ativa o swagger so em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// redireciona as requisições HTTP para HTTPS
app.UseHttpsRedirection();

// mapeia os controllers da aplicação para que o ASP.NET Core saiba como lidar com as requisições HTTP
// o ASP.NET Core usa o padrão de rotas para mapear as requisições HTTP para os controllers
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Inicia o servidor e mantém ele rodando.
app.Run();
