using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Ticket { get; set; }
        public string GrantType { get; set; }
       
    }
}
