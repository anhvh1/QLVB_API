using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models
{
    public class NhacViecModel
    {
        public string Name { get; set; }
        public string NoiDung { get; set; }
        public string Key { get; set; }
        public NhacViecModel()
        {

        }
        public NhacViecModel(string Name, string NoiDung, string Key)
        {
            this.Name = Name;
            this.NoiDung = NoiDung;
            this.Key = Key;
        }
    }
}
