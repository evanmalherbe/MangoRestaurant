﻿using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<CouponDTO, Coupon>().ReverseMap();
				//config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
				//config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
				//config.CreateMap<Cart, CartDTO>().ReverseMap();
			});

			return mappingConfig;
		}
	}
}
