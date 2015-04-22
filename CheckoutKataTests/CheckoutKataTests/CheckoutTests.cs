using System;
using CheckoutKata.Classes;
using NUnit.Framework;

namespace CheckoutKataTests
{
    [TestFixture]
    public class CreateInventoryTests
    {
        [Test]
        public void Add_Stock()
        {
            var inventory = new Inventory();
            Assert.IsTrue (inventory.AddStock('A', "Apple", 10));
            Assert.AreEqual(1, inventory.ProductCount());
        }

        [Test]
        public void Add_Multi_Stock()
        {
            var inventory = new Inventory();
            Assert.IsTrue (inventory.AddStock('A', "Apple", 10));
            Assert.IsTrue (inventory.AddStock('B', "Banana", 20));
            Assert.AreEqual(2, inventory.ProductCount());
        }

        [Test]
        public void Add_Duplicated_Stock()
        {
            var inventory = new Inventory();
            Assert.IsTrue (inventory.AddStock('A', "Apple", 10));
            Assert.IsFalse (inventory.AddStock('a', "APPLE", 10));
            Assert.AreEqual(1, inventory.ProductCount());
        }

        [Test]
        public void Access_Stock()
        {
            var inventory = new Inventory();
            Assert.IsTrue (inventory.AddStock('A', "Apple", 10));
            Assert.IsTrue (inventory.AddStock('B', "Banana", 20));
            Assert.AreEqual(10, inventory.FindProduct('A').Price);
            Assert.AreEqual("Apple", inventory.FindProduct('A').Name);
            Assert.AreEqual(20, inventory.FindProduct('B').Price);
            Assert.AreEqual("Banana", inventory.FindProduct('B').Name);
        }

        [Test]
        public void Access_Invalid_Stock()
        {
            var inventory = new Inventory();
            Assert.AreEqual(0, inventory.ProductCount());
            Assert.IsNull(inventory.FindProduct('A'));
        }
    }
    
    [TestFixture]
    public class CreateDealTests
    {
        [Test]
        public void Add_Single_Deal()
        {
            var inventory = new Inventory();
            inventory.AddDeal('A', 3, 20);
            Assert.AreEqual(1, inventory.DealCount());
        }

        [Test]
        public void Add_Multiple_Deals()
        {
            var inventory = new Inventory();
            inventory.AddDeal('A', 3, 20);
            inventory.AddDeal('B', 2, 15);
            Assert.AreEqual(2, inventory.DealCount());
        }

        [Test]
        public void Add_Multi_Level_Deal()
        {
            var inventory = new Inventory();
            inventory.AddDeal('A', 3, 20);
            inventory.AddDeal('a', 5, 50);
            Assert.AreEqual(2, inventory.DealCount());
        }
    }

    [TestFixture]
    public class CheckoutTests
    {
        private ShoppingCart _shoppingCart;

        [TestFixtureSetUp]
        public void Setup()
        {
            _shoppingCart = new ShoppingCart();
            _shoppingCart.Inventory.AddStock('A', "Apple", 50);
            _shoppingCart.Inventory.AddStock('B', "Banana", 30);
            _shoppingCart.Inventory.AddStock('C', "Cherry", 20);
            _shoppingCart.Inventory.AddStock('D', "Damson", 15);
        }

        [Test]
        public void Test_Checkout_No_Scan()
        {
            Assert.AreEqual(0, _shoppingCart.Checkout());
        }

        [Test]
        [TestCase('A', Result = 50)]
        [TestCase('a', Result = 50)]
        [TestCase('B', Result = 30)]
        [TestCase('C', Result = 20)]
        [TestCase('D', Result = 15)]
        [TestCase('E', Result = 0)]
        public decimal Test_Single_SKU_Total(char sku)
        {
            _shoppingCart.Scan(sku.ToString());
            return _shoppingCart.Checkout();
        }

        [Test]
        [TestCase("Aa", Result = 100)]
        [TestCase("AB", Result = 80)]
        [TestCase("ABCD", Result = 115)]
        [TestCase("DCBA", Result = 115)]
        [TestCase("cdcd", Result = 70)]
        [TestCase("ABCDE", Result = 115)]
        [TestCase(" ABCDE ", Result = 115)]
        public decimal Test_Multi_SKU_Total(String skuList)
        {
            _shoppingCart.Scan(skuList);
            return _shoppingCart.Checkout();
        }

        [Test]
        [TestCase("Ac", "aC", Result = 140)]
        [TestCase("", "AA", Result = 100)]
        public decimal Test_Multi_Scan_SKU_Total(String skuList1, String skuList2)
        {
            _shoppingCart.Scan(skuList1);
            _shoppingCart.Scan(skuList2);
            return _shoppingCart.Checkout();            
        }
    }

    [TestFixture]
    public class CheckoutWithDealTests
    {
        private ShoppingCart _shoppingCart;

        [TestFixtureSetUp]
        public void Setup()
        {
            _shoppingCart = new ShoppingCart();
            _shoppingCart.Inventory.AddStock('A', "Apple", 50);
            _shoppingCart.Inventory.AddStock('B', "Banana", 30);
            _shoppingCart.Inventory.AddStock('C', "Cherry", 20);
            _shoppingCart.Inventory.AddStock('D', "Damson", 15);
            _shoppingCart.Inventory.AddDeal('A', 3, 20); //3 for 130
            _shoppingCart.Inventory.AddDeal('B', 2, 15); //2 for 45
        }

        [Test]
        [TestCase("AAa", Result = 130)]
        [TestCase("Bb", Result = 45)]
        [TestCase("ABABA", Result = 175)]
        [TestCase(" BabAax ", Result = 175)]
        [TestCase("AAABBAAA", Result = 305)]
        [TestCase("BBCBBDBB", Result = 170)]
        public decimal Test_Checkout_With_Applicable_Deal(String skuList)
        {
            _shoppingCart.Scan(skuList);
            return _shoppingCart.Checkout();
        }

        [Test]
        [TestCase("AA", "AAAA", Result = 260)]
        [TestCase("AAB", "AABAA", Result = 305)]
        public decimal Test_Checkout_With_Applicable_Deal_Multi_Scans(String skuList1, String skuList2)
        {
            _shoppingCart.Scan(skuList1);
            _shoppingCart.Scan(skuList2);
            return _shoppingCart.Checkout();           
        }

        [Test]
        public void Test_Multiple_Checkouts()
        {
            _shoppingCart.Scan("AAA");
            _shoppingCart.Checkout();
            _shoppingCart.Scan("AAA");
            Assert.AreEqual(130, _shoppingCart.Checkout());
        }
    }

    [TestFixture]
    public class CheckoutWithMultiLevelDealTests
    {
        private ShoppingCart _shoppingCart;

        [Test]
        [TestCase("AAA", Result = 130)]
        [TestCase("AAAA", Result = 180)]
        [TestCase("AAAAA", Result = 200)]
        [TestCase("AAAAAA", Result = 250)] //Expect 250 from the 5 for 200 + 1 extra unit and not 260 from the 2 x 3 for 130 offer
        public decimal Test_Checkout_With_Applicable_Deal(String skuList)
        {
            _shoppingCart = new ShoppingCart();
            _shoppingCart.Inventory.AddStock('A', "Apple", 50);
            _shoppingCart.Inventory.AddDeal('A', 3, 20); //3 for 130
            _shoppingCart.Inventory.AddDeal('a', 5, 50); //5 for 200
            _shoppingCart.Scan(skuList);
            return _shoppingCart.Checkout();
        }

        [Test]
        [TestCase("AAA", Result = 130)]
        [TestCase("AAAA", Result = 180)]
        [TestCase("AAAAA", Result = 200)]
        [TestCase("AAAAAA", Result = 250)] //Expect 250 from the 5 for 200 + 1 extra unit and not 260 from the 2 x 3 for 130 offer
        public decimal Test_Checkout_With_Applicable_Deal_Reversed_In_Setup(String skuList)
        {
            _shoppingCart = new ShoppingCart();
            _shoppingCart.Inventory.AddStock('A', "Apple", 50);
            _shoppingCart.Inventory.AddDeal('a', 5, 50); //5 for 200
            _shoppingCart.Inventory.AddDeal('A', 3, 20); //3 for 130
            _shoppingCart.Scan(skuList);
            return _shoppingCart.Checkout();
        }
    }
}
