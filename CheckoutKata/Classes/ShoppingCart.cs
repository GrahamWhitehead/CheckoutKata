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

        public void Scan(String skuScanned)
        {
            _goodsInCart = _goodsInCart + skuScanned.Trim().ToUpper();
        }

        private decimal GetNonDiscountTotal()
        {
            decimal total = 0.0m;
            foreach (var sku in _goodsInCart)
            {
                var itemDetails = _inventory.FindStock(sku);
                if (itemDetails != null)
                {
                    total = total + itemDetails.Price;
                }
            }
            return total;
        }

        private decimal GetDiscount()
        {
            decimal discount = 0.0m;
            foreach (var group in _goodsInCart.GroupBy(sku => sku))
            {
                discount = discount + _inventory.FindDealDiscount(group.Key, group.Count());
            }
            return discount;
        }

        public decimal Checkout()
        {
            decimal checkoutBill = GetNonDiscountTotal() - GetDiscount();
            _goodsInCart = String.Empty; //Upon checkout clear the cart
            return checkoutBill;
        }
    }
}
