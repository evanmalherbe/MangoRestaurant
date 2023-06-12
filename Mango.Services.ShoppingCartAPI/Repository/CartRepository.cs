using AutoMapper;
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

    public async Task<bool> ClearCart(string userId)
    {
      throw new NotImplementedException();
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
          cart.CartDetails.FirstOrDefault().Count+= CartDetailsFromDb.Count;
          _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
          await _db.SaveChangesAsync();
        }
      }

        return _mapper.Map<CartDTO>(cart);
    }

    public async Task<CartDTO> GetCartByUserId(string userId)
    {
      throw new NotImplementedException();
    }

    public async Task<bool> RemoveFromCart(int cartDetailsId)
    {
      throw new NotImplementedException();
    }
  }
}
