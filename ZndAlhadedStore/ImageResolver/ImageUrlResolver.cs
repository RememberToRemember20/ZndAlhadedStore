using AutoMapper;
using Shared.DTOs;
using ZndAlhadedStore.Entity;

namespace ZndAlhadedStore.ImageResolver
{
    public class ImageUrlResolver:IValueResolver<Product, GetProductDTO, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Product source, GetProductDTO destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ImageURL))
                return null;

            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{source.ImageURL}";
        }
    }
}
