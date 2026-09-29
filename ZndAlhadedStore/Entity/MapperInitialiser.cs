using AutoMapper;

using Shared.DTOs;
using ZndAlhadedStore.ImageResolver;
using ZndAlhadedStore.ProductCommandHandler.Command;


namespace ZndAlhadedStore.Entity.DTOs
{
    public class MapperInitialiser:Profile
    {
        public MapperInitialiser()
        {
            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, GetProductDTO>().ForMember(dest => dest.ImageURL, opt => opt.MapFrom<ImageUrlResolver>()).ReverseMap();
            CreateMap<CreateProductDTO, CreateProductCommand>().ForMember(dest => dest.Image, opt => opt.Ignore()).ReverseMap(); 
            CreateMap<Product, CreateProductCommand>().ForMember(dest => dest.Image, opt => opt.Ignore()).ReverseMap(); 
            CreateMap<Product, UpdateProductCommand>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));  
            CreateMap<UpdateProductDTO, UpdateProductCommand>().ReverseMap();  
            
            CreateMap <Client,GetClientDTO>().ReverseMap();
            CreateMap<Client, CreateClientDTO>().ReverseMap();
            CreateMap<Quote,CreateQuoteDTO>().ReverseMap();
            CreateMap<Quote,GetQuoteDTO>().ReverseMap();
            CreateMap<QuoteItem,CreateQuoteItemDTO>().ReverseMap();
            CreateMap<QuoteItem,GetQuoteItemDTO>().ReverseMap();
        }
    }
}
