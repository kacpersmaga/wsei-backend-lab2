using ApplicationCore.Models;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using System.Linq;
using WebApi.Dto;

namespace WebApi.Mapper;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<QuizItem, QuizItemDto>()
            .ForMember(
                q => q.Options,
                op => op.MapFrom(i => new List<string>(i.IncorrectAnswers) { i.CorrectAnswer }));
                
        CreateMap<Quiz, QuizDto>()
            .ForMember(
                q => q.Items,
                op => op.MapFrom<List<QuizItem>>(i => i.Items)
            );
            
        CreateMap<NewQuizDto, Quiz>();
        
        CreateMap<QuizDto, Quiz>();
        
        CreateMap<QuizItemDto, QuizItem>()
            .ForMember(
                dest => dest.CorrectAnswer,
                opt => opt.MapFrom(src => src.Options != null && src.Options.Any() ? src.Options[0] : string.Empty))
            .ForMember(
                dest => dest.IncorrectAnswers,
                opt => opt.MapFrom(src => src.Options != null && src.Options.Count > 1 ? src.Options.Skip(1).ToList() : new List<string>()));
        
        CreateMap<IEnumerable<QuizItemUserAnswer>, FeedbackDto>()
            .ForMember(
                dest => dest.QuizId, 
                opt => opt.MapFrom(src => src.Any() ? src.First().QuizId : 0))
            .ForMember(
                dest => dest.UserId, 
                opt => opt.MapFrom(src => src.Any() ? src.First().UserId : 0))
            .ForMember(
                dest => dest.Answers, 
                opt => opt.MapFrom(src => src.Select(a => new AnswerDto
                {
                    Question = a.QuizItem.Question,
                    Answer = a.Answer,
                    IsCorrect = a.IsCorrect()
                })));
        
        CreateMap<NewQuizItemDto, QuizItem>()
            .ForMember(
                dest => dest.Question,
                opt => opt.MapFrom(src => src.Question))
            .ForMember(
                dest => dest.CorrectAnswer,
                opt => opt.MapFrom(src => src.Options != null && src.CorrectOptionIndex >= 0 && src.CorrectOptionIndex < src.Options.Count 
                    ? src.Options[src.CorrectOptionIndex] 
                    : string.Empty))
            .ForMember(
                dest => dest.IncorrectAnswers,
                opt => opt.MapFrom(src => src.Options != null 
                    ? src.Options.Where((option, index) => index != src.CorrectOptionIndex).ToList() 
                    : new List<string>()));
    }
}