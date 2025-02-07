using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface ICongKhaiBanKeKhaiBUS
    {
        public List<CongKhaiBanKeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_And_DanhSachBanKeKhai_KeKhaiID(int KeKhaiID, int CanBoXemKeKhai);
    }
    public class CongKhaiBanKeKhaiBUS : ICongKhaiBanKeKhaiBUS
    {
        private ICongKhaiBanKeKhaiDAL _CongKhaiBanKeKhaiDAL;
        public CongKhaiBanKeKhaiBUS(ICongKhaiBanKeKhaiDAL CongKhaiBanKeKhaiDAL)
        {
            this._CongKhaiBanKeKhaiDAL = CongKhaiBanKeKhaiDAL;
        }

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_And_DanhSachBanKeKhai_KeKhaiID(int KeKhaiID, int CanBoXemKeKhai)
        {
            return _CongKhaiBanKeKhaiDAL.ChiTietThongTinKeKhai_And_DanhSachBanKeKhai_KeKhaiID(KeKhaiID, CanBoXemKeKhai);
        }

        public List<CongKhaiBanKeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow)
        {
            return _CongKhaiBanKeKhaiDAL.GetPagingBySearch(p, CanBoID, ref TotalRow);
        }
    }
}
