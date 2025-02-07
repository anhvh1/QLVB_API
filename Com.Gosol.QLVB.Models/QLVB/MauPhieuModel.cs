using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.QLVB
{
    public class MauPhieuModel
    {
        public int? MauPhieuID { get; set; }
        public string? TenMauPhieu { get; set; }
        public int? Nam { get; set; }
        public string? GhiChu { get; set; }
        //public int CotID { get; set; }
        //public int ThuTu { get; set; }
        //public List<int> DanhSachCotID { get; set; }
        public List<ChiTietMauPhieuModel> DanhSachChiTietMauPhieu { get; set; }

        public MauPhieuModel()
        {
            DanhSachChiTietMauPhieu = new List<ChiTietMauPhieuModel>();
            //DanhSachCotID = new List<int>();
        }
    }

    public class ChiTietMauPhieuModel
    {
        public int? CotID { get; set; }
        public string TieuDeCot { get; set; }
        public string MaCot { get; set; }
        public int? ThuTu { get; set; }
        public int? Loai { get; set; }
        public string? ViTri { get; set; }
        public int? NhomID { get; set; }
        public List<ChiTietMauPhieuModel> DanhSachCon { get; set; }
        //public List<int> DanhSachCotID { get; set; }
    }

    public class ChiTietMauPhieuTemp
    {
        public int? MauPhieuID { get; set; }
        public string? TenMauPhieu { get; set; }
        public int? Nam { get; set; }
        public string? GhiChu { get; set; }
        public int? CotID { get; set; }
        public string TieuDeCot { get; set; }
        public string MaCot { get; set; }
        public int? ThuTu { get; set; }
        public int? Loai { get; set; }
        public string? ViTri { get; set; }
        public int? NhomID { get; set; }
    }

    //public class ReadMauPhieuModel
    //{
    //    public DanhMucHoiDongThiModel HoiDong { get; set; }
    //    public DanhMucKhoaThiModel KhoaThi { get; set; }
    //    public List<ThongTinThiSinh> DanhSachThiSinh { get; set; }

    //}
}
