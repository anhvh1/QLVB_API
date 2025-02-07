using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface ITongHopBanKeKhaiBUS
    {
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow);
        //public string ExportForwardFile(int FileDinhKemID);
        public List<BienDongTaiSanModelNew> BienDongTaiSan(int KeKhaiID, int CanBoID);
    }
    public class TongHopBanKeKhaiBUS : ITongHopBanKeKhaiBUS
    {
        private ITongHopBanKeKhaiDAL _ITongHopBanKeKhaiDAL;
        public TongHopBanKeKhaiBUS(ITongHopBanKeKhaiDAL tongHopBanKeKhaiDAL)
        {
            this._ITongHopBanKeKhaiDAL = tongHopBanKeKhaiDAL;
        }
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow)
        {
            return _ITongHopBanKeKhaiDAL.GetPagingBySearch(p, CoQuanID, CanBoID, ref TotalRow);
        }
        public List<BienDongTaiSanModelNew> BienDongTaiSan(int KeKhaiID, int CanBoID)
        {
            return _ITongHopBanKeKhaiDAL.BienDongTaiSan(KeKhaiID, CanBoID);
        }
        //public string ExportForwardFile(int FileDinhKemID)
        //{
        //    return _ITongHopBanKeKhaiDAL.ExportForwardFile(FileDinhKemID);
        //}
    }
}
