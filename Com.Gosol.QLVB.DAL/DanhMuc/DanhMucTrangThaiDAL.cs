using Com.Gosol.QLVB.DAL.EFCore;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.DanhMuc
{
    public interface IDanhMucTrangThaiDAL
    {
        public List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel);
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel);
        public BaseResultModel Delete(List<int> ListTrangThaiID);
        public DanhMucTrangThaiModel GetByID(int? TrangThaiID);

    }
    public class DanhMucTrangThaiDAL : IDanhMucTrangThaiDAL
    {
        private const string INSERT = @"v1_DanhMuc_TrangThai_Insert";
        private const string UPDATE = @"v1_DanhMuc_TrangThai_Update";
        private const string DELETE = @"v1_DanhMuc_TrangThai_Delete";
        private const string GETBYID = @"v1_DanhMuc_TrangThai_GetByID";
        private const string GETBYNAME = @"v1_DanhMuc_TrangThai_GetByName";
        private const string GETLISTPAGING = @"v1_DanhMuc_TrangThai_GetPagingBySearch";
        private const string GETAll = @"v1_DanhMuc_TrangThai_GetAll";
        // param constant
        private const string PARAM_TrangThaiID = @"TrangThaiID";
        private const string PARAM_TenTrangThai = @"TenTrangThai";
        private const string PARAM_TrangThaiSuDung = @"TrangThaiSuDung";
        // Insert
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucTrangThaiModel == null || DanhMucTrangThaiModel.TenTrangThai == null || DanhMucTrangThaiModel.TenTrangThai.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên trạng thái không được trống";
                }
                else if (DanhMucTrangThaiModel.TenTrangThai.Trim().Length > 50)
                {
                    Result.Status = 0;
                    Result.Message = "Tên trạng thái không được quá 50 ký tự";
                }
                else
                {
                    var TrangThai = GetByName(DanhMucTrangThaiModel.TenTrangThai);
                    if (TrangThai != null && TrangThai.TrangThaiID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Trạng thái đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter(PARAM_TenTrangThai, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_TrangThaiSuDung, SqlDbType.NVarChar)
                          };
                        parameters[0].Value = DanhMucTrangThaiModel.TenTrangThai.Trim();
                        parameters[1].Value = DanhMucTrangThaiModel.TrangThaiSuDung == null ? false : DanhMucTrangThaiModel.TrangThaiSuDung;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục trạng thái");
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
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        // Update
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                int crID;
                if (!int.TryParse(DanhMucTrangThaiModel.TrangThaiID.ToString(), out crID) || DanhMucTrangThaiModel.TrangThaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Trạng thái không tồn tại, hoặc TrangThaiID không đúng định dạng";
                }
                else if (DanhMucTrangThaiModel == null || DanhMucTrangThaiModel.TenTrangThai == null || DanhMucTrangThaiModel.TenTrangThai.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên trạng thái không được trống";
                }
                else if (DanhMucTrangThaiModel.TenTrangThai.Trim().Length > 50)
                {
                    Result.Status = 0;
                    Result.Message = "Tên trạng thái không được quá 50 ký tự";
                }
                else
                {
                    var crObj = GetByID(DanhMucTrangThaiModel.TrangThaiID);
                    var objDouble = GetByName(DanhMucTrangThaiModel.TenTrangThai.Trim());
                    if (crObj == null || crObj.TrangThaiID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Danh mục trạng thái không tồn tại";
                    }
                    else if (objDouble != null && objDouble.TrangThaiID > 0 && objDouble.TrangThaiID != DanhMucTrangThaiModel.TrangThaiID)
                    {
                        Result.Status = 0;
                        Result.Message = "Danh mục trạng thái đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                         {
                            new SqlParameter(PARAM_TrangThaiID, SqlDbType.Int),
                            new SqlParameter(PARAM_TenTrangThai, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_TrangThaiSuDung, SqlDbType.NVarChar)
                         };
                        parameters[0].Value = DanhMucTrangThaiModel.TrangThaiID;
                        parameters[1].Value = DanhMucTrangThaiModel.TenTrangThai.Trim();
                        parameters[2].Value = DanhMucTrangThaiModel.TrangThaiSuDung == null ? false : DanhMucTrangThaiModel.TrangThaiSuDung;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục trạng thái");
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }
        public BaseResultModel Delete(List<int> ListTrangThaiID)
        {
            BaseResultModel Result = new BaseResultModel();
            if (ListTrangThaiID.Count <= 0)
            {

                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                Result.Status = 0;
                return Result;
            }
            else
            {
                for (int i = 0; i < ListTrangThaiID.Count; i++)
                {

                    int crID;
                    if (!int.TryParse(ListTrangThaiID[i].ToString(), out crID))
                    {
                        Result.Message = "Trạng thái '" + ListTrangThaiID[i].ToString() + "' không đúng định dạng";
                        Result.Status = 0;
                        return Result;
                    }
                    else
                    {
                        var crObj = GetByID(ListTrangThaiID[i]);
                        if (crObj == null || crObj.TrangThaiID < 1)
                        {
                            Result.Message = "TrangThaiID " + GetByID(ListTrangThaiID[i]).TenTrangThai + " không tồn tại";
                            Result.Status = 0;
                            return Result;
                        }
                        else if (new HeThongCanBoDAL().GetCanBoByTrangThaiID(ListTrangThaiID[i]).Count > 0)
                        {
                            Result.Message = "Trạng thái " + GetByID(ListTrangThaiID[i]).TenTrangThai + " đang được sử dụng ! Không thể xóa";
                            Result.Status = 0;
                            return Result;
                        }
                        else
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                             {
                                new SqlParameter(PARAM_TrangThaiID, SqlDbType.Int)
                             };
                            parameters[0].Value = ListTrangThaiID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        int val = 0;
                                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE, parameters);
                                        trans.Commit();


                                    }
                                    catch
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                        trans.Rollback();
                                        return Result;
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Danh mục trạng thái");
                return Result;
            }
        }

        public DanhMucTrangThaiModel GetByID(int? TrangThaiID)
        {
            DanhMucTrangThaiModel TrangThai = null;
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_TrangThaiID,SqlDbType.Int)
              };
            parameters[0].Value = TrangThaiID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYID, parameters))
                {
                    while (dr.Read())
                    {
                        TrangThai = new DanhMucTrangThaiModel(Utils.ConvertToInt32(dr["TrangThaiID"], 0), Utils.ConvertToString(dr["TenTrangThai"], string.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return TrangThai;
        }

        public DanhMucTrangThaiModel GetByName(string TenTrangThai)
        {
            DanhMucTrangThaiModel TrangThai = new DanhMucTrangThaiModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenTrangThai",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenTrangThai.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYNAME, parameters))
                {
                    while (dr.Read())
                    {
                        TrangThai = new DanhMucTrangThaiModel(Utils.ConvertToInt32(dr["TrangThaiID"], 0), Utils.ConvertToString(dr["TenTrangThai"], string.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));
                        var id = TrangThai.TrangThaiID;

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return TrangThai;
        }

        public List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            //var db = _DbQLVBContext;
            //var xxx = db.DMTrangThai.Where(t => t.TenTrangThai.Contains("a")).ToList();

            List<DanhMucTrangThaiModel> list = new List<DanhMucTrangThaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),

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
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, GETLISTPAGING, parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucTrangThaiModel item = new DanhMucTrangThaiModel();
                        item.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        item.TenTrangThai = Utils.ConvertToString(dr["TenTrangThai"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true);
                        list.Add(item);
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
        public List<DanhMucTrangThaiModel> GetAll()
        {
            List<DanhMucTrangThaiModel> list = new List<DanhMucTrangThaiModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, GETAll))
                {
                    while (dr.Read())
                    {
                        DanhMucTrangThaiModel item = new DanhMucTrangThaiModel();
                        item.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        item.TenTrangThai = Utils.ConvertToString(dr["TenTrangThai"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true);
                        list.Add(item);
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
