using System;
using System.Web;

namespace Com.Gosol.QLVB.Security
{
    public enum ChucNangEnum
    {
        HeThong = 0,

        HeThong_QuanLy_CanBo = 1,
        HeThong_QuanLy_NguoiDung = 2,
        HeThong_QuanLy_ChucNang = 3,
        HeThong_QuanLy_PhanQuyen = 4,
        HeThong_QuanLy_ThamSoHeThong = 5,
        HeThong_QuanTri_DuLieu = 6,
        HeThong_QuanLy_NhatKyHeThong = 7,
        HeThong_HuongDanSuDung = 8,

        DanhMuc_CoQuan = 9,
        DanhMuc_DiaGioiHanhChinh = 10,
        DanhMuc_Truong = 11,
        DanhMuc_DanToc = 12,
        DanhMuc_MonHoc = 13,
        DanhMuc_XepLoai = 14,
        DanhMuc_DienXetTotNghiep = 15,
        DanhMuc_NguoiDuyetKetQua = 16,

        DanhMuc_HoiDongThi = 17,
        DanhMuc_KhoaThi = 18,
        BangDiemThi = 19,

        DoiMatKhau = 20,
        QuenMatKhau = 21,
        Dashboard = 22,

        QuanTriHeThong = 23,
        DanhMuc = 24,
        QuanLyBangCap = 25,
        BaoCao_ThongKe = 26,
        TraCuuVanBang = 27,

        MauPhieu = 28,
        ThongKe = 29,

    }

};