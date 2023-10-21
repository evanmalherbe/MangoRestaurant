using AutoMapper;

namespace Mango.Services.CouponAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				//config.CreateMap<ProductDTO, Product>().ReverseMap();
				//config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
				//config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
				//config.CreateMap<Cart, CartDTO>().ReverseMap();
			});

			return mappingConfig;
		}
	}
}
