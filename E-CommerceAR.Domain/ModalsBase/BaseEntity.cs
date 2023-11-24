using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsBase
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public string UserCreate { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public string UserUpdate { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
    }
}
