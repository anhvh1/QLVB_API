using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.DanhMuc
{
    public interface IDanhMucTrangThaiBUS
    {
        List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel);
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel);
        public BaseResultModel Delete(List<int> ListTrangThaiID);
        public DanhMucTrangThaiModel GetByID(int TrangThaiID);
    }
    public class DanhMucTrangThaiBUS : IDanhMucTrangThaiBUS
    {
        private IDanhMucTrangThaiDAL _DanhMucTrangThaiDAL;
        public DanhMucTrangThaiBUS(IDanhMucTrangThaiDAL DanhMucTrangThaiDAL)
        {
            _DanhMucTrangThaiDAL = DanhMucTrangThaiDAL;
        }
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            return _DanhMucTrangThaiDAL.Insert(DanhMucTrangThaiModel);
        }
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            return _DanhMucTrangThaiDAL.Update(DanhMucTrangThaiModel);
        }
        public BaseResultModel Delete(List<int> ListTrangThaiID)
        {
            return _DanhMucTrangThaiDAL.Delete(ListTrangThaiID);
        }
        public DanhMucTrangThaiModel GetByID(int TrangThaiID)
        {
            return _DanhMucTrangThaiDAL.GetByID(TrangThaiID);
        }     
        public List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucTrangThaiDAL.GetPagingBySearch(p, ref TotalRow);
        }
    }
}
