using LogicLayer.DTOs;

namespace LogicLayer.InterfacesRepository
{
	public interface IShoppingCartRepository
	{
		bool CreateShoppingCartItem(int clientId, LineItemDTO lineItemDTO);

		ShoppingCartDTO ReadShoppingCart(int id);

		bool UpdateShoppingCartItem(LineItemDTO lineItemDTO);

		bool PlaceOrder(ShoppingCartDTO shoppingCartDTO);


        bool DeleteShoppingCart(LineItemDTO lineItemDTO);
	}
}
