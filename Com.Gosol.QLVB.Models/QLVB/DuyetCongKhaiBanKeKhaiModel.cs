using Com.Gosol.QLVB.Models.HeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class DuyetCongKhaiBanKeKhai
    {

        public int? CongKhaiBanKeKhaiID { get; set; }
        public int NguoiDuyetID { get; set; }
        public DateTime NgayDuyet { get; set; }
        public DateTime NgayHetHan { get; set; }
        public bool TrangThai { get; set; }
        public string GhiChu { get; set; }

        public DuyetCongKhaiBanKeKhai()
        {

        }
    }
    public class DuyetCongKhaiBanKeKhaiPartial
    {
        public DuyetCongKhaiBanKeKhai DuyetBanKeKhaiCongKhai { get; set; }
        public List<int> DanhSachBanKeKhaiID { get; set; }
        public List<int?> DanhSachCanBoXemBanKeKhaiID { get; set; }
        public DuyetCongKhaiBanKeKhaiPartial()
        {

        }

    }
    public class ChiTietCongKhaiBanKeKhai:KeKhaiCanBoModel
    {
        //public int KeKhaiID { get; set; }
        //public int CongKhaiBanKeKhaiID { get; set; }
        public List<CongKhaiCanBoModel> DanhSachCanBoXemBanKeKhai { get; set; }
    }

    public class KeKhaiCanBoModel
    {
        public int KeKhaiID { get; set; }
        public int CanBoID { get; set; }
        public int CongKhaiBanKeKhaiID { get; set; }
        public string TenCanBo { get; set; }
        public int ChucVuID { get; set; }
        public string TenChucVu { get; set; }
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }
        public DateTime NgayDuyet { get; set; }
        public DateTime NgayHetHan { get; set; }
        public bool TrangThai { get; set; }
        public string GhiChu { get; set; }
        public KeKhaiCanBoModel()
        {

        }
        public KeKhaiCanBoModel(int KeKhaiID, int CanBoID)
        {
            this.KeKhaiID = KeKhaiID;
            this.CanBoID = CanBoID;
        }
    }
    public class CongKhaiCanBoModel
    {
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }
        public List<int> DanhSachChucVuID { get; set; }
        public List<string> DanhSachTenChucVu { get; set; }
    }

    public class ThemCanBoXemBanKeKhaiModel
    {
        public int CongKhaiBanKeKhaiID { get; set; }
        public List<int?> DanhSachCanBoXemBanKeKhaiID { get; set; }
        public ThemCanBoXemBanKeKhaiModel()
        {

        }

    }
}
