using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IFileDinhKemLogDAL
    {
        public int Insert(FileDinhKemLogModel fileDinhKemLog, int ThaoTac);
        //public List<FileDinhKemLogModel> GetFileDinhKemLogByKekhaiID(int KeKhaiID);
        public List<FileLogModel> GetFileDinhKemLogByKekhaiID(int KeKhaiID);
    }

    public class FileDinhKemLogDAL : IFileDinhKemLogDAL
    {
        //tên các store procedure
        private const string INSERT_FILE_DINH_KEM_LOG = @"v1_KeKhai_FileDinhKemLog_Insert";
        private const string KE_KHAI_FILE_DINH_KEM_LOG_GET_BY_KEKHAIID = @"v1_KeKhai_FileDinhKemLog_GetByKeKhaiID";

        //Ten các params
        private const string File_LOG_ID = "NV01201";
        private const string FILE_ID = "NV01202";
        private const string TEN_FILE_GOC_CU = "NV01203";
        private const string TEN_FILE_HE_THONG_CU = "NV01204";
        private const string NGAY_CAP_NHAT = "NV01205";
        private const string NGUOI_CAP_NHAT = "NV01206";
        private const string KE_KHAI_ID_FILE_DINH_KEM_LOG = "NV01207";
        private const string DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM_LOG = "NV01208";
        private const string FILE_TYPE = "NV01209";
        private const string BASE64_STRING_CU = "NV01210";
        private const string TRANG_THAI = "NV01211";
        private const string THAO_TAC = "NV01212";
        private const string TEN_FILE_GOC_MOI = "NV01203_1";
        private const string TEN_FILE_HE_THONG_MOI = "NV01204_1";
        private const string BASE64_STRING_MOI = "NV01210_1";
        private const string FILE_ID_MOI = "NV01202_1";

        public List<FileLogModel> GetFileDinhKemLogByKekhaiID(int KeKhaiID)
        {
            var Result = new List<FileLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(KE_KHAI_ID_FILE_DINH_KEM_LOG, SqlDbType.Int),
            };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_FILE_DINH_KEM_LOG_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        if(Utils.ConvertToInt32(dr[FILE_TYPE], 0)==1)
                        {
                            var fileLog = new FileLogModel();
                            fileLog.FileIDCu = Utils.ConvertToInt32(dr[FILE_ID], 0);
                            fileLog.FileIDMoi = Utils.ConvertToInt32(dr[FILE_ID_MOI], 0);
                            fileLog.TenFileCu = Utils.ConvertToString(dr[TEN_FILE_GOC_CU], string.Empty);
                            fileLog.TenFileMoi = Utils.ConvertToString(dr[TEN_FILE_GOC_MOI], string.Empty);
                            fileLog.NgayChinhSua = Utils.ConvertLongToDate(Utils.ConvertToInt64(dr[NGAY_CAP_NHAT], 0));
                            fileLog.NguoiCapNhat = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            fileLog.ThaoTac = Utils.ConvertToInt32(dr[THAO_TAC], 0);
                            Result.Add(fileLog);
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public int Insert(FileDinhKemLogModel fileDinhKemLog, int ThaoTac)
        {
            var result = 0;
            SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter(FILE_ID,SqlDbType.Int),
                        new SqlParameter(TEN_FILE_GOC_CU,SqlDbType.NVarChar),
                        new SqlParameter(TEN_FILE_HE_THONG_CU,SqlDbType.NVarChar),
                        new SqlParameter(NGAY_CAP_NHAT,SqlDbType.BigInt),
                        new SqlParameter(KE_KHAI_ID_FILE_DINH_KEM_LOG,SqlDbType.Int),
                        new SqlParameter(DUYET_BAN_KE_KHAI_ID_FILE_DINH_KEM_LOG,SqlDbType.Int),
                        new SqlParameter(FILE_TYPE,SqlDbType.NVarChar),
                        new SqlParameter(BASE64_STRING_CU,SqlDbType.NVarChar),
                        new SqlParameter(TRANG_THAI,SqlDbType.Int),
                        new SqlParameter(THAO_TAC,SqlDbType.Int),
                        new SqlParameter(TEN_FILE_GOC_MOI,SqlDbType.NVarChar),
                        new SqlParameter(TEN_FILE_HE_THONG_MOI,SqlDbType.NVarChar),
                        new SqlParameter(BASE64_STRING_MOI, SqlDbType.NVarChar),
                        new SqlParameter(NGUOI_CAP_NHAT, SqlDbType.Int),
                        new SqlParameter(FILE_ID_MOI, SqlDbType.Int),
                        //new SqlParameter(NGAY_CAP_NHAT_MOI, SqlDbType.BigInt),

                };
            parameters[0].Value = fileDinhKemLog.FileID;
            parameters[1].Value = string.IsNullOrEmpty(fileDinhKemLog.TenFileGocCu) ? Convert.DBNull : fileDinhKemLog.TenFileGocCu;
            parameters[2].Value = string.IsNullOrEmpty(fileDinhKemLog.TenFileHeThongCu) ? Convert.DBNull : fileDinhKemLog.TenFileHeThongCu;
            parameters[3].Value = DateTime.Now.Ticks;
            parameters[4].Value = fileDinhKemLog.KeKhaiID;
            parameters[5].Value = (fileDinhKemLog.DuyetBanKeKhaiID is null || fileDinhKemLog.DuyetBanKeKhaiID == 0) ? Convert.DBNull : fileDinhKemLog.DuyetBanKeKhaiID == 0;
            parameters[6].Value = string.IsNullOrEmpty(fileDinhKemLog.FileType) ? Convert.DBNull : fileDinhKemLog.FileType;
            parameters[7].Value = string.IsNullOrEmpty(fileDinhKemLog.Base64StringCu) ? Convert.DBNull : fileDinhKemLog.Base64StringCu;
            parameters[8].Value = fileDinhKemLog.TrangThai;
            parameters[9].Value = ThaoTac;
            parameters[10].Value = string.IsNullOrEmpty(fileDinhKemLog.TenFileGocMoi) ? Convert.DBNull : fileDinhKemLog.TenFileGocMoi;
            parameters[11].Value = string.IsNullOrEmpty(fileDinhKemLog.TenFileHeThongMoi) ? Convert.DBNull : fileDinhKemLog.TenFileHeThongMoi;
            parameters[12].Value = string.IsNullOrEmpty(fileDinhKemLog.Base64StringMoi) ? Convert.DBNull : fileDinhKemLog.Base64StringMoi;
            parameters[13].Value = fileDinhKemLog.NguoiCapNhat == 0 ? Convert.DBNull : fileDinhKemLog.NguoiCapNhat;
            parameters[14].Value = fileDinhKemLog.FileIDMoi == 0 ? Convert.DBNull : fileDinhKemLog.FileIDMoi;
            //parameters[13].Value = DateTime.Now.Ticks;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_FILE_DINH_KEM_LOG, parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            return result;

        }

    }
}
