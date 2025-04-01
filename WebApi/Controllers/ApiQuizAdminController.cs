using ApplicationCore.Interfaces.AdminService;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("api/admin/quizzes")]
public class ApiQuizAdminController : ControllerBase
{
    private readonly IQuizAdminService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<QuizItem> _quizItemValidator;
    private readonly IValidator<NewQuizItemDto> _newQuizItemDtoValidator;

    public ApiQuizAdminController(
        IQuizAdminService service, 
        IMapper mapper, 
        IValidator<QuizItem> quizItemValidator,
        IValidator<NewQuizItemDto> newQuizItemDtoValidator)
    {
        _service = service;
        _mapper = mapper;
        _quizItemValidator = quizItemValidator;
        _newQuizItemDtoValidator = newQuizItemDtoValidator;
    }
    
    [HttpPost]
    public ActionResult<object> AddQuiz(LinkGenerator link, NewQuizDto dto)
    {
        var quiz = _service.AddQuiz(_mapper.Map<Quiz>(dto));
        return Created(
            link.GetPathByAction(HttpContext, nameof(GetQuiz), null, new { quizId = quiz.Id }),
            quiz
        );
    }

    [HttpGet("{quizId}")]
    public ActionResult<Quiz> GetQuiz(int quizId)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        return quiz is null ? NotFound() : Ok(quiz);
    }
    
    [HttpPatch]
    [Route("{quizId}")]
    [Consumes("application/json-patch+json")]
    public ActionResult<Quiz> AddQuizItem(int quizId, JsonPatchDocument<Quiz>? patchDoc)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (quiz is null || patchDoc is null)
        {
            return NotFound(new
            {
                error = $"Quiz with id {quizId} not found"
            });
        }
        int previousCount = quiz.Items.Count;
        patchDoc.ApplyTo(quiz, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (previousCount < quiz.Items.Count)
        {
            QuizItem item = quiz.Items[^1];
            quiz.Items.RemoveAt(quiz.Items.Count - 1);
            
            var validationResult = _quizItemValidator.Validate(item);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            _service.AddQuizItemToQuiz(quizId, item);
        }
        return Ok(_service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId));
    }
    
    [HttpPost]
    [Route("{quizId}/items")]
    public ActionResult<QuizItem> AddNewQuizItem(int quizId, NewQuizItemDto dto)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (quiz is null)
        {
            return NotFound(new
            {
                error = $"Quiz with id {quizId} not found"
            });
        }
        
        var dtoValidationResult = _newQuizItemDtoValidator.Validate(dto);
        if (!dtoValidationResult.IsValid)
        {
            return BadRequest(dtoValidationResult.Errors);
        }
        
        var item = _mapper.Map<QuizItem>(dto);
        
        var itemValidationResult = _quizItemValidator.Validate(item);
        if (!itemValidationResult.IsValid)
        {
            return BadRequest(itemValidationResult.Errors);
        }
        
        var addedItem = _service.AddQuizItemToQuiz(quizId, item);
        return CreatedAtAction(nameof(GetQuizItem), new { quizId, itemId = addedItem.Id }, addedItem);
    }
    
    
    [HttpDelete("{quizId}")]
    public ActionResult DeleteQuiz(int quizId)
    {
        bool result = _service.DeleteQuiz(quizId);
        if (result)
        {
            return NoContent();
        }
        else
        {
            return NotFound(new 
            { 
                error = $"Quiz with id {quizId} not found or has associated answers"
            });
        }
    }
    
    [HttpPut("{quizId}")]
    public ActionResult<Quiz> UpdateQuiz(int quizId, QuizDto quizDto)
    {
        var existingQuiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (existingQuiz == null)
        {
            return NotFound(new
            {
                error = $"Quiz with id {quizId} not found"
            });
        }
        
        var quiz = _mapper.Map<Quiz>(quizDto);
        quiz.Id = quizId;
        
        _service.UpdateQuiz(quiz);
        return Ok(quiz);
    }
    
    [HttpGet("{quizId}/items/{itemId}")]
    public ActionResult<QuizItem> GetQuizItem(int quizId, int itemId)
    {
        var quizItem = _service.GetQuizItem(quizId, itemId);
        if (quizItem == null)
        {
            return NotFound(new
            {
                error = $"Quiz item with id {itemId} not found in quiz {quizId}"
            });
        }
        
        return Ok(quizItem);
    }
}