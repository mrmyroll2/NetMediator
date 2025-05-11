using Microsoft.Extensions.DependencyInjection;

namespace NetMediator;

internal sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        if (handlerType == null)
        {
            throw new ArgumentNullException($"Handler for {request.GetType().Name} not found.");
        }

        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        var handleMethod = handler.GetType().GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle));

        // Build pipeline
        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);

        var behaviors = _serviceProvider.GetServices(pipelineType).Cast<dynamic>().ToList();

        // Compose delegate
        RequestHandlerDelegate<TResponse> handlerDelegate = (cancellationToken) => handleMethod!.Invoke(handler, new object[] { request, cancellationToken }) as Task<TResponse>;

        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            handlerDelegate = (cancellationToken) => behavior.Handle((dynamic)request, next, cancellationToken);
        }

        return await handlerDelegate(cancellationToken);
        //return await handler.Handle((dynamic)request, cancellationToken);
    }
}
