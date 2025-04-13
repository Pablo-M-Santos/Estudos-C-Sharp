using AutenticacaoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using AutentificacaoApi.Models;
using FluentValidation;


// Indica que a classe é um API REST
[ApiController]
// Define a rota. Aqui, `[controller]` é substituído por `Auth`, então a rota base é:  `http://localhost:5084/Auth`

[Route("[controller]")]
public class AuthController : ControllerBase
{
    // Injetando o AppDbContext no crontroller permitindo acesso ao banco de dados com _context
    // readonly significa que o valor só pode ser atribuído no construtor ou na declaração
    private readonly AppDbContext _context;
    private readonly JwtOptions _jwtOptions;

    private readonly IValidator<UsuarioDTO> _validator;

    private readonly LoginValidator _loginValidator;

    public AuthController(AppDbContext context, IOptions<JwtOptions> jwtOptions, IValidator<UsuarioDTO> validator, LoginValidator loginValidator)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _validator = validator;
        _loginValidator = loginValidator;
    }

    // Rota Auth/register para registrar um novo usuário
    [HttpPost("register")]

    // fromBody espera os dados do UsuarioDTO no corpo da requisição
    public async Task<IActionResult> Register([FromBody] UsuarioDTO request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var mensagensErro = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(mensagensErro);
        }

        // Verifica se o email já está cadastrado no banco de dados. Se sim, retorna erro 400 com mensagem.
        // O método AnyAsync verifica se existe algum registro que atenda a condição especificada.

        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email já cadastrado.");

        // Cria objeto Usuario
        // criptografa a senha com BCrypt, e salva no banco
        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Cliente" : request.Role
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok("Usuário registrado com sucesso.");
    }

    // Rota de login para autenticar o usuário
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioDTO request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var mensagensErro = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(mensagensErro);
        }

        // Verifica se o email e a senha foram informados. Se não, retorna erro 400 com mensagem.
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Verifica se o usuário existe e se a senha informada é válida. Se não, retorna erro 401 com mensagem.
        // O método Verify verifica se a senha informada corresponde ao hash armazenado no banco de dados.
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return Unauthorized("Credenciais inválidas.");

        var token = GenerateJwtToken(usuario);

        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        // claims são informações sobre o usuário que serão incluídas no token JWT
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        // Cria a chave de segurança e as credenciais de assinatura do token JWT
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        // chave secreta utilizando HMAC SHA256 para assinar o token
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Cria o token JWT com as informações do usuário, data de expiração e credenciais de assinatura
        var token = new JwtSecurityToken(
        issuer: _jwtOptions.Issuer,
        audience: _jwtOptions.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpireHours),
        signingCredentials: creds
    );

        // Devolve uma string (compactado) com o token JWT gerado
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet("admin")]
    public IActionResult GetAdminData()
    {
        return Ok("Você é um administrador!");
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("cliente")]
    public IActionResult GetClienteData()
    {
        return Ok("Você é um cliente!");
    }

}




