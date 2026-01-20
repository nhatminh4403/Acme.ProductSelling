using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Chatbot.Dtos;

public class ChatMessageInputDto
{
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; }

    /// <summary>
    /// Conversation history for context (optional)
    /// </summary>
    public List<ChatHistoryItem> ConversationHistory { get; set; } = new();
}

/// <summary>
/// Response DTO from chatbot
/// </summary>
public class ChatMessageOutputDto
{
    public string Response { get; set; }

    /// <summary>
    /// Related products found in database
    /// </summary>
    public List<ChatbotProductDto> RelatedProducts { get; set; } = new();

    /// <summary>
    /// Indicates if response came from database or web search
    /// </summary>
    public ResponseSource Source { get; set; }

    /// <summary>
    /// User role context
    /// </summary>
    public string UserRole { get; set; }
}

/// <summary>
/// Simplified product DTO for chatbot responses
/// </summary>
public class ChatbotProductDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public double DiscountPercent { get; set; }
    public string UrlSlug { get; set; }
    public string ImageUrl { get; set; }
    public string CategoryName { get; set; }
    public string ManufacturerName { get; set; }
    public bool IsAvailableForPurchase { get; set; }
    public int TotalStock { get; set; }
}

/// <summary>
/// Conversation history item
/// </summary>
public class ChatHistoryItem
{
    public string Role { get; set; } // "user" or "assistant"
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}