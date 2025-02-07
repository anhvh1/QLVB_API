using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.DanhMuc
{
    public interface IDanhMucKhoaThiBUS
    {
        public List<DanhMucKhoaThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public BaseResultModel Insert(DanhMucKhoaThiModel DanhMucKhoaThiModel);
        public BaseResultModel Update(DanhMucKhoaThiModel DanhMucKhoaThiModel);
        public BaseResultModel Delete(int KhoaThiID);
        public DanhMucKhoaThiModel GetByID(int KhoaThiID);
    }
    public class DanhMucKhoaThiBUS : IDanhMucKhoaThiBUS
    {
        private IDanhMucKhoaThiDAL _DanhMucKhoaThiDAL;
        public DanhMucKhoaThiBUS(IDanhMucKhoaThiDAL danhMucKhoaThiDAL)
        {
            _DanhMucKhoaThiDAL = danhMucKhoaThiDAL;
        }
        public List<DanhMucKhoaThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucKhoaThiDAL.GetPagingBySearch(p, ref TotalRow);
        }
        public BaseResultModel Insert(DanhMucKhoaThiModel DanhMucKhoaThiModel)
        {
            return _DanhMucKhoaThiDAL.Insert(DanhMucKhoaThiModel);
        }
        public BaseResultModel Update(DanhMucKhoaThiModel DanhMucKhoaThiModel)
        {
            return _DanhMucKhoaThiDAL.Update(DanhMucKhoaThiModel);
        }
        public BaseResultModel Delete(int KhoaThiID)
        {
            return _DanhMucKhoaThiDAL.Delete(KhoaThiID);
        }
        public DanhMucKhoaThiModel GetByID(int KhoaThiID)
        {
            return _DanhMucKhoaThiDAL.GetByID(KhoaThiID);
        }
    }
}
