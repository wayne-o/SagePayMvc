namespace SagePayMvc.Sample.Models {
	using System;
	using System.Collections.Generic;
	using System.Web;
	using System.Linq;

	public class ShoppingBasketItem {
		public Product Product { get; set; }
		public int Id { get; set; }
		public int Quantity { get; set; }

		public decimal Price {
			get { return Product.Price * Quantity; }
		}
	}

	public class StoreShoppingBasket : IShoppingBasket {

		// This is not thread safe!
		// In reality, this would be generated by the database or your ORM tool.
		private static int _idCounter = 0;

		public ShoppingBasketItem[] GetItemsInBasket() {
			return GetBasketContents().ToArray();
		}

		public void AddProduct(Product product) {
			var contents = GetBasketContents();
			var currentItemForProduct = contents.SingleOrDefault(x => x.Product.Id == product.Id);

			if(currentItemForProduct != null) {
				currentItemForProduct.Quantity++;
			} else {
				// ID count is not thread safe. This is only here for demo purposes.
				// In reality, this would be generated by the database or your ORM tool.
				contents.Add(new ShoppingBasketItem { Id = ++_idCounter, Quantity = 1, Product = product });
			}
		}

		public void RemoveItem(int id) {
			var contents = GetBasketContents();
			var item = contents.Single(x => x.Id == id);
			item.Quantity--;
			if(item.Quantity <= 0) {
				contents.Remove(item);
			}
		}

		private IList<ShoppingBasketItem> GetBasketContents() {
			// Note: We're using the Session here directly.
			// In a real app, the basket contents would probably be stored in the database

			var contents = HttpContext.Current.Session["_basket"] as IList<ShoppingBasketItem>;
			if(contents == null) {
				contents = new List<ShoppingBasketItem>();
				HttpContext.Current.Session["_basket"] = contents;
			}
			return contents;
		}
	}

	public interface IShoppingBasket {
		ShoppingBasketItem[] GetItemsInBasket();
		void AddProduct(Product product);
		void RemoveItem(int id);
	}
}