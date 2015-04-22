using System;
using System.Linq;

namespace CheckoutKata.Classes
{
    public class ShoppingCart : IShoppingCart
    {
        private readonly Inventory _inventory = new Inventory();
        private string _goodsInCart = String.Empty;
        public Inventory Inventory
        {
            get { return _inventory; }
        }

        /// <summary>
        /// Adds additional items to the cart.  
        /// </summary>
        /// <param name="skuScanned">The parameter is a list of SKU's which identify each product
        /// For example, AAABB would add 3 x the product ID A and 2 x product ID B</param>
        public void Scan(String skuScanned)
        {
            _goodsInCart = _goodsInCart + skuScanned.Trim().ToUpper();
        }

        /// <summary>
        /// Returns the total cost of the items in the cart without taking any applicable discounts
        /// into account
        /// </summary>
        /// <returns>The calculated total cost of the items</returns>
        private decimal GetNonDiscountTotal()
        {
            decimal total = 0.0m;
            foreach (var sku in _goodsInCart)
            {
                var productDetails = _inventory.FindProduct(sku);
                if (productDetails != null)
                {
                    total = total + productDetails.Price;
                }
            }
            return total;
        }

        /// <summary>
        /// Calculates the amount of discount applicable based on the goods currently in the cart
        /// </summary>
        /// <returns>The amount of discount to deduct from the bill when checking out</returns>
        private decimal GetDiscount()
        {
            decimal discount = 0.0m;
            foreach (var group in _goodsInCart.GroupBy(sku => sku))
            {
                discount = discount + _inventory.FindDealDiscount(group.Key, group.Count());
            }
            return discount;
        }

        /// <summary>
        /// Checks out the goods currently contained in the shopping cart
        /// </summary>
        /// <returns>The total cost</returns>
        public decimal Checkout()
        {
            decimal checkoutBill = GetNonDiscountTotal() - GetDiscount();
            _goodsInCart = String.Empty; //Upon checkout clear the cart
            return checkoutBill;
        }
    }
}
