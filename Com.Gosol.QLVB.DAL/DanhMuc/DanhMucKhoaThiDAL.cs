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
    public interface IDanhMucKhoaThiDAL
    {
        public List<DanhMucKhoaThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public BaseResultModel Insert(DanhMucKhoaThiModel DanhMucKhoaThiModel);
        public BaseResultModel Update(DanhMucKhoaThiModel DanhMucKhoaThiModel);
        public BaseResultModel Delete(int KhoaThiID);
        public DanhMucKhoaThiModel GetByID(int KhoaThiID);
    }
    public class DanhMucKhoaThiDAL : IDanhMucKhoaThiDAL
    {
        public List<DanhMucKhoaThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<DanhMucKhoaThiModel> list = new List<DanhMucKhoaThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@NgayThi",SqlDbType.DateTime),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.Nam ?? 0;
            parameters[7].Value = p.Ngay;
            if (p.Ngay == null) parameters[7].Value = DBNull.Value;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucKhoaThiModel DanhMucKhoaThiModel = new DanhMucKhoaThiModel();
                        DanhMucKhoaThiModel.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        DanhMucKhoaThiModel.TenKhoaThi = Utils.ConvertToString(dr["TenKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.MaKhoaThi = Utils.ConvertToString(dr["MaKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        DanhMucKhoaThiModel.Ngay = Utils.ConvertToNullableDateTime(dr["Ngay"], null);
                        list.Add(DanhMucKhoaThiModel);
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

        public List<DanhMucKhoaThiModel> GetByNam(int? Nam)
        {
            List<DanhMucKhoaThiModel> list = new List<DanhMucKhoaThiModel>();
            if(Nam == null || Nam == 0)
            {
                return list;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Nam",SqlDbType.Int)
              };
            parameters[0].Value = Nam; 

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_GetByNam", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucKhoaThiModel DanhMucKhoaThiModel = new DanhMucKhoaThiModel();
                        DanhMucKhoaThiModel.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        DanhMucKhoaThiModel.TenKhoaThi = Utils.ConvertToString(dr["TenKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.MaKhoaThi = Utils.ConvertToString(dr["MaKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        DanhMucKhoaThiModel.Ngay = Utils.ConvertToNullableDateTime(dr["Ngay"], null);
                        list.Add(DanhMucKhoaThiModel);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public BaseResultModel Insert(DanhMucKhoaThiModel DanhMucKhoaThiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucKhoaThiModel == null || DanhMucKhoaThiModel.TenKhoaThi == null || DanhMucKhoaThiModel.TenKhoaThi.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên khóa thi không được để trống";
                    return Result;
                }

                var crObj = GetByMaKhoaThi(DanhMucKhoaThiModel.MaKhoaThi);
                if (crObj.KhoaThiID > 0)
                {
                    Result.Status = 0;
                    Result.Message = "Mã khóa thi đã có trong hệ thống !";
                    return Result;
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("KhoaThiID", SqlDbType.Int),
                        new SqlParameter("TenKhoaThi", SqlDbType.NVarChar),
                        new SqlParameter("MaKhoaThi", SqlDbType.NVarChar),
                        new SqlParameter("Nam", SqlDbType.Int),
                        new SqlParameter("Ngay", SqlDbType.DateTime),
                    };

                    parameters[0].Direction = ParameterDirection.Output;
                    parameters[0].Size = 8;
                    parameters[1].Value = DanhMucKhoaThiModel.TenKhoaThi;
                    parameters[2].Value = DanhMucKhoaThiModel.MaKhoaThi ?? Convert.DBNull;
                    parameters[3].Value = DanhMucKhoaThiModel.Nam ?? Convert.DBNull;
                    parameters[4].Value = DanhMucKhoaThiModel.Ngay ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_Insert", parameters);
                                trans.Commit();
                                Result.Data = Utils.ConvertToInt32(parameters[0].Value, 0);
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục khóa thi");
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
        public BaseResultModel Update(DanhMucKhoaThiModel DanhMucKhoaThiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucKhoaThiModel == null || DanhMucKhoaThiModel.TenKhoaThi == null || DanhMucKhoaThiModel.TenKhoaThi.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên khóa thi không được để trống";
                    return Result;
                }

                var kt = GetByMaKhoaThi(DanhMucKhoaThiModel.MaKhoaThi);
                if (kt.KhoaThiID > 0 && kt.KhoaThiID != DanhMucKhoaThiModel.KhoaThiID)
                {
                    Result.Status = 0;
                    Result.Message = "Mã khóa thi đã có trong hệ thống !";
                    return Result;
                }
                else
                {
                    var crObj = GetByID(DanhMucKhoaThiModel.KhoaThiID);
                    if (crObj.KhoaThiID == 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Khóa thi không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("KhoaThiID", SqlDbType.Int),
                            new SqlParameter("TenKhoaThi", SqlDbType.NVarChar),
                            new SqlParameter("MaKhoaThi", SqlDbType.NVarChar),
                            new SqlParameter("Nam", SqlDbType.Int),
                            new SqlParameter("Ngay", SqlDbType.DateTime),
                        };

                        parameters[0].Value = DanhMucKhoaThiModel.KhoaThiID;
                        parameters[1].Value = DanhMucKhoaThiModel.TenKhoaThi;
                        parameters[2].Value = DanhMucKhoaThiModel.MaKhoaThi ?? Convert.DBNull;
                        parameters[3].Value = DanhMucKhoaThiModel.Nam ?? Convert.DBNull;
                        parameters[4].Value = DanhMucKhoaThiModel.Ngay ?? Convert.DBNull;

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_Update", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục khóa thi");
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
        public BaseResultModel Delete(int KhoaThiID)
        {
            var Result = new BaseResultModel();
            try
            {
                var crObj = GetByID(KhoaThiID);
                if (crObj.KhoaThiID == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Khóa thi không tồn tại";
                    return Result;
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                      new SqlParameter("KhoaThiID",SqlDbType.Int)
                    };
                    parameters[0].Value = KhoaThiID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_Delete", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Delete_Success("danh mục khóa thi");
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
        public DanhMucKhoaThiModel GetByID(int KhoaThiID)
        {
            DanhMucKhoaThiModel DanhMucKhoaThiModel = new DanhMucKhoaThiModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("KhoaThiID",SqlDbType.Int)
            };
            parameters[0].Value = KhoaThiID;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucKhoaThiModel.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        DanhMucKhoaThiModel.TenKhoaThi = Utils.ConvertToString(dr["TenKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.MaKhoaThi = Utils.ConvertToString(dr["MaKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        DanhMucKhoaThiModel.Ngay = Utils.ConvertToNullableDateTime(dr["Ngay"], null);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucKhoaThiModel;
        }

        public DanhMucKhoaThiModel GetByMaKhoaThi(string MaKhoaThi)
        {
            DanhMucKhoaThiModel DanhMucKhoaThiModel = new DanhMucKhoaThiModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("MaKhoaThi",SqlDbType.NVarChar)
            };
            parameters[0].Value = MaKhoaThi ?? Convert.DBNull;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucKhoaThi_GetByMaKhoaThi", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucKhoaThiModel.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);
                        DanhMucKhoaThiModel.TenKhoaThi = Utils.ConvertToString(dr["TenKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.MaKhoaThi = Utils.ConvertToString(dr["MaKhoaThi"], string.Empty);
                        DanhMucKhoaThiModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        DanhMucKhoaThiModel.Ngay = Utils.ConvertToNullableDateTime(dr["Ngay"], null);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucKhoaThiModel;
        }
    }
}
