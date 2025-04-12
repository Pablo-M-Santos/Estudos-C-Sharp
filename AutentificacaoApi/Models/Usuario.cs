// Entidade Usuario para o sistema de autenticação
// Esta classe representa um usuário no sistema de autenticação.
// Ela contém propriedades para o ID do usuário, email e senha hash.
// O ID é gerado automaticamente pelo banco de dados.
// O email é único e obrigatório, enquanto a senha é armazenada como um hash para segurança.
// O hash da senha é gerado usando a biblioteca BCrypt.Net, que é uma implementação do algoritmo BCrypt.

namespace AutenticacaoApi.Models

{
    public class Usuario
    {
        // "ID" Chave Primária / Auto Incrementado do banco de dados
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        // "Email" do usuário / "string.Empty" utilizado para evitar valor null garantindo que o valor seja sempre tenha um valor mesmo sendo vazio.
        public string Email { get; set; } = string.Empty;

        // "Senha" do usuário / criptografia com (hash)
        public string SenhaHash { get; set; } = string.Empty;

        // "Role" do usuário / "Cliente" é o valor padrão para novos usuários.
        public string Role { get; set; } = "Cliente";
    }
}
