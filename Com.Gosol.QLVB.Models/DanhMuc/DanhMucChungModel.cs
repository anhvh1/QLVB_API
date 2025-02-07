using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.DanhMuc
{
    public class DanhMucChungModel
    {
        public int ID { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool? TrangThai { get; set; }
        public string GhiChu { get; set; }
        public int? Loai { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string ChucVu { get; set; }
        public int? CoQuanID { get; set; }
        public int? Nam { get; set; }
        public string? ViTri { get; set; }
    }
}
