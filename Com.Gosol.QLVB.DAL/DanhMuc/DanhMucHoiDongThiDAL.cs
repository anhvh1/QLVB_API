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
    public interface IDanhMucHoiDongThiDAL
    {
        public List<DanhMucHoiDongThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public BaseResultModel Insert(DanhMucHoiDongThiModel DanhMucHoiDongThiModel);
        public BaseResultModel Update(DanhMucHoiDongThiModel DanhMucHoiDongThiModel);
        public BaseResultModel Delete(int HoiDongThiID);
        public DanhMucHoiDongThiModel GetByID(int HoiDongThiID);
        public List<DanhMucHoiDongThiModel> GetHoiDongThiByNam(int Nam);
    }
    public class DanhMucHoiDongThiDAL : IDanhMucHoiDongThiDAL
    {
        public List<DanhMucHoiDongThiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<DanhMucHoiDongThiModel> list = new List<DanhMucHoiDongThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucHoiDongThiModel DanhMucHoiDongThiModel = new DanhMucHoiDongThiModel();
                        DanhMucHoiDongThiModel.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        DanhMucHoiDongThiModel.TenHoiDong = Utils.ConvertToString(dr["TenHoiDong"], string.Empty);
                        DanhMucHoiDongThiModel.DiaDiemThi = Utils.ConvertToString(dr["DiaDiemThi"], string.Empty);
                        DanhMucHoiDongThiModel.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        DanhMucHoiDongThiModel.SoThiSinhDuThi = Utils.ConvertToInt32(dr["SoThiSinhDuThi"], 0);
                        list.Add(DanhMucHoiDongThiModel);
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

        public List<DanhMucHoiDongThiModel> GetAll()
        {
            List<DanhMucHoiDongThiModel> list = new List<DanhMucHoiDongThiModel>();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_GetAll", null))
                {
                    while (dr.Read())
                    {
                        DanhMucHoiDongThiModel DanhMucHoiDongThiModel = new DanhMucHoiDongThiModel();
                        DanhMucHoiDongThiModel.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        DanhMucHoiDongThiModel.TenHoiDong = Utils.ConvertToString(dr["TenHoiDong"], string.Empty);
                        DanhMucHoiDongThiModel.DiaDiemThi = Utils.ConvertToString(dr["DiaDiemThi"], string.Empty);
                        DanhMucHoiDongThiModel.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        DanhMucHoiDongThiModel.SoThiSinhDuThi = Utils.ConvertToInt32(dr["SoThiSinhDuThi"], 0);
                        list.Add(DanhMucHoiDongThiModel);
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
        public BaseResultModel Insert(DanhMucHoiDongThiModel DanhMucHoiDongThiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucHoiDongThiModel == null || DanhMucHoiDongThiModel.TenHoiDong == null || DanhMucHoiDongThiModel.TenHoiDong.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên hội đồng không được để trống";
                    return Result;
                } 
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("HoiDongThiID", SqlDbType.Int),
                        new SqlParameter("TenHoiDong", SqlDbType.NVarChar),
                        new SqlParameter("DiaDiemThi", SqlDbType.NVarChar),  
                        new SqlParameter("PhongThi", SqlDbType.NVarChar),
                        new SqlParameter("SoThiSinhDuThi", SqlDbType.Int),
                    };

                    parameters[0].Direction = ParameterDirection.Output;
                    parameters[0].Size = 8;
                    parameters[1].Value = DanhMucHoiDongThiModel.TenHoiDong;
                    parameters[2].Value = DanhMucHoiDongThiModel.DiaDiemThi ?? Convert.DBNull;           
                    parameters[3].Value = DanhMucHoiDongThiModel.PhongThi ?? Convert.DBNull;
                    parameters[4].Value = DanhMucHoiDongThiModel.SoThiSinhDuThi ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_Insert", parameters);
                                trans.Commit();
                                Result.Data = Utils.ConvertToInt32(parameters[0].Value, 0);
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục hội đồng thi");
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
        public BaseResultModel Update(DanhMucHoiDongThiModel DanhMucHoiDongThiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucHoiDongThiModel == null || DanhMucHoiDongThiModel.TenHoiDong == null || DanhMucHoiDongThiModel.TenHoiDong.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên hội đồng không được để trống";
                    return Result;
                }
                else
                {
                    var crObj = GetByID(DanhMucHoiDongThiModel.HoiDongThiID);
                    if (crObj.HoiDongThiID == 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Hội đồng không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("HoiDongThiID", SqlDbType.Int),
                            new SqlParameter("TenHoiDong", SqlDbType.NVarChar),
                            new SqlParameter("DiaDiemThi", SqlDbType.NVarChar),
                            new SqlParameter("PhongThi", SqlDbType.NVarChar),
                            new SqlParameter("SoThiSinhDuThi", SqlDbType.Int),
                        };

                        parameters[0].Value = DanhMucHoiDongThiModel.HoiDongThiID;
                        parameters[1].Value = DanhMucHoiDongThiModel.TenHoiDong;
                        parameters[2].Value = DanhMucHoiDongThiModel.DiaDiemThi ?? Convert.DBNull;
                        parameters[3].Value = DanhMucHoiDongThiModel.PhongThi ?? Convert.DBNull;
                        parameters[4].Value = DanhMucHoiDongThiModel.SoThiSinhDuThi ?? Convert.DBNull;

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_Update", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục hội đồng thi");
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
        public BaseResultModel Delete(int HoiDongThiID)
        {
            var Result = new BaseResultModel();
            try
            {
                var crObj = GetByID(HoiDongThiID);
                if (crObj.HoiDongThiID == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Hội đồng không tồn tại";
                    return Result;
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                      new SqlParameter("HoiDongThiID",SqlDbType.Int)
                    };
                    parameters[0].Value = HoiDongThiID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_Delete", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Delete_Success("danh mục hội đồng");
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
        public DanhMucHoiDongThiModel GetByID(int HoiDongThiID)
        {
            DanhMucHoiDongThiModel DanhMucHoiDongThiModel = new DanhMucHoiDongThiModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("HoiDongThiID",SqlDbType.Int)
            };
            parameters[0].Value = HoiDongThiID;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucHoiDongThiModel.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        DanhMucHoiDongThiModel.TenHoiDong = Utils.ConvertToString(dr["TenHoiDong"], string.Empty);
                        DanhMucHoiDongThiModel.DiaDiemThi = Utils.ConvertToString(dr["DiaDiemThi"], string.Empty);
                        DanhMucHoiDongThiModel.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        DanhMucHoiDongThiModel.SoThiSinhDuThi = Utils.ConvertToInt32(dr["SoThiSinhDuThi"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucHoiDongThiModel;
        }

        public DanhMucHoiDongThiModel GetByTenHoiDong(string TenHoiDong)
        {
            DanhMucHoiDongThiModel DanhMucHoiDongThiModel = new DanhMucHoiDongThiModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("TenHoiDong",SqlDbType.NVarChar)
            };
            parameters[0].Value = TenHoiDong ?? Convert.DBNull;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_GetByTenHoiDong", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucHoiDongThiModel.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        DanhMucHoiDongThiModel.TenHoiDong = Utils.ConvertToString(dr["TenHoiDong"], string.Empty);
                        DanhMucHoiDongThiModel.DiaDiemThi = Utils.ConvertToString(dr["DiaDiemThi"], string.Empty);
                        DanhMucHoiDongThiModel.PhongThi = Utils.ConvertToString(dr["PhongThi"], string.Empty);
                        DanhMucHoiDongThiModel.SoThiSinhDuThi = Utils.ConvertToInt32(dr["SoThiSinhDuThi"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DanhMucHoiDongThiModel;
        }

        public List<DanhMucHoiDongThiModel> GetHoiDongThiByNam(int Nam)
        {
            List<DanhMucHoiDongThiModel> list = new List<DanhMucHoiDongThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
               {
                  new SqlParameter("Nam",SqlDbType.Int)
               };
            parameters[0].Value = Nam;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucHoiDongThi_GetByNam", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucHoiDongThiModel DanhMucHoiDongThiModel = new DanhMucHoiDongThiModel();
                        DanhMucHoiDongThiModel.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThiID"], 0);
                        DanhMucHoiDongThiModel.TenHoiDong = Utils.ConvertToString(dr["TenHoiDong"], string.Empty);
                     
                        list.Add(DanhMucHoiDongThiModel);
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
    }
}
