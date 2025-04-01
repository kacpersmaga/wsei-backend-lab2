using ApplicationCore.Interfaces.UserService;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/v1/user/quizzes")]
public class ApiQuizUserController : ControllerBase
{
    private readonly IQuizUserService _service;
    private readonly IMapper _mapper;

    public ApiQuizUserController(IQuizUserService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [Route("{id}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<Quiz> GetQuiz(int id)
    {
        var quiz = _service.FindQuizById(id);
        return quiz == null ? NotFound() : Ok(quiz);
    }

    [Route("{quizId}/items/{itemId}/answers/{userId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<object> SaveAnswer(
        int quizId,
        int itemId,
        int userId,
        QuizItemUserAnswerDto dto,
        LinkGenerator linkGenerator
    )
    {
        _service.SaveUserAnswerForQuiz(quizId, userId, itemId, dto.Answer ?? "");
        return Created(
            linkGenerator.GetUriByAction(HttpContext, nameof(GetQuizFeedback), null,
                new { quizId = quizId, userId = 1 }),
            new
            {
                answer = dto.Answer,
            });
    }

    [Route("{quizId}/answers/{userId}")]
    [HttpGet]
    public ActionResult<FeedbackDto> GetQuizFeedback(int quizId, int userId)
    {
        var feedback = _service.GetUserAnswersForQuiz(quizId, userId);
        var quiz = _service.FindQuizById(quizId);
        
        if (quiz == null)
        {
            return NotFound(new { error = $"Quiz with id {quizId} not found" });
        }
        
        var feedbackDto = _mapper.Map<FeedbackDto>(feedback);
        feedbackDto.TotalQuestions = quiz.Items.Count;
        
        return feedbackDto;
    }
}