using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Data;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IKeKhaiDAL
    {
        public List<KeKhaiModel> GetAll();

        //edited by CHungNN 20/1/2021
        public int GetKeKhaiIDByThongTinTaiSanID(int ThongTinTaiSanID);
        public KeKhaiModel GetByID(int KeKhaiID);
        public BaseResultModel UpdateBarcode(KeKhaiModel KeKhaiModel);
        //////////////////////////////////////////////////////////////////////
    }
    public class KeKhaiDAL : IKeKhaiDAL
    {
        //tên các store procedure
        private const string KE_KHAI_GET_BY_ID = @"v1_KeKhai_GetByID"; 
        private const string KE_KHAI_GET_BY_DOT_KE_KHAIID_AND_CANBOID = @"v1_KeKhai_GetByDotKeKhaiIDAndCanBoID"; 
        private const string KE_KHAI_GET_BY_CANBOID_AND_NAMKEKHAI = @"v1_KeKhai_GetByCanBoIDAndNamKeKhai"; 
        private const string KE_KHAI_GET_ALL = @"v1_KeKhai_GetAll"; 
        private const string KE_KHAI_GET_BY_DOTKEKHAIID = @"v1_KeKhai_GetBy_DotKeKhaiID"; 
        private const string KE_KHAI_INSERT = @"v1_KeKhai_Insert"; 
        private const string KE_KHAI_UPDATE_BARCODE = @"v1_KeKhai_UpdateBarcode"; 
        private const string KE_KHAI_GET_LIST_BY_CANBOID = @"v1_KeKhai_GetListByCanBoID"; 
        private const string KE_KHAI_GET_BY_CANBOID_AND_TRANGTHAI = @"v1_KeKhai_GetAllKeKhaiByCanBoIDAndTrangThai"; 
        private const string KE_KHAI_GET_BY_CANBOID = @"v1_KeKhai_GetKeKhaiByCanBoID"; 
        private const string KE_KHAI_GET_BY_THONGTINTAISANID = @"v1_KeKhai_GetKeKhaiID_By_ThongTinTaiSanID"; 

        //Ten các params
        private const string KE_KHAI_ID = "NV00301";
        private const string DOT_KE_KHAI_ID = "NV00302";
        private const string CAN_BO_ID = "NV00303";
        private const string NAM_KE_KHAI = "NV00304";
        private const string TRANG_THAI = "NV00305";
        private const string TEN_BAN_KE_KHAI = "NV00306";
        private const string BIEN_DONG = "NV00307";
        private const string TRANG_THAI_NHAC_VIEC_DASBOARD = "NV00308";
        private const string TRANG_THAI_CONG_KHAI = "NV00309";
        private const string BARCODE = "NV00310";
        private const string LOAI_DOT_KE_KHAI = "NV00106";
        private const string THONG_TIN_TAI_SAN_ID = "NV00401";
        private const string KE_KHAI_ID_THONGTINTAISAN = "NV00402";

        public KeKhaiModel GetByID(int KeKhaiID)
        {
            KeKhaiModel KeKhai = new KeKhaiModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(KE_KHAI_ID,System.Data.SqlDbType.Int)
            };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_ID, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhai = new KeKhaiModel();
                        KeKhai.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        KeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        KeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        KeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        KeKhai.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        KeKhai.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        KeKhai.LoaiDotKeKhaiID = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                    }
                    dr.Close();
                }
                return KeKhai;
            }
            catch (Exception ex)
            {
                return new KeKhaiModel();
                throw ex;
            }
        }

        public KeKhaiModel GetByDotKeKhaiIDAndCanBoID(int DotKeKhaiID, int CanBoID)
        {
            KeKhaiModel KeKhai = new KeKhaiModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(DOT_KE_KHAI_ID,System.Data.SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,System.Data.SqlDbType.Int)
            };
            parameters[0].Value = DotKeKhaiID;
            parameters[1].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_DOT_KE_KHAIID_AND_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhai = new KeKhaiModel();
                        KeKhai.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        KeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        KeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        KeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        KeKhai.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        KeKhai.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                    }
                    dr.Close();
                }
                return KeKhai;
            }
            catch (Exception)
            {
                return new KeKhaiModel();
                throw;
            }
        }

        public List<KeKhaiModel> GetByCanBoIDAndNamKeKhai(int CanBoID, int NamKeKhai)
        {
            List<KeKhaiModel> KeKhai = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(CAN_BO_ID,System.Data.SqlDbType.Int),
                new SqlParameter(NAM_KE_KHAI,System.Data.SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            parameters[1].Value = NamKeKhai;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_CANBOID_AND_NAMKEKHAI, parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new KeKhaiModel();

                        crObj.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        crObj.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        crObj.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crObj.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crObj.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        crObj.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        KeKhai.Add(crObj);

                    }
                    dr.Close();
                }
                return KeKhai;
            }
            catch (Exception)
            {
                return new List<KeKhaiModel>();
                throw;
            }
        }

        public List<KeKhaiModel> GetByDotKeKhaiID(int DotKeKhaiID)
        {
            List<KeKhaiModel> Result = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(DOT_KE_KHAI_ID,System.Data.SqlDbType.Int)
            };
            parameters[0].Value = DotKeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_DOTKEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        var KeKhai = new KeKhaiModel();
                        KeKhai.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        KeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        KeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        KeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        KeKhai.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        KeKhai.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        Result.Add(KeKhai);
                    }
                    dr.Close();
                }
                return Result;
            }
            catch (Exception)
            {
                return new List<KeKhaiModel>();
                throw;
            }
        }

        public List<KeKhaiModel> GetAll()
        {
            List<KeKhaiModel> list = new List<KeKhaiModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_ALL))
                {
                    while (dr.Read())
                    {
                        KeKhaiModel KeKhai = new KeKhaiModel();
                        KeKhai.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        KeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        KeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        KeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        KeKhai.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        KeKhai.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        list.Add(KeKhai);
                    }
                    dr.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<KeKhaiModel>();
                throw ex;
            }
        }

        public BaseResultModel Insert(KeKhaiModel KeKhaiModel)
        {
            Boolean laBanTam = false;
            //new DotKeKhaiDAL().CheckTrangThai();
            var Result = new BaseResultModel();
            try
            { 
                if (KeKhaiModel == null || KeKhaiModel.DotKeKhaiID < 1 || KeKhaiModel.CanBoID < 1 || (KeKhaiModel.NamKeKhai < 1 && !laBanTam))
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin bản kê khai chưa đẩy đủ";
                }

                else
                {
                    var dobleKeKhai = GetByDotKeKhaiIDAndCanBoID(KeKhaiModel.DotKeKhaiID, KeKhaiModel.CanBoID);
                    if (dobleKeKhai != null && dobleKeKhai.KeKhaiID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Bản kê khai đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter(DOT_KE_KHAI_ID, SqlDbType.Int),
                            new SqlParameter(CAN_BO_ID, SqlDbType.Int),
                            new SqlParameter(NAM_KE_KHAI, SqlDbType.Int),
                            new SqlParameter(TRANG_THAI, SqlDbType.Int),
                            new SqlParameter(TEN_BAN_KE_KHAI, SqlDbType.NVarChar)
                          };
                        parameters[0].Value = KeKhaiModel.DotKeKhaiID;
                        parameters[1].Value = KeKhaiModel.CanBoID;
                        parameters[2].Value = KeKhaiModel.NamKeKhai;
                        parameters[3].Value = KeKhaiModel.TrangThai;
                        parameters[4].Value = KeKhaiModel.TenBanKeKhai;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var KeKhaiID = SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, KE_KHAI_INSERT, parameters);
                                    //Result.Status = Utils.ConvertToInt32(KeKhaiID, 0);
                                    Result.Data = KeKhaiID;
                                    trans.Commit();
                                    Result.Status = 1;
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Bản kê khai");
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

        public BaseResultModel UpdateBarcode(KeKhaiModel KeKhaiModel)
        {  
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                  new SqlParameter(KE_KHAI_ID, SqlDbType.Int),
                  new SqlParameter(BARCODE, SqlDbType.NVarChar)
                };
                parameters[0].Value = KeKhaiModel.KeKhaiID;
                parameters[1].Value = KeKhaiModel.Barcode;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, KE_KHAI_UPDATE_BARCODE, parameters);
                            Result.Data = KeKhaiModel.KeKhaiID;
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("Bản kê khai");
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

        public List<KeKhaiModel> GetList_By_CanBoID(int CanBoID)
        {
            List<KeKhaiModel> KeKhai = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(CAN_BO_ID,System.Data.SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_LIST_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new KeKhaiModel();
                        crObj.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        crObj.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        crObj.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crObj.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crObj.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        crObj.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        crObj.TrangThaiNhacViec = Utils.ConvertToBoolean(dr[TRANG_THAI_NHAC_VIEC_DASBOARD], false);
                        KeKhai.Add(crObj);
                    }
                    dr.Close();
                }
                return KeKhai;
            }
            catch (Exception ex)
            {
                return new List<KeKhaiModel>();
                throw ex;
            }
        }

        // lấy tất cả kê khai trong List cán bộ con and trang thai
        public List<KeKhaiModel> GetAllKeKhaiByListCanBoAndTrangThai(DataTable ListCanBo, int TrangThai)
        {
            List<KeKhaiModel> Result = new List<KeKhaiModel>();
            var pList = new SqlParameter("@ListCanBo", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(TRANG_THAI,System.Data.SqlDbType.Int),
                pList
            };
            parameters[0].Value = TrangThai;
            parameters[1].Value = ListCanBo;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_CANBOID_AND_TRANGTHAI, parameters))
                {
                    while (dr.Read())
                    {
                        var KeKhai = new KeKhaiModel();
                        KeKhai.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        KeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        KeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        KeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        KeKhai.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        KeKhai.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        KeKhai.BienDong = Utils.ConvertToBoolean(dr[BIEN_DONG], false);
                        Result.Add(KeKhai);
                    }
                    dr.Close();
                }
                return Result;
            }
            catch (Exception ex)
            {
                return new List<KeKhaiModel>();
                throw ex;
            }
        }

        public List<KeKhaiModel> GetKeKhaiByCanBoID(int CanBoID)
        {
            List<KeKhaiModel> KeKhai = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(CAN_BO_ID,System.Data.SqlDbType.Int)

            };
            parameters[0].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new KeKhaiModel();

                        crObj.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        crObj.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        crObj.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crObj.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crObj.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        crObj.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        KeKhai.Add(crObj);
                    }
                    dr.Close();
                }
                return KeKhai;
            }
            catch (Exception)
            {
                return new List<KeKhaiModel>();
                throw;
            }
        }

        public int GetKeKhaiIDByThongTinTaiSanID(int ThongTinTaiSanID)
        {
            var KeKhaiID = 0;
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter(THONG_TIN_TAI_SAN_ID,System.Data.SqlDbType.Int)
           };
            parameters[0].Value = ThongTinTaiSanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_THONGTINTAISANID, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                    }
                    dr.Close();
                }
                return KeKhaiID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
