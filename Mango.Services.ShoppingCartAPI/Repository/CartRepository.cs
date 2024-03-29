﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
	public class CartRepository : ICartRepository
	{
		private readonly ApplicationDbContext _db;
		private readonly IMapper _mapper;

		public CartRepository(ApplicationDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		public async Task<bool> ApplyCoupon(string userId, string couponCode)
		{
			var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);

			cartFromDb.CouponCode = couponCode;
			_db.CartHeaders.Update(cartFromDb);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ClearCart(string userId)
		{
			try
			{
				var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
				if (cartHeaderFromDb == null)
				{
					_db.CartDetails.RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
					_db.CartHeaders.Remove(cartHeaderFromDb);
					await _db.SaveChangesAsync();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.ToString());
				return false;
			}
		}

		public async Task<CartDTO> CreateUpdateCart(CartDTO cartDTO)
		{
			Cart cart = _mapper.Map<Cart>(cartDTO);

			var prodInDb = await _db.Products
			  .FirstOrDefaultAsync(u => u.ProductId == cartDTO.CartDetails
			  .FirstOrDefault().ProductId);

			if (prodInDb == null)
			{
				_db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
				await _db.SaveChangesAsync();
			}

			var cartHeaderFromDb = await _db.CartHeaders
			  .AsNoTracking()
			  .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

			if (cartHeaderFromDb == null)
			{
				_db.CartHeaders.Add(cart.CartHeader);
				await _db.SaveChangesAsync();
				cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
				cart.CartDetails.FirstOrDefault().Product = null;
				_db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
				await _db.SaveChangesAsync();
			}
			else
			{
				var CartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
				  u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
				  u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

				if (CartDetailsFromDb == null)
				{
					cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
					cart.CartDetails.FirstOrDefault().Product = null;
					_db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
					await _db.SaveChangesAsync();
				}
				else
				{
					cart.CartDetails.FirstOrDefault().Product = null;
					cart.CartDetails.FirstOrDefault().Count += CartDetailsFromDb.Count;
					_db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
					await _db.SaveChangesAsync();
				}
			}
			return _mapper.Map<CartDTO>(cart);
		}

		public async Task<CartDTO> GetCartByUserId(string userId)
		{
			Cart cart = new()
			{
				CartHeader = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId)
			};

			cart.CartDetails = _db.CartDetails
				   .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId)
				   .Include(u => u.Product);

			return _mapper.Map<CartDTO>(cart);
		}

		public async Task<bool> RemoveCoupon(string userId)
		{
			var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);

			cartFromDb.CouponCode = "";
			_db.CartHeaders.Update(cartFromDb);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RemoveFromCart(int cartDetailsId)
		{
			try
			{
				CartDetails cartDetails = await _db.CartDetails
				  .FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

				int totalCountOfCartItems = _db.CartDetails
				  .Where(u => u.CartHeaderId == cartDetails.CartHeaderId)
				  .Count();

				_db.CartDetails.Remove(cartDetails);

				if (totalCountOfCartItems == 1)
				{
					var cartHeaderToRemove = await _db.CartHeaders
					  .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

					_db.CartHeaders.Remove(cartHeaderToRemove);
				}

				await _db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.ToString());
				return false;
			}
		}
	}
}
