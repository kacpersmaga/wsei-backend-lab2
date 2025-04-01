using ApplicationCore.Models.QuizAggregate;
using FluentValidation;
using WebApi.Dto;

namespace WebApi.Validators;

public class QuizItemValidator : AbstractValidator<QuizItem>
{
    public QuizItemValidator()
    {
        RuleFor(q => q.Question)
            .MaximumLength(200).WithMessage("Pytanie nie może być dłuższe niż 200 znaków.")
            .MinimumLength(3).WithMessage("Pytanie nie może być krótsze od 3 znaków!");
        RuleForEach(q => q.IncorrectAnswers)
            .MaximumLength(200)
            .MinimumLength(1);
        RuleFor(q => q.CorrectAnswer)
            .MinimumLength(1)
            .MaximumLength(200);
        RuleFor(q => new { q.CorrectAnswer, q.IncorrectAnswers })
            .Must(t => !t.IncorrectAnswers.Contains(t.CorrectAnswer))
            .WithMessage("Poprawna odpowiedź nie powinna występować w liście niepoprawnych odpowiedzi!");
        RuleFor(q => q.IncorrectAnswers)
            .Must(i => i.Count > 0);
    }
}

public class NewQuizItemDtoValidator : AbstractValidator<NewQuizItemDto>
{
    public NewQuizItemDtoValidator()
    {
        RuleFor(q => q.Question)
            .MaximumLength(200).WithMessage("Pytanie nie może być dłuższe niż 200 znaków.")
            .MinimumLength(3).WithMessage("Pytanie nie może być krótsze od 3 znaków!");
            
        RuleFor(q => q.Options)
            .NotEmpty().WithMessage("Lista opcji nie może być pusta");
            
        RuleForEach(q => q.Options)
            .MaximumLength(200).WithMessage("Opcja nie może być dłuższa niż 200 znaków.")
            .MinimumLength(1).WithMessage("Opcja nie może być krótsza od 1 znaku!");
            
        RuleFor(q => q.CorrectOptionIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Indeks poprawnej odpowiedzi musi być większy lub równy 0")
            .Must((dto, index) => index < dto.Options.Count)
            .WithMessage("Indeks poprawnej odpowiedzi musi być mniejszy niż liczba dostępnych opcji");
    }
}