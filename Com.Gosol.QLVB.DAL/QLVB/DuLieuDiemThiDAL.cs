using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Ultilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Workflow;
using Workflow.DatabaseProxy;
using Workflow.Model;

namespace Com.Gosol.QLVB.DAL.DanhMuc
{
    public interface IDuLieuDiemThiDAL
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, ref int TotalRow);
        public List<ThongTinToChucThi> GetPagingBySearch_New(BasePagingParams p, ref int TotalRow, int CanBoID);
        public List<ThongTinToChucThi> GetPagingBySearch_NNC(BasePagingParams p, ref int TotalRow, int CanBoID);
        public BaseResultModel Insert(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel InsertForImportExcel(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel InsertForImportExcelAllPages(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel Update(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID);
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh item, int? CanBoID);
        public DuLieuDiemThiModel GetByID(int KyThiID);
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
        public DuLieuDiemThiModel GetThongTinKyThi(int KyThiID);
        public List<ThongTinThiSinh> GetThiSinh(BasePagingParams p, ref int TotalRow);
        public ThongTinThiSinh CheckTrungCCCD(int? ThiSinhID, string CMND);
    }
    public class DuLieuDiemThiDAL : IDuLieuDiemThiDAL
    {
        public List<ThongTinThiSinh> GetThiSinh(BasePagingParams p, ref int TotalRow)
        {
            List<ThongTinThiSinh> Result = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@NamThi",SqlDbType.Int),
                new SqlParameter("@HoiDongID",SqlDbType.Int),
                new SqlParameter("@Truong",SqlDbType.Int),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                new SqlParameter("@SoTrang",SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.Nam ?? Convert.DBNull;
            parameters[7].Value = p.HoiDongID ?? Convert.DBNull;
            parameters[8].Value = p.Truong ?? Convert.DBNull;
            parameters[9].Value = p.SoQuyen ?? Convert.DBNull;
            parameters[10].Value = p.SoTrang ?? Convert.DBNull;
            parameters[11].Value = p.TrangThai ?? Convert.DBNull;
           

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetThiSinh_New", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NgaySinhStr = Utils.ConvertToString(dr["NgaySinhStr"], string.Empty);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToInt32(dr["DanToc"], 0);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToInt32(dr["TruongTHPT"], 0);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        info.XepLoaiHanhKiem = Utils.ConvertToInt32(dr["XepLoaiHanhKiem"], 0);
                        info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToInt32(dr["XepLoaiHocLuc"], 0);
                        info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToDecimal(dr["DiemTBLop12"], 0);
                        info.DiemKK = Utils.ConvertToDecimal(dr["DiemKK"], 0);
                        info.DienXTN = Utils.ConvertToInt32(dr["DienXTN"], 0);
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.NamTotNghiep = Utils.ConvertToNullableInt32(dr["Nam"], null);
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        if ((info.SoHieuBang != "" && info.SoHieuBang.Length > 0) || (info.VaoSoCapBangSo != "" && info.VaoSoCapBangSo.Length > 0) 
                            || (info.Hang != "" && info.Hang.Length > 0) || (info.NgayCapBang != null))
                        {
                            info.TrangThaiCapBang = 1;
                        }
                        else info.TrangThaiCapBang = 0;

                        Result.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, ref int TotalRow)
        {
            List<ThongTinThiSinh> Result = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@NamThi",SqlDbType.Int),
                new SqlParameter("@Truong",SqlDbType.Int),
                new SqlParameter("@NgayCapBang",SqlDbType.DateTime),
                new SqlParameter("@SoHieuBang",SqlDbType.NVarChar),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = NamThi;
            parameters[7].Value = p.Truong;
            parameters[8].Value = p.NgayCapBang ?? Convert.DBNull;
            parameters[9].Value = p.SoHieuBang == null ? "" : p.Keyword.Trim();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetPagingBySearch_v2", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToInt32(dr["DanToc"], 0);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToInt32(dr["TruongTHPT"], 0);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        info.XepLoaiHanhKiem = Utils.ConvertToInt32(dr["XepLoaiHanhKiem"], 0);
                        info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToInt32(dr["XepLoaiHocLuc"], 0);
                        info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToDecimal(dr["DiemTBLop12"], 0);
                        info.DiemKK = Utils.ConvertToDecimal(dr["DiemKK"], 0);
                        info.DienXTN = Utils.ConvertToInt32(dr["DienXTN"], 0);
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);

                        Result.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }
        public List<ThongTinToChucThi> GetPagingBySearch_New(BasePagingParams p, ref int TotalRow, int CanBoID)
        {
            List<ThongTinToChucThi> Result = new List<ThongTinToChucThi>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@Truong",SqlDbType.Int),
                new SqlParameter("@TenHoiDongThi",SqlDbType.NVarChar),
                new SqlParameter("@Ngay",SqlDbType.DateTime),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                new SqlParameter("@SoTrang",SqlDbType.Int),
                new SqlParameter("@CanBoID",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.Nam ?? Convert.DBNull;
            parameters[7].Value = p.Truong ?? Convert.DBNull;
            parameters[8].Value = p.TenHoiDongThi ?? Convert.DBNull;
            parameters[9].Value = p.Ngay ?? Convert.DBNull;
            parameters[10].Value = p.SoQuyen == null ? "" : p.SoQuyen.Trim();
            parameters[11].Value = p.SoTrang ?? Convert.DBNull;
            parameters[12].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetPagingBySearch_New3", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinToChucThi info = new ThongTinToChucThi();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.HoiDongChamThiID = Utils.ConvertToInt32(dr["HoiDongChamThiID"], 0);
                        info.HoiDongGiamThiID = Utils.ConvertToInt32(dr["HoiDongGiamThiID"], 0);
                        info.HoiDongGiamKhaoID = Utils.ConvertToInt32(dr["HoiDongGiamKhaoID"], 0);
                        info.HoiDongCoiThiID = Utils.ConvertToInt32(dr["HoiDongCoiThiID"], 0);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.SBDDau = Utils.ConvertToString(dr["SBDDau"], string.Empty);
                        info.SBDCuoi = Utils.ConvertToString(dr["SBDCuoi"], string.Empty);
                        info.NguoiDocDiem = Utils.ConvertToString(dr["NguoiDocDiem"], string.Empty);
                        info.NguoiNhapVaInDiem = Utils.ConvertToString(dr["NguoiNhapVaInDiem"], string.Empty);
                        info.NguoiDocSoatBanGhi = Utils.ConvertToString(dr["NguoiDocSoatBanGhi"], string.Empty);
                        info.NgayDuyetBangDiem = Utils.ConvertToNullableDateTime(dr["NgayDuyetBangDiem"], null);
                        info.NgayDuyetCham = Utils.ConvertToNullableDateTime(dr["NgayDuyetCham"], null);
                        //info.NgaySoDuyet = Utils.ConvertToNullableDateTime(dr["NgaySoDuyet"], null);
                        info.CanBoXetDuyet = Utils.ConvertToString(dr["CanBoXetDuyet"], string.Empty);
                        info.CanBoSoKT = Utils.ConvertToString(dr["CanBoSoKT"], string.Empty);
                        info.ChuTichHoiDong = Utils.ConvertToString(dr["ChuTichHoiDong"], string.Empty);
                        info.GiamDocSo = Utils.ConvertToString(dr["GiamDocSo"], string.Empty);
                        info.SoThiSinhDuThi = Utils.ConvertToInt32(dr["SoThiSinhDuThi"], 0);
                        info.DuocCongNhanTotNghiep = Utils.ConvertToInt32(dr["DuocCongNhanTotNghiep"], 0);
                        info.KhongDuocCongNhanTotNghiep = Utils.ConvertToInt32(dr["KhongDuocCongNhanTotNghiep"], 0);
                        info.TNLoaiGioi = Utils.ConvertToInt32(dr["TNLoaiGioi"], 0);
                        info.TNLoaiKha = Utils.ConvertToInt32(dr["TNLoaiKha"], 0);
                        info.TNLoaiTB = Utils.ConvertToInt32(dr["TNLoaiTB"], 0);
                        info.DienTotNghiep2 = Utils.ConvertToInt32(dr["DienTotNghiep2"], 0);
                        info.DienTotNghiep3 = Utils.ConvertToInt32(dr["DienTotNghiep3"], 0);
                        info.TotNghiepDienA = Utils.ConvertToInt32(dr["TotNghiepDienA"], 0);
                        info.TotNghiepDienB = Utils.ConvertToInt32(dr["TotNghiepDienB"], 0);
                        info.DienTotNghiep4_5 = Utils.ConvertToInt32(dr["DienTotNghiep4_5"], 0);
                        info.DienTotNghiep4_75 = Utils.ConvertToInt32(dr["DienTotNghiep4_75"], 0);
                        info.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        info.TrangThaiKhoa = Utils.ConvertToInt32(dr["TrangThaiKhoa"], 0);
                        info.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        info.ThuKy = Utils.ConvertToString(dr["ThuKy"], string.Empty);
                        info.ChanhChuKhao = Utils.ConvertToString(dr["ChanhChuKhao"], string.Empty);
                        info.PhoChuKhao = Utils.ConvertToString(dr["PhoChuKhao"], string.Empty);
                        info.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToInt32(dr["SoTrang"], 0);
                        info.TongSoThiSinh = Utils.ConvertToInt32(dr["TongSoThiSinh"], 0);
                        info.ChuTichHoiDongChamThi = Utils.ConvertToString(dr["ChuTichHoiDongChamThi"], string.Empty);
                        info.ChuTichHoiDongCoiThi = Utils.ConvertToString(dr["ChuTichHoiDongCoiThi"], string.Empty);
                        info.TenNguoiNhap = Utils.ConvertToString(dr["TenNguoiNhap"], string.Empty);
                        info.Type = Utils.ConvertToNullableInt32(dr["type"], null);
                        Result.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }
        public List<ThongTinToChucThi> GetPagingBySearch_NNC(BasePagingParams p, ref int TotalRow, int CanBoID)
        {
            List<ThongTinToChucThi> Result = new List<ThongTinToChucThi>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@Truong",SqlDbType.Int),
                new SqlParameter("@TenHoiDongThi",SqlDbType.NVarChar),
                new SqlParameter("@Ngay",SqlDbType.DateTime),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                new SqlParameter("@SoTrang",SqlDbType.Int),
                new SqlParameter("@CanBoID",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.Nam ?? Convert.DBNull;
            parameters[7].Value = p.Truong ?? Convert.DBNull;
            parameters[8].Value = p.TenHoiDongThi ?? Convert.DBNull;
            parameters[9].Value = p.Ngay ?? Convert.DBNull;
            parameters[10].Value = p.SoQuyen == null ? "" : p.SoQuyen.Trim();
            parameters[11].Value = p.SoTrang ?? Convert.DBNull;
            parameters[12].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetPagingBySearch_New3", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinToChucThi info = new ThongTinToChucThi();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.HoiDongChamThiID = Utils.ConvertToInt32(dr["HoiDongChamThiID"], 0);
                        info.HoiDongGiamThiID = Utils.ConvertToInt32(dr["HoiDongGiamThiID"], 0);
                        info.HoiDongGiamKhaoID = Utils.ConvertToInt32(dr["HoiDongGiamKhaoID"], 0);
                        info.HoiDongCoiThiID = Utils.ConvertToInt32(dr["HoiDongCoiThiID"], 0);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.SBDDau = Utils.ConvertToString(dr["SBDDau"], string.Empty);
                        info.SBDCuoi = Utils.ConvertToString(dr["SBDCuoi"], string.Empty);
                        info.NguoiDocDiem = Utils.ConvertToString(dr["NguoiDocDiem"], string.Empty);
                        info.NguoiNhapVaInDiem = Utils.ConvertToString(dr["NguoiNhapVaInDiem"], string.Empty);
                        info.NguoiDocSoatBanGhi = Utils.ConvertToString(dr["NguoiDocSoatBanGhi"], string.Empty);
                        info.NgayDuyetBangDiem = Utils.ConvertToNullableDateTime(dr["NgayDuyetBangDiem"], null);
                        info.NgayDuyetCham = Utils.ConvertToNullableDateTime(dr["NgayDuyetCham"], null);
                        //info.NgaySoDuyet = Utils.ConvertToNullableDateTime(dr["NgaySoDuyet"], null);
                        info.CanBoXetDuyet = Utils.ConvertToString(dr["CanBoXetDuyet"], string.Empty);
                        info.CanBoSoKT = Utils.ConvertToString(dr["CanBoSoKT"], string.Empty);
                        info.ChuTichHoiDong = Utils.ConvertToString(dr["ChuTichHoiDong"], string.Empty);
                        info.GiamDocSo = Utils.ConvertToString(dr["GiamDocSo"], string.Empty);
                        info.SoThiSinhDuThi = Utils.ConvertToInt32(dr["SoThiSinhDuThi"], 0);
                        info.DuocCongNhanTotNghiep = Utils.ConvertToInt32(dr["DuocCongNhanTotNghiep"], 0);
                        info.KhongDuocCongNhanTotNghiep = Utils.ConvertToInt32(dr["KhongDuocCongNhanTotNghiep"], 0);
                        info.TNLoaiGioi = Utils.ConvertToInt32(dr["TNLoaiGioi"], 0);
                        info.TNLoaiKha = Utils.ConvertToInt32(dr["TNLoaiKha"], 0);
                        info.TNLoaiTB = Utils.ConvertToInt32(dr["TNLoaiTB"], 0);
                        info.DienTotNghiep2 = Utils.ConvertToInt32(dr["DienTotNghiep2"], 0);
                        info.DienTotNghiep3 = Utils.ConvertToInt32(dr["DienTotNghiep3"], 0);
                        info.TotNghiepDienA = Utils.ConvertToInt32(dr["TotNghiepDienA"], 0);
                        info.TotNghiepDienB = Utils.ConvertToInt32(dr["TotNghiepDienB"], 0);
                        info.DienTotNghiep4_5 = Utils.ConvertToInt32(dr["DienTotNghiep4_5"], 0);
                        info.DienTotNghiep4_75 = Utils.ConvertToInt32(dr["DienTotNghiep4_75"], 0);
                        info.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        info.TrangThaiKhoa = Utils.ConvertToInt32(dr["TrangThaiKhoa"], 0);
                        info.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        info.ThuKy = Utils.ConvertToString(dr["ThuKy"], string.Empty);
                        info.ChanhChuKhao = Utils.ConvertToString(dr["ChanhChuKhao"], string.Empty);
                        info.PhoChuKhao = Utils.ConvertToString(dr["PhoChuKhao"], string.Empty);
                        info.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToInt32(dr["SoTrang"], 0);
                        info.TongSoThiSinh = Utils.ConvertToInt32(dr["TongSoThiSinh"], 0);
                        info.ChuTichHoiDongChamThi = Utils.ConvertToString(dr["ChuTichHoiDongChamThi"], string.Empty);
                        info.ChuTichHoiDongCoiThi = Utils.ConvertToString(dr["ChuTichHoiDongCoiThi"], string.Empty);
                        info.TenNguoiNhap = Utils.ConvertToString(dr["TenNguoiNhap"], string.Empty);
                        info.Type = Utils.ConvertToNullableInt32(dr["type"], null);
                        Result.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }
        public List<NamThiTree> GetBySearchNamThi(DuLieuDiemThiParams p)
        {
            List<NamThiTree> Result = new List<NamThiTree>();
            List<ChiTietDuLieuDiemThiModel> Data = new List<ChiTietDuLieuDiemThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                //new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),                
              };
            //parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[0].Value = p.NamID ?? Convert.DBNull;
            parameters[1].Value = p.SoQuyenID == null ? "" : p.SoQuyenID.Trim();           

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NamThi_GetBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietDuLieuDiemThiModel info = new ChiTietDuLieuDiemThiModel();
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToInt32(dr["SoTrang"], 0);
                        info.TongSoThiSinh = Utils.ConvertToInt32(dr["TongSoThiSinh"], 0);
                        info.Type = Utils.ConvertToInt32(dr["type"], 1);
                        Data.Add(info);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (Data.Count > 0)
            {
                Result = (from item1 in Data
                          group item1 by item1.NamThi into temp1
                          select new NamThiTree()
                          {
                              NamThi = temp1.Key,
                              Name = temp1.Key.ToString(),
                              TongSoThiSinh = Data.Where(x => x.NamThi == temp1.Key).Sum(x => x.TongSoThiSinh),
                              children = (from item2 in temp1
                                          select new NamThiTree()
                                          {
                                              NamThi = item2.NamThi,
                                              Name = item2.SoQuyen,
                                              SoQuyen = item2.SoQuyen,
                                              SoTrang = item2.SoTrang,
                                              TongSoThiSinh = item2.TongSoThiSinh,
                                              Type = item2.Type,
                                          }).ToList()
                          }).OrderByDescending(x => x.NamThi).ToList();
            }

            return Result;
        }
        public List<NamThiTree> GetBySearchNamThi(DuLieuDiemThiParams p, int CanBoID)
        {
            List<NamThiTree> Result = new List<NamThiTree>();
            List<ChiTietDuLieuDiemThiModel> Data = new List<ChiTietDuLieuDiemThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                //new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                new SqlParameter("@CanBoID",SqlDbType.Int),
              };
            //parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[0].Value = p.NamID ?? Convert.DBNull;
            parameters[1].Value = p.SoQuyenID == null ? "" : p.SoQuyenID.Trim();
            parameters[2].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NamThi_GetBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietDuLieuDiemThiModel info = new ChiTietDuLieuDiemThiModel();
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToInt32(dr["SoTrang"], 0);
                        info.TongSoThiSinh = Utils.ConvertToInt32(dr["TongSoThiSinh"], 0);
                        info.Type = Utils.ConvertToInt32(dr["type"], 1);
                        Data.Add(info);
                    }
                    dr.Close();
                }  
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if(Data.Count > 0)
            {
                Result = (from item1 in Data
                          group item1 by item1.NamThi into temp1
                          select new NamThiTree()
                          {
                              NamThi = temp1.Key,
                              Name = temp1.Key.ToString(),
                              TongSoThiSinh = Data.Where(x => x.NamThi == temp1.Key).Sum(x => x.TongSoThiSinh),
                              children = (from item2 in temp1
                                                 select new NamThiTree()
                                                 {
                                                     NamThi = item2.NamThi,
                                                     Name = item2.SoQuyen,
                                                     SoQuyen = item2.SoQuyen,
                                                     SoTrang = item2.SoTrang,
                                                     TongSoThiSinh = item2.TongSoThiSinh,
                                                     Type = item2.Type,
                                                 }).ToList()
                          }).OrderByDescending(x => x.NamThi).ToList();
            }

            return Result;
        }
        public List<NamThiTree> GetBySearchNamThiVBNN(DuLieuDiemThiParams p)
        {
            List<NamThiTree> Result = new List<NamThiTree>();
            List<ChiTietDuLieuDiemThiModel> Data = new List<ChiTietDuLieuDiemThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                //new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
              };
            //parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[0].Value = p.NamID ?? Convert.DBNull;
            parameters[1].Value = p.SoQuyenID == null ? "" : p.SoQuyenID.Trim();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NamThi_VBNN_GetBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietDuLieuDiemThiModel info = new ChiTietDuLieuDiemThiModel();
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToInt32(dr["SoTrang"], 0);
                        info.TongSoThiSinh = Utils.ConvertToInt32(dr["TongSoThiSinh"], 0);
                        Data.Add(info);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (Data.Count > 0)
            {
                Result = (from item1 in Data
                          group item1 by item1.NamThi into temp1
                          select new NamThiTree()
                          {
                              NamThi = temp1.Key,
                              Name = temp1.Key.ToString(),
                              TongSoThiSinh = Data.Where(x => x.NamThi == temp1.Key).Sum(x => x.TongSoThiSinh),
                              children = (from item2 in temp1
                                          select new NamThiTree()
                                          {
                                              NamThi = item2.NamThi,
                                              Name = item2.SoQuyen,
                                              SoQuyen = item2.SoQuyen,
                                              SoTrang = item2.SoTrang,
                                              TongSoThiSinh = item2.TongSoThiSinh,
                                          }).ToList()
                          }).OrderByDescending(x => x.NamThi).ToList();
            }

            return Result;
        }
        public BaseResultModel Insert(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            var Result = new BaseResultModel();
            string workflowCode = "Duyet2Cap";
            var WorkflowID = new DataQuery().GetWorkflowIDByCode(workflowCode);
            var StateID = new DataQuery().GetFirstState(workflowCode);
            try
            {
                if (DuLieuDiemThiModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu điểm thi không được để trống";
                    return Result;
                }
                if (DuLieuDiemThiModel.ThongTinToChucThi == null)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin tổ chức thi không được để trống";
                    return Result;
                }
                else if (DuLieuDiemThiModel.ThongTinToChucThi.Nam == null || DuLieuDiemThiModel.ThongTinToChucThi.Nam == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin năm không được để trống";
                    return Result;
                }
                else
                {
                    if(DuLieuDiemThiModel.ThongTinToChucThi.MauPhieuID == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Mẫu phiếu không được để trống";
                        return Result;
                    }

                    if (DuLieuDiemThiModel.ThongTinToChucThi != null)
                    {
                        //var HoiDong = new DanhMucHoiDongThiDAL().GetByTenHoiDong(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim());
                        //if(DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID == 0)
                        if (DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Length > 0)
                        {
                            var HoiDong = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDong.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = HoiDong.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }

                        if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID == 0)
                        {
                            if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Length > 0)
                            {
                                var HoiDongChamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                                if (HoiDongChamThi.ID > 0)
                                {
                                    DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = HoiDongChamThi.ID;
                                }
                                else
                                {
                                    DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                    DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim();
                                    DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                    DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                    var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                    DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = Utils.ConvertToInt32(hd.Data, 0);
                                }
                            }
                        }

                        //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID == 0)
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Length > 0)
                        {
                            var HoiDongGiamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongGiamThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = HoiDongGiamThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }

                        //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID == 0)
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Length > 0)
                        {
                            var HoiDongGiamKhao = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongGiamKhao.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = HoiDongGiamKhao.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }

                        //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID == 0)
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Length > 0)
                        {
                            var HoiDongCoiThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongCoiThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = HoiDongCoiThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                    }

                    if(DuLieuDiemThiModel.ThongTinThiSinh != null)
                    {
                        foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                        {
                            if ((item.TruongTHPT == null || item.TruongTHPT == 0) && item.TenTruongTHPT != null)
                            {
                                var Truong = new DanhMucChungDAL().GetByName(item.TenTruongTHPT.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                                if (Truong.ID > 0)
                                {
                                    item.TruongTHPT = Truong.ID;
                                }
                                else
                                {
                                    DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                    DanhMucChungModel.Ten = item.TenTruongTHPT.Trim();
                                    DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                    DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                    var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                    item.TruongTHPT = Utils.ConvertToInt32(hd.Data, 0);
                                }
                            }
                        }
                    }

                    //check trung số trang, số quyển
                    var dl = CheckTrungSoQuyenSoTrangTheoNam(DuLieuDiemThiModel.ThongTinToChucThi.SoTrang, DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen, DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                    if (dl != null && dl.ThongTinToChucThi != null && dl.ThongTinToChucThi.KyThiID > 0 && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    {
                        var DuLieuDiemThiCu = GetByID(dl.ThongTinToChucThi.KyThiID);
                        if (DuLieuDiemThiCu.ThongTinThiSinh == null) DuLieuDiemThiCu.ThongTinThiSinh = new List<ThongTinThiSinh>();
                        DuLieuDiemThiCu.ThongTinThiSinh.AddRange(DuLieuDiemThiModel.ThongTinThiSinh);
                        Result = Update(DuLieuDiemThiCu, CanBoID);
                        return Result;
                    }

                    //var dl = CheckTrungSoQuyenSoTrang(DuLieuDiemThiModel.ThongTinToChucThi.SoTrang, DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen, DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi, DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay);
                    //if (dl != null && dl.ThongTinToChucThi != null && dl.ThongTinToChucThi.KyThiID > 0 && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    //{
                    //    if (dl.ThongTinThiSinh == null) dl.ThongTinThiSinh = new List<ThongTinThiSinh>();
                    //    dl.ThongTinThiSinh.AddRange(DuLieuDiemThiModel.ThongTinThiSinh);
                    //    Result = Update(dl, CanBoID);
                    //    return Result;
                    //}
                    //check trung số báo danh
                    //if (DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    //{
                    //    var listSBD = DuLieuDiemThiModel.ThongTinThiSinh.Where(x => x.SoBaoDanh != null && x.SoBaoDanh.Length > 0).Select(x => x.SoBaoDanh).ToList();
                    //    foreach (var sbd in listSBD)
                    //    {
                    //        var count = listSBD.Where(x => x == sbd).Count();
                    //        if (count > 1)
                    //        {
                    //            Result.Status = 0;
                    //            Result.Message = "Trùng số báo danh " + sbd;
                    //            return Result;
                    //        }
                    //    }
                    //}
                    #region parameters thông tin bảng ghi điểm
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("KyThiID", SqlDbType.Int),
                        new SqlParameter("TenKyThi", SqlDbType.NVarChar),
                        new SqlParameter("HoiDongThiID", SqlDbType.Int),
                        new SqlParameter("KhoaThiID", SqlDbType.Int),
                        new SqlParameter("SBDDau", SqlDbType.NVarChar),
                        new SqlParameter("SBDCuoi", SqlDbType.NVarChar),
                        new SqlParameter("NguoiDocDiem", SqlDbType.NVarChar),
                        new SqlParameter("NguoiNhapVaInDiem", SqlDbType.NVarChar),
                        new SqlParameter("NguoiDocSoatBanGhi", SqlDbType.NVarChar),
                        new SqlParameter("NgayDuyetBangDiem", SqlDbType.DateTime),
                        new SqlParameter("ChuTichHoiDong", SqlDbType.NVarChar),
                        new SqlParameter("SoThiSinhDuThi", SqlDbType.Int),
                        new SqlParameter("DuocCongNhanTotNghiep", SqlDbType.Int),
                        new SqlParameter("TNLoaiGioi", SqlDbType.Int),
                        new SqlParameter("TNLoaiKha", SqlDbType.Int),
                        new SqlParameter("TNLoaiTB", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep2", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep3", SqlDbType.Int),
                        new SqlParameter("CanBoXetDuyet", SqlDbType.NVarChar),
                        new SqlParameter("GiamDocSo", SqlDbType.NVarChar),
                        new SqlParameter("GhiChu", SqlDbType.NVarChar),
                        new SqlParameter("TrangThai", SqlDbType.Int),
                        new SqlParameter("HoiDongChamThiID", SqlDbType.Int),
                        new SqlParameter("HoiDongGiamThiID", SqlDbType.Int),
                        new SqlParameter("HoiDongGiamKhaoID", SqlDbType.Int),
                        new SqlParameter("HoiDongCoiThiID", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienA", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienB", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep4_5", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep4_75", SqlDbType.Int),
                        new SqlParameter("Ban", SqlDbType.NVarChar),
                        new SqlParameter("NgayDuyetCham", SqlDbType.DateTime),
                        new SqlParameter("CanBoSoKT", SqlDbType.NVarChar),
                        new SqlParameter("KhongDuocCongNhanTotNghiep", SqlDbType.Int),
                        new SqlParameter("MauPhieuID", SqlDbType.Int),
                        new SqlParameter("ThuKy", SqlDbType.NVarChar),
                        new SqlParameter("PhoChuKhao", SqlDbType.NVarChar),
                        new SqlParameter("ChanhChuKhao", SqlDbType.NVarChar),
                        new SqlParameter("KhoaThiNgay", SqlDbType.DateTime),
                        new SqlParameter("PhongThi", SqlDbType.NVarChar),
                        new SqlParameter("SoQuyen", SqlDbType.NVarChar),
                        new SqlParameter("SoTrang", SqlDbType.Int),
                        new SqlParameter("TongSoThiSinh", SqlDbType.Int),
                        new SqlParameter("NgaySoDuyet", SqlDbType.DateTime),
                        new SqlParameter("Tinh", SqlDbType.NVarChar),
                        new SqlParameter("ToTruongHoiPhach", SqlDbType.NVarChar),
                        new SqlParameter("ChuTichHoiDongCoiThi", SqlDbType.NVarChar),
                        new SqlParameter("HieuTruong", SqlDbType.NVarChar),
                        new SqlParameter("ChuTichHoiDongChamThi", SqlDbType.NVarChar),
                        new SqlParameter("Nam", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienC", SqlDbType.Int),
                        new SqlParameter("DiaDanh", SqlDbType.NVarChar),
                        new SqlParameter("GhiChuCuoiTrang", SqlDbType.NVarChar),
                        new SqlParameter("SBDDau_CuoiTrang", SqlDbType.NVarChar),
                        new SqlParameter("SBDCuoi_CuoiTrang", SqlDbType.NVarChar),
                         new SqlParameter("TSDoThang", SqlDbType.Int),
                         new SqlParameter("TSDoThem", SqlDbType.Int),
                         new SqlParameter("TSThiHong", SqlDbType.Int),
                         new SqlParameter("PGiamDoc", SqlDbType.NVarChar),
                         new SqlParameter("NguoiKiemTra", SqlDbType.NVarChar),

                        new SqlParameter("FOOT_RPDD", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_KTD", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_DTBDIS", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_THUKY", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_GIAMSAT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_DCNTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Dien45", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Dien475", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_Gioi", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_Kha", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_TB", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM1DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM15DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM2DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEMTREN2DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VANGMATKHITHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VIPHAMQUYCHETHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSDIENUUTIEN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCOCHUNGNHANNGHE", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NGUOIKIEMTRAHS", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SKĐCNTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_STSDT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SSTSDT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_TND_D", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_TND_E", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_LTHUONG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SLTHUONG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCONLIETSI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCACDIENKHAC", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NGUOILAPBANG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNLAPBANG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNHOIDONGCOITHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNCHAMTHIXTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VTVGDTX", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CTHDPHUCKHAO", SqlDbType.NVarChar),
                        new SqlParameter("HEAD_HDCL", SqlDbType.NVarChar),
                        new SqlParameter("HEAD_TRUONG", SqlDbType.NVarChar),

                        new SqlParameter("NguoiImportID", SqlDbType.Int),
                    };

                    parameters[0].Direction = ParameterDirection.Output;
                    parameters[0].Size = 8;
                    parameters[1].Value = DuLieuDiemThiModel.ThongTinToChucThi.TenKyThi ?? Convert.DBNull;
                    parameters[2].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID ?? Convert.DBNull;
                    parameters[3].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiID ?? Convert.DBNull;
                    parameters[4].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDDau ?? Convert.DBNull;
                    parameters[5].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDCuoi ?? Convert.DBNull;
                    parameters[6].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiDocDiem ?? Convert.DBNull;
                    parameters[7].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiNhapVaInDiem ?? Convert.DBNull;
                    parameters[8].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiDocSoatBanGhi ?? Convert.DBNull;
                    parameters[9].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgayDuyetBangDiem ?? Convert.DBNull;
                    parameters[10].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDong ?? Convert.DBNull;
                    parameters[11].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoThiSinhDuThi ?? Convert.DBNull;
                    parameters[12].Value = DuLieuDiemThiModel.ThongTinToChucThi.DuocCongNhanTotNghiep ?? Convert.DBNull;
                    parameters[13].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiGioi ?? Convert.DBNull;
                    parameters[14].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiKha ?? Convert.DBNull;
                    parameters[15].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiTB ?? Convert.DBNull;
                    parameters[16].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep2 ?? Convert.DBNull;
                    parameters[17].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep3 ?? Convert.DBNull;
                    parameters[18].Value = DuLieuDiemThiModel.ThongTinToChucThi.CanBoXetDuyet ?? Convert.DBNull;
                    parameters[19].Value = DuLieuDiemThiModel.ThongTinToChucThi.GiamDocSo ?? Convert.DBNull;
                    parameters[20].Value = DuLieuDiemThiModel.ThongTinToChucThi.GhiChu ?? Convert.DBNull;
                    parameters[21].Value = DuLieuDiemThiModel.ThongTinToChucThi.TrangThai ?? Convert.DBNull;
                    parameters[22].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID ?? Convert.DBNull;
                    parameters[23].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID ?? Convert.DBNull;
                    parameters[24].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID ?? Convert.DBNull;
                    parameters[25].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID ?? Convert.DBNull;
                    parameters[26].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienA ?? Convert.DBNull;
                    parameters[27].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienB ?? Convert.DBNull;
                    parameters[28].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep4_5 ?? Convert.DBNull;
                    parameters[29].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep4_75 ?? Convert.DBNull;
                    parameters[30].Value = DuLieuDiemThiModel.ThongTinToChucThi.Ban ?? Convert.DBNull;
                    parameters[31].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgayDuyetCham ?? Convert.DBNull;
                    parameters[32].Value = DuLieuDiemThiModel.ThongTinToChucThi.CanBoSoKT ?? Convert.DBNull;
                    parameters[33].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhongDuocCongNhanTotNghiep ?? Convert.DBNull;
                    parameters[34].Value = DuLieuDiemThiModel.ThongTinToChucThi.MauPhieuID ?? Convert.DBNull;
                    parameters[35].Value = DuLieuDiemThiModel.ThongTinToChucThi.ThuKy ?? Convert.DBNull;
                    parameters[36].Value = DuLieuDiemThiModel.ThongTinToChucThi.PhoChuKhao ?? Convert.DBNull;
                    parameters[37].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChanhChuKhao ?? Convert.DBNull;
                    parameters[38].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay ?? Convert.DBNull;
                    parameters[39].Value = DuLieuDiemThiModel.ThongTinToChucThi.PhongThi ?? Convert.DBNull;
                    parameters[40].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen ?? Convert.DBNull;
                    parameters[41].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoTrang ?? Convert.DBNull;
                    parameters[42].Value = DuLieuDiemThiModel.ThongTinThiSinh != null ? DuLieuDiemThiModel.ThongTinThiSinh.Count : 0;
                    parameters[43].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgaySoDuyet ?? Convert.DBNull;
                    parameters[44].Value = DuLieuDiemThiModel.ThongTinToChucThi.Tinh ?? Convert.DBNull;
                    parameters[45].Value = DuLieuDiemThiModel.ThongTinToChucThi.ToTruongHoiPhach ?? Convert.DBNull;
                    parameters[46].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDongCoiThi ?? Convert.DBNull;
                    parameters[47].Value = DuLieuDiemThiModel.ThongTinToChucThi.HieuTruong ?? Convert.DBNull;
                    parameters[48].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDongChamThi ?? Convert.DBNull;
                    parameters[49].Value = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? Convert.DBNull;
                    parameters[50].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienC ?? Convert.DBNull;
                    parameters[51].Value = DuLieuDiemThiModel.ThongTinToChucThi.DiaDanh ?? Convert.DBNull;   
                    parameters[52].Value = DuLieuDiemThiModel.ThongTinToChucThi.GhiChuCuoiTrang ?? Convert.DBNull;
                    parameters[53].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDDau_CuoiTrang ?? Convert.DBNull;
                    parameters[54].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDCuoi_CuoiTrang ?? Convert.DBNull;
                    parameters[55].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSDoThang ?? Convert.DBNull;
                    parameters[56].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSDoThem ?? Convert.DBNull;
                    parameters[57].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSThiHong ?? Convert.DBNull;
                    parameters[58].Value = DuLieuDiemThiModel.ThongTinToChucThi.PGiamDoc ?? Convert.DBNull;
                    parameters[59].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiKiemTra ?? Convert.DBNull;
                    parameters[60].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_RPDD ?? Convert.DBNull;
                    parameters[61].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_KTD ?? Convert.DBNull;
                    parameters[62].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_DTBDIS ?? Convert.DBNull;
                    parameters[63].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_THUKY ?? Convert.DBNull;
                    parameters[64].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_GIAMSAT ?? Convert.DBNull;
                    parameters[65].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_DCNTN ?? Convert.DBNull;
                    parameters[66].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Dien45 ?? Convert.DBNull;
                    parameters[67].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Dien475 ?? Convert.DBNull;
                    parameters[68].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_Gioi ?? Convert.DBNull;
                    parameters[69].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_Kha ?? Convert.DBNull;
                    parameters[70].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_TB ?? Convert.DBNull;
                    parameters[71].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM1DIEM ?? Convert.DBNull;
                    parameters[72].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM15DIEM ?? Convert.DBNull;
                    parameters[73].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM2DIEM ?? Convert.DBNull;
                    parameters[74].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEMTREN2DIEM ?? Convert.DBNull;
                    parameters[75].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VANGMATKHITHI ?? Convert.DBNull;
                    parameters[76].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VIPHAMQUYCHETHI ?? Convert.DBNull;
                    parameters[77].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSDIENUUTIEN ?? Convert.DBNull;
                    parameters[78].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCOCHUNGNHANNGHE ?? Convert.DBNull;
                    parameters[79].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NGUOIKIEMTRAHS ?? Convert.DBNull;
                    parameters[80].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SKĐCNTN ?? Convert.DBNull;
                    parameters[81].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_STSDT ?? Convert.DBNull;
                    parameters[82].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SSTSDT ?? Convert.DBNull;
                    parameters[83].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_TND_D ?? Convert.DBNull;
                    parameters[84].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_TND_E ?? Convert.DBNull;
                    parameters[85].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_LTHUONG ?? Convert.DBNull;
                    parameters[86].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SLTHUONG ?? Convert.DBNull;
                    parameters[87].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCONLIETSI ?? Convert.DBNull;
                    parameters[88].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCACDIENKHAC ?? Convert.DBNull;
                    parameters[89].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NGUOILAPBANG ?? Convert.DBNull;
                    parameters[90].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNLAPBANG ?? Convert.DBNull;
                    parameters[91].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNHOIDONGCOITHI ?? Convert.DBNull;
                    parameters[92].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNCHAMTHIXTN ?? Convert.DBNull;
                    parameters[93].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VTVGDTX ?? Convert.DBNull;
                    parameters[94].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CTHDPHUCKHAO ?? Convert.DBNull;
                    parameters[95].Value = DuLieuDiemThiModel.ThongTinToChucThi.HEAD_HDCL ?? Convert.DBNull;
                    parameters[96].Value = DuLieuDiemThiModel.ThongTinToChucThi.HEAD_TRUONG ?? Convert.DBNull;
                    parameters[97].Value = CanBoID ?? Convert.DBNull;

                    if (DuLieuDiemThiModel.ThongTinToChucThi.Nam == null && DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay != null)
                    {
                        parameters[49].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay.Value.Year;
                    }
                    if (DuLieuDiemThiModel.ChiTietMauPhieu != null && DuLieuDiemThiModel.ChiTietMauPhieu.MauPhieuID > 0)
                    {
                        parameters[34].Value = DuLieuDiemThiModel.ChiTietMauPhieu.MauPhieuID;
                    }
                    #endregion
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                //insert thông tin tổ chức thi
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_Insert_New2", parameters);
                                int KyThiID = Utils.ConvertToInt32(parameters[0].Value, 0);
                                if (KyThiID > 0 && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                                {
                                    foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                                    {
                                        if (item.ThiSinhID > 0)
                                        {
                                            #region update thí sinh thi
                                            SqlParameter[] parms_ts = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("KyThiID", SqlDbType.Int),
                                                new SqlParameter("HoTen", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinh", SqlDbType.DateTime),
                                                new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                                                new SqlParameter("GioiTinh", SqlDbType.Bit),
                                                new SqlParameter("DanToc", SqlDbType.Int),
                                                new SqlParameter("CMND", SqlDbType.NVarChar),
                                                new SqlParameter("SoBaoDanh", SqlDbType.NVarChar),
                                                new SqlParameter("SoDienThoai", SqlDbType.NVarChar),
                                                new SqlParameter("DiaChi", SqlDbType.NVarChar),
                                                new SqlParameter("Lop", SqlDbType.NVarChar),
                                                new SqlParameter("TruongTHPT", SqlDbType.Int),
                                                new SqlParameter("LoaiDuThi", SqlDbType.NVarChar),
                                                new SqlParameter("DonViDKDT", SqlDbType.NVarChar),
                                                new SqlParameter("XepLoaiHanhKiem", SqlDbType.Int),
                                                new SqlParameter("XepLoaiHocLuc", SqlDbType.Int),
                                                new SqlParameter("DiemTBLop12", SqlDbType.Decimal),
                                                new SqlParameter("DiemKK", SqlDbType.Decimal),
                                                new SqlParameter("DienXTN", SqlDbType.Int),
                                                new SqlParameter("HoiDongThi", SqlDbType.Int),
                                                new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal),
                                                new SqlParameter("KetQuaTotNghiep", SqlDbType.NVarChar),
                                                new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                                                new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                                                new SqlParameter("NamThi", SqlDbType.Int),
                                                new SqlParameter("Do", SqlDbType.NVarChar),
                                                new SqlParameter("DoThem", SqlDbType.NVarChar),
                                                new SqlParameter("Hong", SqlDbType.NVarChar),
                                                new SqlParameter("LaoDong", SqlDbType.NVarChar),
                                                new SqlParameter("VanHoa", SqlDbType.NVarChar),
                                                new SqlParameter("RLTT", SqlDbType.NVarChar),
                                                new SqlParameter("TongSoDiemThi", SqlDbType.Decimal),
                                                new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                                                new SqlParameter("DiemXL", SqlDbType.Decimal),
                                                new SqlParameter("DiemUT", SqlDbType.Decimal),
                                                new SqlParameter("GhiChu", SqlDbType.NVarChar),
                                                new SqlParameter("Hang", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBCacBaiThi", SqlDbType.Decimal),
                                                new SqlParameter("DienUuTien", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBC", SqlDbType.Decimal),
                                                new SqlParameter("QueQuan", SqlDbType.NVarChar),
                                                new SqlParameter("ChungNhanNghe", SqlDbType.NVarChar),
                                                new SqlParameter("DTConLietSi", SqlDbType.NVarChar),
                                                new SqlParameter("GiaiTDKT", SqlDbType.NVarChar),
                                                new SqlParameter("HoiDong", SqlDbType.NVarChar),
                                                new SqlParameter("MonKN", SqlDbType.NVarChar),
                                                new SqlParameter("TBCNMonKN", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiCu", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiMoi", SqlDbType.NVarChar),
                                                new SqlParameter("TongBQ", SqlDbType.NVarChar),
                                                new SqlParameter("BQA", SqlDbType.NVarChar),
                                                new SqlParameter("BQT", SqlDbType.NVarChar),
                                                new SqlParameter("DC", SqlDbType.NVarChar),
                                                new SqlParameter("Ban", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinhStr", SqlDbType.NVarChar),

                                                new SqlParameter("BODY_DAODUC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_RLEV", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIENKK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_PHONGTHI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIEMTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TDTCU", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSG", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSGK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHINN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHITH", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TONGDIEMMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQAMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQTMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_SOCAPGIAYCN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLHT", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_QUOCGIA", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinh_Int", SqlDbType.Int),
                                                new SqlParameter("NgaySinhFormat", SqlDbType.DateTime),
                                                new SqlParameter("HoTenFormat", SqlDbType.NVarChar), 
                                            };

                                            parms_ts[0].Value = item.ThiSinhID;
                                            parms_ts[1].Value = KyThiID;
                                            parms_ts[2].Value = item.HoTen ?? Convert.DBNull;
                                            parms_ts[3].Value = item.NgaySinh ?? Convert.DBNull;
                                            parms_ts[4].Value = item.NoiSinh ?? Convert.DBNull;
                                            parms_ts[5].Value = item.GioiTinh ?? Convert.DBNull;
                                            parms_ts[6].Value = item.DanToc ?? Convert.DBNull;
                                            parms_ts[7].Value = item.CMND ?? Convert.DBNull;
                                            parms_ts[8].Value = item.SoBaoDanh ?? Convert.DBNull;
                                            parms_ts[9].Value = item.SoDienThoai ?? Convert.DBNull;
                                            parms_ts[10].Value = item.DiaChi ?? Convert.DBNull;
                                            parms_ts[11].Value = item.Lop ?? Convert.DBNull;
                                            parms_ts[12].Value = item.TruongTHPT ?? Convert.DBNull;
                                            parms_ts[13].Value = item.LoaiDuThi ?? Convert.DBNull;
                                            parms_ts[14].Value = item.DonViDKDT ?? Convert.DBNull;
                                            parms_ts[15].Value = item.XepLoaiHanhKiem ?? Convert.DBNull;
                                            parms_ts[16].Value = item.XepLoaiHocLuc ?? Convert.DBNull;
                                            parms_ts[17].Value = item.DiemTBLop12 ?? Convert.DBNull;
                                            parms_ts[18].Value = item.DiemKK ?? Convert.DBNull;
                                            parms_ts[19].Value = item.DienXTN ?? Convert.DBNull;
                                            parms_ts[20].Value = item.HoiDongThi ?? Convert.DBNull;
                                            parms_ts[21].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                                            parms_ts[22].Value = item.KetQuaTotNghiep ?? Convert.DBNull;
                                            parms_ts[23].Value = item.SoHieuBang ?? Convert.DBNull;
                                            parms_ts[24].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                                            parms_ts[25].Value = item.NamThi ?? Convert.DBNull;
                                            parms_ts[26].Value = item.Do ?? Convert.DBNull;
                                            parms_ts[27].Value = item.DoThem ?? Convert.DBNull;
                                            parms_ts[28].Value = item.Hong ?? Convert.DBNull;
                                            parms_ts[29].Value = item.LaoDong ?? Convert.DBNull;
                                            parms_ts[30].Value = item.VanHoa ?? Convert.DBNull;
                                            parms_ts[31].Value = item.RLTT ?? Convert.DBNull;
                                            parms_ts[32].Value = item.TongSoDiemThi ?? Convert.DBNull;
                                            parms_ts[33].Value = item.NgayCapBang ?? Convert.DBNull;
                                            parms_ts[34].Value = item.DiemXL ?? Convert.DBNull;
                                            parms_ts[35].Value = item.DiemUT ?? Convert.DBNull;
                                            parms_ts[36].Value = item.GhiChu ?? Convert.DBNull;
                                            parms_ts[37].Value = item.Hang ?? Convert.DBNull;
                                            parms_ts[38].Value = item.DiemTBCacBaiThi ?? Convert.DBNull;
                                            parms_ts[39].Value = item.DienUuTien ?? Convert.DBNull;
                                            parms_ts[40].Value = item.DiemTBC ?? Convert.DBNull;
                                            parms_ts[41].Value = item.QueQuan ?? Convert.DBNull;
                                            parms_ts[42].Value = item.ChungNhanNghe ?? Convert.DBNull;
                                            parms_ts[43].Value = item.DTConLietSi ?? Convert.DBNull;
                                            parms_ts[44].Value = item.GiaiTDKT ?? Convert.DBNull;
                                            parms_ts[45].Value = item.HoiDong ?? Convert.DBNull;
                                            parms_ts[46].Value = item.MonKN ?? Convert.DBNull;
                                            parms_ts[47].Value = item.TBCNMonKN ?? Convert.DBNull;
                                            parms_ts[48].Value = item.DiemThiCu ?? Convert.DBNull;
                                            parms_ts[49].Value = item.DiemThiMoi ?? Convert.DBNull;
                                            parms_ts[50].Value = item.TongBQ ?? Convert.DBNull;
                                            parms_ts[51].Value = item.BQA ?? Convert.DBNull;
                                            parms_ts[52].Value = item.BQT ?? Convert.DBNull;
                                            parms_ts[53].Value = item.DC ?? Convert.DBNull;
                                            parms_ts[54].Value = item.Ban ?? Convert.DBNull;
                                            parms_ts[55].Value = item.NgaySinhStr ?? Convert.DBNull;

                                            parms_ts[56].Value = item.BODY_DAODUC ?? Convert.DBNull;
                                            parms_ts[57].Value = item.BODY_RLEV ?? Convert.DBNull;
                                            parms_ts[58].Value = item.BODY_DIENKK ?? Convert.DBNull;
                                            parms_ts[59].Value = item.BODY_PHONGTHI ?? Convert.DBNull;
                                            parms_ts[60].Value = item.BODY_DIEMTNC ?? Convert.DBNull;
                                            parms_ts[61].Value = item.BODY_XLTNC ?? Convert.DBNull;
                                            parms_ts[62].Value = item.BODY_TDTCU ?? Convert.DBNull;
                                            parms_ts[63].Value = item.BODY_GIAIHSG ?? Convert.DBNull;
                                            parms_ts[64].Value = item.BODY_GIAIHSGK ?? Convert.DBNull;
                                            parms_ts[65].Value = item.BODY_CHUNGCHINN ?? Convert.DBNull;
                                            parms_ts[66].Value = item.BODY_CHUNGCHITH ?? Convert.DBNull;
                                            parms_ts[67].Value = item.BODY_TONGDIEMMOI ?? Convert.DBNull;
                                            parms_ts[68].Value = item.BODY_BQAMOI ?? Convert.DBNull;
                                            parms_ts[69].Value = item.BODY_BQTMOI ?? Convert.DBNull;
                                            parms_ts[70].Value = item.BODY_SOCAPGIAYCN ?? Convert.DBNull;
                                            parms_ts[71].Value = item.BODY_XLHT ?? Convert.DBNull;
                                            parms_ts[72].Value = item.BODY_QUOCGIA ?? Convert.DBNull;
                                            parms_ts[73].Value = Utils.ConvertNgaySinhStrToInt(item.NgaySinhStr);
                                            parms_ts[74].Value = Utils.ConvertNgaySinh(item.NgaySinhStr) ?? Convert.DBNull;
                                            parms_ts[75].Value = Utils.NonUnicode(item.HoTen);

                                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_ThongTinThiSinh_Update_New", parms_ts);
                                            #endregion
                                            //Xóa điểm cũ
                                            SqlParameter[] parms_del = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                            };
                                            parms_del[0].Value = item.ThiSinhID;
                                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongTinDiemThi_Delete", parms_del);

                                            if (item.ListThongTinDiemThi != null && item.ListThongTinDiemThi.Count > 0)
                                            {
                                                foreach (var DiemThi in item.ListThongTinDiemThi)
                                                {
                                                    SqlParameter[] parms_dt = new SqlParameter[]{
                                                    new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                    new SqlParameter("MonThiID", SqlDbType.Int),
                                                    new SqlParameter("Diem", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                    new SqlParameter("NhomID", SqlDbType.Int),
                                                };

                                                    parms_dt[0].Value = item.ThiSinhID;
                                                    parms_dt[1].Value = DiemThi.MonThiID ?? Convert.DBNull;
                                                    //parms_dt[2].Value = DiemThi.Diem ?? Convert.DBNull;
                                                    parms_dt[2].Value = Utils.ConvertToNullableDecimal(DiemThi.DiemBaiToHop, null) ?? Convert.DBNull;
                                                    parms_dt[3].Value = DiemThi.DiemBaiToHop ?? Convert.DBNull;
                                                    parms_dt[4].Value = DiemThi.NhomID ?? Convert.DBNull;

                                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinDiemThi_Insert_New", parms_dt);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ////check trườg học có trong DB chưa, chưa có thì thêm vào DB
                                            //if ((item.TruongTHPT == null || item.TruongTHPT == 0) && !string.IsNullOrEmpty(item.TenTruongTHPT))
                                            //{
                                            //    DanhMucChungModel DM_Truong = new DanhMucChungModel();
                                            //    DM_Truong.Ten = item.TenTruongTHPT;
                                            //    DM_Truong.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                            //    var TempInsert = new DanhMucChungDAL().Insert(DM_Truong);
                                            //    item.TruongTHPT = Utils.ConvertToNullableInt32(TempInsert.Data, null);
                                            //}

                                            //insert thí sinh thi
                                            SqlParameter[] parms_ts = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("KyThiID", SqlDbType.Int),
                                                new SqlParameter("HoTen", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinh", SqlDbType.DateTime),
                                                new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                                                new SqlParameter("GioiTinh", SqlDbType.Bit),
                                                new SqlParameter("DanToc", SqlDbType.Int),
                                                new SqlParameter("CMND", SqlDbType.NVarChar),
                                                new SqlParameter("SoBaoDanh", SqlDbType.NVarChar),
                                                new SqlParameter("SoDienThoai", SqlDbType.NVarChar),
                                                new SqlParameter("DiaChi", SqlDbType.NVarChar),
                                                new SqlParameter("Lop", SqlDbType.NVarChar),
                                                new SqlParameter("TruongTHPT", SqlDbType.Int),
                                                new SqlParameter("LoaiDuThi", SqlDbType.NVarChar),
                                                new SqlParameter("DonViDKDT", SqlDbType.NVarChar),
                                                new SqlParameter("XepLoaiHanhKiem", SqlDbType.Int),
                                                new SqlParameter("XepLoaiHocLuc", SqlDbType.Int),
                                                new SqlParameter("DiemTBLop12", SqlDbType.Decimal),
                                                new SqlParameter("DiemKK", SqlDbType.Decimal),
                                                new SqlParameter("DienXTN", SqlDbType.Int),
                                                new SqlParameter("HoiDongThi", SqlDbType.Int),
                                                new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal),
                                                new SqlParameter("KetQuaTotNghiep", SqlDbType.NVarChar),
                                                new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                                                new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                                                new SqlParameter("NamThi", SqlDbType.Int),
                                                new SqlParameter("Do", SqlDbType.NVarChar),
                                                new SqlParameter("DoThem", SqlDbType.NVarChar),
                                                new SqlParameter("Hong", SqlDbType.NVarChar),
                                                new SqlParameter("LaoDong", SqlDbType.NVarChar),
                                                new SqlParameter("VanHoa", SqlDbType.NVarChar),
                                                new SqlParameter("RLTT", SqlDbType.NVarChar),
                                                new SqlParameter("TongSoDiemThi", SqlDbType.Decimal),
                                                new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                                                new SqlParameter("DiemXL", SqlDbType.Decimal),
                                                new SqlParameter("DiemUT", SqlDbType.Decimal),
                                                new SqlParameter("GhiChu", SqlDbType.NVarChar),
                                                new SqlParameter("Hang", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBCacBaiThi", SqlDbType.Decimal),
                                                new SqlParameter("DienUuTien", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBC", SqlDbType.Decimal),
                                                new SqlParameter("QueQuan", SqlDbType.NVarChar),
                                                new SqlParameter("ChungNhanNghe", SqlDbType.NVarChar),
                                                new SqlParameter("DTConLietSi", SqlDbType.NVarChar),
                                                new SqlParameter("GiaiTDKT", SqlDbType.NVarChar),
                                                new SqlParameter("HoiDong", SqlDbType.NVarChar),
                                                new SqlParameter("MonKN", SqlDbType.NVarChar),
                                                new SqlParameter("TBCNMonKN", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiCu", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiMoi", SqlDbType.NVarChar),
                                                new SqlParameter("TongBQ", SqlDbType.NVarChar),
                                                  new SqlParameter("BQA", SqlDbType.NVarChar),
                                                new SqlParameter("BQT", SqlDbType.NVarChar),
                                                new SqlParameter("DC", SqlDbType.NVarChar),
                                                new SqlParameter("Ban", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinhStr", SqlDbType.NVarChar),

                                                new SqlParameter("BODY_DAODUC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_RLEV", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIENKK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_PHONGTHI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIEMTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TDTCU", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSG", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSGK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHINN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHITH", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TONGDIEMMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQAMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQTMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_SOCAPGIAYCN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLHT", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_QUOCGIA", SqlDbType.NVarChar),
                                            };
                                            parms_ts[0].Direction = ParameterDirection.Output;
                                            parms_ts[0].Size = 8;
                                            parms_ts[1].Value = KyThiID;
                                            parms_ts[2].Value = item.HoTen ?? Convert.DBNull;
                                            parms_ts[3].Value = item.NgaySinh ?? Convert.DBNull;
                                            parms_ts[4].Value = item.NoiSinh ?? Convert.DBNull;
                                            parms_ts[5].Value = item.GioiTinh ?? Convert.DBNull;
                                            parms_ts[6].Value = item.DanToc ?? Convert.DBNull;
                                            parms_ts[7].Value = item.CMND ?? Convert.DBNull;
                                            parms_ts[8].Value = item.SoBaoDanh ?? Convert.DBNull;
                                            parms_ts[9].Value = item.SoDienThoai ?? Convert.DBNull;
                                            parms_ts[10].Value = item.DiaChi ?? Convert.DBNull;
                                            parms_ts[11].Value = item.Lop ?? Convert.DBNull;
                                            parms_ts[12].Value = item.TruongTHPT ?? Convert.DBNull;
                                            parms_ts[13].Value = item.LoaiDuThi ?? Convert.DBNull;
                                            parms_ts[14].Value = item.DonViDKDT ?? Convert.DBNull;
                                            parms_ts[15].Value = item.XepLoaiHanhKiem ?? Convert.DBNull;
                                            parms_ts[16].Value = item.XepLoaiHocLuc ?? Convert.DBNull;
                                            parms_ts[17].Value = item.DiemTBLop12 ?? Convert.DBNull;
                                            parms_ts[18].Value = item.DiemKK ?? Convert.DBNull;
                                            parms_ts[19].Value = item.DienXTN ?? Convert.DBNull;
                                            parms_ts[20].Value = item.HoiDongThi ?? Convert.DBNull;
                                            parms_ts[21].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                                            parms_ts[22].Value = item.KetQuaTotNghiep ?? Convert.DBNull;
                                            parms_ts[23].Value = item.SoHieuBang ?? Convert.DBNull;
                                            parms_ts[24].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                                            parms_ts[25].Value = item.NamThi ?? Convert.DBNull;
                                            parms_ts[26].Value = item.Do ?? Convert.DBNull;
                                            parms_ts[27].Value = item.DoThem ?? Convert.DBNull;
                                            parms_ts[28].Value = item.Hong ?? Convert.DBNull;
                                            parms_ts[29].Value = item.LaoDong ?? Convert.DBNull;
                                            parms_ts[30].Value = item.VanHoa ?? Convert.DBNull;
                                            parms_ts[31].Value = item.RLTT ?? Convert.DBNull;
                                            parms_ts[32].Value = item.TongSoDiemThi ?? Convert.DBNull;
                                            parms_ts[33].Value = item.NgayCapBang ?? Convert.DBNull;
                                            parms_ts[34].Value = item.DiemXL ?? Convert.DBNull;
                                            parms_ts[35].Value = item.DiemUT ?? Convert.DBNull;
                                            parms_ts[36].Value = item.GhiChu ?? Convert.DBNull;
                                            parms_ts[37].Value = item.Hang ?? Convert.DBNull;
                                            parms_ts[38].Value = item.DiemTBCacBaiThi ?? Convert.DBNull;
                                            parms_ts[39].Value = item.DienUuTien ?? Convert.DBNull;
                                            parms_ts[40].Value = item.DiemTBC ?? Convert.DBNull;
                                            parms_ts[41].Value = item.QueQuan ?? Convert.DBNull;
                                            parms_ts[42].Value = item.ChungNhanNghe ?? Convert.DBNull;
                                            parms_ts[43].Value = item.DTConLietSi ?? Convert.DBNull;
                                            parms_ts[44].Value = item.GiaiTDKT ?? Convert.DBNull;
                                            parms_ts[45].Value = item.HoiDong ?? Convert.DBNull;
                                            parms_ts[46].Value = item.MonKN ?? Convert.DBNull;
                                            parms_ts[47].Value = item.TBCNMonKN ?? Convert.DBNull;
                                            parms_ts[48].Value = item.DiemThiCu ?? Convert.DBNull;
                                            parms_ts[49].Value = item.DiemThiMoi ?? Convert.DBNull;
                                            parms_ts[50].Value = item.TongBQ ?? Convert.DBNull;
                                            parms_ts[51].Value = item.BQA ?? Convert.DBNull;
                                            parms_ts[52].Value = item.BQT ?? Convert.DBNull;
                                            parms_ts[53].Value = item.DC ?? Convert.DBNull;
                                            parms_ts[54].Value = item.Ban ?? Convert.DBNull;
                                            parms_ts[55].Value = item.NgaySinhStr ?? Convert.DBNull;

                                            parms_ts[56].Value = item.BODY_DAODUC ?? Convert.DBNull;
                                            parms_ts[57].Value = item.BODY_RLEV ?? Convert.DBNull;
                                            parms_ts[58].Value = item.BODY_DIENKK ?? Convert.DBNull;
                                            parms_ts[59].Value = item.BODY_PHONGTHI ?? Convert.DBNull;
                                            parms_ts[60].Value = item.BODY_DIEMTNC ?? Convert.DBNull;
                                            parms_ts[61].Value = item.BODY_XLTNC ?? Convert.DBNull;
                                            parms_ts[62].Value = item.BODY_TDTCU ?? Convert.DBNull;
                                            parms_ts[63].Value = item.BODY_GIAIHSG ?? Convert.DBNull;
                                            parms_ts[64].Value = item.BODY_GIAIHSGK ?? Convert.DBNull;
                                            parms_ts[65].Value = item.BODY_CHUNGCHINN ?? Convert.DBNull;
                                            parms_ts[66].Value = item.BODY_CHUNGCHITH ?? Convert.DBNull;
                                            parms_ts[67].Value = item.BODY_TONGDIEMMOI ?? Convert.DBNull;
                                            parms_ts[68].Value = item.BODY_BQAMOI ?? Convert.DBNull;
                                            parms_ts[69].Value = item.BODY_BQTMOI ?? Convert.DBNull;
                                            parms_ts[70].Value = item.BODY_SOCAPGIAYCN ?? Convert.DBNull;
                                            parms_ts[71].Value = item.BODY_XLHT ?? Convert.DBNull;
                                            parms_ts[72].Value = item.BODY_QUOCGIA ?? Convert.DBNull;

                                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_ThongTinThiSinh_Insert", parms_ts);
                                            int ThiSinhID = Utils.ConvertToInt32(parms_ts[0].Value, 0);
                                            if (ThiSinhID > 0 && item.ListThongTinDiemThi != null && item.ListThongTinDiemThi.Count > 0)
                                            {
                                                foreach (var DiemThi in item.ListThongTinDiemThi)
                                                {
                                                    SqlParameter[] parms_dt = new SqlParameter[]{
                                                    new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                    new SqlParameter("MonThiID", SqlDbType.Int),
                                                    new SqlParameter("Diem", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                    new SqlParameter("NhomID", SqlDbType.Int),
                                                };

                                                    parms_dt[0].Value = ThiSinhID;
                                                    parms_dt[1].Value = DiemThi.MonThiID ?? Convert.DBNull;
                                                    //parms_dt[2].Value = DiemThi.Diem ?? Convert.DBNull;
                                                    parms_dt[2].Value = Utils.ConvertToNullableDecimal(DiemThi.DiemBaiToHop, null) ?? Convert.DBNull;
                                                    parms_dt[3].Value = DiemThi.DiemBaiToHop ?? Convert.DBNull;
                                                    parms_dt[4].Value = DiemThi.NhomID ?? Convert.DBNull;

                                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinDiemThi_Insert_New", parms_dt);
                                                }

                                            }
                                            if (ThiSinhID > 0)
                                            {
                                                SqlParameter[] parms_wf = new SqlParameter[]{
                                                    new SqlParameter("DocumentID", SqlDbType.Int),
                                                    new SqlParameter("WorkflowID", SqlDbType.Int),
                                                    new SqlParameter("StateID", SqlDbType.Int),
                                                    new SqlParameter("CoQuanID", SqlDbType.Int),
                                                    new SqlParameter("DueDate", SqlDbType.DateTime),
                                                };
                                                parms_wf[0].Value = ThiSinhID;
                                                parms_wf[1].Value = WorkflowID;
                                                parms_wf[2].Value = StateID;
                                                parms_wf[3].Value = DuLieuDiemThiModel.CoQuanID != null ? DuLieuDiemThiModel.CoQuanID.Value : 0;
                                                parms_wf[4].Value = DateTime.Now;

                                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_Workflow_Insert_Document", parms_wf);
                                            }
                                        }

                                    }
                                }
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("dữ liệu điểm thi");
                                Result.Data = KyThiID;
                            }
                            catch (Exception ex)
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw ex;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public BaseResultModel InsertForImportExcel(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DuLieuDiemThiModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu điểm thi không được để trống";
                    return Result;
                }
                if (DuLieuDiemThiModel.ThongTinToChucThi == null)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin tổ chức thi không được để trống";
                    return Result;
                }
                else if (DuLieuDiemThiModel.ThongTinToChucThi.Nam == null || DuLieuDiemThiModel.ThongTinToChucThi.Nam == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin năm không được để trống";
                    return Result;
                }
                else
                {
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null)
                    {      
                        if(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Length > 0)
                        {
                            var HoiDong = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDong.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = HoiDong.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Length > 0)
                        {
                            var HoiDongChamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongChamThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = HoiDongChamThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Length > 0)
                        {
                            var HoiDongGiamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongGiamThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = HoiDongGiamThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Length > 0)
                        {
                            var HoiDongGiamKhao = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongGiamKhao.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = HoiDongGiamKhao.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Length > 0)
                        {
                            var HoiDongCoiThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongCoiThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = HoiDongCoiThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                    }
                    //check trung số trang, số quyển
                    var dl = CheckTrungSoQuyenSoTrangTheoNam(DuLieuDiemThiModel.ThongTinToChucThi.SoTrang, DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen, DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                    if (dl != null && dl.ThongTinToChucThi != null && dl.ThongTinToChucThi.KyThiID > 0 && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    {
                        var DuLieuDiemThiCu = GetByID(dl.ThongTinToChucThi.KyThiID);
                        if (DuLieuDiemThiCu.ThongTinThiSinh == null) DuLieuDiemThiCu.ThongTinThiSinh = new List<ThongTinThiSinh>();
                        DuLieuDiemThiCu.ThongTinThiSinh.AddRange(DuLieuDiemThiModel.ThongTinThiSinh);
                        Result = Update(DuLieuDiemThiCu, CanBoID);
                        return Result;
                    }
                    ////check trung số trang, số quyển
                    //var dl = CheckTrungSoQuyenSoTrang(DuLieuDiemThiModel.ThongTinToChucThi.SoTrang, DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen, DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi, DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay);
                    //if (dl != null && dl.ThongTinToChucThi != null && dl.ThongTinToChucThi.KyThiID > 0 && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    //{
                    //    if (dl.ThongTinThiSinh == null) dl.ThongTinThiSinh = new List<ThongTinThiSinh>();
                    //    dl.ThongTinThiSinh.AddRange(DuLieuDiemThiModel.ThongTinThiSinh);
                    //    Result = Update(dl, CanBoID);
                    //    return Result;
                    //}
                    //check trung số báo danh
                    //if (DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    //{
                    //    var listSBD = DuLieuDiemThiModel.ThongTinThiSinh.Where(x => x.SoBaoDanh != null && x.SoBaoDanh.Length > 0).Select(x => x.SoBaoDanh).ToList();
                    //    foreach (var sbd in listSBD)
                    //    {
                    //        var count = listSBD.Where(x => x == sbd).Count();
                    //        if (count > 1)
                    //        {
                    //            Result.Status = 0;
                    //            Result.Message = "Trùng số báo danh " + sbd;
                    //            return Result;
                    //        }
                    //    }
                    //}

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("KyThiID", SqlDbType.Int),
                        new SqlParameter("TenKyThi", SqlDbType.NVarChar),
                        new SqlParameter("HoiDongThiID", SqlDbType.Int),
                        new SqlParameter("KhoaThiID", SqlDbType.Int),
                        new SqlParameter("SBDDau", SqlDbType.NVarChar),
                        new SqlParameter("SBDCuoi", SqlDbType.NVarChar),
                        new SqlParameter("NguoiDocDiem", SqlDbType.NVarChar),
                        new SqlParameter("NguoiNhapVaInDiem", SqlDbType.NVarChar),
                        new SqlParameter("NguoiDocSoatBanGhi", SqlDbType.NVarChar),
                        new SqlParameter("NgayDuyetBangDiem", SqlDbType.DateTime),
                        new SqlParameter("ChuTichHoiDong", SqlDbType.NVarChar),
                        new SqlParameter("SoThiSinhDuThi", SqlDbType.Int),
                        new SqlParameter("DuocCongNhanTotNghiep", SqlDbType.Int),
                        new SqlParameter("TNLoaiGioi", SqlDbType.Int),
                        new SqlParameter("TNLoaiKha", SqlDbType.Int),
                        new SqlParameter("TNLoaiTB", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep2", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep3", SqlDbType.Int),
                        new SqlParameter("CanBoXetDuyet", SqlDbType.NVarChar),
                        new SqlParameter("GiamDocSo", SqlDbType.NVarChar),
                        new SqlParameter("GhiChu", SqlDbType.NVarChar),
                        new SqlParameter("TrangThai", SqlDbType.Int),
                        new SqlParameter("HoiDongChamThiID", SqlDbType.Int),
                        new SqlParameter("HoiDongGiamThiID", SqlDbType.Int),
                        new SqlParameter("HoiDongGiamKhaoID", SqlDbType.Int),
                        new SqlParameter("HoiDongCoiThiID", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienA", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienB", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep4_5", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep4_75", SqlDbType.Int),
                        new SqlParameter("Ban", SqlDbType.NVarChar),
                        new SqlParameter("NgayDuyetCham", SqlDbType.DateTime),
                        new SqlParameter("CanBoSoKT", SqlDbType.NVarChar),
                        new SqlParameter("KhongDuocCongNhanTotNghiep", SqlDbType.Int),
                        new SqlParameter("MauPhieuID", SqlDbType.Int),
                        new SqlParameter("ThuKy", SqlDbType.NVarChar),
                        new SqlParameter("PhoChuKhao", SqlDbType.NVarChar),
                        new SqlParameter("ChanhChuKhao", SqlDbType.NVarChar),
                        new SqlParameter("KhoaThiNgay", SqlDbType.DateTime),
                        new SqlParameter("PhongThi", SqlDbType.NVarChar),
                        new SqlParameter("SoQuyen", SqlDbType.NVarChar),
                        new SqlParameter("SoTrang", SqlDbType.Int),
                        new SqlParameter("TongSoThiSinh", SqlDbType.Int),
                        new SqlParameter("NgaySoDuyet", SqlDbType.DateTime),
                        new SqlParameter("Tinh", SqlDbType.NVarChar),
                        new SqlParameter("ToTruongHoiPhach", SqlDbType.NVarChar),
                        new SqlParameter("ChuTichHoiDongCoiThi", SqlDbType.NVarChar),
                        new SqlParameter("HieuTruong", SqlDbType.NVarChar),
                        new SqlParameter("ChuTichHoiDongChamThi", SqlDbType.NVarChar),
                        new SqlParameter("Nam", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienC", SqlDbType.Int),
                        new SqlParameter("DiaDanh", SqlDbType.NVarChar),
                        new SqlParameter("GhiChuCuoiTrang", SqlDbType.NVarChar),
                        new SqlParameter("SBDDau_CuoiTrang", SqlDbType.NVarChar),
                        new SqlParameter("SBDCuoi_CuoiTrang", SqlDbType.NVarChar),
                        new SqlParameter("TSDoThang", SqlDbType.Int),
                         new SqlParameter("TSDoThem", SqlDbType.Int),
                         new SqlParameter("TSThiHong", SqlDbType.Int),
                         new SqlParameter("PGiamDoc", SqlDbType.NVarChar),
                         new SqlParameter("NguoiKiemTra", SqlDbType.NVarChar),

                        new SqlParameter("FOOT_RPDD", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_KTD", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_DTBDIS", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_THUKY", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_GIAMSAT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_DCNTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Dien45", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Dien475", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_Gioi", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_Kha", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_TB", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM1DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM15DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM2DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEMTREN2DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VANGMATKHITHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VIPHAMQUYCHETHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSDIENUUTIEN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCOCHUNGNHANNGHE", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NGUOIKIEMTRAHS", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SKĐCNTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_STSDT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SSTSDT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_TND_D", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_TND_E", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_LTHUONG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SLTHUONG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCONLIETSI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCACDIENKHAC", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NGUOILAPBANG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNLAPBANG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNHOIDONGCOITHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNCHAMTHIXTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VTVGDTX", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CTHDPHUCKHAO", SqlDbType.NVarChar),
                        new SqlParameter("HEAD_HDCL", SqlDbType.NVarChar),
                        new SqlParameter("HEAD_TRUONG", SqlDbType.NVarChar),
                        new SqlParameter("NguoiImportID", SqlDbType.Int),

                    };

                    parameters[0].Direction = ParameterDirection.Output;
                    parameters[0].Size = 8;
                    parameters[1].Value = DuLieuDiemThiModel.ThongTinToChucThi.TenKyThi ?? Convert.DBNull;
                    parameters[2].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID ?? Convert.DBNull;
                    parameters[3].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiID ?? Convert.DBNull;
                    parameters[4].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDDau ?? Convert.DBNull;
                    parameters[5].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDCuoi ?? Convert.DBNull;
                    parameters[6].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiDocDiem ?? Convert.DBNull;
                    parameters[7].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiNhapVaInDiem ?? Convert.DBNull;
                    parameters[8].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiDocSoatBanGhi ?? Convert.DBNull;
                    parameters[9].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgayDuyetBangDiem ?? Convert.DBNull;
                    parameters[10].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDong ?? Convert.DBNull;
                    parameters[11].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoThiSinhDuThi ?? Convert.DBNull;
                    parameters[12].Value = DuLieuDiemThiModel.ThongTinToChucThi.DuocCongNhanTotNghiep ?? Convert.DBNull;
                    parameters[13].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiGioi ?? Convert.DBNull;
                    parameters[14].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiKha ?? Convert.DBNull;
                    parameters[15].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiTB ?? Convert.DBNull;
                    parameters[16].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep2 ?? Convert.DBNull;
                    parameters[17].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep3 ?? Convert.DBNull;
                    parameters[18].Value = DuLieuDiemThiModel.ThongTinToChucThi.CanBoXetDuyet ?? Convert.DBNull;
                    parameters[19].Value = DuLieuDiemThiModel.ThongTinToChucThi.GiamDocSo ?? Convert.DBNull;
                    parameters[20].Value = DuLieuDiemThiModel.ThongTinToChucThi.GhiChu ?? Convert.DBNull;
                    parameters[21].Value = DuLieuDiemThiModel.ThongTinToChucThi.TrangThai ?? Convert.DBNull;
                    parameters[22].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID ?? Convert.DBNull;
                    parameters[23].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID ?? Convert.DBNull;
                    parameters[24].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID ?? Convert.DBNull;
                    parameters[25].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID ?? Convert.DBNull;
                    parameters[26].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienA ?? Convert.DBNull;
                    parameters[27].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienB ?? Convert.DBNull;
                    parameters[28].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep4_5 ?? Convert.DBNull;
                    parameters[29].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep4_75 ?? Convert.DBNull;
                    parameters[30].Value = DuLieuDiemThiModel.ThongTinToChucThi.Ban ?? Convert.DBNull;
                    parameters[31].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgayDuyetCham ?? Convert.DBNull;
                    parameters[32].Value = DuLieuDiemThiModel.ThongTinToChucThi.CanBoSoKT ?? Convert.DBNull;
                    parameters[33].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhongDuocCongNhanTotNghiep ?? Convert.DBNull;
                    parameters[34].Value = DuLieuDiemThiModel.ThongTinToChucThi.MauPhieuID ?? Convert.DBNull;
                    parameters[35].Value = DuLieuDiemThiModel.ThongTinToChucThi.ThuKy ?? Convert.DBNull;
                    parameters[36].Value = DuLieuDiemThiModel.ThongTinToChucThi.PhoChuKhao ?? Convert.DBNull;
                    parameters[37].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChanhChuKhao ?? Convert.DBNull;
                    parameters[38].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay ?? Convert.DBNull;
                    parameters[39].Value = DuLieuDiemThiModel.ThongTinToChucThi.PhongThi ?? Convert.DBNull;
                    parameters[40].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen ?? Convert.DBNull;
                    parameters[41].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoTrang ?? Convert.DBNull;
                    parameters[42].Value = DuLieuDiemThiModel.ThongTinThiSinh != null ? DuLieuDiemThiModel.ThongTinThiSinh.Count : 0;
                    parameters[43].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgaySoDuyet ?? Convert.DBNull;
                    parameters[44].Value = DuLieuDiemThiModel.ThongTinToChucThi.Tinh ?? Convert.DBNull;
                    parameters[45].Value = DuLieuDiemThiModel.ThongTinToChucThi.ToTruongHoiPhach ?? Convert.DBNull;
                    parameters[46].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDongCoiThi ?? Convert.DBNull;
                    parameters[47].Value = DuLieuDiemThiModel.ThongTinToChucThi.HieuTruong ?? Convert.DBNull;
                    parameters[48].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDongChamThi ?? Convert.DBNull;
                    parameters[49].Value = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? Convert.DBNull;
                    parameters[50].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienC ?? Convert.DBNull;
                    parameters[51].Value = DuLieuDiemThiModel.ThongTinToChucThi.DiaDanh ?? Convert.DBNull;
                    parameters[52].Value = DuLieuDiemThiModel.ThongTinToChucThi.GhiChuCuoiTrang ?? Convert.DBNull;
                    parameters[53].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDDau_CuoiTrang ?? Convert.DBNull;
                    parameters[54].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDCuoi_CuoiTrang ?? Convert.DBNull;
                    parameters[55].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSDoThang ?? Convert.DBNull;
                    parameters[56].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSDoThem ?? Convert.DBNull;
                    parameters[57].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSThiHong ?? Convert.DBNull;
                    parameters[58].Value = DuLieuDiemThiModel.ThongTinToChucThi.PGiamDoc ?? Convert.DBNull;
                    parameters[59].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiKiemTra ?? Convert.DBNull;

                    parameters[60].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_RPDD ?? Convert.DBNull;
                    parameters[61].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_KTD ?? Convert.DBNull;
                    parameters[62].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_DTBDIS ?? Convert.DBNull;
                    parameters[63].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_THUKY ?? Convert.DBNull;
                    parameters[64].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_GIAMSAT ?? Convert.DBNull;     
                    parameters[65].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_DCNTN ?? Convert.DBNull;
                    parameters[66].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Dien45  ?? Convert.DBNull;
                    parameters[67].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Dien475 ?? Convert.DBNull;
                    parameters[68].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_Gioi ?? Convert.DBNull;
                    parameters[69].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_Kha ?? Convert.DBNull;
                    parameters[70].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_TB ?? Convert.DBNull;
                    parameters[71].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM1DIEM ?? Convert.DBNull;
                    parameters[72].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM15DIEM ?? Convert.DBNull;
                    parameters[73].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM2DIEM ?? Convert.DBNull;
                    parameters[74].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEMTREN2DIEM ?? Convert.DBNull;
                    parameters[75].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VANGMATKHITHI ?? Convert.DBNull;
                    parameters[76].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VIPHAMQUYCHETHI ?? Convert.DBNull;
                    parameters[77].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSDIENUUTIEN ?? Convert.DBNull;
                    parameters[78].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCOCHUNGNHANNGHE ?? Convert.DBNull;
                    parameters[79].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NGUOIKIEMTRAHS  ?? Convert.DBNull;
                    parameters[80].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SKĐCNTN ?? Convert.DBNull;
                    parameters[81].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_STSDT ?? Convert.DBNull;
                    parameters[82].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SSTSDT ?? Convert.DBNull;
                    parameters[83].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_TND_D ?? Convert.DBNull;
                    parameters[84].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_TND_E ?? Convert.DBNull;
                    parameters[85].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_LTHUONG ?? Convert.DBNull;
                    parameters[86].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SLTHUONG ?? Convert.DBNull;
                    parameters[87].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCONLIETSI ?? Convert.DBNull;
                    parameters[88].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCACDIENKHAC ?? Convert.DBNull;
                    parameters[89].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NGUOILAPBANG ?? Convert.DBNull;
                    parameters[90].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNLAPBANG ?? Convert.DBNull;
                    parameters[91].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNHOIDONGCOITHI ?? Convert.DBNull;
                    parameters[92].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNCHAMTHIXTN ?? Convert.DBNull;
                    parameters[93].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VTVGDTX ?? Convert.DBNull;
                    parameters[94].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CTHDPHUCKHAO ?? Convert.DBNull;
                    parameters[95].Value = DuLieuDiemThiModel.ThongTinToChucThi.HEAD_HDCL ?? Convert.DBNull;
                    parameters[96].Value = DuLieuDiemThiModel.ThongTinToChucThi.HEAD_TRUONG ?? Convert.DBNull;
                    parameters[97].Value = CanBoID ?? Convert.DBNull;

                    if (DuLieuDiemThiModel.ThongTinToChucThi.Nam == null && DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay != null)
                    {
                        parameters[49].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay.Value.Year;
                    }
                    if (DuLieuDiemThiModel.ChiTietMauPhieu != null && DuLieuDiemThiModel.ChiTietMauPhieu.MauPhieuID > 0)
                    {
                        parameters[34].Value = DuLieuDiemThiModel.ChiTietMauPhieu.MauPhieuID;
                    }
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                //insert thông tin tổ chức thi
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_Insert_New2", parameters);
                                int KyThiID = Utils.ConvertToInt32(parameters[0].Value, 0);
                                if (KyThiID > 0 && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                                {
                                    DataTable tableThiSinh = new DataTable();
                                    tableThiSinh.Columns.Add("ThiSinhID_Str");
                                    tableThiSinh.Columns.Add("KyThiID");
                                    tableThiSinh.Columns.Add("HoTen");
                                    tableThiSinh.Columns.Add("NgaySinh");
                                    tableThiSinh.Columns.Add("NgaySinhStr");
                                    tableThiSinh.Columns.Add("NoiSinh");
                                    tableThiSinh.Columns.Add("GioiTinh");
                                    tableThiSinh.Columns.Add("DanToc");
                                    tableThiSinh.Columns.Add("CMND");
                                    tableThiSinh.Columns.Add("SoBaoDanh");
                                    tableThiSinh.Columns.Add("SoDienThoai");
                                    tableThiSinh.Columns.Add("DiaChi");
                                    tableThiSinh.Columns.Add("Lop");
                                    tableThiSinh.Columns.Add("TruongTHPT");
                                    tableThiSinh.Columns.Add("LoaiDuThi");
                                    tableThiSinh.Columns.Add("DonViDKDT");
                                    tableThiSinh.Columns.Add("XepLoaiHanhKiem");
                                    tableThiSinh.Columns.Add("XepLoaiHocLuc");
                                    tableThiSinh.Columns.Add("DiemTBLop12");
                                    tableThiSinh.Columns.Add("DiemKK");
                                    tableThiSinh.Columns.Add("DienXTN");
                                    tableThiSinh.Columns.Add("HoiDongThi");
                                    tableThiSinh.Columns.Add("DiemXetTotNghiep");
                                    tableThiSinh.Columns.Add("KetQuaTotNghiep");
                                    tableThiSinh.Columns.Add("SoHieuBang");
                                    tableThiSinh.Columns.Add("VaoSoCapBangSo");
                                    tableThiSinh.Columns.Add("NamThi");
                                    tableThiSinh.Columns.Add("Do");
                                    tableThiSinh.Columns.Add("DoThem");
                                    tableThiSinh.Columns.Add("Hong");
                                    tableThiSinh.Columns.Add("LaoDong");
                                    tableThiSinh.Columns.Add("VanHoa");
                                    tableThiSinh.Columns.Add("RLTT");
                                    tableThiSinh.Columns.Add("TongSoDiemThi");
                                    tableThiSinh.Columns.Add("NgayCapBang");
                                    tableThiSinh.Columns.Add("DiemXL");
                                    tableThiSinh.Columns.Add("DiemUT");
                                    tableThiSinh.Columns.Add("GhiChu");
                                    tableThiSinh.Columns.Add("Hang");
                                    tableThiSinh.Columns.Add("DiemTBCacBaiThi");
                                    tableThiSinh.Columns.Add("DienUuTien");
                                    tableThiSinh.Columns.Add("DiemTBC");
                                    tableThiSinh.Columns.Add("QueQuan");
                                    tableThiSinh.Columns.Add("ChungNhanNghe");
                                    tableThiSinh.Columns.Add("DTConLietSi");
                                    tableThiSinh.Columns.Add("GiaiTDKT");
                                    tableThiSinh.Columns.Add("HoiDong");
                                    tableThiSinh.Columns.Add("MonKN");
                                    tableThiSinh.Columns.Add("TBCNMonKN");
                                    tableThiSinh.Columns.Add("DiemThiCu");
                                    tableThiSinh.Columns.Add("DiemThiMoi");
                                    tableThiSinh.Columns.Add("TongBQ");
                                    tableThiSinh.Columns.Add("BQA");
                                    tableThiSinh.Columns.Add("BQT");
                                    tableThiSinh.Columns.Add("DC");
                                    tableThiSinh.Columns.Add("Ban");

                                    tableThiSinh.Columns.Add("BODY_DAODUC");
                                    tableThiSinh.Columns.Add("BODY_RLEV");
                                    tableThiSinh.Columns.Add("BODY_DIENKK");
                                    tableThiSinh.Columns.Add("BODY_PHONGTHI");
                                    tableThiSinh.Columns.Add("BODY_DIEMTNC");
                                    tableThiSinh.Columns.Add("BODY_XLTNC");
                                    tableThiSinh.Columns.Add("BODY_TDTCU");
                                    tableThiSinh.Columns.Add("BODY_GIAIHSG");
                                    tableThiSinh.Columns.Add("BODY_GIAIHSGK");
                                    tableThiSinh.Columns.Add("BODY_CHUNGCHINN");
                                    tableThiSinh.Columns.Add("BODY_CHUNGCHITH");
                                    tableThiSinh.Columns.Add("BODY_TONGDIEMMOI");
                                    tableThiSinh.Columns.Add("BODY_BQAMOI");
                                    tableThiSinh.Columns.Add("BODY_BQTMOI");
                                    tableThiSinh.Columns.Add("BODY_SOCAPGIAYCN");
                                    tableThiSinh.Columns.Add("BODY_XLHT");
                                    tableThiSinh.Columns.Add("BODY_QUOCGIA");
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    DataTable tableDiem = new DataTable();
                                    tableDiem.Columns.Add("ThiSinhID_Str");
                                    tableDiem.Columns.Add("MonThiID");
                                    tableDiem.Columns.Add("Diem");
                                    tableDiem.Columns.Add("DiemBaiToHop");
                                    tableDiem.Columns.Add("NhomID");

                                    foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                                    {
                                        var ThiSinhID_Str = Guid.NewGuid().ToString();
                                        tableThiSinh.Rows.Add(ThiSinhID_Str,
                                                                KyThiID,
                                                                item.HoTen,
                                                                item.NgaySinh,
                                                                item.NgaySinhStr,
                                                                item.NoiSinh,
                                                                item.GioiTinh,
                                                                item.DanToc,
                                                                item.CMND,
                                                                item.SoBaoDanh,
                                                                item.SoDienThoai,
                                                                item.DiaChi,
                                                                item.Lop,
                                                                item.TruongTHPT,
                                                                item.LoaiDuThi,
                                                                item.DonViDKDT,
                                                                item.XepLoaiHanhKiem,
                                                                item.XepLoaiHocLuc,
                                                                item.DiemTBLop12,
                                                                item.DiemKK,
                                                                item.DienXTN,
                                                                item.HoiDongThi,
                                                                item.DiemXetTotNghiep,
                                                                item.KetQuaTotNghiep,
                                                                item.SoHieuBang,
                                                                item.VaoSoCapBangSo,
                                                                item.NamThi,
                                                                item.Do,
                                                                item.DoThem,
                                                                item.Hong,
                                                                item.LaoDong,
                                                                item.VanHoa,
                                                                item.RLTT,
                                                                item.TongSoDiemThi,
                                                                item.NgayCapBang,
                                                                item.DiemXL,
                                                                item.DiemUT,
                                                                item.GhiChu,
                                                                item.Hang,
                                                                item.DiemTBCacBaiThi,
                                                                item.DienUuTien,
                                                                item.DiemTBC,
                                                                item.QueQuan,
                                                                item.ChungNhanNghe,
                                                                item.DTConLietSi,
                                                                item.GiaiTDKT,
                                                                item.HoiDong,
                                                                item.MonKN,
                                                                item.TBCNMonKN,
                                                                item.DiemThiCu,
                                                                item.DiemThiMoi,
                                                                item.TongBQ,
                                                                item.BQA,
                                                                item.BQT,
                                                                item.DC,
                                                                item.Ban,

                                                                item.BODY_DAODUC,
                                                                item.BODY_RLEV,
                                                                item.BODY_DIENKK,
                                                                item.BODY_PHONGTHI,
                                                                item.BODY_DIEMTNC,
                                                                item.BODY_XLTNC,
                                                                item.BODY_TDTCU,
                                                                item.BODY_GIAIHSG,
                                                                item.BODY_GIAIHSGK,
                                                                item.BODY_CHUNGCHINN,
                                                                item.BODY_CHUNGCHITH,
                                                                item.BODY_TONGDIEMMOI,
                                                                item.BODY_BQAMOI,
                                                                item.BODY_BQTMOI,
                                                                item.BODY_SOCAPGIAYCN,
                                                                item.BODY_XLHT,
                                                                item.BODY_QUOCGIA
                                                                );
                                        foreach (var itemDiem in item.ListThongTinDiemThi)
                                        {
                                            tableDiem.Rows.Add(ThiSinhID_Str, itemDiem.MonThiID, itemDiem.Diem, itemDiem.DiemBaiToHop, itemDiem.NhomID);
                                        }
                                    }

                                    SqlParameter[] param = new SqlParameter[]
                                    {
                                        new SqlParameter("TableThiSinh", SqlDbType.Structured),
                                        new SqlParameter("TableDiem", SqlDbType.Structured),
                                    };

                                    param[0].Value = tableThiSinh;
                                    param[1].Value = tableDiem;

                                    var temp = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ImportExcel", param);
                                }
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("dữ liệu điểm thi");
                                Result.Data = KyThiID;
                            }
                            catch (Exception ex)
                            {
                                Result.Status = -1;
                                Result.Message = ex.Message;
                                //Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw ex;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ex.Message;
                //Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public BaseResultModel InsertForImportExcelAllPages(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DuLieuDiemThiModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu điểm thi không được để trống";
                    return Result;
                }
                if (DuLieuDiemThiModel.ThongTinToChucThi == null)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin tổ chức thi không được để trống";
                    return Result;
                }
                else if (DuLieuDiemThiModel.ThongTinToChucThi.Nam == null || DuLieuDiemThiModel.ThongTinToChucThi.Nam == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin năm không được để trống";
                    return Result;
                }
                else
                {
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null)
                    {
                        if (DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Length > 0)
                        {
                            var HoiDong = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDong.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = HoiDong.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }

                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Length > 0)
                        {
                            var HoiDongChamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongChamThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = HoiDongChamThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Length > 0)
                        {
                            var HoiDongGiamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongGiamThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = HoiDongGiamThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Length > 0)
                        {
                            var HoiDongGiamKhao = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongGiamKhao.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = HoiDongGiamKhao.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                        if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Length > 0)
                        {
                            var HoiDongCoiThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                            if (HoiDongCoiThi.ID > 0)
                            {
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = HoiDongCoiThi.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                                DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                    }

                    List<string> Quyen = DuLieuDiemThiModel.ThongTinThiSinh.Select(x => x.BODY_QUYEN).Distinct().ToList();                  
                    if(Quyen != null && Quyen.Count > 0)
                    {
                        foreach (var tenQuyen in Quyen)
                        {
                            var ThiSinhTheoQuyen = DuLieuDiemThiModel.ThongTinThiSinh.Where(x => x.BODY_QUYEN == tenQuyen).ToList();
                            if(ThiSinhTheoQuyen.Count > 0)
                            {
                                List<string> Trang = ThiSinhTheoQuyen.Select(x => x.BODY_TRANG).Distinct().ToList();
                                if(Trang != null && Trang.Count > 0)
                                {
                                    foreach (var trangSo in Trang)
                                    {
                                        var ThiSinhTheoTrang = ThiSinhTheoQuyen.Where(x => x.BODY_TRANG == trangSo).ToList();
                                        DuLieuDiemThiModel data = new DuLieuDiemThiModel();
                                        data.ThongTinToChucThi = DuLieuDiemThiModel.ThongTinToChucThi;
                                        data.ThongTinToChucThi.SoQuyen = tenQuyen;
                                        data.ThongTinToChucThi.SoTrang = Utils.ConvertToNullableInt32(trangSo, null);
                                        data.ThongTinThiSinh = ThiSinhTheoTrang;
                                        Result = InsertForImportExcel(data, CanBoID);
                                        ////check trung số trang, số quyển
                                        //var dl = CheckTrungSoQuyenSoTrangTheoNam(data.ThongTinToChucThi.SoTrang, data.ThongTinToChucThi.SoQuyen, data.ThongTinToChucThi.Nam ?? 0);
                                        //if (dl != null && dl.ThongTinToChucThi != null && dl.ThongTinToChucThi.KyThiID > 0 && data.ThongTinThiSinh != null && data.ThongTinThiSinh.Count > 0)
                                        //{
                                        //    var DuLieuDiemThiCu = GetByID(dl.ThongTinToChucThi.KyThiID);
                                        //    if (DuLieuDiemThiCu.ThongTinThiSinh == null) DuLieuDiemThiCu.ThongTinThiSinh = new List<ThongTinThiSinh>();
                                        //    DuLieuDiemThiCu.ThongTinThiSinh.AddRange(data.ThongTinThiSinh);
                                        //    Result = Update(DuLieuDiemThiCu, CanBoID);
                                        //    //return Result;
                                        //}
                                        //else
                                        //{
                                        //    Result = InsertForImportExcel(data, CanBoID);
                                        //}
                                    }
                                }     
                            }
                           
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ex.Message;
                //Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public BaseResultModel Update(DuLieuDiemThiModel DuLieuDiemThiModel, int? CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DuLieuDiemThiModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu điểm thi không được để trống";
                    return Result;
                }
                if (DuLieuDiemThiModel.ThongTinToChucThi == null)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin tổ chức thi không được để trống";
                    return Result;
                }
                else if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.KyThiID == 0)
                {
                    Result = Insert(DuLieuDiemThiModel, CanBoID);
                    return Result;
                }
                else
                {
                    //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID == 0)
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Length > 0)
                    {
                        //var HoiDong = new DanhMucHoiDongThiDAL().GetByTenHoiDong(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim());
                        var HoiDong = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                        if (HoiDong.ID > 0)
                        {
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = HoiDong.ID;
                        }
                        else
                        {
                            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                            DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongThi.Trim();
                            DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                            DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                            var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID = Utils.ConvertToInt32(hd.Data, 0);
                        }
                    }
                    //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID == 0)
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Length > 0)
                    {
                        var HoiDongChamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                        if (HoiDongChamThi.ID > 0)
                        {
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = HoiDongChamThi.ID;
                        }
                        else
                        {
                            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                            DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongChamThi.Trim();
                            DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                            DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                            var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID = Utils.ConvertToInt32(hd.Data, 0);
                        }
                    }

                    //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID == 0)
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Length > 0)
                    {
                        var HoiDongGiamThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                        if (HoiDongGiamThi.ID > 0)
                        {
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = HoiDongGiamThi.ID;
                        }
                        else
                        {
                            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                            DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamThi.Trim();
                            DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                            DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                            var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID = Utils.ConvertToInt32(hd.Data, 0);
                        }
                    }

                    //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID == 0)
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Length > 0)
                    {
                        var HoiDongGiamKhao = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                        if (HoiDongGiamKhao.ID > 0)
                        {
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = HoiDongGiamKhao.ID;
                        }
                        else
                        {
                            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                            DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongGiamKhao.Trim();
                            DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                            DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                            var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID = Utils.ConvertToInt32(hd.Data, 0);
                        }
                    }

                    //if (DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID == null || DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID == 0)
                    if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi != null && DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Length > 0)
                    {
                        var HoiDongCoiThi = new DanhMucChungDAL().GetByName(DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                        if (HoiDongCoiThi.ID > 0)
                        {
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = HoiDongCoiThi.ID;
                        }
                        else
                        {
                            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                            DanhMucChungModel.Ten = DuLieuDiemThiModel.ThongTinToChucThi.TenHoiDongCoiThi.Trim();
                            DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
                            DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                            var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                            DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID = Utils.ConvertToInt32(hd.Data, 0);
                        }
                    }

                    if (DuLieuDiemThiModel.ThongTinThiSinh != null)
                    {
                        foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                        {
                            if (item.NgaySinhStr != null && item.NgaySinh != null)
                            {
                                item.NgaySinh = Utils.ConvertToNullableDateTime(item.NgaySinhStr, null);
                            }

                            if ((item.TruongTHPT == null || item.TruongTHPT == 0) && item.TenTruongTHPT != null)
                            {
                                var Truong = new DanhMucChungDAL().GetByName(item.TenTruongTHPT.Trim(), DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0);
                                if (Truong.ID > 0)
                                {
                                    item.TruongTHPT = Truong.ID;
                                }
                                else
                                {
                                    DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                    DanhMucChungModel.Ten = item.TenTruongTHPT.Trim();
                                    DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                    DanhMucChungModel.Nam = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? 0;
                                    var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                    item.TruongTHPT = Utils.ConvertToInt32(hd.Data, 0);
                                }
                            }
                        }
                    }

                    //check trung số báo danh
                    //if (DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    //{
                    //    //var cr = GetByID(DuLieuDiemThiModel.ThongTinToChucThi.KyThiID);
                    //    var listSBD = DuLieuDiemThiModel.ThongTinThiSinh.Where(x => x.SoBaoDanh != null && x.SoBaoDanh.Length > 0).Select(x => x.SoBaoDanh).ToList();
                    //    foreach (var sbd in listSBD)
                    //    {
                    //        var count = listSBD.Where(x => x == sbd).Count();
                    //        if (count > 1)
                    //        {
                    //            Result.Status = 0;
                    //            Result.Message = "Trùng số báo danh " + sbd;
                    //            return Result;
                    //        }
                    //    }
                    //}

                    List<ThongTinThiSinh> ListThiSinhCrr = new List<ThongTinThiSinh>();
                    if(DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                    {
                        foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                        {
                            if(item.ThiSinhID > 0)
                            {
                                ThongTinThiSinh ThiSinhCrr = new QuanLyThiSinhDAL().GetThongTinThiSinh(item.ThiSinhID);
                                ListThiSinhCrr.Add(ThiSinhCrr);
                            }  
                        }
                    }

                    //var crObj = GetByID(DanhMucKhoaThiModel.KhoaThiID);
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                       new SqlParameter("KyThiID", SqlDbType.Int),
                        new SqlParameter("TenKyThi", SqlDbType.NVarChar),
                        new SqlParameter("HoiDongThiID", SqlDbType.Int),
                        new SqlParameter("KhoaThiID", SqlDbType.Int),
                        new SqlParameter("SBDDau", SqlDbType.NVarChar),
                        new SqlParameter("SBDCuoi", SqlDbType.NVarChar),
                        new SqlParameter("NguoiDocDiem", SqlDbType.NVarChar),
                        new SqlParameter("NguoiNhapVaInDiem", SqlDbType.NVarChar),
                        new SqlParameter("NguoiDocSoatBanGhi", SqlDbType.NVarChar),
                        new SqlParameter("NgayDuyetBangDiem", SqlDbType.DateTime),
                        new SqlParameter("ChuTichHoiDong", SqlDbType.NVarChar),
                        new SqlParameter("SoThiSinhDuThi", SqlDbType.Int),
                        new SqlParameter("DuocCongNhanTotNghiep", SqlDbType.Int),
                        new SqlParameter("TNLoaiGioi", SqlDbType.Int),
                        new SqlParameter("TNLoaiKha", SqlDbType.Int),
                        new SqlParameter("TNLoaiTB", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep2", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep3", SqlDbType.Int),
                        new SqlParameter("CanBoXetDuyet", SqlDbType.NVarChar),
                        new SqlParameter("GiamDocSo", SqlDbType.NVarChar),
                        new SqlParameter("GhiChu", SqlDbType.NVarChar),
                        new SqlParameter("TrangThai", SqlDbType.Int),
                        new SqlParameter("HoiDongChamThiID", SqlDbType.Int),
                        new SqlParameter("HoiDongGiamThiID", SqlDbType.Int),
                        new SqlParameter("HoiDongGiamKhaoID", SqlDbType.Int),
                        new SqlParameter("HoiDongCoiThiID", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienA", SqlDbType.Int),
                        new SqlParameter("TotNghiepDienB", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep4_5", SqlDbType.Int),
                        new SqlParameter("DienTotNghiep4_75", SqlDbType.Int),
                        new SqlParameter("Ban", SqlDbType.NVarChar),
                        new SqlParameter("NgayDuyetCham", SqlDbType.DateTime),
                        new SqlParameter("CanBoSoKT", SqlDbType.NVarChar),
                        new SqlParameter("KhongDuocCongNhanTotNghiep", SqlDbType.Int),
                        new SqlParameter("MauPhieuID", SqlDbType.Int),
                        new SqlParameter("ThuKy", SqlDbType.NVarChar),
                        new SqlParameter("PhoChuKhao", SqlDbType.NVarChar),
                        new SqlParameter("ChanhChuKhao", SqlDbType.NVarChar),
                        new SqlParameter("KhoaThiNgay", SqlDbType.DateTime),
                        new SqlParameter("PhongThi", SqlDbType.NVarChar),
                        new SqlParameter("SoQuyen", SqlDbType.NVarChar),
                        new SqlParameter("SoTrang", SqlDbType.Int),
                        new SqlParameter("TongSoThiSinh", SqlDbType.Int),
                        new SqlParameter("NgaySoDuyet", SqlDbType.DateTime),
                        new SqlParameter("Tinh", SqlDbType.NVarChar),
                        new SqlParameter("ToTruongHoiPhach", SqlDbType.NVarChar),
                        new SqlParameter("ChuTichHoiDongCoiThi", SqlDbType.NVarChar),
                        new SqlParameter("HieuTruong", SqlDbType.NVarChar),
                        new SqlParameter("ChuTichHoiDongChamThi", SqlDbType.NVarChar),
                         new SqlParameter("Nam", SqlDbType.Int),
                          new SqlParameter("TotNghiepDienC", SqlDbType.Int),
                            new SqlParameter("DiaDanh", SqlDbType.NVarChar),
                            new SqlParameter("GhiChuCuoiTrang", SqlDbType.NVarChar),
                            new SqlParameter("SBDDau_CuoiTrang", SqlDbType.NVarChar),
                            new SqlParameter("SBDCuoi_CuoiTrang", SqlDbType.NVarChar),
                            new SqlParameter("TSDoThang", SqlDbType.Int),
                         new SqlParameter("TSDoThem", SqlDbType.Int),
                         new SqlParameter("TSThiHong", SqlDbType.Int),
                         new SqlParameter("PGiamDoc", SqlDbType.NVarChar),
                         new SqlParameter("NguoiKiemTra", SqlDbType.NVarChar),

                        new SqlParameter("FOOT_RPDD", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_KTD", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_DTBDIS", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_THUKY", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_GIAMSAT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_DCNTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Dien45", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Dien475", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_Gioi", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_Kha", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_So_Loai_TB", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM1DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM15DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEM2DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CONGTHEMTREN2DIEM", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VANGMATKHITHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VIPHAMQUYCHETHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSDIENUUTIEN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCOCHUNGNHANNGHE", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NGUOIKIEMTRAHS", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SKĐCNTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_STSDT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SSTSDT", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_TND_D", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_TND_E", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_LTHUONG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_SLTHUONG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCONLIETSI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_HSCACDIENKHAC", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NGUOILAPBANG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNLAPBANG", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNHOIDONGCOITHI", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_NXNCHAMTHIXTN", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_VTVGDTX", SqlDbType.NVarChar),
                        new SqlParameter("FOOT_CTHDPHUCKHAO", SqlDbType.NVarChar),
                        new SqlParameter("HEAD_HDCL", SqlDbType.NVarChar),
                        new SqlParameter("HEAD_TRUONG", SqlDbType.NVarChar),
                    };

                    parameters[0].Value = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                    parameters[1].Value = DuLieuDiemThiModel.ThongTinToChucThi.TenKyThi ?? Convert.DBNull;
                    parameters[2].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongThiID ?? Convert.DBNull;
                    parameters[3].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiID ?? Convert.DBNull;
                    parameters[4].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDDau ?? Convert.DBNull;
                    parameters[5].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDCuoi ?? Convert.DBNull;
                    parameters[6].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiDocDiem ?? Convert.DBNull;
                    parameters[7].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiNhapVaInDiem ?? Convert.DBNull;
                    parameters[8].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiDocSoatBanGhi ?? Convert.DBNull;
                    parameters[9].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgayDuyetBangDiem ?? Convert.DBNull;
                    parameters[10].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDong ?? Convert.DBNull;
                    parameters[11].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoThiSinhDuThi ?? Convert.DBNull;
                    parameters[12].Value = DuLieuDiemThiModel.ThongTinToChucThi.DuocCongNhanTotNghiep ?? Convert.DBNull;
                    parameters[13].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiGioi ?? Convert.DBNull;
                    parameters[14].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiKha ?? Convert.DBNull;
                    parameters[15].Value = DuLieuDiemThiModel.ThongTinToChucThi.TNLoaiTB ?? Convert.DBNull;
                    parameters[16].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep2 ?? Convert.DBNull;
                    parameters[17].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep3 ?? Convert.DBNull;
                    parameters[18].Value = DuLieuDiemThiModel.ThongTinToChucThi.CanBoXetDuyet ?? Convert.DBNull;
                    parameters[19].Value = DuLieuDiemThiModel.ThongTinToChucThi.GiamDocSo ?? Convert.DBNull;
                    parameters[20].Value = DuLieuDiemThiModel.ThongTinToChucThi.GhiChu ?? Convert.DBNull;
                    parameters[21].Value = DuLieuDiemThiModel.ThongTinToChucThi.TrangThai ?? Convert.DBNull;
                    parameters[22].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongChamThiID ?? Convert.DBNull;
                    parameters[23].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamThiID ?? Convert.DBNull;
                    parameters[24].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongGiamKhaoID ?? Convert.DBNull;
                    parameters[25].Value = DuLieuDiemThiModel.ThongTinToChucThi.HoiDongCoiThiID ?? Convert.DBNull;
                    parameters[26].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienA ?? Convert.DBNull;
                    parameters[27].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienB ?? Convert.DBNull;
                    parameters[28].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep4_5 ?? Convert.DBNull;
                    parameters[29].Value = DuLieuDiemThiModel.ThongTinToChucThi.DienTotNghiep4_75 ?? Convert.DBNull;
                    parameters[30].Value = DuLieuDiemThiModel.ThongTinToChucThi.Ban ?? Convert.DBNull;
                    parameters[31].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgayDuyetCham ?? Convert.DBNull;
                    parameters[32].Value = DuLieuDiemThiModel.ThongTinToChucThi.CanBoSoKT ?? Convert.DBNull;
                    parameters[33].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhongDuocCongNhanTotNghiep ?? Convert.DBNull;
                    parameters[34].Value = DuLieuDiemThiModel.ThongTinToChucThi.MauPhieuID ?? Convert.DBNull;
                    parameters[34].Value = DuLieuDiemThiModel.ThongTinToChucThi.MauPhieuID ?? Convert.DBNull;
                    parameters[35].Value = DuLieuDiemThiModel.ThongTinToChucThi.ThuKy ?? Convert.DBNull;
                    parameters[36].Value = DuLieuDiemThiModel.ThongTinToChucThi.PhoChuKhao ?? Convert.DBNull;
                    parameters[37].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChanhChuKhao ?? Convert.DBNull;
                    parameters[38].Value = DuLieuDiemThiModel.ThongTinToChucThi.KhoaThiNgay ?? Convert.DBNull;
                    parameters[39].Value = DuLieuDiemThiModel.ThongTinToChucThi.PhongThi ?? Convert.DBNull;
                    parameters[40].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoQuyen ?? Convert.DBNull;
                    parameters[41].Value = DuLieuDiemThiModel.ThongTinToChucThi.SoTrang ?? Convert.DBNull;
                    parameters[42].Value = DuLieuDiemThiModel.ThongTinThiSinh != null ? DuLieuDiemThiModel.ThongTinThiSinh.Count : 0;
                    parameters[43].Value = DuLieuDiemThiModel.ThongTinToChucThi.NgaySoDuyet ?? Convert.DBNull;
                    parameters[44].Value = DuLieuDiemThiModel.ThongTinToChucThi.Tinh ?? Convert.DBNull;
                    parameters[45].Value = DuLieuDiemThiModel.ThongTinToChucThi.ToTruongHoiPhach ?? Convert.DBNull;
                    parameters[46].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDongCoiThi ?? Convert.DBNull;
                    parameters[47].Value = DuLieuDiemThiModel.ThongTinToChucThi.HieuTruong ?? Convert.DBNull;
                    parameters[48].Value = DuLieuDiemThiModel.ThongTinToChucThi.ChuTichHoiDongChamThi ?? Convert.DBNull;
                    parameters[49].Value = DuLieuDiemThiModel.ThongTinToChucThi.Nam ?? Convert.DBNull;
                    parameters[50].Value = DuLieuDiemThiModel.ThongTinToChucThi.TotNghiepDienC ?? Convert.DBNull;
                    parameters[51].Value = DuLieuDiemThiModel.ThongTinToChucThi.DiaDanh ?? Convert.DBNull;
                    parameters[52].Value = DuLieuDiemThiModel.ThongTinToChucThi.GhiChuCuoiTrang ?? Convert.DBNull;
                    parameters[53].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDDau_CuoiTrang ?? Convert.DBNull;
                    parameters[54].Value = DuLieuDiemThiModel.ThongTinToChucThi.SBDCuoi_CuoiTrang ?? Convert.DBNull;
                    parameters[55].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSDoThang ?? Convert.DBNull;
                    parameters[56].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSDoThem ?? Convert.DBNull;
                    parameters[57].Value = DuLieuDiemThiModel.ThongTinToChucThi.TSThiHong ?? Convert.DBNull;
                    parameters[58].Value = DuLieuDiemThiModel.ThongTinToChucThi.PGiamDoc ?? Convert.DBNull;
                    parameters[59].Value = DuLieuDiemThiModel.ThongTinToChucThi.NguoiKiemTra ?? Convert.DBNull;
                    parameters[60].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_RPDD ?? Convert.DBNull;
                    parameters[61].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_KTD ?? Convert.DBNull;
                    parameters[62].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_DTBDIS ?? Convert.DBNull;
                    parameters[63].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_THUKY ?? Convert.DBNull;
                    parameters[64].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_GIAMSAT ?? Convert.DBNull;
                    parameters[65].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_DCNTN ?? Convert.DBNull;
                    parameters[66].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Dien45 ?? Convert.DBNull;
                    parameters[67].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Dien475 ?? Convert.DBNull;
                    parameters[68].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_Gioi ?? Convert.DBNull;
                    parameters[69].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_Kha ?? Convert.DBNull;
                    parameters[70].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_So_Loai_TB ?? Convert.DBNull;
                    parameters[71].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM1DIEM ?? Convert.DBNull;
                    parameters[72].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM15DIEM ?? Convert.DBNull;
                    parameters[73].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEM2DIEM ?? Convert.DBNull;
                    parameters[74].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CONGTHEMTREN2DIEM ?? Convert.DBNull;
                    parameters[75].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VANGMATKHITHI ?? Convert.DBNull;
                    parameters[76].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VIPHAMQUYCHETHI ?? Convert.DBNull;
                    parameters[77].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSDIENUUTIEN ?? Convert.DBNull;
                    parameters[78].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCOCHUNGNHANNGHE ?? Convert.DBNull;
                    parameters[79].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NGUOIKIEMTRAHS ?? Convert.DBNull;
                    parameters[80].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SKĐCNTN ?? Convert.DBNull;
                    parameters[81].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_STSDT ?? Convert.DBNull;
                    parameters[82].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SSTSDT ?? Convert.DBNull;
                    parameters[83].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_TND_D ?? Convert.DBNull;
                    parameters[84].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_TND_E ?? Convert.DBNull;
                    parameters[85].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_LTHUONG ?? Convert.DBNull;
                    parameters[86].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_SLTHUONG ?? Convert.DBNull;
                    parameters[87].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCONLIETSI ?? Convert.DBNull;
                    parameters[88].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_HSCACDIENKHAC ?? Convert.DBNull;
                    parameters[89].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NGUOILAPBANG ?? Convert.DBNull;
                    parameters[90].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNLAPBANG ?? Convert.DBNull;
                    parameters[91].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNHOIDONGCOITHI ?? Convert.DBNull;
                    parameters[92].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_NXNCHAMTHIXTN ?? Convert.DBNull;
                    parameters[93].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_VTVGDTX ?? Convert.DBNull;
                    parameters[94].Value = DuLieuDiemThiModel.ThongTinToChucThi.FOOT_CTHDPHUCKHAO ?? Convert.DBNull;
                    parameters[95].Value = DuLieuDiemThiModel.ThongTinToChucThi.HEAD_HDCL ?? Convert.DBNull;
                    parameters[96].Value = DuLieuDiemThiModel.ThongTinToChucThi.HEAD_TRUONG ?? Convert.DBNull;

                    if (DuLieuDiemThiModel.ChiTietMauPhieu != null && DuLieuDiemThiModel.ChiTietMauPhieu.MauPhieuID > 0)
                    {
                        parameters[34].Value = DuLieuDiemThiModel.ChiTietMauPhieu.MauPhieuID;
                    }

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_Update_New2", parameters);
                                if (DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                                {

                                    foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                                    {
                                        if (item.ThiSinhID > 0)
                                        {
                                            //Lưu log chỉnh sửa thông tin thí sinh
                                            ThongTinThiSinh ThiSinhCrr = ListThiSinhCrr.Where(x => x.ThiSinhID == item.ThiSinhID).ToList().FirstOrDefault();
                                            ThongTinThiSinhLog ThiSinhLog = new ThongTinThiSinhLog();
                                            ThiSinhLog.ThiSinhID = item.ThiSinhID;
                                            ThiSinhLog.KyThiCuID = ThiSinhCrr.KyThiID;
                                            if (ThiSinhCrr.HoTen != item.HoTen)
                                            {
                                                ThiSinhLog.HoTenCu = ThiSinhCrr.HoTen;
                                                ThiSinhLog.HoTenMoi = item.HoTen;
                                            }
                                            if (ThiSinhCrr.NgaySinh != item.NgaySinh)
                                            {
                                                ThiSinhLog.NgaySinhCu = ThiSinhCrr.NgaySinh;
                                                ThiSinhLog.NgaySinhMoi = item.NgaySinh;
                                            }
                                            if (ThiSinhCrr.GioiTinh != item.GioiTinh)
                                            {
                                                ThiSinhLog.GioiTinhCu = ThiSinhCrr.GioiTinh;
                                                ThiSinhLog.GioiTinhMoi = item.GioiTinh;
                                            }
                                            if (ThiSinhCrr.DanToc != item.DanToc)
                                            {
                                                ThiSinhLog.DanTocCu = ThiSinhCrr.DanToc;
                                                ThiSinhLog.DanTocMoi = item.DanToc;
                                            }
                                            if (ThiSinhCrr.NoiSinh != item.NoiSinh)
                                            {
                                                ThiSinhLog.NoiSinhCu = ThiSinhCrr.NoiSinh;
                                                ThiSinhLog.NoiSinhMoi = item.NoiSinh;
                                            }
                                            if (ThiSinhCrr.CMND != item.CMND)
                                            {
                                                ThiSinhLog.CMNDCu = ThiSinhCrr.CMND;
                                                ThiSinhLog.CMNDMoi = item.CMND;
                                            }
                                            if (ThiSinhCrr.SoBaoDanh != item.SoBaoDanh)
                                            {
                                                ThiSinhLog.SoBaoDanhCu = ThiSinhCrr.SoBaoDanh;
                                                ThiSinhLog.SoBaoDanhMoi = item.SoBaoDanh;
                                            }
                                            if (ThiSinhCrr.SoDienThoai != item.SoDienThoai)
                                            {
                                                ThiSinhLog.SoDienThoaiCu = ThiSinhCrr.SoDienThoai;
                                                ThiSinhLog.SoDienThoaiMoi = item.SoDienThoai;
                                            }
                                            if (ThiSinhCrr.DiaChi != item.DiaChi)
                                            {
                                                ThiSinhLog.DiaChiCu = ThiSinhCrr.DiaChi;
                                                ThiSinhLog.DiaChiMoi = item.DiaChi;
                                            }
                                            if (ThiSinhCrr.Lop != item.Lop)
                                            {
                                                ThiSinhLog.LopCu = ThiSinhCrr.Lop;
                                                ThiSinhLog.LopMoi = item.Lop;
                                            }
                                            if (ThiSinhCrr.TruongTHPT != item.TruongTHPT)
                                            {
                                                ThiSinhLog.TruongTHPTCu = ThiSinhCrr.TruongTHPT;
                                                ThiSinhLog.TruongTHPTMoi = item.TruongTHPT;
                                            }
                                            if (ThiSinhCrr.LoaiDuThi != item.LoaiDuThi)
                                            {
                                                ThiSinhLog.LoaiDuThiCu = ThiSinhCrr.LoaiDuThi;
                                                ThiSinhLog.LoaiDuThiMoi = item.LoaiDuThi;
                                            }
                                            if (ThiSinhCrr.DonViDKDT != item.DonViDKDT)
                                            {
                                                ThiSinhLog.DonViDKDTCu = ThiSinhCrr.DonViDKDT;
                                                ThiSinhLog.DonViDKDTMoi = item.DonViDKDT;
                                            }
                                            if (ThiSinhCrr.XepLoaiHanhKiem != item.XepLoaiHanhKiem)
                                            {
                                                ThiSinhLog.XepLoaiHanhKiemCu = ThiSinhCrr.XepLoaiHanhKiem;
                                                ThiSinhLog.XepLoaiHanhKiemMoi = item.XepLoaiHanhKiem;
                                            }
                                            if (ThiSinhCrr.XepLoaiHocLuc != item.XepLoaiHocLuc)
                                            {
                                                ThiSinhLog.XepLoaiHocLucCu = ThiSinhCrr.XepLoaiHocLuc;
                                                ThiSinhLog.XepLoaiHocLucMoi = item.XepLoaiHocLuc;
                                            }
                                            if (ThiSinhCrr.DiemTBLop12 != item.DiemTBLop12)
                                            {
                                                ThiSinhLog.DiemTBLop12Cu = ThiSinhCrr.DiemTBLop12;
                                                ThiSinhLog.DiemTBLop12Moi = item.DiemTBLop12;
                                            }
                                            if (ThiSinhCrr.DiemKK != item.DiemKK)
                                            {
                                                ThiSinhLog.DiemKKCu = ThiSinhCrr.DiemKK;
                                                ThiSinhLog.DiemKKMoi = item.DiemKK;
                                            }
                                            if (ThiSinhCrr.DienXTN != item.DienXTN)
                                            {
                                                ThiSinhLog.DienXTNCu = ThiSinhCrr.DienXTN;
                                                ThiSinhLog.DienXTNMoi = item.DienXTN;
                                            }
                                            if (ThiSinhCrr.HoiDongThi != item.HoiDongThi)
                                            {
                                                ThiSinhLog.HoiDongThiCu = ThiSinhCrr.HoiDongThi;
                                                ThiSinhLog.HoiDongThiMoi = item.HoiDongThi;
                                            }
                                            if (ThiSinhCrr.DiemXetTotNghiep != item.DiemXetTotNghiep)
                                            {
                                                ThiSinhLog.DiemXetTotNghiepCu = ThiSinhCrr.DiemXetTotNghiep;
                                                ThiSinhLog.DiemXetTotNghiepMoi = item.DiemXetTotNghiep;
                                            }
                                            if (ThiSinhCrr.KetQuaTotNghiep != item.KetQuaTotNghiep)
                                            {
                                                ThiSinhLog.KetQuaTotNghiepCu = ThiSinhCrr.KetQuaTotNghiep;
                                                ThiSinhLog.KetQuaTotNghiepMoi = item.KetQuaTotNghiep;
                                            }
                                            if (ThiSinhCrr.SoHieuBang != item.SoHieuBang)
                                            {
                                                ThiSinhLog.SoHieuBangCu = ThiSinhCrr.SoHieuBang;
                                                ThiSinhLog.SoHieuBangMoi = item.SoHieuBang;
                                            }
                                            if (ThiSinhCrr.VaoSoCapBangSo != item.VaoSoCapBangSo)
                                            {
                                                ThiSinhLog.VaoSoCapBangSoCu = ThiSinhCrr.VaoSoCapBangSo;
                                                ThiSinhLog.VaoSoCapBangSoMoi = item.VaoSoCapBangSo;
                                            }
                                            if (ThiSinhCrr.NgayCapBang != item.NgayCapBang)
                                            {
                                                ThiSinhLog.NgayCapBangCu = ThiSinhCrr.NgayCapBang;
                                                ThiSinhLog.NgayCapBangMoi = item.NgayCapBang;
                                            }
                                            if (ThiSinhCrr.NamThi != item.NamThi)
                                            {
                                                ThiSinhLog.NamThiCu = ThiSinhCrr.NamThi;
                                                ThiSinhLog.NamThiMoi = item.NamThi;
                                            }
                                            if (ThiSinhCrr.Do != item.Do)
                                            {
                                                ThiSinhLog.DoCu = ThiSinhCrr.Do;
                                                ThiSinhLog.DoMoi = item.Do;
                                            }
                                            if (ThiSinhCrr.DoThem != item.DoThem)
                                            {
                                                ThiSinhLog.DoThemCu = ThiSinhCrr.DoThem;
                                                ThiSinhLog.DoThemMoi = item.DoThem;
                                            }
                                            if (ThiSinhCrr.Hong != item.Hong)
                                            {
                                                ThiSinhLog.HongCu = ThiSinhCrr.Hong;
                                                ThiSinhLog.HongMoi = item.Hong;
                                            }
                                            if (ThiSinhCrr.LaoDong != item.LaoDong)
                                            {
                                                ThiSinhLog.LaoDongCu = ThiSinhCrr.LaoDong;
                                                ThiSinhLog.LaoDongMoi = item.LaoDong;
                                            }
                                            if (ThiSinhCrr.VanHoa != item.VanHoa)
                                            {
                                                ThiSinhLog.VanHoaCu = ThiSinhCrr.VanHoa;
                                                ThiSinhLog.VanHoaMoi = item.VanHoa;
                                            }
                                            if (ThiSinhCrr.RLTT != item.RLTT)
                                            {
                                                ThiSinhLog.RLTTCu = ThiSinhCrr.RLTT;
                                                ThiSinhLog.RLTTMoi = item.RLTT;
                                            }
                                            if (ThiSinhCrr.TongSoDiemThi != item.TongSoDiemThi)
                                            {
                                                ThiSinhLog.TongSoDiemThiCu = ThiSinhCrr.TongSoDiemThi;
                                                ThiSinhLog.TongSoDiemThiMoi = item.TongSoDiemThi;
                                            }
                                            if (ThiSinhCrr.TongSoDiemThi != item.TongSoDiemThi)
                                            {
                                                ThiSinhLog.TongSoDiemThiCu = ThiSinhCrr.TongSoDiemThi;
                                                ThiSinhLog.TongSoDiemThiMoi = item.TongSoDiemThi;
                                            }
                                            if (ThiSinhCrr.DiemXL != item.DiemXL)
                                            {
                                                ThiSinhLog.DiemXLCu = ThiSinhCrr.DiemXL;
                                                ThiSinhLog.DiemXLMoi = item.DiemXL;
                                            }
                                            if (ThiSinhCrr.DiemUT != item.DiemUT)
                                            {
                                                ThiSinhLog.DiemUTCu = ThiSinhCrr.DiemUT;
                                                ThiSinhLog.DiemUTMoi = item.DiemUT;
                                            }
                                            if (ThiSinhCrr.GhiChu != item.GhiChu)
                                            {
                                                ThiSinhLog.GhiChuCu = ThiSinhCrr.GhiChu;
                                                ThiSinhLog.GhiChuMoi = item.GhiChu;
                                            }
                                            if (ThiSinhCrr.Hang != item.Hang)
                                            {
                                                ThiSinhLog.HangCu = ThiSinhCrr.Hang;
                                                ThiSinhLog.HangMoi = item.Hang;
                                            }
                                            if (ThiSinhCrr.DiemTBCacBaiThi != item.DiemTBCacBaiThi)
                                            {
                                                ThiSinhLog.DiemTBCacBaiThiCu = ThiSinhCrr.DiemTBCacBaiThi;
                                                ThiSinhLog.DiemTBCacBaiThiMoi = item.DiemTBCacBaiThi;
                                            }
                                            if (ThiSinhCrr.DiemTBC != item.DiemTBC)
                                            {
                                                ThiSinhLog.DiemTBCCu = ThiSinhCrr.DiemTBC;
                                                ThiSinhLog.DiemTBCMoi = item.DiemTBC;
                                            }
                                            if (ThiSinhCrr.QueQuan != item.QueQuan)
                                            {
                                                ThiSinhLog.QueQuanCu = ThiSinhCrr.QueQuan;
                                                ThiSinhLog.QueQuanMoi = item.QueQuan;
                                            }
                                            if (ThiSinhCrr.ChungNhanNghe != item.ChungNhanNghe)
                                            {
                                                ThiSinhLog.ChungNhanNgheCu = ThiSinhCrr.ChungNhanNghe;
                                                ThiSinhLog.ChungNhanNgheMoi = item.ChungNhanNghe;
                                            }
                                            if (ThiSinhCrr.DTConLietSi != item.DTConLietSi)
                                            {
                                                ThiSinhLog.DTConLietSiCu = ThiSinhCrr.DTConLietSi;
                                                ThiSinhLog.DTConLietSiMoi = item.DTConLietSi;
                                            }
                                            if (ThiSinhCrr.GiaiTDKT != item.GiaiTDKT)
                                            {
                                                ThiSinhLog.GiaiTDKTCu = ThiSinhCrr.GiaiTDKT;
                                                ThiSinhLog.GiaiTDKTMoi = item.GiaiTDKT;
                                            }

                                            if (ThiSinhCrr.HoiDong != item.HoiDong)
                                            {
                                                ThiSinhLog.HoiDongCu = ThiSinhCrr.HoiDong;
                                                ThiSinhLog.HoiDongMoi = item.HoiDong;
                                            }

                                            if (ThiSinhCrr.MonKN != item.MonKN)
                                            {
                                                ThiSinhLog.MonKNCu = ThiSinhCrr.MonKN;
                                                ThiSinhLog.MonKNMoi = item.MonKN;
                                            }

                                            if (ThiSinhCrr.TBCNMonKN != item.TBCNMonKN)
                                            {
                                                ThiSinhLog.TBCNMonKNCu = ThiSinhCrr.TBCNMonKN;
                                                ThiSinhLog.TBCNMonKNMoi = item.TBCNMonKN;
                                            }

                                            if (ThiSinhCrr.DiemThiCu != item.DiemThiCu)
                                            {
                                                ThiSinhLog.DiemThiCuCu = ThiSinhCrr.DiemThiCu;
                                                ThiSinhLog.DiemThiCuMoi = item.DiemThiCu;
                                            }

                                            if (ThiSinhCrr.DiemThiMoi != item.DiemThiMoi)
                                            {
                                                ThiSinhLog.DiemThiMoiCu = ThiSinhCrr.DiemThiMoi;
                                                ThiSinhLog.DiemThiMoiMoi = item.DiemThiMoi;
                                            }

                                            if (ThiSinhCrr.TongBQ != item.TongBQ)
                                            {
                                                ThiSinhLog.TongBQCu = ThiSinhCrr.TongBQ;
                                                ThiSinhLog.TongBQMoi = item.TongBQ;
                                            }

                                            if (ThiSinhCrr.BQA != item.BQA)
                                            {
                                                ThiSinhLog.BQACu = ThiSinhCrr.BQA;
                                                ThiSinhLog.BQAMoi = item.BQA;
                                            }

                                            if (ThiSinhCrr.BQT != item.BQT)
                                            {
                                                ThiSinhLog.BQTCu = ThiSinhCrr.BQT;
                                                ThiSinhLog.BQTMoi = item.BQT;
                                            }

                                            if (ThiSinhCrr.DC != item.DC)
                                            {
                                                ThiSinhLog.DCCu = ThiSinhCrr.DC;
                                                ThiSinhLog.DCMoi = item.DC;
                                            }

                                            if (ThiSinhCrr.Ban != item.Ban)
                                            {
                                                ThiSinhLog.BanCu = ThiSinhCrr.Ban;
                                                ThiSinhLog.BanMoi = item.Ban;
                                            }

                                            if (ThiSinhCrr.NgaySinhStr != item.NgaySinhStr)
                                            {
                                                ThiSinhLog.NgaySinhStrCu = ThiSinhCrr.NgaySinhStr;
                                                ThiSinhLog.NgaySinhStrMoi = item.NgaySinhStr;
                                            }

                                            if (ThiSinhCrr.BODY_QUOCGIA != item.BODY_QUOCGIA)
                                            {
                                                ThiSinhLog.BODY_QUOCGIACu = ThiSinhCrr.BODY_QUOCGIA;
                                                ThiSinhLog.BODY_QUOCGIAMoi = item.BODY_QUOCGIA;
                                            }

                                            if (ThiSinhCrr.ListThongTinDiemThi != null && ThiSinhCrr.ListThongTinDiemThi.Count > 0)
                                            {
                                                ThiSinhLog.ListThongTinDiemThiMoi = new List<ThongTinDiemThiLog>();
                                                foreach (var itemDiemCrr in ThiSinhCrr.ListThongTinDiemThi)
                                                {
                                                    foreach (var itemDiemEdit in item.ListThongTinDiemThi)
                                                    {
                                                        if (itemDiemCrr.MonThiID == itemDiemEdit.MonThiID && itemDiemCrr.NhomID == itemDiemEdit.NhomID)
                                                        {
                                                            ThongTinDiemThiLog DiemThiLog = new ThongTinDiemThiLog();
                                                            DiemThiLog.MonThiID = itemDiemCrr.MonThiID;
                                                            DiemThiLog.ThiSinhID = itemDiemCrr.ThiSinhID;
                                                            DiemThiLog.NhomID = itemDiemCrr.NhomID;
                                                            if (itemDiemCrr.Diem != itemDiemEdit.Diem)
                                                            {
                                                                DiemThiLog.DiemCu = itemDiemCrr.Diem;
                                                                DiemThiLog.DiemMoi = itemDiemEdit.Diem;
                                                            }
                                                            if (itemDiemCrr.DiemBaiToHop != itemDiemEdit.DiemBaiToHop)
                                                            {
                                                                DiemThiLog.DiemBaiToHopCu = itemDiemCrr.DiemBaiToHop;
                                                                DiemThiLog.DiemBaiToHopMoi = itemDiemEdit.DiemBaiToHop;
                                                            }
                                                            if (DiemThiLog.DiemMoi != null || DiemThiLog.DiemBaiToHopMoi != null)
                                                            {
                                                                ThiSinhLog.ListThongTinDiemThiMoi.Add(DiemThiLog);
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            bool check = false;
                                            if(ThiSinhCrr.ListThongTinDiemThi != null && item.ListThongTinDiemThi != null)
                                            {
                                                if(ThiSinhCrr.ListThongTinDiemThi.Count != item.ListThongTinDiemThi.Count)
                                                {
                                                    check = true;
                                                }
                                            }

                                            //câp nhật cả thong tin thí sinh và điểm
                                            if ((check || (ThiSinhLog.ListThongTinDiemThiMoi != null && ThiSinhLog.ListThongTinDiemThiMoi.Count > 0)) && (!string.IsNullOrEmpty(ThiSinhLog.HoTenMoi) || ThiSinhLog.NgaySinhMoi != null || !string.IsNullOrEmpty(ThiSinhLog.NoiSinhMoi) || ThiSinhLog.GioiTinhMoi != null || ThiSinhLog.DanTocMoi != null || !string.IsNullOrEmpty(ThiSinhLog.CMNDMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.SoBaoDanhMoi) || !string.IsNullOrEmpty(ThiSinhLog.SoDienThoaiMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiaChiMoi) || !string.IsNullOrEmpty(ThiSinhLog.LopMoi) || ThiSinhLog.TruongTHPTMoi != null
                                                || !string.IsNullOrEmpty(ThiSinhLog.LoaiDuThiMoi) || !string.IsNullOrEmpty(ThiSinhLog.DonViDKDTMoi) || ThiSinhLog.XepLoaiHanhKiemMoi != null || ThiSinhLog.XepLoaiHocLucMoi != null || ThiSinhLog.DiemTBLop12Moi != null || ThiSinhLog.DiemKKMoi != null
                                                || ThiSinhLog.DienXTNMoi != null || ThiSinhLog.HoiDongThiMoi != null || ThiSinhLog.DiemXetTotNghiepMoi != null || !string.IsNullOrEmpty(ThiSinhLog.KetQuaTotNghiepMoi) || !string.IsNullOrEmpty(ThiSinhLog.SoHieuBangMoi) || !string.IsNullOrEmpty(ThiSinhLog.VaoSoCapBangSoMoi)
                                                || ThiSinhLog.NgayCapBangMoi != null || ThiSinhLog.NamThiMoi != null || !string.IsNullOrEmpty(ThiSinhLog.DoMoi) || !string.IsNullOrEmpty(ThiSinhLog.DoThemMoi) || !string.IsNullOrEmpty(ThiSinhLog.HongMoi) || !string.IsNullOrEmpty(ThiSinhLog.LaoDongMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.VanHoaMoi) || !string.IsNullOrEmpty(ThiSinhLog.RLTTMoi) || ThiSinhLog.TongSoDiemThiMoi != null || ThiSinhLog.DiemXLMoi != null || ThiSinhLog.DiemUTMoi != null || !string.IsNullOrEmpty(ThiSinhLog.GhiChuMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.HangMoi) || ThiSinhLog.DiemTBCacBaiThiMoi != null || ThiSinhLog.DiemTBCMoi != null || ThiSinhLog.DienUuTienMoi != null 
                                                || !string.IsNullOrEmpty(ThiSinhLog.QueQuanMoi) || !string.IsNullOrEmpty(ThiSinhLog.ChungNhanNgheMoi) || !string.IsNullOrEmpty(ThiSinhLog.DTConLietSiMoi) || !string.IsNullOrEmpty(ThiSinhLog.GiaiTDKTMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.HoiDongMoi) || !string.IsNullOrEmpty(ThiSinhLog.MonKNMoi) || !string.IsNullOrEmpty(ThiSinhLog.TBCNMonKNMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiemThiCuMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiemThiMoiMoi) || !string.IsNullOrEmpty(ThiSinhLog.TongBQMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.BQAMoi) || !string.IsNullOrEmpty(ThiSinhLog.BQTMoi) || !string.IsNullOrEmpty(ThiSinhLog.DCMoi) || !string.IsNullOrEmpty(ThiSinhLog.BanMoi) || !string.IsNullOrEmpty(ThiSinhLog.NgaySinhStrMoi) 
                                                || !string.IsNullOrEmpty(ThiSinhLog.BODY_QUOCGIAMoi)))
                                            {
                                                if (!check)
                                                {
                                                    var temp = InsertThiSinhLog(ThiSinhLog, EnumLog.Update.GetHashCode(), CanBoID, 1);
                                                }
                                                
                                                //update thí sinh thi
                                                SqlParameter[] parms_ts = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("KyThiID", SqlDbType.Int),
                                                new SqlParameter("HoTen", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinh", SqlDbType.DateTime),
                                                new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                                                new SqlParameter("GioiTinh", SqlDbType.Bit),
                                                new SqlParameter("DanToc", SqlDbType.Int),
                                                new SqlParameter("CMND", SqlDbType.NVarChar),
                                                new SqlParameter("SoBaoDanh", SqlDbType.NVarChar),
                                                new SqlParameter("SoDienThoai", SqlDbType.NVarChar),
                                                new SqlParameter("DiaChi", SqlDbType.NVarChar),
                                                new SqlParameter("Lop", SqlDbType.NVarChar),
                                                new SqlParameter("TruongTHPT", SqlDbType.Int),
                                                new SqlParameter("LoaiDuThi", SqlDbType.NVarChar),
                                                new SqlParameter("DonViDKDT", SqlDbType.NVarChar),
                                                new SqlParameter("XepLoaiHanhKiem", SqlDbType.Int),
                                                new SqlParameter("XepLoaiHocLuc", SqlDbType.Int),
                                                new SqlParameter("DiemTBLop12", SqlDbType.Decimal),
                                                new SqlParameter("DiemKK", SqlDbType.Decimal),
                                                new SqlParameter("DienXTN", SqlDbType.Int),
                                                new SqlParameter("HoiDongThi", SqlDbType.Int),
                                                new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal),
                                                new SqlParameter("KetQuaTotNghiep", SqlDbType.NVarChar),
                                                new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                                                new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                                                new SqlParameter("NamThi", SqlDbType.Int),
                                                new SqlParameter("Do", SqlDbType.NVarChar),
                                                new SqlParameter("DoThem", SqlDbType.NVarChar),
                                                new SqlParameter("Hong", SqlDbType.NVarChar),
                                                new SqlParameter("LaoDong", SqlDbType.NVarChar),
                                                new SqlParameter("VanHoa", SqlDbType.NVarChar),
                                                new SqlParameter("RLTT", SqlDbType.NVarChar),
                                                new SqlParameter("TongSoDiemThi", SqlDbType.Decimal),
                                                new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                                                new SqlParameter("DiemXL", SqlDbType.Decimal),
                                                new SqlParameter("DiemUT", SqlDbType.Decimal),
                                                new SqlParameter("GhiChu", SqlDbType.NVarChar),
                                                new SqlParameter("Hang", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBCacBaiThi", SqlDbType.Decimal),
                                                new SqlParameter("DienUuTien", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBC", SqlDbType.Decimal),
                                                new SqlParameter("QueQuan", SqlDbType.NVarChar),
                                                new SqlParameter("ChungNhanNghe", SqlDbType.NVarChar),
                                                new SqlParameter("DTConLietSi", SqlDbType.NVarChar),
                                                new SqlParameter("GiaiTDKT", SqlDbType.NVarChar),
                                                new SqlParameter("HoiDong", SqlDbType.NVarChar),
                                                new SqlParameter("MonKN", SqlDbType.NVarChar),
                                                new SqlParameter("TBCNMonKN", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiCu", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiMoi", SqlDbType.NVarChar),
                                                new SqlParameter("TongBQ", SqlDbType.NVarChar),
                                                  new SqlParameter("BQA", SqlDbType.NVarChar),
                                                new SqlParameter("BQT", SqlDbType.NVarChar),
                                                new SqlParameter("DC", SqlDbType.NVarChar),
                                                new SqlParameter("Ban", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinhStr", SqlDbType.NVarChar),

                                                new SqlParameter("BODY_DAODUC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_RLEV", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIENKK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_PHONGTHI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIEMTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TDTCU", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSG", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSGK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHINN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHITH", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TONGDIEMMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQAMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQTMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_SOCAPGIAYCN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLHT", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_QUOCGIA", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinh_Int", SqlDbType.Int),
                                                new SqlParameter("NgaySinhFormat", SqlDbType.DateTime),
                                                new SqlParameter("HoTenFormat", SqlDbType.NVarChar),
                                            };

                                                parms_ts[0].Value = item.ThiSinhID;
                                                parms_ts[1].Value = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                                                parms_ts[2].Value = item.HoTen ?? Convert.DBNull;
                                                parms_ts[3].Value = item.NgaySinh ?? Convert.DBNull;
                                                parms_ts[4].Value = item.NoiSinh ?? Convert.DBNull;
                                                parms_ts[5].Value = item.GioiTinh ?? Convert.DBNull;
                                                parms_ts[6].Value = item.DanToc ?? Convert.DBNull;
                                                parms_ts[7].Value = item.CMND ?? Convert.DBNull;
                                                parms_ts[8].Value = item.SoBaoDanh ?? Convert.DBNull;
                                                parms_ts[9].Value = item.SoDienThoai ?? Convert.DBNull;
                                                parms_ts[10].Value = item.DiaChi ?? Convert.DBNull;
                                                parms_ts[11].Value = item.Lop ?? Convert.DBNull;
                                                parms_ts[12].Value = item.TruongTHPT ?? Convert.DBNull;
                                                parms_ts[13].Value = item.LoaiDuThi ?? Convert.DBNull;
                                                parms_ts[14].Value = item.DonViDKDT ?? Convert.DBNull;
                                                parms_ts[15].Value = item.XepLoaiHanhKiem ?? Convert.DBNull;
                                                parms_ts[16].Value = item.XepLoaiHocLuc ?? Convert.DBNull;
                                                parms_ts[17].Value = item.DiemTBLop12 ?? Convert.DBNull;
                                                parms_ts[18].Value = item.DiemKK ?? Convert.DBNull;
                                                parms_ts[19].Value = item.DienXTN ?? Convert.DBNull;
                                                parms_ts[20].Value = item.HoiDongThi ?? Convert.DBNull;
                                                parms_ts[21].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                                                parms_ts[22].Value = item.KetQuaTotNghiep ?? Convert.DBNull;
                                                parms_ts[23].Value = item.SoHieuBang ?? Convert.DBNull;
                                                parms_ts[24].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                                                parms_ts[25].Value = item.NamThi ?? Convert.DBNull;
                                                parms_ts[26].Value = item.Do ?? Convert.DBNull;
                                                parms_ts[27].Value = item.DoThem ?? Convert.DBNull;
                                                parms_ts[28].Value = item.Hong ?? Convert.DBNull;
                                                parms_ts[29].Value = item.LaoDong ?? Convert.DBNull;
                                                parms_ts[30].Value = item.VanHoa ?? Convert.DBNull;
                                                parms_ts[31].Value = item.RLTT ?? Convert.DBNull;
                                                parms_ts[32].Value = item.TongSoDiemThi ?? Convert.DBNull;
                                                parms_ts[33].Value = item.NgayCapBang ?? Convert.DBNull;
                                                parms_ts[34].Value = item.DiemXL ?? Convert.DBNull;
                                                parms_ts[35].Value = item.DiemUT ?? Convert.DBNull;
                                                parms_ts[36].Value = item.GhiChu ?? Convert.DBNull;
                                                parms_ts[37].Value = item.Hang ?? Convert.DBNull;
                                                parms_ts[38].Value = item.DiemTBCacBaiThi ?? Convert.DBNull;
                                                parms_ts[39].Value = item.DienUuTien ?? Convert.DBNull;
                                                parms_ts[40].Value = item.DiemTBC ?? Convert.DBNull;
                                                parms_ts[41].Value = item.QueQuan ?? Convert.DBNull;
                                                parms_ts[42].Value = item.ChungNhanNghe ?? Convert.DBNull;
                                                parms_ts[43].Value = item.DTConLietSi ?? Convert.DBNull;
                                                parms_ts[44].Value = item.GiaiTDKT ?? Convert.DBNull;
                                                parms_ts[45].Value = item.HoiDong ?? Convert.DBNull;
                                                parms_ts[46].Value = item.MonKN ?? Convert.DBNull;
                                                parms_ts[47].Value = item.TBCNMonKN ?? Convert.DBNull;
                                                parms_ts[48].Value = item.DiemThiCu ?? Convert.DBNull;
                                                parms_ts[49].Value = item.DiemThiMoi ?? Convert.DBNull;
                                                parms_ts[50].Value = item.TongBQ ?? Convert.DBNull;
                                                parms_ts[51].Value = item.BQA ?? Convert.DBNull;
                                                parms_ts[52].Value = item.BQT ?? Convert.DBNull;
                                                parms_ts[53].Value = item.DC ?? Convert.DBNull;
                                                parms_ts[54].Value = item.Ban ?? Convert.DBNull;
                                                parms_ts[55].Value = item.NgaySinhStr ?? Convert.DBNull;

                                                parms_ts[56].Value = item.BODY_DAODUC ?? Convert.DBNull;
                                                parms_ts[57].Value = item.BODY_RLEV ?? Convert.DBNull;
                                                parms_ts[58].Value = item.BODY_DIENKK ?? Convert.DBNull;
                                                parms_ts[59].Value = item.BODY_PHONGTHI ?? Convert.DBNull;
                                                parms_ts[60].Value = item.BODY_DIEMTNC ?? Convert.DBNull;
                                                parms_ts[61].Value = item.BODY_XLTNC ?? Convert.DBNull;
                                                parms_ts[62].Value = item.BODY_TDTCU ?? Convert.DBNull;
                                                parms_ts[63].Value = item.BODY_GIAIHSG ?? Convert.DBNull;
                                                parms_ts[64].Value = item.BODY_GIAIHSGK ?? Convert.DBNull;
                                                parms_ts[65].Value = item.BODY_CHUNGCHINN ?? Convert.DBNull;
                                                parms_ts[66].Value = item.BODY_CHUNGCHITH ?? Convert.DBNull;
                                                parms_ts[67].Value = item.BODY_TONGDIEMMOI ?? Convert.DBNull;
                                                parms_ts[68].Value = item.BODY_BQAMOI ?? Convert.DBNull;
                                                parms_ts[69].Value = item.BODY_BQTMOI ?? Convert.DBNull;
                                                parms_ts[70].Value = item.BODY_SOCAPGIAYCN ?? Convert.DBNull;
                                                parms_ts[71].Value = item.BODY_XLHT ?? Convert.DBNull;
                                                parms_ts[72].Value = item.BODY_QUOCGIA ?? Convert.DBNull;
                                                parms_ts[73].Value = Utils.ConvertNgaySinhStrToInt(item.NgaySinhStr);
                                                parms_ts[74].Value = Utils.ConvertNgaySinh(item.NgaySinhStr) ?? Convert.DBNull;
                                                parms_ts[75].Value = Utils.NonUnicode(item.HoTen);

                                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_ThongTinThiSinh_Update_New", parms_ts);

                                                //Xóa điểm cũ
                                                SqlParameter[] parms_del = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                };
                                                parms_del[0].Value = item.ThiSinhID;
                                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongTinDiemThi_Delete", parms_del);

                                                if (item.ListThongTinDiemThi != null && item.ListThongTinDiemThi.Count > 0)
                                                {
                                                    foreach (var DiemThi in item.ListThongTinDiemThi)
                                                    {
                                                        SqlParameter[] parms_dt = new SqlParameter[]{
                                                        new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                        new SqlParameter("MonThiID", SqlDbType.Int),
                                                        new SqlParameter("Diem", SqlDbType.Decimal),
                                                        new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                        new SqlParameter("NhomID", SqlDbType.Int),
                                                        };

                                                        parms_dt[0].Value = item.ThiSinhID;
                                                        parms_dt[1].Value = DiemThi.MonThiID ?? Convert.DBNull;
                                                        //parms_dt[2].Value = DiemThi.Diem ?? Convert.DBNull;
                                                        parms_dt[2].Value = Utils.ConvertToNullableDecimal(DiemThi.DiemBaiToHop, null) ?? Convert.DBNull;
                                                        parms_dt[3].Value = DiemThi.DiemBaiToHop ?? Convert.DBNull;
                                                        parms_dt[4].Value = DiemThi.NhomID ?? Convert.DBNull;

                                                        SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinDiemThi_Insert_New", parms_dt);
                                                    }
                                                }
                                            }
                                            //cập nhật chỉ điểm
                                            else if ((ThiSinhLog.ListThongTinDiemThiMoi != null && ThiSinhLog.ListThongTinDiemThiMoi.Count > 0) && !(!string.IsNullOrEmpty(ThiSinhLog.HoTenMoi) || ThiSinhLog.NgaySinhMoi != null || !string.IsNullOrEmpty(ThiSinhLog.NoiSinhMoi) || ThiSinhLog.GioiTinhMoi != null || ThiSinhLog.DanTocMoi != null || !string.IsNullOrEmpty(ThiSinhLog.CMNDMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.SoBaoDanhMoi) || !string.IsNullOrEmpty(ThiSinhLog.SoDienThoaiMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiaChiMoi) || !string.IsNullOrEmpty(ThiSinhLog.LopMoi) || ThiSinhLog.TruongTHPTMoi != null
                                                || !string.IsNullOrEmpty(ThiSinhLog.LoaiDuThiMoi) || !string.IsNullOrEmpty(ThiSinhLog.DonViDKDTMoi) || ThiSinhLog.XepLoaiHanhKiemMoi != null || ThiSinhLog.XepLoaiHocLucMoi != null || ThiSinhLog.DiemTBLop12Moi != null || ThiSinhLog.DiemKKMoi != null
                                                || ThiSinhLog.DienXTNMoi != null || ThiSinhLog.HoiDongThiMoi != null || ThiSinhLog.DiemXetTotNghiepMoi != null || !string.IsNullOrEmpty(ThiSinhLog.KetQuaTotNghiepMoi) || !string.IsNullOrEmpty(ThiSinhLog.SoHieuBangMoi) || !string.IsNullOrEmpty(ThiSinhLog.VaoSoCapBangSoMoi)
                                                || ThiSinhLog.NgayCapBangMoi != null || ThiSinhLog.NamThiMoi != null || !string.IsNullOrEmpty(ThiSinhLog.DoMoi) || !string.IsNullOrEmpty(ThiSinhLog.DoThemMoi) || !string.IsNullOrEmpty(ThiSinhLog.HongMoi) || !string.IsNullOrEmpty(ThiSinhLog.LaoDongMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.VanHoaMoi) || !string.IsNullOrEmpty(ThiSinhLog.RLTTMoi) || ThiSinhLog.TongSoDiemThiMoi != null || ThiSinhLog.DiemXLMoi != null || ThiSinhLog.DiemUTMoi != null || !string.IsNullOrEmpty(ThiSinhLog.GhiChuMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.HangMoi) || ThiSinhLog.DiemTBCacBaiThiMoi != null || ThiSinhLog.DiemTBCMoi != null || ThiSinhLog.DienUuTienMoi != null
                                                || !string.IsNullOrEmpty(ThiSinhLog.QueQuanMoi) || !string.IsNullOrEmpty(ThiSinhLog.ChungNhanNgheMoi) || !string.IsNullOrEmpty(ThiSinhLog.DTConLietSiMoi) || !string.IsNullOrEmpty(ThiSinhLog.GiaiTDKTMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.HoiDongMoi) || !string.IsNullOrEmpty(ThiSinhLog.MonKNMoi) || !string.IsNullOrEmpty(ThiSinhLog.TBCNMonKNMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiemThiCuMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiemThiMoiMoi) || !string.IsNullOrEmpty(ThiSinhLog.TongBQMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.BQAMoi) || !string.IsNullOrEmpty(ThiSinhLog.BQTMoi) || !string.IsNullOrEmpty(ThiSinhLog.DCMoi) || !string.IsNullOrEmpty(ThiSinhLog.BanMoi) || !string.IsNullOrEmpty(ThiSinhLog.NgaySinhStrMoi) 
                                                || !string.IsNullOrEmpty(ThiSinhLog.BODY_QUOCGIAMoi)))
                                            {
                                                var temp = InsertThiSinhLog(ThiSinhLog, EnumLog.Update.GetHashCode(), CanBoID, 3);

                                                //update điẻm  thí sinh thi

                                                //Xóa điểm cũ
                                                SqlParameter[] parms_del = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                };
                                                parms_del[0].Value = item.ThiSinhID;
                                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongTinDiemThi_Delete", parms_del);

                                                if (item.ListThongTinDiemThi != null && item.ListThongTinDiemThi.Count > 0)
                                                {
                                                    foreach (var DiemThi in item.ListThongTinDiemThi)
                                                    {
                                                        SqlParameter[] parms_dt = new SqlParameter[]{
                                                        new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                        new SqlParameter("MonThiID", SqlDbType.Int),
                                                        new SqlParameter("Diem", SqlDbType.Decimal),
                                                        new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                        new SqlParameter("NhomID", SqlDbType.Int),
                                                        };

                                                        parms_dt[0].Value = item.ThiSinhID;
                                                        parms_dt[1].Value = DiemThi.MonThiID ?? Convert.DBNull;
                                                        //parms_dt[2].Value = DiemThi.Diem ?? Convert.DBNull;
                                                        parms_dt[2].Value = Utils.ConvertToNullableDecimal(DiemThi.DiemBaiToHop, null) ?? Convert.DBNull;
                                                        parms_dt[3].Value = DiemThi.DiemBaiToHop ?? Convert.DBNull;
                                                        parms_dt[4].Value = DiemThi.NhomID ?? Convert.DBNull;

                                                        SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinDiemThi_Insert_New", parms_dt);
                                                    }
                                                }
                                            }
                                            //cập nhật chỉ thông tin thí sinh
                                            else if (!(ThiSinhLog.ListThongTinDiemThiMoi != null && ThiSinhLog.ListThongTinDiemThiMoi.Count > 0) && (!string.IsNullOrEmpty(ThiSinhLog.HoTenMoi) || ThiSinhLog.NgaySinhMoi != null || !string.IsNullOrEmpty(ThiSinhLog.NoiSinhMoi) || ThiSinhLog.GioiTinhMoi != null || ThiSinhLog.DanTocMoi != null || !string.IsNullOrEmpty(ThiSinhLog.CMNDMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.SoBaoDanhMoi) || !string.IsNullOrEmpty(ThiSinhLog.SoDienThoaiMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiaChiMoi) || !string.IsNullOrEmpty(ThiSinhLog.LopMoi) || ThiSinhLog.TruongTHPTMoi != null
                                                || !string.IsNullOrEmpty(ThiSinhLog.LoaiDuThiMoi) || !string.IsNullOrEmpty(ThiSinhLog.DonViDKDTMoi) || ThiSinhLog.XepLoaiHanhKiemMoi != null || ThiSinhLog.XepLoaiHocLucMoi != null || ThiSinhLog.DiemTBLop12Moi != null || ThiSinhLog.DiemKKMoi != null
                                                || ThiSinhLog.DienXTNMoi != null || ThiSinhLog.HoiDongThiMoi != null || ThiSinhLog.DiemXetTotNghiepMoi != null || !string.IsNullOrEmpty(ThiSinhLog.KetQuaTotNghiepMoi) || !string.IsNullOrEmpty(ThiSinhLog.SoHieuBangMoi) || !string.IsNullOrEmpty(ThiSinhLog.VaoSoCapBangSoMoi)
                                                || ThiSinhLog.NgayCapBangMoi != null || ThiSinhLog.NamThiMoi != null || !string.IsNullOrEmpty(ThiSinhLog.DoMoi) || !string.IsNullOrEmpty(ThiSinhLog.DoThemMoi) || !string.IsNullOrEmpty(ThiSinhLog.HongMoi) || !string.IsNullOrEmpty(ThiSinhLog.LaoDongMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.VanHoaMoi) || !string.IsNullOrEmpty(ThiSinhLog.RLTTMoi) || ThiSinhLog.TongSoDiemThiMoi != null || ThiSinhLog.DiemXLMoi != null || ThiSinhLog.DiemUTMoi != null || !string.IsNullOrEmpty(ThiSinhLog.GhiChuMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.HangMoi) || ThiSinhLog.DiemTBCacBaiThiMoi != null || ThiSinhLog.DiemTBCMoi != null || ThiSinhLog.DienUuTienMoi != null
                                                || !string.IsNullOrEmpty(ThiSinhLog.QueQuanMoi) || !string.IsNullOrEmpty(ThiSinhLog.ChungNhanNgheMoi) || !string.IsNullOrEmpty(ThiSinhLog.DTConLietSiMoi) || !string.IsNullOrEmpty(ThiSinhLog.GiaiTDKTMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.HoiDongMoi) || !string.IsNullOrEmpty(ThiSinhLog.MonKNMoi) || !string.IsNullOrEmpty(ThiSinhLog.TBCNMonKNMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiemThiCuMoi) || !string.IsNullOrEmpty(ThiSinhLog.DiemThiMoiMoi) || !string.IsNullOrEmpty(ThiSinhLog.TongBQMoi)
                                                || !string.IsNullOrEmpty(ThiSinhLog.BQAMoi) || !string.IsNullOrEmpty(ThiSinhLog.BQTMoi) || !string.IsNullOrEmpty(ThiSinhLog.DCMoi) || !string.IsNullOrEmpty(ThiSinhLog.BanMoi) || !string.IsNullOrEmpty(ThiSinhLog.NgaySinhStrMoi)))
                                            {
                                                var temp = InsertThiSinhLog(ThiSinhLog, EnumLog.Update.GetHashCode(), CanBoID, 2);

                                                //update thí sinh thi
                                                SqlParameter[] parms_ts_2 = new SqlParameter[]{
                                                new SqlParameter("@ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("@KyThiID", SqlDbType.Int),
                                                new SqlParameter("@HoTen", SqlDbType.NVarChar),
                                                new SqlParameter("@NgaySinh", SqlDbType.DateTime),
                                                new SqlParameter("@NoiSinh", SqlDbType.NVarChar),
                                                new SqlParameter("@GioiTinh", SqlDbType.Bit),
                                                new SqlParameter("@DanToc", SqlDbType.Int),
                                                new SqlParameter("@CMND", SqlDbType.NVarChar),
                                                new SqlParameter("@SoBaoDanh", SqlDbType.NVarChar),
                                                new SqlParameter("@SoDienThoai", SqlDbType.NVarChar),
                                                new SqlParameter("@DiaChi", SqlDbType.NVarChar),
                                                new SqlParameter("@Lop", SqlDbType.NVarChar),
                                                new SqlParameter("@TruongTHPT", SqlDbType.Int),
                                                new SqlParameter("@LoaiDuThi", SqlDbType.NVarChar),
                                                new SqlParameter("@DonViDKDT", SqlDbType.NVarChar),
                                                new SqlParameter("@XepLoaiHanhKiem", SqlDbType.Int),
                                                new SqlParameter("@XepLoaiHocLuc", SqlDbType.Int),
                                                new SqlParameter("@DiemTBLop12", SqlDbType.Decimal),
                                                new SqlParameter("@DiemKK", SqlDbType.Decimal),
                                                new SqlParameter("@DienXTN", SqlDbType.Int),
                                                new SqlParameter("@HoiDongThi", SqlDbType.Int),
                                                new SqlParameter("@DiemXetTotNghiep", SqlDbType.Decimal),
                                                new SqlParameter("@KetQuaTotNghiep", SqlDbType.NVarChar),
                                                new SqlParameter("@SoHieuBang", SqlDbType.NVarChar),
                                                new SqlParameter("@VaoSoCapBangSo", SqlDbType.NVarChar),
                                                new SqlParameter("@NamThi", SqlDbType.Int),
                                                new SqlParameter("@Do", SqlDbType.NVarChar),
                                                new SqlParameter("@DoThem", SqlDbType.NVarChar),
                                                new SqlParameter("@Hong", SqlDbType.NVarChar),
                                                new SqlParameter("@LaoDong", SqlDbType.NVarChar),
                                                new SqlParameter("@VanHoa", SqlDbType.NVarChar),
                                                new SqlParameter("@RLTT", SqlDbType.NVarChar),
                                                new SqlParameter("@TongSoDiemThi", SqlDbType.Decimal),
                                                new SqlParameter("@NgayCapBang", SqlDbType.DateTime),
                                                new SqlParameter("@DiemXL", SqlDbType.Decimal),
                                                new SqlParameter("@DiemUT", SqlDbType.Decimal),
                                                new SqlParameter("@GhiChu", SqlDbType.NVarChar),
                                                new SqlParameter("@Hang", SqlDbType.NVarChar),
                                                new SqlParameter("@DiemTBCacBaiThi", SqlDbType.Decimal),
                                                new SqlParameter("@DienUuTien", SqlDbType.NVarChar),
                                                new SqlParameter("@DiemTBC", SqlDbType.Decimal),
                                                new SqlParameter("@QueQuan", SqlDbType.NVarChar),
                                                new SqlParameter("@ChungNhanNghe", SqlDbType.NVarChar),
                                                new SqlParameter("@DTConLietSi", SqlDbType.NVarChar),
                                                new SqlParameter("@GiaiTDKT", SqlDbType.NVarChar),
                                                new SqlParameter("@HoiDong", SqlDbType.NVarChar),
                                                new SqlParameter("@MonKN", SqlDbType.NVarChar),
                                                new SqlParameter("@TBCNMonKN", SqlDbType.NVarChar),
                                                new SqlParameter("@DiemThiCu", SqlDbType.NVarChar),
                                                new SqlParameter("@DiemThiMoi", SqlDbType.NVarChar),
                                                new SqlParameter("@TongBQ", SqlDbType.NVarChar),
                                                new SqlParameter("@BQA", SqlDbType.NVarChar),
                                                new SqlParameter("@BQT", SqlDbType.NVarChar),
                                                new SqlParameter("@DC", SqlDbType.NVarChar),
                                                new SqlParameter("@Ban", SqlDbType.NVarChar),
                                                new SqlParameter("@NgaySinhStr", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_DAODUC", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_RLEV", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_DIENKK", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_PHONGTHI", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_DIEMTNC", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_XLTNC", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_TDTCU", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_GIAIHSG", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_GIAIHSGK", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_CHUNGCHINN", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_CHUNGCHITH", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_TONGDIEMMOI", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_BQAMOI", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_BQTMOI", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_SOCAPGIAYCN", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_XLHT", SqlDbType.NVarChar),
                                                new SqlParameter("@BODY_QUOCGIA", SqlDbType.NVarChar),
                                                new SqlParameter("@NgaySinh_Int", SqlDbType.Int),
                                                //new SqlParameter("@NgaySinhFormat", SqlDbType.DateTime),
                                                //new SqlParameter("@HoTenFormat", SqlDbType.NVarChar),
                                            };

                                                parms_ts_2[0].Value = item.ThiSinhID;
                                                parms_ts_2[1].Value = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                                                parms_ts_2[2].Value = item.HoTen ?? Convert.DBNull;
                                                parms_ts_2[3].Value = item.NgaySinh ?? Convert.DBNull;
                                                parms_ts_2[4].Value = item.NoiSinh ?? Convert.DBNull;
                                                parms_ts_2[5].Value = item.GioiTinh ?? Convert.DBNull;
                                                parms_ts_2[6].Value = item.DanToc ?? Convert.DBNull;
                                                parms_ts_2[7].Value = item.CMND ?? Convert.DBNull;
                                                parms_ts_2[8].Value = item.SoBaoDanh ?? Convert.DBNull;
                                                parms_ts_2[9].Value = item.SoDienThoai ?? Convert.DBNull;
                                                parms_ts_2[10].Value = item.DiaChi ?? Convert.DBNull;
                                                parms_ts_2[11].Value = item.Lop ?? Convert.DBNull;
                                                parms_ts_2[12].Value = item.TruongTHPT ?? Convert.DBNull;
                                                parms_ts_2[13].Value = item.LoaiDuThi ?? Convert.DBNull;
                                                parms_ts_2[14].Value = item.DonViDKDT ?? Convert.DBNull;
                                                parms_ts_2[15].Value = item.XepLoaiHanhKiem ?? Convert.DBNull;
                                                parms_ts_2[16].Value = item.XepLoaiHocLuc ?? Convert.DBNull;
                                                parms_ts_2[17].Value = item.DiemTBLop12 ?? Convert.DBNull;
                                                parms_ts_2[18].Value = item.DiemKK ?? Convert.DBNull;
                                                parms_ts_2[19].Value = item.DienXTN ?? Convert.DBNull;
                                                parms_ts_2[20].Value = item.HoiDongThi ?? Convert.DBNull;
                                                parms_ts_2[21].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                                                parms_ts_2[22].Value = item.KetQuaTotNghiep ?? Convert.DBNull;
                                                parms_ts_2[23].Value = item.SoHieuBang ?? Convert.DBNull;
                                                parms_ts_2[24].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                                                parms_ts_2[25].Value = item.NamThi ?? Convert.DBNull;
                                                parms_ts_2[26].Value = item.Do ?? Convert.DBNull;
                                                parms_ts_2[27].Value = item.DoThem ?? Convert.DBNull;
                                                parms_ts_2[28].Value = item.Hong ?? Convert.DBNull;
                                                parms_ts_2[29].Value = item.LaoDong ?? Convert.DBNull;
                                                parms_ts_2[30].Value = item.VanHoa ?? Convert.DBNull;
                                                parms_ts_2[31].Value = item.RLTT ?? Convert.DBNull;
                                                parms_ts_2[32].Value = item.TongSoDiemThi ?? Convert.DBNull;
                                                parms_ts_2[33].Value = item.NgayCapBang ?? Convert.DBNull;
                                                parms_ts_2[34].Value = item.DiemXL ?? Convert.DBNull;
                                                parms_ts_2[35].Value = item.DiemUT ?? Convert.DBNull;
                                                parms_ts_2[36].Value = item.GhiChu ?? Convert.DBNull;
                                                parms_ts_2[37].Value = item.Hang ?? Convert.DBNull;
                                                parms_ts_2[38].Value = item.DiemTBCacBaiThi ?? Convert.DBNull;
                                                parms_ts_2[39].Value = item.DienUuTien ?? Convert.DBNull;
                                                parms_ts_2[40].Value = item.DiemTBC ?? Convert.DBNull;
                                                parms_ts_2[41].Value = item.QueQuan ?? Convert.DBNull;
                                                parms_ts_2[42].Value = item.ChungNhanNghe ?? Convert.DBNull;
                                                parms_ts_2[43].Value = item.DTConLietSi ?? Convert.DBNull;
                                                parms_ts_2[44].Value = item.GiaiTDKT ?? Convert.DBNull;
                                                parms_ts_2[45].Value = item.HoiDong ?? Convert.DBNull;
                                                parms_ts_2[46].Value = item.MonKN ?? Convert.DBNull;
                                                parms_ts_2[47].Value = item.TBCNMonKN ?? Convert.DBNull;
                                                parms_ts_2[48].Value = item.DiemThiCu ?? Convert.DBNull;
                                                parms_ts_2[49].Value = item.DiemThiMoi ?? Convert.DBNull;
                                                parms_ts_2[50].Value = item.TongBQ ?? Convert.DBNull;
                                                parms_ts_2[51].Value = item.BQA ?? Convert.DBNull;
                                                parms_ts_2[52].Value = item.BQT ?? Convert.DBNull;
                                                parms_ts_2[53].Value = item.DC ?? Convert.DBNull;
                                                parms_ts_2[54].Value = item.Ban ?? Convert.DBNull;
                                                parms_ts_2[55].Value = item.NgaySinhStr ?? Convert.DBNull;
                                                parms_ts_2[56].Value = item.BODY_DAODUC ?? Convert.DBNull;
                                                parms_ts_2[57].Value = item.BODY_RLEV ?? Convert.DBNull;
                                                parms_ts_2[58].Value = item.BODY_DIENKK ?? Convert.DBNull;
                                                parms_ts_2[59].Value = item.BODY_PHONGTHI ?? Convert.DBNull;
                                                parms_ts_2[60].Value = item.BODY_DIEMTNC ?? Convert.DBNull;
                                                parms_ts_2[61].Value = item.BODY_XLTNC ?? Convert.DBNull;
                                                parms_ts_2[62].Value = item.BODY_TDTCU ?? Convert.DBNull;
                                                parms_ts_2[63].Value = item.BODY_GIAIHSG ?? Convert.DBNull;
                                                parms_ts_2[64].Value = item.BODY_GIAIHSGK ?? Convert.DBNull;
                                                parms_ts_2[65].Value = item.BODY_CHUNGCHINN ?? Convert.DBNull;
                                                parms_ts_2[66].Value = item.BODY_CHUNGCHITH ?? Convert.DBNull;
                                                parms_ts_2[67].Value = item.BODY_TONGDIEMMOI ?? Convert.DBNull;
                                                parms_ts_2[68].Value = item.BODY_BQAMOI ?? Convert.DBNull;
                                                parms_ts_2[69].Value = item.BODY_BQTMOI ?? Convert.DBNull;
                                                parms_ts_2[70].Value = item.BODY_SOCAPGIAYCN ?? Convert.DBNull;
                                                parms_ts_2[71].Value = item.BODY_XLHT ?? Convert.DBNull;
                                                parms_ts_2[72].Value = item.BODY_QUOCGIA ?? Convert.DBNull;                                  
                                                parms_ts_2[73].Value = Utils.ConvertNgaySinhStrToInt(item.NgaySinhStr);
                                                //parms_ts_2[74].Value = Utils.ConvertNgaySinh(item.NgaySinhStr) ?? Convert.DBNull;
                                                //parms_ts_2[75].Value = Utils.NonUnicode(item.HoTen) ?? Convert.DBNull;
                                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_ThongTinThiSinh_Update_New", parms_ts_2);

                                            }

                                        }
                                        else
                                        {
                                            ThongTinThiSinhLog ThiSinhLog = new ThongTinThiSinhLog();
                                            ThiSinhLog.ListThongTinDiemThiMoi = new List<ThongTinDiemThiLog>();
                                            //insert thí sinh thi
                                            SqlParameter[] parms_ts = new SqlParameter[]{
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("KyThiID", SqlDbType.Int),
                                                new SqlParameter("HoTen", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinh", SqlDbType.DateTime),
                                                new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                                                new SqlParameter("GioiTinh", SqlDbType.Bit),
                                                new SqlParameter("DanToc", SqlDbType.Int),
                                                new SqlParameter("CMND", SqlDbType.NVarChar),
                                                new SqlParameter("SoBaoDanh", SqlDbType.NVarChar),
                                                new SqlParameter("SoDienThoai", SqlDbType.NVarChar),
                                                new SqlParameter("DiaChi", SqlDbType.NVarChar),
                                                new SqlParameter("Lop", SqlDbType.NVarChar),
                                                new SqlParameter("TruongTHPT", SqlDbType.Int),
                                                new SqlParameter("LoaiDuThi", SqlDbType.NVarChar),
                                                new SqlParameter("DonViDKDT", SqlDbType.NVarChar),
                                                new SqlParameter("XepLoaiHanhKiem", SqlDbType.Int),
                                                new SqlParameter("XepLoaiHocLuc", SqlDbType.Int),
                                                new SqlParameter("DiemTBLop12", SqlDbType.Decimal),
                                                new SqlParameter("DiemKK", SqlDbType.Decimal),
                                                new SqlParameter("DienXTN", SqlDbType.Int),
                                                new SqlParameter("HoiDongThi", SqlDbType.Int),
                                                new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal),
                                                new SqlParameter("KetQuaTotNghiep", SqlDbType.NVarChar),
                                                new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                                                new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                                                new SqlParameter("NamThi", SqlDbType.Int),
                                                new SqlParameter("Do", SqlDbType.NVarChar),
                                                new SqlParameter("DoThem", SqlDbType.NVarChar),
                                                new SqlParameter("Hong", SqlDbType.NVarChar),
                                                new SqlParameter("LaoDong", SqlDbType.NVarChar),
                                                new SqlParameter("VanHoa", SqlDbType.NVarChar),
                                                new SqlParameter("RLTT", SqlDbType.NVarChar),
                                                new SqlParameter("TongSoDiemThi", SqlDbType.Decimal),
                                                new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                                                new SqlParameter("DiemXL", SqlDbType.Decimal),
                                                new SqlParameter("DiemUT", SqlDbType.Decimal),
                                                new SqlParameter("GhiChu", SqlDbType.NVarChar),
                                                new SqlParameter("Hang", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBCacBaiThi", SqlDbType.Decimal),
                                                new SqlParameter("DienUuTien", SqlDbType.NVarChar),
                                                new SqlParameter("DiemTBC", SqlDbType.Decimal),
                                                 new SqlParameter("QueQuan", SqlDbType.NVarChar),
                                                new SqlParameter("ChungNhanNghe", SqlDbType.NVarChar),
                                                new SqlParameter("DTConLietSi", SqlDbType.NVarChar),
                                                new SqlParameter("GiaiTDKT", SqlDbType.NVarChar),
                                                new SqlParameter("HoiDong", SqlDbType.NVarChar),
                                                new SqlParameter("MonKN", SqlDbType.NVarChar),
                                                new SqlParameter("TBCNMonKN", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiCu", SqlDbType.NVarChar),
                                                new SqlParameter("DiemThiMoi", SqlDbType.NVarChar),
                                                new SqlParameter("TongBQ", SqlDbType.NVarChar),
                                                  new SqlParameter("BQA", SqlDbType.NVarChar),
                                                new SqlParameter("BQT", SqlDbType.NVarChar),
                                                new SqlParameter("DC", SqlDbType.NVarChar),
                                                new SqlParameter("Ban", SqlDbType.NVarChar),
                                                new SqlParameter("NgaySinhStr", SqlDbType.NVarChar),

                                                new SqlParameter("BODY_DAODUC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_RLEV", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIENKK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_PHONGTHI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_DIEMTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLTNC", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TDTCU", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSG", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_GIAIHSGK", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHINN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_CHUNGCHITH", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_TONGDIEMMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQAMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_BQTMOI", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_SOCAPGIAYCN", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_XLHT", SqlDbType.NVarChar),
                                                new SqlParameter("BODY_QUOCGIA", SqlDbType.NVarChar),
                                            };
                                            parms_ts[0].Direction = ParameterDirection.Output;
                                            parms_ts[0].Size = 8;
                                            parms_ts[1].Value = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                                            parms_ts[2].Value = item.HoTen ?? Convert.DBNull;
                                            parms_ts[3].Value = item.NgaySinh ?? Convert.DBNull;
                                            parms_ts[4].Value = item.NoiSinh ?? Convert.DBNull;
                                            parms_ts[5].Value = item.GioiTinh ?? Convert.DBNull;
                                            parms_ts[6].Value = item.DanToc ?? Convert.DBNull;
                                            parms_ts[7].Value = item.CMND ?? Convert.DBNull;
                                            parms_ts[8].Value = item.SoBaoDanh ?? Convert.DBNull;
                                            parms_ts[9].Value = item.SoDienThoai ?? Convert.DBNull;
                                            parms_ts[10].Value = item.DiaChi ?? Convert.DBNull;
                                            parms_ts[11].Value = item.Lop ?? Convert.DBNull;
                                            parms_ts[12].Value = item.TruongTHPT ?? Convert.DBNull;
                                            parms_ts[13].Value = item.LoaiDuThi ?? Convert.DBNull;
                                            parms_ts[14].Value = item.DonViDKDT ?? Convert.DBNull;
                                            parms_ts[15].Value = item.XepLoaiHanhKiem ?? Convert.DBNull;
                                            parms_ts[16].Value = item.XepLoaiHocLuc ?? Convert.DBNull;
                                            parms_ts[17].Value = item.DiemTBLop12 ?? Convert.DBNull;
                                            parms_ts[18].Value = item.DiemKK ?? Convert.DBNull;
                                            parms_ts[19].Value = item.DienXTN ?? Convert.DBNull;
                                            parms_ts[20].Value = item.HoiDongThi ?? Convert.DBNull;
                                            parms_ts[21].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                                            parms_ts[22].Value = item.KetQuaTotNghiep ?? Convert.DBNull;
                                            parms_ts[23].Value = item.SoHieuBang ?? Convert.DBNull;
                                            parms_ts[24].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                                            parms_ts[25].Value = item.NamThi ?? Convert.DBNull;
                                            parms_ts[26].Value = item.Do ?? Convert.DBNull;
                                            parms_ts[27].Value = item.DoThem ?? Convert.DBNull;
                                            parms_ts[28].Value = item.Hong ?? Convert.DBNull;
                                            parms_ts[29].Value = item.LaoDong ?? Convert.DBNull;
                                            parms_ts[30].Value = item.VanHoa ?? Convert.DBNull;
                                            parms_ts[31].Value = item.RLTT ?? Convert.DBNull;
                                            parms_ts[32].Value = item.TongSoDiemThi ?? Convert.DBNull;
                                            parms_ts[33].Value = item.NgayCapBang ?? Convert.DBNull;
                                            parms_ts[34].Value = item.DiemXL ?? Convert.DBNull;
                                            parms_ts[35].Value = item.DiemUT ?? Convert.DBNull;
                                            parms_ts[36].Value = item.GhiChu ?? Convert.DBNull;
                                            parms_ts[37].Value = item.Hang ?? Convert.DBNull;
                                            parms_ts[38].Value = item.DiemTBCacBaiThi ?? Convert.DBNull;
                                            parms_ts[39].Value = item.DienUuTien ?? Convert.DBNull;
                                            parms_ts[40].Value = item.DiemTBC ?? Convert.DBNull;
                                            parms_ts[41].Value = item.QueQuan ?? Convert.DBNull;
                                            parms_ts[42].Value = item.ChungNhanNghe ?? Convert.DBNull;
                                            parms_ts[43].Value = item.DTConLietSi ?? Convert.DBNull;
                                            parms_ts[44].Value = item.GiaiTDKT ?? Convert.DBNull;
                                            parms_ts[45].Value = item.HoiDong ?? Convert.DBNull;
                                            parms_ts[46].Value = item.MonKN ?? Convert.DBNull;
                                            parms_ts[47].Value = item.TBCNMonKN ?? Convert.DBNull;
                                            parms_ts[48].Value = item.DiemThiCu ?? Convert.DBNull;
                                            parms_ts[49].Value = item.DiemThiMoi ?? Convert.DBNull;
                                            parms_ts[50].Value = item.TongBQ ?? Convert.DBNull;
                                            parms_ts[51].Value = item.BQA ?? Convert.DBNull;
                                            parms_ts[52].Value = item.BQT ?? Convert.DBNull;
                                            parms_ts[53].Value = item.DC ?? Convert.DBNull;
                                            parms_ts[54].Value = item.Ban ?? Convert.DBNull;
                                            parms_ts[55].Value = item.NgaySinhStr ?? Convert.DBNull;

                                            parms_ts[56].Value = item.BODY_DAODUC ?? Convert.DBNull;
                                            parms_ts[57].Value = item.BODY_RLEV ?? Convert.DBNull;
                                            parms_ts[58].Value = item.BODY_DIENKK ?? Convert.DBNull;
                                            parms_ts[59].Value = item.BODY_PHONGTHI ?? Convert.DBNull;
                                            parms_ts[60].Value = item.BODY_DIEMTNC ?? Convert.DBNull;
                                            parms_ts[61].Value = item.BODY_XLTNC ?? Convert.DBNull;
                                            parms_ts[62].Value = item.BODY_TDTCU ?? Convert.DBNull;
                                            parms_ts[63].Value = item.BODY_GIAIHSG ?? Convert.DBNull;
                                            parms_ts[64].Value = item.BODY_GIAIHSGK ?? Convert.DBNull;
                                            parms_ts[65].Value = item.BODY_CHUNGCHINN ?? Convert.DBNull;
                                            parms_ts[66].Value = item.BODY_CHUNGCHITH ?? Convert.DBNull;
                                            parms_ts[67].Value = item.BODY_TONGDIEMMOI ?? Convert.DBNull;
                                            parms_ts[68].Value = item.BODY_BQAMOI ?? Convert.DBNull;
                                            parms_ts[69].Value = item.BODY_BQTMOI ?? Convert.DBNull;
                                            parms_ts[70].Value = item.BODY_SOCAPGIAYCN ?? Convert.DBNull;
                                            parms_ts[71].Value = item.BODY_XLHT ?? Convert.DBNull;
                                            parms_ts[72].Value = item.BODY_QUOCGIA ?? Convert.DBNull;

                                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_ThongTinThiSinh_Insert", parms_ts);
                                            int ThiSinhID = Utils.ConvertToInt32(parms_ts[0].Value, 0);
                                            if (ThiSinhID > 0 && item.ListThongTinDiemThi != null && item.ListThongTinDiemThi.Count > 0)
                                            {
                                                foreach (var DiemThi in item.ListThongTinDiemThi)
                                                {
                                                    SqlParameter[] parms_dt = new SqlParameter[]{
                                                    new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                    new SqlParameter("MonThiID", SqlDbType.Int),
                                                    new SqlParameter("Diem", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                    new SqlParameter("NhomID", SqlDbType.Int),
                                                    };

                                                    parms_dt[0].Value = ThiSinhID;
                                                    parms_dt[1].Value = DiemThi.MonThiID ?? Convert.DBNull;
                                                    //parms_dt[2].Value = DiemThi.Diem ?? Convert.DBNull;
                                                    parms_dt[2].Value = Utils.ConvertToNullableDecimal(DiemThi.DiemBaiToHop, null) ?? Convert.DBNull;
                                                    parms_dt[3].Value = DiemThi.DiemBaiToHop ?? Convert.DBNull;
                                                    parms_dt[4].Value = DiemThi.NhomID ?? Convert.DBNull;

                                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinDiemThi_Insert_New", parms_dt);

                                                    ThongTinDiemThiLog DiemThiLog = new ThongTinDiemThiLog();
                                                    DiemThiLog.MonThiID = DiemThi.MonThiID;
                                                    DiemThiLog.NhomID = DiemThi.NhomID;
                                                    DiemThiLog.DiemMoi = DiemThi.Diem;
                                                    DiemThiLog.DiemBaiToHopMoi = DiemThi.DiemBaiToHop;
                                                    DiemThiLog.ThiSinhID = ThiSinhID;
                                                    if (DiemThiLog.DiemMoi != null || !string.IsNullOrEmpty(DiemThiLog.DiemBaiToHopMoi))
                                                    {
                                                        ThiSinhLog.ListThongTinDiemThiMoi.Add(DiemThiLog);
                                                    }

                                                }
                                            }
                                            ThiSinhLog.ThiSinhID = ThiSinhID;
                                            ThiSinhLog.KyThiCuID = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                                            ThiSinhLog.HoTenMoi = item.HoTen;
                                            ThiSinhLog.NgaySinhMoi = item.NgaySinh;
                                            ThiSinhLog.NoiSinhMoi = item.NoiSinh;
                                            ThiSinhLog.GioiTinhMoi = item.GioiTinh;
                                            ThiSinhLog.DanTocMoi = item.DanToc;
                                            ThiSinhLog.CMNDMoi = item.CMND;
                                            ThiSinhLog.SoBaoDanhMoi = item.SoBaoDanh;
                                            ThiSinhLog.SoDienThoaiMoi = item.SoDienThoai;
                                            ThiSinhLog.DiaChiMoi = item.DiaChi;
                                            ThiSinhLog.LopMoi = item.Lop;
                                            ThiSinhLog.TruongTHPTMoi = item.TruongTHPT;
                                            ThiSinhLog.LoaiDuThiMoi = item.LoaiDuThi;
                                            ThiSinhLog.DonViDKDTMoi = item.DonViDKDT;
                                            ThiSinhLog.XepLoaiHanhKiemMoi = item.XepLoaiHanhKiem;
                                            ThiSinhLog.XepLoaiHocLucMoi = item.XepLoaiHocLuc;
                                            ThiSinhLog.DiemTBLop12Moi = item.DiemTBLop12;
                                            ThiSinhLog.DiemKKMoi = item.DiemKK;
                                            ThiSinhLog.DienXTNMoi = item.DienXTN;
                                            ThiSinhLog.HoiDongThiMoi = item.HoiDongThi;
                                            ThiSinhLog.DiemXetTotNghiepMoi = item.DiemXetTotNghiep;
                                            ThiSinhLog.KetQuaTotNghiepMoi = item.KetQuaTotNghiep;
                                            ThiSinhLog.SoHieuBangMoi = item.SoHieuBang;
                                            ThiSinhLog.VaoSoCapBangSoMoi = item.VaoSoCapBangSo;
                                            ThiSinhLog.NamThiMoi = item.NamThi;
                                            ThiSinhLog.DoMoi = item.Do;
                                            ThiSinhLog.DoThemMoi = item.DoThem;
                                            ThiSinhLog.HongMoi = item.Hong;
                                            ThiSinhLog.LaoDongMoi = item.LaoDong;
                                            ThiSinhLog.VanHoaMoi = item.VanHoa;
                                            ThiSinhLog.RLTTMoi = item.RLTT;
                                            ThiSinhLog.TongSoDiemThiMoi = item.TongSoDiemThi;
                                            ThiSinhLog.NgayCapBangMoi = item.NgayCapBang;
                                            ThiSinhLog.DiemXLMoi = item.DiemXL;
                                            ThiSinhLog.DiemUTMoi = item.DiemUT;
                                            ThiSinhLog.GhiChuMoi = item.GhiChu;
                                            ThiSinhLog.HangMoi = item.Hang;
                                            ThiSinhLog.DiemTBCacBaiThiMoi = item.DiemTBCacBaiThi;
                                            ThiSinhLog.DienUuTienMoi = item.DienUuTien;
                                            ThiSinhLog.DiemTBCMoi = item.DiemTBC;

                                            var temp = InsertThiSinhLog(ThiSinhLog, EnumLog.Insert.GetHashCode(), CanBoID, 1);
                                        }

                                    }
                                }
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Update_Success("dữ liệu điểm thi");
                                return Result;
                            }
                            catch
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh item, int? CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (item == null)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin thí sinh không được để trống";
                    return Result;
                }
                else
                {
                    //update thí sinh thi
                    SqlParameter[] parms_ts = new SqlParameter[]{
                         new SqlParameter("ThiSinhID", SqlDbType.Int),
                         new SqlParameter("KyThiID", SqlDbType.Int),    
                         new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal), 
                         new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                         new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                         new SqlParameter("NamThi", SqlDbType.Int),                    
                         new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                         new SqlParameter("Hang", SqlDbType.NVarChar),
                    };

                    parms_ts[0].Value = item.ThiSinhID;
                    parms_ts[1].Value = item.KyThiID ?? Convert.DBNull;
                    parms_ts[2].Value = item.DiemXetTotNghiep ?? Convert.DBNull;    
                    parms_ts[3].Value = item.SoHieuBang ?? Convert.DBNull;
                    parms_ts[4].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                    parms_ts[5].Value = item.NamTotNghiep ?? Convert.DBNull;
                    parms_ts[6].Value = item.NgayCapBang ?? Convert.DBNull;
                    parms_ts[7].Value = item.Hang ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                
                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_UpdateThongTinCapBang", parms_ts);

                                trans.Commit();
                                Result.Status = 1;
                                Result.Message = ConstantLogMessage.Alert_Update_Success("thông tin thí sinh");
                                return Result;
                            }
                            catch
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public DuLieuDiemThiModel GetByID_Old(int KyThiID)
        {
            List<DuLieuDiemThiModel> Result = new List<DuLieuDiemThiModel>();
            List<ChiTietDuLieuDiemThiModel> chiTiets = new List<ChiTietDuLieuDiemThiModel>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@KyThiID",SqlDbType.Int)
            };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietDuLieuDiemThiModel info = new ChiTietDuLieuDiemThiModel();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.HoiDongChamThiID = Utils.ConvertToInt32(dr["HoiDongChamThiID"], 0);
                        info.TenHoiDongChamThi = Utils.ConvertToString(dr["TenHoiDongChamThi"], string.Empty);
                        info.HoiDongGiamThiID = Utils.ConvertToInt32(dr["HoiDongGiamThiID"], 0);
                        info.TenHoiDongGiamThi = Utils.ConvertToString(dr["TenHoiDongGiamThi"], string.Empty);
                        info.HoiDongGiamKhaoID = Utils.ConvertToInt32(dr["HoiDongGiamKhaoID"], 0);
                        info.TenHoiDongGiamKhao = Utils.ConvertToString(dr["TenHoiDongGiamKhao"], string.Empty);
                        info.HoiDongCoiThiID = Utils.ConvertToInt32(dr["HoiDongCoiThiID"], 0);
                        info.TenHoiDongCoiThi = Utils.ConvertToString(dr["TenHoiDongCoiThi"], string.Empty);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.SBDDau = Utils.ConvertToString(dr["SBDDau"], string.Empty);
                        info.SBDCuoi = Utils.ConvertToString(dr["SBDCuoi"], string.Empty);
                        info.NguoiDocDiem = Utils.ConvertToString(dr["NguoiDocDiem"], string.Empty);
                        info.NguoiNhapVaInDiem = Utils.ConvertToString(dr["NguoiNhapVaInDiem"], string.Empty);
                        info.NguoiDocSoatBanGhi = Utils.ConvertToString(dr["NguoiDocSoatBanGhi"], string.Empty);
                        info.NgayDuyetBangDiem = Utils.ConvertToNullableDateTime(dr["NgayDuyetBangDiem"], null);
                        info.NgayDuyetCham = Utils.ConvertToNullableDateTime(dr["NgayDuyetCham"], null);
                        info.NgaySoDuyet = Utils.ConvertToNullableDateTime(dr["NgaySoDuyet"], null);
                        info.CanBoXetDuyet = Utils.ConvertToString(dr["CanBoXetDuyet"], string.Empty);
                        info.CanBoSoKT = Utils.ConvertToString(dr["CanBoSoKT"], string.Empty);
                        info.ChuTichHoiDong = Utils.ConvertToString(dr["ChuTichHoiDong"], string.Empty);
                        info.GiamDocSo = Utils.ConvertToString(dr["GiamDocSo"], string.Empty);
                        info.SoThiSinhDuThi = Utils.ConvertToNullableInt32(dr["SoThiSinhDuThi"], null);
                        info.DuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["DuocCongNhanTotNghiep"], null);
                        info.KhongDuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["KhongDuocCongNhanTotNghiep"], null);
                        info.TNLoaiGioi = Utils.ConvertToNullableInt32(dr["TNLoaiGioi"], null);
                        info.TNLoaiKha = Utils.ConvertToNullableInt32(dr["TNLoaiKha"], null);
                        info.TNLoaiTB = Utils.ConvertToNullableInt32(dr["TNLoaiTB"], null);
                        info.DienTotNghiep2 = Utils.ConvertToNullableInt32(dr["DienTotNghiep2"], null);
                        info.DienTotNghiep3 = Utils.ConvertToNullableInt32(dr["DienTotNghiep3"], null);
                        info.TotNghiepDienA = Utils.ConvertToNullableInt32(dr["TotNghiepDienA"], null);
                        info.TotNghiepDienB = Utils.ConvertToNullableInt32(dr["TotNghiepDienB"], null);
                        info.DienTotNghiep4_5 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_5"], null);
                        info.DienTotNghiep4_75 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_75"], null);
                        info.GhiChuKyThi = Utils.ConvertToString(dr["GhiChuKyThi"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        info.TrangThaiKyThi = Utils.ConvertToNullableInt32(dr["TrangThaiKyThi"], null);
                        info.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        info.ThuKy = Utils.ConvertToString(dr["ThuKy"], string.Empty);
                        info.ChanhChuKhao = Utils.ConvertToString(dr["ChanhChuKhao"], string.Empty);
                        info.PhoChuKhao = Utils.ConvertToString(dr["PhoChuKhao"], string.Empty);
                        info.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToNullableInt32(dr["SoTrang"], null);
                        info.TongSoThiSinh = Utils.ConvertToNullableInt32(dr["TongSoThiSinh"], null);
                        info.Tinh = Utils.ConvertToString(dr["Tinh"], string.Empty);
                        info.ToTruongHoiPhach = Utils.ConvertToString(dr["ToTruongHoiPhach"], string.Empty);
                        info.ChuTichHoiDongCoiThi = Utils.ConvertToString(dr["ChuTichHoiDongCoiThi"], string.Empty);
                        info.HieuTruong = Utils.ConvertToString(dr["HieuTruong"], string.Empty);
                        info.ChuTichHoiDongChamThi = Utils.ConvertToString(dr["ChuTichHoiDongChamThi"], string.Empty);

                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToNullableInt32(dr["TruongTHPT"], null);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        info.LaoDong = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        info.VanHoa = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        info.RLTT = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        info.XepLoaiHanhKiem = Utils.ConvertToNullableInt32(dr["XepLoaiHanhKiem"], null);
                        info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.Do = Utils.ConvertToString(dr["Do"], string.Empty);
                        info.DoThem = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        info.Hong = Utils.ConvertToString(dr["Hong"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToNullableInt32(dr["XepLoaiHocLuc"], null);
                        info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToNullableDecimal(dr["DiemTBLop12"], null);
                        info.DiemXL = Utils.ConvertToNullableDecimal(dr["DiemXL"], null);
                        info.DiemUT = Utils.ConvertToNullableDecimal(dr["DiemUT"], null);
                        info.DiemKK = Utils.ConvertToNullableDecimal(dr["DiemKK"], null);
                        info.DienXTN = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        info.HoiDongThi = Utils.ConvertToNullableInt32(dr["HoiDongThi"], null);
                        info.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        info.TrangThaiThiSinh = Utils.ConvertToInt32(dr["TrangThaiThiSinh"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.TongSoDiemThi = Utils.ConvertToNullableDecimal(dr["TongSoDiemThi"], null);
                        info.GhiChuThiSinh = Utils.ConvertToString(dr["GhiChuThiSinh"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(dr["DiemTBCacBaiThi"], null);
                        info.DienUuTien = Utils.ConvertToString(dr["DienUuTien"], string.Empty);
                        info.DiemTBC = Utils.ConvertToNullableDecimal(dr["DiemTBC"], null);

                        info.DiemThiID = Utils.ConvertToInt32(dr["DiemThiID"], 0);
                        info.NhomID = Utils.ConvertToInt32(dr["NhomID"], 0);
                        info.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                        info.Diem = Utils.ConvertToNullableDecimal(dr["Diem"], null);
                        info.DiemBaiToHop = Utils.ConvertToString(dr["DiemBaiToHop"], string.Empty);
                        chiTiets.Add(info);
                    }
                    dr.Close();
                }

                if (chiTiets.Count > 0)
                {
                    Result = (from m in chiTiets
                              group m by m.KyThiID into ctt
                              from item in ctt
                              select new DuLieuDiemThiModel()
                              {
                                  ThongTinToChucThi = (from i in chiTiets.Where(x => x.KyThiID > 0).ToList().GroupBy(x => x.KyThiID)
                                                       select new ThongTinToChucThi()
                                                       {
                                                           KyThiID = i.FirstOrDefault().KyThiID,
                                                           TenKyThi = i.FirstOrDefault().TenKyThi,
                                                           HoiDongThiID = i.FirstOrDefault().HoiDongThiID,
                                                           TenHoiDongThi = i.FirstOrDefault().TenHoiDongThi,
                                                           HoiDongChamThiID = i.FirstOrDefault().HoiDongChamThiID,
                                                           TenHoiDongChamThi = i.FirstOrDefault().TenHoiDongChamThi,
                                                           HoiDongGiamThiID = i.FirstOrDefault().HoiDongGiamThiID,
                                                           TenHoiDongGiamThi = i.FirstOrDefault().TenHoiDongGiamThi,
                                                           HoiDongGiamKhaoID = i.FirstOrDefault().HoiDongGiamKhaoID,
                                                           TenHoiDongGiamKhao = i.FirstOrDefault().TenHoiDongGiamKhao,
                                                           HoiDongCoiThiID = i.FirstOrDefault().HoiDongCoiThiID,
                                                           TenHoiDongCoiThi = i.FirstOrDefault().TenHoiDongCoiThi,
                                                           KhoaThiID = i.FirstOrDefault().KhoaThiID,
                                                           SBDDau = i.FirstOrDefault().SBDDau,
                                                           SBDCuoi = i.FirstOrDefault().SBDCuoi,
                                                           NguoiDocDiem = i.FirstOrDefault().NguoiDocDiem,
                                                           NguoiNhapVaInDiem = i.FirstOrDefault().NguoiNhapVaInDiem,
                                                           NguoiDocSoatBanGhi = i.FirstOrDefault().NguoiDocSoatBanGhi,
                                                           NgayDuyetBangDiem = i.FirstOrDefault().NgayDuyetBangDiem,
                                                           NgayDuyetCham = i.FirstOrDefault().NgayDuyetCham,
                                                           NgaySoDuyet = i.FirstOrDefault().NgaySoDuyet,
                                                           CanBoXetDuyet = i.FirstOrDefault().CanBoXetDuyet,
                                                           CanBoSoKT = i.FirstOrDefault().CanBoSoKT,
                                                           ChuTichHoiDong = i.FirstOrDefault().ChuTichHoiDong,
                                                           GiamDocSo = i.FirstOrDefault().GiamDocSo,
                                                           SoThiSinhDuThi = i.FirstOrDefault().SoThiSinhDuThi,
                                                           DuocCongNhanTotNghiep = i.FirstOrDefault().DuocCongNhanTotNghiep,
                                                           KhongDuocCongNhanTotNghiep = i.FirstOrDefault().KhongDuocCongNhanTotNghiep,
                                                           TNLoaiGioi = i.FirstOrDefault().TNLoaiGioi,
                                                           TNLoaiKha = i.FirstOrDefault().TNLoaiKha,
                                                           TNLoaiTB = i.FirstOrDefault().TNLoaiTB,
                                                           DienTotNghiep2 = i.FirstOrDefault().DienTotNghiep2,
                                                           DienTotNghiep3 = i.FirstOrDefault().DienTotNghiep3,
                                                           TotNghiepDienA = i.FirstOrDefault().TotNghiepDienA,
                                                           TotNghiepDienB = i.FirstOrDefault().TotNghiepDienB,
                                                           DienTotNghiep4_5 = i.FirstOrDefault().DienTotNghiep4_5,
                                                           DienTotNghiep4_75 = i.FirstOrDefault().DienTotNghiep4_75,
                                                           TrangThai = i.FirstOrDefault().TrangThaiKyThi,
                                                           GhiChu = i.FirstOrDefault().GhiChuKyThi,
                                                           PhongThi = i.FirstOrDefault().PhongThi,
                                                           Ban = i.FirstOrDefault().Ban,
                                                           MauPhieuID = i.FirstOrDefault().MauPhieuID,
                                                           ThuKy = i.FirstOrDefault().ThuKy,
                                                           ChanhChuKhao = i.FirstOrDefault().ChanhChuKhao,
                                                           PhoChuKhao = i.FirstOrDefault().PhoChuKhao,
                                                           KhoaThiNgay = i.FirstOrDefault().KhoaThiNgay,
                                                           SoQuyen = i.FirstOrDefault().SoQuyen,
                                                           SoTrang = i.FirstOrDefault().SoTrang,
                                                           TongSoThiSinh = i.FirstOrDefault().TongSoThiSinh,
                                                           Tinh = i.FirstOrDefault().Tinh,
                                                           ToTruongHoiPhach = i.FirstOrDefault().ToTruongHoiPhach,
                                                           ChuTichHoiDongCoiThi = i.FirstOrDefault().ChuTichHoiDongCoiThi,
                                                           HieuTruong = i.FirstOrDefault().HieuTruong,
                                                           ChuTichHoiDongChamThi = i.FirstOrDefault().ChuTichHoiDongChamThi
                                                       }
                                  ).FirstOrDefault(),
                                  ThongTinThiSinh = (from i in chiTiets.Where(x => x.ThiSinhID > 0).ToList().GroupBy(x => x.ThiSinhID)
                                                     select new ThongTinThiSinh()
                                                     {
                                                         ThiSinhID = i.FirstOrDefault().ThiSinhID,
                                                         KyThiID = i.FirstOrDefault().KyThiID,
                                                         HoTen = i.FirstOrDefault().HoTen,
                                                         NgaySinh = i.FirstOrDefault().NgaySinh,
                                                         NoiSinh = i.FirstOrDefault().NoiSinh,
                                                         GioiTinh = i.FirstOrDefault().GioiTinh,
                                                         DanToc = i.FirstOrDefault().DanToc,
                                                         CMND = i.FirstOrDefault().CMND,
                                                         SoBaoDanh = i.FirstOrDefault().SoBaoDanh,
                                                         SoDienThoai = i.FirstOrDefault().SoDienThoai,
                                                         DiaChi = i.FirstOrDefault().DiaChi,
                                                         Lop = i.FirstOrDefault().Lop,
                                                         TruongTHPT = i.FirstOrDefault().TruongTHPT,
                                                         TenTruongTHPT = i.FirstOrDefault().TenTruongTHPT,
                                                         LoaiDuThi = i.FirstOrDefault().LoaiDuThi,
                                                         DonViDKDT = i.FirstOrDefault().DonViDKDT,
                                                         LaoDong = i.FirstOrDefault().LaoDong,
                                                         VanHoa = i.FirstOrDefault().VanHoa,
                                                         RLTT = i.FirstOrDefault().RLTT,
                                                         XepLoaiHanhKiem = i.FirstOrDefault().XepLoaiHanhKiem,
                                                         XepLoaiHanhKiemStr = i.FirstOrDefault().XepLoaiHanhKiemStr,
                                                         Do = i.FirstOrDefault().Do,
                                                         DoThem = i.FirstOrDefault().DoThem,
                                                         Hong = i.FirstOrDefault().Hong,
                                                         XepLoaiHocLuc = i.FirstOrDefault().XepLoaiHocLuc,
                                                         XepLoaiHocLucStr = i.FirstOrDefault().XepLoaiHocLucStr,
                                                         DiemTBLop12 = i.FirstOrDefault().DiemTBLop12,
                                                         DiemXL = i.FirstOrDefault().DiemXL,
                                                         DiemUT = i.FirstOrDefault().DiemUT,
                                                         DiemKK = i.FirstOrDefault().DiemKK,
                                                         DienXTN = i.FirstOrDefault().DienXTN,
                                                         HoiDongThi = i.FirstOrDefault().HoiDongThi,
                                                         DiemXetTotNghiep = i.FirstOrDefault().DiemXetTotNghiep,
                                                         KetQuaTotNghiep = i.FirstOrDefault().KetQuaTotNghiep,
                                                         SoHieuBang = i.FirstOrDefault().SoHieuBang,
                                                         VaoSoCapBangSo = i.FirstOrDefault().VaoSoCapBangSo,
                                                         NgayCapBang = i.FirstOrDefault().NgayCapBang,
                                                         NamThi = i.FirstOrDefault().NamThi,
                                                         TrangThai = i.FirstOrDefault().TrangThaiThiSinh,
                                                         TrangThaiCapBang = i.FirstOrDefault().TrangThaiCapBang,
                                                         TongSoDiemThi = i.FirstOrDefault().TongSoDiemThi,
                                                         GhiChu = i.FirstOrDefault().GhiChuThiSinh,
                                                         Hang = i.FirstOrDefault().Hang,
                                                         DiemTBCacBaiThi = i.FirstOrDefault().DiemTBCacBaiThi,
                                                         DienUuTien = i.FirstOrDefault().DienUuTien,
                                                         DiemTBC = i.FirstOrDefault().DiemTBC,
                                                         ListThongTinDiemThi = (from j in chiTiets.Where(x => x.DiemThiID > 0 && x.ThiSinhID == i.FirstOrDefault().ThiSinhID).ToList().GroupBy(x => x.DiemThiID)
                                                                                select new ThongTinDiemThi()
                                                                                {
                                                                                    DiemThiID = j.FirstOrDefault().DiemThiID,
                                                                                    ThiSinhID = j.FirstOrDefault().ThiSinhID,
                                                                                    MonThiID = j.FirstOrDefault().MonThiID,
                                                                                    Diem = j.FirstOrDefault().Diem,
                                                                                    DiemBaiToHop = j.FirstOrDefault().DiemBaiToHop,
                                                                                    NhomID = j.FirstOrDefault().NhomID,
                                                                                }
                                                               ).ToList(),
                                                     }
                                  ).ToList(),
                              }
                            ).ToList();
                }

                return Result.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }
        public DuLieuDiemThiModel GetByID(int KyThiID)
        {
            DuLieuDiemThiModel DuLieuDiemThiModel = new DuLieuDiemThiModel();
            DuLieuDiemThiModel.ThongTinToChucThi = new ThongTinToChucThi();
            DuLieuDiemThiModel.ThongTinThiSinh = new List<ThongTinThiSinh>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@KyThiID",SqlDbType.Int)
            };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_GetByKyThiID", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinToChucThi info = new ThongTinToChucThi();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.HoiDongChamThiID = Utils.ConvertToInt32(dr["HoiDongChamThiID"], 0);
                        info.TenHoiDongChamThi = Utils.ConvertToString(dr["TenHoiDongChamThi"], string.Empty);
                        info.HoiDongGiamThiID = Utils.ConvertToInt32(dr["HoiDongGiamThiID"], 0);
                        info.TenHoiDongGiamThi = Utils.ConvertToString(dr["TenHoiDongGiamThi"], string.Empty);
                        info.HoiDongGiamKhaoID = Utils.ConvertToInt32(dr["HoiDongGiamKhaoID"], 0);
                        info.TenHoiDongGiamKhao = Utils.ConvertToString(dr["TenHoiDongGiamKhao"], string.Empty);
                        info.HoiDongCoiThiID = Utils.ConvertToInt32(dr["HoiDongCoiThiID"], 0);
                        info.TenHoiDongCoiThi = Utils.ConvertToString(dr["TenHoiDongCoiThi"], string.Empty);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.SBDDau = Utils.ConvertToString(dr["SBDDau"], string.Empty);
                        info.SBDCuoi = Utils.ConvertToString(dr["SBDCuoi"], string.Empty);
                        info.NguoiDocDiem = Utils.ConvertToString(dr["NguoiDocDiem"], string.Empty);
                        info.NguoiNhapVaInDiem = Utils.ConvertToString(dr["NguoiNhapVaInDiem"], string.Empty);
                        info.NguoiDocSoatBanGhi = Utils.ConvertToString(dr["NguoiDocSoatBanGhi"], string.Empty);
                        info.NgayDuyetBangDiem = Utils.ConvertToNullableDateTime(dr["NgayDuyetBangDiem"], null);
                        info.NgayDuyetCham = Utils.ConvertToNullableDateTime(dr["NgayDuyetCham"], null);
                        info.NgaySoDuyet = Utils.ConvertToNullableDateTime(dr["NgaySoDuyet"], null);
                        info.CanBoXetDuyet = Utils.ConvertToString(dr["CanBoXetDuyet"], string.Empty);
                        info.CanBoSoKT = Utils.ConvertToString(dr["CanBoSoKT"], string.Empty);
                        info.ChuTichHoiDong = Utils.ConvertToString(dr["ChuTichHoiDong"], string.Empty);
                        info.GiamDocSo = Utils.ConvertToString(dr["GiamDocSo"], string.Empty);
                        info.SoThiSinhDuThi = Utils.ConvertToNullableInt32(dr["SoThiSinhDuThi"], null);
                        info.DuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["DuocCongNhanTotNghiep"], null);
                        info.KhongDuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["KhongDuocCongNhanTotNghiep"], null);
                        info.TNLoaiGioi = Utils.ConvertToNullableInt32(dr["TNLoaiGioi"], null);
                        info.TNLoaiKha = Utils.ConvertToNullableInt32(dr["TNLoaiKha"], null);
                        info.TNLoaiTB = Utils.ConvertToNullableInt32(dr["TNLoaiTB"], null);
                        info.DienTotNghiep2 = Utils.ConvertToNullableInt32(dr["DienTotNghiep2"], null);
                        info.DienTotNghiep3 = Utils.ConvertToNullableInt32(dr["DienTotNghiep3"], null);
                        info.TotNghiepDienA = Utils.ConvertToNullableInt32(dr["TotNghiepDienA"], null);
                        info.TotNghiepDienB = Utils.ConvertToNullableInt32(dr["TotNghiepDienB"], null);
                        info.TotNghiepDienC = Utils.ConvertToNullableInt32(dr["TotNghiepDienC"], null);
                        info.DienTotNghiep4_5 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_5"], null);
                        info.DienTotNghiep4_75 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_75"], null);
                        info.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        info.TrangThai = Utils.ConvertToNullableInt32(dr["TrangThai"], null);
                        info.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        info.ThuKy = Utils.ConvertToString(dr["ThuKy"], string.Empty);
                        info.ChanhChuKhao = Utils.ConvertToString(dr["ChanhChuKhao"], string.Empty);
                        info.PhoChuKhao = Utils.ConvertToString(dr["PhoChuKhao"], string.Empty);
                        info.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToNullableInt32(dr["SoTrang"], null);
                        info.TongSoThiSinh = Utils.ConvertToNullableInt32(dr["TongSoThiSinh"], null);
                        info.Tinh = Utils.ConvertToString(dr["Tinh"], string.Empty);
                        info.ToTruongHoiPhach = Utils.ConvertToString(dr["ToTruongHoiPhach"], string.Empty);
                        info.ChuTichHoiDongCoiThi = Utils.ConvertToString(dr["ChuTichHoiDongCoiThi"], string.Empty);
                        info.HieuTruong = Utils.ConvertToString(dr["HieuTruong"], string.Empty);
                        info.ChuTichHoiDongChamThi = Utils.ConvertToString(dr["ChuTichHoiDongChamThi"], string.Empty);
                        info.DiaDanh = Utils.ConvertToString(dr["DiaDanh"], string.Empty);     
                        info.GhiChuCuoiTrang = Utils.ConvertToString(dr["GhiChuCuoiTrang"], string.Empty);
                        info.SBDDau_CuoiTrang = Utils.ConvertToString(dr["SBDDau_CuoiTrang"], string.Empty);
                        info.SBDCuoi_CuoiTrang = Utils.ConvertToString(dr["SBDCuoi_CuoiTrang"], string.Empty);
                        info.TSDoThang = Utils.ConvertToNullableInt32(dr["TSDoThang"], null);
                        info.TSDoThem = Utils.ConvertToNullableInt32(dr["TSDoThem"], null);
                        info.TSThiHong = Utils.ConvertToNullableInt32(dr["TSThiHong"], null);
                        info.PGiamDoc = Utils.ConvertToString(dr["PGiamDoc"], string.Empty);
                        info.NguoiKiemTra = Utils.ConvertToString(dr["NguoiKiemTra"], string.Empty);
                        info.Nam = Utils.ConvertToNullableInt32(dr["Nam"], null);

                        info.FOOT_RPDD = Utils.ConvertToString(dr["FOOT_RPDD"], string.Empty);
                        info.FOOT_KTD = Utils.ConvertToString(dr["FOOT_KTD"], string.Empty);
                        info.FOOT_DTBDIS = Utils.ConvertToString(dr["FOOT_DTBDIS"], string.Empty);
                        info.FOOT_THUKY = Utils.ConvertToString(dr["FOOT_THUKY"], string.Empty);
                        if(info.FOOT_THUKY == string.Empty && info.ThuKy != string.Empty)
                        {
                            info.FOOT_THUKY = info.ThuKy;
                        }
                        info.FOOT_GIAMSAT = Utils.ConvertToString(dr["FOOT_GIAMSAT"], string.Empty);
                        info.FOOT_So_DCNTN = Utils.ConvertToString(dr["FOOT_So_DCNTN"], string.Empty);
                        info.FOOT_So_Dien45 = Utils.ConvertToString(dr["FOOT_So_Dien45"], string.Empty);
                        info.FOOT_So_Dien475 = Utils.ConvertToString(dr["FOOT_So_Dien475"], string.Empty);
                        info.FOOT_So_Loai_Gioi = Utils.ConvertToString(dr["FOOT_So_Loai_Gioi"], string.Empty);
                        info.FOOT_So_Loai_Kha = Utils.ConvertToString(dr["FOOT_So_Loai_Kha"], string.Empty);
                        info.FOOT_So_Loai_TB = Utils.ConvertToString(dr["FOOT_So_Loai_TB"], string.Empty);
                        info.FOOT_CONGTHEM1DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEM1DIEM"], string.Empty);
                        info.FOOT_CONGTHEM15DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEM15DIEM"], string.Empty);
                        info.FOOT_CONGTHEM2DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEM2DIEM"], string.Empty);
                        info.FOOT_CONGTHEMTREN2DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEMTREN2DIEM"], string.Empty);
                        info.FOOT_VANGMATKHITHI = Utils.ConvertToString(dr["FOOT_VANGMATKHITHI"], string.Empty);
                        info.FOOT_VIPHAMQUYCHETHI = Utils.ConvertToString(dr["FOOT_VIPHAMQUYCHETHI"], string.Empty);
                        info.FOOT_HSDIENUUTIEN = Utils.ConvertToString(dr["FOOT_HSDIENUUTIEN"], string.Empty);
                        info.FOOT_HSCOCHUNGNHANNGHE = Utils.ConvertToString(dr["FOOT_HSCOCHUNGNHANNGHE"], string.Empty);
                        info.FOOT_NGUOIKIEMTRAHS = Utils.ConvertToString(dr["FOOT_NGUOIKIEMTRAHS"], string.Empty);
                        info.FOOT_SKĐCNTN = Utils.ConvertToString(dr["FOOT_SKĐCNTN"], string.Empty);
                        info.FOOT_STSDT = Utils.ConvertToString(dr["FOOT_STSDT"], string.Empty);
                        info.FOOT_SSTSDT = Utils.ConvertToString(dr["FOOT_SSTSDT"], string.Empty);
                        info.FOOT_TND_D = Utils.ConvertToString(dr["FOOT_TND_D"], string.Empty);
                        info.FOOT_TND_E = Utils.ConvertToString(dr["FOOT_TND_E"], string.Empty);
                        info.FOOT_LTHUONG = Utils.ConvertToString(dr["FOOT_LTHUONG"], string.Empty);
                        info.FOOT_SLTHUONG = Utils.ConvertToString(dr["FOOT_SLTHUONG"], string.Empty);
                        info.FOOT_HSCONLIETSI = Utils.ConvertToString(dr["FOOT_HSCONLIETSI"], string.Empty);
                        info.FOOT_HSCACDIENKHAC = Utils.ConvertToString(dr["FOOT_HSCACDIENKHAC"], string.Empty);
                        info.FOOT_NGUOILAPBANG = Utils.ConvertToString(dr["FOOT_NGUOILAPBANG"], string.Empty);
                        info.FOOT_NXNLAPBANG = Utils.ConvertToString(dr["FOOT_NXNLAPBANG"], string.Empty);
                        info.FOOT_NXNHOIDONGCOITHI = Utils.ConvertToString(dr["FOOT_NXNHOIDONGCOITHI"], string.Empty);
                        info.FOOT_NXNCHAMTHIXTN = Utils.ConvertToString(dr["FOOT_NXNCHAMTHIXTN"], string.Empty);
                        info.FOOT_VTVGDTX = Utils.ConvertToString(dr["FOOT_VTVGDTX"], string.Empty);
                        info.FOOT_CTHDPHUCKHAO = Utils.ConvertToString(dr["FOOT_CTHDPHUCKHAO"], string.Empty);
                        info.HEAD_TRUONG = Utils.ConvertToString(dr["HEAD_TRUONG"], string.Empty);
                        info.HEAD_HDCL = Utils.ConvertToString(dr["HEAD_HDCL"], string.Empty);
                        info.HEAD_HDCT = info.TenHoiDongChamThi;
                        DuLieuDiemThiModel.ThongTinToChucThi = info;
                    }
                    dr.Close();
                }

                DuLieuDiemThiModel.ThongTinThiSinh = GetThongTinThiSinh(KyThiID);
                DuLieuDiemThiModel.DanhSachChinhSua = GetDanhSachThiSinhLog(KyThiID);
                return DuLieuDiemThiModel;
            }
            catch
            {
                throw;
            }
        }
        public DuLieuDiemThiModel GetThongTinKyThi(int KyThiID)
        {
            DuLieuDiemThiModel DuLieuDiemThiModel = new DuLieuDiemThiModel();
            DuLieuDiemThiModel.ThongTinToChucThi = new ThongTinToChucThi();
            DuLieuDiemThiModel.ThongTinThiSinh = new List<ThongTinThiSinh>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@KyThiID",SqlDbType.Int)
            };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_GetByKyThiID", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinToChucThi info = new ThongTinToChucThi();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.HoiDongChamThiID = Utils.ConvertToInt32(dr["HoiDongChamThiID"], 0);
                        info.TenHoiDongChamThi = Utils.ConvertToString(dr["TenHoiDongChamThi"], string.Empty);
                        info.HoiDongGiamThiID = Utils.ConvertToInt32(dr["HoiDongGiamThiID"], 0);
                        info.TenHoiDongGiamThi = Utils.ConvertToString(dr["TenHoiDongGiamThi"], string.Empty);
                        info.HoiDongGiamKhaoID = Utils.ConvertToInt32(dr["HoiDongGiamKhaoID"], 0);
                        info.TenHoiDongGiamKhao = Utils.ConvertToString(dr["TenHoiDongGiamKhao"], string.Empty);
                        info.HoiDongCoiThiID = Utils.ConvertToInt32(dr["HoiDongCoiThiID"], 0);
                        info.TenHoiDongCoiThi = Utils.ConvertToString(dr["TenHoiDongCoiThi"], string.Empty);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.SBDDau = Utils.ConvertToString(dr["SBDDau"], string.Empty);
                        info.SBDCuoi = Utils.ConvertToString(dr["SBDCuoi"], string.Empty);
                        info.NguoiDocDiem = Utils.ConvertToString(dr["NguoiDocDiem"], string.Empty);
                        info.NguoiNhapVaInDiem = Utils.ConvertToString(dr["NguoiNhapVaInDiem"], string.Empty);
                        info.NguoiDocSoatBanGhi = Utils.ConvertToString(dr["NguoiDocSoatBanGhi"], string.Empty);
                        info.NgayDuyetBangDiem = Utils.ConvertToNullableDateTime(dr["NgayDuyetBangDiem"], null);
                        info.NgayDuyetCham = Utils.ConvertToNullableDateTime(dr["NgayDuyetCham"], null);
                        info.NgaySoDuyet = Utils.ConvertToNullableDateTime(dr["NgaySoDuyet"], null);
                        info.CanBoXetDuyet = Utils.ConvertToString(dr["CanBoXetDuyet"], string.Empty);
                        info.CanBoSoKT = Utils.ConvertToString(dr["CanBoSoKT"], string.Empty);
                        info.ChuTichHoiDong = Utils.ConvertToString(dr["ChuTichHoiDong"], string.Empty);
                        info.GiamDocSo = Utils.ConvertToString(dr["GiamDocSo"], string.Empty);
                        info.SoThiSinhDuThi = Utils.ConvertToNullableInt32(dr["SoThiSinhDuThi"], null);
                        info.DuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["DuocCongNhanTotNghiep"], null);
                        info.KhongDuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["KhongDuocCongNhanTotNghiep"], null);
                        info.TNLoaiGioi = Utils.ConvertToNullableInt32(dr["TNLoaiGioi"], null);
                        info.TNLoaiKha = Utils.ConvertToNullableInt32(dr["TNLoaiKha"], null);
                        info.TNLoaiTB = Utils.ConvertToNullableInt32(dr["TNLoaiTB"], null);
                        info.DienTotNghiep2 = Utils.ConvertToNullableInt32(dr["DienTotNghiep2"], null);
                        info.DienTotNghiep3 = Utils.ConvertToNullableInt32(dr["DienTotNghiep3"], null);
                        info.TotNghiepDienA = Utils.ConvertToNullableInt32(dr["TotNghiepDienA"], null);
                        info.TotNghiepDienB = Utils.ConvertToNullableInt32(dr["TotNghiepDienB"], null);
                        info.TotNghiepDienC = Utils.ConvertToNullableInt32(dr["TotNghiepDienC"], null);
                        info.DienTotNghiep4_5 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_5"], null);
                        info.DienTotNghiep4_75 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_75"], null);
                        info.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        info.TrangThai = Utils.ConvertToNullableInt32(dr["TrangThai"], null);
                        info.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        info.ThuKy = Utils.ConvertToString(dr["ThuKy"], string.Empty);
                        info.ChanhChuKhao = Utils.ConvertToString(dr["ChanhChuKhao"], string.Empty);
                        info.PhoChuKhao = Utils.ConvertToString(dr["PhoChuKhao"], string.Empty);
                        info.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToNullableInt32(dr["SoTrang"], null);
                        info.TongSoThiSinh = Utils.ConvertToNullableInt32(dr["TongSoThiSinh"], null);
                        info.Tinh = Utils.ConvertToString(dr["Tinh"], string.Empty);
                        info.ToTruongHoiPhach = Utils.ConvertToString(dr["ToTruongHoiPhach"], string.Empty);
                        info.ChuTichHoiDongCoiThi = Utils.ConvertToString(dr["ChuTichHoiDongCoiThi"], string.Empty);
                        info.HieuTruong = Utils.ConvertToString(dr["HieuTruong"], string.Empty);
                        info.ChuTichHoiDongChamThi = Utils.ConvertToString(dr["ChuTichHoiDongChamThi"], string.Empty);
                        info.DiaDanh = Utils.ConvertToString(dr["DiaDanh"], string.Empty);
                        info.GhiChuCuoiTrang = Utils.ConvertToString(dr["GhiChuCuoiTrang"], string.Empty);
                        info.SBDDau_CuoiTrang = Utils.ConvertToString(dr["SBDDau_CuoiTrang"], string.Empty);
                        info.SBDCuoi_CuoiTrang = Utils.ConvertToString(dr["SBDCuoi_CuoiTrang"], string.Empty);
                        info.TSDoThang = Utils.ConvertToNullableInt32(dr["TSDoThang"], null);
                        info.TSDoThem = Utils.ConvertToNullableInt32(dr["TSDoThem"], null);
                        info.TSThiHong = Utils.ConvertToNullableInt32(dr["TSThiHong"], null);
                        info.PGiamDoc = Utils.ConvertToString(dr["PGiamDoc"], string.Empty);
                        info.NguoiKiemTra = Utils.ConvertToString(dr["NguoiKiemTra"], string.Empty);
                        info.Nam = Utils.ConvertToNullableInt32(dr["Nam"], null);

                        //info.FOOT_RPDD = Utils.ConvertToString(dr["FOOT_RPDD"], string.Empty);
                        //info.FOOT_KTD = Utils.ConvertToString(dr["FOOT_KTD"], string.Empty);
                        //info.FOOT_DTBDIS = Utils.ConvertToString(dr["FOOT_DTBDIS"], string.Empty);
                        //info.FOOT_THUKY = Utils.ConvertToString(dr["FOOT_THUKY"], string.Empty);
                        //if (info.FOOT_THUKY == string.Empty && info.ThuKy != string.Empty)
                        //{
                        //    info.FOOT_THUKY = info.ThuKy;
                        //}
                        //info.FOOT_GIAMSAT = Utils.ConvertToString(dr["FOOT_GIAMSAT"], string.Empty);
                        //info.FOOT_So_DCNTN = Utils.ConvertToString(dr["FOOT_So_DCNTN"], string.Empty);
                        //info.FOOT_So_Dien45 = Utils.ConvertToString(dr["FOOT_So_Dien45"], string.Empty);
                        //info.FOOT_So_Dien475 = Utils.ConvertToString(dr["FOOT_So_Dien475"], string.Empty);
                        //info.FOOT_So_Loai_Gioi = Utils.ConvertToString(dr["FOOT_So_Loai_Gioi"], string.Empty);
                        //info.FOOT_So_Loai_Kha = Utils.ConvertToString(dr["FOOT_So_Loai_Kha"], string.Empty);
                        //info.FOOT_So_Loai_TB = Utils.ConvertToString(dr["FOOT_So_Loai_TB"], string.Empty);
                        //info.FOOT_CONGTHEM1DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEM1DIEM"], string.Empty);
                        //info.FOOT_CONGTHEM15DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEM15DIEM"], string.Empty);
                        //info.FOOT_CONGTHEM2DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEM2DIEM"], string.Empty);
                        //info.FOOT_CONGTHEMTREN2DIEM = Utils.ConvertToString(dr["FOOT_CONGTHEMTREN2DIEM"], string.Empty);
                        //info.FOOT_VANGMATKHITHI = Utils.ConvertToString(dr["FOOT_VANGMATKHITHI"], string.Empty);
                        //info.FOOT_VIPHAMQUYCHETHI = Utils.ConvertToString(dr["FOOT_VIPHAMQUYCHETHI"], string.Empty);
                        //info.FOOT_HSDIENUUTIEN = Utils.ConvertToString(dr["FOOT_HSDIENUUTIEN"], string.Empty);
                        //info.FOOT_HSCOCHUNGNHANNGHE = Utils.ConvertToString(dr["FOOT_HSCOCHUNGNHANNGHE"], string.Empty);
                        //info.FOOT_NGUOIKIEMTRAHS = Utils.ConvertToString(dr["FOOT_NGUOIKIEMTRAHS"], string.Empty);
                        //info.FOOT_SKĐCNTN = Utils.ConvertToString(dr["FOOT_SKĐCNTN"], string.Empty);
                        //info.FOOT_STSDT = Utils.ConvertToString(dr["FOOT_STSDT"], string.Empty);
                        //info.FOOT_SSTSDT = Utils.ConvertToString(dr["FOOT_SSTSDT"], string.Empty);
                        //info.FOOT_TND_D = Utils.ConvertToString(dr["FOOT_TND_D"], string.Empty);
                        //info.FOOT_TND_E = Utils.ConvertToString(dr["FOOT_TND_E"], string.Empty);
                        //info.FOOT_LTHUONG = Utils.ConvertToString(dr["FOOT_LTHUONG"], string.Empty);
                        //info.FOOT_SLTHUONG = Utils.ConvertToString(dr["FOOT_SLTHUONG"], string.Empty);
                        //info.FOOT_HSCONLIETSI = Utils.ConvertToString(dr["FOOT_HSCONLIETSI"], string.Empty);
                        //info.FOOT_HSCACDIENKHAC = Utils.ConvertToString(dr["FOOT_HSCACDIENKHAC"], string.Empty);
                        //info.FOOT_NGUOILAPBANG = Utils.ConvertToString(dr["FOOT_NGUOILAPBANG"], string.Empty);
                        //info.FOOT_NXNLAPBANG = Utils.ConvertToString(dr["FOOT_NXNLAPBANG"], string.Empty);
                        //info.FOOT_NXNHOIDONGCOITHI = Utils.ConvertToString(dr["FOOT_NXNHOIDONGCOITHI"], string.Empty);
                        //info.FOOT_NXNCHAMTHIXTN = Utils.ConvertToString(dr["FOOT_NXNCHAMTHIXTN"], string.Empty);
                        //info.FOOT_VTVGDTX = Utils.ConvertToString(dr["FOOT_VTVGDTX"], string.Empty);
                        //info.FOOT_CTHDPHUCKHAO = Utils.ConvertToString(dr["FOOT_CTHDPHUCKHAO"], string.Empty);
                        //info.HEAD_TRUONG = Utils.ConvertToString(dr["HEAD_TRUONG"], string.Empty);
                        //info.HEAD_HDCL = Utils.ConvertToString(dr["HEAD_HDCL"], string.Empty);
                        //info.HEAD_HDCT = info.TenHoiDongChamThi;
                        DuLieuDiemThiModel.ThongTinToChucThi = info;
                    }
                    dr.Close();
                }
             
                return DuLieuDiemThiModel;
            }
            catch
            {
                throw;
            }
        }
        public List<ThiSinhLog> GetDanhSachThiSinhLog(int KyThiID)
        {
            List<ThiSinhLog> Result = new List<ThiSinhLog>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@KyThiID",SqlDbType.Int)
            };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ListThiSinhLog_GetByKyThiID", parameters))
                {
                    while (dr.Read())
                    {
                        ThiSinhLog info = new ThiSinhLog();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTenThiSinh = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NguoiChinhSua = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        info.SBD = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.NgayChinhSua = Utils.ConvertToNullableDateTime(dr["NgayThaoTac"], null);
                        info.ThaoTac = Utils.ConvertToNullableInt32(dr["ThaoTac"], null);
                        if (info.ThaoTac == EnumLog.Insert.GetHashCode())
                        {
                            info.NoiDungChinhSua = "Thêm mới thí sinh";
                        }
                        else if (info.ThaoTac == EnumLog.Update.GetHashCode())
                        {
                            info.NoiDungChinhSua = "Chỉnh sửa thông tin thí sinh";
                        }
                        else if (info.ThaoTac == EnumLog.Update.GetHashCode())
                        {
                            info.NoiDungChinhSua = "Chỉnh sửa thông tin thí sinh";
                        }

                        Result.Add(info);
                    }
                    dr.Close();
                }

                return Result;
            }
            catch
            {
                throw;
            }
        }
        public List<ThongTinThiSinh> GetThongTinThiSinh(int KyThiID)
        {
            List<ThongTinThiSinh> Result = new List<ThongTinThiSinh>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@KyThiID",SqlDbType.Int)
            };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetByKyThiID", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.KyThiID = KyThiID;
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NgaySinhStr = Utils.ConvertToString(dr["NgaySinhStr"], string.Empty);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToNullableInt32(dr["TruongTHPT"], null);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        info.LaoDong = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        info.VanHoa = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        info.RLTT = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        info.XepLoaiHanhKiem = Utils.ConvertToNullableInt32(dr["XepLoaiHanhKiem"], null);
                        info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.Do = Utils.ConvertToString(dr["Do"], string.Empty);
                        info.DoThem = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        info.Hong = Utils.ConvertToString(dr["Hong"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToNullableInt32(dr["XepLoaiHocLuc"], null);
                        info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToNullableDecimal(dr["DiemTBLop12"], null);
                        info.DiemXL = Utils.ConvertToNullableDecimal(dr["DiemXL"], null);
                        info.DiemUT = Utils.ConvertToNullableDecimal(dr["DiemUT"], null);
                        info.DiemKK = Utils.ConvertToNullableDecimal(dr["DiemKK"], null);
                        info.DienXTN = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        info.HoiDongThi = Utils.ConvertToNullableInt32(dr["HoiDongThi"], null);
                        info.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.TongSoDiemThi = Utils.ConvertToNullableDecimal(dr["TongSoDiemThi"], null);
                        info.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(dr["DiemTBCacBaiThi"], null);
                        info.DienUuTien = Utils.ConvertToString(dr["DienUuTien"], string.Empty);
                        info.DiemTBC = Utils.ConvertToNullableDecimal(dr["DiemTBC"], null);
                        info.QueQuan = Utils.ConvertToString(dr["QueQuan"], string.Empty);
                        info.ChungNhanNghe = Utils.ConvertToString(dr["ChungNhanNghe"], string.Empty);
                        info.DTConLietSi = Utils.ConvertToString(dr["DTConLietSi"], string.Empty);
                        info.GiaiTDKT = Utils.ConvertToString(dr["GiaiTDKT"], string.Empty);
                        info.HoiDong = Utils.ConvertToString(dr["HoiDong"], string.Empty);
                        info.MonKN = Utils.ConvertToString(dr["MonKN"], string.Empty);
                        info.TBCNMonKN = Utils.ConvertToString(dr["TBCNMonKN"], string.Empty);
                        info.DiemThiCu = Utils.ConvertToString(dr["DiemThiCu"], string.Empty);
                        info.DiemThiMoi = Utils.ConvertToString(dr["DiemThiMoi"], string.Empty);
                        info.TongBQ = Utils.ConvertToString(dr["TongBQ"], string.Empty);
                        info.BQA = Utils.ConvertToString(dr["BQA"], string.Empty);
                        info.BQT = Utils.ConvertToString(dr["BQT"], string.Empty);
                        info.DC = Utils.ConvertToString(dr["DC"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);

                        info.BODY_DAODUC = Utils.ConvertToString(dr["BODY_DAODUC"], string.Empty);
                        info.BODY_RLEV = Utils.ConvertToString(dr["BODY_RLEV"], string.Empty);
                        info.BODY_DIENKK = Utils.ConvertToString(dr["BODY_DIENKK"], string.Empty);
                        info.BODY_PHONGTHI = Utils.ConvertToString(dr["BODY_PHONGTHI"], string.Empty);
                        info.BODY_DIEMTNC = Utils.ConvertToString(dr["BODY_DIEMTNC"], string.Empty);
                        info.BODY_XLTNC = Utils.ConvertToString(dr["BODY_XLTNC"], string.Empty);
                        info.BODY_TDTCU = Utils.ConvertToString(dr["BODY_TDTCU"], string.Empty);
                        info.BODY_GIAIHSG = Utils.ConvertToString(dr["BODY_GIAIHSG"], string.Empty);
                        info.BODY_GIAIHSGK = Utils.ConvertToString(dr["BODY_GIAIHSGK"], string.Empty);
                        info.BODY_CHUNGCHINN = Utils.ConvertToString(dr["BODY_CHUNGCHINN"], string.Empty);
                        info.BODY_CHUNGCHITH = Utils.ConvertToString(dr["BODY_CHUNGCHITH"], string.Empty);
                        info.BODY_TONGDIEMMOI = Utils.ConvertToString(dr["BODY_TONGDIEMMOI"], string.Empty);
                        info.BODY_BQAMOI = Utils.ConvertToString(dr["BODY_BQAMOI"], string.Empty);
                        info.BODY_BQTMOI = Utils.ConvertToString(dr["BODY_BQTMOI"], string.Empty);
                        info.BODY_SOCAPGIAYCN = Utils.ConvertToString(dr["BODY_SOCAPGIAYCN"], string.Empty);
                        info.BODY_XLHT = Utils.ConvertToString(dr["BODY_XLHT"], string.Empty);
                        info.BODY_QUOCGIA = Utils.ConvertToString(dr["BODY_QUOCGIA"], string.Empty);

                        info.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                        Result.Add(info);
                    }
                    dr.Close();
                }
                if (Result.Count > 0)
                {
                    List<ThongTinDiemThi> ListDiemThi = GetDuLieuDiemThi(KyThiID);
                    if (ListDiemThi.Count > 0)
                    {
                        foreach (var item in Result)
                        {
                            item.ListThongTinDiemThi = ListDiemThi.Where(x => x.ThiSinhID == item.ThiSinhID).ToList();
                        }
                    }
                    Result = Result.OrderBy(x => x.ThiSinhID).ToList();
                }

                return Result;
            }
            catch
            {
                throw;
            }
        }
        public List<ThongTinDiemThi> GetDuLieuDiemThi(int KyThiID)
        {
            List<ThongTinDiemThi> Result = new List<ThongTinDiemThi>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@KyThiID",SqlDbType.Int)
            };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetByKyThiID", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinDiemThi info = new ThongTinDiemThi();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.DiemThiID = Utils.ConvertToInt32(dr["DiemThiID"], 0);
                        info.NhomID = Utils.ConvertToInt32(dr["NhomID"], 0);
                        info.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                        info.Diem = Utils.ConvertToNullableDecimal(dr["Diem"], null);
                        info.DiemBaiToHop = Utils.ConvertToString(dr["DiemBaiToHop"], string.Empty);
                        Result.Add(info);
                    }
                    dr.Close();
                }

                return Result;
            }
            catch
            {
                throw;
            }
        }
        public BaseResultModel DeleteDuLieuTep(int KyThiID)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                  new SqlParameter("KyThiID",SqlDbType.Int)
                };
                parameters[0].Value = KyThiID;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_Delete", parameters);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Delete_Success("dữ liệu");
                            return Result;
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public List<ThongTinToChucThi> GetPagingBySearch_NhapDuLieuDiem(BasePagingParams p, int HoiDongID, int KhoaThiID, ref int TotalRow)
        {
            List<ThongTinToChucThi> Result = new List<ThongTinToChucThi>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@HoiDongID",SqlDbType.Int),
                new SqlParameter("@KhoaThiID",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = HoiDongID;
            parameters[7].Value = p.Nam ?? 0;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NhapDuLieuDiem_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinToChucThi info = new ThongTinToChucThi();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDong"], string.Empty);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.TenKhoaThi = Utils.ConvertToString(dr["TenKhoaThi"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        Result.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Result;
        }
        public BaseResultModel Update_TrangThai(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            var Result = new BaseResultModel();
            try
            {

                if (ListThongTinToChucThi == null || (ListThongTinToChucThi != null && ListThongTinToChucThi.Count == 0))
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu không được để trống";
                    return Result;
                }
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in ListThongTinToChucThi)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("KyThiID", SqlDbType.Int),
                                    new SqlParameter("TrangThai", SqlDbType.Int),
                                };
                                parameters[0].Value = item.KyThiID;
                                parameters[1].Value = item.TrangThai ?? Convert.DBNull;

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_UpdateTrangThai", parameters);
                            }

                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("dữ liệu");
                            return Result;
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public BaseResultModel Update_TrangThaiKhoa(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            var Result = new BaseResultModel();
            try
            {

                if (ListThongTinToChucThi == null || (ListThongTinToChucThi != null && ListThongTinToChucThi.Count == 0))
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu không được để trống";
                    return Result;
                }
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in ListThongTinToChucThi)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("KyThiID", SqlDbType.Int),
                                    new SqlParameter("TrangThai", SqlDbType.Int),
                                };
                                parameters[0].Value = item.KyThiID;
                                parameters[1].Value = item.TrangThai ?? Convert.DBNull;

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_UpdateTrangThaiKhoa", parameters);
                            }

                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("dữ liệu");
                            return Result;
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public BaseResultModel Update_TrangThaiTrang(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            var Result = new BaseResultModel();
            try
            {

                if (ListThongTinToChucThi == null || (ListThongTinToChucThi != null && ListThongTinToChucThi.Count == 0))
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu không được để trống";
                    return Result;
                }
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in ListThongTinToChucThi)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("KyThiID", SqlDbType.Int),
                                    new SqlParameter("TrangThai", SqlDbType.Int),
                                };
                                parameters[0].Value = item.KyThiID;
                                parameters[1].Value = item.Type ?? Convert.DBNull;

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_UpdateTrangThaiTrang", parameters);
                            }

                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("dữ liệu");
                            return Result;
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public BaseResultModel Update_TrangThaiDiemThi(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            var Result = new BaseResultModel();
            try
            {

                if (ListThongTinThiSinh == null || (ListThongTinThiSinh != null && ListThongTinThiSinh.Count == 0))
                {
                    Result.Status = 0;
                    Result.Message = "Dữ liệu không được để trống";
                    return Result;
                }
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in ListThongTinThiSinh)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("ThiSinhID", SqlDbType.Int),
                                    new SqlParameter("TrangThai", SqlDbType.Int),
                                };
                                parameters[0].Value = item.ThiSinhID;
                                parameters[1].Value = item.TrangThai ?? Convert.DBNull;

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_UpdateTrangThai", parameters);
                            }

                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("duyệt điểm thi");
                            return Result;
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public DuLieuDiemThiModel CheckTrungSoQuyenSoTrang(int? SoTrang, string SoQuyen, string TenHoiDongThi, DateTime? KhoaThiNgay)
        {
            List<DuLieuDiemThiModel> Result = new List<DuLieuDiemThiModel>();
            List<ChiTietDuLieuDiemThiModel> chiTiets = new List<ChiTietDuLieuDiemThiModel>();

            var HoiDong = new DanhMucChungModel();
            if (TenHoiDongThi !=null && TenHoiDongThi.Length > 0)
            {
                HoiDong = new DanhMucChungDAL().GetByName(TenHoiDongThi.Trim());
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SoTrang",SqlDbType.Int),
                  new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                  new SqlParameter("@HoiDongThiID",SqlDbType.Int),
                  new SqlParameter("@KhoaThiNgay",SqlDbType.DateTime)
            };
            parameters[0].Value = SoTrang ?? Convert.DBNull;
            parameters[1].Value = SoQuyen ?? Convert.DBNull;
            parameters[2].Value = HoiDong.ID > 0 ? HoiDong.ID : Convert.DBNull;
            parameters[3].Value = KhoaThiNgay ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_GetBySoTrangSoQuyen", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietDuLieuDiemThiModel info = new ChiTietDuLieuDiemThiModel();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.HoiDongChamThiID = Utils.ConvertToInt32(dr["HoiDongChamThiID"], 0);
                        info.TenHoiDongChamThi = Utils.ConvertToString(dr["TenHoiDongChamThi"], string.Empty);
                        info.HoiDongGiamThiID = Utils.ConvertToInt32(dr["HoiDongGiamThiID"], 0);
                        info.TenHoiDongGiamThi = Utils.ConvertToString(dr["TenHoiDongGiamThi"], string.Empty);
                        info.HoiDongGiamKhaoID = Utils.ConvertToInt32(dr["HoiDongGiamKhaoID"], 0);
                        info.TenHoiDongGiamKhao = Utils.ConvertToString(dr["TenHoiDongGiamKhao"], string.Empty);
                        info.HoiDongCoiThiID = Utils.ConvertToInt32(dr["HoiDongCoiThiID"], 0);
                        info.TenHoiDongCoiThi = Utils.ConvertToString(dr["TenHoiDongCoiThi"], string.Empty);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        info.SBDDau = Utils.ConvertToString(dr["SBDDau"], string.Empty);
                        info.SBDCuoi = Utils.ConvertToString(dr["SBDCuoi"], string.Empty);
                        info.NguoiDocDiem = Utils.ConvertToString(dr["NguoiDocDiem"], string.Empty);
                        info.NguoiNhapVaInDiem = Utils.ConvertToString(dr["NguoiNhapVaInDiem"], string.Empty);
                        info.NguoiDocSoatBanGhi = Utils.ConvertToString(dr["NguoiDocSoatBanGhi"], string.Empty);
                        info.NgayDuyetBangDiem = Utils.ConvertToNullableDateTime(dr["NgayDuyetBangDiem"], null);
                        info.NgayDuyetCham = Utils.ConvertToNullableDateTime(dr["NgayDuyetCham"], null);
                        info.NgaySoDuyet = Utils.ConvertToNullableDateTime(dr["NgaySoDuyet"], null);
                        info.CanBoXetDuyet = Utils.ConvertToString(dr["CanBoXetDuyet"], string.Empty);
                        info.CanBoSoKT = Utils.ConvertToString(dr["CanBoSoKT"], string.Empty);
                        info.ChuTichHoiDong = Utils.ConvertToString(dr["ChuTichHoiDong"], string.Empty);
                        info.GiamDocSo = Utils.ConvertToString(dr["GiamDocSo"], string.Empty);
                        info.SoThiSinhDuThi = Utils.ConvertToNullableInt32(dr["SoThiSinhDuThi"], null);
                        info.DuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["DuocCongNhanTotNghiep"], null);
                        info.KhongDuocCongNhanTotNghiep = Utils.ConvertToNullableInt32(dr["KhongDuocCongNhanTotNghiep"], null);
                        info.TNLoaiGioi = Utils.ConvertToNullableInt32(dr["TNLoaiGioi"], null);
                        info.TNLoaiKha = Utils.ConvertToNullableInt32(dr["TNLoaiKha"], null);
                        info.TNLoaiTB = Utils.ConvertToNullableInt32(dr["TNLoaiTB"], null);
                        info.DienTotNghiep2 = Utils.ConvertToNullableInt32(dr["DienTotNghiep2"], null);
                        info.DienTotNghiep3 = Utils.ConvertToNullableInt32(dr["DienTotNghiep3"], null);
                        info.TotNghiepDienA = Utils.ConvertToNullableInt32(dr["TotNghiepDienA"], null);
                        info.TotNghiepDienB = Utils.ConvertToNullableInt32(dr["TotNghiepDienB"], null);
                        info.DienTotNghiep4_5 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_5"], null);
                        info.DienTotNghiep4_75 = Utils.ConvertToNullableInt32(dr["DienTotNghiep4_75"], null);
                        info.GhiChuKyThi = Utils.ConvertToString(dr["GhiChuKyThi"], string.Empty);
                        info.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        info.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        info.TrangThaiKyThi = Utils.ConvertToNullableInt32(dr["TrangThaiKyThi"], null);
                        info.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        info.ThuKy = Utils.ConvertToString(dr["ThuKy"], string.Empty);
                        info.ChanhChuKhao = Utils.ConvertToString(dr["ChanhChuKhao"], string.Empty);
                        info.PhoChuKhao = Utils.ConvertToString(dr["PhoChuKhao"], string.Empty);
                        info.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        info.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        info.SoTrang = Utils.ConvertToNullableInt32(dr["SoTrang"], null);
                        info.TongSoThiSinh = Utils.ConvertToNullableInt32(dr["TongSoThiSinh"], null);
                        info.Tinh = Utils.ConvertToString(dr["Tinh"], string.Empty);
                        info.ToTruongHoiPhach = Utils.ConvertToString(dr["ToTruongHoiPhach"], string.Empty);
                        info.ChuTichHoiDongCoiThi = Utils.ConvertToString(dr["ChuTichHoiDongCoiThi"], string.Empty);
                        info.HieuTruong = Utils.ConvertToString(dr["HieuTruong"], string.Empty);
                        info.ChuTichHoiDongChamThi = Utils.ConvertToString(dr["ChuTichHoiDongChamThi"], string.Empty);

                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToNullableInt32(dr["TruongTHPT"], null);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        info.LaoDong = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        info.VanHoa = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        info.RLTT = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        info.XepLoaiHanhKiem = Utils.ConvertToNullableInt32(dr["XepLoaiHanhKiem"], null);
                        info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.Do = Utils.ConvertToString(dr["Do"], string.Empty);
                        info.DoThem = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        info.Hong = Utils.ConvertToString(dr["Hong"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToNullableInt32(dr["XepLoaiHocLuc"], null);
                        info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToNullableDecimal(dr["DiemTBLop12"], null);
                        info.DiemXL = Utils.ConvertToNullableDecimal(dr["DiemXL"], null);
                        info.DiemUT = Utils.ConvertToNullableDecimal(dr["DiemUT"], null);
                        info.DiemKK = Utils.ConvertToNullableDecimal(dr["DiemKK"], null);
                        info.DienXTN = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        info.HoiDongThi = Utils.ConvertToNullableInt32(dr["HoiDongThi"], null);
                        info.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        info.TrangThaiThiSinh = Utils.ConvertToInt32(dr["TrangThaiThiSinh"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.TongSoDiemThi = Utils.ConvertToNullableDecimal(dr["TongSoDiemThi"], null);
                        info.GhiChuThiSinh = Utils.ConvertToString(dr["GhiChuThiSinh"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(dr["DiemTBCacBaiThi"], null);
                        info.DienUuTien = Utils.ConvertToString(dr["DienUuTien"], string.Empty);
                        info.DiemTBC = Utils.ConvertToNullableDecimal(dr["DiemTBC"], null);

                        info.DiemThiID = Utils.ConvertToInt32(dr["DiemThiID"], 0);
                        info.NhomID = Utils.ConvertToInt32(dr["NhomID"], 0);
                        info.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                        info.Diem = Utils.ConvertToNullableDecimal(dr["Diem"], null);
                        info.DiemBaiToHop = Utils.ConvertToString(dr["DiemBaiToHop"], string.Empty);
                        chiTiets.Add(info);
                    }
                    dr.Close();
                }

                if (chiTiets.Count > 0)
                {
                    Result = (from m in chiTiets
                              group m by m.KyThiID into ctt
                              from item in ctt
                              select new DuLieuDiemThiModel()
                              {
                                  ThongTinToChucThi = (from i in chiTiets.Where(x => x.KyThiID > 0).ToList().GroupBy(x => x.KyThiID)
                                                       select new ThongTinToChucThi()
                                                       {
                                                           KyThiID = i.FirstOrDefault().KyThiID,
                                                           TenKyThi = i.FirstOrDefault().TenKyThi,
                                                           HoiDongThiID = i.FirstOrDefault().HoiDongThiID,
                                                           TenHoiDongThi = i.FirstOrDefault().TenHoiDongThi,
                                                           HoiDongChamThiID = i.FirstOrDefault().HoiDongChamThiID,
                                                           TenHoiDongChamThi = i.FirstOrDefault().TenHoiDongChamThi,
                                                           HoiDongGiamThiID = i.FirstOrDefault().HoiDongGiamThiID,
                                                           TenHoiDongGiamThi = i.FirstOrDefault().TenHoiDongGiamThi,
                                                           HoiDongGiamKhaoID = i.FirstOrDefault().HoiDongGiamKhaoID,
                                                           TenHoiDongGiamKhao = i.FirstOrDefault().TenHoiDongGiamKhao,
                                                           HoiDongCoiThiID = i.FirstOrDefault().HoiDongCoiThiID,
                                                           TenHoiDongCoiThi = i.FirstOrDefault().TenHoiDongCoiThi,
                                                           KhoaThiID = i.FirstOrDefault().KhoaThiID,
                                                           SBDDau = i.FirstOrDefault().SBDDau,
                                                           SBDCuoi = i.FirstOrDefault().SBDCuoi,
                                                           NguoiDocDiem = i.FirstOrDefault().NguoiDocDiem,
                                                           NguoiNhapVaInDiem = i.FirstOrDefault().NguoiNhapVaInDiem,
                                                           NguoiDocSoatBanGhi = i.FirstOrDefault().NguoiDocSoatBanGhi,
                                                           NgayDuyetBangDiem = i.FirstOrDefault().NgayDuyetBangDiem,
                                                           NgayDuyetCham = i.FirstOrDefault().NgayDuyetCham,
                                                           NgaySoDuyet = i.FirstOrDefault().NgaySoDuyet,
                                                           CanBoXetDuyet = i.FirstOrDefault().CanBoXetDuyet,
                                                           CanBoSoKT = i.FirstOrDefault().CanBoSoKT,
                                                           ChuTichHoiDong = i.FirstOrDefault().ChuTichHoiDong,
                                                           GiamDocSo = i.FirstOrDefault().GiamDocSo,
                                                           SoThiSinhDuThi = i.FirstOrDefault().SoThiSinhDuThi,
                                                           DuocCongNhanTotNghiep = i.FirstOrDefault().DuocCongNhanTotNghiep,
                                                           KhongDuocCongNhanTotNghiep = i.FirstOrDefault().KhongDuocCongNhanTotNghiep,
                                                           TNLoaiGioi = i.FirstOrDefault().TNLoaiGioi,
                                                           TNLoaiKha = i.FirstOrDefault().TNLoaiKha,
                                                           TNLoaiTB = i.FirstOrDefault().TNLoaiTB,
                                                           DienTotNghiep2 = i.FirstOrDefault().DienTotNghiep2,
                                                           DienTotNghiep3 = i.FirstOrDefault().DienTotNghiep3,
                                                           TotNghiepDienA = i.FirstOrDefault().TotNghiepDienA,
                                                           TotNghiepDienB = i.FirstOrDefault().TotNghiepDienB,
                                                           DienTotNghiep4_5 = i.FirstOrDefault().DienTotNghiep4_5,
                                                           DienTotNghiep4_75 = i.FirstOrDefault().DienTotNghiep4_75,
                                                           TrangThai = i.FirstOrDefault().TrangThaiKyThi,
                                                           GhiChu = i.FirstOrDefault().GhiChuKyThi,
                                                           PhongThi = i.FirstOrDefault().PhongThi,
                                                           Ban = i.FirstOrDefault().Ban,
                                                           MauPhieuID = i.FirstOrDefault().MauPhieuID,
                                                           ThuKy = i.FirstOrDefault().ThuKy,
                                                           ChanhChuKhao = i.FirstOrDefault().ChanhChuKhao,
                                                           PhoChuKhao = i.FirstOrDefault().PhoChuKhao,
                                                           KhoaThiNgay = i.FirstOrDefault().KhoaThiNgay,
                                                           SoQuyen = i.FirstOrDefault().SoQuyen,
                                                           SoTrang = i.FirstOrDefault().SoTrang,
                                                           TongSoThiSinh = i.FirstOrDefault().TongSoThiSinh,
                                                           Tinh = i.FirstOrDefault().Tinh,
                                                           ToTruongHoiPhach = i.FirstOrDefault().ToTruongHoiPhach,
                                                           ChuTichHoiDongCoiThi = i.FirstOrDefault().ChuTichHoiDongCoiThi,
                                                           HieuTruong = i.FirstOrDefault().HieuTruong,
                                                           ChuTichHoiDongChamThi = i.FirstOrDefault().ChuTichHoiDongChamThi
                                                       }
                                  ).FirstOrDefault(),
                                  ThongTinThiSinh = (from i in chiTiets.Where(x => x.ThiSinhID > 0).ToList().GroupBy(x => x.ThiSinhID)
                                                     select new ThongTinThiSinh()
                                                     {
                                                         ThiSinhID = i.FirstOrDefault().ThiSinhID,
                                                         KyThiID = i.FirstOrDefault().KyThiID,
                                                         HoTen = i.FirstOrDefault().HoTen,
                                                         NgaySinh = i.FirstOrDefault().NgaySinh,
                                                         NoiSinh = i.FirstOrDefault().NoiSinh,
                                                         GioiTinh = i.FirstOrDefault().GioiTinh,
                                                         DanToc = i.FirstOrDefault().DanToc,
                                                         CMND = i.FirstOrDefault().CMND,
                                                         SoBaoDanh = i.FirstOrDefault().SoBaoDanh,
                                                         SoDienThoai = i.FirstOrDefault().SoDienThoai,
                                                         DiaChi = i.FirstOrDefault().DiaChi,
                                                         Lop = i.FirstOrDefault().Lop,
                                                         TruongTHPT = i.FirstOrDefault().TruongTHPT,
                                                         TenTruongTHPT = i.FirstOrDefault().TenTruongTHPT,
                                                         LoaiDuThi = i.FirstOrDefault().LoaiDuThi,
                                                         DonViDKDT = i.FirstOrDefault().DonViDKDT,
                                                         LaoDong = i.FirstOrDefault().LaoDong,
                                                         VanHoa = i.FirstOrDefault().VanHoa,
                                                         RLTT = i.FirstOrDefault().RLTT,
                                                         XepLoaiHanhKiem = i.FirstOrDefault().XepLoaiHanhKiem,
                                                         XepLoaiHanhKiemStr = i.FirstOrDefault().XepLoaiHanhKiemStr,
                                                         Do = i.FirstOrDefault().Do,
                                                         DoThem = i.FirstOrDefault().DoThem,
                                                         Hong = i.FirstOrDefault().Hong,
                                                         XepLoaiHocLuc = i.FirstOrDefault().XepLoaiHocLuc,
                                                         XepLoaiHocLucStr = i.FirstOrDefault().XepLoaiHocLucStr,
                                                         DiemTBLop12 = i.FirstOrDefault().DiemTBLop12,
                                                         DiemXL = i.FirstOrDefault().DiemXL,
                                                         DiemUT = i.FirstOrDefault().DiemUT,
                                                         DiemKK = i.FirstOrDefault().DiemKK,
                                                         DienXTN = i.FirstOrDefault().DienXTN,
                                                         HoiDongThi = i.FirstOrDefault().HoiDongThi,
                                                         DiemXetTotNghiep = i.FirstOrDefault().DiemXetTotNghiep,
                                                         KetQuaTotNghiep = i.FirstOrDefault().KetQuaTotNghiep,
                                                         SoHieuBang = i.FirstOrDefault().SoHieuBang,
                                                         VaoSoCapBangSo = i.FirstOrDefault().VaoSoCapBangSo,
                                                         NgayCapBang = i.FirstOrDefault().NgayCapBang,
                                                         NamThi = i.FirstOrDefault().NamThi,
                                                         TrangThai = i.FirstOrDefault().TrangThaiThiSinh,
                                                         TrangThaiCapBang = i.FirstOrDefault().TrangThaiCapBang,
                                                         TongSoDiemThi = i.FirstOrDefault().TongSoDiemThi,
                                                         GhiChu = i.FirstOrDefault().GhiChuThiSinh,
                                                         Hang = i.FirstOrDefault().Hang,
                                                         DiemTBCacBaiThi = i.FirstOrDefault().DiemTBCacBaiThi,
                                                         DienUuTien = i.FirstOrDefault().DienUuTien,
                                                         DiemTBC = i.FirstOrDefault().DiemTBC,
                                                         ListThongTinDiemThi = (from j in chiTiets.Where(x => x.DiemThiID > 0 && x.ThiSinhID == i.FirstOrDefault().ThiSinhID).ToList().GroupBy(x => x.DiemThiID)
                                                                                select new ThongTinDiemThi()
                                                                                {
                                                                                    DiemThiID = j.FirstOrDefault().DiemThiID,
                                                                                    ThiSinhID = j.FirstOrDefault().ThiSinhID,
                                                                                    MonThiID = j.FirstOrDefault().MonThiID,
                                                                                    Diem = j.FirstOrDefault().Diem,
                                                                                    DiemBaiToHop = j.FirstOrDefault().DiemBaiToHop,
                                                                                    NhomID = j.FirstOrDefault().NhomID,
                                                                                }
                                                               ).ToList(),
                                                     }
                                  ).ToList(),
                              }
                            ).ToList();
                }

                return Result.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }
        public DuLieuDiemThiModel CheckTrungSoQuyenSoTrangTheoNam(int? SoTrang, string SoQuyen, int Nam)
        {
            DuLieuDiemThiModel Result = new DuLieuDiemThiModel();
            Result.ThongTinToChucThi = new ThongTinToChucThi();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SoTrang",SqlDbType.Int),
                  new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                  new SqlParameter("@Nam",SqlDbType.Int),           
            };
            parameters[0].Value = SoTrang ?? Convert.DBNull;
            parameters[1].Value = SoQuyen ?? Convert.DBNull;
            parameters[2].Value = Nam;
         
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinToChucThi_GetBySoTrangSoQuyenTheoNam", parameters))
                {
                    while (dr.Read())
                    {
                        Result.ThongTinToChucThi.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        break;
                    }
                    dr.Close();
                }


                return Result;
            }
            catch
            {
                throw;
            }
        }
        public BaseResultModel InsertThiSinhLog(ThongTinThiSinhLog ThiSinhLog, int? ThaoTac, int? CanBoID, int LoaiGhiLog)
        {
            var Result = new BaseResultModel();
            try
            {
                if (ThiSinhLog == null)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin thí sinh log không được để trống";
                    return Result;
                }
                else
                {
                    var ThoiGianLog = DateTime.Now;
                    SqlParameter[] parms_ts = new SqlParameter[]{
                           new SqlParameter("ThiSinhID", SqlDbType.Int),
                           new SqlParameter("KyThiID", SqlDbType.Int),
                           new SqlParameter("HoTen", SqlDbType.NVarChar),
                           new SqlParameter("NgaySinh", SqlDbType.DateTime),
                           new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                           new SqlParameter("GioiTinh", SqlDbType.Bit),
                           new SqlParameter("DanToc", SqlDbType.Int),
                           new SqlParameter("CMND", SqlDbType.NVarChar),
                           new SqlParameter("SoBaoDanh", SqlDbType.NVarChar),
                           new SqlParameter("SoDienThoai", SqlDbType.NVarChar),
                           new SqlParameter("DiaChi", SqlDbType.NVarChar),
                           new SqlParameter("Lop", SqlDbType.NVarChar),
                           new SqlParameter("TruongTHPT", SqlDbType.Int),
                           new SqlParameter("LoaiDuThi", SqlDbType.NVarChar),
                           new SqlParameter("DonViDKDT", SqlDbType.NVarChar),
                           new SqlParameter("XepLoaiHanhKiem", SqlDbType.Int),
                           new SqlParameter("XepLoaiHocLuc", SqlDbType.Int),
                           new SqlParameter("DiemTBLop12", SqlDbType.Decimal),
                           new SqlParameter("DiemKK", SqlDbType.Decimal),
                           new SqlParameter("DienXTN", SqlDbType.Int),
                           new SqlParameter("HoiDongThi", SqlDbType.Int),
                           new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal),
                           new SqlParameter("KetQuaTotNghiep", SqlDbType.NVarChar),
                           new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                           new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                           new SqlParameter("NamThi", SqlDbType.Int),
                           new SqlParameter("Do", SqlDbType.NVarChar),
                           new SqlParameter("DoThem", SqlDbType.NVarChar),
                           new SqlParameter("Hong", SqlDbType.NVarChar),
                           new SqlParameter("LaoDong", SqlDbType.NVarChar),
                           new SqlParameter("VanHoa", SqlDbType.NVarChar),
                           new SqlParameter("RLTT", SqlDbType.NVarChar),
                           new SqlParameter("TongSoDiemThi", SqlDbType.Decimal),
                           new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                           new SqlParameter("DiemXL", SqlDbType.Decimal),
                           new SqlParameter("DiemUT", SqlDbType.Decimal),
                           new SqlParameter("GhiChu", SqlDbType.NVarChar),
                           new SqlParameter("Hang", SqlDbType.NVarChar),
                           new SqlParameter("DiemTBCacBaiThi", SqlDbType.Decimal),
                           new SqlParameter("DienUuTien", SqlDbType.NVarChar),
                           new SqlParameter("DiemTBC", SqlDbType.Decimal),
                           new SqlParameter("KyThiMoiID", SqlDbType.Int),
                           new SqlParameter("HoTenMoi", SqlDbType.NVarChar),
                           new SqlParameter("NgaySinhMoi", SqlDbType.DateTime),
                           new SqlParameter("NoiSinhMoi", SqlDbType.NVarChar),
                           new SqlParameter("GioiTinhMoi", SqlDbType.Bit),
                           new SqlParameter("DanTocMoi", SqlDbType.Int),
                           new SqlParameter("CMNDMoi", SqlDbType.NVarChar),
                           new SqlParameter("SoBaoDanhMoi", SqlDbType.NVarChar),
                           new SqlParameter("SoDienThoaiMoi", SqlDbType.NVarChar),
                           new SqlParameter("DiaChiMoi", SqlDbType.NVarChar),
                           new SqlParameter("LopMoi", SqlDbType.NVarChar),
                           new SqlParameter("TruongTHPTMoi", SqlDbType.Int),
                           new SqlParameter("LoaiDuThiMoi", SqlDbType.NVarChar),
                           new SqlParameter("DonViDKDTMoi", SqlDbType.NVarChar),
                           new SqlParameter("XepLoaiHanhKiemMoi", SqlDbType.Int),
                           new SqlParameter("XepLoaiHocLucMoi", SqlDbType.Int),
                           new SqlParameter("DiemTBLop12Moi", SqlDbType.Decimal),
                           new SqlParameter("DiemKKMoi", SqlDbType.Decimal),
                           new SqlParameter("DienXTNMoi", SqlDbType.Int),
                           new SqlParameter("HoiDongThiMoi", SqlDbType.Int),
                           new SqlParameter("DiemXetTotNghiepMoi", SqlDbType.Decimal),
                           new SqlParameter("KetQuaTotNghiepMoi", SqlDbType.NVarChar),
                           new SqlParameter("SoHieuBangMoi", SqlDbType.NVarChar),
                           new SqlParameter("VaoSoCapBangSoMoi", SqlDbType.NVarChar),
                           new SqlParameter("NamThiMoi", SqlDbType.Int),
                           new SqlParameter("DoMoi", SqlDbType.NVarChar),
                           new SqlParameter("DoThemMoi", SqlDbType.NVarChar),
                           new SqlParameter("HongMoi", SqlDbType.NVarChar),
                           new SqlParameter("LaoDongMoi", SqlDbType.NVarChar),
                           new SqlParameter("VanHoaMoi", SqlDbType.NVarChar),
                           new SqlParameter("RLTTMoi", SqlDbType.NVarChar),
                           new SqlParameter("TongSoDiemThiMoi", SqlDbType.Decimal),
                           new SqlParameter("NgayCapBangMoi", SqlDbType.DateTime),
                           new SqlParameter("DiemXLMoi", SqlDbType.Decimal),
                           new SqlParameter("DiemUTMoi", SqlDbType.Decimal),
                           new SqlParameter("GhiChuMoi", SqlDbType.NVarChar),
                           new SqlParameter("HangMoi", SqlDbType.NVarChar),
                           new SqlParameter("DiemTBCacBaiThiMoi", SqlDbType.Decimal),
                           new SqlParameter("DienUuTienMoi", SqlDbType.NVarChar),
                           new SqlParameter("DiemTBCMoi", SqlDbType.Decimal),
                           new SqlParameter("ThaoTac", SqlDbType.Int),
                           new SqlParameter("@NgayThaoTac", SqlDbType.DateTime),
                           new SqlParameter("@NguoiThaoTac", SqlDbType.Int),
                     };
                    parms_ts[0].Value = ThiSinhLog.ThiSinhID;
                    parms_ts[1].Value = ThiSinhLog.KyThiCuID ?? Convert.DBNull;
                    parms_ts[2].Value = ThiSinhLog.HoTenCu ?? Convert.DBNull;
                    parms_ts[3].Value = ThiSinhLog.NgaySinhCu ?? Convert.DBNull;
                    parms_ts[4].Value = ThiSinhLog.NoiSinhCu ?? Convert.DBNull;
                    parms_ts[5].Value = ThiSinhLog.GioiTinhCu ?? Convert.DBNull;
                    parms_ts[6].Value = ThiSinhLog.DanTocCu ?? Convert.DBNull;
                    parms_ts[7].Value = ThiSinhLog.CMNDCu ?? Convert.DBNull;
                    parms_ts[8].Value = ThiSinhLog.SoBaoDanhCu ?? Convert.DBNull;
                    parms_ts[9].Value = ThiSinhLog.SoDienThoaiCu ?? Convert.DBNull;
                    parms_ts[10].Value = ThiSinhLog.DiaChiCu ?? Convert.DBNull;
                    parms_ts[11].Value = ThiSinhLog.LopCu ?? Convert.DBNull;
                    parms_ts[12].Value = ThiSinhLog.TruongTHPTCu ?? Convert.DBNull;
                    parms_ts[13].Value = ThiSinhLog.LoaiDuThiCu ?? Convert.DBNull;
                    parms_ts[14].Value = ThiSinhLog.DonViDKDTCu ?? Convert.DBNull;
                    parms_ts[15].Value = ThiSinhLog.XepLoaiHanhKiemCu ?? Convert.DBNull;
                    parms_ts[16].Value = ThiSinhLog.XepLoaiHocLucCu ?? Convert.DBNull;
                    parms_ts[17].Value = ThiSinhLog.DiemTBLop12Cu ?? Convert.DBNull;
                    parms_ts[18].Value = ThiSinhLog.DiemKKCu ?? Convert.DBNull;
                    parms_ts[19].Value = ThiSinhLog.DienXTNCu ?? Convert.DBNull;
                    parms_ts[20].Value = ThiSinhLog.HoiDongThiCu ?? Convert.DBNull;
                    parms_ts[21].Value = ThiSinhLog.DiemXetTotNghiepCu ?? Convert.DBNull;
                    parms_ts[22].Value = ThiSinhLog.KetQuaTotNghiepCu ?? Convert.DBNull;
                    parms_ts[23].Value = ThiSinhLog.SoHieuBangCu ?? Convert.DBNull;
                    parms_ts[24].Value = ThiSinhLog.VaoSoCapBangSoCu ?? Convert.DBNull;
                    parms_ts[25].Value = ThiSinhLog.NamThiCu ?? Convert.DBNull;
                    parms_ts[26].Value = ThiSinhLog.DoCu ?? Convert.DBNull;
                    parms_ts[27].Value = ThiSinhLog.DoThemCu ?? Convert.DBNull;
                    parms_ts[28].Value = ThiSinhLog.HongCu ?? Convert.DBNull;
                    parms_ts[29].Value = ThiSinhLog.LaoDongCu ?? Convert.DBNull;
                    parms_ts[30].Value = ThiSinhLog.VanHoaCu ?? Convert.DBNull;
                    parms_ts[31].Value = ThiSinhLog.RLTTCu ?? Convert.DBNull;
                    parms_ts[32].Value = ThiSinhLog.TongSoDiemThiCu ?? Convert.DBNull;
                    parms_ts[33].Value = ThiSinhLog.NgayCapBangCu ?? Convert.DBNull;
                    parms_ts[34].Value = ThiSinhLog.DiemXLCu ?? Convert.DBNull;
                    parms_ts[35].Value = ThiSinhLog.DiemUTCu ?? Convert.DBNull;
                    parms_ts[36].Value = ThiSinhLog.GhiChuCu ?? Convert.DBNull;
                    parms_ts[37].Value = ThiSinhLog.HangCu ?? Convert.DBNull;
                    parms_ts[38].Value = ThiSinhLog.DiemTBCacBaiThiCu ?? Convert.DBNull;
                    parms_ts[39].Value = ThiSinhLog.DienUuTienCu ?? Convert.DBNull;
                    parms_ts[40].Value = ThiSinhLog.DiemTBCCu ?? Convert.DBNull;
                    parms_ts[41].Value = ThiSinhLog.KyThiMoiID ?? Convert.DBNull;
                    parms_ts[42].Value = ThiSinhLog.HoTenMoi ?? Convert.DBNull;
                    parms_ts[43].Value = ThiSinhLog.NgaySinhMoi ?? Convert.DBNull;
                    parms_ts[44].Value = ThiSinhLog.NoiSinhMoi ?? Convert.DBNull;
                    parms_ts[45].Value = ThiSinhLog.GioiTinhMoi ?? Convert.DBNull;
                    parms_ts[46].Value = ThiSinhLog.DanTocMoi ?? Convert.DBNull;
                    parms_ts[47].Value = ThiSinhLog.CMNDMoi ?? Convert.DBNull;
                    parms_ts[48].Value = ThiSinhLog.SoBaoDanhMoi ?? Convert.DBNull;
                    parms_ts[49].Value = ThiSinhLog.SoDienThoaiMoi ?? Convert.DBNull;
                    parms_ts[50].Value = ThiSinhLog.DiaChiMoi ?? Convert.DBNull;
                    parms_ts[51].Value = ThiSinhLog.LopMoi ?? Convert.DBNull;
                    parms_ts[52].Value = ThiSinhLog.TruongTHPTMoi ?? Convert.DBNull;
                    parms_ts[53].Value = ThiSinhLog.LoaiDuThiMoi ?? Convert.DBNull;
                    parms_ts[54].Value = ThiSinhLog.DonViDKDTMoi ?? Convert.DBNull;
                    parms_ts[55].Value = ThiSinhLog.XepLoaiHanhKiemMoi ?? Convert.DBNull;
                    parms_ts[56].Value = ThiSinhLog.XepLoaiHocLucMoi ?? Convert.DBNull;
                    parms_ts[57].Value = ThiSinhLog.DiemTBLop12Moi ?? Convert.DBNull;
                    parms_ts[58].Value = ThiSinhLog.DiemKKMoi ?? Convert.DBNull;
                    parms_ts[59].Value = ThiSinhLog.DienXTNMoi ?? Convert.DBNull;
                    parms_ts[60].Value = ThiSinhLog.HoiDongThiMoi ?? Convert.DBNull;
                    parms_ts[61].Value = ThiSinhLog.DiemXetTotNghiepMoi ?? Convert.DBNull;
                    parms_ts[62].Value = ThiSinhLog.KetQuaTotNghiepMoi ?? Convert.DBNull;
                    parms_ts[63].Value = ThiSinhLog.SoHieuBangMoi ?? Convert.DBNull;
                    parms_ts[64].Value = ThiSinhLog.VaoSoCapBangSoMoi ?? Convert.DBNull;
                    parms_ts[65].Value = ThiSinhLog.NamThiMoi ?? Convert.DBNull;
                    parms_ts[66].Value = ThiSinhLog.DoMoi ?? Convert.DBNull;
                    parms_ts[67].Value = ThiSinhLog.DoThemMoi ?? Convert.DBNull;
                    parms_ts[68].Value = ThiSinhLog.HongMoi ?? Convert.DBNull;
                    parms_ts[69].Value = ThiSinhLog.LaoDongMoi ?? Convert.DBNull;
                    parms_ts[70].Value = ThiSinhLog.VanHoaMoi ?? Convert.DBNull;
                    parms_ts[71].Value = ThiSinhLog.RLTTMoi ?? Convert.DBNull;
                    parms_ts[72].Value = ThiSinhLog.TongSoDiemThiMoi ?? Convert.DBNull;
                    parms_ts[73].Value = ThiSinhLog.NgayCapBangMoi ?? Convert.DBNull;
                    parms_ts[74].Value = ThiSinhLog.DiemXLMoi ?? Convert.DBNull;
                    parms_ts[75].Value = ThiSinhLog.DiemUTMoi ?? Convert.DBNull;
                    parms_ts[76].Value = ThiSinhLog.GhiChuMoi ?? Convert.DBNull;
                    parms_ts[77].Value = ThiSinhLog.HangMoi ?? Convert.DBNull;
                    parms_ts[78].Value = ThiSinhLog.DiemTBCacBaiThiMoi ?? Convert.DBNull;
                    parms_ts[79].Value = ThiSinhLog.DienUuTienMoi ?? Convert.DBNull;
                    parms_ts[80].Value = ThiSinhLog.DiemTBCMoi ?? Convert.DBNull;
                    parms_ts[81].Value = ThaoTac ?? Convert.DBNull;
                    parms_ts[82].Value = ThoiGianLog;
                    parms_ts[83].Value = CanBoID ?? Convert.DBNull;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                //LoaiGhiLog = 1: Log cả sửa thông tin thí sinh, điểm ; = 2: log chỉ thông tin thí sinh; = 3: log chỉ thông tin bảng ddiểm
                                if (LoaiGhiLog == 1)
                                {
                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_ThiSinhLog_Insert", parms_ts);
                                    //if (ThiSinhLog.ListThongTinDiemThiMoi != null && ThiSinhLog.ListThongTinDiemThiMoi.Count > 0)
                                    //{
                                    foreach (var DiemThiLog in ThiSinhLog.ListThongTinDiemThiMoi)
                                    {
                                        SqlParameter[] parms_dt = new SqlParameter[]
                                        {
                                                    new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                    new SqlParameter("MonThiID", SqlDbType.Int),
                                                    new SqlParameter("Diem", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                    new SqlParameter("NhomID", SqlDbType.Int),
                                                    new SqlParameter("DiemMoi", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHopMoi", SqlDbType.NVarChar),
                                                    new SqlParameter("@NgayThaoTac", SqlDbType.DateTime),
                                                    new SqlParameter("@NguoiThaoTac", SqlDbType.NVarChar),
                                                     new SqlParameter("@ThaoTac", SqlDbType.NVarChar),
                                         };

                                        parms_dt[0].Value = ThiSinhLog.ThiSinhID;
                                        parms_dt[1].Value = DiemThiLog.MonThiID ?? Convert.DBNull;
                                        parms_dt[2].Value = DiemThiLog.DiemCu ?? Convert.DBNull;
                                        parms_dt[3].Value = DiemThiLog.DiemBaiToHopCu ?? Convert.DBNull;
                                        parms_dt[4].Value = DiemThiLog.NhomID ?? Convert.DBNull;
                                        parms_dt[5].Value = DiemThiLog.DiemMoi ?? Convert.DBNull;
                                        parms_dt[6].Value = DiemThiLog.DiemBaiToHopMoi ?? Convert.DBNull;
                                        parms_dt[7].Value = ThoiGianLog;
                                        parms_dt[8].Value = CanBoID;
                                        parms_dt[9].Value = ThaoTac;

                                        SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_BangGhiDiemThiLog_Insert", parms_dt);
                                    }
                                    //}
                                }
                                else if (LoaiGhiLog == 2)
                                {
                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_ThiSinhLog_Insert", parms_ts);
                                }
                                else if (LoaiGhiLog == 3)
                                {
                                    foreach (var DiemThiLog in ThiSinhLog.ListThongTinDiemThiMoi)
                                    {
                                        SqlParameter[] parms_dt = new SqlParameter[]
                                        {
                                                    new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                    new SqlParameter("MonThiID", SqlDbType.Int),
                                                    new SqlParameter("Diem", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHop", SqlDbType.NVarChar),
                                                    new SqlParameter("NhomID", SqlDbType.Int),
                                                    new SqlParameter("DiemMoi", SqlDbType.Decimal),
                                                    new SqlParameter("DiemBaiToHopMoi", SqlDbType.NVarChar),
                                                    new SqlParameter("@NgayThaoTac", SqlDbType.DateTime),
                                                    new SqlParameter("@NguoiThaoTac", SqlDbType.NVarChar),
                                                    new SqlParameter("@ThaoTac", SqlDbType.NVarChar),
                                         };

                                        parms_dt[0].Value = ThiSinhLog.ThiSinhID;
                                        parms_dt[1].Value = DiemThiLog.MonThiID ?? Convert.DBNull;
                                        parms_dt[2].Value = DiemThiLog.DiemCu ?? Convert.DBNull;
                                        parms_dt[3].Value = DiemThiLog.DiemBaiToHopCu ?? Convert.DBNull;
                                        parms_dt[4].Value = DiemThiLog.NhomID ?? Convert.DBNull;
                                        parms_dt[5].Value = DiemThiLog.DiemMoi ?? Convert.DBNull;
                                        parms_dt[6].Value = DiemThiLog.DiemBaiToHopMoi ?? Convert.DBNull;
                                        parms_dt[7].Value = ThoiGianLog;
                                        parms_dt[8].Value = CanBoID;
                                        parms_dt[9].Value = ThaoTac;


                                        SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_BangGhiDiemThiLog_Insert", parms_dt);
                                    }
                                }

                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("Lưu log chỉnh sửa thông tin thí sinh thành công!");
                                //Result.Data = KyThiID;
                            }
                            catch (Exception ex)
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public List<ThiSinhLogModel> GetThiSinhLog(int KyThiID)
        {
            var Result = new List<ThiSinhLogModel>();
            var BangDiemThiLog = new List<ThiSinhLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("KyThiID", SqlDbType.Int),
           };
            parameters[0].Value = KyThiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_NV_ThiSinhLog_GetByKyThiID", parameters))
                {
                    while (dr.Read())
                    {
                        var itemThiSinhLog = new ThongTinThiSinhLog();
                        itemThiSinhLog.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        itemThiSinhLog.KyThiCuID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        itemThiSinhLog.HoTenCu = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        itemThiSinhLog.HoTenMoi = Utils.ConvertToString(dr["HoTenMoi"], string.Empty);
                        itemThiSinhLog.NgaySinhCu = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        itemThiSinhLog.NgaySinhMoi = Utils.ConvertToNullableDateTime(dr["NgaySinhMoi"], null);
                        itemThiSinhLog.NoiSinhCu = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        itemThiSinhLog.NoiSinhMoi = Utils.ConvertToString(dr["NoiSinhMoi"], string.Empty);
                        itemThiSinhLog.GioiTinhCu = Utils.ConvertToBoolean(dr["GioiTinh"], false);
                        itemThiSinhLog.GioiTinhMoi = Utils.ConvertToBoolean(dr["GioiTinhMoi"], false);
                        itemThiSinhLog.DanTocCu = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        itemThiSinhLog.DanTocMoi = Utils.ConvertToNullableInt32(dr["DanTocMoi"], null);
                        itemThiSinhLog.DanTocStrCu = Utils.ConvertToString(dr["DanTocStrCu"], string.Empty);
                        itemThiSinhLog.DanTocStrMoi = Utils.ConvertToString(dr["DanTocStrMoi"], string.Empty);
                        itemThiSinhLog.CMNDCu = Utils.ConvertToString(dr["CMND"], string.Empty);
                        itemThiSinhLog.CMNDMoi = Utils.ConvertToString(dr["CMNDMoi"], string.Empty);
                        itemThiSinhLog.SoBaoDanhCu = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        itemThiSinhLog.SoBaoDanhMoi = Utils.ConvertToString(dr["SoBaoDanhMoi"], string.Empty);
                        itemThiSinhLog.SoDienThoaiCu = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        itemThiSinhLog.SoDienThoaiMoi = Utils.ConvertToString(dr["SoDienThoaiMoi"], string.Empty);
                        itemThiSinhLog.DiaChiCu = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        itemThiSinhLog.DiaChiMoi = Utils.ConvertToString(dr["DiaChiMoi"], string.Empty);
                        itemThiSinhLog.LopCu = Utils.ConvertToString(dr["Lop"], string.Empty);
                        itemThiSinhLog.LopMoi = Utils.ConvertToString(dr["LopMoi"], string.Empty);
                        itemThiSinhLog.TenTruongTHPTCu = Utils.ConvertToString(dr["TenTruongTHPTCu"], string.Empty);
                        itemThiSinhLog.TenTruongTHPTMoi = Utils.ConvertToString(dr["TenTruongTHPTMoi"], string.Empty);
                        itemThiSinhLog.LoaiDuThiCu = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        itemThiSinhLog.LoaiDuThiMoi = Utils.ConvertToString(dr["LoaiDuThiMoi"], string.Empty);
                        itemThiSinhLog.DonViDKDTCu = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        itemThiSinhLog.DonViDKDTMoi = Utils.ConvertToString(dr["DonViDKDTMoi"], string.Empty);
                        itemThiSinhLog.XepLoaiHanhKiemStrCu = Utils.ConvertToString(dr["XepLoaiHanhKiemStrCu"], string.Empty);
                        itemThiSinhLog.XepLoaiHanhKiemStrMoi = Utils.ConvertToString(dr["XepLoaiHanhKiemStrMoi"], string.Empty);
                        itemThiSinhLog.XepLoaiHocLucStrCu = Utils.ConvertToString(dr["XepLoaiHocLucStrCu"], string.Empty);
                        itemThiSinhLog.XepLoaiHocLucStrMoi = Utils.ConvertToString(dr["XepLoaiHocLucStrMoi"], string.Empty);
                        itemThiSinhLog.DiemTBLop12Cu = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBLop12"], null);
                        itemThiSinhLog.DiemTBLop12Moi = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBLop12Moi"], null);
                        itemThiSinhLog.DiemKKCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemKK"], null);
                        itemThiSinhLog.DiemKKMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemKKMoi"], null);
                        itemThiSinhLog.DienXTNCu = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        itemThiSinhLog.DienXTNMoi = Utils.ConvertToNullableInt32(dr["DienXTNMoi"], null);
                        itemThiSinhLog.HoiDongThiStrCu = Utils.ConvertToString(dr["HoiDongThiStrCu"], string.Empty);
                        itemThiSinhLog.HoiDongThiStrMoi = Utils.ConvertToString(dr["HoiDongThiStrMoi"], string.Empty);
                        itemThiSinhLog.DiemXetTotNghiepCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemXetTotNghiep"], null);
                        itemThiSinhLog.DiemXetTotNghiepMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemXetTotNghiepMoi"], null);
                        itemThiSinhLog.KetQuaTotNghiepCu = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        itemThiSinhLog.KetQuaTotNghiepMoi = Utils.ConvertToString(dr["KetQuaTotNghiepMoi"], string.Empty);
                        itemThiSinhLog.SoHieuBangCu = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        itemThiSinhLog.SoHieuBangMoi = Utils.ConvertToString(dr["SoHieuBangMoi"], string.Empty);
                        itemThiSinhLog.VaoSoCapBangSoCu = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        itemThiSinhLog.VaoSoCapBangSoMoi = Utils.ConvertToString(dr["VaoSoCapBangSoMoi"], string.Empty);
                        itemThiSinhLog.NgayCapBangCu = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        itemThiSinhLog.NgayCapBangMoi = Utils.ConvertToNullableDateTime(dr["NgayCapBangMoi"], null);
                        itemThiSinhLog.NamThiCu = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        itemThiSinhLog.NamThiMoi = Utils.ConvertToNullableInt32(dr["NamThiMoi"], null);
                        itemThiSinhLog.DoCu = Utils.ConvertToString(dr["Do"], string.Empty);
                        itemThiSinhLog.DoMoi = Utils.ConvertToString(dr["DoMoi"], string.Empty);
                        itemThiSinhLog.DoThemCu = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        itemThiSinhLog.DoThemMoi = Utils.ConvertToString(dr["DoThemMoi"], string.Empty);
                        itemThiSinhLog.HongCu = Utils.ConvertToString(dr["Hong"], string.Empty);
                        itemThiSinhLog.HongMoi = Utils.ConvertToString(dr["HongMoi"], string.Empty);
                        itemThiSinhLog.LaoDongCu = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        itemThiSinhLog.LaoDongMoi = Utils.ConvertToString(dr["LaoDongMoi"], string.Empty);
                        itemThiSinhLog.VanHoaCu = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        itemThiSinhLog.VanHoaMoi = Utils.ConvertToString(dr["VanHoaMoi"], string.Empty);
                        itemThiSinhLog.RLTTCu = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        itemThiSinhLog.RLTTMoi = Utils.ConvertToString(dr["RLTTMoi"], string.Empty);
                        itemThiSinhLog.TongSoDiemThiCu = Utils.ConvertToNullableDecimalFromInt(dr["TongSoDiemThi"], null);
                        itemThiSinhLog.TongSoDiemThiMoi = Utils.ConvertToNullableDecimalFromInt(dr["TongSoDiemThiMoi"], null);
                        itemThiSinhLog.DiemXLCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemXL"], null);
                        itemThiSinhLog.DiemXLMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemXLMoi"], null);
                        itemThiSinhLog.DiemUTCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemUT"], null);
                        itemThiSinhLog.DiemUTMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemUTMoi"], null);
                        itemThiSinhLog.GhiChuCu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        itemThiSinhLog.GhiChuMoi = Utils.ConvertToString(dr["GhiChuMoi"], string.Empty);
                        itemThiSinhLog.HangCu = Utils.ConvertToString(dr["Hang"], string.Empty);
                        itemThiSinhLog.HangMoi = Utils.ConvertToString(dr["HangMoi"], string.Empty);
                        itemThiSinhLog.DiemTBCacBaiThiCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBCacBaiThi"], null);
                        itemThiSinhLog.DiemTBCacBaiThiMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBCacBaiThiMoi"], null);
                        itemThiSinhLog.DienUuTienCu = Utils.ConvertToString(dr["DienUuTien"], null);
                        itemThiSinhLog.DienUuTienMoi = Utils.ConvertToString(dr["DienUuTienMoi"], null);
                        itemThiSinhLog.DiemTBCCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBC"], null);
                        itemThiSinhLog.DiemTBCMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBCMoi"], null);
                        itemThiSinhLog.ThaoTac = Utils.ConvertToNullableInt32(dr["ThaoTac"], null);
                        itemThiSinhLog.CanBoID = Utils.ConvertToNullableInt32(dr["CanBoSuaThongTinID"], null);
                        itemThiSinhLog.TenCanBo = Utils.ConvertToString(dr["TenCanBoSuaThongTin"], null);
                        itemThiSinhLog.NgayChinhSua = Utils.ConvertToNullableDateTime(dr["NgayThaoTac"], null);

                        var item = new ThiSinhLogModel();
                        item.NgayChinhSua = itemThiSinhLog.NgayChinhSua;
                        item.NguoiChinhSua = itemThiSinhLog.TenCanBo;
                        item.ThaoTac = itemThiSinhLog.ThaoTac;
                        item.DanhSachThiSinhChinhSua = new List<ThiSinhChinhSuaModel>();
                        var itemThiSinh = new ThiSinhChinhSuaModel();
                        itemThiSinh.ThiSinhID = itemThiSinhLog.ThiSinhID;
                        itemThiSinh.TenThiSinh = string.IsNullOrEmpty(itemThiSinhLog.HoTenCu) ? itemThiSinhLog.HoTenMoi : itemThiSinhLog.HoTenCu;
                        itemThiSinh.DanhSachChinhSua = new List<CacTruongSua>();
                        if (item.ThaoTac == EnumLog.Insert.GetHashCode())
                        {
                            //var itemTruongChinhSua = new CacTruongSua();
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoTenMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoTenCu,
                                    Moi = itemThiSinhLog.HoTenMoi,
                                    ThongTinChinhSua = "Họ tên"
                                });
                            }
                            if (itemThiSinhLog.NgaySinhMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgaySinhCu == null ? "" : itemThiSinhLog.NgaySinhCu.ToString(),
                                    Moi = itemThiSinhLog.NgaySinhMoi == null ? "" : itemThiSinhLog.NgaySinhMoi.ToString(),
                                    ThongTinChinhSua = "Ngày sinh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.NoiSinhMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NoiSinhCu,
                                    Moi = itemThiSinhLog.NoiSinhMoi,
                                    ThongTinChinhSua = "Nơi sinh"
                                });
                            }
                            if (itemThiSinhLog.GioiTinhMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GioiTinhCu == false ? "Nam" : "Nữ",
                                    Moi = itemThiSinhLog.GioiTinhMoi == false ? "Nam" : "Nữ",
                                    ThongTinChinhSua = "Giới tính"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DanTocStrMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DanTocStrCu,
                                    Moi = itemThiSinhLog.DanTocStrMoi,
                                    ThongTinChinhSua = "Dân tộc"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.CMNDMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.CMNDCu,
                                    Moi = itemThiSinhLog.CMNDMoi,
                                    ThongTinChinhSua = "CMND/CCCD"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoBaoDanhMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoBaoDanhCu,
                                    Moi = itemThiSinhLog.SoBaoDanhMoi,
                                    ThongTinChinhSua = "Số báo danh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoDienThoaiMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoDienThoaiCu,
                                    Moi = itemThiSinhLog.SoDienThoaiMoi,
                                    ThongTinChinhSua = "SĐT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DiaChiMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiaChiCu,
                                    Moi = itemThiSinhLog.DiaChiMoi,
                                    ThongTinChinhSua = "Địa chỉ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LopMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LopCu,
                                    Moi = itemThiSinhLog.LopMoi,
                                    ThongTinChinhSua = "Lớp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.TenTruongTHPTMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TenTruongTHPTCu,
                                    Moi = itemThiSinhLog.TenTruongTHPTMoi,
                                    ThongTinChinhSua = "Trường"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LoaiDuThiMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LoaiDuThiCu,
                                    Moi = itemThiSinhLog.LoaiDuThiMoi,
                                    ThongTinChinhSua = "Loại dự thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DonViDKDTMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DonViDKDTCu,
                                    Moi = itemThiSinhLog.DonViDKDTMoi,
                                    ThongTinChinhSua = "Đơn vị ĐKDT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHanhKiemStrMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHanhKiemStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHanhKiemStrMoi,
                                    ThongTinChinhSua = "Hạnh kiểm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHocLucStrMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHocLucStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHocLucStrMoi,
                                    ThongTinChinhSua = "Học lực"
                                });
                            }
                            if (itemThiSinhLog.DiemTBLop12Moi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBLop12Cu == null ? "" : itemThiSinhLog.DiemTBLop12Cu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBLop12Moi == null ? "" : itemThiSinhLog.DiemTBLop12Moi.ToString(),
                                    ThongTinChinhSua = "Điểm TB lớp 12"
                                });
                            }
                            if (itemThiSinhLog.DiemKKMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemKKCu == null ? "" : itemThiSinhLog.DiemKKCu.ToString(),
                                    Moi = itemThiSinhLog.DiemKKMoi == null ? "" : itemThiSinhLog.DiemKKMoi.ToString(),
                                    ThongTinChinhSua = "Điểm khuyến khích"
                                });
                            }
                            if (itemThiSinhLog.DienXTNMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienXTNCu == null ? "" : itemThiSinhLog.DienXTNCu.ToString(),
                                    Moi = itemThiSinhLog.DienXTNMoi == null ? "" : itemThiSinhLog.DienXTNMoi.ToString(),
                                    ThongTinChinhSua = "Diện xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoiDongThiStrMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoiDongThiStrCu,
                                    Moi = itemThiSinhLog.HoiDongThiStrMoi,
                                    ThongTinChinhSua = "Hội đồng thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXetTotNghiepMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXetTotNghiepCu == null ? "" : itemThiSinhLog.DiemXetTotNghiepCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXetTotNghiepMoi == null ? "" : itemThiSinhLog.DiemXetTotNghiepMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.KetQuaTotNghiepMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.KetQuaTotNghiepCu,
                                    Moi = itemThiSinhLog.KetQuaTotNghiepMoi,
                                    ThongTinChinhSua = "Kết quả tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoHieuBangMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoHieuBangCu,
                                    Moi = itemThiSinhLog.SoHieuBangMoi,
                                    ThongTinChinhSua = "Số hiệu bằng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VaoSoCapBangSoMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VaoSoCapBangSoCu,
                                    Moi = itemThiSinhLog.VaoSoCapBangSoMoi,
                                    ThongTinChinhSua = "Vào sổ cấp bằng số"
                                });
                            }
                            if (itemThiSinhLog.NgayCapBangMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgayCapBangCu == null ? "" : itemThiSinhLog.NgayCapBangCu.ToString(),
                                    Moi = itemThiSinhLog.NgayCapBangMoi == null ? "" : itemThiSinhLog.NgayCapBangMoi.ToString(),
                                    ThongTinChinhSua = "Ngày cấp bằng"
                                });
                            }
                            if (itemThiSinhLog.NamThiMoi != null && itemThiSinhLog.NamThiMoi != 0)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NamThiCu == null || itemThiSinhLog.NamThiCu == 0 ? "" : itemThiSinhLog.NamThiCu.ToString(),
                                    Moi = itemThiSinhLog.NamThiMoi == null || itemThiSinhLog.NamThiMoi == 0 ? "" : itemThiSinhLog.NamThiMoi.ToString(),
                                    ThongTinChinhSua = "Năm thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoCu,
                                    Moi = itemThiSinhLog.DoMoi,
                                    ThongTinChinhSua = "Đỗ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoThemMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoThemCu,
                                    Moi = itemThiSinhLog.DoThemMoi,
                                    ThongTinChinhSua = "Đỗ thêm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HongMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HongCu,
                                    Moi = itemThiSinhLog.HongMoi,
                                    ThongTinChinhSua = "Hỏng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LaoDongMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LaoDongCu,
                                    Moi = itemThiSinhLog.LaoDongMoi,
                                    ThongTinChinhSua = "Lao động"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VanHoaMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VanHoaCu,
                                    Moi = itemThiSinhLog.VanHoaMoi,
                                    ThongTinChinhSua = "Văn hoá"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.RLTTMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.RLTTCu,
                                    Moi = itemThiSinhLog.RLTTMoi,
                                    ThongTinChinhSua = "RLTT"
                                });
                            }
                            if (itemThiSinhLog.TongSoDiemThiMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TongSoDiemThiCu == null ? "" : itemThiSinhLog.TongSoDiemThiCu.ToString(),
                                    Moi = itemThiSinhLog.TongSoDiemThiMoi == null ? "" : itemThiSinhLog.TongSoDiemThiMoi.ToString(),
                                    ThongTinChinhSua = "Tổng số điểm thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXLMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXLCu == null ? "" : itemThiSinhLog.DiemXLCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXLMoi == null ? "" : itemThiSinhLog.DiemXLMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xếp loại"
                                });
                            }
                            if (itemThiSinhLog.DiemUTMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemUTCu == null ? "" : itemThiSinhLog.DiemUTCu.ToString(),
                                    Moi = itemThiSinhLog.DiemUTMoi == null ? "" : itemThiSinhLog.DiemUTMoi.ToString(),
                                    ThongTinChinhSua = "Điểm ưu tiên"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.GhiChuMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GhiChuCu,
                                    Moi = itemThiSinhLog.GhiChuMoi,
                                    ThongTinChinhSua = "Ghi chú"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HangMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HangCu,
                                    Moi = itemThiSinhLog.HangMoi,
                                    ThongTinChinhSua = "Hạng"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCacBaiThiMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCacBaiThiCu == null ? "" : itemThiSinhLog.DiemTBCacBaiThiCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCacBaiThiMoi == null ? "" : itemThiSinhLog.DiemTBCacBaiThiMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TB các bài thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DienUuTienMoi))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienUuTienCu,
                                    Moi = itemThiSinhLog.DienUuTienMoi,
                                    ThongTinChinhSua = "Diện ưu tiên"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCMoi != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCCu == null ? "" : itemThiSinhLog.DiemTBCCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCMoi == null ? "" : itemThiSinhLog.DiemTBCMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TBC"
                                });
                            }
                            item.DanhSachThiSinhChinhSua.Add(itemThiSinh);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoTenCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoTenCu,
                                    Moi = itemThiSinhLog.HoTenMoi,
                                    ThongTinChinhSua = "Họ tên"
                                });
                            }
                            if (itemThiSinhLog.NgaySinhCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgaySinhCu == null ? "" : itemThiSinhLog.NgaySinhCu.ToString(),
                                    Moi = itemThiSinhLog.NgaySinhMoi == null ? "" : itemThiSinhLog.NgaySinhMoi.ToString(),
                                    ThongTinChinhSua = "Ngày sinh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.NoiSinhCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NoiSinhCu,
                                    Moi = itemThiSinhLog.NoiSinhMoi,
                                    ThongTinChinhSua = "Nơi sinh"
                                });
                            }
                            if (itemThiSinhLog.NoiSinhCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GioiTinhCu == false ? "Nam" : "Nữ",
                                    Moi = itemThiSinhLog.GioiTinhMoi == false ? "Nam" : "Nữ",
                                    ThongTinChinhSua = "Giới tính"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DanTocStrCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DanTocStrCu,
                                    Moi = itemThiSinhLog.DanTocStrMoi,
                                    ThongTinChinhSua = "Dân tộc"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.CMNDCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.CMNDCu,
                                    Moi = itemThiSinhLog.CMNDMoi,
                                    ThongTinChinhSua = "CMND/CCCD"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoBaoDanhCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoBaoDanhCu,
                                    Moi = itemThiSinhLog.SoBaoDanhMoi,
                                    ThongTinChinhSua = "Số báo danh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoDienThoaiCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoDienThoaiCu,
                                    Moi = itemThiSinhLog.SoDienThoaiMoi,
                                    ThongTinChinhSua = "SĐT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DiaChiCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiaChiCu,
                                    Moi = itemThiSinhLog.DiaChiMoi,
                                    ThongTinChinhSua = "Địa chỉ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LopCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LopCu,
                                    Moi = itemThiSinhLog.LopMoi,
                                    ThongTinChinhSua = "Lớp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.TenTruongTHPTCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TenTruongTHPTCu,
                                    Moi = itemThiSinhLog.TenTruongTHPTMoi,
                                    ThongTinChinhSua = "Trường"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LoaiDuThiCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LoaiDuThiCu,
                                    Moi = itemThiSinhLog.LoaiDuThiMoi,
                                    ThongTinChinhSua = "Loại dự thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DonViDKDTCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DonViDKDTCu,
                                    Moi = itemThiSinhLog.DonViDKDTMoi,
                                    ThongTinChinhSua = "Đơn vị ĐKDT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHanhKiemStrCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHanhKiemStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHanhKiemStrMoi,
                                    ThongTinChinhSua = "Hạnh kiểm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHocLucStrCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHocLucStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHocLucStrMoi,
                                    ThongTinChinhSua = "Học lực"
                                });
                            }
                            if (itemThiSinhLog.DiemTBLop12Cu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBLop12Cu == null ? "" : itemThiSinhLog.DiemTBLop12Cu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBLop12Moi == null ? "" : itemThiSinhLog.DiemTBLop12Moi.ToString(),
                                    ThongTinChinhSua = "Điểm TB lớp 12"
                                });
                            }
                            if (itemThiSinhLog.DiemKKCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemKKCu == null ? "" : itemThiSinhLog.DiemKKCu.ToString(),
                                    Moi = itemThiSinhLog.DiemKKMoi == null ? "" : itemThiSinhLog.DiemKKMoi.ToString(),
                                    ThongTinChinhSua = "Điểm khuyến khích"
                                });
                            }
                            if (itemThiSinhLog.DienXTNCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienXTNCu == null ? "" : itemThiSinhLog.DienXTNCu.ToString(),
                                    Moi = itemThiSinhLog.DienXTNMoi == null ? "" : itemThiSinhLog.DienXTNMoi.ToString(),
                                    ThongTinChinhSua = "Diện xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoiDongThiStrCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoiDongThiStrCu,
                                    Moi = itemThiSinhLog.HoiDongThiStrMoi,
                                    ThongTinChinhSua = "Hội đồng thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXetTotNghiepCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXetTotNghiepCu == null ? "" : itemThiSinhLog.DiemXetTotNghiepCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXetTotNghiepMoi == null ? "" : itemThiSinhLog.DiemXetTotNghiepMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.KetQuaTotNghiepCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.KetQuaTotNghiepCu,
                                    Moi = itemThiSinhLog.KetQuaTotNghiepMoi,
                                    ThongTinChinhSua = "Kết quả tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoHieuBangCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoHieuBangCu,
                                    Moi = itemThiSinhLog.SoHieuBangMoi,
                                    ThongTinChinhSua = "Số hiệu bằng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VaoSoCapBangSoCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VaoSoCapBangSoCu,
                                    Moi = itemThiSinhLog.VaoSoCapBangSoMoi,
                                    ThongTinChinhSua = "Vào sổ cấp bằng số"
                                });
                            }
                            if (itemThiSinhLog.NgayCapBangCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgayCapBangCu == null ? "" : itemThiSinhLog.NgayCapBangCu.ToString(),
                                    Moi = itemThiSinhLog.NgayCapBangMoi == null ? "" : itemThiSinhLog.NgayCapBangMoi.ToString(),
                                    ThongTinChinhSua = "Ngày cấp bằng"
                                });
                            }
                            if (itemThiSinhLog.NamThiCu != null && itemThiSinhLog.NamThiCu != 0)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NamThiCu == null || itemThiSinhLog.NamThiCu == 0 ? "" : itemThiSinhLog.NamThiCu.ToString(),
                                    Moi = itemThiSinhLog.NamThiMoi == null || itemThiSinhLog.NamThiMoi == 0 ? "" : itemThiSinhLog.NamThiMoi.ToString(),
                                    ThongTinChinhSua = "Năm thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoCu,
                                    Moi = itemThiSinhLog.DoMoi,
                                    ThongTinChinhSua = "Đỗ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoThemCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoThemCu,
                                    Moi = itemThiSinhLog.DoThemMoi,
                                    ThongTinChinhSua = "Đỗ thêm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HongCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HongCu,
                                    Moi = itemThiSinhLog.HongMoi,
                                    ThongTinChinhSua = "Hỏng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LaoDongCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LaoDongCu,
                                    Moi = itemThiSinhLog.LaoDongMoi,
                                    ThongTinChinhSua = "Lao động"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VanHoaCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VanHoaCu,
                                    Moi = itemThiSinhLog.VanHoaMoi,
                                    ThongTinChinhSua = "Văn hoá"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.RLTTCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.RLTTCu,
                                    Moi = itemThiSinhLog.RLTTMoi,
                                    ThongTinChinhSua = "RLTT"
                                });
                            }
                            if (itemThiSinhLog.TongSoDiemThiCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TongSoDiemThiCu == null ? "" : itemThiSinhLog.TongSoDiemThiCu.ToString(),
                                    Moi = itemThiSinhLog.TongSoDiemThiMoi == null ? "" : itemThiSinhLog.TongSoDiemThiMoi.ToString(),
                                    ThongTinChinhSua = "Tổng số điểm thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXLCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXLCu == null ? "" : itemThiSinhLog.DiemXLCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXLMoi == null ? "" : itemThiSinhLog.DiemXLMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xếp loại"
                                });
                            }
                            if (itemThiSinhLog.DiemUTCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemUTCu == null ? "" : itemThiSinhLog.DiemUTCu.ToString(),
                                    Moi = itemThiSinhLog.DiemUTMoi == null ? "" : itemThiSinhLog.DiemUTMoi.ToString(),
                                    ThongTinChinhSua = "Điểm ưu tiên"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.GhiChuCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GhiChuCu,
                                    Moi = itemThiSinhLog.GhiChuMoi,
                                    ThongTinChinhSua = "Ghi chú"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HangCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HangCu,
                                    Moi = itemThiSinhLog.HangMoi,
                                    ThongTinChinhSua = "Hạng"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCacBaiThiCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCacBaiThiCu == null ? "" : itemThiSinhLog.DiemTBCacBaiThiCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCacBaiThiMoi == null ? "" : itemThiSinhLog.DiemTBCacBaiThiMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TB các bài thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DienUuTienCu))
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienUuTienCu,
                                    Moi = itemThiSinhLog.DienUuTienMoi,
                                    ThongTinChinhSua = "Diện ưu tiên"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCCu != null)
                            {
                                itemThiSinh.DanhSachChinhSua.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCCu == null ? "" : itemThiSinhLog.DiemTBCCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCMoi == null ? "" : itemThiSinhLog.DiemTBCMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TBC"
                                });
                            }
                            item.DanhSachThiSinhChinhSua.Add(itemThiSinh);
                        }
                        Result.Add(item);
                    }
                    dr.Close();
                }

                //using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_NV_BangDiemGhiDiemLog_GetByKyThiID", parameters))
                //{
                //    while (dr.Read())
                //    {
                //        var itemDiemThiLog = new ThongTinDiemThiLog();
                //        itemDiemThiLog.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                //        itemDiemThiLog.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                //        itemDiemThiLog.TenMonThi = Utils.ConvertToString(dr["TenMonThi"], string.Empty);
                //        itemDiemThiLog.DiemCu = Utils.ConvertToNullableDecimalFromInt(dr["Diem"], null);
                //        itemDiemThiLog.DiemMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemMoi"], null);
                //        itemDiemThiLog.DiemBaiToHopCu = Utils.ConvertToString(dr["DiemBaiToHop"], string.Empty);
                //        itemDiemThiLog.DiemBaiToHopMoi = Utils.ConvertToString(dr["DiemBaiToHopMoi"], string.Empty);
                //        itemDiemThiLog.ThaoTac = Utils.ConvertToNullableInt32(dr["ThaoTac"], null);
                //        itemDiemThiLog.NgayChinhSua = Utils.ConvertToNullableDateTime(dr["NgayThaoTac"], null);

                //        var itemThiSinhLog = new ThiSinhLogModel();
                //        itemThiSinhLog.Th
                //    }
                //    dr.Close();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        public List<CacTruongSua> GetChiTietThiSinhLog(int ThiSinhID, DateTime? NgayChinhSua)
        {
            //var Result = new List<CacTruongSua>();
            //var BangDiemThiLog = new List<CacTruongSua>();
            var ThongTinThiSinhLog = new List<CacTruongSua>();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("ThiSinhID", SqlDbType.Int),
               new SqlParameter("NgayChinhSua", SqlDbType.DateTime),
           };
            parameters[0].Value = ThiSinhID;
            parameters[1].Value = NgayChinhSua;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_NV_ThiSinhLog_GetByThiSinhID", parameters))
                {
                    while (dr.Read())
                    {
                        var itemThiSinhLog = new ThongTinThiSinhLog();
                        itemThiSinhLog.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        itemThiSinhLog.KyThiCuID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        itemThiSinhLog.HoTenCu = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        itemThiSinhLog.HoTenMoi = Utils.ConvertToString(dr["HoTenMoi"], string.Empty);
                        itemThiSinhLog.NgaySinhCu = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        itemThiSinhLog.NgaySinhMoi = Utils.ConvertToNullableDateTime(dr["NgaySinhMoi"], null);
                        itemThiSinhLog.NoiSinhCu = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        itemThiSinhLog.NoiSinhMoi = Utils.ConvertToString(dr["NoiSinhMoi"], string.Empty);
                        itemThiSinhLog.GioiTinhCu = Utils.ConvertToBoolean(dr["GioiTinh"], false);
                        itemThiSinhLog.GioiTinhMoi = Utils.ConvertToBoolean(dr["GioiTinhMoi"], false);
                        itemThiSinhLog.DanTocCu = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        itemThiSinhLog.DanTocMoi = Utils.ConvertToNullableInt32(dr["DanTocMoi"], null);
                        itemThiSinhLog.DanTocStrCu = Utils.ConvertToString(dr["DanTocStrCu"], string.Empty);
                        itemThiSinhLog.DanTocStrMoi = Utils.ConvertToString(dr["DanTocStrMoi"], string.Empty);
                        itemThiSinhLog.CMNDCu = Utils.ConvertToString(dr["CMND"], string.Empty);
                        itemThiSinhLog.CMNDMoi = Utils.ConvertToString(dr["CMNDMoi"], string.Empty);
                        itemThiSinhLog.SoBaoDanhCu = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        itemThiSinhLog.SoBaoDanhMoi = Utils.ConvertToString(dr["SoBaoDanhMoi"], string.Empty);
                        itemThiSinhLog.SoDienThoaiCu = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        itemThiSinhLog.SoDienThoaiMoi = Utils.ConvertToString(dr["SoDienThoaiMoi"], string.Empty);
                        itemThiSinhLog.DiaChiCu = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        itemThiSinhLog.DiaChiMoi = Utils.ConvertToString(dr["DiaChiMoi"], string.Empty);
                        itemThiSinhLog.LopCu = Utils.ConvertToString(dr["Lop"], string.Empty);
                        itemThiSinhLog.LopMoi = Utils.ConvertToString(dr["LopMoi"], string.Empty);
                        itemThiSinhLog.TenTruongTHPTCu = Utils.ConvertToString(dr["TenTruongTHPTCu"], string.Empty);
                        itemThiSinhLog.TenTruongTHPTMoi = Utils.ConvertToString(dr["TenTruongTHPTMoi"], string.Empty);
                        itemThiSinhLog.LoaiDuThiCu = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        itemThiSinhLog.LoaiDuThiMoi = Utils.ConvertToString(dr["LoaiDuThiMoi"], string.Empty);
                        itemThiSinhLog.DonViDKDTCu = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        itemThiSinhLog.DonViDKDTMoi = Utils.ConvertToString(dr["DonViDKDTMoi"], string.Empty);
                        itemThiSinhLog.XepLoaiHanhKiemStrCu = Utils.ConvertToString(dr["XepLoaiHanhKiemStrCu"], string.Empty);
                        itemThiSinhLog.XepLoaiHanhKiemStrMoi = Utils.ConvertToString(dr["XepLoaiHanhKiemStrMoi"], string.Empty);
                        itemThiSinhLog.XepLoaiHocLucStrCu = Utils.ConvertToString(dr["XepLoaiHocLucStrCu"], string.Empty);
                        itemThiSinhLog.XepLoaiHocLucStrMoi = Utils.ConvertToString(dr["XepLoaiHocLucStrMoi"], string.Empty);
                        itemThiSinhLog.DiemTBLop12Cu = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBLop12"], null);
                        itemThiSinhLog.DiemTBLop12Moi = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBLop12Moi"], null);
                        itemThiSinhLog.DiemKKCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemKK"], null);
                        itemThiSinhLog.DiemKKMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemKKMoi"], null);
                        itemThiSinhLog.DienXTNCu = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        itemThiSinhLog.DienXTNMoi = Utils.ConvertToNullableInt32(dr["DienXTNMoi"], null);
                        itemThiSinhLog.HoiDongThiStrCu = Utils.ConvertToString(dr["HoiDongThiStrCu"], string.Empty);
                        itemThiSinhLog.HoiDongThiStrMoi = Utils.ConvertToString(dr["HoiDongThiStrMoi"], string.Empty);
                        itemThiSinhLog.DiemXetTotNghiepCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemXetTotNghiep"], null);
                        itemThiSinhLog.DiemXetTotNghiepMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemXetTotNghiepMoi"], null);
                        itemThiSinhLog.KetQuaTotNghiepCu = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        itemThiSinhLog.KetQuaTotNghiepMoi = Utils.ConvertToString(dr["KetQuaTotNghiepMoi"], string.Empty);
                        itemThiSinhLog.SoHieuBangCu = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        itemThiSinhLog.SoHieuBangMoi = Utils.ConvertToString(dr["SoHieuBangMoi"], string.Empty);
                        itemThiSinhLog.VaoSoCapBangSoCu = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        itemThiSinhLog.VaoSoCapBangSoMoi = Utils.ConvertToString(dr["VaoSoCapBangSoMoi"], string.Empty);
                        itemThiSinhLog.NgayCapBangCu = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        itemThiSinhLog.NgayCapBangMoi = Utils.ConvertToNullableDateTime(dr["NgayCapBangMoi"], null);
                        itemThiSinhLog.NamThiCu = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        itemThiSinhLog.NamThiMoi = Utils.ConvertToNullableInt32(dr["NamThiMoi"], null);
                        itemThiSinhLog.DoCu = Utils.ConvertToString(dr["Do"], string.Empty);
                        itemThiSinhLog.DoMoi = Utils.ConvertToString(dr["DoMoi"], string.Empty);
                        itemThiSinhLog.DoThemCu = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        itemThiSinhLog.DoThemMoi = Utils.ConvertToString(dr["DoThemMoi"], string.Empty);
                        itemThiSinhLog.HongCu = Utils.ConvertToString(dr["Hong"], string.Empty);
                        itemThiSinhLog.HongMoi = Utils.ConvertToString(dr["HongMoi"], string.Empty);
                        itemThiSinhLog.LaoDongCu = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        itemThiSinhLog.LaoDongMoi = Utils.ConvertToString(dr["LaoDongMoi"], string.Empty);
                        itemThiSinhLog.VanHoaCu = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        itemThiSinhLog.VanHoaMoi = Utils.ConvertToString(dr["VanHoaMoi"], string.Empty);
                        itemThiSinhLog.RLTTCu = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        itemThiSinhLog.RLTTMoi = Utils.ConvertToString(dr["RLTTMoi"], string.Empty);
                        itemThiSinhLog.TongSoDiemThiCu = Utils.ConvertToNullableDecimalFromInt(dr["TongSoDiemThi"], null);
                        itemThiSinhLog.TongSoDiemThiMoi = Utils.ConvertToNullableDecimalFromInt(dr["TongSoDiemThiMoi"], null);
                        itemThiSinhLog.DiemXLCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemXL"], null);
                        itemThiSinhLog.DiemXLMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemXLMoi"], null);
                        itemThiSinhLog.DiemUTCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemUT"], null);
                        itemThiSinhLog.DiemUTMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemUTMoi"], null);
                        itemThiSinhLog.GhiChuCu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        itemThiSinhLog.GhiChuMoi = Utils.ConvertToString(dr["GhiChuMoi"], string.Empty);
                        itemThiSinhLog.HangCu = Utils.ConvertToString(dr["Hang"], string.Empty);
                        itemThiSinhLog.HangMoi = Utils.ConvertToString(dr["HangMoi"], string.Empty);
                        itemThiSinhLog.DiemTBCacBaiThiCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBCacBaiThi"], null);
                        itemThiSinhLog.DiemTBCacBaiThiMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBCacBaiThiMoi"], null);
                        itemThiSinhLog.DienUuTienCu = Utils.ConvertToString(dr["DienUuTien"], null);
                        itemThiSinhLog.DienUuTienMoi = Utils.ConvertToString(dr["DienUuTienMoi"], null);
                        itemThiSinhLog.DiemTBCCu = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBC"], null);
                        itemThiSinhLog.DiemTBCMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemTBCMoi"], null);
                        itemThiSinhLog.ThaoTac = Utils.ConvertToNullableInt32(dr["ThaoTac"], null);
                        itemThiSinhLog.CanBoID = Utils.ConvertToNullableInt32(dr["CanBoSuaThongTinID"], null);
                        itemThiSinhLog.TenCanBo = Utils.ConvertToString(dr["TenCanBoSuaThongTin"], null);
                        itemThiSinhLog.NgayChinhSua = Utils.ConvertToNullableDateTime(dr["NgayThaoTac"], null);


                        if (itemThiSinhLog.ThaoTac == EnumLog.Insert.GetHashCode())
                        {
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoTenMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoTenCu,
                                    Moi = itemThiSinhLog.HoTenMoi,
                                    ThongTinChinhSua = "Họ tên"
                                });
                            }
                            if (itemThiSinhLog.NgaySinhMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgaySinhCu == null ? "" : itemThiSinhLog.NgaySinhCu.ToString(),
                                    Moi = itemThiSinhLog.NgaySinhMoi == null ? "" : itemThiSinhLog.NgaySinhMoi.ToString(),
                                    ThongTinChinhSua = "Ngày sinh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.NoiSinhMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NoiSinhCu,
                                    Moi = itemThiSinhLog.NoiSinhMoi,
                                    ThongTinChinhSua = "Nơi sinh"
                                });
                            }
                            if (itemThiSinhLog.GioiTinhMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    //Cu = itemThiSinhLog.GioiTinhCu == false ? "Nam" : "Nữ",
                                    Cu = "",
                                    Moi = itemThiSinhLog.GioiTinhMoi == false ? "Nam" : "Nữ",
                                    ThongTinChinhSua = "Giới tính"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DanTocStrMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DanTocStrCu,
                                    Moi = itemThiSinhLog.DanTocStrMoi,
                                    ThongTinChinhSua = "Dân tộc"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.CMNDMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.CMNDCu,
                                    Moi = itemThiSinhLog.CMNDMoi,
                                    ThongTinChinhSua = "CMND/CCCD"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoBaoDanhMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoBaoDanhCu,
                                    Moi = itemThiSinhLog.SoBaoDanhMoi,
                                    ThongTinChinhSua = "Số báo danh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoDienThoaiMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoDienThoaiCu,
                                    Moi = itemThiSinhLog.SoDienThoaiMoi,
                                    ThongTinChinhSua = "SĐT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DiaChiMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiaChiCu,
                                    Moi = itemThiSinhLog.DiaChiMoi,
                                    ThongTinChinhSua = "Địa chỉ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LopMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LopCu,
                                    Moi = itemThiSinhLog.LopMoi,
                                    ThongTinChinhSua = "Lớp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.TenTruongTHPTMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TenTruongTHPTCu,
                                    Moi = itemThiSinhLog.TenTruongTHPTMoi,
                                    ThongTinChinhSua = "Trường"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LoaiDuThiMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LoaiDuThiCu,
                                    Moi = itemThiSinhLog.LoaiDuThiMoi,
                                    ThongTinChinhSua = "Loại dự thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DonViDKDTMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DonViDKDTCu,
                                    Moi = itemThiSinhLog.DonViDKDTMoi,
                                    ThongTinChinhSua = "Đơn vị ĐKDT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHanhKiemStrMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHanhKiemStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHanhKiemStrMoi,
                                    ThongTinChinhSua = "Hạnh kiểm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHocLucStrMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHocLucStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHocLucStrMoi,
                                    ThongTinChinhSua = "Học lực"
                                });
                            }
                            if (itemThiSinhLog.DiemTBLop12Moi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBLop12Cu == null ? "" : itemThiSinhLog.DiemTBLop12Cu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBLop12Moi == null ? "" : itemThiSinhLog.DiemTBLop12Moi.ToString(),
                                    ThongTinChinhSua = "Điểm TB lớp 12"
                                });
                            }
                            if (itemThiSinhLog.DiemKKMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemKKCu == null ? "" : itemThiSinhLog.DiemKKCu.ToString(),
                                    Moi = itemThiSinhLog.DiemKKMoi == null ? "" : itemThiSinhLog.DiemKKMoi.ToString(),
                                    ThongTinChinhSua = "Điểm khuyến khích"
                                });
                            }
                            if (itemThiSinhLog.DienXTNMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienXTNCu == null ? "" : itemThiSinhLog.DienXTNCu.ToString(),
                                    Moi = itemThiSinhLog.DienXTNMoi == null ? "" : itemThiSinhLog.DienXTNMoi.ToString(),
                                    ThongTinChinhSua = "Diện xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoiDongThiStrMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoiDongThiStrCu,
                                    Moi = itemThiSinhLog.HoiDongThiStrMoi,
                                    ThongTinChinhSua = "Hội đồng thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXetTotNghiepMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXetTotNghiepCu == null ? "" : itemThiSinhLog.DiemXetTotNghiepCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXetTotNghiepMoi == null ? "" : itemThiSinhLog.DiemXetTotNghiepMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.KetQuaTotNghiepMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.KetQuaTotNghiepCu,
                                    Moi = itemThiSinhLog.KetQuaTotNghiepMoi,
                                    ThongTinChinhSua = "Kết quả tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoHieuBangMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoHieuBangCu,
                                    Moi = itemThiSinhLog.SoHieuBangMoi,
                                    ThongTinChinhSua = "Số hiệu bằng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VaoSoCapBangSoMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VaoSoCapBangSoCu,
                                    Moi = itemThiSinhLog.VaoSoCapBangSoMoi,
                                    ThongTinChinhSua = "Vào sổ cấp bằng số"
                                });
                            }
                            if (itemThiSinhLog.NgayCapBangMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgayCapBangCu == null ? "" : itemThiSinhLog.NgayCapBangCu.ToString(),
                                    Moi = itemThiSinhLog.NgayCapBangMoi == null ? "" : itemThiSinhLog.NgayCapBangMoi.ToString(),
                                    ThongTinChinhSua = "Ngày cấp bằng"
                                });
                            }
                            if (itemThiSinhLog.NamThiMoi != null && itemThiSinhLog.NamThiMoi != 0)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NamThiCu == null || itemThiSinhLog.NamThiCu == 0 ? "" : itemThiSinhLog.NamThiCu.ToString(),
                                    Moi = itemThiSinhLog.NamThiMoi == null || itemThiSinhLog.NamThiMoi == 0 ? "" : itemThiSinhLog.NamThiMoi.ToString(),
                                    ThongTinChinhSua = "Năm thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoCu,
                                    Moi = itemThiSinhLog.DoMoi,
                                    ThongTinChinhSua = "Đỗ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoThemMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoThemCu,
                                    Moi = itemThiSinhLog.DoThemMoi,
                                    ThongTinChinhSua = "Đỗ thêm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HongMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HongCu,
                                    Moi = itemThiSinhLog.HongMoi,
                                    ThongTinChinhSua = "Hỏng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LaoDongMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LaoDongCu,
                                    Moi = itemThiSinhLog.LaoDongMoi,
                                    ThongTinChinhSua = "Lao động"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VanHoaMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VanHoaCu,
                                    Moi = itemThiSinhLog.VanHoaMoi,
                                    ThongTinChinhSua = "Văn hoá"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.RLTTMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.RLTTCu,
                                    Moi = itemThiSinhLog.RLTTMoi,
                                    ThongTinChinhSua = "RLTT"
                                });
                            }
                            if (itemThiSinhLog.TongSoDiemThiMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TongSoDiemThiCu == null ? "" : itemThiSinhLog.TongSoDiemThiCu.ToString(),
                                    Moi = itemThiSinhLog.TongSoDiemThiMoi == null ? "" : itemThiSinhLog.TongSoDiemThiMoi.ToString(),
                                    ThongTinChinhSua = "Tổng số điểm thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXLMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXLCu == null ? "" : itemThiSinhLog.DiemXLCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXLMoi == null ? "" : itemThiSinhLog.DiemXLMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xếp loại"
                                });
                            }
                            if (itemThiSinhLog.DiemUTMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemUTCu == null ? "" : itemThiSinhLog.DiemUTCu.ToString(),
                                    Moi = itemThiSinhLog.DiemUTMoi == null ? "" : itemThiSinhLog.DiemUTMoi.ToString(),
                                    ThongTinChinhSua = "Điểm ưu tiên"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.GhiChuMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GhiChuCu,
                                    Moi = itemThiSinhLog.GhiChuMoi,
                                    ThongTinChinhSua = "Ghi chú"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HangMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HangCu,
                                    Moi = itemThiSinhLog.HangMoi,
                                    ThongTinChinhSua = "Hạng"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCacBaiThiMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCacBaiThiCu == null ? "" : itemThiSinhLog.DiemTBCacBaiThiCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCacBaiThiMoi == null ? "" : itemThiSinhLog.DiemTBCacBaiThiMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TB các bài thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DienUuTienMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienUuTienCu,
                                    Moi = itemThiSinhLog.DienUuTienMoi,
                                    ThongTinChinhSua = "Diện ưu tiên"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCCu == null ? "" : itemThiSinhLog.DiemTBCCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCMoi == null ? "" : itemThiSinhLog.DiemTBCMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TBC"
                                });
                            }

                        }
                        else if (itemThiSinhLog.ThaoTac == EnumLog.Update.GetHashCode())
                        {
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoTenCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoTenCu,
                                    Moi = itemThiSinhLog.HoTenMoi,
                                    ThongTinChinhSua = "Họ tên"
                                });
                            }
                            if (itemThiSinhLog.NgaySinhCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgaySinhCu == null ? "" : itemThiSinhLog.NgaySinhCu.ToString(),
                                    Moi = itemThiSinhLog.NgaySinhMoi == null ? "" : itemThiSinhLog.NgaySinhMoi.ToString(),
                                    ThongTinChinhSua = "Ngày sinh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.NoiSinhCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NoiSinhCu,
                                    Moi = itemThiSinhLog.NoiSinhMoi,
                                    ThongTinChinhSua = "Nơi sinh"
                                });
                            }
                            if (itemThiSinhLog.NoiSinhCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GioiTinhCu == false ? "Nam" : "Nữ",
                                    Moi = itemThiSinhLog.GioiTinhMoi == false ? "Nam" : "Nữ",
                                    ThongTinChinhSua = "Giới tính"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DanTocStrCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DanTocStrCu,
                                    Moi = itemThiSinhLog.DanTocStrMoi,
                                    ThongTinChinhSua = "Dân tộc"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.CMNDCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.CMNDCu,
                                    Moi = itemThiSinhLog.CMNDMoi,
                                    ThongTinChinhSua = "CMND/CCCD"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoBaoDanhCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoBaoDanhCu,
                                    Moi = itemThiSinhLog.SoBaoDanhMoi,
                                    ThongTinChinhSua = "Số báo danh"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoDienThoaiCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoDienThoaiCu,
                                    Moi = itemThiSinhLog.SoDienThoaiMoi,
                                    ThongTinChinhSua = "SĐT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DiaChiCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiaChiCu,
                                    Moi = itemThiSinhLog.DiaChiMoi,
                                    ThongTinChinhSua = "Địa chỉ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LopCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LopCu,
                                    Moi = itemThiSinhLog.LopMoi,
                                    ThongTinChinhSua = "Lớp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.TenTruongTHPTCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TenTruongTHPTCu,
                                    Moi = itemThiSinhLog.TenTruongTHPTMoi,
                                    ThongTinChinhSua = "Trường"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LoaiDuThiCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LoaiDuThiCu,
                                    Moi = itemThiSinhLog.LoaiDuThiMoi,
                                    ThongTinChinhSua = "Loại dự thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DonViDKDTCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DonViDKDTCu,
                                    Moi = itemThiSinhLog.DonViDKDTMoi,
                                    ThongTinChinhSua = "Đơn vị ĐKDT"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHanhKiemStrCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHanhKiemStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHanhKiemStrMoi,
                                    ThongTinChinhSua = "Hạnh kiểm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.XepLoaiHocLucStrCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.XepLoaiHocLucStrCu,
                                    Moi = itemThiSinhLog.XepLoaiHocLucStrMoi,
                                    ThongTinChinhSua = "Học lực"
                                });
                            }
                            if (itemThiSinhLog.DiemTBLop12Cu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBLop12Cu == null ? "" : itemThiSinhLog.DiemTBLop12Cu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBLop12Moi == null ? "" : itemThiSinhLog.DiemTBLop12Moi.ToString(),
                                    ThongTinChinhSua = "Điểm TB lớp 12"
                                });
                            }
                            if (itemThiSinhLog.DiemKKCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemKKCu == null ? "" : itemThiSinhLog.DiemKKCu.ToString(),
                                    Moi = itemThiSinhLog.DiemKKMoi == null ? "" : itemThiSinhLog.DiemKKMoi.ToString(),
                                    ThongTinChinhSua = "Điểm khuyến khích"
                                });
                            }
                            if (itemThiSinhLog.DienXTNCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienXTNCu == null ? "" : itemThiSinhLog.DienXTNCu.ToString(),
                                    Moi = itemThiSinhLog.DienXTNMoi == null ? "" : itemThiSinhLog.DienXTNMoi.ToString(),
                                    ThongTinChinhSua = "Diện xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HoiDongThiStrCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HoiDongThiStrCu,
                                    Moi = itemThiSinhLog.HoiDongThiStrMoi,
                                    ThongTinChinhSua = "Hội đồng thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXetTotNghiepCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXetTotNghiepCu == null ? "" : itemThiSinhLog.DiemXetTotNghiepCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXetTotNghiepMoi == null ? "" : itemThiSinhLog.DiemXetTotNghiepMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xét tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.KetQuaTotNghiepCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.KetQuaTotNghiepCu,
                                    Moi = itemThiSinhLog.KetQuaTotNghiepMoi,
                                    ThongTinChinhSua = "Kết quả tốt nghiệp"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.SoHieuBangCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.SoHieuBangCu,
                                    Moi = itemThiSinhLog.SoHieuBangMoi,
                                    ThongTinChinhSua = "Số hiệu bằng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VaoSoCapBangSoCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VaoSoCapBangSoCu,
                                    Moi = itemThiSinhLog.VaoSoCapBangSoMoi,
                                    ThongTinChinhSua = "Vào sổ cấp bằng số"
                                });
                            }
                            if (itemThiSinhLog.NgayCapBangCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NgayCapBangCu == null ? "" : itemThiSinhLog.NgayCapBangCu.ToString(),
                                    Moi = itemThiSinhLog.NgayCapBangMoi == null ? "" : itemThiSinhLog.NgayCapBangMoi.ToString(),
                                    ThongTinChinhSua = "Ngày cấp bằng"
                                });
                            }
                            if (itemThiSinhLog.NamThiCu != null && itemThiSinhLog.NamThiCu != 0)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.NamThiCu == null || itemThiSinhLog.NamThiCu == 0 ? "" : itemThiSinhLog.NamThiCu.ToString(),
                                    Moi = itemThiSinhLog.NamThiMoi == null || itemThiSinhLog.NamThiMoi == 0 ? "" : itemThiSinhLog.NamThiMoi.ToString(),
                                    ThongTinChinhSua = "Năm thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoCu,
                                    Moi = itemThiSinhLog.DoMoi,
                                    ThongTinChinhSua = "Đỗ"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DoThemCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DoThemCu,
                                    Moi = itemThiSinhLog.DoThemMoi,
                                    ThongTinChinhSua = "Đỗ thêm"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HongCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HongCu,
                                    Moi = itemThiSinhLog.HongMoi,
                                    ThongTinChinhSua = "Hỏng"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.LaoDongCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.LaoDongCu,
                                    Moi = itemThiSinhLog.LaoDongMoi,
                                    ThongTinChinhSua = "Lao động"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.VanHoaCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.VanHoaCu,
                                    Moi = itemThiSinhLog.VanHoaMoi,
                                    ThongTinChinhSua = "Văn hoá"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.RLTTCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.RLTTCu,
                                    Moi = itemThiSinhLog.RLTTMoi,
                                    ThongTinChinhSua = "RLTT"
                                });
                            }
                            if (itemThiSinhLog.TongSoDiemThiCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.TongSoDiemThiCu == null ? "" : itemThiSinhLog.TongSoDiemThiCu.ToString(),
                                    Moi = itemThiSinhLog.TongSoDiemThiMoi == null ? "" : itemThiSinhLog.TongSoDiemThiMoi.ToString(),
                                    ThongTinChinhSua = "Tổng số điểm thi"
                                });
                            }
                            if (itemThiSinhLog.DiemXLCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemXLCu == null ? "" : itemThiSinhLog.DiemXLCu.ToString(),
                                    Moi = itemThiSinhLog.DiemXLMoi == null ? "" : itemThiSinhLog.DiemXLMoi.ToString(),
                                    ThongTinChinhSua = "Điểm xếp loại"
                                });
                            }
                            if (itemThiSinhLog.DiemUTCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemUTCu == null ? "" : itemThiSinhLog.DiemUTCu.ToString(),
                                    Moi = itemThiSinhLog.DiemUTMoi == null ? "" : itemThiSinhLog.DiemUTMoi.ToString(),
                                    ThongTinChinhSua = "Điểm ưu tiên"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.GhiChuCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.GhiChuCu,
                                    Moi = itemThiSinhLog.GhiChuMoi,
                                    ThongTinChinhSua = "Ghi chú"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.HangCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.HangCu,
                                    Moi = itemThiSinhLog.HangMoi,
                                    ThongTinChinhSua = "Hạng"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCacBaiThiCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCacBaiThiCu == null ? "" : itemThiSinhLog.DiemTBCacBaiThiCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCacBaiThiMoi == null ? "" : itemThiSinhLog.DiemTBCacBaiThiMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TB các bài thi"
                                });
                            }
                            if (!string.IsNullOrEmpty(itemThiSinhLog.DienUuTienCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DienUuTienCu,
                                    Moi = itemThiSinhLog.DienUuTienMoi,
                                    ThongTinChinhSua = "Diện ưu tiên"
                                });
                            }
                            if (itemThiSinhLog.DiemTBCCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemThiSinhLog.DiemTBCCu == null ? "" : itemThiSinhLog.DiemTBCCu.ToString(),
                                    Moi = itemThiSinhLog.DiemTBCMoi == null ? "" : itemThiSinhLog.DiemTBCMoi.ToString(),
                                    ThongTinChinhSua = "Điểm TBC"
                                });
                            }
                        }
                    }
                    dr.Close();
                }

                SqlParameter[] parameters1 = new SqlParameter[]
          {
               new SqlParameter("ThiSinhID", SqlDbType.Int),
               new SqlParameter("NgayChinhSua", SqlDbType.DateTime),
          };
                parameters1[0].Value = ThiSinhID;
                parameters1[1].Value = NgayChinhSua;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_NV_BangDiemGhiDiemLog_GetByThiSinhID", parameters1))
                {
                    while (dr.Read())
                    {
                        var itemDiemThiLog = new ThongTinDiemThiLog();
                        itemDiemThiLog.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        itemDiemThiLog.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                        itemDiemThiLog.TenMonThi = Utils.ConvertToString(dr["TenMonThi"], string.Empty);
                        itemDiemThiLog.DiemCu = Utils.ConvertToNullableDecimalFromInt(dr["Diem"], null);
                        itemDiemThiLog.DiemMoi = Utils.ConvertToNullableDecimalFromInt(dr["DiemMoi"], null);
                        itemDiemThiLog.DiemBaiToHopCu = Utils.ConvertToString(dr["DiemBaiToHop"], string.Empty);
                        itemDiemThiLog.DiemBaiToHopMoi = Utils.ConvertToString(dr["DiemBaiToHopMoi"], string.Empty);
                        itemDiemThiLog.ThaoTac = Utils.ConvertToNullableInt32(dr["ThaoTac"], null);
                        itemDiemThiLog.NgayChinhSua = Utils.ConvertToNullableDateTime(dr["NgayThaoTac"], null);

                        if (itemDiemThiLog.ThaoTac == EnumLog.Insert.GetHashCode())
                        {
                            if (!string.IsNullOrEmpty(itemDiemThiLog.DiemBaiToHopMoi))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemDiemThiLog.DiemBaiToHopCu,
                                    Moi = itemDiemThiLog.DiemBaiToHopMoi,
                                    ThongTinChinhSua = itemDiemThiLog.TenMonThi
                                });
                            }
                            if (itemDiemThiLog.DiemMoi != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemDiemThiLog.DiemCu == null ? "" : itemDiemThiLog.DiemCu.ToString(),
                                    Moi = itemDiemThiLog.DiemMoi == null ? "" : itemDiemThiLog.DiemMoi.ToString(),
                                    ThongTinChinhSua = itemDiemThiLog.TenMonThi
                                });
                            }
                        }
                        else if (itemDiemThiLog.ThaoTac == EnumLog.Update.GetHashCode())
                        {
                            if (!string.IsNullOrEmpty(itemDiemThiLog.DiemBaiToHopCu))
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemDiemThiLog.DiemBaiToHopCu,
                                    Moi = itemDiemThiLog.DiemBaiToHopMoi,
                                    ThongTinChinhSua = itemDiemThiLog.TenMonThi
                                });
                            }
                            if (itemDiemThiLog.DiemCu != null)
                            {
                                ThongTinThiSinhLog.Add(new CacTruongSua()
                                {
                                    Cu = itemDiemThiLog.DiemCu == null ? "" : itemDiemThiLog.DiemCu.ToString(),
                                    Moi = itemDiemThiLog.DiemMoi == null ? "" : itemDiemThiLog.DiemMoi.ToString(),
                                    ThongTinChinhSua = itemDiemThiLog.TenMonThi
                                });
                            }
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ThongTinThiSinhLog;
        }
        public ThongTinThiSinh CheckTrungCCCD(int? ThiSinhID, string CMND)
        {
            ThongTinThiSinh Result = new ThongTinThiSinh();
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@ThiSinhID",SqlDbType.Int),
                  new SqlParameter("@CMND",SqlDbType.NVarChar),
            };
            parameters[0].Value = ThiSinhID ?? Convert.DBNull;
            parameters[1].Value = CMND ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_CheckTrungCCCD", parameters))
                {
                    while (dr.Read())
                    {
                        Result.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        break;
                    }
                    dr.Close();
                }


                return Result;
            }
            catch
            {
                throw;
            }
        }
    }
}
