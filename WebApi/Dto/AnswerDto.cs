namespace WebApi.Dto;

public class AnswerDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}