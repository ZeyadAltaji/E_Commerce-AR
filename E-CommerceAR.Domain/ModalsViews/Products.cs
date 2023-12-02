using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsViews
{
    [FirestoreData]
    public class Products
    {
        [FirestoreProperty("category")]
        public string Category { get; set; }




        [FirestoreProperty("dealerId")]
        public string DealerId { get; set; }

        [FirestoreProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("id")]
        public string Id { get; set; }



        [FirestoreProperty("model")]
        public string Model { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("offerPercentage")]
        public double OfferPercentage { get; set; }

        [FirestoreProperty("price")]
        public int Price { get; set; }

    }
  

    
    
}
