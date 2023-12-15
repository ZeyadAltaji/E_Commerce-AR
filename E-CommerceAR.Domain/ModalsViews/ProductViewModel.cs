using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceAR.Domain.ModalsBase;

namespace E_CommerceAR.Domain.ModalsViews
{
    public class ProductViewModel
    {
        public Products Product { get; set; }
        public string DocumentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
