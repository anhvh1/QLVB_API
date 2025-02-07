using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class ThongTinTaiSanLogModel
    {
        public int ThongTinTaiSanLogID { get; set; }
        public int ThongTinTaiSanID { get; set; }
        public int? KeKhaiID { get; set; }
        public int? NhomTaiSanID { get; set; }
        public string TenNhomTaiSan { get; set; }
        public string TenTaiSanCu { get; set; }
        public string TenTaiSanMoi { get; set; }
        public string DienTichCu { get; set; }
        public string DienTichMoi { get; set; }
        public string GiaTriCu { get; set; }
        public string GiaTriMoi { get; set; }
        public string GiayChungNhanQuyenSuDungCu { get; set; }
        public string GiayChungNhanQuyenSuDungMoi { get; set; }
        public int? LoaiTaiSanIDCu { get; set; }
        public string TenLoaiTaiSanCu { get; set; }
        public int? LoaiTaiSanIDMoi { get; set; }
        public string TenLoaiTaiSanMoi { get; set; }
        public int? CapCongTrinhID { get; set; }
        public string GiaiTrinhNguonGocCu { get; set; }
        public string GiaiTrinhNguonGocMoi { get; set; }
        public string ThongTinKhacCu { get; set; }
        public string ThongTinKhacMoi { get; set; }
        public int? CanBoID { get; set; }
        public string TenCanbo { get; set; }
        public string NguoiDungTenCu { get; set; }
        public string NguoiDungTenMoi { get; set; }
        public bool? NguoiDungTenLaCanBoCu { get; set; }
        public bool? NguoiDungTenLaCanBoMoi { get; set; }
        public int NamKeKhai { get; set; }
        public string SoLuongCu { get; set; }
        public string SoLuongMoi { get; set; }
        public int? NhomTaiSanConID { get; set; }
        public string DiaChiCu { get; set; }
        public string DiaChiMoi { get; set; }
        public bool? LaBanTam { get; set; }
        public DateTime NgayChinhSua { get; set; }
        public int ThaoTac { get; set; }

        public ThongTinTaiSanLogModel()
        {

        }
        public ThongTinTaiSanLogModel(int ThongTinTaiSanID, int KeKhaiID, int NhomTaiSanID, string TenTaiSan, string DienTich, string GiaTri, string GiayChungNhanQuyenSuDungDat,
            int LoaiTaiSanID, int CapCongTrinhID, string GiaiTrinhNguonGoc, string ThongTinKhac, int CanBoID, string NguoiDungTen, bool NguoiDungTenLaCanBo, int NamKekhai, string SoLuong, int NhomTaiSanConID, string DiaChi)
        {
            this.ThongTinTaiSanID = ThongTinTaiSanID;
            this.KeKhaiID = KeKhaiID;
            this.NhomTaiSanID = NhomTaiSanID;
            this.TenTaiSanMoi = TenTaiSan;
            this.DienTichMoi = DienTich;
            this.GiaTriMoi = GiaTri;
            this.GiayChungNhanQuyenSuDungMoi = GiayChungNhanQuyenSuDungDat;
            this.LoaiTaiSanIDMoi = LoaiTaiSanID;
            this.CapCongTrinhID = CapCongTrinhID;
            this.GiaiTrinhNguonGocMoi = GiaiTrinhNguonGoc;
            this.ThongTinKhacMoi = ThongTinKhac;
            this.CanBoID = CanBoID;
            this.NguoiDungTenMoi = NguoiDungTen;
            this.NguoiDungTenLaCanBoMoi = NguoiDungTenLaCanBo;
            this.NamKeKhai = NamKekhai;
            this.SoLuongMoi = SoLuong;
            this.NhomTaiSanConID = NhomTaiSanConID;
            this.DiaChiMoi = DiaChi;
        }

    }
    public class ThongTinChinhSuaModel
    {

        public string Cu { get; set; }
        public string Moi { get; set; }
        public string ThongTinChinhSua { get; set; }

    }

    public class TaiSanLogModel
    {
        public int NhomTaiSanID { get; set; }
        public int ThongTinTaiSanID { get; set; }
        public string TenNhomTaisan { get; set; }
        public DateTime NgayChinhSua { get; set; }
        public string NguoiChinhSua { get; set; }
        public int ThaoTac { get; set; }
        public List<ThongTinChinhSuaModel> DanhSachChinhSua { get; set; }

    }
}
