using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.QLVB
{
    public class DashboardModel
    {
        //List<SoLieuThongKe> SoLuong5NamGanDay { get; set; }
        //List<SoLieuThongKe> SoLuongThiSinhDuThiVaDo { get; set; }
    }
    public class SoLuong5NamGanDay
    {
        public int TongTotNghiepLoaiGioi { get; set; }
        public int TongTotNghiepLoaiKha { get; set; }
        public int TongTotNghiepTrungBinh { get; set; }
        public int TongTruot { get; set; }
        public List<SoLieuThongKe> SoLieuChiTiet { get; set; }
    }
    public class SoLuongThiSinhDuThiVaDo
    {
        public int Nam { get; set; }
        public int ThiSinhDuThi { get; set; }
        public int ThiSinhDo { get; set; }
        public int ThiSinhDuocCapBang { get; set; }
    }
    public class SoLieuThongKe
    {
        public int Nam { get; set; }
        public int TotNghiepLoaiGioi { get; set; }
        public int TotNghiepLoaiKha { get; set; }
        public int TotNghiepTrungBinh { get; set; }
        public int Truot { get; set; }
    }
}
