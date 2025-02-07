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
    public interface IQuanLyThiSinhDAL
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow);
        public BaseResultModel Insert(List<ThongTinThiSinh> ListThongTinThiSinh);
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh);
        public BaseResultModel Delete(int ThiSinhID);
        public ThongTinThiSinh GetByID(int ThiSinhID);
        public ThongTinThiSinh GetThongTinThiSinh(int ThiSinhID);
        public ThongTinThiSinh GetThongTinThiSinh_New(int ThiSinhID);
        public BaseResultModel Update_TrangThai(List<ThongTinThiSinh> ListThongTinThiSinh);

    }
    public class QuanLyThiSinhDAL : IQuanLyThiSinhDAL
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow)
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
                new SqlParameter("@Truong",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = NamThi;
            parameters[7].Value = Truong;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetPagingBySearch", parameters))
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
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
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
        public List<ThongTinThiSinh> GetAll(ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter("@TotalRow",SqlDbType.Int),
              };
            parameters[0].Direction = ParameterDirection.Output;
            parameters[0].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetAll", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        list.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[0].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        public BaseResultModel Insert(List<ThongTinThiSinh> ListThongTinThiSinh)
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
                            if (ListThongTinThiSinh != null && ListThongTinThiSinh.Count > 0)
                            {
                                //insert thí sinh thi
                                foreach (var item in ListThongTinThiSinh)
                                {
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
                                    };
                                    parms_ts[0].Direction = ParameterDirection.Output;
                                    parms_ts[0].Size = 8;
                                    parms_ts[1].Value = item.KyThiID ?? Convert.DBNull;
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

                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_Insert", parms_ts);
                                }
                            }
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("thi sinh");
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
                    };

                    parameters[0].Value = ThongTinThiSinh.ThiSinhID;
                    parameters[1].Value = ThongTinThiSinh.KyThiID ?? Convert.DBNull;
                    parameters[2].Value = ThongTinThiSinh.HoTen ?? Convert.DBNull;
                    parameters[3].Value = ThongTinThiSinh.NgaySinh ?? Convert.DBNull;
                    parameters[4].Value = ThongTinThiSinh.NoiSinh ?? Convert.DBNull;
                    parameters[5].Value = ThongTinThiSinh.GioiTinh ?? Convert.DBNull;
                    parameters[6].Value = ThongTinThiSinh.DanToc ?? Convert.DBNull;
                    parameters[7].Value = ThongTinThiSinh.CMND ?? Convert.DBNull;
                    parameters[8].Value = ThongTinThiSinh.SoBaoDanh ?? Convert.DBNull;
                    parameters[9].Value = ThongTinThiSinh.SoDienThoai ?? Convert.DBNull;
                    parameters[10].Value = ThongTinThiSinh.DiaChi ?? Convert.DBNull;
                    parameters[11].Value = ThongTinThiSinh.Lop ?? Convert.DBNull;
                    parameters[12].Value = ThongTinThiSinh.TruongTHPT ?? Convert.DBNull;
                    parameters[13].Value = ThongTinThiSinh.LoaiDuThi ?? Convert.DBNull;
                    parameters[14].Value = ThongTinThiSinh.DonViDKDT ?? Convert.DBNull;
                    parameters[15].Value = ThongTinThiSinh.XepLoaiHanhKiem ?? Convert.DBNull;
                    parameters[16].Value = ThongTinThiSinh.XepLoaiHocLuc ?? Convert.DBNull;
                    parameters[17].Value = ThongTinThiSinh.DiemTBLop12 ?? Convert.DBNull;
                    parameters[18].Value = ThongTinThiSinh.DiemKK ?? Convert.DBNull;
                    parameters[19].Value = ThongTinThiSinh.DienXTN ?? Convert.DBNull;
                    parameters[20].Value = ThongTinThiSinh.HoiDongThi ?? Convert.DBNull;
                    parameters[21].Value = ThongTinThiSinh.DiemXetTotNghiep ?? Convert.DBNull;
                    parameters[22].Value = ThongTinThiSinh.KetQuaTotNghiep ?? Convert.DBNull;
                    parameters[23].Value = ThongTinThiSinh.SoHieuBang ?? Convert.DBNull;
                    parameters[24].Value = ThongTinThiSinh.VaoSoCapBangSo ?? Convert.DBNull;
                    parameters[25].Value = ThongTinThiSinh.NamThi ?? Convert.DBNull;
                    parameters[26].Value = ThongTinThiSinh.Do ?? Convert.DBNull;
                    parameters[27].Value = ThongTinThiSinh.DoThem ?? Convert.DBNull;
                    parameters[28].Value = ThongTinThiSinh.Hong ?? Convert.DBNull;
                    parameters[29].Value = ThongTinThiSinh.LaoDong ?? Convert.DBNull;
                    parameters[30].Value = ThongTinThiSinh.VanHoa ?? Convert.DBNull;
                    parameters[31].Value = ThongTinThiSinh.RLTT ?? Convert.DBNull;
                    parameters[32].Value = ThongTinThiSinh.TongSoDiemThi ?? Convert.DBNull;
                    parameters[33].Value = ThongTinThiSinh.NgayCapBang ?? Convert.DBNull;
                    parameters[34].Value = ThongTinThiSinh.DiemXL ?? Convert.DBNull;
                    parameters[35].Value = ThongTinThiSinh.DiemUT ?? Convert.DBNull;
                    parameters[36].Value = ThongTinThiSinh.GhiChu ?? Convert.DBNull;
                    parameters[37].Value = ThongTinThiSinh.Hang ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_Update", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Update_Success("thí sinh");
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
        public BaseResultModel Update_TrangThai(List<ThongTinThiSinh> ListThongTinThiSinh)
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
                                    new SqlParameter("Maped", SqlDbType.Int),
                                };
                                parameters[0].Value = item.ThiSinhID;
                                parameters[1].Value = item.Maped ?? Convert.DBNull;

                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThiSinh_UpdateTrangThaiMap", parameters);
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
        public BaseResultModel Delete(int ThiSinhID)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                  new SqlParameter("ThiSinhID",SqlDbType.Int)
                };
                parameters[0].Value = ThiSinhID;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_Delete", parameters);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Delete_Success("thí sinh");
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
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
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
        public ThongTinThiSinh GetByCMND(string CMND)
        {
            ThongTinThiSinh info = new ThongTinThiSinh();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("CMND",SqlDbType.NVarChar)
            };
            parameters[0].Value = CMND;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetByCMND", parameters))
                {
                    while (dr.Read())
                    {
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
                        info.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        info.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        info.TongSoDiemThi = Utils.ConvertToNullableDecimal(dr["TongSoDiemThi"], null);
                        info.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        info.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        info.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(dr["DiemTBCacBaiThi"], null);
                        info.DienUuTien = Utils.ConvertToString(dr["DienUuTien"], string.Empty);
                        info.DiemTBC = Utils.ConvertToNullableDecimal(dr["DiemTBC"], null);
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

        public ThongTinThiSinh GetThongTinThiSinh(int ThiSinhID)
        {
            ThongTinThiSinh Result = new ThongTinThiSinh();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@ThiSinhID",SqlDbType.Int)
            };
            parameters[0].Value = ThiSinhID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        //ThongTinThiSinh info = new ThongTinThiSinh();
                        Result.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        Result.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0); ;
                        Result.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        Result.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        Result.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        Result.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        Result.DanToc = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        Result.TenDanToc = Utils.ConvertToString(dr["TenDanToc"], string.Empty);
                        Result.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        Result.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        Result.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        Result.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        Result.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        Result.TruongTHPT = Utils.ConvertToNullableInt32(dr["TruongTHPT"], null);
                        Result.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        Result.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        Result.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        Result.LaoDong = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        Result.VanHoa = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        Result.RLTT = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        Result.XepLoaiHanhKiem = Utils.ConvertToNullableInt32(dr["XepLoaiHanhKiem"], null);
                        Result.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        Result.Do = Utils.ConvertToString(dr["Do"], string.Empty);
                        Result.DoThem = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        Result.Hong = Utils.ConvertToString(dr["Hong"], string.Empty);
                        Result.XepLoaiHocLuc = Utils.ConvertToNullableInt32(dr["XepLoaiHocLuc"], null);
                        Result.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        Result.DiemTBLop12 = Utils.ConvertToNullableDecimal(dr["DiemTBLop12"], null);
                        Result.DiemXL = Utils.ConvertToNullableDecimal(dr["DiemXL"], null);
                        Result.DiemUT = Utils.ConvertToNullableDecimal(dr["DiemUT"], null);
                        Result.DiemKK = Utils.ConvertToNullableDecimal(dr["DiemKK"], null);
                        Result.DienXTN = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        Result.HoiDongThi = Utils.ConvertToNullableInt32(dr["HoiDongThi"], null);
                        Result.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        Result.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        Result.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        Result.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        Result.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        Result.NamThi = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        Result.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        Result.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        Result.TongSoDiemThi = Utils.ConvertToNullableDecimal(dr["TongSoDiemThi"], null);
                        Result.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        Result.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        Result.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(dr["DiemTBCacBaiThi"], null);
                        Result.DienUuTien = Utils.ConvertToString(dr["DienUuTien"], string.Empty);
                        Result.DiemTBC = Utils.ConvertToNullableDecimal(dr["DiemTBC"], null);
                        Result.QueQuan = Utils.ConvertToString(dr["QueQuan"], string.Empty);
                        Result.ChungNhanNghe = Utils.ConvertToString(dr["ChungNhanNghe"], string.Empty);
                        Result.DTConLietSi = Utils.ConvertToString(dr["DTConLietSi"], string.Empty);
                        Result.GiaiTDKT = Utils.ConvertToString(dr["GiaiTDKT"], string.Empty);
                        Result.HoiDong = Utils.ConvertToString(dr["HoiDong"], string.Empty);
                        Result.MonKN = Utils.ConvertToString(dr["MonKN"], string.Empty);
                        Result.TBCNMonKN = Utils.ConvertToString(dr["TBCNMonKN"], string.Empty);
                        Result.DiemThiCu = Utils.ConvertToString(dr["DiemThiCu"], string.Empty);
                        Result.DiemThiMoi = Utils.ConvertToString(dr["DiemThiMoi"], string.Empty);
                        Result.TongBQ = Utils.ConvertToString(dr["TongBQ"], string.Empty);

                        Result.BQA = Utils.ConvertToString(dr["BQA"], string.Empty);
                        Result.BQT = Utils.ConvertToString(dr["BQT"], string.Empty);
                        Result.DC = Utils.ConvertToString(dr["DC"], string.Empty);
                        Result.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);              
                        Result.BODY_DAODUC = Utils.ConvertToString(dr["BODY_DAODUC"], string.Empty);
                        Result.BODY_RLEV = Utils.ConvertToString(dr["BODY_RLEV"], string.Empty);
                        Result.BODY_DIENKK = Utils.ConvertToString(dr["BODY_DIENKK"], string.Empty);
                        Result.BODY_PHONGTHI = Utils.ConvertToString(dr["BODY_PHONGTHI"], string.Empty);
                        Result.BODY_DIEMTNC = Utils.ConvertToString(dr["BODY_DIEMTNC"], string.Empty);
                        Result.BODY_XLTNC = Utils.ConvertToString(dr["BODY_XLTNC"], string.Empty);
                        Result.BODY_TDTCU = Utils.ConvertToString(dr["BODY_TDTCU"], string.Empty);
                        Result.BODY_GIAIHSG = Utils.ConvertToString(dr["BODY_GIAIHSG"], string.Empty);
                        Result.BODY_GIAIHSGK = Utils.ConvertToString(dr["BODY_GIAIHSGK"], string.Empty);
                        Result.BODY_CHUNGCHINN = Utils.ConvertToString(dr["BODY_CHUNGCHINN"], string.Empty);
                        Result.BODY_CHUNGCHITH = Utils.ConvertToString(dr["BODY_CHUNGCHITH"], string.Empty);
                        Result.BODY_TONGDIEMMOI = Utils.ConvertToString(dr["BODY_TONGDIEMMOI"], string.Empty);
                        Result.BODY_BQAMOI = Utils.ConvertToString(dr["BODY_BQAMOI"], string.Empty);
                        Result.BODY_BQTMOI = Utils.ConvertToString(dr["BODY_BQTMOI"], string.Empty);
                        Result.BODY_SOCAPGIAYCN = Utils.ConvertToString(dr["BODY_SOCAPGIAYCN"], string.Empty);
                        Result.BODY_XLHT = Utils.ConvertToString(dr["BODY_XLHT"], string.Empty);
                        Result.BODY_QUOCGIA = Utils.ConvertToString(dr["BODY_QUOCGIA"], string.Empty);

                        Result.ListThongTinDiemThi = GetDuLieuDiemThi(ThiSinhID);
                        //if(Result.NgayCapBang != null)
                        //{
                        //    Result.NamTotNghiep = Result.NgayCapBang.Value.Year;
                        //}
                        //else if((Result.Hong == null) || (Result.Hong != null && Result.Hong.Length == 0))
                        //{
                        //    Result.NamTotNghiep = Result.NamThi;
                        //}
                        //Result.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        //Result.SoTrang = Utils.ConvertToNullableInt32(dr["SoTrang"], null);
                        //if(Result.NgaySinh != null)
                        //{
                        //    Result.NgaySinhStr = Result.NgaySinh.Value.ToString("dd/MM/yyyy");
                        //}
                        //Result.Add(info);
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
        public ThongTinThiSinh GetThongTinThiSinh_New(int ThiSinhID)
        {
            ThongTinThiSinh Result = new ThongTinThiSinh();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@ThiSinhID",SqlDbType.Int)
            };
            parameters[0].Value = ThiSinhID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetByID_New", parameters))
                {
                    while (dr.Read())
                    {
                        Result.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        Result.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        Result.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0); ;
                        Result.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        Result.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        Result.NgaySinhStr = Utils.ConvertToString(dr["NgaySinhStr"], string.Empty);
                        if (Result.NgaySinh != null && (Result.NgaySinhStr == null || Result.NgaySinhStr == ""))
                        {
                            Result.NgaySinhStr = Result.NgaySinh.Value.ToString("dd/MM/yyyy");
                        }
                        Result.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        Result.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
                        Result.DanToc = Utils.ConvertToNullableInt32(dr["DanToc"], null);
                        Result.TenDanToc = Utils.ConvertToString(dr["TenDanToc"], string.Empty);
                        Result.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        Result.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        Result.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        Result.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        Result.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        Result.TruongTHPT = Utils.ConvertToNullableInt32(dr["TruongTHPT"], null);
                        Result.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        Result.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        Result.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        Result.LaoDong = Utils.ConvertToString(dr["LaoDong"], string.Empty);
                        Result.VanHoa = Utils.ConvertToString(dr["VanHoa"], string.Empty);
                        Result.RLTT = Utils.ConvertToString(dr["RLTT"], string.Empty);
                        Result.XepLoaiHanhKiem = Utils.ConvertToNullableInt32(dr["XepLoaiHanhKiem"], null);
                        Result.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        Result.Do = Utils.ConvertToString(dr["Do"], string.Empty);
                        Result.DoThem = Utils.ConvertToString(dr["DoThem"], string.Empty);
                        Result.Hong = Utils.ConvertToString(dr["Hong"], string.Empty);
                        Result.XepLoaiHocLuc = Utils.ConvertToNullableInt32(dr["XepLoaiHocLuc"], null);
                        Result.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        Result.DiemTBLop12 = Utils.ConvertToNullableDecimal(dr["DiemTBLop12"], null);
                        Result.DiemXL = Utils.ConvertToNullableDecimal(dr["DiemXL"], null);
                        Result.DiemUT = Utils.ConvertToNullableDecimal(dr["DiemUT"], null);
                        Result.DiemKK = Utils.ConvertToNullableDecimal(dr["DiemKK"], null);
                        Result.DienXTN = Utils.ConvertToNullableInt32(dr["DienXTN"], null);
                        Result.HoiDongThi = Utils.ConvertToNullableInt32(dr["HoiDongThi"], null);
                        if(Result.HoiDongThi == null)
                        {
                            Result.HoiDongThi = Utils.ConvertToNullableInt32(dr["HoiDongThiID"], null);
                        }
                        Result.KhoaThiNgay = Utils.ConvertToNullableDateTime(dr["KhoaThiNgay"], null);
                        Result.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(dr["DiemXetTotNghiep"], null);
                        Result.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        Result.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        Result.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        Result.NgayCapBang = Utils.ConvertToNullableDateTime(dr["NgayCapBang"], null);
                        Result.NamThi = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        Result.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        Result.TrangThaiCapBang = Utils.ConvertToInt32(dr["TrangThaiCapBang"], 0);
                        Result.TongSoDiemThi = Utils.ConvertToNullableDecimal(dr["TongSoDiemThi"], null);
                        Result.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        Result.Hang = Utils.ConvertToString(dr["Hang"], string.Empty);
                        Result.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(dr["DiemTBCacBaiThi"], null);
                        Result.DienUuTien = Utils.ConvertToString(dr["DienUuTien"], string.Empty);
                        Result.DiemTBC = Utils.ConvertToNullableDecimal(dr["DiemTBC"], null);
                        Result.QueQuan = Utils.ConvertToString(dr["QueQuan"], string.Empty);
                        Result.ChungNhanNghe = Utils.ConvertToString(dr["ChungNhanNghe"], string.Empty);
                        Result.DTConLietSi = Utils.ConvertToString(dr["DTConLietSi"], string.Empty);
                        Result.GiaiTDKT = Utils.ConvertToString(dr["GiaiTDKT"], string.Empty);
                        Result.HoiDong = Utils.ConvertToString(dr["HoiDong"], string.Empty);
                        if (Result.HoiDong == string.Empty)
                        {
                            Result.HoiDong = Utils.ConvertToString(dr["TenHoiDongThi"], string.Empty);
                        }
                        Result.MonKN = Utils.ConvertToString(dr["MonKN"], string.Empty);
                        Result.TBCNMonKN = Utils.ConvertToString(dr["TBCNMonKN"], string.Empty);
                        Result.DiemThiCu = Utils.ConvertToString(dr["DiemThiCu"], string.Empty);
                        Result.DiemThiMoi = Utils.ConvertToString(dr["DiemThiMoi"], string.Empty);
                        Result.TongBQ = Utils.ConvertToString(dr["TongBQ"], string.Empty);

                        Result.BQA = Utils.ConvertToString(dr["BQA"], string.Empty);
                        Result.BQT = Utils.ConvertToString(dr["BQT"], string.Empty);
                        Result.DC = Utils.ConvertToString(dr["DC"], string.Empty);
                        Result.Ban = Utils.ConvertToString(dr["Ban"], string.Empty);
                        Result.BODY_DAODUC = Utils.ConvertToString(dr["BODY_DAODUC"], string.Empty);
                        Result.BODY_RLEV = Utils.ConvertToString(dr["BODY_RLEV"], string.Empty);
                        Result.BODY_DIENKK = Utils.ConvertToString(dr["BODY_DIENKK"], string.Empty);
                        Result.BODY_PHONGTHI = Utils.ConvertToString(dr["BODY_PHONGTHI"], string.Empty);
                        Result.BODY_DIEMTNC = Utils.ConvertToString(dr["BODY_DIEMTNC"], string.Empty);
                        Result.BODY_XLTNC = Utils.ConvertToString(dr["BODY_XLTNC"], string.Empty);
                        Result.BODY_TDTCU = Utils.ConvertToString(dr["BODY_TDTCU"], string.Empty);
                        Result.BODY_GIAIHSG = Utils.ConvertToString(dr["BODY_GIAIHSG"], string.Empty);
                        Result.BODY_GIAIHSGK = Utils.ConvertToString(dr["BODY_GIAIHSGK"], string.Empty);
                        Result.BODY_CHUNGCHINN = Utils.ConvertToString(dr["BODY_CHUNGCHINN"], string.Empty);
                        Result.BODY_CHUNGCHITH = Utils.ConvertToString(dr["BODY_CHUNGCHITH"], string.Empty);
                        Result.BODY_TONGDIEMMOI = Utils.ConvertToString(dr["BODY_TONGDIEMMOI"], string.Empty);
                        Result.BODY_BQAMOI = Utils.ConvertToString(dr["BODY_BQAMOI"], string.Empty);
                        Result.BODY_BQTMOI = Utils.ConvertToString(dr["BODY_BQTMOI"], string.Empty);
                        Result.BODY_SOCAPGIAYCN = Utils.ConvertToString(dr["BODY_SOCAPGIAYCN"], string.Empty);
                        Result.BODY_XLHT = Utils.ConvertToString(dr["BODY_XLHT"], string.Empty);
                        Result.BODY_QUOCGIA = Utils.ConvertToString(dr["BODY_QUOCGIA"], string.Empty);
                        Result.NamThi = Utils.ConvertToNullableInt32(dr["Nam"], null);
                        Result.NamTotNghiep = Utils.ConvertToNullableInt32(dr["NamThi"], null);
                        if (Result.NamTotNghiep == null && Result.NamThi > 0)
                        {
                            Result.NamTotNghiep = Result.NamThi;
                        }

                        Result.ListThongTinDiemThi = GetDuLieuDiemThi(ThiSinhID);
                     
                       
                        Result.SoQuyen = Utils.ConvertToString(dr["SoQuyen"], string.Empty);
                        Result.SoTrang = Utils.ConvertToNullableInt32(dr["SoTrang"], null);
                        //if (Result.NgaySinh != null)
                        //{
                        //    Result.NgaySinhStr = Result.NgaySinh.Value.ToString("dd/MM/yyyy");
                        //}
                        //Result.Add(info);
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
        public List<ThongTinDiemThi> GetDuLieuDiemThi(int ThiSinhID)
        {
            List<ThongTinDiemThi> Result = new List<ThongTinDiemThi>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@ThiSinhID",SqlDbType.Int)
            };
            parameters[0].Value = ThiSinhID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetByThiSinhID", parameters))
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
                        info.TenMonThi = Utils.ConvertToString(dr["TenMonThi"], string.Empty);
                        info.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
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
    }
}
