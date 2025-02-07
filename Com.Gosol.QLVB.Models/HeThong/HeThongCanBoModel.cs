using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class HeThongCanBoModel
    {
        [Required]
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinh { get; set; }
        //public string DiaChi { get; set; }
        //public int? ChucVuID { get; set; }
        //public int? QuyenKy { get; set; }
        public string Email { get; set; }
        //public string DienThoai { get; set; }
        //public int? PhongBanID { get; set; }
        public int? CoQuanID { get; set; }
        //public int? RoleID { get; set; }
        //public int? QuanTridonVi { get; set; }
        //public int? CoQuanCuID { get; set; }
        //public int? CanBoCuID { get; set; }
        //public int? XemTaiLieuMat { get; set; }
        public string AnhHoSo { get; set; }
        public int? IsStatus { get; set; }
        //public string HoKhau { get; set; }
        public string MaCB { get; set; }
        //public int ThanNhanID { get; set; }
        //public string HoTenThanNhan { get; set; }
        //public string ChucVuStr { get; set; }
        //public int? CapQuanLy { get; set; }
        //public List<int> DanhSachChucVuID { get; set; }
        //public List<string> DanhSachTenChucVu { get; set; }
        public int? TrangThaiID { get; set; }
        public int? TrangThaiTaiKhoan { get; set; }
        public int? NguoiDungID { get; set; }
        public string TenCoQuan { get; set; }
        public string TenCoQuanCha { get; set; }
        public string TenNguoiDung { get; set; }
        public HeThongCanBoModel() { }
        public HeThongCanBoModel(int CanBoID, string TenCanBo, DateTime NgaySinh, int GioiTinh, string DiaChi, int ChucVuID, int QuyenKy, string Email, string DienThoai,
            int PhongBanID, int CoQuanID, int RoleID, int QuanTridonVi, int CoQuanCuID, int CanBoCuID, int XemTaiLieuMat, string AnhHoSo, string HoKhau,
            string MaCB, int TrangThaiID, int NguoiDungID)
        {
            this.CanBoID = CanBoID;
            this.TenCanBo = TenCanBo;
            this.NgaySinh = NgaySinh;
            this.GioiTinh = GioiTinh;
            //this.DiaChi = DiaChi;
            this.Email = Email;
            //this.DienThoai = DienThoai;
            this.CoQuanID = CoQuanID;
            //this.IsStatus = IsStatus;
            this.AnhHoSo = AnhHoSo;
            this.MaCB = MaCB;
            this.TrangThaiID = TrangThaiID;
            this.NguoiDungID = NguoiDungID;
        }


    }
    public class HeThongCanBoPartialModel : HeThongCanBoModel
    {
        public string TenChucVu { get; set; }
        
        public string TenTrangThai { get; set; }
        public string TenCapQuanLy { get; set; }
        public List<string> NguyenNhan { get; set; }
    }
    public class HeThongCanBoShortModel : HeThongCanBoModel
    {
        public int CanBoID { get; set; }
        public int ThanNhanID { get; set; }
        public string TenCanBo { get; set; }
        public string HoTenThanNhan { get; set; }
        public string TenDotKeKhai { get; set; }
        public HeThongCanBoShortModel() { }
    }
    public class CanBoChuVu
    {
        public int CanBoID { get; set; }
        public int ChucVuID { get; set; }
        public bool KeKhaiHangNam { get; set; }
        public int CapQuanLy { get; set; }

        public int TrangThaiID { get; set; }
        public int CoQuanID { get; set; }
    }
    public class Files
    {
        public string files { get; set; }
        public Files()
        {


        }
        public Files(string files)
        {
            this.files = files;
        }
    }

    public class ThongTinDonViModel
    {
        public string TenCanBo { get; set; }
        public string TenCoQuan { get; set; }
        public string TenCoQuanCha { get; set; }
        public int CanBoID { get; set; }
        public int CoQuanID { get; set; }
        public int CoQuanChaID { get; set; }
    }

    public class ThongTinCanBoModel
    {
        public HeThongCanBoModel ThongTinCanBo { get; set; }
        //public ThanNhanCanBoModel ThongTinThanNhan { get; set; }
    }
}
