using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.FileDinhKem
{
    public interface IFileDinhKemBUS
    {
        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel);
        public List<FileDinhKemModel> GetListFileDinhKemByKeKhaiID(int KeKhaiID);
        public FileDinhKemModel GetByID(int FileDinhKemID);
        public List<FileDinhKemModel> GetByNghiepVuID(int NghiepVuID, int Type);
        public BaseResultModel Delete(List<int> ListFileDinhKemID);
        public BaseResultModel Delete(List<FileDinhKemModel> ListFileDinhKem);
        public List<FileDinhKemModelPar> GetHistoryFileDinhKem(int KeKhaiID);
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByKeKhaiID(int KeKhaiID);
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByNghiepVuID_AndType(int NghiepVuID, int Type);
        public BaseResultModel CopyFile(string Url, string UrlNew);
        public int GetLastFileDinhKemID();
        public BaseResultModel Insert_NhieuBanKeKhaiIDCungFile(FileDinhKemModel FileDinhKemModel);
    }
    public class FileDinhKemBUS : IFileDinhKemBUS
    {
        private IFileDinhKemDAL _FileDinhKemDAL;

        public FileDinhKemBUS(IFileDinhKemDAL FileDinhKemDAL)
        {
            this._FileDinhKemDAL = FileDinhKemDAL;
        }

        public BaseResultModel Delete(List<int> ListFileDinhKemID)
        {
            return _FileDinhKemDAL.Delete(ListFileDinhKemID);
        }
        public BaseResultModel Delete(List<FileDinhKemModel> ListFileDinhKem)
        {
            return _FileDinhKemDAL.Delete(ListFileDinhKem);
        }
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByKeKhaiID(int KeKhaiID)
        {
            return _FileDinhKemDAL.GetAllField_FileDinhKem_ByKeKhaiID(KeKhaiID);
        }

        public FileDinhKemModel GetByID(int FileDinhKemID)
        {
            return _FileDinhKemDAL.GetByID(FileDinhKemID);
        }

        public List<FileDinhKemModel> GetListFileDinhKemByKeKhaiID(int KeKhaiID)
        {
            return _FileDinhKemDAL.GetListFileDinhKemByKeKhaiID(KeKhaiID);
        }

        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel)
        {
            return _FileDinhKemDAL.Insert(FileDinhKemModel);
        }
        public List<FileDinhKemModelPar> GetHistoryFileDinhKem(int KeKhaiID)
        {
            return _FileDinhKemDAL.GetHistoryFileDinhKem(KeKhaiID);
        }
        public BaseResultModel CopyFile(string Url, string UrlNew)
        {
            return _FileDinhKemDAL.CopyFile(Url, UrlNew);
        }
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByNghiepVuID_AndType(int NghiepVuID, int Type)
        {
            return _FileDinhKemDAL.GetAllField_FileDinhKem_ByNghiepVuID_AndType(NghiepVuID, Type);
        }

        public int GetLastFileDinhKemID()
        {
            return _FileDinhKemDAL.GetLastFileDinhKemID();
        }

        public BaseResultModel Insert_NhieuBanKeKhaiIDCungFile(FileDinhKemModel FileDinhKemModel)
        {
            return _FileDinhKemDAL.Insert_NhieuBanKeKhaiIDCungFile(FileDinhKemModel);
        }

        public List<FileDinhKemModel> GetByNghiepVuID(int NghiepVuID, int Type)
        {
            return _FileDinhKemDAL.GetByNghiepVuID(NghiepVuID, Type);
        }
    }
}
