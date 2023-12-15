using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsBase
{
    [FirestoreData]

    public class ProductList
    {
        [FirestoreProperty("product")]

        public ProductDetails ProductDetails { get; set; }
        [FirestoreProperty("quantity")]

        public int Quantity { get; set; }
        [FirestoreProperty("selectedColor")]

        public int SelectedColor { get; set; }
        [FirestoreProperty("selectedSize")]

        public string? SelectedSize { get; set; }
    }
}
