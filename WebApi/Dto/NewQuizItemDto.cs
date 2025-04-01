using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;

public class NewQuizItemDto
{
    [Required]
    public string Question { get; set; } = string.Empty;
    
    [Required]
    public List<string> Options { get; set; } = new List<string>();
    
    [Required]
    public int CorrectOptionIndex { get; set; }
}