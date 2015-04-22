using System;

namespace CheckoutKata.Classes
{
    interface IInventory
    {
        Boolean AddStock(char sku, String name, decimal price);
        ItemDetails FindStock(char sku);
        int StockCount();
        void AddDeal(char sku, uint qualifyingUnits, decimal discount);
        int DealCount();
    }   
}
