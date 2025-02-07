using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class DuyetBanKeKhaiModel
    {
        public int DuyetBanKeKhaiID { get; set; }
        public int KeKhaiID { get; set; }
        public int NguoiDuyetID { get; set; }
        public int PheDuyet { get; set; } // 1 - Duyệt; 2 - Hủy duyệt
        public int TrangThaiDuyet { get; set; } // 101 - hủy duyệt cấp 1; 300 - Duyệt cấp 1; 201 - hủy duyệt cấp 2; 400 - duyệt cấp 2
        public DateTime NgayDuyet { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string GhiChu { get; set; }
        public bool TrangThaiNhacViec { get; set; }
        public List<FileModel> DanhSachFileDinhKem { get; set; }
        public DuyetBanKeKhaiModel()
        {

        }
    }
    public class DuyetBanKeKhaiModelPar : DuyetBanKeKhaiModel
    {
        public string TenCanBo { get; set; }
    }


}
