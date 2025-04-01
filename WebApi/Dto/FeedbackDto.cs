namespace WebApi.Dto;

public class FeedbackDto
{
    public int QuizId { get; set; }
    public int UserId { get; set; }
    public int TotalQuestions { get; set; }
    public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
}