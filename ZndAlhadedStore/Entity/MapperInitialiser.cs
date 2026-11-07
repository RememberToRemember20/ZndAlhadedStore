using AutoMapper;

using Shared.DTOs;
using ZndAlhadedStore.Feautres.ClientCommandHandler.Command;
using ZndAlhadedStore.Feautres.ProductCommandHandler.Command;
using ZndAlhadedStore.Feautres.QueteItemCommandHandler.Command;
using ZndAlhadedStore.ImageResolver;


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
            CreateMap<CreateClientDTO, CreateClientCommand>().ReverseMap();
            CreateMap<Client, CreateClientCommand>().ReverseMap();
            CreateMap<CreateClientDTO, UpdateClientCommand>().ReverseMap();
            CreateMap<Client, UpdateClientCommand>().ReverseMap();
            CreateMap <Client,GetClientDTO>().ReverseMap();
            CreateMap<Client, CreateClientDTO>().ReverseMap();
            CreateMap<Quote,CreateQuoteDTO>().ReverseMap();
            CreateMap<Quote,GetQuoteDTO>().ReverseMap();
            CreateMap<QuoteItem,CreateQuoteItemDTO>().ReverseMap();
            CreateMap<QuoteItem,GetQuoteItemDTO>().ReverseMap();
            CreateMap<CreateQuoteCommand, CreateQuoteDTO>().ReverseMap();
        }
    }
}
