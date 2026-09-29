using MediatR;

namespace ZndAlhadedStore.ProductCommandHandler.Command
{
    public class UpdateProductImageCommand:IRequest<string>
    {
        public int Id { get; set; }
        public IFormFile Image {  get; set; }
        public UpdateProductImageCommand(int id,IFormFile file)
        {
            Id=id;
            Image = file;
        }
    }
}
