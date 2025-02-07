using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.QLVB.DAL.DanhMuc
{
    public interface IDanhMucChungDAL
    {
        public List<DanhMucChungModel> GetPagingBySearch(BasePagingParams p, int LoaiDanhMuc, ref int TotalRow);
        public BaseResultModel Insert(DanhMucChungModel DanhMucChungModel);
        public BaseResultModel Update(DanhMucChungModel DanhMucChungModel);
        public BaseResultModel Delete(int ID);
        public DanhMucChungModel GetByID(int ID);
        public DanhMucChungModel GetByName(string Ten);
        public List<DanhMucChungModel> GetAll(int LoaiDanhMuc, int Nam);
        public List<DanhMucChungModel> GetTruongByNam(int Nam);
    }
    public class DanhMucChungDAL : IDanhMucChungDAL
    {
        public List<DanhMucChungModel> GetPagingBySearch(BasePagingParams p, int LoaiDanhMuc, ref int TotalRow)
        {
            List<DanhMucChungModel> list = new List<DanhMucChungModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@Loai",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = LoaiDanhMuc;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;
            parameters[7].Value = p.Nam ?? Convert.DBNull;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        DanhMucChungModel.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        DanhMucChungModel.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        DanhMucChungModel.ChucVu = Utils.ConvertToString(dr["ChucVu"], string.Empty);
                        DanhMucChungModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        DanhMucChungModel.Nam = Utils.ConvertToNullableInt32(dr["Nam"], null);
                        list.Add(DanhMucChungModel);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public BaseResultModel Insert(DanhMucChungModel DanhMucChungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucChungModel == null || DanhMucChungModel.Ten == null || DanhMucChungModel.Ten.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên không được để trống";
                    return Result;
                }

                if (DanhMucChungModel.Loai == null || DanhMucChungModel.Loai == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Loại danh mục không được để trống";
                    return Result;
                }
                else
                {
                    if (!string.IsNullOrEmpty(DanhMucChungModel.Ma))
                    {
                        var TempDM_Chung = GetByMa(DanhMucChungModel.Ma, DanhMucChungModel.Loai);
                        if (TempDM_Chung != null && TempDM_Chung.ID > 0)
                        {
                            Result.Status = 0;
                            if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode())
                            {
                                Result.Message = "Mã dân tộc đã có trong hệ thống !";
                            }
                            else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                            {
                                Result.Message = "Mã trường đã có trong hệ thống !";
                            }
                            else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_DienXetTotNghiep.GetHashCode())
                            {
                                Result.Message = "Mã diện xét tốt nghiệp đã có trong hệ thống !";
                            }
                            else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode())
                            {
                                Result.Message = "Mã hội đồng đã có trong hệ thống !";
                            }
                            else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode())
                            {
                                Result.Message = "Mã xếp loại đã có trong hệ thống !";
                            }
                            else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode())
                            {
                                Result.Message = "Mã môn học đã có trong hệ thống !";
                            }
                            else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode())
                            {
                                Result.Message = "Mã người duyệt đã có trong hệ thống !";
                            }
                            return Result;
                            //Random rd = new Random();
                            //int temp = rd.Next(1, 1000);
                            //DanhMucChungModel.Ma = DanhMucChungModel.Ma + temp;
                        }
                    }
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("ID", SqlDbType.Int),
                        new SqlParameter("Ten", SqlDbType.NVarChar),
                        new SqlParameter("Ma", SqlDbType.NVarChar),
                        new SqlParameter("TrangThai", SqlDbType.Bit),
                        new SqlParameter("GhiChu", SqlDbType.NVarChar),
                        new SqlParameter("Loai", SqlDbType.Int),
                        new SqlParameter("DiaChi", SqlDbType.NVarChar),
                        new SqlParameter("Email", SqlDbType.NVarChar),
                        new SqlParameter("DienThoai", SqlDbType.NVarChar),
                        new SqlParameter("ChucVu", SqlDbType.NVarChar),
                        new SqlParameter("CoQuanID", SqlDbType.Int),
                        new SqlParameter("Nam", SqlDbType.Int),
                    };
                    if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode())
                    {
                        if (string.IsNullOrEmpty(DanhMucChungModel.Ma))
                        {
                            Random rd = new Random();
                            int temp = rd.Next(1, 1000);
                            DanhMucChungModel.Ma = "MH_" + temp;
                        }
                        else
                        {
                            if (!DanhMucChungModel.Ma.Contains("mh") || DanhMucChungModel.Ma.ToUpper().Contains("MH"))
                            {
                                DanhMucChungModel.Ma = "MH_" + DanhMucChungModel.Ma;
                            }
                        }
                    }
                    //check danh mục theo tên
                    var DanhMucChungByTen = GetByName(DanhMucChungModel.Ten);
                    if (DanhMucChungByTen != null && DanhMucChungByTen.ID > 0 && DanhMucChungByTen.Loai == DanhMucChungModel.Loai)
                    {
                        Result.Status = 0;
                        if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode())
                        {
                            Result.Message = "Tên dân tộc đã có trong hệ thống !";
                        }
                        else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                        {
                            Result.Message = "Tên trường đã có trong hệ thống !";
                        }
                        else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_DienXetTotNghiep.GetHashCode())
                        {
                            Result.Message = "Tên diện xét tốt nghiệp đã có trong hệ thống !";
                        }
                        else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode())
                        {
                            Result.Message = "Tên hội dồng đã có trong hệ thống !";
                        }
                        else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode())
                        {
                            Result.Message = "Tên xếp loại đã có trong hệ thống !";
                        }
                        else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode())
                        {
                            Result.Message = "Tên môn học đã có trong hệ thống !";
                        }
                        else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode())
                        {
                            Result.Message = "Tên người duyệt đã có trong hệ thống !";
                        }
                        return Result;
                    }

                    parameters[0].Direction = ParameterDirection.Output;
                    parameters[0].Size = 8;
                    parameters[1].Value = DanhMucChungModel.Ten;
                    parameters[2].Value = string.IsNullOrEmpty(DanhMucChungModel.Ma) ? Convert.DBNull : DanhMucChungModel.Ma.ToUpper();
                    parameters[3].Value = DanhMucChungModel.TrangThai ?? Convert.DBNull;
                    parameters[4].Value = DanhMucChungModel.GhiChu ?? Convert.DBNull;
                    parameters[5].Value = DanhMucChungModel.Loai ?? Convert.DBNull;
                    parameters[6].Value = DanhMucChungModel.DiaChi ?? Convert.DBNull;
                    parameters[7].Value = DanhMucChungModel.Email ?? Convert.DBNull;
                    parameters[8].Value = DanhMucChungModel.DienThoai ?? Convert.DBNull;
                    parameters[9].Value = DanhMucChungModel.ChucVu ?? Convert.DBNull;
                    parameters[10].Value = DanhMucChungModel.CoQuanID ?? Convert.DBNull;
                    parameters[11].Value = DanhMucChungModel.Nam ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_Insert_New", parameters);
                                trans.Commit();
                                Result.Data = Utils.ConvertToInt32(parameters[0].Value, 0);
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục chung");
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
        public BaseResultModel Update(DanhMucChungModel DanhMucChungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucChungModel == null || DanhMucChungModel.Ten == null || DanhMucChungModel.Ten.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên không được để trống";
                    return Result;
                }
                else
                {
                    var crObj = GetByID(DanhMucChungModel.ID);
                    if (crObj.ID == 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Không tồn tại";
                        return Result;
                    }
                    else
                    {
                        if(DanhMucChungModel.Ma != crObj.Ma && !string.IsNullOrEmpty(DanhMucChungModel.Ma))
                        {
                            var TempDM_Chung = GetByMa(DanhMucChungModel.Ma, DanhMucChungModel.Loai);
                            if (TempDM_Chung != null && TempDM_Chung.ID > 0)
                            {
                                Result.Status = 0;
                                if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode())
                                {
                                    Result.Message = "Mã dân tộc đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                {
                                    Result.Message = "Mã trường đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_DienXetTotNghiep.GetHashCode())
                                {
                                    Result.Message = "Mã diện xét tốt nghiệp đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode())
                                {
                                    Result.Message = "Mã hội đồng đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode())
                                {
                                    Result.Message = "Mã xếp loại đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode())
                                {
                                    Result.Message = "Mã môn học đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode())
                                {
                                    Result.Message = "Mã người duyệt đã có trong hệ thống !";
                                }
                                return Result;
                                //Random rd = new Random();
                                //int temp = rd.Next(1, 1000);
                                //DanhMucChungModel.Ma = DanhMucChungModel.Ma + temp;
                            }
                        }
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("ID", SqlDbType.Int),
                            new SqlParameter("Ten", SqlDbType.NVarChar),
                            new SqlParameter("Ma", SqlDbType.NVarChar),
                            new SqlParameter("TrangThai", SqlDbType.Bit),
                            new SqlParameter("GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("Loai", SqlDbType.Int),
                            new SqlParameter("DiaChi", SqlDbType.NVarChar),
                            new SqlParameter("Email", SqlDbType.NVarChar),
                            new SqlParameter("DienThoai", SqlDbType.NVarChar),
                            new SqlParameter("ChucVu", SqlDbType.NVarChar),
                            new SqlParameter("CoQuanID", SqlDbType.Int),
                            new SqlParameter("Nam", SqlDbType.Int),
                        };

                        if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode() && DanhMucChungModel.Ma != crObj.Ma)
                        {
                            if (string.IsNullOrEmpty(DanhMucChungModel.Ma))
                            {
                                Random rd = new Random();
                                int temp = rd.Next(1, 1000);
                                DanhMucChungModel.Ma = "MH_" + temp;
                            }
                            else
                            {
                                if (!DanhMucChungModel.Ma.Contains("mh") || DanhMucChungModel.Ma.ToUpper().Contains("MH"))
                                {
                                    DanhMucChungModel.Ma = "MH_" + DanhMucChungModel.Ma;
                                }
                            }
                        }
                        if (crObj.Ten.Trim() != DanhMucChungModel.Ten.Trim())
                        {
                            //check danh mục theo tên
                            var DanhMucChungByTen = GetByName(DanhMucChungModel.Ten);
                            if (DanhMucChungByTen != null && DanhMucChungByTen.ID > 0 && DanhMucChungByTen.Loai == DanhMucChungModel.Loai && DanhMucChungByTen.Ten.Equals(DanhMucChungModel.Ten))
                            {
                                Result.Status = 0;
                                if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode())
                                {
                                    Result.Message = "Tên dân tộc đã có trong hệ thống !";
                                }
                                else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                {
                                    Result.Message = "Tên trường đã có trong hệ thống !";
                                }
                                else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_DienXetTotNghiep.GetHashCode())
                                {
                                    Result.Message = "Tên diện xét tốt nghiệp đã có trong hệ thống !";
                                }
                                else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode())
                                {
                                    Result.Message = "Tên hội đồng đã có trong hệ thống !";
                                }
                                else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode())
                                {
                                    Result.Message = "Tên xếp loại đã có trong hệ thống !";
                                }
                                else if (DanhMucChungByTen.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode())
                                {
                                    Result.Message = "Tên môn học đã có trong hệ thống !";
                                }
                                else if (DanhMucChungModel.Loai == EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode())
                                {
                                    Result.Message = "Tên người duyệt đã có trong hệ thống !";
                                }
                                return Result;
                            }
                        }

                        parameters[0].Value = DanhMucChungModel.ID;
                        parameters[1].Value = DanhMucChungModel.Ten;
                        parameters[2].Value = DanhMucChungModel.Ma ?? Convert.DBNull;
                        parameters[3].Value = DanhMucChungModel.TrangThai ?? Convert.DBNull;
                        parameters[4].Value = DanhMucChungModel.GhiChu ?? Convert.DBNull;
                        parameters[5].Value = DanhMucChungModel.Loai ?? Convert.DBNull;
                        parameters[6].Value = DanhMucChungModel.DiaChi ?? Convert.DBNull;
                        parameters[7].Value = DanhMucChungModel.Email ?? Convert.DBNull;
                        parameters[8].Value = DanhMucChungModel.DienThoai ?? Convert.DBNull;
                        parameters[9].Value = DanhMucChungModel.ChucVu ?? Convert.DBNull;
                        parameters[10].Value = DanhMucChungModel.CoQuanID ?? Convert.DBNull;
                        parameters[11].Value = DanhMucChungModel.Nam ?? Convert.DBNull;

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_Update_New", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục chung");
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
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public bool CheckDanhMucDuocSuDung(int ID)
        {
            var result = true;
            var SoBanGhiSuDung = 0;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                      new SqlParameter("ID",SqlDbType.Int)
                };
                parameters[0].Value = ID;

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_CheckDanhMucDuocSuDung", parameters))
                {
                    while (dr.Read())
                    {
                        SoBanGhiSuDung = Utils.ConvertToInt32(dr["SoBanGhiSuDung"], 0);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (SoBanGhiSuDung > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public DanhMucChungModel GetByMa(string Ma, int? Loai)
        {
            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Ma",SqlDbType.NVarChar),
              new SqlParameter("Loai",SqlDbType.Int),
            };
            parameters[0].Value = string.IsNullOrEmpty(Ma) ? Ma : Ma.Trim();
            parameters[1].Value = Loai ?? 0;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetByMa_New", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        DanhMucChungModel.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        DanhMucChungModel.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        DanhMucChungModel.ChucVu = Utils.ConvertToString(dr["ChucVu"], string.Empty);
                        DanhMucChungModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucChungModel;
        }
        public BaseResultModel Delete(int ID)
        {
            var Result = new BaseResultModel();
            try
            {
                var crObj = GetByID(ID);
                if (crObj.ID == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Không tồn tại";
                    return Result;
                }
                else
                {
                    if (CheckDanhMucDuocSuDung(ID))
                    {
                        Result.Status = 0;
                        if (crObj.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode())
                        {
                            Result.Message = "Dân tộc đang được sử dụng! Không thể xoá!";
                        }
                        else if (crObj.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                        {
                            Result.Message = "Trường đang được sử dụng! Không thể xoá!";
                        }
                        else if (crObj.Loai == EnumLoaiDanhMuc.DM_DienXetTotNghiep.GetHashCode())
                        {
                            Result.Message = "Diện xét tốt nghiệp đang được sử dụng! Không thể xoá!";
                        }
                        else if (crObj.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode())
                        {
                            Result.Message = "Hội dồng đang được sử dụng! Không thể xoá!";
                        }
                        else if (crObj.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode())
                        {
                            Result.Message = "Xếp loại đang được sử dụng! Không thể xoá!";
                        }
                        else if (crObj.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode())
                        {
                            Result.Message = "Môn học đang được sử dụng! Không thể xoá!";
                        }
                        return Result;
                    }

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                      new SqlParameter("ID",SqlDbType.Int)
                    };
                    parameters[0].Value = ID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_Delete", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Delete_Success("danh mục chung");
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
                Result.Message = ex.Message;
                throw ex;
            }
        }
        public DanhMucChungModel GetByID(int ID)
        {
            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("ID",SqlDbType.Int)
            };
            parameters[0].Value = ID;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        DanhMucChungModel.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        DanhMucChungModel.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        DanhMucChungModel.ChucVu = Utils.ConvertToString(dr["ChucVu"], string.Empty);
                        DanhMucChungModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        DanhMucChungModel.Nam = Utils.ConvertToNullableInt32(dr["Nam"], null);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucChungModel;
        }

        public DanhMucChungModel GetByName(string Ten)
        {
            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Ten",SqlDbType.NVarChar)
            };
            parameters[0].Value = string.IsNullOrEmpty(Ten) ? Ten : Ten.Trim();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetByTen", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        DanhMucChungModel.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        DanhMucChungModel.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        DanhMucChungModel.ChucVu = Utils.ConvertToString(dr["ChucVu"], string.Empty);
                        DanhMucChungModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucChungModel;
        }

        public DanhMucChungModel GetByName(string Ten, int Nam)
        {
            DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Ten",SqlDbType.NVarChar),
              new SqlParameter("Nam",SqlDbType.Int),
            };
            parameters[0].Value = string.IsNullOrEmpty(Ten) ? Ten : Ten.Trim();
            parameters[1].Value = Nam;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetByTen_New", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        DanhMucChungModel.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        DanhMucChungModel.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        DanhMucChungModel.ChucVu = Utils.ConvertToString(dr["ChucVu"], string.Empty);
                        DanhMucChungModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucChungModel;
        }
        public List<DanhMucChungModel> GetAll(int LoaiDanhMuc)
        {
            List<DanhMucChungModel> List = new List<DanhMucChungModel>();

            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Loai",SqlDbType.Int),
            };
            parameters[0].Value = LoaiDanhMuc;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetAll", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.ViTri = Utils.ConvertToString(dr["ViTri"], null);
                        DanhMucChungModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        List.Add(DanhMucChungModel);
                    }
                    dr.Close();
                }

            }
            catch
            {
                throw;
            }
            return List;
        }
        public List<DanhMucChungModel> GetAll(int LoaiDanhMuc, int Nam)
        {
            List<DanhMucChungModel> List = new List<DanhMucChungModel>();

            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Loai",SqlDbType.Int),
              new SqlParameter("Nam",SqlDbType.Int),
            };
            parameters[0].Value = LoaiDanhMuc;
            parameters[1].Value = Nam;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetAll_New", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        DanhMucChungModel.ViTri = Utils.ConvertToString(dr["ViTri"], null);
                        DanhMucChungModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        List.Add(DanhMucChungModel);
                    }
                    dr.Close();
                }

            }
            catch
            {
                throw;
            }
            return List;
        }

        public List<DanhMucChungModel> GetTruongByNam(int Nam)
        {
            List<DanhMucChungModel> List = new List<DanhMucChungModel>();

            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("Nam",SqlDbType.Int)
            };
            parameters[0].Value = Nam;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChung_GetTruongByNam", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                        DanhMucChungModel.ID = Utils.ConvertToInt32(dr["ID"], 0);
                        DanhMucChungModel.Ten = Utils.ConvertToString(dr["Ten"], string.Empty);
                        //DanhMucChungModel.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        //DanhMucChungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        //DanhMucChungModel.TrangThai = Utils.ConvertToBoolean(dr["TrangThai"], false);
                        //DanhMucChungModel.Loai = Utils.ConvertToInt32(dr["Loai"], 0);
                        //DanhMucChungModel.ViTri = Utils.ConvertToString(dr["ViTri"], null);
                        List.Add(DanhMucChungModel);
                    }
                    dr.Close();
                }

            }
            catch
            {
                throw;
            }
            return List;
        }
    }
}
