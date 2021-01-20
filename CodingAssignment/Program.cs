using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodingAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Item> itemDb = new List<Item> {
            new Item { ItemSku = "A", UnitPrice = 50},
            new Item { ItemSku = "B", UnitPrice = 30 },
            new Item { ItemSku = "C", UnitPrice = 20 },
            new Item { ItemSku = "D", UnitPrice = 15 }
            };
            List<AvailablePromotion> availablePromotions = new List<AvailablePromotion>
            {
                new AvailablePromotion("A", PromotionType.percentage, 70, 130, 3, 50, null),
                new AvailablePromotion("B", PromotionType.quantity, 100, 45, 2, 45, null),
                new AvailablePromotion("C", PromotionType.combo, 100, 30, 2, 20, new List<string> {"C", "D" })
            };
            Console.WriteLine("Calculate Promotion");
            List<Order> orders = new List<Order>
            {
                new Order { ItemSku = "A", Quantity = 3 },
                new Order { ItemSku = "B", Quantity = 2 },
                new Order { ItemSku = "C", Quantity = 0 },
                new Order { ItemSku = "D", Quantity = 3 }
            };
            var calculatedPrice = (Discount.GetTotalPriceAfterPromotionApplied(orders, availablePromotions, itemDb));
            Console.WriteLine(calculatedPrice);
            Console.ReadLine();
        }
    }

    public class AvailablePromotion
    {
        public string PromotionApplicableOnSku { get; set; }
        public PromotionType PromotionType { get; set; }
        // discount percentage
        public int DiscountPercentage { get; set; } = 100;
        // offer price
        public decimal PromotionPrice { get; set; } = 0;
        // minimum order quantity required for being eligible for the discount 
        public int MinQuantityToAvailPromotion { get; set; } = 1;
        public decimal PromotedPriceForUnitItem { get; set; }
        public decimal RegularPriceForUnitItem { get; set; }
        public List<string> CombinationItemList { get; set; }
        /// <summary>
        /// initializes the available promotion object with passed values
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="promotionType"></param>
        /// <param name="discountPercentage"></param>
        /// <param name="promotionPrice"></param>
        /// <param name="minQuantityToAvailPromotion"></param>
        /// <param name="regularPriceForUnitItem"></param>
        /// <param name="combinationList"></param>
        public AvailablePromotion(string sku, PromotionType promotionType, int discountPercentage,
            decimal promotionPrice, int minQuantityToAvailPromotion, decimal regularPriceForUnitItem, List<string> combinationList)
        {
            this.PromotionApplicableOnSku = sku;
            this.PromotionType = promotionType;
            this.DiscountPercentage = discountPercentage;
            this.PromotionPrice = promotionPrice;
            this.MinQuantityToAvailPromotion = minQuantityToAvailPromotion;
            this.RegularPriceForUnitItem = regularPriceForUnitItem;
            this.CombinationItemList = combinationList;
            this.PromotedPriceForUnitItem = CalculatePromotedPriceForUnitItem();
        }
        /// <summary>
        /// calculates the unit price for the item, for which promotion is available
        /// </summary>
        /// <returns></returns>
        private decimal CalculatePromotedPriceForUnitItem()
        {
            decimal unitPrice = 0;
            switch (PromotionType)
            {
                // 
                case PromotionType.percentage: unitPrice = (RegularPriceForUnitItem * (100 - DiscountPercentage) / 100); break;
                //
                case PromotionType.quantity: unitPrice = (PromotionPrice * 1 / MinQuantityToAvailPromotion); break;
                //
                case PromotionType.combo: unitPrice = (PromotionPrice / CombinationItemList.Count); break;

                default: break;
            }
            return unitPrice;
        }
    }

    public enum PromotionType
    {
        combo = 1,
        percentage,
        quantity
    }

    public class Item
    {
        public string ItemSku { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Order
    {
        public string ItemSku { get; set; }
        public int Quantity { get; set; }
    }
    public class Discount
    {
        public static double GetTotalPriceAfterPromotionApplied(List<Order> orderItems, List<AvailablePromotion> availablePromotions, List<Item> itemDb)
        {
            decimal totalPrice = 0;
            foreach (Order orderItem in orderItems)
            {
                // fetching item from item db
                var item = itemDb.Where(item => item.ItemSku == orderItem.ItemSku).FirstOrDefault();
                // if item does not belong to db then stop calculation for that item entered
                if (item == null)
                {
                    break;
                }
                // check if promotion offer available
                AvailablePromotion availablePromotion = availablePromotions.Where(x => x.PromotionApplicableOnSku == orderItem.ItemSku).FirstOrDefault();
                // promotion available, then calculate price using promotion
                if (availablePromotion != null && orderItem.Quantity >= availablePromotion.MinQuantityToAvailPromotion)
                {
                    if (availablePromotion.PromotionType != PromotionType.combo)
                    {
                        var quantityAvailableForPromotion = (orderItem.Quantity / availablePromotion.MinQuantityToAvailPromotion)
                        * availablePromotion.MinQuantityToAvailPromotion;
                        var quantityNotAvailableForPromotion = orderItem.Quantity - quantityAvailableForPromotion;
                        // promotion available
                        totalPrice += availablePromotion.PromotedPriceForUnitItem * availablePromotion.MinQuantityToAvailPromotion;
                        totalPrice += (quantityNotAvailableForPromotion * item.UnitPrice);
                    }
                    // check if promotion type is of combination or not
                    if (availablePromotion.PromotionType == PromotionType.combo)
                    {
                        var availableComboOffers = availablePromotions.Where(promotion => promotion.PromotionType == PromotionType.combo
                        && promotion.PromotionApplicableOnSku == orderItem.ItemSku).FirstOrDefault();
                        var skusOtherThanSelectedSku = availableComboOffers.CombinationItemList.Where(comboItems => comboItems != orderItem.ItemSku);
                        var combinationCount = orderItem.Quantity;
                        foreach (string comboSku in skusOtherThanSelectedSku)
                        {
                            var oItem = orderItems.Where(orderItem => orderItem.ItemSku == comboSku).FirstOrDefault();
                            if (oItem != null && oItem.Quantity < combinationCount)
                            {
                                combinationCount = oItem.Quantity;
                            }
                        }
                        // combinationCount = (combinationCount / availablePromotion.CombinationItemList.Count);
                        totalPrice += (combinationCount * availablePromotion.PromotionPrice);
                        totalPrice += (orderItem.Quantity - combinationCount) * item.UnitPrice;
                        foreach (string comboSku in skusOtherThanSelectedSku)
                        {
                            var oItem = orderItems.Where(orderItem => orderItem.ItemSku == comboSku).FirstOrDefault();
                            if (oItem != null)
                            {
                                oItem.Quantity -= combinationCount;
                            }
                        }
                    }
                }
                // promotion not available
                else
                {
                    totalPrice += orderItem.Quantity * item.UnitPrice;
                }
            }
            return Math.Round(Convert.ToDouble(totalPrice));
        }
    }

}
