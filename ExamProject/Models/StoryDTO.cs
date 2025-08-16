﻿using System;
using System.Text.Json.Serialization;

namespace ExamProject.Models
{
	public class StoryDTO
	{
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }

            [JsonPropertyName("url")]
            public string? Url { get; set; }
    }
}

