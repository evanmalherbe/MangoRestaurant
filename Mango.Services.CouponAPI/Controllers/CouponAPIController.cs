using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Mango.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
	[ApiController]
	[Route("api/coupon")]
	public class CouponAPIController : Controller
	{
		private readonly ICouponRepository _couponRepository;
		protected ResponseDTO _response;
		public CouponAPIController(ICouponRepository couponRepository)
		{
			_couponRepository = couponRepository;
			this._response = new ResponseDTO();
		}

		[HttpGet("{code}")]
		public async Task<object> GetDiscountForCode(string code)
		{
			try
			{
				CouponDTO coupon = await _couponRepository.GetCouponByCode(code);
				_response.Result = coupon;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return _response;
		}
	}
}
