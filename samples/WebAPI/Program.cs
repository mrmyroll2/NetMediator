using NetMediator;
using System.Reflection;
using System.Reflection.Metadata;
using WebAPI.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNetMediator(Assembly.GetExecutingAssembly());

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));

var app = builder.Build();

app.MapGet("/", async (ISender sender) =>
{
    var data = await sender.Send(new GetPersons.Query());

    return Results.Ok(data);
});

app.Run();


public static class GetPersons
{
    public sealed record Query : IRequest<Response>;

    public sealed record Response(string Name, int Age);

    internal sealed class Handler : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new Response("John Doe", 30));
        }
    }
}