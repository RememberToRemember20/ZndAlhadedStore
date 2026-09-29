using MediatR;

namespace ZndAlhadedStore.ProductCommandHandler.Command
{
    public record DeleteImageCommand:IRequest<string>
    {
        public int Id { get; init; }
        public DeleteImageCommand(int id)
        {
            Id = id;
        }
    }
}
