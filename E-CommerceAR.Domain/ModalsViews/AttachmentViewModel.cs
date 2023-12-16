using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsViews
{
    public class AttachmentViewModel
    {
        public UsersViewModel UsersViewModel { get; set; }

        public string AttachmentId { get; set; }
        public string FileName { get; set; }
    }
}
