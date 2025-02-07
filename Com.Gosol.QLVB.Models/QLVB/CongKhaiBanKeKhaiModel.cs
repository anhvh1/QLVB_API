using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class CongKhaiBanKeKhaiModel
    {

        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public DateTime NgaySinh { get; set; }
        public string AnhHoSo { get; set; }
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }


        public int CongKhaiBanKeKhaiID { get; set; }
        public int KeKhaiID { get; set; }
        public int NamKeKhai { get; set; }
        public string TenBanKeKhai { get; set; }

        public List<int> DanhSachChucVuID { get; set; }
        public List<string> DanhSachTenChucVu { get; set; }
        public DateTime NgayDuyet { get; set; }
        public DateTime NgayHetHan { get; set; }
        public CongKhaiBanKeKhaiModel()
        {

        }

    }


}
