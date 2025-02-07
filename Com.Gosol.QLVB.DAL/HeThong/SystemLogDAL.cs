using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Ultilities;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;
using OfficeOpenXml;
using System.IO;

namespace Com.Gosol.QLVB.DAL.HeThong
{
    public interface ISystemLogDAL
    {
        public int Insert(SystemLogModel systemLogInfo);
        public List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow);
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay);
        public string NhatKyHeThong_ExportExcel(string rootPath, string pathFile, List<SystemLogPartialModel> data);
    }

    public class SystemLogDAL : ISystemLogDAL
    {
        #region Database query string

        private const string V1_SYSTEMLOG_INSERT = @"v1_SystemLog_Insert";
        private const string V1_SYSTEMLOG_GETPAGINGBYSEARCH = @"v1_SystemLog_GetPagingBySearch";
        private const string V1_SYSTEMLOG_GETPAGINGBYQUANTRIDULIEU = @"v1_SystemLog_GetPagingByQuanTriDuLieu";

        #endregion

        #region paramaters constant

        private const string PARM_SYSTEMLOGid = @"SystemLogid";
        private const string PARM_CanBoID = @"CanBoID";
        private const string PARM_LOGINFO = @"LogInfo";
        private const string PARM_LOGTIME = @"LogTime";
        private const string PARM_LOGTYPE = @"LogType";
        private const string PARAM_KEY = "@Keyword";
        private const string PARAM_START = "@Start";
        private const string PARAM_END = "@End";
        #endregion

        #region Insert, update
        private SqlParameter[] GetInsertParms()
        {
            SqlParameter[] parms = new SqlParameter[] {
                new SqlParameter(PARM_CanBoID, SqlDbType.Int),
                new SqlParameter(PARM_LOGINFO, SqlDbType.NVarChar),
                new SqlParameter(PARM_LOGTIME, SqlDbType.DateTime),
                new SqlParameter(PARM_LOGTYPE, SqlDbType.Int)
            };
            return parms;
        }

        private void SetInsertParms(SqlParameter[] parms, SystemLogModel systemLogInfo)
        {
            parms[0].Value = systemLogInfo.CanBoID;
            parms[1].Value = systemLogInfo.LogInfo;
            parms[2].Value = systemLogInfo.LogTime;
            parms[3].Value = systemLogInfo.LogType;
        }

        public int Insert(SystemLogModel systemLogInfo)
        {
            object val = null;
            SqlParameter[] parameters = GetInsertParms();
            SetInsertParms(parameters, systemLogInfo);
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, V1_SYSTEMLOG_INSERT, parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                conn.Close();
            }
            return Convert.ToInt32(val);
        }


        public List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {

            List<SystemLogPartialModel> list = new List<SystemLogPartialModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter("@TuNgay",SqlDbType.DateTime),
                new SqlParameter("@DenNgay",SqlDbType.DateTime)
            };
            parameters[0].Value = p.Keyword != null ? p.Keyword.Trim() : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.TuNgay ?? Convert.DBNull;
            parameters[7].Value = p.DenNgay ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_SystemLog_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogPartialModel item = new SystemLogPartialModel();
                        item.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        item.LogInfo = Utils.ConvertToString(dr["LogInfo"], "");
                        item.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.MinValue);

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


        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow)
        {

            List<SystemLogPartialModel> list = new List<SystemLogPartialModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),

            };
            parameters[0].Value = p.Keyword != null ? p.Keyword : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, V1_SYSTEMLOG_GETPAGINGBYQUANTRIDULIEU, parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogPartialModel item = new SystemLogPartialModel();
                        item.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        item.LogInfo = Utils.ConvertToString(dr["LogInfo"], "");
                        item.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.MinValue);

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
        #endregion
        // Lấy danh sách systemlog từ ngày đến ngày
        public List<SystemLogModel> GetAllSystemLogFromTo(string TuNgay, string DenNgay)
        {
            List<SystemLogModel> ListSystemLogs = new List<SystemLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TuNgay",SqlDbType.VarChar),
                   new SqlParameter("@DenNgay",SqlDbType.VarChar)
              };
            parameters[0].Value = TuNgay ?? Convert.DBNull;
            parameters[1].Value = DenNgay ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetListSystemLogFromTo", parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogModel SystemLogModel = new SystemLogModel();
                        SystemLogModel.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        SystemLogModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        SystemLogModel.LogInfo = Utils.ConvertToString(dr["LogInfo"], string.Empty);
                        SystemLogModel.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.Now);
                        SystemLogModel.LogType = Utils.ConvertToInt32(dr["LogType"], 0);
                        ListSystemLogs.Add(SystemLogModel);
                    }
                    dr.Close();
                    //if (canBo != null && canBo.CanBoID > 0)
                    //{
                    //    canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(CanBoID.Value);
                    //    // lây danh sách chức vụ của cán bộ
                    //}
                }
            }
            catch
            {
                throw;
            }
            return ListSystemLogs;
        }

        // Tạo file log
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay)
        {
            XDocument doc = new XDocument();
            var ListSystemLog = GetAllSystemLogFromTo(TuNgay, DenNgay);
            doc =
                  new XDocument(
                  new XElement("LogConfig", ListSystemLog.Select(x =>
                  new XElement("SystemLog", new XAttribute("SystemLogID", x.SystemLogid), new XAttribute("CanBoID", x.CanBoID), new XAttribute("LogInfo", x.LogInfo), new XAttribute("LogTime", x.LogTime), new
                XAttribute("LogType", x.LogType)))));
            doc.Save(SystemLogPath);
            return doc;
        }

        // Get log by canboid and  system log info

        public List<SystemLogModel> GetListSystemLogByCanBoAndLogInfo(int? CanBoID, string LogInfo)
        {
            List<SystemLogModel> ListSystemLogs = new List<SystemLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int),
                   new SqlParameter("@LogInfo",SqlDbType.NVarChar)
              };
            parameters[0].Value = CanBoID ?? Convert.DBNull;
            parameters[1].Value = LogInfo ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetListSystemLogByCanBoIDAndLogInfo", parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogModel SystemLogModel = new SystemLogModel();
                        SystemLogModel.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        SystemLogModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        SystemLogModel.LogInfo = Utils.ConvertToString(dr["LogInfo"], string.Empty);
                        SystemLogModel.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.Now);
                        SystemLogModel.LogType = Utils.ConvertToInt32(dr["LogType"], 0);
                        ListSystemLogs.Add(SystemLogModel);
                    }
                    dr.Close();
                    //if (canBo != null && canBo.CanBoID > 0)
                    //{
                    //    canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(CanBoID.Value);
                    //    // lây danh sách chức vụ của cán bộ
                    //}
                }
            }
            catch
            {
                throw;
            }
            return ListSystemLogs;
        }

        public string NhatKyHeThong_ExportExcel(string rootPath, string pathFile, List<SystemLogPartialModel> data)
        {  
            string path = rootPath + @"\Upload\NhatKyHeThong_Template.xlsx";
            FileInfo fileInfo = new FileInfo(path);
            FileInfo file = new FileInfo(rootPath + "\\" + pathFile);

            ExcelPackage package = new ExcelPackage(fileInfo);
            if (package.Workbook.Worksheets != null)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // get number of rows in the sheet
                int rows = worksheet.Dimension.Rows;
                int cols = worksheet.Dimension.Columns;
                
                if (data.Count > 0)
                {
                    worksheet.InsertRow(4, data.Count - 1, 3);
                    int stt = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        worksheet.Cells[i + 3, 1].Value = stt++;
                        worksheet.Cells[i + 3, 2].Value = data[i].TenCoQuan;
                        worksheet.Cells[i + 3, 3].Value = data[i].TenCanBo;
                        worksheet.Cells[i + 3, 4].Value = data[i].LogInfo;
                        worksheet.Cells[i + 3, 5].Value = data[i].LogTime.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                }

                // save changes
                package.SaveAs(file);
            }

            return pathFile;
        }
    }
}
