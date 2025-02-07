using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.DanhMuc
{
    public class DanhMucKhoaThiModel
    {
        public int KhoaThiID { get; set; }
        public string TenKhoaThi { get; set; }
        public string MaKhoaThi { get; set; }
        public int? Nam { get; set; }
        public DateTime? Ngay { get; set; }
    }
}
