using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IKeKhaiTaiSanBUS
    {
        public int Insert(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message);
        public int Update(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message);
     
        public KeKhaiTaiSanModel GetByID(int KeKhaiID);
        public List<KeKhaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? NamKeKhai);
        public string Export_Exel(string PathFile);
    }
    public class KeKhaiTaiSanBUS : IKeKhaiTaiSanBUS
    {
        private IKeKhaiTaiSanDAL _KeKhaiTaiSanDAL;
        public KeKhaiTaiSanBUS(IKeKhaiTaiSanDAL keKhaiDAL)
        {
            this._KeKhaiTaiSanDAL = keKhaiDAL;
        }
        public string Export_Exel(string FilePath)
        {
            return _KeKhaiTaiSanDAL.Export_Exel(FilePath);
        }
        public int Insert(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message)
        {
            return _KeKhaiTaiSanDAL.Insert(KeKhaiTaiSanModel, ref Message);
        }
        public int Update(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message)
        {
            return _KeKhaiTaiSanDAL.Update(KeKhaiTaiSanModel, ref Message);
        }
        
        public KeKhaiTaiSanModel GetByID(int KeKhaiID)
        {
            return _KeKhaiTaiSanDAL.GetByID(KeKhaiID);
        }
        //public List<KeKhaiModel> GetAll()
        //{
        //    return _KeKhaiTaiSanDAL.GetAll();
        //}
        public List<KeKhaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? NamKeKhai)
        {
            return _KeKhaiTaiSanDAL.GetPagingBySearch(p, ref TotalRow, NamKeKhai);
        }
    }
}
