using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevIO.Business.Models.Validations
{
    public class EnderecoValidation : AbstractValidator<Endereco>
    {
        public EnderecoValidation()
        {
            RuleFor(e => e.Bairro)
                .NotEmpty().WithMessage("O {PropertyName} deve ser informado")
                .Length(2, 100).WithMessage("O {PropertyName} deve conter entre {MinLength} e {MaxLength} caracteres.");
            RuleFor(e => e.Cep)
                .NotEmpty().WithMessage("O {PropertyName} deve ser informado")
                .Length(8).WithMessage("O {PropertyName} deve conter {MaxLength} caracteres.");
            RuleFor(e => e.Cidade)
                .NotEmpty().WithMessage("O {PropertyName} deve ser informado")
                .Length(2, 100).WithMessage("O {PropertyName} deve conter entre {MinLength} e {MaxLength} caracteres.");
            RuleFor(e => e.Estado)
                .NotEmpty().WithMessage("O {PropertyName} deve ser informado")
                .Length(2, 50).WithMessage("O {PropertyName} deve conter entre {MinLength} e {MaxLength} caracteres.");
            RuleFor(e => e.Logradouro)
                .NotEmpty().WithMessage("O {PropertyName} deve ser informado")
                .Length(2, 200).WithMessage("O {PropertyName} deve conter entre {MinLength} e {MaxLength} caracteres.");
            RuleFor(e => e.Estado)
               .NotEmpty().WithMessage("O {PropertyName} deve ser informado")
               .Length(2, 50).WithMessage("O {PropertyName} deve conter entre {MinLength} e {MaxLength} caracteres.");


        }
    }
}
