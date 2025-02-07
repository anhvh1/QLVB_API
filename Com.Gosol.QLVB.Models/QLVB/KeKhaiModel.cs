using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class KeKhaiModel
    {

        public int KeKhaiID { get; set; }
        public int DotKeKhaiID { get; set; }
        public int CanBoID { get; set; }
        public int NamKeKhai { get; set; }
        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string TenBanKeKhai { get; set; }
        public int? CongKhaiBanKeKhaiID { get; set; }
        public int LoaiDotKeKhaiID { get; set; }
        public string TenLoaiDotKeKhai { get; set; }
        public bool BienDong { get; set; }
        public bool DongKeKhai { get; set; }
        public bool TrangThaiDotKeKhai { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool TrangThaiNhacViec { get; set; }
        public int TrangThaiCongKhai { get; set; }
        public string Barcode { get; set; }
        public List<FileDinhKemModel> DanhSachFileDinhKem { get; set; }
        public List<FileDinhKemModel> DanhSachFileDuyetDinhKem { get; set; }
        public KeKhaiModel()
        {

        }
        public KeKhaiModel(int KeKhaiID, int DotKeKhaiID, int CanBoID, int NamKeKhai, int TrangThaiID, string TenTrangThai, string TenBanKeKhai,int TrangThaiCongKhai)
        {
            this.KeKhaiID = KeKhaiID;
            this.DotKeKhaiID = DotKeKhaiID;
            this.CanBoID = CanBoID;
            this.NamKeKhai = NamKeKhai;
            this.TrangThai = TrangThaiID;
            this.TenTrangThai = TenTrangThai;
            this.TenBanKeKhai = TenBanKeKhai;
            this.TrangThaiCongKhai = TrangThaiCongKhai;
        }

        public KeKhaiModel(int KeKhaiID, string TenBanKeKhai)
        {
            this.KeKhaiID = KeKhaiID;
            this.TenBanKeKhai = TenBanKeKhai;
        }

        //public KeKhaiModel(int v1, int v2, int v3, int v4, int v5)
        //{
        //    this.v1 = v1;
        //    this.v2 = v2;
        //    this.v3 = v3;
        //    this.v4 = v4;
        //    this.v5 = v5;
        //}
    }
    public class KeKhaiModelPartial : KeKhaiModel
    {
        public string TenCanBo { get; set; }
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }
        public bool isPheDuyet { get; set; }
        public int CapCoQuan { get; set; }
        public int CapQuanLy { get; set; }
        public string ChucVuStr { get; set; }
        public List<FileDinhKemModel> DanhSachFileDinhKem { get; set; }
        public List<FileDinhKemModel> DanhSachFileDuyetDinhKem { get; set; }
        public List<FileDinhKemModel> DanhSachFileCongVan { get; set; }
        public string TenLoai { get; set; }
        public string SoCongVan { get; set; }
        public DateTime NgayDuyet { get; set; }
        public DateTime NgayHetHan { get; set; }
        public int? TrangThaiFilter { get; set; }
        public KeKhaiModelPartial()
        {

        }
        public KeKhaiModelPartial(string TenBanKeKhai, string TenCanBo, string TenCoQuan, string TenLoai)
        {
            this.TenBanKeKhai = TenBanKeKhai;
            this.TenCanBo = TenCanBo;
            this.TenCoQuan = TenCoQuan;
            this.TenLoai = TenLoai;
        }
    }
    public class KeKhaiModelParNew : KeKhaiModelPartial
    {
        public List<FileDinhKemModel> ListFileDinhKem { get; set; }
        public KeKhaiModelParNew()
        {

        }
        public KeKhaiModelParNew(List<FileDinhKemModel> ListFileDinhKem)
        {
            this.ListFileDinhKem = ListFileDinhKem;
        }
    }

}
