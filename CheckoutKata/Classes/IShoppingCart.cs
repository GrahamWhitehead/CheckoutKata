using System;

namespace CheckoutKata.Classes
{
    interface IShoppingCart
    {
        void Scan(String skuScanned);
        decimal Checkout();
    }
}
