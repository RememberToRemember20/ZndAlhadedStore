using MediatR;

namespace ZndAlhadedStore.Feautres.ProductCommandHandler.Command
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
