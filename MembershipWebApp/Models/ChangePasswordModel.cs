using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MembershipWebApp.Models
{
    public class ChangePasswordModel
    {
        public string AncienPassword { get; set; }
        public string NouveauPassword { get; set; }
    }
}