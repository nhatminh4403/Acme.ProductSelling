// Create: Middleware/DatabaseLimitMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Acme.ProductSelling.Web.Middleware
{
    public class DatabaseLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseLimitMiddleware> _logger;
        private static bool _databaseLimitReached = false;
        private static DateTime? _lastCheckTime = null;
        private static string _resetDateInfo = "soon";
        private static string _fullErrorMessage = "";
        private static string _currentMonth = "";
        private static string _nextMonth = "";
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

        public DatabaseLimitMiddleware(RequestDelegate next, ILogger<DatabaseLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // If we know the limit is reached, immediately show the error page
            if (_databaseLimitReached &&
                _lastCheckTime.HasValue &&
                DateTime.UtcNow - _lastCheckTime.Value < CheckInterval)
            {
                await ShowLimitReachedPage(context);
                return;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Check if it's a database limit exception
                if (IsDatabaseLimitException(ex, out string resetDate, out string errorMessage, out string currentMonth, out string nextMonth))
                {
                    _databaseLimitReached = true;
                    _lastCheckTime = DateTime.UtcNow;
                    _resetDateInfo = resetDate;
                    _fullErrorMessage = errorMessage;
                    _currentMonth = currentMonth;
                    _nextMonth = nextMonth;

                    _logger.LogWarning("Database limit reached. Reset date: {ResetDate}", resetDate);

                    await ShowLimitReachedPage(context);
                }
                else
                {
                    throw; // Re-throw other exceptions
                }
            }
        }

        private bool IsDatabaseLimitException(Exception ex, out string resetDate, out string errorMessage, out string currentMonth, out string nextMonth)
        {
            resetDate = "soon";
            errorMessage = "";
            currentMonth = DateTime.UtcNow.ToString("MMMM yyyy");
            nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MMMM yyyy");

            var exception = ex;
            while (exception != null)
            {
                if (exception is SqlException sqlEx)
                {
                    errorMessage = sqlEx.Message;

                    // Azure SQL Database free tier limit error code
                    if (sqlEx.Number == 42119)
                    {
                        // Extract reset date and month info from error message
                        // Pattern: "for the month of December 2025"
                        var monthPattern = @"for the month of ([A-Za-z]+\s+\d{4})";
                        var monthMatch = Regex.Match(sqlEx.Message, monthPattern);

                        if (monthMatch.Success)
                        {
                            currentMonth = monthMatch.Groups[1].Value;
                        }

                        // Extract reset date from error message
                        // Pattern: "at 12:00 AM (UTC) on January 01, 2026"
                        var datePattern = @"at\s+(\d{1,2}:\d{2}\s+[AP]M)\s+\(UTC\)\s+on\s+([A-Za-z]+\s+\d{1,2},\s+\d{4})";
                        var match = Regex.Match(sqlEx.Message, datePattern);

                        if (match.Success)
                        {
                            var time = match.Groups[1].Value;
                            var date = match.Groups[2].Value;
                            resetDate = $"{date} at {time} UTC";

                            // Extract next month from reset date
                            var nextMonthPattern = @"([A-Za-z]+)\s+\d{1,2},\s+(\d{4})";
                            var nextMonthMatch = Regex.Match(date, nextMonthPattern);
                            if (nextMonthMatch.Success)
                            {
                                nextMonth = $"{nextMonthMatch.Groups[1].Value} {nextMonthMatch.Groups[2].Value}";
                            }

                            // Try to parse the full date and calculate time remaining
                            if (DateTime.TryParse($"{date} {time}", out DateTime parsedDate))
                            {
                                var timeUntilReset = parsedDate - DateTime.UtcNow;
                                if (timeUntilReset.TotalDays > 1)
                                {
                                    resetDate = $"{date} at {time} UTC (in {(int)timeUntilReset.TotalDays} days)";
                                }
                                else if (timeUntilReset.TotalHours > 1)
                                {
                                    resetDate = $"{date} at {time} UTC (in {(int)timeUntilReset.TotalHours} hours)";
                                }
                                else if (timeUntilReset.TotalMinutes > 0)
                                {
                                    resetDate = $"{date} at {time} UTC (in {(int)timeUntilReset.TotalMinutes} minutes)";
                                }
                            }
                        }
                        else
                        {
                            // Fallback: try to find any date mention
                            var simpleDatePattern = @"([A-Za-z]+\s+\d{1,2},\s+\d{4})";
                            var simpleMatch = Regex.Match(sqlEx.Message, simpleDatePattern);
                            if (simpleMatch.Success)
                            {
                                resetDate = simpleMatch.Groups[1].Value;
                            }
                        }

                        return true;
                    }

                    // Check for other Azure SQL limit error codes
                    if (sqlEx.Number == 40613 || sqlEx.Number == 40501 ||
                        sqlEx.Number == 49918 || sqlEx.Number == 10928 ||
                        sqlEx.Number == 10929)
                    {
                        return true;
                    }

                    // Check error message for quota/limit keywords
                    if (sqlEx.Message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                        sqlEx.Message.Contains("free amount allowance", StringComparison.OrdinalIgnoreCase) ||
                        sqlEx.Message.Contains("DTU", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                exception = exception.InnerException;
            }

            return false;
        }

        private async Task ShowLimitReachedPage(HttpContext context)
        {
            context.Response.StatusCode = 503; // Service Unavailable
            context.Response.ContentType = "text/html; charset=utf-8";

            var html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Oops! Out of Free Credits</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
        }}
        .container {{
            background: white;
            padding: 3rem;
            border-radius: 15px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            text-align: center;
            max-width: 600px;
            width: 100%;
        }}
        .emoji {{
            font-size: 5rem;
            margin-bottom: 1.5rem;
            animation: bounce 2s infinite;
        }}
        @keyframes bounce {{
            0%, 100% {{ transform: translateY(0); }}
            50% {{ transform: translateY(-20px); }}
        }}
        h1 {{
            color: #667eea;
            margin-bottom: 1rem;
            font-size: 2rem;
            font-weight: 700;
        }}
        .subtitle {{
            color: #e74c3c;
            font-size: 1.1rem;
            font-weight: 600;
            margin-bottom: 1.5rem;
        }}
        p {{
            color: #555;
            line-height: 1.8;
            margin-bottom: 1.5rem;
            font-size: 1rem;
        }}
        .casual-text {{
            font-size: 1.05rem;
            color: #444;
            font-style: italic;
        }}
        .info-box {{
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
            padding: 1.5rem;
            border-radius: 10px;
            margin: 1.5rem 0;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }}
        .info-box strong {{
            font-size: 1.2rem;
            display: block;
            margin-bottom: 0.5rem;
        }}
        .reset-date {{
            font-size: 1.3rem;
            font-weight: bold;
            color: #fff;
            background: rgba(0,0,0,0.2);
            padding: 0.8rem;
            border-radius: 8px;
            margin-top: 1rem;
        }}
        .details {{
            background: #f8f9fa;
            padding: 1.5rem;
            border-radius: 10px;
            margin-top: 1.5rem;
            border-left: 5px solid #667eea;
            text-align: left;
        }}
        .details h3 {{
            color: #667eea;
            margin-bottom: 1rem;
            font-size: 1.2rem;
        }}
        .details p {{
            color: #666;
            font-size: 0.95rem;
            margin-bottom: 0.8rem;
        }}
        .icon {{
            display: inline-block;
            margin-right: 0.5rem;
        }}
        .footer {{
            margin-top: 2rem;
            padding-top: 1.5rem;
            border-top: 2px solid #eee;
            color: #999;
            font-size: 0.9rem;
        }}
        .student-badge {{
            display: inline-block;
            background: #ffd700;
            color: #333;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.85rem;
            font-weight: bold;
            margin-bottom: 1rem;
        }}
        .error-details {{
            background: #fff3cd;
            border: 1px solid #ffc107;
            padding: 1rem;
            border-radius: 8px;
            margin-top: 1rem;
            text-align: left;
            font-size: 0.85rem;
            color: #856404;
            max-height: 200px;
            overflow-y: auto;
        }}
        .error-details pre {{
            white-space: pre-wrap;
            word-wrap: break-word;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='emoji'>💸</div>
        <div class='student-badge'>👨‍🎓 Student Project on Azure Free Tier</div>
        <h1>Oops! We Hit the Free Limit 😅</h1>
        <p class='subtitle'>Azure SQL Free Credits Exhausted for {_currentMonth}</p>
        
        <p class='casual-text'>
            Hey there! So... I burned through Azure's 32GB free database quota for {_currentMonth}. 
            As a fresh grad running this on the free tier, the database auto-paused until next month. 
        </p>
        
        <p class='casual-text'>
            Don't worry though - all the data is safe! It'll be back up automatically when the free quota resets in {_nextMonth}. 
            Just the reality of learning and building on a student budget! 💪
        </p>

        <div class='info-box'>
            <strong><span class='icon'>⏰</span>Service Will Resume On:</strong>
            <div class='reset-date'>{_resetDateInfo}</div>
        </div>

        <div class='details'>
            <h3><span class='icon'>🎓</span>Why This Happened (Learning Moment!)</h3>
            <p>
                <strong>• Free Tier Limits:</strong> Azure gives 32GB storage/month for free. Great for students & side projects!
            </p>
            <p>
                <strong>• Auto-Pause:</strong> When limits hit, Azure pauses the DB (doesn't delete anything, thankfully).
            </p>
            <p>
                <strong>• Monthly Reset:</strong> Everything resets at midnight UTC on the 1st of each month.
            </p>
            <p>
                <strong>• No Charges:</strong> As long as I stay on free tier, no credit card charges! 🎉
            </p>
        </div>

        {(string.IsNullOrEmpty(_fullErrorMessage) ? "" : $@"
        <details style='margin-top: 1.5rem;'>
            <summary style='cursor: pointer; color: #667eea; font-weight: bold; padding: 0.5rem;'>
                🔍 Show Technical Error Details
            </summary>
            <div class='error-details'>
                <pre>{System.Net.WebUtility.HtmlEncode(_fullErrorMessage)}</pre>
            </div>
        </details>
        ")}

        <div class='footer'>
            <p>Thanks for checking out my project! 🙏</p>
            <p><strong>See you in {_nextMonth}!</strong></p>
            <p style='margin-top: 1rem; font-size: 0.8rem;'>
                P.S. If you're a recruiter and seeing this... yes, I know how to handle production databases too! 😄
            </p>
        </div>
    </div>
</body>
</html>";

            await context.Response.WriteAsync(html);
        }
    }
}