using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace TodoAPI;

public class TodoGlobalExceptionHandler(
    IHostEnvironment environment, 
    ILogger<TodoGlobalExceptionHandler> logger) 
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, exception.Message);
        
        var problemDetails = CreateProblemDetails(context, exception);
        var problemDetailsAsJson = ToJson(problemDetails);
        await context.Response.WriteAsync(problemDetailsAsJson, cancellationToken: cancellationToken);
        return true;
    }

    private string ToJson(ProblemDetails problemDetails)
    {
        try
        {
            return JsonSerializer.Serialize(problemDetails);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occurred trying to serialize problem details");
        }

        return string.Empty;
    }

    /// <summary>
    /// Ensures the problem details (exception, strack trace, etc) only
    /// shows within certain environments. Add addtional data in this
    /// area. Also choose each environment.
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="exception">Exception being thrown.</param>
    /// <returns>Problem details as part of RFC 9457 spec.</returns>
    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var statusCode = context.Response.StatusCode;
        var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);

        if (string.IsNullOrEmpty(reasonPhrase))
        {
            reasonPhrase = "An unhandled exception occurred while processing this request.";
        }

        var problemDetails = new ProblemDetails
        {
           Status = statusCode,
           Title = reasonPhrase,
        };

        if (environment.IsProduction())
        {
            return problemDetails;
        }

        problemDetails.Detail = exception.ToString();
        problemDetails.Extensions["TraceId"] = context.TraceIdentifier;
        // todo: remember when throwing exception to populate data.
        problemDetails.Extensions["Data"] = exception.Data;

        return problemDetails;
    }
}