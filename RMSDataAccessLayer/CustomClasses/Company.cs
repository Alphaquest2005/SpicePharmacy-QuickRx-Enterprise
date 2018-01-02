using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMSDataAccessLayer
{
    public partial class Company
    {
        public string AddressInfo
        {
            get { return Address + " " + Address1 + " " + " " + PhoneNumber; }
        }
    }
}
