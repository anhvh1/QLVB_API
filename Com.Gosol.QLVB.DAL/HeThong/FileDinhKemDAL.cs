using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.IO;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.DanhMuc;

namespace Com.Gosol.QLVB.DAL.HeThong
{
    public interface IFileDinhKemDAL
    {
        public FileDinhKemModel GetByID(int FileDinhKemID, int FileType);
        public List<FileDinhKemModel> GetByNgiepVuID(int NghiepVuID, int FileType);
        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel);
        public BaseResultModel Delete(List<FileDinhKemModel> ListFileDinhKem);
    }
    public class FileDinhKemDAL : IFileDinhKemDAL
    {
        //store procedure
        private const string FILEDINHKEM_GET_BY_FILEID = @"v2_FileDinhKem_GetByID";
        private const string INSERT_FILEDINHKEM = @"v2_FileDinhKem_Insert";
        private const string FILEDINHKEM_GET_BY_NGHIEPVUID = @"v2_FileDinhKem_GetByNghiepVuID";
        private const string DELETE_FILEDINHKEM = @"v2_FielDinhKem_Delete";

        //params
        private const string FILE_ID = "FileID";
        private const string TEN_FILE = "TenFile";
        private const string TEN_FILE_HE_THONG = "TenFileHeThong";
        private const string NOI_DUNG = "NoiDung";
        private const string NGAY_CAP_NHAT = "NgayCapNhat";
        private const string NGUOI_CAP_NHAT = "NguoiCapNhat";
        private const string NGHIEP_VU_ID = "NghiepVuID";
        private const string FILE_TYPE = "FileType";
        private const string FILE_URL = "FileUrl";
        private const string ISMAHOA = "IsMaHoa";
        private const string ISBAOMAT = "IsBaoMat";

        public FileDinhKemModel GetByID(int FileDinhKemID, int FileType)
        {
            FileDinhKemModel crFile = new FileDinhKemModel();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(FILE_ID, SqlDbType.Int),
                new SqlParameter(FILE_TYPE,SqlDbType.Int)
             };
            parameters[0].Value = FileDinhKemID;
            parameters[1].Value = FileType;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILEDINHKEM_GET_BY_FILEID, parameters))
                {
                    while (dr.Read())
                    {
                        crFile = new FileDinhKemModel();
                        crFile.FileID = Utils.ConvertToInt32(dr[FILE_ID], 0);
                        crFile.NguoiCapNhat = Utils.ConvertToInt32(dr[NGUOI_CAP_NHAT], 0);
                        crFile.NgayCapNhat = Utils.ConvertToDateTime(dr[NGAY_CAP_NHAT], DateTime.Now);
                        crFile.TenFile = Utils.ConvertToString(dr[TEN_FILE], string.Empty);
                        crFile.NoiDung = Utils.ConvertToString(dr[NOI_DUNG], string.Empty);
                        crFile.FileType = Utils.ConvertToInt32(dr[FILE_TYPE], 0);
                        crFile.NghiepVuID = Utils.ConvertToInt32(dr[NGHIEP_VU_ID], 0);
                        crFile.FileUrl = Utils.ConvertToString(dr[FILE_URL], string.Empty);
                        crFile.IsBaoMat = Utils.ConvertToBoolean(dr[ISBAOMAT], false);
                        crFile.IsMaHoa = Utils.ConvertToBoolean(dr[ISMAHOA], false);
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

        public List<FileDinhKemModel> GetByNgiepVuID(int NghiepVuID, int FileType)
        {
            List<FileDinhKemModel> listFile = new List<FileDinhKemModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(NGHIEP_VU_ID, SqlDbType.Int),
                new SqlParameter(FILE_TYPE,SqlDbType.Int)
             };
            parameters[0].Value = NghiepVuID;
            parameters[1].Value = FileType;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILEDINHKEM_GET_BY_NGHIEPVUID, parameters))
                {
                    while (dr.Read())
                    {
                        var crFile = new FileDinhKemModel();
                        crFile.FileID = Utils.ConvertToInt32(dr[FILE_ID], 0);
                        crFile.NguoiCapNhat = Utils.ConvertToInt32(dr[NGUOI_CAP_NHAT], 0);
                        crFile.NgayCapNhat = Utils.ConvertToDateTime(dr[NGAY_CAP_NHAT], DateTime.Now);
                        crFile.TenFile = Utils.ConvertToString(dr[TEN_FILE], string.Empty);
                        crFile.NoiDung = Utils.ConvertToString(dr[NOI_DUNG], string.Empty);
                        crFile.FileType = Utils.ConvertToInt32(dr[FILE_TYPE], 0);
                        crFile.NghiepVuID = Utils.ConvertToInt32(dr[NGHIEP_VU_ID], 0);            
                        crFile.FileUrl = Utils.ConvertToString(dr[FILE_URL], string.Empty);
                        crFile.IsBaoMat = Utils.ConvertToBoolean(dr[ISBAOMAT], false);
                        crFile.IsMaHoa = Utils.ConvertToBoolean(dr[ISMAHOA], false);
                        listFile.Add(crFile);
                    }
                    dr.Close();
                }
                return listFile;
            }
            catch (Exception ex)
            {
                return new List<FileDinhKemModel>();
                throw ex;
            }
        }

        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (FileDinhKemModel == null || FileDinhKemModel.TenFile == null || FileDinhKemModel.TenFile.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên file gốc không được trống";
                    return Result;
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                         new SqlParameter(TEN_FILE_HE_THONG, SqlDbType.NVarChar),
                         new SqlParameter(NGAY_CAP_NHAT, SqlDbType.DateTime),
                         new SqlParameter(NGUOI_CAP_NHAT, SqlDbType.Int),
                         new SqlParameter(NGHIEP_VU_ID, SqlDbType.Int),
                         new SqlParameter(FILE_TYPE, SqlDbType.NVarChar),
                         new SqlParameter(FILE_URL, SqlDbType.NVarChar),
                    };
                    parameters[0].Value = FileDinhKemModel.TenFile.Trim();
                    parameters[1].Value = FileDinhKemModel.NgayCapNhat;
                    parameters[2].Value = FileDinhKemModel.NguoiCapNhat;
                    parameters[3].Value = FileDinhKemModel.NghiepVuID ?? Convert.DBNull;
                    parameters[4].Value = FileDinhKemModel.FileType;
                    parameters[5].Value = FileDinhKemModel.FileUrl ?? Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, INSERT_FILEDINHKEM, parameters), 0);
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
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in ListFileDinhKem)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter(FILE_ID, SqlDbType.Int),
                                    new SqlParameter(FILE_TYPE,SqlDbType.Int)
                                };
                                parameters[0].Value = item.FileID;
                                parameters[1].Value = item.FileType;
                                var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_FILEDINHKEM, parameters);
                            }

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
                    foreach (var item in ListFileDinhKem)
                    {
                        File.Delete(item.FileUrl);
                    }
                }
                Result.Message = ConstantLogMessage.Alert_Delete_Success("File đính kèm");
                return Result;
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

        public void ClearFolderFileTemp(string FolderPath)
        {
            //File.
        }
    }
}
