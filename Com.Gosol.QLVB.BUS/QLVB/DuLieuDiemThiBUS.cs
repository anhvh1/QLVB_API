using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.DanhMuc
{
    public interface IDuLieuDiemThiBUS
    {
        public List<ThongTinThiSinh> GetThiSinh(BasePagingParams p, ref int TotalRow);
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, ref int TotalRow);
        public List<ThongTinToChucThi> GetPagingBySearch_New(BasePagingParams p, ref int TotalRow, int CanBoID);
        public List<ThongTinToChucThi> GetPagingBySearch_NNC(BasePagingParams p, ref int TotalRow, int CanBoID);
        public BaseResultModel Insert(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel InsertForImportExcel(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel InsertForImportExcelAllPages(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel Update(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh ThongTinThiSinh, int? CanBoID);
        public DuLieuDiemThiModel GetByID(int KyThiID);
        public DuLieuDiemThiModel GetThongTinKyThi(int KyThiID);
        public BaseResultModel DeleteDuLieuTep(int KyThiID);
        public List<ThongTinToChucThi> GetPagingBySearch_NhapDuLieuDiem(BasePagingParams p, int HoiDongID, int KhoaThiID, ref int TotalRow);
        public BaseResultModel Update_TrangThai(List<ThongTinToChucThi> ListThongTinToChucThi);
        public BaseResultModel Update_TrangThaiKhoa(List<ThongTinToChucThi> ListThongTinToChucThi);
        public BaseResultModel Update_TrangThaiTrang(List<ThongTinToChucThi> ListThongTinToChucThi);
        public BaseResultModel Update_TrangThaiDiemThi(List<ThongTinThiSinh> ListThongTinThiSinh);
        public DuLieuDiemThiModel CheckTrungSoQuyenSoTrang(int? SoTrang, string SoQuyen, string TenHoiDongThi, DateTime? KhoaThiNgay);
        public List<ThiSinhLogModel> GetThiSinhLog(int KyThiID);
        public List<CacTruongSua> GetChiTietThiSinhLog(int ThiSinhID, DateTime? NgayChinhSua);
        public List<NamThiTree> GetBySearchNamThi(DuLieuDiemThiParams p, int CanBoID);
        public List<NamThiTree> GetBySearchNamThiVBNN(DuLieuDiemThiParams p);
        public ThongTinThiSinh CheckTrungCCCD(int? ThiSinhID, string CMND);
    }
    public class DuLieuDiemThiBUS : IDuLieuDiemThiBUS
    {
        private IDuLieuDiemThiDAL _DuLieuDiemThiDAL;
        public DuLieuDiemThiBUS(IDuLieuDiemThiDAL duLieuDiemThiDAL)
        {
            _DuLieuDiemThiDAL = duLieuDiemThiDAL;
        }
        public List<ThongTinThiSinh> GetThiSinh(BasePagingParams p, ref int TotalRow)
        {
            return _DuLieuDiemThiDAL.GetThiSinh(p, ref TotalRow);
        }
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, ref int TotalRow)
        {
            return _DuLieuDiemThiDAL.GetPagingBySearch(p, NamThi, ref TotalRow);
        }
        public List<ThongTinToChucThi> GetPagingBySearch_New(BasePagingParams p, ref int TotalRow, int CanBoID)
        {
            return _DuLieuDiemThiDAL.GetPagingBySearch_New(p, ref TotalRow, CanBoID);
        }
        public List<ThongTinToChucThi> GetPagingBySearch_NNC(BasePagingParams p, ref int TotalRow, int CanBoID)
        {
            return _DuLieuDiemThiDAL.GetPagingBySearch_NNC(p, ref TotalRow, CanBoID);
        }
        public BaseResultModel Insert(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            return _DuLieuDiemThiDAL.Insert(DuLieuDiemThiModel,CanBoID);
        }
        public BaseResultModel Update(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            return _DuLieuDiemThiDAL.Update(DuLieuDiemThiModel, CanBoID);
        }  
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh ThongTinThiSinh, int? CanBoID)
        {
            var Result = _DuLieuDiemThiDAL.UpdateThongTinCapBang(ThongTinThiSinh, CanBoID);
            if (Result.Status == 1 && ThongTinThiSinh.ThiSinhMaped > 0)
            {
                ThongTinThiSinh.Maped = 1;
                List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
                list.Add(ThongTinThiSinh);
                new QuanLyThiSinhDAL().Update_TrangThai(list);
            }
            return Result;
        }
        public DuLieuDiemThiModel GetByID(int KyThiID)
        {
            return _DuLieuDiemThiDAL.GetByID(KyThiID);
        }
        public DuLieuDiemThiModel GetThongTinKyThi(int KyThiID)
        {
            return _DuLieuDiemThiDAL.GetThongTinKyThi(KyThiID);
        }
        public BaseResultModel DeleteDuLieuTep(int KyThiID)
        {
            return _DuLieuDiemThiDAL.DeleteDuLieuTep(KyThiID);
        }
        public List<ThongTinToChucThi> GetPagingBySearch_NhapDuLieuDiem(BasePagingParams p, int HoiDongID, int KhoaThiID, ref int TotalRow)
        {
            return _DuLieuDiemThiDAL.GetPagingBySearch_NhapDuLieuDiem(p, HoiDongID, KhoaThiID, ref TotalRow);
        }
        public BaseResultModel Update_TrangThai(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            return _DuLieuDiemThiDAL.Update_TrangThai(ListThongTinToChucThi);
        }
        public BaseResultModel Update_TrangThaiKhoa(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            return _DuLieuDiemThiDAL.Update_TrangThaiKhoa(ListThongTinToChucThi);
        }
        public BaseResultModel Update_TrangThaiTrang(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            return _DuLieuDiemThiDAL.Update_TrangThaiTrang(ListThongTinToChucThi);
        }
        public BaseResultModel Update_TrangThaiDiemThi(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            return _DuLieuDiemThiDAL.Update_TrangThaiDiemThi(ListThongTinThiSinh);
        }
        public DuLieuDiemThiModel CheckTrungSoQuyenSoTrang(int? SoTrang, string SoQuyen, string TenHoiDongThi, DateTime? KhoaThiNgay)
        {
            return _DuLieuDiemThiDAL.CheckTrungSoQuyenSoTrang(SoTrang, SoQuyen, TenHoiDongThi, KhoaThiNgay);
        }

        public BaseResultModel InsertForImportExcel(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            return _DuLieuDiemThiDAL.InsertForImportExcel(DuLieuDiemThiModel, CanBoID);
        }
        
        public BaseResultModel InsertForImportExcelAllPages(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            return _DuLieuDiemThiDAL.InsertForImportExcelAllPages(DuLieuDiemThiModel, CanBoID);
        }

        public List<ThiSinhLogModel> GetThiSinhLog(int KyThiID)
        {
            return _DuLieuDiemThiDAL.GetThiSinhLog(KyThiID);
        }

        public List<CacTruongSua> GetChiTietThiSinhLog(int ThiSinhID, DateTime? NgayChinhSua)
        {
            return _DuLieuDiemThiDAL.GetChiTietThiSinhLog(ThiSinhID, NgayChinhSua);
        }
        public List<NamThiTree> GetBySearchNamThi(DuLieuDiemThiParams p, int CanBoID)
        {
            return _DuLieuDiemThiDAL.GetBySearchNamThi(p, CanBoID);
        }
        public List<NamThiTree> GetBySearchNamThiVBNN(DuLieuDiemThiParams p)
        {
            return _DuLieuDiemThiDAL.GetBySearchNamThiVBNN(p);
        }
        public ThongTinThiSinh CheckTrungCCCD(int? ThiSinhID, string CMND)
        {
            return _DuLieuDiemThiDAL.CheckTrungCCCD(ThiSinhID, CMND);
        }
    }
}
