using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Gosol.QLVB.Models.QLVB;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace Com.Gosol.QLVB.BUS.QLVB
{
    public interface ICapNhatPhuLucChinhSuaBUS
    {
        public List<NamTotNghiepTree> GetBySearchNamTotNghiep(CapNhatPhuLucParams p);
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow);
        public BaseResultModel Insert(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID);
        public BaseResultModel Update(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID);
        public BaseResultModel Delete(int CapNhatPhuLucID);
        public CapNhatPhuLucChinhSuaModel GetByID(int CapNhatPhuLucID);
    }
    public class CapNhatPhuLucChinhSuaBUS : ICapNhatPhuLucChinhSuaBUS
    {
        private ICapNhatPhuLucChinhSuaDAL _CapNhatPhuLucChinhSuaDAL;
        public CapNhatPhuLucChinhSuaBUS(ICapNhatPhuLucChinhSuaDAL CapNhatPhuLucChinhSuaDAL)
        {
            _CapNhatPhuLucChinhSuaDAL = CapNhatPhuLucChinhSuaDAL;
        }
        public List<NamTotNghiepTree> GetBySearchNamTotNghiep(CapNhatPhuLucParams p)
        {
            return _CapNhatPhuLucChinhSuaDAL.GetBySearchNamTotNghiep(p);
        }
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow)
        {
            return _CapNhatPhuLucChinhSuaDAL.GetPagingBySearch(p, NamThi, Truong, ref TotalRow);
        }
        public BaseResultModel Insert(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID)
        {
            return _CapNhatPhuLucChinhSuaDAL.Insert(PhuLucChinhSua, CanBoID);
        }
        public BaseResultModel Update(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID)
        {
            return _CapNhatPhuLucChinhSuaDAL.Update(PhuLucChinhSua, CanBoID);
        }
        public BaseResultModel Delete(int CapNhatPhuLucID)
        {
            return _CapNhatPhuLucChinhSuaDAL.Delete(CapNhatPhuLucID);
        }
        public CapNhatPhuLucChinhSuaModel GetByID(int CapNhatPhuLucID)
        {
            return _CapNhatPhuLucChinhSuaDAL.GetByID(CapNhatPhuLucID);
        }
    }
}
