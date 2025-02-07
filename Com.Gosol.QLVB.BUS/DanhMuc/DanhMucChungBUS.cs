using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.DanhMuc
{
    public interface IDanhMucChungBUS
    {
        public List<DanhMucChungModel> GetPagingBySearch(BasePagingParams p, int LoaiDanhMuc, ref int TotalRow);
        public BaseResultModel Insert(DanhMucChungModel DanhMucChungModel);
        public BaseResultModel Update(DanhMucChungModel DanhMucChungModel);
        public BaseResultModel Delete(int ID);
        public DanhMucChungModel GetByID(int ID);
        public List<DanhMucChungModel> GetTruongByNam(int Nam);
        public List<DanhMucChungModel> GetAll(int LoaiDanhMuc, int Nam);
    }
    public class DanhMucChungBUS : IDanhMucChungBUS
    {
        private IDanhMucChungDAL _DanhMucChungDAL;
        public DanhMucChungBUS(IDanhMucChungDAL danhMucChungDAL)
        {
            _DanhMucChungDAL = danhMucChungDAL;
        }
        public List<DanhMucChungModel> GetPagingBySearch(BasePagingParams p, int LoaiDanhMuc, ref int TotalRow)
        {
            return _DanhMucChungDAL.GetPagingBySearch(p, LoaiDanhMuc, ref TotalRow);
        }
        public BaseResultModel Insert(DanhMucChungModel DanhMucChungModel)
        {
            return _DanhMucChungDAL.Insert(DanhMucChungModel);
        }
        public BaseResultModel Update(DanhMucChungModel DanhMucChungModel)
        {
            return _DanhMucChungDAL.Update(DanhMucChungModel);
        }
        public BaseResultModel Delete(int ID)
        {
            return _DanhMucChungDAL.Delete(ID);
        }
        public DanhMucChungModel GetByID(int ID)
        {
            return _DanhMucChungDAL.GetByID(ID);
        }
        public List<DanhMucChungModel> GetAll(int LoaiDanhMuc, int Nam)
        {
            return _DanhMucChungDAL.GetAll(LoaiDanhMuc, Nam);
        }
        public List<DanhMucChungModel> GetTruongByNam(int Nam)
        {
            return _DanhMucChungDAL.GetTruongByNam(Nam);
        }
    }
}
