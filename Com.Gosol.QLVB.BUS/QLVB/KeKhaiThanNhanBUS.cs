using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IKeKhaiThanNhanBUS
    {

        public BaseResultModel Insert(KeKhaiThanNhanModel KeKhaiThanNhanModel);
        public BaseResultModel Update(KeKhaiThanNhanModel KeKhaiThanNhanModel);
        public BaseResultModel Delete(List<int> ListThanNhanID);
        public KeKhaiThanNhanModel GetByID(int ThanNhanID);
        //public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow);
        public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CanBoID, int CoQuanID, int NguoiDungID, int VaiTro);
        public BaseResultModel InsertAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel);
        public BaseResultModel UpdateAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel);
        public ThanNhanCanBoModel GetThanNhanCanBo_Byu_CanBoID(int CanBoID);
    }

    public class KeKhaiThanNhanBUS : IKeKhaiThanNhanBUS
    {
        private IKeKhaiThanNhanDAL _KeKhaiThanNhanDAL;
        public KeKhaiThanNhanBUS(IKeKhaiThanNhanDAL keKhaiThanNhanDAL)
        {
            this._KeKhaiThanNhanDAL = keKhaiThanNhanDAL;
        }
        public BaseResultModel Insert(KeKhaiThanNhanModel KeKhaiThanNhanModel)
        {
            return _KeKhaiThanNhanDAL.Insert(KeKhaiThanNhanModel);
        }
        public BaseResultModel Update(KeKhaiThanNhanModel KeKhaiThanNhanModel)
        {
            return _KeKhaiThanNhanDAL.Update(KeKhaiThanNhanModel);
        }
        public BaseResultModel Delete(List<int> ListThanNhanID)
        {
            return _KeKhaiThanNhanDAL.Delete(ListThanNhanID);
        }
        public KeKhaiThanNhanModel GetByID(int ThanNhanID)
        {
            return _KeKhaiThanNhanDAL.GetByID(ThanNhanID);
        }
        //public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow)
        //{
        //    return _KeKhaiThanNhanDAL.GetPagingBySearch(p,  ref TotalRow);
        //}

        public BaseResultModel InsertAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel)
        {
            return _KeKhaiThanNhanDAL.InsertAll(ListKeKhaiThanNhanModel);
        }

        public BaseResultModel UpdateAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel)
        {
            return _KeKhaiThanNhanDAL.UpdateAll(ListKeKhaiThanNhanModel);
        }

        public ThanNhanCanBoModel GetThanNhanCanBo_Byu_CanBoID(int CanBoID)
        {
            return _KeKhaiThanNhanDAL.GetThanNhanCanBo_By_CanBoID(CanBoID);
        }

        public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CanBoID, int CoQuanID, int NguoiDungID, int VaiTro)
        {
            return _KeKhaiThanNhanDAL.GetPagingBySearch(p, ref TotalRow, CanBoID, CoQuanID, NguoiDungID, VaiTro);
        }
    }
}
