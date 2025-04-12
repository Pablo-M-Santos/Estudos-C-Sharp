using AutenticacaoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Indica que a classe é um API REST
[ApiController]
// Define a rota. Aqui, `[controller]` é substituído por `Auth`, então a rota base é:  `http://localhost:5084/Auth`

[Route("[controller]")]
public class AuthController : ControllerBase
{
    // Injetando o AppDbContext no crontroller permitindo acesso ao banco de dados com _context
    // readonly significa que o valor só pode ser atribuído no construtor ou na declaração
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // Rota Auth/register para registrar um novo usuário
    [HttpPost("register")]

    // fromBody espera os dados do UsuarioDTO no corpo da requisição
    public async Task<IActionResult> Register([FromBody] UsuarioDTO request)
    {

        // Se algum campo estiver vazio ou nulo, retorna erro 400 com mensagem.
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest("O campo 'Nome' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("O campo 'Email' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest("O campo 'Senha' é obrigatório.");

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
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok("Usuário registrado com sucesso.");
    }

    // Rota de login para autenticar o usuário
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioDTO request)
    {
        // Verifica se o email e a senha foram informados. Se não, retorna erro 400 com mensagem.
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Verifica se o usuário existe e se a senha informada é válida. Se não, retorna erro 401 com mensagem.
        // O método Verify verifica se a senha informada corresponde ao hash armazenado no banco de dados.
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return Unauthorized("Credenciais inválidas.");

        return Ok("Login efetuado com sucesso.");
    }
}
