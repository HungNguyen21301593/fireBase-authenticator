using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBaseAuthenticator.Model
{
    public class V3
    {
        public int RemainingPostLimit { get; set; }
        public string? UserName { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
