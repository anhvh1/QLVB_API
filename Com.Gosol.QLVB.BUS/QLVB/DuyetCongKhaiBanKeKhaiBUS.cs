using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IDuyetCongKhaiBanKeKhaiBUS
    {
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, int NguoiDungID, ref int TotalRow);
        public BaseResultModel DuyetCongKhaiBanKeKhai(DuyetCongKhaiBanKeKhaiPartial objInsert, int CanBoID, int CoQuanID);
        public ChiTietCongKhaiBanKeKhai ChieTietCongKhaiBanKeKhai(int KeKhaiID);
        public BaseResultModel ThemCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID);
        public BaseResultModel XoaCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objDelete, int CanBoID, int CoQuanID);
        public BaseResultModel CapNhatCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID);
        public BaseResultModel HuyDuyetCongKhaiBanKeKhai(int KeKhaiID);
        public BaseResultModel CapNhatTrangThaiNhacViec(int? DuyetBanKeKhaiID);
        public BaseResultModel CapNhatTrangThaiNhacViec_Multi(List<int> DanhSachDuyetKeKhaiID);
    }
    public class DuyetCongKhaiBanKeKhaiBUS : IDuyetCongKhaiBanKeKhaiBUS
    {
        private IDuyetCongKhaiBanKeKhaiDAL _DuyetCongKhaiBanKeKhaiDAL;
        public DuyetCongKhaiBanKeKhaiBUS(IDuyetCongKhaiBanKeKhaiDAL duyetKeKhaiCongKhaiDAL)
        {
            this._DuyetCongKhaiBanKeKhaiDAL = duyetKeKhaiCongKhaiDAL;
        }

        public BaseResultModel CapNhatCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.CapNhatCanBoXemBanKeKhai(objInsert, CanBoID, CoQuanID);
        }

        public ChiTietCongKhaiBanKeKhai ChieTietCongKhaiBanKeKhai(int KeKhaiID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.ChieTietCongKhaiBanKeKhai(KeKhaiID);
        }

        public BaseResultModel DuyetCongKhaiBanKeKhai(DuyetCongKhaiBanKeKhaiPartial objInsert, int CanBoID, int CoQuanID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.DuyetCongKhaiBanKeKhai(objInsert, CanBoID, CoQuanID);
        }

        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, int NguoiDungID, ref int TotalRow)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.GetPagingBySearch(p, CoQuanID, CanBoID,  NguoiDungID, ref TotalRow);
        }

        public BaseResultModel HuyDuyetCongKhaiBanKeKhai(int KeKhaiID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.HuyDuyetCongKhaiBanKeKhai(KeKhaiID);
        }

        public BaseResultModel ThemCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.ThemCanBoXemBanKeKhai(objInsert, CanBoID, CoQuanID);
        }

        public BaseResultModel XoaCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objDelete, int CanBoID, int CoQuanID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.XoaCanBoXemBanKeKhai(objDelete, CanBoID, CoQuanID);
        }
        public BaseResultModel CapNhatTrangThaiNhacViec(int? DuyetBanKeKhaiID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.CapNhatTrangThaiNhacViec(DuyetBanKeKhaiID);
        }
        public BaseResultModel CapNhatTrangThaiNhacViec_Multi(List<int> DanhSachDuyetKeKhaiID)
        {
            return _DuyetCongKhaiBanKeKhaiDAL.CapNhatTrangThaiNhacViec_Multi(DanhSachDuyetKeKhaiID);
        }
    }
}
