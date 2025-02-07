using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class User
    {
        public int id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
