using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		// GET all products
		public async Task<IActionResult> ProductIndex()
		{
			List<ProductDTO> list = new();
			var response = await _productService.GetAllProductsAsync<ResponseDTO>();
			if(response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}

		public async Task<IActionResult> ProductCreate()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ProductCreate(ProductDTO model)
		{
			if (ModelState.IsValid)
			{
				var response = await _productService.CreateProductAsync<ResponseDTO>(model);
		
				if(response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(ProductIndex));
				}
			}
			return View(model);
		}
	}
}
