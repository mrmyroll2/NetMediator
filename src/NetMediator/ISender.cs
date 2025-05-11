using Microsoft.Extensions.DependencyInjection;

namespace NetMediator;

public interface ISender
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}