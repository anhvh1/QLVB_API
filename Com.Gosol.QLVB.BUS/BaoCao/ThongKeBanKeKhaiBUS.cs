using Com.Gosol.QLVB.DAL.BaoCao;
using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.BaoCao;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.BaoCao
{
    public interface IThongKeBanKeKhaiBUS
    {
        public List<ThongKeBanKeKhaiModel> ThongKeBanKeKhai(ThongKeParams p, int CanBoID, int CoQuanID, int NguoiDungID);
        public List<DotKeKhaiModel> ListDotKeKhai(int? CanBoID, int? CoQuanIDCuaCanBoDangNhap, int? CoQuanFilter, int? NamKeKhai);
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_ID,ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan);
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSan(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_ID, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan);
        public List<HeThongCanBoShortModel> GetCanBoByCoQuanAndLoaiKeKhai(int? CoQuanID, int? NamKeKhai, int? CapQuanLy, int? LoaiKeKhai, int CoQuan_ID, int NguoiDungID);
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeByCoQuanID(int? CoQuanID, int? CapID, int? CapQuanLy, int? CoQuanFilTer, int? NamKeKhai, int? CoQuanCuaCanBoDangNhap, int NguoiDungID);
        public List<NhacViecModel> Dasboard_Notification(int? NguoiDungID, int? CanBoID, int? CoQuanID);
        public ThongKeGuiThanhTraTinhModel ThongKeGuiThanhTraTinh(int CanBoID, int CoQuanID, int NguoiDungID);
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart_New(int? Type, int? CoQuanID_Filter, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan);
    }
    public class ThongKeBanKeKhaiBUS : IThongKeBanKeKhaiBUS
    {
        private IThongKeBanKeKhaiDAL _ThongKeBanKeKhaiDAL;
        public ThongKeBanKeKhaiBUS(IThongKeBanKeKhaiDAL ThongKeBanKeKhaiDAL)
        {
            _ThongKeBanKeKhaiDAL = ThongKeBanKeKhaiDAL;
        }

        public List<ThongKeBanKeKhaiModel> ThongKeBanKeKhai(ThongKeParams p, int CanBoID, int CoQuanID, int NguoiDungID)
        {
            return _ThongKeBanKeKhaiDAL.ThongKeBanKeKhai(p, CanBoID, CoQuanID, NguoiDungID);
        }
        public List<DotKeKhaiModel> ListDotKeKhai(int? CanBoID, int? CoQuanIDCuaCanBoDangNhap, int? CoQuanFilter, int? NamKeKhai)
        {
            return _ThongKeBanKeKhaiDAL.ListDotKeKhai(CanBoID.Value, CoQuanIDCuaCanBoDangNhap.Value, CoQuanFilter.Value, NamKeKhai.Value);
        }
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_ID,ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan)
        {
            return _ThongKeBanKeKhaiDAL.ThongKeChiTietKeKhaiTaiSan_Chart(CoQuanID, CapQuanLy, NamKeKhai, NguoiDungID, CoQuan_ID, ref ListThongKeChiTietKeKhaiTaiSan);
        }
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSan(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_ID, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan)
        {
            return _ThongKeBanKeKhaiDAL.ThongKeChiTietKeKhaiTaiSan(CoQuanID, CapQuanLy, NamKeKhai, NguoiDungID, CoQuan_ID, ref ListThongKeChiTietKeKhaiTaiSan);
        }
        public List<HeThongCanBoShortModel> GetCanBoByCoQuanAndLoaiKeKhai(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? LoaiKeKhai, int CoQuan_ID, int NguoiDungID)
        {
            return _ThongKeBanKeKhaiDAL.GetCanBoByCoQuanAndLoaiKeKhai_New(CoQuanID, CapQuanLy, NamKeKhai, LoaiKeKhai, CoQuan_ID, NguoiDungID);
        }
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeByCoQuanID(int? CoQuanID,int? CapID,int? CapQuanLy,int? CoQuanFilTer,int? NamKeKhai, int? CoQuanCuaCanBoDangNhap, int NguoiDungID)
        {
            return _ThongKeBanKeKhaiDAL.ThongKeByCoQuanID(CoQuanID,CapID, CapQuanLy, CoQuanFilTer, NamKeKhai,  CoQuanCuaCanBoDangNhap,  NguoiDungID);
        }
        public List<NhacViecModel> Dasboard_Notification(int? NguoiDungID, int? CanBoID, int? CoQuanID)
        {
            return _ThongKeBanKeKhaiDAL.Dasboard_Notification(NguoiDungID,CanBoID,CoQuanID);
        }

        public ThongKeGuiThanhTraTinhModel ThongKeGuiThanhTraTinh(int CanBoID, int CoQuanID, int NguoiDungID)
        {
            return _ThongKeBanKeKhaiDAL.ThongKeGuiThanhTraTinh(CanBoID, CoQuanID, NguoiDungID);
        }

        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart_New(int? Type, int? CoQuanID_Filter, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan)
        {
            return _ThongKeBanKeKhaiDAL.ThongKeChiTietKeKhaiTaiSan_Chart_New_v2(Type, CoQuanID_Filter, CapQuanLy, NamKeKhai, NguoiDungID, CoQuan_CuaCanBoDangNhap, ref ListThongKeChiTietKeKhaiTaiSan);
        }
    }
}
