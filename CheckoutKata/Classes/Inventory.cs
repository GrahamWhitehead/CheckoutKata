using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckoutKata.Classes
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<char, ItemDetails> _productList = new Dictionary<char, ItemDetails>();
        private readonly List<ItemDeal> _dealList = new List<ItemDeal>();

        /// <summary>
        /// Adds a new product into the repository
        /// </summary>
        /// <param name="sku">An SKU to identify this new product.  This must be unique and no duplicated products will be added.</param>
        /// <param name="name">The name / description of the product</param>
        /// <param name="price">The current price per unit for this product</param>
        /// <returns></returns>
        public Boolean AddStock (char sku, String name, decimal price)
        {
            sku = Char.ToUpper(sku);
            if (_productList.ContainsKey(sku))
            {
                return false;
            }
            _productList.Add(sku, new ItemDetails()
            {
                Name = name,
                Price = price
            });
            return true;
        }

        /// <summary>
        /// Searches for a product referenced by the specified SKU
        /// </summary>
        /// <param name="sku">The SKU identifying the product to search for in the repository</param>
        /// <returns>Either the product object or null if no match is found</returns>
        public ItemDetails FindProduct(char sku)
        {
            if (_productList.ContainsKey(sku))
            {
                return (_productList[sku]);
            }
            return null;
        }

        /// <summary>
        /// Returns the number of stock items configured in the system
        /// </summary>
        /// <returns>Returns the number of stock items configured in the system</returns>
        public int ProductCount()
        {
            return _productList.Count;
        }

        /// <summary>
        /// Adds a new deal into the system
        /// </summary>
        /// <param name="sku">The product ID linking to the deal</param>
        /// <param name="qualifyingUnits">The number of units that must be purchased for the deal to be applicable</param>
        /// <param name="discount">The amount of discount to subtract from the total billed to the user</param>
        public void AddDeal(char sku, uint qualifyingUnits, decimal discount)
        {
            _dealList.Add(new ItemDeal()
            {
                Sku = Char.ToUpper(sku),
                QualifyingUnits = qualifyingUnits,
                Discount = discount
            });
        }

        /// <summary>
        /// The number of deals currently configured in the system
        /// </summary>
        /// <returns>The number of deals currently configured in the system</returns>
        public int DealCount()
        {
            return _dealList.Count;
        }

        /// <summary>
        /// Searches the current inventory to obtain the best deal against the specified sku and
        /// the number of units purchased.
        /// </summary>
        /// <param name="sku">An SKU which identifies the product to look up</param>
        /// <param name="units">The number of units purchased to use in the deal calculation</param>
        /// <returns>The best deal found or zero if no deals are applicable</returns>
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
