//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class ThongTinTaiSanModel
    {
        public int ThongTinTaiSanID { get; set; }
        public int? KeKhaiID { get; set; }
        public int? NhomTaiSanID { get; set; }
        public string TenTaiSan { get; set; }
        public double? DienTich { get; set; }
        public double? GiaTri { get; set; }
        public string GiayChungNhanQuyenSuDung { get; set; }
        public int? LoaiTaiSanID { get; set; }
        public int? CapCongTrinhID { get; set; }
        public string GiaiTrinhNguonGoc { get; set; }
        public string ThongTinKhac { get; set; }
        public int? CanBoID { get; set; }
        public int? NguoiDungTen { get; set; }
        public bool? NguoiDungTenLaCanBo { get; set; }
        public int NamKeKhai { get; set; }
        public int? SoLuong { get; set; }
        public int? NhomTaiSanConID { get; set; }
        public string DiaChi { get; set; }
        public bool? LaBanTam { get; set; }
        public ThongTinTaiSanModel()
        {

        }
        public ThongTinTaiSanModel(int ThongTinTaiSanID, int KeKhaiID, int NhomTaiSanID, string TenTaiSan, double DienTich, double GiaTri, string GiayChungNhanQuyenSuDungDat,
            int LoaiTaiSanID, int CapCongTrinhID, string GiaiTrinhNguonGoc, string ThongTinKhac, int SoLuong, int NhomTaiSanConID)
        {
            this.ThongTinTaiSanID = ThongTinTaiSanID;
            this.KeKhaiID = KeKhaiID;
            this.NhomTaiSanID = NhomTaiSanID;
            this.TenTaiSan = TenTaiSan;
            this.DienTich = DienTich;
            this.GiaTri = GiaTri;
            this.GiayChungNhanQuyenSuDung = GiayChungNhanQuyenSuDungDat;
            this.LoaiTaiSanID = LoaiTaiSanID;
            this.CapCongTrinhID = CapCongTrinhID;
            this.GiaiTrinhNguonGoc = GiaiTrinhNguonGoc;
            this.ThongTinKhac = ThongTinKhac;
            this.SoLuong = SoLuong;
            this.NhomTaiSanConID = NhomTaiSanConID;
        }
    }
    //[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThongTinTaiSanModelPartial : ThongTinTaiSanModel
    {
        //[JsonProperty("TenCapCongTrinh")]
        public string TenCapCongTrinh { get; set; }
        //[JsonProperty("NguoiDungTenID")]
        public int NguoiDungTenID { get; set; }
        public int? NhomTaiSanChaID { get; set; }
        public string TenLoaiTaiSan { get; set; }
        public int TrangThaiBanKeKhai { get; set; }
        public string TenNhomTaiSan { get; set; }
        public string TenNguoiDungTen { get; set; }
        public string TenBanKeKhai { get; set; }
        public int TangGiam { get; set; }
        public bool TrangThaiSuDung { get; set; }
        public int SoLuongTangGiam { get; set; }
        public ThongTinTaiSanModelPartial()
        {

        }
        public ThongTinTaiSanModelPartial(string TenCapCongTrinh, int NguoiDungTenID)
        {
            this.TenCapCongTrinh = TenCapCongTrinh;
            this.NguoiDungTenID = NguoiDungTenID;
        }
    }
    public class NguoiDungTenModel
    {
        public int ID { get; set; }
        public string Ten { get; set; }
        public NguoiDungTenModel()
        {

        }
        public NguoiDungTenModel(int ID, string Ten)
        {
            this.ID = ID;
            this.Ten = Ten;
        }
    }

    public class ThongTinTaiSanParams
    {
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSanModelPartial { get; set; }
        //List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModelPartial, bool? BienDong
        public bool? BienDong { get; set; }
        public bool? LaBanTam { get; set; }
        public List<int> ListIDDelete { get; set; }
        public int KeKhaiID { get; set; }
    }

    public class BanKeKhaiModel
    {
        public List<ThongTinTaiSanModelPartial> DanhSachThongTinTaiSan { get; set; }
        public List<FileDinhKemModel> DanhSachFileDinhKem { get; set; }
        public List<FileDinhKemModel> DanhSachFileDuyetDinhKem { get; set; }
        public BanKeKhaiModel()
        {

        }
    }
    public class CheckKeKhaiTaiSan
    {
        public bool KeKhai { get; set; } = false;
        public bool ThemKeKhai { get; set; } = false;
        public int LoaiDotKeKhaiID { get; set; }
        public string TenDotLoaiKeKhai { get; set; }
        public int TrangThaiBanKeKhai { get; set; } = 0;
        public int KeKhaiID { get; set; }
    }
}
