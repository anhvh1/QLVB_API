using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IFileDinhKemDAL
    {
        public FileDinhKemModel GetByID(int FileDinhKemID);
        public List<FileDinhKemModel> GetByNghiepVuID(int NghiepVuID, int Type);
        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel);
        public List<FileDinhKemModel> GetListFileDinhKemByKeKhaiID(int KeKhaiID);
        public BaseResultModel Delete(List<int> ListFileDinhKemID);
        public BaseResultModel Delete(List<FileDinhKemModel> ListFileDinhKem);
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByKeKhaiID(int KeKhaiID);
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByNghiepVuID_AndType(int NghiepVuID, int Type);
        public List<FileDinhKemModelPar> GetHistoryFileDinhKem(int KeKhaiID);
        public BaseResultModel CopyFile(string Url, string UrlNew);
        public int GetLastFileDinhKemID();
        public BaseResultModel Insert_NhieuBanKeKhaiIDCungFile(FileDinhKemModel FileDinhKemModel);
    }
    public class FileDinhKemDAL : IFileDinhKemDAL
    {
        //tên các store procedure
        private const string FILEDINHKEM_GET_BY_FILEID = @"v1_KeKhai_FileDinhKem_GetByID";
        private const string INSERT_FILEDINHKEM = @"v1_KeKhai_FileDinhKem_Insert";
        private const string FILEDINHKEM_GET_BY_KEKHAIID = @"v1_KeKhai_GetFileDinhKemByKeKhaiID";
        private const string FILEDINHKEM_GET_BY_NGHIEPVUID = @"v1_KeKhai_GetFileDinhKemByNghiepVuID";
        private const string DELETE_FILEDINHKEM = @"v1_FielDinhKem_Delete";
        private const string DELETE_FILEDINHKEM_NEW = @"v1_KeKhai_FielDinhKem_Delete_New";
        private const string FILEDINHKEM_GET_HISTORYFILE = @"v1_KeKhai_GetHistoryFileDinhKem";
        private const string GET_LAST_FILE_DINH_KEM_ID = @"v1_KeKhai_FileDinhKem_GetLastID";

        //Ten các params
        private const string FILE_ID = "NV00601";
        private const string TEN_FILE_GOC = "NV00602";
        private const string TEN_FILE_HE_THONG = "NV00603";
        private const string NGAY_CAP_NHAT = "NV00604";
        private const string NGUOI_CAP_NHAT = "NV00605";
        private const string KE_KHAI_ID_FILE_DINH_KEM = "NV00606";
        private const string DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM = "NV00607";
        private const string FILE_TYPE = "NV00608";
        private const string BASE64_STRING = "NV00609";
        private const string TRANG_THAI = "NV00610";
        private const string FILE_URL = "NV00611";
        private const string FOLDER_PATH = "NV00612";

        public FileDinhKemModel GetByID(int FileDinhKemID)
        {
            FileDinhKemModel crFile = new FileDinhKemModel();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("FileDinhKemID", SqlDbType.Int)
             };
            parameters[0].Value = FileDinhKemID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_FileDinhKem_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        crFile = new FileDinhKemModel();
                        crFile.FileID = Utils.ConvertToInt32(dr["FileDinhKemID"], 0);
                        crFile.NguoiCapNhat = Utils.ConvertToInt32(dr["NguoiCapNhat"], 0);
                        crFile.NgayCapNhat = Utils.ConvertToDateTime(dr["NgayCapNhat"], DateTime.Now);
                        crFile.TenFileGoc = Utils.ConvertToString(dr["TenFileGoc"], string.Empty);
                        crFile.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        crFile.FileType = Utils.ConvertToInt32(dr["FileType"], 0);
                        crFile.NghiepVuID = Utils.ConvertToInt32(dr["NghiepVuID"], 0);
                        crFile.FileUrl = Utils.ConvertToString(dr["FileUrl"], string.Empty);
                        break;
                    }
                    dr.Close();
                }
                return crFile;
            }
            catch (Exception ex)
            {
                return new FileDinhKemModel();
                throw ex;
            }
        }

        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (FileDinhKemModel == null || FileDinhKemModel.TenFileGoc == null || FileDinhKemModel.TenFileGoc.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên file gốc không được trống";
                    return Result;
                }
                else
                {

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                         new SqlParameter("TenFileGoc", SqlDbType.NVarChar),
                         new SqlParameter("TenFileHeThong", SqlDbType.NVarChar),
                         new SqlParameter("NgayCapNhat", SqlDbType.DateTime),
                         new SqlParameter("NguoiCapNhat", SqlDbType.Int),
                         new SqlParameter("NghiepVuID", SqlDbType.Int),
                         new SqlParameter("FileType", SqlDbType.NVarChar),
                         new SqlParameter("FileUrl", SqlDbType.NVarChar),
                    };
                    parameters[0].Value = FileDinhKemModel.TenFileGoc.Trim();
                    parameters[1].Value = FileDinhKemModel.TenFileHeThong.Trim();
                    parameters[2].Value = FileDinhKemModel.NgayCapNhat;
                    parameters[3].Value = FileDinhKemModel.NguoiCapNhat;
                    parameters[4].Value = FileDinhKemModel.NghiepVuID ?? Convert.DBNull;      
                    parameters[5].Value = FileDinhKemModel.FileType;
                    parameters[6].Value = FileDinhKemModel.FileUrl ?? Convert.DBNull;


                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, "v1_FileDinhKem_Insert", parameters), 0);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1; Result.Data = query;
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("File đính kèm");
                                }
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public List<FileDinhKemModel> GetByNghiepVuID(int NghiepVuID, int Type)
        {
            List<FileDinhKemModel> list = new List<FileDinhKemModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("NghiepVuID", SqlDbType.Int),
                new SqlParameter("FileType", SqlDbType.Int)
             };
            parameters[0].Value = NghiepVuID;
            parameters[1].Value = Type;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_FileDinhKem_GetByNghiepVuID", parameters))
                {
                    while (dr.Read())
                    {
                        FileDinhKemModel FileDinhKemModel = new FileDinhKemModel();
                        FileDinhKemModel.FileID = Utils.ConvertToInt32(dr["FileDinhKemID"], 0);
                        FileDinhKemModel.NguoiCapNhat = Utils.ConvertToInt32(dr["NguoiCapNhat"], 0);
                        FileDinhKemModel.NgayCapNhat = Utils.ConvertToDateTime(dr["NgayCapNhat"], DateTime.Now);
                        FileDinhKemModel.TenFileGoc = Utils.ConvertToString(dr["TenFileGoc"], string.Empty);
                        FileDinhKemModel.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        FileDinhKemModel.FileType = Utils.ConvertToInt32(dr["FileType"], 0);
                        FileDinhKemModel.NghiepVuID = Utils.ConvertToInt32(dr["NghiepVuID"], 0);
                        FileDinhKemModel.FileUrl = Utils.ConvertToString(dr["FileUrl"], string.Empty);
                        list.Add(FileDinhKemModel);
                    }
                    dr.Close();
                }
                return list.OrderByDescending(x => x.KeKhaiID).ToList();
            }
            catch (Exception ex)
            {
                return new List<FileDinhKemModel>();
            }
        }


        public BaseResultModel Insert_NhieuBanKeKhaiIDCungFile(FileDinhKemModel FileDinhKemModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (FileDinhKemModel == null || FileDinhKemModel.TenFileGoc == null || FileDinhKemModel.TenFileGoc.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên file gốc không được trống";
                    return Result;
                }
                else
                {
                    var pList = new SqlParameter("@DanhSachKeKhaiID", SqlDbType.Structured);
                    pList.TypeName = "dbo.list_ID";
                    var tbKeKhaiID = new DataTable();
                    tbKeKhaiID.Columns.Add("ID", typeof(string));
                    FileDinhKemModel.DanhSachKeKhaiID.ForEach(x => tbKeKhaiID.Rows.Add(x));
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                         new SqlParameter(TEN_FILE_GOC, SqlDbType.NVarChar),
                         new SqlParameter(TEN_FILE_HE_THONG, SqlDbType.NVarChar),
                         new SqlParameter(NGAY_CAP_NHAT, SqlDbType.DateTime),
                         new SqlParameter(NGUOI_CAP_NHAT, SqlDbType.Int),
                         pList,
                         new SqlParameter(DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM, SqlDbType.Int),
                         new SqlParameter(FILE_TYPE, SqlDbType.NVarChar),
                         new SqlParameter(BASE64_STRING, SqlDbType.NVarChar),

                    };
                    parameters[0].Value = FileDinhKemModel.TenFileGoc.Trim();
                    parameters[1].Value = FileDinhKemModel.TenFileHeThong.Trim();
                    parameters[2].Value = FileDinhKemModel.NgayCapNhat;
                    parameters[3].Value = FileDinhKemModel.NguoiCapNhat;
                    parameters[4].Value = tbKeKhaiID;
                    parameters[5].Value = FileDinhKemModel.DuyetBanKeKhaiID ?? Convert.DBNull;
                    parameters[6].Value = FileDinhKemModel.FileType;
                    parameters[7].Value = FileDinhKemModel.FileUrl ?? Convert.DBNull;


                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "[v1_KeKhai_FileDinhKem_InsertNhieuBanKeKhaiID]", parameters), 0);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1; Result.Data = query;
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("File đính kèm");
                                }
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        // Lấy danh sách file đính kèm by KekhaiID

        public List<FileDinhKemModel> GetListFileDinhKemByKeKhaiID(int KeKhaiID)
        {
            List<FileDinhKemModel> list = new List<FileDinhKemModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(KE_KHAI_ID_FILE_DINH_KEM,System.Data.SqlDbType.Int)
             };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILEDINHKEM_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        FileDinhKemModelPar FileDinhKemModel = new FileDinhKemModelPar();
                        FileDinhKemModel.FileID = Utils.ConvertToInt32(dr[FILE_ID], 0);
                        FileDinhKemModel.DuyetBanKeKhaiID = Utils.ConvertToInt32(dr[DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.TenFileGoc = Utils.ConvertToString(dr[TEN_FILE_GOC], string.Empty);
                        FileDinhKemModel.TenFileHeThong = Utils.ConvertToString(dr[TEN_FILE_HE_THONG], string.Empty);
                        FileDinhKemModel.NgayCapNhat = Utils.ConvertToDateTime(dr[NGAY_CAP_NHAT], DateTime.Now);
                        FileDinhKemModel.NguoiCapNhat = Utils.ConvertToInt32(dr[NGUOI_CAP_NHAT], 0);
                        FileDinhKemModel.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        FileDinhKemModel.NghiepVuID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.FileUrl = Utils.ConvertToString(dr[BASE64_STRING], string.Empty);
                        FileDinhKemModel.TenCanBo = new HeThongCanBoDAL().GetCanBoByID(FileDinhKemModel.NguoiCapNhat).TenCanBo;
                        list.Add(FileDinhKemModel);
                    }
                    dr.Close();
                }
                list = list.OrderByDescending(x => x.FileID).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return new List<FileDinhKemModel>();
            }
        }

        // Lấy danh sách file đính kèm by KekhaiID
        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByKeKhaiID(int KeKhaiID)
        {
            List<FileDinhKemModel> list = new List<FileDinhKemModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(KE_KHAI_ID_FILE_DINH_KEM,System.Data.SqlDbType.Int)
             };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILEDINHKEM_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        FileDinhKemModel FileDinhKemModel = new FileDinhKemModel();
                        FileDinhKemModel.FileID = Utils.ConvertToInt32(dr[FILE_ID], 0);
                        FileDinhKemModel.DuyetBanKeKhaiID = Utils.ConvertToInt32(dr[DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.TenFileGoc = Utils.ConvertToString(dr[TEN_FILE_GOC], string.Empty);
                        FileDinhKemModel.TenFileHeThong = Utils.ConvertToString(dr[TEN_FILE_HE_THONG], string.Empty);
                        FileDinhKemModel.NgayCapNhat = Utils.ConvertToDateTime(dr[NGAY_CAP_NHAT], DateTime.Now);
                        FileDinhKemModel.NguoiCapNhat = Utils.ConvertToInt32(dr[NGUOI_CAP_NHAT], 0);
                        FileDinhKemModel.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.Base64String = Utils.ConvertToString(dr[BASE64_STRING], string.Empty);
                        FileDinhKemModel.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        FileDinhKemModel.NghiepVuID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.FileUrl = Utils.ConvertToString(dr[BASE64_STRING], string.Empty);
                        FileDinhKemModel.FileType = Utils.ConvertToInt32(dr[FILE_TYPE], 0);
                        list.Add(FileDinhKemModel);
                    }
                    dr.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<FileDinhKemModel>();
            }
        }

        public List<FileDinhKemModel> GetAllField_FileDinhKem_ByNghiepVuID_AndType(int NghiepVuID, int Type)
        {
            List<FileDinhKemModel> list = new List<FileDinhKemModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(KE_KHAI_ID_FILE_DINH_KEM,System.Data.SqlDbType.Int),
                new SqlParameter(FILE_TYPE,System.Data.SqlDbType.Int)
             };
            parameters[0].Value = NghiepVuID;
            parameters[1].Value = Type;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILEDINHKEM_GET_BY_NGHIEPVUID, parameters))
                {
                    while (dr.Read())
                    {
                        FileDinhKemModel FileDinhKemModel = new FileDinhKemModel();
                        FileDinhKemModel.FileID = Utils.ConvertToInt32(dr[FILE_ID], 0);
                        FileDinhKemModel.DuyetBanKeKhaiID = Utils.ConvertToInt32(dr[DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.TenFileGoc = Utils.ConvertToString(dr[TEN_FILE_GOC], string.Empty);
                        FileDinhKemModel.TenFileHeThong = Utils.ConvertToString(dr[TEN_FILE_HE_THONG], string.Empty);
                        FileDinhKemModel.NgayCapNhat = Utils.ConvertToDateTime(dr[NGAY_CAP_NHAT], DateTime.Now);
                        FileDinhKemModel.NguoiCapNhat = Utils.ConvertToInt32(dr[NGUOI_CAP_NHAT], 0);
                        FileDinhKemModel.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.Base64String = Utils.ConvertToString(dr[BASE64_STRING], string.Empty);
                        FileDinhKemModel.TenCanBo = new HeThongCanBoDAL().GetCanBoByID(FileDinhKemModel.NguoiCapNhat).TenCanBo;
                        FileDinhKemModel.NghiepVuID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.FileUrl = Utils.ConvertToString(dr[BASE64_STRING], string.Empty);
                        FileDinhKemModel.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        list.Add(FileDinhKemModel);
                    }
                    dr.Close();
                }
                return list.OrderByDescending(x => x.KeKhaiID).ToList();
            }
            catch (Exception ex)
            {
                return new List<FileDinhKemModel>();
            }
        }

        public BaseResultModel Delete(List<int> ListFileDinhKemID)
        {
            var Result = new BaseResultModel();
            if (ListFileDinhKemID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                for (int i = 0; i < ListFileDinhKemID.Count; i++)
                {
                    int crID;
                    if (!int.TryParse(ListFileDinhKemID[i].ToString(), out crID))
                    {
                        Result.Status = 0;
                        Result.Message = "FileDinhKemID '" + ListFileDinhKemID[i].ToString() + "' không đúng định dạng";
                        return Result;
                    }
                    else
                    {
                        var crObj = GetByID(ListFileDinhKemID[i]);
                        if (crObj == null || crObj.FileID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "FileDinhKemID '" + ListFileDinhKemID[i].ToString() + "' không tồn tại";
                            return Result;
                        }
                        else
                        {
                            ////Edited by ChungNN 21/01/2021
                            //KeKhaiModel BanKeKhai = new KeKhaiDAL().GetByID(crObj.KeKhaiID.Value);
                            //if (BanKeKhai.TrangThai > 100)
                            //{
                            //    FileDinhKemLogModel fileDinhKemLogForInsertLog = new FileDinhKemLogModel();
                            //    fileDinhKemLogForInsertLog.FileID = crObj.FileID;
                            //    fileDinhKemLogForInsertLog.TenFileGocCu = crObj.TenFileGoc;
                            //    fileDinhKemLogForInsertLog.TenFileHeThongCu = crObj.TenFileHeThong;
                            //    //fileDinhKemLogForInsertLog.NgayCapNhat = fileDinhKem.NgayCapNhat;
                            //    fileDinhKemLogForInsertLog.KeKhaiID = crObj.KeKhaiID;
                            //    fileDinhKemLogForInsertLog.NguoiCapNhat = crObj.NguoiCapNhat;
                            //    fileDinhKemLogForInsertLog.DuyetBanKeKhaiID = crObj.DuyetBanKeKhaiID;
                            //    fileDinhKemLogForInsertLog.FileType = crObj.FileType;
                            //    fileDinhKemLogForInsertLog.Base64StringCu = crObj.Base64String;
                            //    int temp = new FileDinhKemLogDAL().Insert(fileDinhKemLogForInsertLog, (int)EnumBanKeKhailog.Delete);
                            //}
                            ///////////////////////////////////////////////////////////////////////////////////////
                            var table = new DataTable();
                            table.Columns.Add("ID", typeof(string));
                            ListFileDinhKemID.ForEach(x => table.Rows.Add(x));

                            var pList = new SqlParameter("@ListFileDinhKemID", SqlDbType.Structured);
                            pList.TypeName = "dbo.list_ID";
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                pList
                            };
                            parameters[0].Value = table;

                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_FILEDINHKEM, parameters);
                                        trans.Commit();
                                        if (val < 0)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Không thể xóa FileDinhKem = " + ListFileDinhKemID[i];
                                            return Result;
                                        }
                                        else
                                        {
                                            break;
                                        }

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
                Result.Message = ConstantLogMessage.Alert_Delete_Success("File đính kèm");
                return Result;
            }
        }

        public BaseResultModel Delete(List<FileDinhKemModel> ListFileDinhKem)
        {
            var Result = new BaseResultModel();
            if (ListFileDinhKem.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                List<FileDinhKemModel> fileDinhKemOld = new List<FileDinhKemModel>();
                foreach (var item in ListFileDinhKem)
                {
                    fileDinhKemOld.Add(GetByID(item.FileID));
                }

                var table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                ListFileDinhKem.ForEach(x => table.Rows.Add(x.FileID));

                var pList = new SqlParameter("ListFileDinhKemID", SqlDbType.Structured);
                pList.TypeName = "dbo.list_ID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    pList
                };
                parameters[0].Value = table;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_FielDinhKem_Delete", parameters);
                            trans.Commit();
                            Result.Status = 1;
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
                if (Result.Status == 1)
                {
                    foreach (var item in fileDinhKemOld)
                    {
                        if (item.FileUrl != null && item.FileUrl.Length > 0) File.Delete(item.FileUrl);
                    }
                }
                Result.Message = ConstantLogMessage.Alert_Delete_Success("File đính kèm");
                return Result;
            }
        }
        // Get lịch sử upload file đính kèm
        public List<FileDinhKemModelPar> GetHistoryFileDinhKem(int KeKhaiID)
        {
            List<FileDinhKemModelPar> list = new List<FileDinhKemModelPar>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(KE_KHAI_ID_FILE_DINH_KEM,System.Data.SqlDbType.Int)
             };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILEDINHKEM_GET_HISTORYFILE, parameters))
                {
                    while (dr.Read())
                    {
                        FileDinhKemModelPar FileDinhKemModel = new FileDinhKemModelPar();
                        FileDinhKemModel.FileID = Utils.ConvertToInt32(dr[FILE_ID], 0);
                        FileDinhKemModel.DuyetBanKeKhaiID = Utils.ConvertToInt32(dr[DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.TenFileGoc = Utils.ConvertToString(dr[TEN_FILE_GOC], string.Empty);
                        FileDinhKemModel.TenFileHeThong = Utils.ConvertToString(dr[TEN_FILE_HE_THONG], string.Empty);
                        FileDinhKemModel.NgayCapNhat = Utils.ConvertToDateTime(dr[NGAY_CAP_NHAT], DateTime.Now);
                        FileDinhKemModel.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        FileDinhKemModel.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_FILE_DINH_KEM], 0);
                        FileDinhKemModel.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        FileDinhKemModel.Base64String = Utils.ConvertToString(dr[BASE64_STRING], string.Empty);
                        list.Add(FileDinhKemModel);
                    }
                    dr.Close();
                }
                list = list.OrderByDescending(x => x.FileID).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return new List<FileDinhKemModelPar>();
            }
        }

        public BaseResultModel CopyFile(string Url, string UrlNew)
        {
            var Result = new BaseResultModel();
            try
            {
                File.Copy(Url, UrlNew, true);
                Result.Status = 1;
            }
            catch (Exception)
            {
                Result.Status = 0;
                throw;
            }

            return Result;
        }

        public int GetLastFileDinhKemID()
        {
            var Result = 0;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GET_LAST_FILE_DINH_KEM_ID, null))
                {
                    if (dr.Read())
                    {
                        Result = Utils.ConvertToInt32(dr["LastID"], 0);
                    }
                    dr.Close();
                }
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }
        public void ClearFolderFileTemp(string FolderPath)
        {
            //File.
        }
    }
}
