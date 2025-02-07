using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class NguoiDungModel
    {
        public int NguoiDungID { get; set; }
        public string TenNguoiDung { get; set; }
        public string MatKhau { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public int CanBoID { get; set; }
        public int CoQuanID { get; set; }
        public string TenCanBo { get; set; }
        //public int RoleID { get; set; }
        public string Email { get; set; }
        //public string DienThoai { get; set; }
        //public string RoleName { get; set; }
        public int GioiTinh { get; set; }
        public string Token { get; set; }
        public int CapCoQuan { get; set; }
        public string TenCoQuan { get; set; }
        public string TenCoQuanCha { get; set; }
        //public int VaiTro { get; set; }
        public DateTime? expires_at { get; set; }
        public string AnhHoSo { get; set; }
        //public int QuanLyThanNhan { get; set; }
        public DateTime? ThoiGianLogin { get; set; }
        public int? SoLanLogin { get; set; }
        public int? MapThongTin { get; set; }
        public NguoiDungModel() { }
        public NguoiDungModel(int NguoiDungID, string TenNguoiDung)
        {
            this.NguoiDungID = NguoiDungID;
            this.TenNguoiDung = TenNguoiDung;
        }
    }

}
