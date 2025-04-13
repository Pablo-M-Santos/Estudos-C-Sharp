// Configura serviços (injeção de dependência, banco, Swagger, etc)
// Define o pipeline de requisições (middleware: HTTPS, rotas, autenticação...).
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutentificacaoApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

// Cria o builder para a aplicação
// O builder é responsável por configurar os serviços e o pipeline da aplicação
var builder = WebApplication.CreateBuilder(args);

// regista os controllers da aplicação para que o ASP.NET Core saiba como lidar com requisições HTTP
builder.Services.AddControllers();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IValidator<UsuarioDTO>, UsuarioDTOValidator>(); // para register
builder.Services.AddScoped<LoginValidator>(); 

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>();

if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.Key))
{
    throw new Exception("JWT options not configured correctly in appsettings.json");
}

var key = Encoding.ASCII.GetBytes(jwtOptions.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience
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
