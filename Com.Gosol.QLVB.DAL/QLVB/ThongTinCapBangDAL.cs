using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.QLVB.DAL.QLVB
{
    public interface IThongTinCapBangDAL
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, DateTime? NgayCapBang, int DonViDaoTao, ref int TotalRow);
        public List<ThongTinThiSinh> DanhSachThiSinhCapBang(BasePagingParams p, ref int TotalRow);
        public List<ThongTinThiSinh> DanhSachThiSinhTrung(TraCuuParams p, ref int TotalRow);
        public BaseResultModel Insert(ThongTinThiSinh ThongTinThiSinh);
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh);
        public ThongTinThiSinh GetByID(int ThiSinhID);
        public BaseResultModel Update_TrangThaiCapBang(List<ThongTinThiSinh> ListThongTinThiSinh);
        public List<ThongTinThiSinh> TraCuuVanBangChungChi(TraCuuParams p, ref int TotalRow);
        public List<ThongTinThiSinh> TraCuuVanBangChungChiNuocNgoaiCap(TraCuuParams p, ref int TotalRow);
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh item, int? CanBoID);
        public ThongTinThiSinh GetThongTinCapBang(int ThiSinhID);
        public Boolean CheckTicket(string Ticket);
    }
    public class ThongTinCapBangDAL : IThongTinCapBangDAL
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, DateTime? NgayCapBang, int DonViDaoTao, ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@NgayCapBang",SqlDbType.DateTime),
                new SqlParameter("@DonViDaoTao",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = NgayCapBang ?? Convert.DBNull;
            parameters[7].Value = DonViDaoTao;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
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
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        list.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public List<ThongTinThiSinh> DanhSachThiSinhCapBang(BasePagingParams p, ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
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
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.ThongTinThiSinhID = Utils.ConvertToInt32(dr["ThongTinThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.QueQuan = Utils.ConvertToString(dr["QueQuan"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToInt32(dr["DanToc"], 0);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToInt32(dr["TruongTHPT"], 0);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.NamTotNghiep = info.NamThi;
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        //info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        if ((info.SoHieuBang != "" && info.SoHieuBang.Length > 0) || (info.VaoSoCapBangSo != "" && info.VaoSoCapBangSo.Length > 0)
                       || (info.Hang != "" && info.Hang.Length > 0) || (info.NgayCapBang != null))
                        {
                            info.TrangThaiCapBang = 1;
                        }
                        else info.TrangThaiCapBang = 0;
                        info.TrangThaiMap = Utils.ConvertToInt32(dr["TrangThaiMap"], 0);
                        info.ThiSinhTrung1 = Utils.ConvertToInt32(dr["ThiSinhTrung1"], 0);

                        list.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
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
                         new SqlParameter("ThiSinhMaped", SqlDbType.Int),
                    };

                    parms_ts[0].Value = item.ThiSinhID;
                    parms_ts[1].Value = item.KyThiID ?? Convert.DBNull;
                    parms_ts[2].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                    parms_ts[3].Value = item.SoHieuBang ?? Convert.DBNull;
                    parms_ts[4].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                    parms_ts[5].Value = item.NamTotNghiep ?? Convert.DBNull;
                    parms_ts[6].Value = item.NgayCapBang ?? Convert.DBNull;
                    parms_ts[7].Value = item.Hang ?? Convert.DBNull;
                    parms_ts[8].Value = item.ThiSinhMaped ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_UpdateThongTinCapBang", parms_ts);

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
        public List<ThongTinThiSinh> DanhSachThiSinhTrung(TraCuuParams p, ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@HoTen",SqlDbType.NVarChar),
                new SqlParameter("@DonViDaoTao",SqlDbType.Int),
                new SqlParameter("@Lop",SqlDbType.NVarChar),
                new SqlParameter("@CMND",SqlDbType.NVarChar),
                new SqlParameter("@NamCapBang",SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
                new SqlParameter("@NgaySinh",SqlDbType.DateTime),
                new SqlParameter("@HoiDongThiID",SqlDbType.Int),
                new SqlParameter("@GioiTinh",SqlDbType.Bit),
                new SqlParameter("@SoBaoDanh",SqlDbType.NVarChar),
                new SqlParameter("@NoiSinh",SqlDbType.NVarChar),
                new SqlParameter("@ThiSinhID",SqlDbType.Int),
                new SqlParameter("@SoQuyen",SqlDbType.NVarChar),
                new SqlParameter("@SoTrang",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.HoTen ?? "";
            parameters[7].Value = p.TruongTHPT ?? 0;
            parameters[8].Value = p.Lop ?? "";
            parameters[9].Value = p.CMND ?? "";
            parameters[10].Value = p.NamTotNghiep ?? 0;
            parameters[11].Value = p.TrangThai ?? 0;
            parameters[12].Value = p.NgaySinh ?? Convert.DBNull;
            parameters[13].Value = p.HoiDongThiID ?? Convert.DBNull;
            parameters[14].Value = p.GioiTinh ?? Convert.DBNull;
            parameters[15].Value = p.SoBaoDanh ?? Convert.DBNull;
            parameters[16].Value = p.NoiSinh ?? "";
            parameters[17].Value = p.ThiSinhID ?? Convert.DBNull;
            parameters[18].Value = p.SoQuyen ?? Convert.DBNull;
            parameters[19].Value = p.SoTrang ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhSachThiSinhTrung", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
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
                        info.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.NamTotNghiep = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        list.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public BaseResultModel Insert(ThongTinThiSinh ThongTinThiSinh)
        {
            var Result = new BaseResultModel();
            try
            {
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SqlParameter[] parms_ts = new SqlParameter[]{
                                new SqlParameter("ThiSinhID", SqlDbType.Int),           
                                new SqlParameter("HoTen", SqlDbType.NVarChar),
                                new SqlParameter("NgaySinh", SqlDbType.DateTime),
                                new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                                new SqlParameter("GioiTinh", SqlDbType.Bit),
                                new SqlParameter("DanToc", SqlDbType.Int),
                                new SqlParameter("CMND", SqlDbType.NVarChar),    
                                new SqlParameter("Lop", SqlDbType.NVarChar),
                                new SqlParameter("TruongTHPT", SqlDbType.Int),            
                                new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                                new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                                new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                                new SqlParameter("TrangThaiCapBang", SqlDbType.Int),
                            };
                            parms_ts[0].Direction = ParameterDirection.Output;
                            parms_ts[0].Size = 8;
                            parms_ts[1].Value = ThongTinThiSinh.HoTen ?? Convert.DBNull;
                            parms_ts[2].Value = ThongTinThiSinh.NgaySinh ?? Convert.DBNull;
                            parms_ts[3].Value = ThongTinThiSinh.NoiSinh ?? Convert.DBNull;
                            parms_ts[4].Value = ThongTinThiSinh.GioiTinh ?? Convert.DBNull;
                            parms_ts[5].Value = ThongTinThiSinh.DanToc ?? Convert.DBNull;
                            parms_ts[6].Value = ThongTinThiSinh.CMND ?? Convert.DBNull;
                            parms_ts[7].Value = ThongTinThiSinh.Lop ?? Convert.DBNull;
                            parms_ts[8].Value = ThongTinThiSinh.TruongTHPT ?? Convert.DBNull;
                            parms_ts[9].Value = ThongTinThiSinh.SoHieuBang ?? Convert.DBNull;
                            parms_ts[10].Value = ThongTinThiSinh.VaoSoCapBangSo ?? Convert.DBNull;
                            parms_ts[11].Value = ThongTinThiSinh.NgayCapBang ?? Convert.DBNull;
                            parms_ts[12].Value = ThongTinThiSinh.TrangThaiCapBang ?? Convert.DBNull;

                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_Insert", parms_ts);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Data = Utils.ConvertToInt32(parms_ts[0].Value, 0);
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("bằng cấp");
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
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh)
        {
            var Result = new BaseResultModel();
            try
            {
                if (ThongTinThiSinh == null || ThongTinThiSinh.HoTen == null || ThongTinThiSinh.HoTen.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Họ tên thí sinh không được để trống";
                    return Result;
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                     {
                          new SqlParameter("ThiSinhID", SqlDbType.Int),
                          new SqlParameter("HoTen", SqlDbType.NVarChar),
                          new SqlParameter("NgaySinh", SqlDbType.DateTime),
                          new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                          new SqlParameter("GioiTinh", SqlDbType.Bit),
                          new SqlParameter("DanToc", SqlDbType.Int),
                          new SqlParameter("CMND", SqlDbType.NVarChar),
                          new SqlParameter("Lop", SqlDbType.NVarChar),
                          new SqlParameter("TruongTHPT", SqlDbType.Int),
                          new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                          new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                          new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                          new SqlParameter("TrangThaiCapBang", SqlDbType.Int),
                     };

                    parameters[0].Value = ThongTinThiSinh.ThiSinhID;
                    parameters[1].Value = ThongTinThiSinh.HoTen ?? Convert.DBNull;
                    parameters[2].Value = ThongTinThiSinh.NgaySinh ?? Convert.DBNull;
                    parameters[3].Value = ThongTinThiSinh.NoiSinh ?? Convert.DBNull;
                    parameters[4].Value = ThongTinThiSinh.GioiTinh ?? Convert.DBNull;
                    parameters[5].Value = ThongTinThiSinh.DanToc ?? Convert.DBNull;
                    parameters[6].Value = ThongTinThiSinh.CMND ?? Convert.DBNull;
                    parameters[7].Value = ThongTinThiSinh.Lop ?? Convert.DBNull;
                    parameters[8].Value = ThongTinThiSinh.TruongTHPT ?? Convert.DBNull;
                    parameters[9].Value = ThongTinThiSinh.SoHieuBang ?? Convert.DBNull;
                    parameters[10].Value = ThongTinThiSinh.VaoSoCapBangSo ?? Convert.DBNull;
                    parameters[11].Value = ThongTinThiSinh.NgayCapBang ?? Convert.DBNull;
                    parameters[12].Value = ThongTinThiSinh.TrangThaiCapBang ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_Update", parameters);
                                trans.Commit();
                                Result.Status = 1;
                                Result.Message = ConstantLogMessage.Alert_Update_Success("bằng cấp");
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
        public ThongTinThiSinh GetByID(int ThiSinhID)
        {
            ThongTinThiSinh info = new ThongTinThiSinh();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("ThiSinhID",SqlDbType.Int)
            };
            parameters[0].Value = ThiSinhID;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetByID", parameters))
                {
                    while (dr.Read())
                    {
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
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        info.TenHoiDongThi = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return info;
        }
        public ThongTinThiSinh GetThongTinCapBang(int ThiSinhID)
        {
            ThongTinThiSinh info = new ThongTinThiSinh();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("ThiSinhID",SqlDbType.Int)
            };
            parameters[0].Value = ThiSinhID;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetThongTinCapBang", parameters))
                {
                    while (dr.Read())
                    {
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.ThongTinThiSinhID = Utils.ConvertToInt32(dr["ThongTinThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.QueQuan = Utils.ConvertToString(dr["QueQuan"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToInt32(dr["DanToc"], 0);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToInt32(dr["TruongTHPT"], 0);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.NamTotNghiep = info.NamThi;
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        //info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        if ((info.SoHieuBang != "" && info.SoHieuBang.Length > 0) || (info.VaoSoCapBangSo != "" && info.VaoSoCapBangSo.Length > 0)
                            || (info.Hang != "" && info.Hang.Length > 0) || (info.NgayCapBang != null))
                        {
                            info.TrangThaiCapBang = 1;
                        }
                        else info.TrangThaiCapBang = 0;
                        info.ThiSinhTrung1 = Utils.ConvertToInt32(dr["ThiSinhTrung1"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return info;
        }
        public BaseResultModel Update_TrangThaiCapBang(List<ThongTinThiSinh> ListThongTinThiSinh)
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
                                    new SqlParameter("TrangThaiCapBang", SqlDbType.Int),
                                };
                                parameters[0].Value = item.ThiSinhID;
                                parameters[1].Value = item.TrangThaiCapBang ?? Convert.DBNull;

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_UpdateTrangThaiCapBang", parameters);
                            }

                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("duyệt thông tin cấp bằng");
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
        public List<ThongTinThiSinh> TraCuuVanBangChungChi(TraCuuParams p, ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@HoTen",SqlDbType.NVarChar),
                new SqlParameter("@DonViDaoTao",SqlDbType.Int),
                new SqlParameter("@Lop",SqlDbType.NVarChar),
                new SqlParameter("@CMND",SqlDbType.NVarChar),
                new SqlParameter("@NamCapBang",SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
                new SqlParameter("@NgaySinh",SqlDbType.DateTime),
                new SqlParameter("@HoiDongThiID",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.HoTen == null ? "" : p.HoTen.Trim();
            parameters[7].Value = p.DonViDaoTao ?? 0;
            parameters[8].Value = p.Lop == null ? "" : p.Lop.Trim();
            parameters[9].Value = p.CMND == null ? "" : p.CMND.Trim();
            parameters[10].Value = p.NamCapBang ?? 0;
            parameters[11].Value = p.TrangThai ?? 0;
            parameters[12].Value = p.NgaySinh ?? Convert.DBNull;
            parameters[13].Value = p.HoiDongThiID ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_TraCuuVanBangChungChi_New", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NgaySinhStr = Utils.ConvertToString(dr["NgaySinhStr"], string.Empty);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        info.DanToc = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        //info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToInt32(dr["TruongTHPT"], 0);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        //info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        //info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        //info.XepLoaiHanhKiem = Utils.ConvertToInt32(dr["XepLoaiHanhKiem"], 0);
                        //info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        //info.XepLoaiHocLuc = Utils.ConvertToInt32(dr["XepLoaiHocLuc"], 0);
                        //info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        //info.DiemTBLop12 = Utils.ConvertToDecimal(dr["DiemTBLop12"], 0);
                        //info.DiemKK = Utils.ConvertToDecimal(dr["DiemKK"], 0);
                        //info.DienXTN = Utils.ConvertToInt32(dr["DienXTN"], 0);
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        //info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.NamTotNghiep = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        list.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public List<ThongTinThiSinh> TraCuuVanBangChungChiNuocNgoaiCap(TraCuuParams p, ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@HoTen",SqlDbType.NVarChar),
                new SqlParameter("@DonViDaoTao",SqlDbType.Int),
                new SqlParameter("@Lop",SqlDbType.NVarChar),
                new SqlParameter("@CMND",SqlDbType.NVarChar),
                new SqlParameter("@NamCapBang",SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
                new SqlParameter("@NgaySinh",SqlDbType.DateTime),
                new SqlParameter("@HoiDongThiID",SqlDbType.Int),
                new SqlParameter("@QuocGia",SqlDbType.NVarChar),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.HoTen == null ? "" : p.HoTen.Trim();
            parameters[7].Value = p.DonViDaoTao ?? 0;
            parameters[8].Value = p.Lop == null ? "" : p.Lop.Trim();
            parameters[9].Value = p.CMND == null ? "" : p.CMND.Trim();
            parameters[10].Value = p.NamCapBang ?? 0;
            parameters[11].Value = p.TrangThai ?? 0;
            parameters[12].Value = p.NgaySinh ?? Convert.DBNull;
            parameters[13].Value = p.HoiDongThiID ?? Convert.DBNull;
            parameters[14].Value = p.QuocGia == null ? "" : p.QuocGia.Trim();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinCapBang_TraCuuVanBangChungChiNNC", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
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
                        info.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        info.NamTotNghiep = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.BODY_QUOCGIA = Utils.ConvertToString(dr["BODY_QUOCGIA"], string.Empty);
                        list.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Boolean CheckTicket(string Ticket)
        {
            var check = false;
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Ticket",SqlDbType.NVarChar)
            };
            parameters[0].Value = Ticket;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetTicket", parameters))
                {
                    while (dr.Read())
                    {
                        int ID = Utils.ConvertToInt32(dr["ID"], 0);
                        if(ID > 0)
                        {
                            check = true;
                            break;
                        }
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return check;
        }

    }
}
