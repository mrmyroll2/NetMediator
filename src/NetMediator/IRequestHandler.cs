namespace NetMediator;

public interface IRequestHandler<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}

//public interface IRequestHandler<in TRequest>
//{
//    Task Handle(TRequest request, CancellationToken cancellationToken);
//}
