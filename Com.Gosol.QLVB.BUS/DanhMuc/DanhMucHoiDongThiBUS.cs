using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.DanhMuc
{
    public interface IDanhMucHoiDongThiBUS
    {
        public List<DanhMucHoiDongThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public BaseResultModel Insert(DanhMucHoiDongThiModel DanhMucHoiDongThiModel);
        public BaseResultModel Update(DanhMucHoiDongThiModel DanhMucHoiDongThiModel);
        public BaseResultModel Delete(int HoiDongThiID);
        public DanhMucHoiDongThiModel GetByID(int HoiDongThiID);
        public List<DanhMucHoiDongThiModel> GetHoiDongThiByNam(int Nam);
    }
    public class DanhMucHoiDongThiBUS : IDanhMucHoiDongThiBUS
    {
        private IDanhMucHoiDongThiDAL _DanhMucHoiDongThiDAL;
        public DanhMucHoiDongThiBUS(IDanhMucHoiDongThiDAL danhMucHoiDongThiDAL)
        {
            _DanhMucHoiDongThiDAL = danhMucHoiDongThiDAL;
        }
        public List<DanhMucHoiDongThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucHoiDongThiDAL.GetPagingBySearch(p, ref TotalRow);
        }
        public BaseResultModel Insert(DanhMucHoiDongThiModel DanhMucHoiDongThiModel)
        {
            return _DanhMucHoiDongThiDAL.Insert(DanhMucHoiDongThiModel);
        }
        public BaseResultModel Update(DanhMucHoiDongThiModel DanhMucHoiDongThiModel)
        {
            return _DanhMucHoiDongThiDAL.Update(DanhMucHoiDongThiModel);
        }
        public BaseResultModel Delete(int HoiDongThiID)
        {
            return _DanhMucHoiDongThiDAL.Delete(HoiDongThiID);
        }
        public DanhMucHoiDongThiModel GetByID(int HoiDongThiID)
        {
            return _DanhMucHoiDongThiDAL.GetByID(HoiDongThiID);
        }
        public List<DanhMucHoiDongThiModel> GetHoiDongThiByNam(int Nam)
        {
            return _DanhMucHoiDongThiDAL.GetHoiDongThiByNam(Nam);
        }
    }
}
