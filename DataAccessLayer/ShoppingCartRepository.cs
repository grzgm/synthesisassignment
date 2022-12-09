using LogicLayer.DTOs;
using LogicLayer.InterfacesRepository;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;

namespace DataAccessLayer
{
	public class ShoppingCartRepository: MainRepository, IShoppingCartRepository
	{

		private IEnumerable<LineItemDTO> GetShoppingCart(string Query, List<SqlParameter>? sqlParameters)
		{
			List<LineItemDTO> lineItems = new List<LineItemDTO>();

			try
			{
				SqlConnection conn = GetConnection();
				using (SqlCommand command = new SqlCommand(Query, conn))
				{
					conn.Open();
					if (sqlParameters != null)
					{
						command.Parameters.AddRange(sqlParameters.ToArray());
					}
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						LineItemDTO lineItemDTO = new LineItemDTO();
						ItemDTO itemDTO = new ItemDTO();
						ItemCategoryDTO itemCategoryDTO = new ItemCategoryDTO();
						ItemCategoryDTO itemSubCategoryDTO = new ItemCategoryDTO();

						lineItemDTO.PurchasePrice = reader.GetDecimal(reader.GetOrdinal("purchasePrice"));
						lineItemDTO.Amount = reader.GetInt32(reader.GetOrdinal("amount"));

						itemDTO.Id = reader.GetInt32(reader.GetOrdinal("itemId"));
						itemDTO.Name = reader.GetString(reader.GetOrdinal("itemName"));
						itemDTO.Price = reader.GetDecimal(reader.GetOrdinal("price"));
						itemDTO.UnitType = reader.GetString(reader.GetOrdinal("unitType"));
						itemDTO.Available = reader.GetBoolean(reader.GetOrdinal("available"));
						itemDTO.StockAmount = reader.GetInt32(reader.GetOrdinal("stockAmount"));

						itemCategoryDTO.Id = reader.GetInt32(reader.GetOrdinal("catId"));
						itemCategoryDTO.Name = reader.GetString(reader.GetOrdinal("catName"));
						itemCategoryDTO.ParentId = null;

						itemSubCategoryDTO.Id = reader.GetInt32(reader.GetOrdinal("subCatId"));
						itemSubCategoryDTO.Name = reader.GetString(reader.GetOrdinal("subCatName"));
						itemSubCategoryDTO.ParentId = reader.GetInt32(reader.GetOrdinal("parentCategory"));

						itemDTO.Category = itemCategoryDTO;
						itemDTO.SubCategory = itemSubCategoryDTO;

						lineItemDTO.ItemDTO = itemDTO;

						lineItems.Add(lineItemDTO);
					}
				}
			}
			catch (SqlException ex)
			{
				throw new Exception(ex.ToString());
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
			finally
			{
				conn.Close();
			}

			return lineItems;
		}

		bool IShoppingCartRepository.CreateShoppingCart(int clientId, LineItemDTO lineItemDTO)
		{
			GetConnection();
			conn.Open();
			SqlCommand cmd;
			SqlDataReader dreader;

			string sql = "BEGIN TRANSACTION;" +
						"INSERT INTO LineItem VALUES (@itemId, NULL, @purchasePrice, @amount);" +
						"DECLARE @id INT;" +
						"SET @id = IDENT_CURRENT('LineItem')" +
						"INSERT INTO ShoppingCart VALUES (@clientId, @id);" +
						"COMMIT;";

			//"INSERT INTO Item VALUES (@name, @subCategory, @price, @unitType, @available, @stockAmount);";

			cmd = new SqlCommand(sql, conn);
			cmd.Parameters.Add(new SqlParameter { ParameterName = "@itemId", Value = lineItemDTO.ItemDTO.Id });
			cmd.Parameters.Add(new SqlParameter { ParameterName = "@purchasePrice", Value = lineItemDTO.ItemDTO.Price });
			cmd.Parameters.Add(new SqlParameter { ParameterName = "@amount", Value = lineItemDTO.Amount });
			cmd.Parameters.Add(new SqlParameter { ParameterName = "@clientId", Value =  clientId});

			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				throw new Exception("Database error");
			}
			catch (Exception ex)
			{
				throw new Exception("Application error");
			}
			finally
			{
				cmd.Dispose();
				conn.Close();
			}
			return true;
		}
		ShoppingCartDTO IShoppingCartRepository.ReadShoppingCart(int clientId)
		{
			string Query = "SELECT clientId, lineItemId, purchasePrice, amount, " +
				"Item.id AS itemId, Item.name AS itemName, price, unitType, available, stockAmount, " +
				"Cat.id AS catId, cat.name AS catName, " +
				"SubCat.id AS subCatId, SubCat.name AS subCatName, SubCat.parentCategory " +
				"FROM ShoppingCart " +
				"LEFT JOIN LineItem ON LineItem.id = ShoppingCart.lineItemId " +
				"LEFT JOIN Item ON Item.id = LineItem.itemId " +
				"LEFT JOIN Category SubCat ON Item.subCategory = SubCat.id " +
				"LEFT JOIN Category Cat ON SubCat.parentCategory = Cat.id " +
				"WHERE ShoppingCart.clientId = @clientId";
			List<SqlParameter> sqlParameters = new List<SqlParameter>();

			try
			{
				sqlParameters.Add(new SqlParameter("@clientId", clientId));
				return new ShoppingCartDTO(){ AddedItems = GetShoppingCart(Query, sqlParameters).ToList()};
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		bool IShoppingCartRepository.UpdateShoppingCart(ShoppingCartDTO shoppingCartDTO)
		{
			throw new NotImplementedException();
		}

		bool IShoppingCartRepository.DeleteShoppingCart(ShoppingCartDTO shoppingCartDTO)
		{
			throw new NotImplementedException();
		}
	}
}