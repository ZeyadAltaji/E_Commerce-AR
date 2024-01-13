using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace E_CommerceAR.Domain.ModalsBase
{
    [FirestoreData]
    public class Products
    {
        [FirestoreProperty("category")]
        public string? Category { get; set; }

        [FirestoreProperty("colors")]
        public List<int> ColorValues { get; set; }

        [FirestoreProperty("images")]
        public List<string> images { get; set; }
        [FirestoreProperty("sizes")]
        public List<string> sizes { get; set; }
        [FirestoreProperty("dealerId")]
        public string? DealerId { get; set; }

        [FirestoreProperty("description")]
        public string? Description { get; set; }

        [FirestoreProperty("id")]
        public string? Id { get; set; }

        [FirestoreProperty("model")]
        public string? Model { get; set; }

        [FirestoreProperty("name")]
        public string? Name { get; set; }

        [FirestoreProperty("offerPercentage")]
        public double? OfferPercentage { get; set; }

        [FirestoreProperty("price")]
        public int? Price { get; set; }
        [FirestoreProperty("Attachments_Id")]
        public string Attachments_Id { get; set; }

    }



}
