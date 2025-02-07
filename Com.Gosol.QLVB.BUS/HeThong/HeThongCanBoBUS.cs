using Com.Gosol.QLVB.BUS.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.HeThong
{
    public interface IHeThongCanBoBUS
    {
        public string GenerationMaCanBo(int CoQuanID);
        public int Insert(HeThongCanBoModel HeThongCanBoModel, ref int CanBoID, ref string Message);
        public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message);
        public List<string> Delete(List<int> ListCanBoID);
        public HeThongCanBoModel GetCanBoByID(int CanBoID);
        //public List<HeThongCanBoModel> FilterByName(string TenCanBo, int IsStatus, int CoQuanID);
        public List<HeThongCanBoPartialModel> ReadExcelFile_Old(string FilePath, int? CoQuanID);
        public List<HeThongCanBoPartialModel> ReadExcelFile(string FilePath, int? CoQuanID, int? CanBoDangNhapID);
        public List<HeThongCanBoModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID);
        public int ImportToExel(string FilePath, int? CoQuanID);
        public List<HeThongCanBoShortModel> GetThanNhanByCanBoID(int CanBoID);
        public List<HeThongCanBoModel> GetAllCanBoByCoQuanID(int CoQuanID, int CoQuan_ID);
        public List<HeThongCanBoModel> GetAllByCoQuanID(int CoQuanID);
        //public BaseResultModel CheckMaMail(int Ma);
        public List<HeThongCanBoModel> GetAllInCoQuanCha(int CoQuanID);

        public ThongTinDonViModel HeThongCanBo_GetThongTinCoQuan(int CanBoID, int NguoiDungID);

        public List<HeThongCanBoModel> GetAllByListCoQuanID(List<int> CoQuanID);

        public List<HeThongCanBoModel> GetAll();
        public List<HeThongCanBoModel> GetAllCanBoByNguoiDung(BasePagingParams p, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID);
    }
    public class HeThongCanBoBUS : IHeThongCanBoBUS
    {
        private IDanhMucChucVuBUS _DanhMucChucVuBUS;
        public HeThongCanBoBUS(IDanhMucChucVuBUS DanhMucChucVuBUS)
        {
            this._DanhMucChucVuBUS = DanhMucChucVuBUS;
        }
        public string GenerationMaCanBo(int CoQuanID)
        {
            return new HeThongCanBoDAL().GenerationMaCanBo(CoQuanID);
        }
        //Insert
        public int Insert(HeThongCanBoModel HeThongCanBoModel, ref int CanBoID, ref string Message)
        {
            int val = 0;
            try
            {
                return new HeThongCanBoDAL().Insert(HeThongCanBoModel, ref CanBoID, ref Message);
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        //Update
        public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message)
        {
            int val = 0;
            try
            {
                //if (HeThongCanBoModel.ChucVuID != null)
                //{
                //    var ChucVu = _DanhMucChucVuBUS.GetByID(HeThongCanBoModel.ChucVuID);
                //    if (ChucVu == null || ChucVu.ChucVuID <= 0)
                //    {
                //        val = 2;
                //        return val;
                //    }
                //}
                val = new HeThongCanBoDAL().Update(HeThongCanBoModel, ref Message);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }

        // Delete
        public List<string> Delete(List<int> ListCanBoID)
        {
            List<string> dic = new List<string>();
            try
            {
                dic = new HeThongCanBoDAL().Delete(ListCanBoID);
                //return dic;
            }
            catch (Exception ex)
            {
                dic.Add(ex.Message);
                throw ex;
            }
            return dic;
        }

        // Get By id
        public HeThongCanBoModel GetCanBoByID(int CanBoID)
        {

            try
            {
                return new HeThongCanBoDAL().GetCanBoByID(CanBoID);

            }
            catch (Exception ex)
            {
                return new HeThongCanBoModel();
                throw ex;
            }
        }

        // Get list by paging and search
        public List<HeThongCanBoModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? CoQuanID, int? IsStatus, int CoQuan_ID, int NguoiDungID)
        {
            try
            {
                return new HeThongCanBoDAL().GetPagingBySearch(p, ref TotalRow, CoQuanID, IsStatus, CoQuan_ID, NguoiDungID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // Read Exel file
        public List<HeThongCanBoPartialModel> ReadExcelFile_Old(string FilePath, int? CoQuanID)
        {
            List<HeThongCanBoPartialModel> val = new List<HeThongCanBoPartialModel>();
            try
            {
                val = new HeThongCanBoDAL().ReadExcelFile_Old(FilePath, CoQuanID);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            };
        }
        //
        public int ImportToExel(string FilePath, int? CoQuanID)
        {
            return new HeThongCanBoDAL().ImportToExel(FilePath, CoQuanID);
        }

        public List<HeThongCanBoShortModel> GetThanNhanByCanBoID(int CanBoID)
        {
            return new HeThongCanBoDAL().GetThanNhanByCanBoID(CanBoID);
        }

        public List<HeThongCanBoModel> GetAllCanBoByCoQuanID(int CoQuanID, int CoQuan_ID)
        {
            return new HeThongCanBoDAL().GetAllCanBoByCoQuanID(CoQuanID, CoQuan_ID);
        }

        public List<HeThongCanBoModel> GetAllByCoQuanID(int CoQuanID)
        {
            return new HeThongCanBoDAL().GetAllByCoQuanID(CoQuanID);
        }

        public List<HeThongCanBoModel> GetAllInCoQuanCha(int CoQuanID)
        {
            return new HeThongCanBoDAL().GetAllInCoQuanCha(CoQuanID);
        }

        public ThongTinDonViModel HeThongCanBo_GetThongTinCoQuan(int CanBoID, int NguoiDungID)
        {
            return new HeThongCanBoDAL().HeThongCanBo_GetThongTinCoQuan(CanBoID, NguoiDungID);
        }

        public List<HeThongCanBoModel> GetAllByListCoQuanID(List<int> CoQuanID)
        {
            return new HeThongCanBoDAL().GetAllByListCoQuanID(CoQuanID);
        }

        public List<HeThongCanBoModel> GetAll()
        {
            return new HeThongCanBoDAL().GetAll();
        }
        public List<HeThongCanBoModel> GetAllCanBoByNguoiDung(BasePagingParams p, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID)
        {
            return new HeThongCanBoDAL().GetAllCanBoByNguoiDung(p, CoQuanID, TrangThaiID, CoQuan_ID, NguoiDungID);
        }

        public List<HeThongCanBoPartialModel> ReadExcelFile(string FilePath, int? CoQuanID, int? CanBoDangNhapID)
        {
            return new HeThongCanBoDAL().ReadExcelFile(FilePath, CoQuanID, CanBoDangNhapID);
        }
    }
}
