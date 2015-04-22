using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckoutKata.Classes
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<char, ItemDetails> _goodsList = new Dictionary<char, ItemDetails>();
        private readonly List<ItemDeal> _dealList = new List<ItemDeal>();

        public Boolean AddStock (char sku, String name, decimal price)
        {
            sku = Char.ToUpper(sku);
            if (_goodsList.ContainsKey(sku))
            {
                return false;
            }
            _goodsList.Add(sku, new ItemDetails()
            {
                Name = name,
                Price = price
            });
            return true;
        }

        public ItemDetails FindStock(char sku)
        {
            if (_goodsList.ContainsKey(sku))
            {
                return (_goodsList[sku]);
            }
            return null;
        }

        public int StockCount()
        {
            return _goodsList.Count;
        }

        public void AddDeal(char sku, uint qualifyingUnits, decimal discount)
        {
            _dealList.Add(new ItemDeal()
            {
                Sku = Char.ToUpper(sku),
                QualifyingUnits = qualifyingUnits,
                Discount = discount
            });
        }

        public int DealCount()
        {
            return _dealList.Count;
        }

        public decimal FindDealDiscount (char sku, int units)
        {
            //Locate the best deal on offer for the number of units specified or return zero if N/A
            decimal discount = 0;
            foreach (var deal in _dealList
                .Where(deal => (deal.Sku == sku) && (units >= deal.QualifyingUnits))
                .OrderBy(deal => deal.QualifyingUnits))
            {
                discount = Math.Floor (units / (decimal)deal.QualifyingUnits) * deal.Discount;
            }
            return discount;
        }
    }
}
