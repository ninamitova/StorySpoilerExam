using System;
using System.Text.Json.Serialization;

namespace ExamProject.Models
{
        public class ApiResponseDTO
        {
            [JsonPropertyName("msg")]
            public string Msg { get; set; }

            [JsonPropertyName("storyId")]
            public string? StoryId { get; set; } 
        }
}