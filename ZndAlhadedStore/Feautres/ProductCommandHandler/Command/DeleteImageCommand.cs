using MediatR;

namespace ZndAlhadedStore.Feautres.ProductCommandHandler.Command
{
    public record DeleteImageCommand(int Id) : IRequest<string>;
}
