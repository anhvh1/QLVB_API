using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IDotKeKhaiBUS
    {
        public BaseResultModel Insert(DotKeKhaiPartial DotKeKhaiPartial);
        public BaseResultModel Delete(List<int> ListDotKeKhaiID);
        public List<DotKeKhaiPartial> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CoQuanID, int CanBoID);
        public DotKeKhaiModel GetByID(int DotKeKhaiID);
        public DotKeKhaiPartial GetByIDForEdit(int DotKeKhaiID);
        public BaseResultModel Insert_New(DotKeKhaiPartial DotKeKhaiPartial,int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID);
        public BaseResultModel Update_New(DotKeKhaiPartial DotKeKhaiPartial, int NguoiDungID, int CoQuanID);
        //public int GetDotKeKhaiFitForCanBo(int CanBoID);

    }
    public class DotKeKhaiBUS : IDotKeKhaiBUS
    {
        private IDotKeKhaiDAL _DotKeKhaiDAL;
        public DotKeKhaiBUS(IDotKeKhaiDAL dotKeKhaiDAL)
        {
            this._DotKeKhaiDAL = dotKeKhaiDAL;
        }
        public BaseResultModel Insert(DotKeKhaiPartial DotKeKhaiPartial)
        {
            return _DotKeKhaiDAL.Insert(DotKeKhaiPartial);
        }
        public BaseResultModel Delete(List<int> ListDotKeKhaiID)
        {
            return _DotKeKhaiDAL.Delete(ListDotKeKhaiID);
        }
        public List<DotKeKhaiPartial> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CoQuanID, int CanBoID)
        {
            return _DotKeKhaiDAL.GetPagingBySearch(p, ref TotalRow, CoQuanID,CanBoID);
        }
        public DotKeKhaiModel GetByID(int DotKeKhaiID)
        {
            return _DotKeKhaiDAL.GetByID(DotKeKhaiID);
        }

        public DotKeKhaiPartial GetByIDForEdit(int DotKeKhaiID)
        {
            return _DotKeKhaiDAL.GetByIDForEdit(DotKeKhaiID);
        }

        public BaseResultModel Insert_New(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID)
        {
            return _DotKeKhaiDAL.Insert(DotKeKhaiPartial,CapQuanLy, CoQuanID, CanBoID, NguoiDungID);
        }

        public BaseResultModel Update_New(DotKeKhaiPartial DotKeKhaiPartial, int NguoiDungID, int CoQuanID)
        {
            return _DotKeKhaiDAL.Update(DotKeKhaiPartial, NguoiDungID, CoQuanID);
        }
    }
}
