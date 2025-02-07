using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.BUS.HeThong
{
    public interface IChucNangBUS
    {
        public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
    }
    public class ChucNangBUS : IChucNangBUS
    {
        private IChucNangDAL _ChucNangDAL;
        public ChucNangBUS(IChucNangDAL ChucNangDAL)
        {
            _ChucNangDAL = ChucNangDAL;
        }
       
        public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _ChucNangDAL.GetPagingBySearch(p, ref TotalRow);
        }
    }
}