using Com.Gosol.QLVB.DAL.ThongKe;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.ThongKe
{
    public interface IThongKeBUS
    {
        public List<ChiTietDuLieuDiemThiModel> GetThongKe(BasePagingParams p, ref int TotalRow);
    }
    public class ThongKeBUS : IThongKeBUS
    {
        private IThongKeDAL _thongkeDAL;
        public ThongKeBUS(IThongKeDAL thongKeDAL)
        {
            this._thongkeDAL = thongKeDAL;
        }

        public List<ChiTietDuLieuDiemThiModel> GetThongKe(BasePagingParams p, ref int TotalRow)
        {
            return _thongkeDAL.GetThongKe(p, ref TotalRow);
        }
    }
}
