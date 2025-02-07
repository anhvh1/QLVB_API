using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models
{
    public class ThongBaoModel
    {
        public int? ThongBaoID { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string Name { get { return NoiDung; } }
        public string Key { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public int? LoaiThongBao { get; set; }
        public int? NghiepVuID { get; set; }
        public string TenNghiepVu { get; set; }
        public Boolean? HienThi { get; set; }
        public List<DoiTuongThongBaoModel> DoiTuongThongBao { get; set; }
    }

    public class DoiTuongThongBaoModel
    {
        public int DoiTuongThongBaoID { get; set; }
        public int ThongBaoID { get; set; }
        public int? CanBoID { get; set; }
        public int? CoQuanID { get; set; }
        public string Email { get; set; }
        public string TenCanBo { get; set; }
        public int? GioiTinh { get; set; }
    }

    public class ThongBaoChiTietModel : ThongBaoModel
    {
        public int DoiTuongThongBaoID { get; set; }
        public int? CanBoID { get; set; }
        public int? CoQuanID { get; set; }
        public string Email { get; set; }
        public string TenCanBo { get; set; }
        public int? GioiTinh { get; set; }
    }
}
