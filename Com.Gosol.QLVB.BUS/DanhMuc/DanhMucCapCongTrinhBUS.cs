using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.DanhMuc
{
    public interface IDanhMucCapCongTrinhBUS
    {
        List<DanhMucCapCongTrinhModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucCapCongTrinhModel DanhMucCapCongTrinh);
        public BaseResultModel Update(DanhMucCapCongTrinhModel DanhMucCapCongTrinh);
        public List<string> Delete(List<int> ListCapCongTRinhID);
        public DanhMucCapCongTrinhModel GetByID(int CapCongTrinhID);
    }
    public class DanhMucCapCongTrinhBUS : IDanhMucCapCongTrinhBUS
    {
        private IDanhMucCapCongTrinhDAL _DanhMucCapCongTrinhDAL;
        public DanhMucCapCongTrinhBUS(IDanhMucCapCongTrinhDAL DanhMucCapCongTrinhDAL)
        {
            _DanhMucCapCongTrinhDAL = DanhMucCapCongTrinhDAL;
        }
        public BaseResultModel Insert(DanhMucCapCongTrinhModel DanhMucCapCongTrinh)
        {
            return _DanhMucCapCongTrinhDAL.Insert(DanhMucCapCongTrinh);
        }

        public BaseResultModel Update(DanhMucCapCongTrinhModel DanhMucCapCongTrinh)
        {
            return _DanhMucCapCongTrinhDAL.Update(DanhMucCapCongTrinh);
        }
        public List<string> Delete(List<int> ListCapCongTRinhID)
        {
            return _DanhMucCapCongTrinhDAL.Delete(ListCapCongTRinhID);
        }

        public DanhMucCapCongTrinhModel GetByID(int CapCongTrinhID)
        {
            return _DanhMucCapCongTrinhDAL.GetByID(CapCongTrinhID);
        }

        public List<DanhMucCapCongTrinhModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucCapCongTrinhDAL.GetPagingBySearch(p, ref TotalRow);
        }


    }
}
