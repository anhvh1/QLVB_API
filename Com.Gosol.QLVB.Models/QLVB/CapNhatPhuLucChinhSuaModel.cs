using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.QLVB
{
    public class CapNhatPhuLucChinhSuaModel
    {
        public int CapNhatPhuLucID { get; set; }
        public int? Nam { get; set; }
        public string SoQuyetDinh { get; set; }
        public string VeViec { get; set; }
        public FileDinhKemModel FileQuyetDinh { get; set; }
        public List<ThongTinThiSinh> ThongTinThiSinh { get; set; } 
    }
    public class ThongTinChinhSua
    {
        public int? ThiSinhID { get; set; }
        public int? CapNhatPhuLucID { get; set; }
        public string TenThongTin { get; set; }
        public string Ma { get; set; }
        public string GiaTriHienTai { get; set; }
        public string GiaTriMoi { get; set; }
        public string LyDo { get; set; }
        public DateTime? NgaySua { get; set; }
        public int? NguoiSua { get; set; }
        public string TenNguoiSua { get; set; }
    }

    public class NamTotNghiepTree
    {
        public string Name { get; set; }
        public int? Nam { get; set; }
        public int? CapNhatPhuLucID { get; set; }
        public string SoQuyetDinh { get; set; }
        public string VeViec { get; set; }
        public int? TongSoThiSinh { get; set; }
        public List<NamTotNghiepTree> children { get; set; }
    }
}
