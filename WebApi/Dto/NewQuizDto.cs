using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Dto;

public class NewQuizDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}