using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class BienDongTaiSanModelNew
    {


        public string TenTaiSan { get; set; }
        public string GiaiTrinhNguonGoc { get; set; }
        public string TrangThai { get; set; }
        public BienDongTaiSanModelNew()
        {

        }
        public BienDongTaiSanModelNew(string TenTaiSan, string GiaiTrinhNguonGoc, string TrangThai)
        {

            this.TenTaiSan = TenTaiSan;
            this.GiaiTrinhNguonGoc = GiaiTrinhNguonGoc;
            this.TrangThai = TrangThai;

        }
    }
}
