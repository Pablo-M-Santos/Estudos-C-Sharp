using FluentValidation;
using AutenticacaoApi.Models; 

public class UsuarioDTOValidator : AbstractValidator<UsuarioDTO>
{
    public UsuarioDTOValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");

        RuleFor(x => x.Role)
            .Must(role => role == "Cliente" || role == "Administrador")
            .WithMessage("O role deve ser 'Cliente' ou 'Administrador'.")
            .When(x => !string.IsNullOrWhiteSpace(x.Role));
    }
}
