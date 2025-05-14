using NetMediator;

namespace WebAPI.Behaviors;

public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        string requestName = typeof(TRequest).FullName!;

        logger.LogInformation("Processing request {RequestName}", requestName);

        TResponse result = await next(cancellationToken);

        logger.LogInformation("Completed request {RequestName}", requestName);

        return result;
    }

    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}