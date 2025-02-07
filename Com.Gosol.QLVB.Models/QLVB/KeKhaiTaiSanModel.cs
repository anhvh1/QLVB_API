using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class KeKhaiTaiSanModel
    {
        public int KeKhaiID { get; set; }
        public int? DotKeKhaiID { get; set; }
        public int? CanBoID { get; set; }
        public int? NamKeKhai { get; set; }
        public int? TrangThaiID { get; set; }
        public KeKhaiTaiSanModel()
        {

        }
        public KeKhaiTaiSanModel(int KeKhaiID, int DotKeKhaiID, int CanBoID, int NamKeKhai, int TrangThaiID)
        {
            this.KeKhaiID = KeKhaiID;
            this.DotKeKhaiID = DotKeKhaiID;
            this.CanBoID = CanBoID;
            this.NamKeKhai = NamKeKhai;
            this.TrangThaiID = TrangThaiID;
        }
    }
}
