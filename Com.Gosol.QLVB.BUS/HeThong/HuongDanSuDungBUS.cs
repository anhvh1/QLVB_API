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
    public interface IHuongDanSuDungBUS
    {
        public List<HuongDanSuDungModel> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow);
        public HuongDanSuDungModel GetByID(int HuongDanSuDungID);
        public BaseResultModel Insert(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID);
        public BaseResultModel Update(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID);
        public BaseResultModel Delete(List<int> ListHuongDanSuDungID, int CanBoID);
        public HuongDanSuDungModel GetByMaChucNang(string MaChucNang);
    }
    public class HuongDanSuDungBUS : IHuongDanSuDungBUS
    {
        private IHuongDanSuDungDAL _HuongDanSuDungDAL;
        public HuongDanSuDungBUS(IHuongDanSuDungDAL HuongDanSuDungDAL)
        {
            _HuongDanSuDungDAL = HuongDanSuDungDAL;
        }

        public BaseResultModel Delete(List<int> ListHuongDanSuDungID, int CanBoID)
        {
            return _HuongDanSuDungDAL.Delete(ListHuongDanSuDungID, CanBoID);
        }

        public HuongDanSuDungModel GetByID(int HuongDanSuDungID)
        {
            return _HuongDanSuDungDAL.GetByID(HuongDanSuDungID);
        }

        public HuongDanSuDungModel GetByMaChucNang(string MaChucNang)
        {
            return _HuongDanSuDungDAL.GetByMaChucNang(MaChucNang);
        }

        public List<HuongDanSuDungModel> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow)
        {
            return _HuongDanSuDungDAL.GetPagingBySearch(p, ref TotalRow);
        }

        public BaseResultModel Insert(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID)
        {
            return _HuongDanSuDungDAL.Insert(HuongDanSuDungModel, CanBoID);
        }

        public BaseResultModel Update(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID)
        {
            return _HuongDanSuDungDAL.Update(HuongDanSuDungModel, CanBoID);
        }
    }
}