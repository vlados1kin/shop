using AutoMapper;
using Product.DTO;

namespace Product;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Models.Product, ProductDto>();
        CreateMap<ProductForCreationDto, Entities.Models.Product>();
        CreateMap<ProductForUpdateDto, Entities.Models.Product>();
    }
}