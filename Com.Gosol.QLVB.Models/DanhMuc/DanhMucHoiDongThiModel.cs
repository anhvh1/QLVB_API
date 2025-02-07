using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.DanhMuc
{
    public class DanhMucHoiDongThiModel
    {
        public int HoiDongThiID { get; set; }
        public string TenHoiDong { get; set; }
        public string DiaDiemThi { get; set; }
        public string PhongThi { get; set; }  
        public int? SoThiSinhDuThi { get; set; }
    }
}
