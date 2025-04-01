using ApplicationCore.Commons.Repository;
using ApplicationCore.Interfaces.Criteria;
using ApplicationCore.Models;
using ApplicationCore.Models.QuizAggregate;

namespace ApplicationCore.Interfaces.AdminService;

public class QuizAdminService : IQuizAdminService
{
    private readonly IGenericRepository<Quiz, int> _quizRepository;
    private readonly IGenericRepository<QuizItem, int> _itemRepository;
    private readonly IGenericRepository<QuizItemUserAnswer, string> _answerRepository;

    public QuizAdminService(
        IGenericRepository<Quiz, int> quizRepository,
        IGenericRepository<QuizItem, int> itemRepository,
        IGenericRepository<QuizItemUserAnswer, string> answerRepository)
    {
        this._quizRepository = quizRepository;
        this._itemRepository = itemRepository;
        this._answerRepository = answerRepository;
    }

    public QuizItem AddQuizItemToQuiz(int quizId, QuizItem item)
    {
        var quiz = _quizRepository.FindById(quizId);
        if (quiz is null)
        {
            throw new Exception();
        }
        var newItem = _itemRepository.Add(item);
        quiz.Items.Add(newItem);
        _quizRepository.Update(quizId, quiz);
        return newItem;
    }

    public void UpdateQuizItem(QuizItem item)
    {
        _itemRepository.Update(item.Id, item);
    }

    public QuizItem? FindQuizItemById(int id)
    {
        return _itemRepository.FindById(id);
    }
    
    public Quiz AddQuiz(Quiz quiz)
    {
        return _quizRepository.Add(quiz);
    }

    public void UpdateQuiz(Quiz quiz)
    {
        _quizRepository.Update(quiz.Id, quiz);
    }

    public IQueryable<QuizItem> FindAllQuizItems()
    {
        return _itemRepository.FindAll().AsQueryable();
    }

    public IQueryable<Quiz> FindAllQuizzes()
    {
        return _quizRepository.FindAll().AsQueryable();
    }

    public IEnumerable<Quiz> FindBySpecification(ISpecification<Quiz> specification)
    {
        return _quizRepository.FindBySpecification(specification);
    }
    
    
    public bool DeleteQuiz(int quizId)
    {
        var quiz = _quizRepository.FindById(quizId);
        if (quiz is null)
        {
            return false;
        }

        var answers = _answerRepository.FindAll()
            .Where(a => a.QuizId == quizId)
            .ToList();
            
        if (answers.Any())
        {
            return false;
        }
        
        _quizRepository.RemoveById(quizId);
        return true;
    }
    
    public QuizItem? GetQuizItem(int quizId, int itemId)
    {
        var quiz = _quizRepository.FindById(quizId);
        if (quiz is null)
        {
            return null;
        }
        
        return quiz.Items.FirstOrDefault(item => item.Id == itemId);
    }
}