using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace E_CommerceAR.Domain.ModalsViews
{
	[FirestoreData]

	public class ProductDetails
	{
		[FirestoreProperty("category")]

		public string? Category { get; set; }
		[FirestoreProperty("colors")]

		public List<int>? Colors { get; set; }
		[FirestoreProperty("description")]

		public string? Description { get; set; }
		[FirestoreProperty("id")]

		public string? Id { get; set; }
		[FirestoreProperty("images")]

		public List<Images> Images { get; set; }
		[FirestoreProperty("model")]

		public string? Model { get; set; }
		[FirestoreProperty("name")]

		public string? Name { get; set; }
		[FirestoreProperty("offerPercentage")]

		public double OfferPercentage { get; set; }
		[FirestoreProperty("price")]

		public double Price { get; set; }
		[FirestoreProperty("sizes")]

		public List<string>? Sizes { get; set; }
	}
}
