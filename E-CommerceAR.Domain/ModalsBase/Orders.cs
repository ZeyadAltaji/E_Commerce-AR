using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace E_CommerceAR.Domain.ModalsBase
{
    [FirestoreData]
    public class Address
    {
        [FirestoreProperty("phoneNumb")]
        public string PhoneNumber { get; set; }

        [FirestoreProperty("addressTitle")]
        public string AddressTitle { get; set; }

        [FirestoreProperty("city")]
        public string City { get; set; }

        [FirestoreProperty("street")]
        public string Street { get; set; }

        [FirestoreProperty("fullName")]
        public string FullName { get; set; }

        [FirestoreProperty("state")]
        public string State { get; set; }
    }

    [FirestoreData]
    public class Product
    {
        [FirestoreProperty("images")]
        public List<string> Images { get; set; }

        [FirestoreProperty("sizes")]
        public List<string> Sizes { get; set; }

        [FirestoreProperty("offerPercentage")]
        public double? OfferPercentage { get; set; }

        [FirestoreProperty("price")]
        public double? Price { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("model")]
        public string Model { get; set; }

        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("category")]
        public string Category { get; set; }

        [FirestoreProperty("colors")]
        public List<int> Colors { get; set; }
        [FirestoreProperty("dealerId")]
        public string dealerId { get; set; }
    }

    [FirestoreData]
    public class OrderItem
    {
        [FirestoreProperty("product")]
        public Product Product { get; set; }

        [FirestoreProperty("quantity")]
        public int Quantity { get; set; }

        [FirestoreProperty("selectedColor")]
        public int? SelectedColor { get; set; }
    }

    [FirestoreData]
    public class Orders
    {
        [FirestoreProperty("date")]
        public string Date { get; set; }

        [FirestoreProperty("address")]
        public Address Address { get; set; }

        [FirestoreProperty("orderId")]
        public long OrderId { get; set; }

        [FirestoreProperty("totalPrice")]
        public double TotalPrice { get; set; }

        [FirestoreProperty("orderStatus")]
        public string OrderStatus { get; set; }

        [FirestoreProperty("products")]
        public List<OrderItem> Products { get; set; }

        [FirestoreProperty("createTime")]
        public DateTime CreateTime { get; set; }

        [FirestoreProperty("updateTime")]
        public DateTime UpdateTime { get; set; }
 
    }
}
