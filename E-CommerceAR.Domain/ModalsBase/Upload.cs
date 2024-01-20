using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsBase
{
    public class Upload
    {
        public string ProductId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ext { get; set; }
        public string linkDB { get; set; }
        public string base64 { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public string Types { get; set; }
 
    }
}
