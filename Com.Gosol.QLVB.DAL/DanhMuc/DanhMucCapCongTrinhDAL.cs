using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Com.Gosol.QLVB.DAL.KeKhai;

namespace Com.Gosol.QLVB.DAL.DanhMuc
{
    public interface IDanhMucCapCongTrinhDAL
    {
        public List<DanhMucCapCongTrinhModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public BaseResultModel Insert(DanhMucCapCongTrinhModel DanhMucChucVuModel);
        public BaseResultModel Update(DanhMucCapCongTrinhModel DanhMucChucVuModel);
        public List<string> Delete(List<int> ListChucVuID);
        public DanhMucCapCongTrinhModel GetByID(int ChucVuID);
    }
    public class DanhMucCapCongTrinhDAL : IDanhMucCapCongTrinhDAL
    {
        private const string INSERT = @"v1_DanhMuc_CapCongTrinh_Insert";
        private const string UPDATE = @"v1_DanhMuc_CapCongTrinh_Update";
        private const string DELETE = @"v1_DanhMuc_CapCongTrinh_Delete";
        private const string GETBYID = @"v1_DanhMuc_CapCongTrinh_GetByID";
        private const string GETBYNAME = @"v1_DanhMuc_CapCongTrinh_GetByName";
        private const string GETLISTPAGING = @"v1_DanhMuc_CapCongTrinh_GetPagingBySearch";

        // param constant
        private const string PARAM_CapCongTrinhID = @"CapCongTrinhID";
        private const string PARAM_TenCapCongTrinh = @"TenCapCongTrinh";
        private const string PARAM_CoQuanID = @"CoQuanID";
        private const string PARAM_TrangThaiSuDung = @"TrangThaiSuDung";


        public BaseResultModel Insert(DanhMucCapCongTrinhModel CapCongTrinh)
        {
            var Result = new BaseResultModel();
            try
            {
                if (CapCongTrinh == null || CapCongTrinh.TenCapCongTrinh == null || CapCongTrinh.TenCapCongTrinh.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên cấp công trình không được trống";
                }
                else if (CapCongTrinh.TenCapCongTrinh.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên cấp công trình không được quá 200 ký tự";
                }
                else
                {
                    var objDouble = GetByName(CapCongTrinh.TenCapCongTrinh.Trim());
                    if (objDouble != null && objDouble.CapCongTrinhID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Cấp công trình đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                     {
                        new SqlParameter(PARAM_TenCapCongTrinh, SqlDbType.NVarChar),
                        new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                        new SqlParameter(PARAM_TrangThaiSuDung, SqlDbType.Bit),
                    };
                        parameters[0].Value = CapCongTrinh.TenCapCongTrinh.Trim();
                        parameters[1].Value = CapCongTrinh.CoQuanID;
                        parameters[2].Value = CapCongTrinh.TrangThaiSuDung == null ? false : CapCongTrinh.TrangThaiSuDung;

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, INSERT, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Cấp công trình");
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

        public BaseResultModel Update(DanhMucCapCongTrinhModel CapCongTrinh)
        {
            var Result = new BaseResultModel();
            try
            {
                int crID;
                if (!int.TryParse(CapCongTrinh.CapCongTrinhID.ToString(), out crID) || CapCongTrinh.CapCongTrinhID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Cấp công trình không tồn tại, hoặc CapCongTrinhID không đúng định dạng";
                }
                else if (CapCongTrinh == null || CapCongTrinh.TenCapCongTrinh == null || CapCongTrinh.TenCapCongTrinh.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên cấp công trình không được trống";
                }
                else if (CapCongTrinh.TenCapCongTrinh.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên cấp công trình không được quá 200 ký tự";
                }
                else
                {
                    var crObj = GetByID(CapCongTrinh.CapCongTrinhID);
                    var objDouble = GetByName(CapCongTrinh.TenCapCongTrinh.Trim());
                    if (crObj == null || crObj.CapCongTrinhID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Cấp công trình không tồn tại";
                    }
                    else if (objDouble != null && objDouble.CapCongTrinhID > 0 && objDouble.CapCongTrinhID != CapCongTrinh.CapCongTrinhID)
                    {
                        Result.Status = 0;
                        Result.Message = "Cấp công trình đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                       {
                        new SqlParameter(PARAM_CapCongTrinhID, SqlDbType.Int),
                        new SqlParameter(PARAM_TenCapCongTrinh, SqlDbType.NVarChar),
                        new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                        new SqlParameter(PARAM_TrangThaiSuDung, SqlDbType.Bit),
                       };
                        parameters[0].Value = CapCongTrinh.CapCongTrinhID;
                        parameters[1].Value = CapCongTrinh.TenCapCongTrinh.Trim();
                        parameters[2].Value = CapCongTrinh.CoQuanID;
                        parameters[3].Value = CapCongTrinh.TrangThaiSuDung == null ? false : CapCongTrinh.TrangThaiSuDung;

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, UPDATE, parameters);
                                    trans.Commit();
                                    Result.Status = 1;
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("Tham số hệ thống");
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

        public List<string> Delete(List<int> ListCapCongTrinhID)
        {
             List<string> Result = new List<string>();
            if (ListCapCongTrinhID.Count <= 0)
            {
        
                Result.Add("Vui lòng chọn dữ liệu trước khi xóa");
               
            }
            else
            {
                for (int i = 0; i < ListCapCongTrinhID.Count; i++)
                {
                    int crID;
                    if (!int.TryParse(ListCapCongTrinhID[i].ToString(), out crID))
                    {                       
                        Result.Add(ListCapCongTrinhID[i].ToString());
                    }
                    else
                    {
                        var crObject = GetByID(ListCapCongTrinhID[i]);
                        if (crObject == null || crObject.CapCongTrinhID < 1)
                        {
                           Result.Add("CapCongTrinhID '" + ListCapCongTrinhID[i].ToString() + "' không tồn tại");
                        }

                        else if (new ThongTinTaiSanDAL().GetByIDCapCongTrinh(ListCapCongTrinhID[i]).Count > 0)
                        {                         
                            Result.Add("CapCongTrinhID '" + ListCapCongTrinhID[i].ToString() + "' đã có Thông tin tài sản sử dụng, không thể xóa!");
                        }
                        else
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                            new SqlParameter(PARAM_CapCongTrinhID, SqlDbType.Int)
                            };
                            parameters[0].Value = ListCapCongTrinhID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE, parameters);
                                        trans.Commit();
                                    }
                                    catch
                                    {
                                        trans.Rollback();
                                        return Result;
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                }
              
            }
            return Result;
        }


        public DanhMucCapCongTrinhModel GetByID(int CapCongTrinhID)
        {
            DanhMucCapCongTrinhModel CapCongTrinh = null;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_CapCongTrinhID,SqlDbType.Int)
            };
            parameters[0].Value = CapCongTrinhID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYID, parameters))
                {
                    while (dr.Read())
                    {
                        CapCongTrinh = new DanhMucCapCongTrinhModel();
                        CapCongTrinh.CapCongTrinhID = Utils.ConvertToInt32(dr["CapCongTrinhID"], 0);
                        CapCongTrinh.TenCapCongTrinh = Utils.ConvertToString(dr["TenCapCongTrinh"], "");
                        CapCongTrinh.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        CapCongTrinh.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return CapCongTrinh;
        }

        public DanhMucCapCongTrinhModel GetByName(string TenCapCongTrinh)
        {
            DanhMucCapCongTrinhModel CapCongTrinh = new DanhMucCapCongTrinhModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_TenCapCongTrinh,SqlDbType.NVarChar)
            };
            parameters[0].Value = TenCapCongTrinh;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, GETBYNAME, parameters))
                {
                    while (dr.Read())
                    {
                        CapCongTrinh = new DanhMucCapCongTrinhModel();
                        CapCongTrinh.CapCongTrinhID = Utils.ConvertToInt32(dr["CapCongTrinhID"], 0);
                        CapCongTrinh.TenCapCongTrinh = Utils.ConvertToString(dr["TenCapCongTrinh"], "");
                        CapCongTrinh.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        CapCongTrinh.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);
                        break;
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return CapCongTrinh;
        }
        public List<DanhMucCapCongTrinhModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {

            List<DanhMucCapCongTrinhModel> list = new List<DanhMucCapCongTrinhModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
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
                        DanhMucCapCongTrinhModel item = new DanhMucCapCongTrinhModel();
                        item.CapCongTrinhID = Utils.ConvertToInt32(dr["CapCongTrinhID"], 0);
                        item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        item.TenCapCongTrinh = Utils.ConvertToString(dr["TenCapCongTrinh"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);

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


    }
}
