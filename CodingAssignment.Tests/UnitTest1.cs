using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CodingAssignment.Tests
{
    public class TestDataGenerator : IEnumerable<object[]>
    {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] {
                    new List<Order>
                    {
                        new Order { ItemSku = "A", Quantity = 5 },
                        new Order { ItemSku = "B", Quantity = 3 },
                        new Order { ItemSku = "C", Quantity = 2 },
                        new Order { ItemSku = "D", Quantity = 1 }
                    },
                    355
                },
                new object[] {
                    new List<Order>
                    {
                        new Order { ItemSku = "A", Quantity = 3 },
                        new Order { ItemSku = "B", Quantity = 2 },
                        new Order { ItemSku = "C", Quantity = 3 },
                        new Order { ItemSku = "D", Quantity = 1 }
                    },
                    245
                },
                new object[] {
                    new List<Order>
                    {
                        new Order { ItemSku = "A", Quantity = 3 },
                        new Order { ItemSku = "B", Quantity = 2 },
                        new Order { ItemSku = "C", Quantity = 0 },
                        new Order { ItemSku = "D", Quantity = 3 }
                    },
                    220
                }
            };
            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class UnitTest1
        {
            List<Item> itemDb = new List<Item> {
                new Item { ItemSku = "A", UnitPrice = 50},
                new Item { ItemSku = "B", UnitPrice = 30 },
                new Item { ItemSku = "C", UnitPrice = 20 },
                new Item { ItemSku = "D", UnitPrice = 15 }
            };
            List<AvailablePromotion> availablePromotions = new List<AvailablePromotion> {
                new AvailablePromotion("A", PromotionType.quantity, 100, 130, 3, 50, null),
                new AvailablePromotion("B", PromotionType.quantity, 100, 45, 2, 45, null),
                new AvailablePromotion("C", PromotionType.combo, 100, 30, 1, 20, new List<string> {"C", "D" })
            };
            
        
            [Theory]
            [ClassData(typeof(TestDataGenerator))]
            public void TestGetTotalPriceAfterPromotionApplied(List<Order> orders, decimal expectedResult)
            {
                var actualResult =  Discount.GetTotalPriceAfterPromotionApplied(orders, availablePromotions, itemDb);
                Console.WriteLine(actualResult);
                Assert.Equal(expectedResult.ToString(), actualResult.ToString());
            }
        }
}
