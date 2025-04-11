using AutenticacaoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UsuarioDTO request)
    {

        // validação de campos

        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest("O campo 'Nome' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("O campo 'Email' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest("O campo 'Senha' é obrigatório.");


        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email já cadastrado.");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok("Usuário registrado com sucesso.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioDTO request)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return Unauthorized("Credenciais inválidas.");

        return Ok("Login efetuado com sucesso.");
    }
}
