using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class KeKhaiThanNhanModel
    {
        public int ThanNhanID { get; set; }
        public int? CanBoID { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string HoKhauThuongTru { get; set; }
        public string ChoOHienNay { get; set; }
        public string ChucVu { get; set; }
        public string NoiCongTac { get; set; }
        public int? QuanHe { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenQuanHe { get; set; }
        public int? TrangThai { get; set; }
        public string CMND { get; set; }
        public DateTime? NgayCap { get; set; }
        public string NoiCap { get; set; }
        public KeKhaiThanNhanModel()
        {

        }
        public KeKhaiThanNhanModel(int ThanNhanID, int CanBoID, string HoTen, int NamSinh, string HoKhauThuongTru, string ChoOHienNay,
            string ChucVu, string NoiCongTac, int QuanHe)
        {
            this.ThanNhanID = ThanNhanID;
            this.CanBoID = CanBoID;
            this.HoTen = HoTen;
            this.NamSinh = NamSinh;
            this.HoKhauThuongTru = HoKhauThuongTru;
            this.ChoOHienNay = ChoOHienNay;
            this.ChucVu = ChucVu;
            this.NoiCongTac = NoiCongTac;
            this.QuanHe = QuanHe;
        }
    }
    public class KeKhaiThanNhanPartial_New : KeKhaiThanNhanModel
    {
        public string TenCanBo { get; set; }
        public KeKhaiThanNhanPartial_New()
        {

        }
        public KeKhaiThanNhanPartial_New(string TenCanBo)
        {
            this.TenCanBo = TenCanBo;
        }
    }
    public class KeKhaiThanNhanPartial : KeKhaiThanNhanModel
    {
        public string MaCB { get; set; }
        public string TenCanBo { get; set; }
        public int GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public int ChucVuID { get; set; }
        public string Email { get; set; }

        public string DienThoai { get; set; }
        public int PhongBanID { get; set; }
        public int CoQuanID { get; set; }
        public int RoleID { get; set; }
        public int IsStatus { get; set; }
        public string HoKhau { get; set; }
        public int KeKhaiID { get; set; }
        public int DotKeKhaiID { get; set; }
        public int NamKeKhai { get; set; }

        //public int NhomTaiSanID { get; set; }
        //public string TenTaiSan { get; set; }
        //public int DienTich { get; set; }
        //public double? GiaTri { get; set; }
        //public string GiayChungNhanQuyenSuDung { get; set; }
        //public int CapCongTrinhID { get; set; }
        //public string GiaiTrinhNguonGoc { get; set; }
        //public string ThongTinKhac { get; set; }
        public string TenCoQuan { get; set; }
        public int TinhID { get; set; }
        public int HuyenID { get; set; }
        public int XaID { get; set; }
        public List<KeKhaiThanNhanModel> ThanNhan { get; set; }
        //public List<ThongTinTaiSanModel> ThongTinTaiSan { get; set; }
        public KeKhaiThanNhanPartial()
        {

        }
        public KeKhaiThanNhanPartial(int CanBoID, string MaCB, string TenCanBo, int NamSinh, int GioiTinh, string DiaChi, int ChucVuID, string Email,
           string DienThoai, int PhongBanID, int CoQuanID, int RoleID, int IsStatus, string HoKhau, int KeKhaiID, int DotKeKhaiID, int NamKeKhai, int ThongTinTaiSanID,
             string TenCoQuan, int TinhID, int HuyenID, int XaID, List<KeKhaiThanNhanModel> ThanNhan)
        {
            this.CanBoID = CanBoID;
            this.MaCB = MaCB;
            this.TenCanBo = TenCanBo;
            this.NamSinh = NamSinh;
            this.GioiTinh = GioiTinh;
            this.DiaChi = DiaChi;
            this.ChucVuID = ChucVuID;
            this.Email = Email;
            this.DienThoai = DienThoai;
            this.PhongBanID = PhongBanID;
            this.CoQuanID = CoQuanID;
            this.RoleID = RoleID;
            this.IsStatus = IsStatus;
            this.HoKhau = HoKhau;
            this.KeKhaiID = KeKhaiID;
            this.DotKeKhaiID = DotKeKhaiID;
            this.NamKeKhai = NamKeKhai;
            this.TenCoQuan = TenCoQuan;
            this.TinhID = TinhID;
            this.HuyenID = HuyenID;
            this.XaID = XaID;
            this.ThanNhan = ThanNhan;
            //this.ThongTinTaiSan = ThongTinTaiSan;
        }

    }

    public class ThanNhanCanBoModel
    {
        public KeKhaiThanNhanModel VoChong { get; set; }
        public List<KeKhaiThanNhanModel> ConChuaThanhNien { get; set; }
        public ThanNhanCanBoModel() { }
    }
}
