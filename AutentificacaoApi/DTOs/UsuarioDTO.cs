// Criação da classe 
public class UsuarioDTO
{
    public string Nome { get; set; } = string.Empty;

    // "Email" campo onde o usuário envia ao fazer cadastro ou login / string.Empty para evitar valores null
    public string Email { get; set; } = string.Empty;

    // "Senha" campo onde o usuário envia ao fazer cadastro ou login / com criptografia
    public string Senha { get; set; } = string.Empty;

    // Role Cliente e Admin / padrão Cliente
     public string Role { get; set; } = "Cliente"; 
}

// Porque utilizar o DTO só utiliza o email e senha quando precisar melhorando a segurança e organização.