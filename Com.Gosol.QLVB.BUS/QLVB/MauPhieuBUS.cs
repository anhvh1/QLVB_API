using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.QLVB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.QLVB
{
    public interface IMauPhieuBUS
    {
        public BaseResultModel Insert(MauPhieuModel _mauPhieuModel);
        public BaseResultModel Update(MauPhieuModel _mauPhieuModel);
        public List<string> Delete(List<int> DanhSachMauPhieuID);
        public List<MauPhieuModel> GetListPaging(BasePagingParams p, ref int TotalRow);
        public MauPhieuModel GetByID(int? MauPhieuID);
        public MauPhieuModel GetChiTietByNam(int? Nam, string TenMauPhieu);
        public string ImportDataToExcel(string rootpath, int? MauPhieuID);
        public DuLieuDiemThiModel ReadFileExcel(string FilePath, ref string Mess, int? Nam);
        public DuLieuDiemThiModel ReadFileExcelUpdate(string FilePath, ref string Mess);
        public DuLieuDiemThiModel ReadFileExcel_AllPages(string FilePath, ref string Mess, int? NamTotNghiep);
        public List<MauPhieuModel> GetAll(ref int TotalRow);
        public List<MauPhieuModel> GetAll_VBNN(ref int TotalRow);
    }

    public class MauPhieuBUS : IMauPhieuBUS
    {
        private IMauPhieuDAL _mauPhieuDAL;
        public MauPhieuBUS(IMauPhieuDAL mauPhieuDAL)
        {
            this._mauPhieuDAL = mauPhieuDAL;
        }

        public List<string> Delete(List<int> DanhSachMauPhieuID)
        {
            return _mauPhieuDAL.Delete(DanhSachMauPhieuID);
        }

        public List<MauPhieuModel> GetAll(ref int TotalRow)
        {
            return _mauPhieuDAL.GetAll(ref TotalRow);
        }

        public List<MauPhieuModel> GetAll_VBNN(ref int TotalRow)
        {
            return _mauPhieuDAL.GetAll_VBNN(ref TotalRow);
        }

        public MauPhieuModel GetByID(int? MauPhieuID)
        {
            return _mauPhieuDAL.GetByID(MauPhieuID);
        }

        public MauPhieuModel GetChiTietByNam(int? Nam, string TenMauPhieu)
        {
            return _mauPhieuDAL.GetChiTietByNam(Nam, TenMauPhieu);
        }

        public List<MauPhieuModel> GetListPaging(BasePagingParams p, ref int TotalRow)
        {
            return _mauPhieuDAL.GetListPaging(p, ref TotalRow);
        }

        public string ImportDataToExcel(string rootpath, int? MauPhieuID)
        {
            return _mauPhieuDAL.ImportDataToExcel(rootpath, MauPhieuID);
        }

        public BaseResultModel Insert(MauPhieuModel _mauPhieuModel)
        {
            return _mauPhieuDAL.Insert(_mauPhieuModel);
        }

        public DuLieuDiemThiModel ReadFileExcel(string FilePath, ref string Mess, int? Nam)
        {
            return _mauPhieuDAL.ReadFileExcel(FilePath ,ref Mess, Nam);
        }

        public DuLieuDiemThiModel ReadFileExcelUpdate(string FilePath, ref string Mess)
        {
            return _mauPhieuDAL.ReadFileExcelUpdate(FilePath, ref Mess);

        } public DuLieuDiemThiModel ReadFileExcel_AllPages(string FilePath, ref string Mess, int? NamTotNghiep)
        {
            return _mauPhieuDAL.ReadFileExcel_AllPages(FilePath, ref Mess, NamTotNghiep);
        }

        public BaseResultModel Update(MauPhieuModel _mauPhieuModel)
        {
            return _mauPhieuDAL.Update(_mauPhieuModel);
        }
    }
}
