namespace eAgenda.WebApi.Compartilhado;

public static class ProblemDetailsTypes
{
    private const string BaseDocumentationUrl = "https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Reference/Status";

    public const string BadRequest = $"{BaseDocumentationUrl}/400";
    public const string NotFound = $"{BaseDocumentationUrl}/404";
    public const string Conflict = $"{BaseDocumentationUrl}/409";
    public const string InternalServerError = $"{BaseDocumentationUrl}/500";

    public static string? ObterPorStatus(int? statusCode)
    {
        switch (statusCode)
        {
            case StatusCodes.Status400BadRequest: return BadRequest;
            case StatusCodes.Status404NotFound: return NotFound;
            case StatusCodes.Status409Conflict: return Conflict;
            case StatusCodes.Status500InternalServerError: return InternalServerError;
            default: return null;
        }
    }
}
