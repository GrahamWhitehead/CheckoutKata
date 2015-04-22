using System;

namespace CheckoutKata.Classes
{
    interface IInventory
    {
        Boolean AddStock(char sku, String name, decimal price);
        ItemDetails FindProduct(char sku);
        int ProductCount();
        void AddDeal(char sku, uint qualifyingUnits, decimal discount);
        int DealCount();
    }   
}
