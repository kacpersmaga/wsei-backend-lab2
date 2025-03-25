using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;

public class NewQuizDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; }
}