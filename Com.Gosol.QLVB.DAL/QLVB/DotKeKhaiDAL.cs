using AutoMapper;
using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IDotKeKhaiDAL
    {
        public BaseResultModel Insert(DotKeKhaiPartial DotKeKhaiPartial);
        public BaseResultModel Delete(List<int> ListDotKeKhaiID);
        public List<DotKeKhaiPartial> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CoQuanID, int CanBoID);
        public DotKeKhaiPartial GetByID(int DotKeKhaiID);
        public DotKeKhaiPartial GetByIDForEdit(int DotKeKhaiID);
        public BaseResultModel Insert_New(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID);
        public BaseResultModel Insert_New_v2(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID);
        public BaseResultModel Update_New(DotKeKhaiPartial DotKeKhaiPartial, int NguoiDungID, int CoQuanID);
        public int GetDotKeKhaiFitForCanBo(int CanBoID, int CoQuanID);
        public BaseResultModel Update(DotKeKhaiPartial DotKeKhaiPartial, int NguoiDungID, int CoQuanID);
        public BaseResultModel Insert(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID);
    }
    public class DotKeKhaiDAL : IDotKeKhaiDAL
    {
        //tên các store procedure
        private const string INSERT_DOT_KE_KHAI = @"v1_KeKhai_DotKeKhai_Insert";    // Insert Đợt kê khai
        private const string INSERT_DOT_KE_KHAI_CAN_BO_COQUAN = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_Insert";
        private const string CHECK_TRANG_THAI = @"v1_DotKeKhai_UpDateTrangThai";  //Check trạng thái đợt kê khai
        private const string UPDATE_DOT_KE_KHAI_CAN_BO_COQUAN = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_Update";
        private const string DOT_KE_KHAI_GETALL = @"v1_KeKhai_DotKeKhai_GetAll";
        private const string DOT_KE_KHAI_GET_BY_NAMKEKHAI = @"v1_KeKhai_DotKeKhai_GetListByNamKeKhai";
        private const string DOT_KE_KHAI_GET_BY_NAMKEKHAI_1 = @"v1_KeKhai_DotKeKhai_GetByNamKeKhai";
        private const string DOT_KE_KHAI_GET_BY_COQUANID = @"v1_KeKhai_DotKeKhai_GetByCoQuanID";
        private const string DOT_KE_KHAI_GET_BY_CANBOID = @"v1_KeKhai_DotKeKhai_GetByCanBoID";
        private const string DOT_KE_KHAI_GET_LAST_BY_CANBOID = @"v1_KeKhai_DotKeKhai_GetLast_ByCanBoID";
        private const string DOT_KE_KHAI_GET_BY_CAPCOQUAN = @"v1_DotKeKhai_GetDotKeKhaiByCapCoQuan";
        private const string DOT_KE_KHAI_GET_BY_LISTCANBOID = @"v1_KeKhai_DotKeKhai_GetList_ByListCanBoID";
        private const string DELETE_DOT_KE_KHAI = @"v1_KeKhai_DotKeKhai_Delete";
        private const string DOT_KE_KHAI_GET_BY_ID = @"v1_DotKeKhai_GetByID";
        private const string KEKHAI_DOT_KE_KHAI_GET_BY_ID = @"v1_KeKhai_DotKeKhai_GetByID";
        private const string UPDATE_NEW_DOTKEKHAI = @"v1_KeKhai_DotKeKhai_Update";
        private const string KE_KHAI_DOT_KE_KHAI_GETPAGING = @"v1_KeKhai_DotKeKhai_GetPagingBySearch";
        private const string KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_DOTKEKHAIID = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_Get_By_DotKeKhaiID";
        private const string KEKHAI_DOTKEKHAICOQUANCANBO_GET_DANHSACHCANBO_BY_DANHSACHDOTKEKHAI = @"v1_KeKhai_DotKeKhai_GetDanhSachCanBo_By_DanhSachDotKeKhai";
        private const string KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_ApDungChoAndLoaiDotKeKhai = @"v1_GetDotKeKhaiByApDungChoAndLoaiDotKeKhai";
        private const string KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_COQUANID = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_Get_By_DotCoQuanID";
        private const string KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_COQUANID_AND_NAMKEKHAI = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_Get_By_DotCoQuanIDAndNamKeKhai";
        private const string DELETE_KEKHAI_DOTKEKHAICOQUANCANBO = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_Delete";
        private const string INSERT_KEKHAI_DOTKEKHAICOQUANCANBO = @"v1_KeKhai_InsertCanBoDotKeKhaiCoQuan";
        private const string INSERT_LIST_KEKHAI_DOTKEKHAICOQUANCANBO = @"v1_KeKhai_DotKeKhaiCoQuanCanBo_InsertList";
        private const string KEKHAI_DOTKEKHAI_GETALLACTIVE = @"v1_KeKhai_GetAllDotKeKhaiActive";
        private const string KEKHAI_DOTKEKHAI_GETALL_CANBODOTKEKHAI_ACTIVE = @"v1_KeKhai_GetAllCanBoDotKeKhaiActive";


        //Ten các params
        private const string DOT_KE_KHAI_ID = "NV00101";
        private const string TU_NGAY = "NV00102";
        private const string DEN_NGAY = "NV00103";
        private const string TRANG_THAI = "NV00104";
        private const string NAM_KE_KHAI = "NV00105";
        private const string LOAI_DOT_KE_KHAI = "NV00106";
        private const string AP_DUNG_CHO = "NV00107";
        private const string TEN_DOT_KE_KHAI = "NV00108";
        private const string MO_TA_DOT_KE_KHAI = "NV00109";
        private const string CAP_QUAN_LY = "NV00110";
        private const string CO_QUAN_TAO = "NV00111";
        private const string DOT_KE_KHAI_ID_CANBO_DONVI = "NV00202";
        private const string CO_QUAN_ID = "NV00203";
        private const string CAN_BO_ID = "NV00204";

        public DotKeKhaiPartial GetData(SqlDataReader dr)
        {
            DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
            DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
            DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
            DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
            DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
            DotKeKhai.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], false);
            DotKeKhai.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
            DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
            return DotKeKhai;
        }

        //Check trạng thái đợt kê khai
        public void CheckTrangThai()
        {
            try
            {
                //string DateTimeNow = DateTime.Now.ToString("yyyyMMdd");
                List<DotKeKhaiPartial> ListDotKeKhai = GetDotKeKhaiActive();
                foreach (var item in ListDotKeKhai)
                {
                    var DenNgay = Utils.ConvertToDateTime(item.DenNgay, DateTime.Now.Date);
                    if (DateTime.Now.Date > DenNgay.Date)
                    {
                        bool trangthai = false;
                        SqlParameter[] parameter = new SqlParameter[]
                        {
                            new SqlParameter(TRANG_THAI,SqlDbType.Bit),
                            new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int)
                        };
                        parameter[0].Value = trangthai;
                        parameter[1].Value = item.DotKeKhaiID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var query = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, CHECK_TRANG_THAI, parameter);
                                    trans.Commit();
                                }
                                catch
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Insert Đợt kê khai
        public BaseResultModel Insert(DotKeKhaiPartial DotKeKhaiPartial)
        {

            var Result = new BaseResultModel();
            if (DotKeKhaiPartial.TuNgay == null || DotKeKhaiPartial.TuNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.DenNgay == null || DotKeKhaiPartial.DenNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.TuNgay > DotKeKhaiPartial.DenNgay)
            {
                Result.Status = 0;
                Result.Message = "Thời gian kê khai không hợp lệ";
                return Result;
            }

            if (DotKeKhaiPartial.DanhSachCanBo == null || DotKeKhaiPartial.DanhSachCanBo.Count < 1)// trường hợp 2
            {
                if (DotKeKhaiPartial.DanhSachCoQuan == null || DotKeKhaiPartial.DanhSachCoQuan.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Danh sách cơ quan không được trống";
                    return Result;
                }
                else
                {
                    for (int i = 0; i < DotKeKhaiPartial.DanhSachCoQuan.Count; i++)
                    {
                        DotKeKhaiPartial result = GetDotKeKhaiByCoQuanID(DotKeKhaiPartial.DanhSachCoQuan[i]);
                        if (result == null || result.DotKeKhaiID < 1 || DotKeKhaiPartial.TuNgay > result.DenNgay)
                        {
                            //đúng
                        }
                        else
                        {
                            Result.Status = 0;
                            Result.Message = "Cơ quan hoặc cán bộ đã có đợt kê khai";
                            return Result;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                {
                    var crCanBo = new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]);
                    if (crCanBo == null || crCanBo.CanBoID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Không có dữ liệu cán bộ";
                        return Result;
                    }
                    else
                    {
                        int CoQuanID = Utils.ConvertToInt32(crCanBo.CoQuanID, 0);
                        DotKeKhaiPartial result = GetDotKeKhaiByCoQuanID(CoQuanID);
                        if (result == null || result.DotKeKhaiID < 1 || DotKeKhaiPartial.TuNgay > result.DenNgay)
                        {
                            //đúng
                        }
                        else
                        {
                            Result.Status = 0;
                            Result.Message = "Cơ quan hoặc cán bộ đã có đợt kê khai";
                            return Result;
                        }
                    }
                }
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                            new SqlParameter(TU_NGAY, SqlDbType.DateTime),
                            new SqlParameter(DEN_NGAY, SqlDbType.DateTime),
                            new SqlParameter(TRANG_THAI, SqlDbType.Bit),
                            new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
              };
            parameters[0].Value = DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd");
            parameters[1].Value = DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd");
            parameters[2].Value = DotKeKhaiPartial.TrangThai ?? Convert.DBNull;
            parameters[3].Value = DotKeKhaiPartial.TuNgay.Year;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {

                        var query = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, INSERT_DOT_KE_KHAI, parameters);
                        if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)// trường hợp 2
                        {
                            for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                            {
                                SqlParameter[] parameters1 = new SqlParameter[]
                                {
                                    new SqlParameter(DOT_KE_KHAI_ID_CANBO_DONVI, SqlDbType.Int),
                                    new SqlParameter(CAN_BO_ID, SqlDbType.Int),
                                    new SqlParameter(CO_QUAN_ID, SqlDbType.Int)
                                };
                                var crCanBo = new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]);
                                parameters1[0].Value = Utils.ConvertToInt32(query, 0);
                                parameters1[1].Value = crCanBo.CanBoID;
                                parameters1[2].Value = Convert.DBNull;
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, INSERT_DOT_KE_KHAI_CAN_BO_COQUAN, parameters1);
                            }
                        }
                        else //trường hợp 1
                        {
                            for (int i = 0; i < DotKeKhaiPartial.DanhSachCoQuan.Count; i++)
                            {
                                SqlParameter[] parameters1 = new SqlParameter[]
                                {
                                    new SqlParameter(DOT_KE_KHAI_ID_CANBO_DONVI, SqlDbType.Int),
                                    new SqlParameter(CAN_BO_ID, SqlDbType.Int),
                                    new SqlParameter(CO_QUAN_ID, SqlDbType.Int)
                                };
                                parameters1[0].Value = Utils.ConvertToInt32(query, 0);
                                parameters1[1].Value = Convert.DBNull;
                                parameters1[2].Value = DotKeKhaiPartial.DanhSachCoQuan[i];
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, INSERT_DOT_KE_KHAI_CAN_BO_COQUAN, parameters1);
                            }
                        }
                        Result.Message = "Thêm đợt kê khai thành công";
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Result.Status = 0;
                        Result.Message = ex.ToString();
                        trans.Rollback();
                        throw ex;
                    }
                    return Result;
                }
            }
        }

        //GetAll
        public List<DotKeKhaiModel> GetAll()
        {
            List<DotKeKhaiModel> List = new List<DotKeKhaiModel>();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GETALL))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiModel DotKeKhai = new DotKeKhaiModel();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        DotKeKhai.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], true);

                        List.Add(DotKeKhai);
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

        /// <summary>
        /// lấy danh sách đợt kê khai đang thực hiện trang NamKeKhai
        /// </summary>
        /// <param name="NamKeKhai"></param>
        /// <returns></returns>
        public List<DotKeKhaiPartial> GetListByNamKeKhai(int NamKeKhai)
        {
            //CheckTrangThai();
            List<DotKeKhaiPartial> Result = new List<DotKeKhaiPartial>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int)
              };
            parameters[0].Value = NamKeKhai;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_NAMKEKHAI, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = GetData(dr);
                        Result.Add(DotKeKhai);
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

        // Get đợi kê khai by năm kê khai
        public List<DotKeKhaiPartial> GetByNamKeKhai(int NamKeKhai)
        {
            //CheckTrangThai();
            List<DotKeKhaiPartial> Result = new List<DotKeKhaiPartial>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int)
              };
            parameters[0].Value = NamKeKhai;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_NAMKEKHAI_1, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = GetData(dr);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], String.Empty);
                        Result.Add(DotKeKhai);
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

        public List<DotKeKhaiPartial> GetByCanBoID_NamKeKhai(int NamKeKhai, int? CanBoID)
        {
            //CheckTrangThai();
            List<DotKeKhaiPartial> Result = new List<DotKeKhaiPartial>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,SqlDbType.Int),
              };
            parameters[0].Value = NamKeKhai;
            parameters[1].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_KeKhai_DotKeKhai_GetByCanBoID_NamKeKhai", parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = GetData(dr);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], String.Empty);
                        Result.Add(DotKeKhai);
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

        // get đợt kê khai by CoQuanID
        public DotKeKhaiPartial GetDotKeKhaiByCoQuanID(int CoQuanID)
        {
            //CheckTrangThai();
            DotKeKhaiPartial Result = new DotKeKhaiPartial();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(CO_QUAN_ID,SqlDbType.Int)
             };
            parameters[0].Value = CoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_COQUANID, parameters))
                {
                    while (dr.Read())
                    {
                        Result.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID_CANBO_DONVI], 0);
                        Result.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        Result.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        Result.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        Result.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        Result.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        Result.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], true);
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

        // get đợt kê khai by CanBoID
        public List<DotKeKhaiPartial> GetDotKeKhaiByCanBoID(int CanBoID)
        {
            //CheckTrangThai();
            List<DotKeKhaiPartial> Result = new List<DotKeKhaiPartial>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(CAN_BO_ID,SqlDbType.Int)
             };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var item = GetData(dr);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        item.MoTaDotKeKhai = Utils.ConvertToString(dr[MO_TA_DOT_KE_KHAI], string.Empty);
                        Result.Add(item);
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

        // Get đợt kê khai cuối cùng by CanBoID
        public DotKeKhaiPartial GetLast_By_CanBoID(int? CanBoID)
        {
            DotKeKhaiPartial Result = new DotKeKhaiPartial();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter(CAN_BO_ID,SqlDbType.Int)
             };
            parameters[0].Value = CanBoID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_LAST_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        Result.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        Result.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        Result.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        Result.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        Result.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        Result.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        Result.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], false);
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

        // Lấy danh sách đợt kê khai theo cấp cơ quan
        public List<DotKeKhaiPartial> GetDotKeKhai_By_CapCoQuan(int? CapID, int? NamKeKhai)
        {
            List<DotKeKhaiPartial> Result = new List<DotKeKhaiPartial>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@CapID",SqlDbType.Int),
                        new SqlParameter(NAM_KE_KHAI,SqlDbType.Int)
             };
            parameters[0].Value = CapID ?? Convert.DBNull;
            parameters[1].Value = NamKeKhai ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_CAPCOQUAN, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhaiPartial = new DotKeKhaiPartial();
                        DotKeKhaiPartial.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID_CANBO_DONVI], 0);
                        DotKeKhaiPartial.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        Result.Add(DotKeKhaiPartial);
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

        public List<DotKeKhaiModel> GetList_ByListCanBoID(List<int> DanhSachCanBo, int? NamKeKhai)
        {
            List<DotKeKhaiModel> Result = new List<DotKeKhaiModel>();
            var table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            DanhSachCanBo.ForEach(x => table.Rows.Add(x));
            var ListCanBo = new SqlParameter("@DanhSachCanBo", SqlDbType.Structured);
            ListCanBo.TypeName = "dbo.list_ID";
            SqlParameter[] parameters = new SqlParameter[]
             {
                 ListCanBo,
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int)
             };
            parameters[0].Value = table;
            parameters[1].Value = NamKeKhai ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_LISTCANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var item = new DotKeKhaiModel();
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        item.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        item.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], true);
                        item.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        item.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        item.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        item.MoTaDotKeKhai = Utils.ConvertToString(dr[MO_TA_DOT_KE_KHAI], string.Empty);
                        if (!Result.Select(x => x.DotKeKhaiID).ToList().Contains(item.DotKeKhaiID))
                        {
                            Result.Add(item);
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

        #region Create by AnhVH 07.12.2019

        public BaseResultModel Insert_New(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID)
        {
            var Result = new BaseResultModel();
            if (DotKeKhaiPartial.LoaiDotKeKhai != EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
            {
                var DotKeKhaiByApDungChoAndLoaiDotAll = GetDotKeKhaiByApDungChoAndLoaiDotKeKhai(DotKeKhaiPartial.ApDungCho.Value, DotKeKhaiPartial.LoaiDotKeKhai);
                var DotKeKhaiByApDungChoAndLoaiDot = new List<DotKeKhaiPartial>();
                for (int i = 0; i < DotKeKhaiByApDungChoAndLoaiDotAll.Count; i++)
                {
                    var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(DotKeKhaiByApDungChoAndLoaiDotAll[i].CoQuanTao.Value).Select(x => x.CoQuanID).ToList();
                    if (listCoQuanCon.Contains(CoQuanID))
                    {
                        DotKeKhaiByApDungChoAndLoaiDot.Add(DotKeKhaiByApDungChoAndLoaiDotAll[i]);
                    }
                }
                if (DotKeKhaiByApDungChoAndLoaiDot != null && DotKeKhaiByApDungChoAndLoaiDot.Count > 0
                    && DotKeKhaiPartial.TuNgay.Date < DotKeKhaiByApDungChoAndLoaiDot.OrderByDescending(x => x.TuNgay).FirstOrDefault().DenNgay.Date
                    && DotKeKhaiByApDungChoAndLoaiDot.Count > 0
                    && DotKeKhaiByApDungChoAndLoaiDot.Select(x => x.CoQuanTao).ToList().Contains(CoQuanID)
                    )
                {
                    Result.Status = 0;
                    Result.Message = "Đã có đợt kê khai này trên hệ thống!";
                    return Result;
                }
            }
            var DanhSachCanBo = DotKeKhaiPartial.DanhSachCanBo ?? new List<int>();
            if (DotKeKhaiPartial.TuNgay == null || DotKeKhaiPartial.TuNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.DenNgay == null || DotKeKhaiPartial.DenNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.TuNgay > DotKeKhaiPartial.DenNgay)
            {
                Result.Status = 0;
                Result.Message = "Thời gian kê khai không hợp lệ";
                return Result;
            }
            if (DotKeKhaiPartial.TenDotKeKhai == null || DotKeKhaiPartial.TenDotKeKhai.Length < 1 || DotKeKhaiPartial.TenDotKeKhai.Length > 1000)
            {
                Result.Status = 0;
                Result.Message = "Tên đợt kê khai không được trống và độ dài không quá 1000 ký tự";
                return Result;
            }

            var listDotKeKhaiTrongNam = GetListByNamKeKhai(DotKeKhaiPartial.TuNgay.Year);// trang thai=1 - dang con hieu luc
            var listDotKeKhaiDangThucHien = listDotKeKhaiTrongNam.Where(x =>
                   (
                        (x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.DenNgay <= DotKeKhaiPartial.DenNgay.Date)
                        || (x.DenNgay >= DotKeKhaiPartial.DenNgay.Date && x.TuNgay <= DotKeKhaiPartial.DenNgay.Date)
                        || (x.TuNgay <= DotKeKhaiPartial.TuNgay.Date && x.DenNgay >= DotKeKhaiPartial.DenNgay.Date)
                   ) &&
                   (
                        (DotKeKhaiPartial.DenNgay >= x.TuNgay.Date && DotKeKhaiPartial.DenNgay <= x.DenNgay.Date)
                        || (DotKeKhaiPartial.DenNgay >= x.DenNgay.Date && DotKeKhaiPartial.TuNgay <= x.DenNgay.Date)
                        || (DotKeKhaiPartial.TuNgay <= x.TuNgay.Date && DotKeKhaiPartial.DenNgay >= x.DenNgay.Date)
                   )
                   && x.DotKeKhaiID != DotKeKhaiPartial.DotKeKhaiID
                   ).ToList();


            // lấy danh sách cán bộ đang thực hiện kê khai trong khoảng thời gian của đợt đang sửa
            var danhSachCanBoDaCoDotKeKhai = GetDanhSachCanBo_By_DanhSachDotKeKhai(listDotKeKhaiDangThucHien.Select(x => x.DotKeKhaiID).ToList()).Select(x => x.CanBoID.Value).ToList();
            //check thanh tra tinh
            var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);

            var DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(((UserRole.CheckAdmin(NguoiDungID) == true) || laThanhTraTinh) ? 0 : CoQuanID).Select(x => x.CoQuanID).ToList();
            var listCanBo_HopLe = new HeThongCanBoDAL().CanBoChucVu_GetAll().Where(x =>
                x.TrangThaiID == EnumTrangThaiCanBo.DangLamViec.GetHashCode()
               && DanhSachCoQuanCon.Contains(x.CoQuanID)
                && !danhSachCanBoDaCoDotKeKhai.Contains(x.CanBoID)
                );
            if (DotKeKhaiPartial.LoaiDotKeKhai == null || DotKeKhaiPartial.LoaiDotKeKhai < 1)
            {
                Result.Status = 0;
                Result.Message = "Loại đợt kê khai không được trống";
                return Result;
            }
            if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()) // hàng năm
            {
                var listDotKeKhaiHangNam = listDotKeKhaiTrongNam.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.TrangThai == true && x.CoQuanTao == CoQuanID).ToList();
                if (listDotKeKhaiHangNam.Count > 0 && listDotKeKhaiHangNam.Any(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.ApDungCho == DotKeKhaiPartial.ApDungCho))
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai đã có trên hệ thống. Vui lòng thử lại";
                    return Result;
                }
                // lấy danh sách cán bộ được kê khai đợt này
                // cán bộ có chức vụ phải kê khai hàng năm và thuộc diện quản lý == DotKeKhaiPartial.ApDungCho

                DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x => x.KeKhaiHangNam == true
                    && (DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                    ).Select(x => x.CanBoID));
                if (DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                    return Result;
                }
            }
            else if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()) // bỏ sung
            {
                var listDotKeKhaiHangNam = listDotKeKhaiTrongNam.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanTao == CoQuanID).ToList();
                if (listDotKeKhaiHangNam.Count > 0 && listDotKeKhaiHangNam.Any(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && (x.ApDungCho == DotKeKhaiPartial.ApDungCho || x.ApDungCho == 3 || DotKeKhaiPartial.ApDungCho == 3)))
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai trong năm đã được tạo";
                    return Result;
                }
                // lấy danh sách cán bộ được kê khai đợt này
                DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x =>
                    DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy
                     ).Select(x => x.CanBoID));
                if (DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                    return Result;
                }
            }
            else if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()) // bổ nhiệm
            {
                var listDotKeKhaiBoNhiem = GetByNamKeKhai(DotKeKhaiPartial.TuNgay.Year).Where(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.TrangThai == true).ToList();
                if (listDotKeKhaiBoNhiem.Count > 0)
                {
                    for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                    {
                        if (listDotKeKhaiBoNhiem.Any(x => x.CanBoID.Value == DotKeKhaiPartial.DanhSachCanBo[i]))
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ " + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + " đang thực hiện đợt kê khai khác";
                            return Result;
                        }
                    }

                }
                // lấy danh sách cán bộ được kê khai đợt này
                DanhSachCanBo = DotKeKhaiPartial.DanhSachCanBo.Where(x => !danhSachCanBoDaCoDotKeKhai.Contains(x)).ToList();
            }
            else // lần đầu
            {
                // lấy danh sách cán bộ được kê khai đợt này - tất cả cán bộ chưa có bản kê khai nào
                var listCanBoDaCoBanKeKhai = new KeKhaiDAL().GetAll().Select(x => x.CanBoID);
                DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x =>
                    (DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                    && !listCanBoDaCoBanKeKhai.Contains(x.CanBoID)
                    //  && !danhSachCanBoDaCoDotKeKhai.Contains(x.CanBoID)
                    ).Select(x => x.CanBoID));
                if (DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Cán bộ đang thực hiện đợt kê khai khác";
                    return Result;
                }

            }

            DanhSachCanBo = DanhSachCanBo.Distinct().ToList();
            if (DanhSachCanBo == null || DanhSachCanBo.Count < 1)
            {
                Result.Status = 0;
                Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                return Result;
            }
            var table = new DataTable();
            table.Columns.Add("ID1", typeof(string));
            table.Columns.Add("ID2", typeof(string));

            DanhSachCanBo.ForEach(x => table.Rows.Add(new HeThongCanBoDAL().GetCanBoByID(x).CoQuanID.Value, x));
            var ListCanBo = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
            ListCanBo.TypeName = "dbo.id_id_list";

            SqlParameter[] parameters = new SqlParameter[]
              {
                            new SqlParameter(TU_NGAY, SqlDbType.DateTime),
                            new SqlParameter(DEN_NGAY, SqlDbType.DateTime),
                            new SqlParameter(TRANG_THAI, SqlDbType.Bit),
                            new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                            new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                            new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                            ListCanBo,
                            new SqlParameter(TEN_DOT_KE_KHAI,SqlDbType.NVarChar),
                            new SqlParameter(MO_TA_DOT_KE_KHAI,SqlDbType.NVarChar),
                            new SqlParameter(CAP_QUAN_LY,SqlDbType.Int),
                             new SqlParameter(CO_QUAN_TAO,SqlDbType.Int)
              };
            parameters[0].Value = DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd");
            parameters[1].Value = DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd");
            parameters[2].Value = 1;
            parameters[3].Value = DotKeKhaiPartial.TuNgay.Year;
            parameters[4].Value = DotKeKhaiPartial.LoaiDotKeKhai;
            parameters[5].Value = DotKeKhaiPartial.ApDungCho ?? Convert.DBNull;
            parameters[6].Value = table;
            parameters[7].Value = DotKeKhaiPartial.TenDotKeKhai;

            var MoTaDotKeKhai = "Áp dụng cho";
            if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
            {
                MoTaDotKeKhai = MoTaDotKeKhai + " một số cán bộ (";

                for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                {
                    MoTaDotKeKhai = MoTaDotKeKhai + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + ", ";
                }
                MoTaDotKeKhai = MoTaDotKeKhai.Trim().Substring(0, MoTaDotKeKhai.LastIndexOf(','));
                MoTaDotKeKhai = MoTaDotKeKhai + ")";
            }
            else
            {
                if (DotKeKhaiPartial.ApDungCho == 1)
                {
                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp tỉnh quản lý";
                }
                else if (DotKeKhaiPartial.ApDungCho == 2)
                {
                    var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);

                    if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) // người tạo thuộc tỉnh hoặc sở
                    {
                        MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn tỉnh";
                    }
                    else // người tạo thuộc huyện hoặc phòng
                    {
                        if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuan;
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuanCha;
                        }
                        else
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý";
                        }
                    }
                }
                else if (DotKeKhaiPartial.ApDungCho == 3)
                {
                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ trên toàn tỉnh";
                }
            }
            MoTaDotKeKhai = MoTaDotKeKhai.Trim();
            parameters[8].Value = MoTaDotKeKhai;
            parameters[9].Value = CapQuanLy ?? Convert.DBNull;
            if (CoQuanID == 0) parameters[10].Value = Convert.DBNull;
            else parameters[10].Value = CoQuanID;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        var query = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, INSERT_DOT_KE_KHAI, parameters);
                        //thêm thông báo
                        ThongBaoModel ThongBaoModel = new ThongBaoModel();
                        ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                        ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + DotKeKhaiPartial.TuNgay.ToString("dd/MM/yyyy")
                            + " đến ngày " + DotKeKhaiPartial.DenNgay.ToString("dd/MM/yyyy")
                            + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                        ThongBaoModel.ThoiGianBatDau = DotKeKhaiPartial.TuNgay;
                        ThongBaoModel.ThoiGianKetThuc = DotKeKhaiPartial.DenNgay;
                        ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                        ThongBaoModel.NghiepVuID = Utils.ConvertToInt32(query, 0);
                        ThongBaoModel.TenNghiepVu = DotKeKhaiPartial.TenDotKeKhai;
                        ThongBaoModel.HienThi = true;
                        ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                        if (DanhSachCanBo != null && DanhSachCanBo.Count > 0)
                        {
                            foreach (var cb in DanhSachCanBo)
                            {
                                DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                                int ID = Utils.ConvertToInt32(cb, 0);
                                var CanBo = new HeThongCanBoDAL().GetCanBoByID(ID);
                                dt.CanBoID = ID;
                                dt.CoQuanID = CanBo.CoQuanID;
                                dt.TenCanBo = CanBo.TenCanBo;
                                dt.GioiTinh = CanBo.GioiTinh;
                                dt.Email = CanBo.Email;
                                ThongBaoModel.DoiTuongThongBao.Add(dt);
                            }
                        }
                        new NhacViecDAL().Edit_ThongBao(ThongBaoModel);

                        trans.Commit();
                        Result.Status = 1;
                        Result.Message = "Thêm đợt kê khai thành công";
                    }
                    catch (Exception ex)
                    {
                        Result.Status = -1;
                        Result.Message = ex.ToString();
                        trans.Rollback();
                        throw ex;
                    }
                    return Result;
                }
            }
        }

        public BaseResultModel Insert_New_v2(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID)
        {
            var Result = new BaseResultModel();
            if (DotKeKhaiPartial.LoaiDotKeKhai != EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
            {
                var DotKeKhaiByApDungChoAndLoaiDotAll = GetDotKeKhaiByApDungChoAndLoaiDotKeKhai(DotKeKhaiPartial.ApDungCho.Value, DotKeKhaiPartial.LoaiDotKeKhai);
                var DotKeKhaiByApDungChoAndLoaiDot = new List<DotKeKhaiPartial>();
                for (int i = 0; i < DotKeKhaiByApDungChoAndLoaiDotAll.Count; i++)
                {
                    var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(DotKeKhaiByApDungChoAndLoaiDotAll[i].CoQuanTao.Value).Select(x => x.CoQuanID).ToList();
                    if (listCoQuanCon.Contains(CoQuanID))
                    {
                        DotKeKhaiByApDungChoAndLoaiDot.Add(DotKeKhaiByApDungChoAndLoaiDotAll[i]);
                    }
                }
                if (DotKeKhaiByApDungChoAndLoaiDot != null && DotKeKhaiByApDungChoAndLoaiDot.Count > 0
                    && DotKeKhaiPartial.TuNgay.Date < DotKeKhaiByApDungChoAndLoaiDot.OrderByDescending(x => x.TuNgay).FirstOrDefault().DenNgay.Date
                    && DotKeKhaiByApDungChoAndLoaiDot.Count > 0
                    && DotKeKhaiByApDungChoAndLoaiDot.Select(x => x.CoQuanTao).ToList().Contains(CoQuanID)
                    )
                {
                    Result.Status = 0;
                    Result.Message = "Đã có đợt kê khai này trên hệ thống!";
                    return Result;
                }
            }
            var DanhSachCanBo = DotKeKhaiPartial.DanhSachCanBo ?? new List<int>();
            if (DotKeKhaiPartial.TuNgay == null || DotKeKhaiPartial.TuNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.DenNgay == null || DotKeKhaiPartial.DenNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.TuNgay > DotKeKhaiPartial.DenNgay)
            {
                Result.Status = 0;
                Result.Message = "Thời gian kê khai không hợp lệ";
                return Result;
            }
            if (DotKeKhaiPartial.TenDotKeKhai == null || DotKeKhaiPartial.TenDotKeKhai.Length < 1 || DotKeKhaiPartial.TenDotKeKhai.Length > 1000)
            {
                Result.Status = 0;
                Result.Message = "Tên đợt kê khai không được trống và độ dài không quá 1000 ký tự";
                return Result;
            }

            var listDotKeKhaiTrongNam = GetListByNamKeKhai(DotKeKhaiPartial.TuNgay.Year);// trang thai=1 - dang con hieu luc
            var listDotKeKhaiDangThucHien = listDotKeKhaiTrongNam.Where(x =>
                   (
                        (x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.DenNgay <= DotKeKhaiPartial.DenNgay.Date)
                        || (x.DenNgay >= DotKeKhaiPartial.DenNgay.Date && x.TuNgay <= DotKeKhaiPartial.DenNgay.Date)
                        || (x.TuNgay <= DotKeKhaiPartial.TuNgay.Date && x.DenNgay >= DotKeKhaiPartial.DenNgay.Date)
                   ) &&
                   (
                        (DotKeKhaiPartial.DenNgay >= x.TuNgay.Date && DotKeKhaiPartial.DenNgay <= x.DenNgay.Date)
                        || (DotKeKhaiPartial.DenNgay >= x.DenNgay.Date && DotKeKhaiPartial.TuNgay <= x.DenNgay.Date)
                        || (DotKeKhaiPartial.TuNgay <= x.TuNgay.Date && DotKeKhaiPartial.DenNgay >= x.DenNgay.Date)
                   )
                   && x.DotKeKhaiID != DotKeKhaiPartial.DotKeKhaiID
                   ).ToList();


            // lấy danh sách cán bộ đang thực hiện kê khai trong khoảng thời gian của đợt đang sửa
            var danhSachCanBoDaCoDotKeKhai = GetDanhSachCanBo_By_DanhSachDotKeKhai(listDotKeKhaiDangThucHien.Select(x => x.DotKeKhaiID).ToList()).Select(x => x.CanBoID.Value).ToList();
            //check thanh tra tinh
            var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);

            var DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(((UserRole.CheckAdmin(NguoiDungID) == true) || laThanhTraTinh) ? 0 : CoQuanID).Select(x => x.CoQuanID).ToList();
            var listCanBo_HopLe = new HeThongCanBoDAL().CanBoChucVu_GetAll().Where(x =>
                x.TrangThaiID == EnumTrangThaiCanBo.DangLamViec.GetHashCode()
               && DanhSachCoQuanCon.Contains(x.CoQuanID)
                && !danhSachCanBoDaCoDotKeKhai.Contains(x.CanBoID)
                );
            if (DotKeKhaiPartial.LoaiDotKeKhai == null || DotKeKhaiPartial.LoaiDotKeKhai < 1)
            {
                Result.Status = 0;
                Result.Message = "Loại đợt kê khai không được trống";
                return Result;
            }
            if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()) // hàng năm
            {
                var listDotKeKhaiHangNam = listDotKeKhaiTrongNam.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.TrangThai == true && x.CoQuanTao == CoQuanID).ToList();
                if (listDotKeKhaiHangNam.Count > 0 && listDotKeKhaiHangNam.Any(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date))
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai đã có trên hệ thống. Vui lòng thử lại";
                    return Result;
                }
                // lấy danh sách cán bộ được kê khai đợt này
                // cán bộ có chức vụ phải kê khai hàng năm và thuộc diện quản lý == DotKeKhaiPartial.ApDungCho

                DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x => x.KeKhaiHangNam == true).Select(x => x.CanBoID));
                if (DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                    return Result;
                }
            }
            else if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()) // bỏ sung
            {
                var listDotKeKhaiHangNam = listDotKeKhaiTrongNam.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanTao == CoQuanID).ToList();
                if (listDotKeKhaiHangNam.Count > 0 && listDotKeKhaiHangNam.Any(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date))
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai trong năm đã được tạo";
                    return Result;
                }
                // lấy danh sách cán bộ được kê khai đợt này
                DanhSachCanBo.AddRange(listCanBo_HopLe.Select(x => x.CanBoID));
                if (DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                    return Result;
                }
            }
            else if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()) // bổ nhiệm
            {
                var listDotKeKhaiBoNhiem = GetByNamKeKhai(DotKeKhaiPartial.TuNgay.Year).Where(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.TrangThai == true).ToList();
                if (listDotKeKhaiBoNhiem.Count > 0)
                {
                    for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                    {
                        if (listDotKeKhaiBoNhiem.Any(x => x.CanBoID.Value == DotKeKhaiPartial.DanhSachCanBo[i]))
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ " + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + " đang thực hiện đợt kê khai khác";
                            return Result;
                        }
                    }

                }
                // lấy danh sách cán bộ được kê khai đợt này
                DanhSachCanBo = DotKeKhaiPartial.DanhSachCanBo.Where(x => !danhSachCanBoDaCoDotKeKhai.Contains(x)).ToList();
            }
            else // lần đầu
            {
                // lấy danh sách cán bộ được kê khai đợt này - tất cả cán bộ chưa có bản kê khai nào
                var listCanBoDaCoBanKeKhai = new KeKhaiDAL().GetAll().Select(x => x.CanBoID);
                DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x =>
                    //(DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                    //&& 
                    !listCanBoDaCoBanKeKhai.Contains(x.CanBoID)
                    //  && !danhSachCanBoDaCoDotKeKhai.Contains(x.CanBoID)
                    ).Select(x => x.CanBoID));
                if (DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Cán bộ đang thực hiện đợt kê khai khác";
                    return Result;
                }

            }

            DanhSachCanBo = DanhSachCanBo.Distinct().ToList();
            if (DanhSachCanBo == null || DanhSachCanBo.Count < 1)
            {
                Result.Status = 0;
                Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                return Result;
            }
            var table = new DataTable();
            table.Columns.Add("ID1", typeof(string));
            table.Columns.Add("ID2", typeof(string));

            DanhSachCanBo.ForEach(x => table.Rows.Add(new HeThongCanBoDAL().GetCanBoByID(x).CoQuanID.Value, x));
            var ListCanBo = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
            ListCanBo.TypeName = "dbo.id_id_list";

            SqlParameter[] parameters = new SqlParameter[]
              {
                            new SqlParameter(TU_NGAY, SqlDbType.DateTime),
                            new SqlParameter(DEN_NGAY, SqlDbType.DateTime),
                            new SqlParameter(TRANG_THAI, SqlDbType.Bit),
                            new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                            new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                            new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                            ListCanBo,
                            new SqlParameter(TEN_DOT_KE_KHAI,SqlDbType.NVarChar),
                            new SqlParameter(MO_TA_DOT_KE_KHAI,SqlDbType.NVarChar),
                            new SqlParameter(CAP_QUAN_LY,SqlDbType.Int),
                             new SqlParameter(CO_QUAN_TAO,SqlDbType.Int)
              };
            parameters[0].Value = DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd");
            parameters[1].Value = DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd");
            parameters[2].Value = 1;
            parameters[3].Value = DotKeKhaiPartial.TuNgay.Year;
            parameters[4].Value = DotKeKhaiPartial.LoaiDotKeKhai;
            parameters[5].Value = DotKeKhaiPartial.ApDungCho ?? Convert.DBNull;
            parameters[6].Value = table;
            parameters[7].Value = DotKeKhaiPartial.TenDotKeKhai;

            var MoTaDotKeKhai = "Áp dụng cho";
            if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
            {
                MoTaDotKeKhai = MoTaDotKeKhai + " một số cán bộ (";

                for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                {
                    MoTaDotKeKhai = MoTaDotKeKhai + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + ", ";
                }
                MoTaDotKeKhai = MoTaDotKeKhai.Trim().Substring(0, MoTaDotKeKhai.LastIndexOf(','));
                MoTaDotKeKhai = MoTaDotKeKhai + ")";
            }
            else
            {
                MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ trên toàn tỉnh";

                //if (DotKeKhaiPartial.ApDungCho == 1)
                //{
                //    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp tỉnh quản lý";
                //}
                //else if (DotKeKhaiPartial.ApDungCho == 2)
                //{
                //    var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);

                //    if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) // người tạo thuộc tỉnh hoặc sở
                //    {
                //        MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn tỉnh";
                //    }
                //    else // người tạo thuộc huyện hoặc phòng
                //    {
                //        if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                //        {
                //            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuan;
                //        }
                //        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())
                //        {
                //            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuanCha;
                //        }
                //        else
                //        {
                //            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý";
                //        }
                //    }
                //}
                //else if (DotKeKhaiPartial.ApDungCho == 3)
                //{
                //    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ trên toàn tỉnh";
                //}
            }
            MoTaDotKeKhai = MoTaDotKeKhai.Trim();
            parameters[8].Value = MoTaDotKeKhai;
            parameters[9].Value = CapQuanLy ?? Convert.DBNull;
            if (CoQuanID == 0) parameters[10].Value = Convert.DBNull;
            else parameters[10].Value = CoQuanID;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        var query = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, INSERT_DOT_KE_KHAI, parameters);
                        //thêm thông báo
                        ThongBaoModel ThongBaoModel = new ThongBaoModel();
                        ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                        ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + DotKeKhaiPartial.TuNgay.ToString("dd/MM/yyyy")
                            + " đến ngày " + DotKeKhaiPartial.DenNgay.ToString("dd/MM/yyyy")
                            + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                        ThongBaoModel.ThoiGianBatDau = DotKeKhaiPartial.TuNgay;
                        ThongBaoModel.ThoiGianKetThuc = DotKeKhaiPartial.DenNgay;
                        ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                        ThongBaoModel.NghiepVuID = Utils.ConvertToInt32(query, 0);
                        ThongBaoModel.TenNghiepVu = DotKeKhaiPartial.TenDotKeKhai;
                        ThongBaoModel.HienThi = true;
                        ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                        if (DanhSachCanBo != null && DanhSachCanBo.Count > 0)
                        {
                            foreach (var cb in DanhSachCanBo)
                            {
                                DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                                int ID = Utils.ConvertToInt32(cb, 0);
                                var CanBo = new HeThongCanBoDAL().GetCanBoByID(ID);
                                dt.CanBoID = ID;
                                dt.CoQuanID = CanBo.CoQuanID;
                                dt.TenCanBo = CanBo.TenCanBo;
                                dt.GioiTinh = CanBo.GioiTinh;
                                dt.Email = CanBo.Email;
                                ThongBaoModel.DoiTuongThongBao.Add(dt);
                            }
                        }
                        new NhacViecDAL().Edit_ThongBao(ThongBaoModel);

                        trans.Commit();
                        Result.Status = 1;
                        Result.Message = "Thêm đợt kê khai thành công";
                    }
                    catch (Exception ex)
                    {
                        Result.Status = -1;
                        Result.Message = ex.ToString();
                        trans.Rollback();
                        throw ex;
                    }
                    return Result;
                }
            }
        }

        public BaseResultModel Insert(DotKeKhaiPartial DotKeKhaiPartial, int? CapQuanLy, int CoQuanID, int CanBoID, int NguoiDungID)
        {
            var Result = new BaseResultModel();
       
            if (DotKeKhaiPartial.TuNgay == null || DotKeKhaiPartial.TuNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.DenNgay == null || DotKeKhaiPartial.DenNgay == DateTime.MinValue)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu ngày kê khai";
                return Result;
            }
            if (DotKeKhaiPartial.TuNgay > DotKeKhaiPartial.DenNgay)
            {
                Result.Status = 0;
                Result.Message = "Thời gian kê khai không hợp lệ";
                return Result;
            }
            if (DotKeKhaiPartial.TenDotKeKhai == null || DotKeKhaiPartial.TenDotKeKhai.Length < 1 || DotKeKhaiPartial.TenDotKeKhai.Length > 1000)
            {
                Result.Status = 0;
                Result.Message = "Tên đợt kê khai không được trống và độ dài không quá 1000 ký tự";
                return Result;
            }

            //check thanh tra tinh
            var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);

            var table = new DataTable();
            table.Columns.Add("ID1", typeof(string));
            table.Columns.Add("ID2", typeof(string));

            if(DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
            {
                DotKeKhaiPartial.DanhSachCanBo.ForEach(x => table.Rows.Add(0, x));
            }
        
            var ListCanBo = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
            ListCanBo.TypeName = "dbo.id_id_list";

            SqlParameter[] parameters = new SqlParameter[]
              {
                            new SqlParameter(TU_NGAY, SqlDbType.DateTime),
                            new SqlParameter(DEN_NGAY, SqlDbType.DateTime),
                            new SqlParameter(TRANG_THAI, SqlDbType.Bit),
                            new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                            new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                            new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                            new SqlParameter(TEN_DOT_KE_KHAI,SqlDbType.NVarChar),
                            new SqlParameter(MO_TA_DOT_KE_KHAI,SqlDbType.NVarChar),
                            new SqlParameter(CAP_QUAN_LY,SqlDbType.Int),
                            new SqlParameter(CO_QUAN_TAO,SqlDbType.Int),
                            new SqlParameter("LaThanhTraTinh",SqlDbType.Int),
                            new SqlParameter("Message",SqlDbType.NVarChar),
                            ListCanBo,
              };
            parameters[0].Value = DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd");
            parameters[1].Value = DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd");
            parameters[2].Value = 1;
            parameters[3].Value = DotKeKhaiPartial.TuNgay.Year;
            parameters[4].Value = DotKeKhaiPartial.LoaiDotKeKhai;
            parameters[5].Value = DotKeKhaiPartial.ApDungCho ?? Convert.DBNull;
            parameters[6].Value = DotKeKhaiPartial.TenDotKeKhai;
            var MoTaDotKeKhai = "Áp dụng cho";
            if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
            {
                MoTaDotKeKhai = MoTaDotKeKhai + " một số cán bộ (";

                for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                {
                    MoTaDotKeKhai = MoTaDotKeKhai + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + ", ";
                }
                MoTaDotKeKhai = MoTaDotKeKhai.Trim().Substring(0, MoTaDotKeKhai.LastIndexOf(','));
                MoTaDotKeKhai = MoTaDotKeKhai + ")";
            }
            else
            {
                MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ trên toàn tỉnh";
            }
            MoTaDotKeKhai = MoTaDotKeKhai.Trim();
            parameters[7].Value = MoTaDotKeKhai;
            parameters[8].Value = CapQuanLy ?? Convert.DBNull;
            if (CoQuanID == 0) parameters[10].Value = Convert.DBNull;
            else parameters[9].Value = CoQuanID;
            parameters[10].Value = laThanhTraTinh;
            parameters[11].Direction = ParameterDirection.Output;
            parameters[11].Size = 500;
            parameters[12].Value = table;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        var query = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, "v1_KeKhai_DotKeKhai_Insert_New", parameters);
                        Result.Message = Utils.ConvertToString(parameters[11].Value, String.Empty);
                        if(Result.Message.Length > 0)
                        {
                            Result.Status = 0;
                        }
                        else
                        {
                            Result.Status = 1;
                            Result.Data = Utils.ConvertToInt32(query, 0);
                            Result.Message = "Thêm đợt kê khai thành công";
                        }
 
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Result.Status = -1;
                        Result.Message = ex.ToString();
                        trans.Rollback();
                        throw ex;
                    }
                    if (Result.Status == 1)
                    {
                        //thêm thông báo
                        ThongBaoModel ThongBaoModel = new ThongBaoModel();
                        ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                        ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + DotKeKhaiPartial.TuNgay.ToString("dd/MM/yyyy")
                            + " đến ngày " + DotKeKhaiPartial.DenNgay.ToString("dd/MM/yyyy")
                            + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                        ThongBaoModel.ThoiGianBatDau = DotKeKhaiPartial.TuNgay;
                        ThongBaoModel.ThoiGianKetThuc = DotKeKhaiPartial.DenNgay;
                        ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                        ThongBaoModel.NghiepVuID = Utils.ConvertToInt32(Result.Data, 0);
                        ThongBaoModel.TenNghiepVu = DotKeKhaiPartial.TenDotKeKhai;
                        ThongBaoModel.HienThi = true;
                        ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();

                        new NhacViecDAL().Edit_ThongBao_New(ThongBaoModel);
                    }
                    return Result;
                }
            }
        }
        // Delete  Đợt kê khai
        public BaseResultModel Delete(List<int> ListID)
        {
            var Result = new BaseResultModel();
            if (ListID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Không có danh sách đợt kê khai cần xóa";
                return Result;
            }
            else
            {
                for (int i = 0; i < ListID.Count; i++)
                {
                    var crDotKeKhai = GetByID(ListID[i]);
                    if (crDotKeKhai == null || crDotKeKhai.DotKeKhaiID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Đợt kê khai không tồn tại";
                        return Result;
                    }
                    else
                    {
                        var danhSachBanKeKhai = new KeKhaiDAL().GetByDotKeKhaiID(ListID[i]);
                        if (danhSachBanKeKhai != null && danhSachBanKeKhai.Count > 0)
                        {
                            Result.Status = 0;
                            Result.Message = "Đợt kê khai đã có bản kê khai, không thể xóa!";
                            return Result;
                        }

                        SqlParameter[] parameters = new SqlParameter[]
                        {
                        new SqlParameter(DOT_KE_KHAI_ID, SqlDbType.Int)

                        };
                        parameters[0].Value = ListID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_DOT_KE_KHAI, parameters);
                                    Result.Message = "Xóa đợt kê khai thành công";
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = 0;
                                    Result.Message = ex.ToString();
                                    trans.Rollback();
                                    break;
                                    throw;
                                }
                            }
                        }
                    }
                }
                return Result;
            }
        }

        public DotKeKhaiModel GetBy_ID(int DotKeKhaiID)
        {
            //CheckTrangThai();
            DotKeKhaiModel DotKeKhai = new DotKeKhaiModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int)
              };
            parameters[0].Value = DotKeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, DOT_KE_KHAI_GET_BY_ID, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        DotKeKhai.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], true);
                        DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        DotKeKhai.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        DotKeKhai.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.MoTaDotKeKhai = Utils.ConvertToString(dr[MO_TA_DOT_KE_KHAI], string.Empty);

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return DotKeKhai;
        }
        //Get By id
        public DotKeKhaiPartial GetByID(int DotKeKhaiID)
        {
            //CheckTrangThai();
            DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int)
              };
            parameters[0].Value = DotKeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOT_KE_KHAI_GET_BY_ID, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], true);
                        DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        DotKeKhai.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        DotKeKhai.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.MoTaDotKeKhai = Utils.ConvertToString(dr[MO_TA_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                    }
                    dr.Close();
                }
                if (DotKeKhai.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
                {
                    DotKeKhai.DanhSachCanBo = DotKeKhaiDonViCanBo_GetBy_DotKeKhaiID(DotKeKhai.DotKeKhaiID).Select(x => x.CanBoID.Value).ToList();
                }
                var danhSachBanKeKhaiCuaDot = new KeKhaiDAL().GetByDotKeKhaiID(DotKeKhai.DotKeKhaiID);
                if (danhSachBanKeKhaiCuaDot != null && danhSachBanKeKhaiCuaDot.Count > 0)
                    DotKeKhai.CoBanKeKhai = true;
                else
                    DotKeKhai.CoBanKeKhai = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DotKeKhai;
        }

        public DotKeKhaiPartial GetByIDForEdit(int DotKeKhaiID)
        {
            //CheckTrangThai();
            DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int)
              };
            parameters[0].Value = DotKeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOT_KE_KHAI_GET_BY_ID, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], true);
                        DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        DotKeKhai.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        DotKeKhai.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.MoTaDotKeKhai = Utils.ConvertToString(dr[MO_TA_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        DotKeKhai.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                    }
                    dr.Close();
                }
                if (DotKeKhai.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
                {
                    var DanhSachCanBo = DotKeKhaiDonViCanBo_GetBy_DotKeKhaiID(DotKeKhai.DotKeKhaiID);
                    DotKeKhai.DanhSachCanBo = DanhSachCanBo.Select(x => x.CanBoID.Value).ToList().Distinct().ToList();
                    DotKeKhai.DanhSachCoQuan = DanhSachCanBo.Select(x => x.CoQuanID.Value).ToList().Distinct().ToList();
                }
                var danhSachBanKeKhaiCuaDot = new KeKhaiDAL().GetByDotKeKhaiID(DotKeKhai.DotKeKhaiID);
                if (danhSachBanKeKhaiCuaDot != null && danhSachBanKeKhaiCuaDot.Count > 0)
                    DotKeKhai.CoBanKeKhai = true;
                else
                    DotKeKhai.CoBanKeKhai = false;
            }
            catch
            {
                throw;
            }
            return DotKeKhai;
        }

        // Update  Đợt kê khai
        public BaseResultModel Update_New(DotKeKhaiPartial DotKeKhaiPartial, int NguoiDungID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            if (DotKeKhaiPartial.DotKeKhaiID == 0 || DotKeKhaiPartial.DotKeKhaiID == null)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu đợt kê khai cần sửa";
                return Result;
            }
            else
            {
                if (DotKeKhaiPartial.TuNgay == null || DotKeKhaiPartial.TuNgay == DateTime.MinValue)
                {
                    Result.Status = 0;
                    Result.Message = "Không có dữ liệu ngày kê khai";
                    return Result;
                }
                if (DotKeKhaiPartial.DenNgay == null || DotKeKhaiPartial.DenNgay == DateTime.MinValue)
                {
                    Result.Status = 0;
                    Result.Message = "Không có dữ liệu ngày kê khai";
                    return Result;
                }
                DotKeKhaiModel crDotKeKhai = GetBy_ID(DotKeKhaiPartial.DotKeKhaiID);
                if (crDotKeKhai == null || crDotKeKhai.DotKeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai không tồn tại";
                    return Result;
                }
                if (DotKeKhaiPartial.TrangThai == null)
                {
                    Result.Status = 0;
                    Result.Message = "Không có dữ liệu trạng thái";
                    return Result;
                }
                if (DotKeKhaiPartial.TenDotKeKhai == null || DotKeKhaiPartial.TenDotKeKhai.Length < 1 || DotKeKhaiPartial.TenDotKeKhai.Length > 1000)
                {
                    Result.Status = 0;
                    Result.Message = "Tên đợt kê khai không được trống và độ dài không quá 1000 ký tự";
                    return Result;
                }


                var banKeKhaiCuaDotNay = new KeKhaiDAL().GetByDotKeKhaiID(crDotKeKhai.DotKeKhaiID);
                var table = new DataTable();
                table.Columns.Add("ID1", typeof(string));
                table.Columns.Add("ID2", typeof(string));
                var DanhSachCanBo = DotKeKhaiPartial.DanhSachCanBo ?? new List<int>();
                List<int> DanhSachCanBoCheck = new List<int>();
                // check đợt kê khai đã có bản kê khai hay chưa'
                //if (banKeKhaiCuaDotNay.Count == null || banKeKhaiCuaDotNay.Count < 1)
                //{
                    var listDotKeKhaiTrongNam = GetListByNamKeKhai(DotKeKhaiPartial.TuNgay.Year);// trang thai=1 - dang con hieu luc
                    var listDotKeKhaiDangThucHien = listDotKeKhaiTrongNam.Where(x =>
                           (
                                (x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.DenNgay <= DotKeKhaiPartial.DenNgay.Date)
                                || (x.DenNgay >= DotKeKhaiPartial.DenNgay.Date && x.TuNgay <= DotKeKhaiPartial.DenNgay.Date)
                                || (x.TuNgay <= DotKeKhaiPartial.TuNgay.Date && x.DenNgay >= DotKeKhaiPartial.DenNgay.Date)
                           ) &&
                           (
                                (DotKeKhaiPartial.DenNgay >= x.TuNgay.Date && DotKeKhaiPartial.DenNgay <= x.DenNgay.Date)
                                || (DotKeKhaiPartial.DenNgay >= x.DenNgay.Date && DotKeKhaiPartial.TuNgay <= x.DenNgay.Date)
                                || (DotKeKhaiPartial.TuNgay <= x.TuNgay.Date && DotKeKhaiPartial.DenNgay >= x.DenNgay.Date)
                           )
                           && x.DotKeKhaiID != DotKeKhaiPartial.DotKeKhaiID
                           ).ToList();

                    //check thanh tra tinh
                    var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);

                    // lấy danh sách cán bộ đang thực hiện kê khai trong khoảng thời gian của đợt đang sửa
                    var danhSachCanBoDaCoDotKeKhai = GetDanhSachCanBo_By_DanhSachDotKeKhai(listDotKeKhaiDangThucHien.Select(x => x.DotKeKhaiID).ToList()).Select(x => x.CanBoID.Value).ToList();
                    var DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(((UserRole.CheckAdmin(NguoiDungID) == true) || laThanhTraTinh) ? 0 : CoQuanID).Select(x => x.CoQuanID).ToList();
                    var listCanBo_HopLe = new HeThongCanBoDAL().CanBoChucVu_GetAll().Where(x =>
                        x.TrangThaiID == EnumTrangThaiCanBo.DangLamViec.GetHashCode()
                       && DanhSachCoQuanCon.Contains(x.CoQuanID)
                        && !danhSachCanBoDaCoDotKeKhai.Contains(x.CanBoID)
                        );

                    var listAllCanBo = new HeThongCanBoDAL().CanBoChucVu_GetAll().Where(x =>
                    x.TrangThaiID == EnumTrangThaiCanBo.DangLamViec.GetHashCode()
                   && DanhSachCoQuanCon.Contains(x.CoQuanID) );

                if (DotKeKhaiPartial.LoaiDotKeKhai == null || DotKeKhaiPartial.LoaiDotKeKhai < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Loại đợt kê khai không được trống";
                        return Result;
                    }
                    if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()) // hàng năm
                    {
                        var listDotKeKhaiHangNam = listDotKeKhaiTrongNam.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()
                            && x.TrangThai == true && x.CoQuanTao == CoQuanID
                            && x.DotKeKhaiID != DotKeKhaiPartial.DotKeKhaiID
                            ).ToList();
                        if (listDotKeKhaiHangNam.Count > 0 && listDotKeKhaiHangNam.Any(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && x.ApDungCho == DotKeKhaiPartial.ApDungCho))
                        {
                            Result.Status = 0;
                            Result.Message = "Đợt kê khai đã có trên hệ thống. Vui lòng thử lại";
                            return Result;
                        }
                        // lấy danh sách cán bộ được kê khai đợt này
                        // cán bộ có chức vụ phải kê khai hàng năm và thuộc diện quản lý == DotKeKhaiPartial.ApDungCho

                        DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x => x.KeKhaiHangNam == true
                            && (DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                            ).Select(x => x.CanBoID));
                        if (DanhSachCanBo.Count < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                            return Result;
                        }
                    if (DotKeKhaiPartial.TrangThai == true)
                    {
                        DanhSachCanBoCheck.AddRange(listAllCanBo.Where(x => x.KeKhaiHangNam == true
                       && (DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                       ).Select(x => x.CanBoID));

                        if(DanhSachCanBoCheck.Count != DanhSachCanBo.Count)
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ đang thực hiện đợt kê khai khác";
                            return Result;
                        }
                    }
                    
                    
                }
                    else if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()) // bỏ sung
                    {
                        var listDotKeKhaiHangNam = listDotKeKhaiTrongNam.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()
                            && x.CoQuanTao == CoQuanID
                            && x.DotKeKhaiID != DotKeKhaiPartial.DotKeKhaiID
                            ).ToList();
                        if (listDotKeKhaiHangNam.Count > 0 && listDotKeKhaiHangNam.Any(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date && (x.ApDungCho == DotKeKhaiPartial.ApDungCho || x.ApDungCho == 3 || DotKeKhaiPartial.ApDungCho == 3)))
                        {
                            Result.Status = 0;
                            Result.Message = "Đợt kê khai trong năm đã được tạo";
                            return Result;
                        }
                        // lấy danh sách cán bộ được kê khai đợt này
                        DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x =>
                            DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy
                             ).Select(x => x.CanBoID));
                        if (DanhSachCanBo.Count < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                            return Result;
                        }

                        if (DotKeKhaiPartial.TrangThai == true)
                        {
                            DanhSachCanBoCheck.AddRange(listAllCanBo.Where(x =>
                            DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy
                             ).Select(x => x.CanBoID));

                            if (DanhSachCanBoCheck.Count != DanhSachCanBo.Count)
                            {
                                Result.Status = 0;
                                Result.Message = "Cán bộ đang thực hiện đợt kê khai khác";
                                return Result;
                            }
                        }
                    }
                    else if (DotKeKhaiPartial.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()) // bổ nhiệm
                    {
                        var listDotKeKhaiBoNhiem = GetByNamKeKhai(DotKeKhaiPartial.TuNgay.Year).Where(x => x.DenNgay >= DotKeKhaiPartial.TuNgay.Date
                            && x.TrangThai == true
                            && x.DotKeKhaiID != DotKeKhaiPartial.DotKeKhaiID
                            ).ToList();
                        if (listDotKeKhaiBoNhiem.Count > 0)
                        {
                            for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                            {
                                if (listDotKeKhaiBoNhiem.Any(x => x.CanBoID.Value == DotKeKhaiPartial.DanhSachCanBo[i]))
                                {
                                    Result.Status = 0;
                                    Result.Message = "Cán bộ " + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + " đang thực hiện đợt kê khai khác";
                                    return Result;
                                }
                            }

                        }
                        // lấy danh sách cán bộ được kê khai đợt này
                        DanhSachCanBo = DotKeKhaiPartial.DanhSachCanBo.Where(x => !danhSachCanBoDaCoDotKeKhai.Contains(x)).ToList();
                    }
                    else // lần đầu
                    {
                        // lấy danh sách cán bộ được kê khai đợt này - tất cả cán bộ chưa có bản kê khai nào
                        var listCanBoDaCoBanKeKhai = new KeKhaiDAL().GetAll().Select(x => x.CanBoID);
                        DanhSachCanBo.AddRange(listCanBo_HopLe.Where(x =>
                            (DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                            && !listCanBoDaCoBanKeKhai.Contains(x.CanBoID)
                            //  && !danhSachCanBoDaCoDotKeKhai.Contains(x.CanBoID)
                            ).Select(x => x.CanBoID));
                        if (DanhSachCanBo.Count < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ đang thực hiện đợt kê khai khác";
                            return Result;
                        }

                        if (DotKeKhaiPartial.TrangThai == true)
                        {
                            DanhSachCanBoCheck.AddRange(listAllCanBo.Where(x =>
                            (DotKeKhaiPartial.ApDungCho < 3 ? x.CapQuanLy == DotKeKhaiPartial.ApDungCho : x.CapQuanLy == x.CapQuanLy)
                            && !listCanBoDaCoBanKeKhai.Contains(x.CanBoID)
                            ).Select(x => x.CanBoID));

                        if (DanhSachCanBoCheck.Count != DanhSachCanBo.Count)
                            {
                                Result.Status = 0;
                                Result.Message = "Cán bộ đang thực hiện đợt kê khai khác";
                                return Result;
                            }
                        }
                    }

                    DanhSachCanBo = DanhSachCanBo.Distinct().ToList();
                    if (DanhSachCanBo == null || DanhSachCanBo.Count < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Không có cán bộ nào thuộc diện kê khai";
                        return Result;
                    }

                    DanhSachCanBo.ForEach(x => table.Rows.Add(new HeThongCanBoDAL().GetCanBoByID(x).CoQuanID.Value, x));
                    if (DanhSachCanBo.Count > 0) DotKeKhai_DonVi_CanBo_Delete_By_DotKeKhaiID(crDotKeKhai.DotKeKhaiID);
                //}
                var ListCanBo = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
                ListCanBo.TypeName = "dbo.id_id_list";

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter(TU_NGAY, SqlDbType.DateTime),
                new SqlParameter(DEN_NGAY, SqlDbType.DateTime),
                new SqlParameter(TRANG_THAI, SqlDbType.Bit),
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int),
                    ListCanBo,
                new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                new SqlParameter(TEN_DOT_KE_KHAI,SqlDbType.NVarChar),
                new SqlParameter(MO_TA_DOT_KE_KHAI,SqlDbType.NVarChar),
                 new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                };

                parameters[0].Value = DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd");
                parameters[1].Value = DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd");
                parameters[2].Value = DotKeKhaiPartial.TrangThai ?? Convert.DBNull;
                parameters[3].Value = DotKeKhaiPartial.TuNgay.Year;
                parameters[4].Value = DotKeKhaiPartial.DotKeKhaiID;
                parameters[5].Value = table;
                parameters[6].Value = DotKeKhaiPartial.ApDungCho ?? Convert.DBNull;
                parameters[7].Value = DotKeKhaiPartial.TenDotKeKhai;
                if (crDotKeKhai.LoaiDotKeKhai != DotKeKhaiPartial.LoaiDotKeKhai || crDotKeKhai.ApDungCho != DotKeKhaiPartial.ApDungCho)
                {
                    var MoTaDotKeKhai = "Áp dụng cho";
                    if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
                    {
                        MoTaDotKeKhai = MoTaDotKeKhai + " một số cán bộ (";

                        for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + ", ";
                        }
                        MoTaDotKeKhai = MoTaDotKeKhai.Trim().Substring(0, MoTaDotKeKhai.LastIndexOf(','));
                        MoTaDotKeKhai = MoTaDotKeKhai + ")";
                    }
                    else
                    {
                        if (DotKeKhaiPartial.ApDungCho == 1)
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp tỉnh quản lý";
                        }
                        else if (DotKeKhaiPartial.ApDungCho == 2)
                        {
                            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);

                            if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) // người tạo thuộc tỉnh hoặc sở
                            {
                                MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn tỉnh";
                            }
                            else // người tạo thuộc huyện hoặc phòng
                            {
                                if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                                {
                                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuan;
                                }
                                else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())
                                {
                                    //var crCoQuanCha = new DanhMucCoQuanDonViDAL().GetByID(crCoQuan.CoQuanChaID);
                                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuanCha;
                                }
                                else
                                {
                                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý";
                                }
                            }
                        }
                        else if (DotKeKhaiPartial.ApDungCho == 3)
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ trên toàn tỉnh";
                        }
                    }

                    MoTaDotKeKhai = MoTaDotKeKhai.Trim();

                    parameters[8].Value = MoTaDotKeKhai; // sửa chỗ này
                }
                else parameters[8].Value = crDotKeKhai.MoTaDotKeKhai;


                parameters[9].Value = DotKeKhaiPartial.LoaiDotKeKhai;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int val1 = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_NEW_DOTKEKHAI, parameters);
                            if (val1 > 0)
                            {
                                Result.Status = 1;
                                Result.Message = "Sửa thông tin đợt kê khai thành công";
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            Result.Status = 0;
                            Result.Message = ex.ToString();
                            trans.Rollback();
                            throw;
                        }

                    }
                    //return Result;
                }
                if (Result.Status == 1)
                {
                    //thêm thông báo
                    ThongBaoModel ThongBaoModel = new ThongBaoModel();
                    ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                    ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + DotKeKhaiPartial.TuNgay.ToString("dd/MM/yyyy")
                        + " đến ngày " + DotKeKhaiPartial.DenNgay.ToString("dd/MM/yyyy")
                        + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                    ThongBaoModel.ThoiGianBatDau = DotKeKhaiPartial.TuNgay;
                    ThongBaoModel.ThoiGianKetThuc = DotKeKhaiPartial.DenNgay;
                    ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                    ThongBaoModel.NghiepVuID = DotKeKhaiPartial.DotKeKhaiID;
                    ThongBaoModel.TenNghiepVu = DotKeKhaiPartial.TenDotKeKhai;
                    ThongBaoModel.HienThi = DotKeKhaiPartial.TrangThai;
                    ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                    var DoiTuongThongBao = DotKeKhaiDonViCanBo_GetBy_DotKeKhaiID(DotKeKhaiPartial.DotKeKhaiID);
                    if (DoiTuongThongBao != null && DoiTuongThongBao.Count > 0)
                    {
                        foreach (var cb in DoiTuongThongBao)
                        {
                            DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                            var CanBo = new HeThongCanBoDAL().GetCanBoByID(cb.CanBoID);
                            dt.CanBoID = CanBo.CanBoID;
                            dt.CoQuanID = CanBo.CoQuanID;
                            dt.TenCanBo = CanBo.TenCanBo;
                            dt.GioiTinh = CanBo.GioiTinh;
                            dt.Email = CanBo.Email;
                            ThongBaoModel.DoiTuongThongBao.Add(dt);
                        }
                    }
                    new NhacViecDAL().Edit_ThongBao(ThongBaoModel);
                }
                return Result;
            }
        }

        public BaseResultModel Update(DotKeKhaiPartial DotKeKhaiPartial, int NguoiDungID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            if (DotKeKhaiPartial.DotKeKhaiID == 0 || DotKeKhaiPartial.DotKeKhaiID == null)
            {
                Result.Status = 0;
                Result.Message = "Không có dữ liệu đợt kê khai cần sửa";
                return Result;
            }
            else
            {
                if (DotKeKhaiPartial.TuNgay == null || DotKeKhaiPartial.TuNgay == DateTime.MinValue)
                {
                    Result.Status = 0;
                    Result.Message = "Không có dữ liệu ngày kê khai";
                    return Result;
                }
                if (DotKeKhaiPartial.DenNgay == null || DotKeKhaiPartial.DenNgay == DateTime.MinValue)
                {
                    Result.Status = 0;
                    Result.Message = "Không có dữ liệu ngày kê khai";
                    return Result;
                }
                DotKeKhaiModel crDotKeKhai = GetBy_ID(DotKeKhaiPartial.DotKeKhaiID);
                if (crDotKeKhai == null || crDotKeKhai.DotKeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai không tồn tại";
                    return Result;
                }
                if (DotKeKhaiPartial.TrangThai == null)
                {
                    Result.Status = 0;
                    Result.Message = "Không có dữ liệu trạng thái";
                    return Result;
                }
                if (DotKeKhaiPartial.TenDotKeKhai == null || DotKeKhaiPartial.TenDotKeKhai.Length < 1 || DotKeKhaiPartial.TenDotKeKhai.Length > 1000)
                {
                    Result.Status = 0;
                    Result.Message = "Tên đợt kê khai không được trống và độ dài không quá 1000 ký tự";
                    return Result;
                }
                if (DotKeKhaiPartial.LoaiDotKeKhai < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Loại đợt kê khai không được trống";
                    return Result;
                }
                //check thanh tra tinh
                var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);

                var table = new DataTable();
                table.Columns.Add("ID1", typeof(string));
                table.Columns.Add("ID2", typeof(string));

                if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
                {
                    DotKeKhaiPartial.DanhSachCanBo.ForEach(x => table.Rows.Add(0, x));
                }

                var ListCanBo = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
                ListCanBo.TypeName = "dbo.id_id_list";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(TU_NGAY, SqlDbType.DateTime),
                    new SqlParameter(DEN_NGAY, SqlDbType.DateTime),
                    new SqlParameter(TRANG_THAI, SqlDbType.Bit),
                    new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                    new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int),
                    new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                    new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                    new SqlParameter(TEN_DOT_KE_KHAI,SqlDbType.NVarChar),
                    new SqlParameter(MO_TA_DOT_KE_KHAI,SqlDbType.NVarChar),
                    new SqlParameter("CoQuanID",SqlDbType.Int),
                    new SqlParameter("LaThanhTraTinh",SqlDbType.Int),
                    new SqlParameter("Message",SqlDbType.NVarChar),
                    ListCanBo,
                };

                parameters[0].Value = DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.TuNgay.ToString("yyyy/MM/dd");
                parameters[1].Value = DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd") == "" ? DateTime.Now.ToString() : DotKeKhaiPartial.DenNgay.ToString("yyyy/MM/dd");
                parameters[2].Value = DotKeKhaiPartial.TrangThai ?? Convert.DBNull;
                parameters[3].Value = DotKeKhaiPartial.TuNgay.Year;
                parameters[4].Value = DotKeKhaiPartial.DotKeKhaiID;
                parameters[5].Value = DotKeKhaiPartial.LoaiDotKeKhai;
                parameters[6].Value = DotKeKhaiPartial.ApDungCho ?? Convert.DBNull;
                parameters[7].Value = DotKeKhaiPartial.TenDotKeKhai;
                if (crDotKeKhai.LoaiDotKeKhai != DotKeKhaiPartial.LoaiDotKeKhai || crDotKeKhai.ApDungCho != DotKeKhaiPartial.ApDungCho)
                {
                    var MoTaDotKeKhai = "Áp dụng cho";
                    if (DotKeKhaiPartial.DanhSachCanBo != null && DotKeKhaiPartial.DanhSachCanBo.Count > 0)
                    {
                        MoTaDotKeKhai = MoTaDotKeKhai + " một số cán bộ (";

                        for (int i = 0; i < DotKeKhaiPartial.DanhSachCanBo.Count; i++)
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + new HeThongCanBoDAL().GetCanBoByID(DotKeKhaiPartial.DanhSachCanBo[i]).TenCanBo + ", ";
                        }
                        MoTaDotKeKhai = MoTaDotKeKhai.Trim().Substring(0, MoTaDotKeKhai.LastIndexOf(','));
                        MoTaDotKeKhai = MoTaDotKeKhai + ")";
                    }
                    else
                    {
                        if (DotKeKhaiPartial.ApDungCho == 1)
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp tỉnh quản lý";
                        }
                        else if (DotKeKhaiPartial.ApDungCho == 2)
                        {
                            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);

                            if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) // người tạo thuộc tỉnh hoặc sở
                            {
                                MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn tỉnh";
                            }
                            else // người tạo thuộc huyện hoặc phòng
                            {
                                if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                                {
                                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuan;
                                }
                                else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())
                                {
                                    //var crCoQuanCha = new DanhMucCoQuanDonViDAL().GetByID(crCoQuan.CoQuanChaID);
                                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý trên toàn huyện " + crCoQuan.TenCoQuanCha;
                                }
                                else
                                {
                                    MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ thuộc cấp huyện quản lý";
                                }
                            }
                        }
                        else if (DotKeKhaiPartial.ApDungCho == 3)
                        {
                            MoTaDotKeKhai = MoTaDotKeKhai + " cán bộ trên toàn tỉnh";
                        }
                    }

                    MoTaDotKeKhai = MoTaDotKeKhai.Trim();

                    parameters[8].Value = MoTaDotKeKhai; // sửa chỗ này
                }
                else parameters[8].Value = crDotKeKhai.MoTaDotKeKhai;
                parameters[9].Value = CoQuanID;
                parameters[10].Value = laThanhTraTinh;
                parameters[11].Direction = ParameterDirection.Output;
                parameters[11].Size = 500;
                parameters[12].Value = table;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int val1 = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_KeKhai_DotKeKhai_Update_New", parameters);
                            Result.Message = Utils.ConvertToString(parameters[11].Value, String.Empty);
                            if (Result.Message != null && Result.Message.Length > 0)
                            {
                                Result.Status = 0;                            
                            }
                            else
                            {
                                Result.Status = 1;
                                Result.Message = "Sửa thông tin đợt kê khai thành công";
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            Result.Status = 0;
                            Result.Message = ex.ToString();
                            trans.Rollback();
                            throw;
                        }

                    }
                }
                if (Result.Status == 1)
                {
                    //thêm thông báo
                    ThongBaoModel ThongBaoModel = new ThongBaoModel();
                    ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                    ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + DotKeKhaiPartial.TuNgay.ToString("dd/MM/yyyy")
                        + " đến ngày " + DotKeKhaiPartial.DenNgay.ToString("dd/MM/yyyy")
                        + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                    ThongBaoModel.ThoiGianBatDau = DotKeKhaiPartial.TuNgay;
                    ThongBaoModel.ThoiGianKetThuc = DotKeKhaiPartial.DenNgay;
                    ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                    ThongBaoModel.NghiepVuID = DotKeKhaiPartial.DotKeKhaiID;
                    ThongBaoModel.TenNghiepVu = DotKeKhaiPartial.TenDotKeKhai;
                    ThongBaoModel.HienThi = DotKeKhaiPartial.TrangThai;
                    ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();

                    new NhacViecDAL().Edit_ThongBao_New(ThongBaoModel);
                }
                return Result;
            }
        }

        /// <summary>
        /// Lấy dánh sách phân trang, có bộ lọc theo năm, cơ quan
        /// </summary>
        /// <param name="p"></param>
        /// <param name="TotalRow"></param>
        /// <param name=CO_QUAN_ID></param>
        /// <param name="CanBoID"></param>
        /// <returns></returns>
        public List<DotKeKhaiPartial> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CoQuanID, int CanBoID)
        {
            //CheckTrangThai();
            List<DotKeKhaiPartial> list = new List<DotKeKhaiPartial>();
            var table = new DataTable();
            table.Columns.Add("list_ID", typeof(string));
            var DanhSachCoQuanID = new List<DanhMucCoQuanDonViModel>();
            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            if (UserRole.CheckAdmin(CanBoID) || listThanhTraTinh.Contains(CoQuanID))
            {
                DanhSachCoQuanID = new DanhMucCoQuanDonViDAL().GetAllCapCon(0);
            }
            else if (listThanhTraHuyen.Contains(CoQuanID))
            {
                DanhSachCoQuanID = new DanhMucCoQuanDonViDAL().GetAllCapCon(new DanhMucCoQuanDonViDAL().GetByID(CoQuanID).CoQuanChaID);
            }
            else
            {
                DanhSachCoQuanID = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
            }
            DanhSachCoQuanID.ForEach(x => table.Rows.Add(x.CoQuanID));
            var ListCanBo = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
            ListCanBo.TypeName = "dbo.list_ID";

            SqlParameter[] parameters = new SqlParameter[]
              {
                   new SqlParameter("TotalRow",SqlDbType.Int),
                    new SqlParameter("pLimit",SqlDbType.Int),
                    new SqlParameter("pOffset",SqlDbType.Int),
                    new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                    new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                    new SqlParameter(TRANG_THAI,SqlDbType.Int),
                    new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                    new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                    ListCanBo,
                    new SqlParameter(CO_QUAN_ID,SqlDbType.Int),
              };
            parameters[0].Direction = ParameterDirection.Output;
            parameters[0].Size = 8;
            parameters[1].Value = p.Limit;
            parameters[2].Value = p.Offset;
            parameters[3].Value = p.ApDungCho ?? Convert.DBNull;
            parameters[4].Value = p.CanBoID ?? Convert.DBNull;
            parameters[5].Value = p.TrangThai ?? Convert.DBNull;
            parameters[6].Value = p.Nam ?? Convert.DBNull;
            parameters[7].Value = p.LoaiDotKeKhai ?? Convert.DBNull;
            parameters[8].Value = table;
            parameters[9].Value = p.CoQuanID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KE_KHAI_DOT_KE_KHAI_GETPAGING, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = GetData(dr);
                        DotKeKhai.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.MoTaDotKeKhai = Utils.ConvertToString(dr[MO_TA_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.CapQuanLy = Utils.ConvertToInt32(dr[CAP_QUAN_LY], 0);
                        DotKeKhai.CoQuanTao = Utils.ConvertToInt32(dr[CO_QUAN_TAO], 0);
                        if (UserRole.CheckAdmin(CanBoID))
                        {
                            if (DotKeKhai.CoQuanTao == 0) DotKeKhai.QuyenChinhSua = true;
                            else DotKeKhai.QuyenChinhSua = false;
                        }
                        else
                        {
                            if (DotKeKhai.CoQuanTao == CoQuanID)
                                DotKeKhai.QuyenChinhSua = true;
                            else DotKeKhai.QuyenChinhSua = false;
                        }
                        list.Add(DotKeKhai);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[0].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {

                }
            }
            return list;
        }

        #endregion

        #region DotKeKhai_DonVi_CanBo

        // Get DotKeKhai_CanBo_CoQuanDonVi By DotKeKhaiID
        public List<DotKeKhaiPartial> DotKeKhaiDonViCanBo_GetBy_DotKeKhaiID(int DotKeKhaiID)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    new SqlParameter(DOT_KE_KHAI_ID_CANBO_DONVI,SqlDbType.Int)
                 };
                parameters[0].Value = DotKeKhaiID;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_DOTKEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID_CANBO_DONVI], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        List.Add(DotKeKhai);
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

        /// <summary>
        /// lấy danh sách cán bộ có trong danh sách đợt kê khai
        /// </summary>
        /// <param name="DanhSacDotKeKhaiID"></param>
        /// <returns></returns>
        public List<DotKeKhaiPartial> GetDanhSachCanBo_By_DanhSachDotKeKhai(List<int> DanhSacDotKeKhaiID)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                DanhSacDotKeKhaiID.ForEach(x => table.Rows.Add(x));
                var listDotKeKhai = new SqlParameter("@DanhSachDotKeKhaiID", SqlDbType.Structured);
                listDotKeKhai.TypeName = "dbo.list_ID";
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    listDotKeKhai
                 };
                parameters[0].Value = table;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAICOQUANCANBO_GET_DANHSACHCANBO_BY_DANHSACHDOTKEKHAI, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        List.Add(DotKeKhai);
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

        //Get Đợt kê khai by áp dụng cho và loại đợt kê khai 
        public List<DotKeKhaiPartial> GetDotKeKhaiByApDungChoAndLoaiDotKeKhai(int ApDungCho, int LoaiDotKeKhai)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    new SqlParameter(AP_DUNG_CHO,SqlDbType.Int),
                     new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int)
                 };
                parameters[0].Value = ApDungCho;
                parameters[1].Value = LoaiDotKeKhai;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_ApDungChoAndLoaiDotKeKhai, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        DotKeKhai.CoQuanTao = Utils.ConvertToInt32(dr[CO_QUAN_TAO], 0);
                        List.Add(DotKeKhai);
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

        //Get đợt kê khai đơn vị cán bộ by CoQuanID
        public List<DotKeKhaiPartial> DotKeKhaiDonViCanBo_GetBy_CoQuanID(int CoQuanID)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    new SqlParameter(CO_QUAN_ID,SqlDbType.Int)
                 };
                parameters[0].Value = CoQuanID;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_COQUANID, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        List.Add(DotKeKhai);
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

        //Get đợt kê khai đơn vị cán bộ by CoQuanID
        public List<DotKeKhaiPartial> DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(List<int> DanhSachCoQuanID , int Nam)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                var pList = new SqlParameter("@DanhSachCoQuanID", SqlDbType.Structured);
                pList.TypeName = "dbo.list_ID";
                var tbCoQuanID = new DataTable();
                tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
                DanhSachCoQuanID.Distinct().ToList().ForEach(x => tbCoQuanID.Rows.Add(x));
                SqlParameter[] parameters = new SqlParameter[]
                 {
                   pList,
                    new SqlParameter("Nam",SqlDbType.Int),
                 };
                parameters[0].Value = tbCoQuanID;
                parameters[1].Value = Nam;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_DotKeKhai_GetByCoQuanID", parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        List.Add(DotKeKhai);
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

        // Lấy đợt kê khai đơn vị cán bộ by CoQuanID và Năm kê khai
        public List<DotKeKhaiPartial> DotKeKhaiDonViCanBo_GetBy_CoQuanIDAndNamKeKhai(int? CoQuanID, int? NamKeKhai)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    new SqlParameter(CO_QUAN_ID,SqlDbType.Int),
                       new SqlParameter(NAM_KE_KHAI,SqlDbType.Int)
                 };
                parameters[0].Value = CoQuanID ?? Convert.DBNull;
                parameters[1].Value = NamKeKhai ?? Convert.DBNull;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAICOQUANCANBO_GET_BY_COQUANID_AND_NAMKEKHAI, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        DotKeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        List.Add(DotKeKhai);
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

        // Xóa DotKeKhai_CanBo_CoQuanDonVi by DotKeKhaiID
        public BaseResultModel DotKeKhai_DonVi_CanBo_Delete_By_DotKeKhaiID(int DotKeKhaiID)
        {
            var Result = new BaseResultModel();
            if (DotKeKhaiID < 1)
            {
                Result.Status = 0;
                Result.Message = "Không có đợt kê khai";
                return Result;
            }
            else
            {
                var crDotKeKhai = GetByID(DotKeKhaiID);
                if (crDotKeKhai == null || crDotKeKhai.DotKeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai không tồn tại";
                    return Result;
                }
                else
                {
                    var danhSachBanKeKhai = new KeKhaiDAL().GetByDotKeKhaiID(DotKeKhaiID);
                    if (danhSachBanKeKhai != null && danhSachBanKeKhai.Count > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Đợt kê khai đã có bản kê khai";
                        return Result;
                    }

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter(DOT_KE_KHAI_ID_CANBO_DONVI, SqlDbType.Int)
                    };
                    parameters[0].Value = DotKeKhaiID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_KEKHAI_DOTKEKHAICOQUANCANBO, parameters);
                                Result.Message = "Xóa thành công";
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                Result.Status = 0;
                                Result.Message = ex.ToString();
                                trans.Rollback();
                                throw;
                            }
                        }
                    }
                }

                return Result;
            }
        }

        // insert 1 dotkekhaicanbocoquan
        public BaseResultModel DotKeKhai_DonVi_CanBo_Insert(int? CanBoID, int? CoQuanID, int? DotKeKhaiID)
        {
            var Result = new BaseResultModel();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]
                  {
                            new SqlParameter(CAN_BO_ID, SqlDbType.Int),
                               new SqlParameter(CO_QUAN_ID, SqlDbType.Int),
                          new SqlParameter(DOT_KE_KHAI_ID_CANBO_DONVI, SqlDbType.Int),

                  };
                parameters[0].Value = CanBoID ?? Convert.DBNull;
                parameters[1].Value = CoQuanID ?? Convert.DBNull;
                parameters[2].Value = DotKeKhaiID ?? Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_KEKHAI_DOTKEKHAICOQUANCANBO, parameters);
                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("DotKeKhai_DonVi_CanBo");
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

        // Thêm 1 list DotKeKhai_CanBo_CoQuanDonVi 
        public BaseResultModel DotKeKhai_DonVi_CanBo_InsertList(int DotKeKhaiID, List<int> DanhSachCanBo)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DotKeKhaiID == null || DotKeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai không được trống";
                    return Result;
                }
                if (DanhSachCanBo == null || DanhSachCanBo.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Danh sách cán bộ không được trống";
                }
                else
                {
                    var table = new DataTable();
                    table.Columns.Add(CO_QUAN_ID, typeof(string));
                    table.Columns.Add("CanBoID", typeof(string));
                    Dictionary<int, int> List = new Dictionary<int, int>();
                    DanhSachCanBo.ForEach(x => List.Add(new HeThongCanBoDAL().GetCanBoByID(x).CoQuanID.Value, x));
                    foreach (var item in List)
                    {
                        table.Rows.Add(item);
                    }
                    var ListCanBoCoQuan = new SqlParameter("@list_idCanBo", SqlDbType.Structured);
                    ListCanBoCoQuan.TypeName = "dbo.id_id_list";

                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter(DOT_KE_KHAI_ID_CANBO_DONVI, SqlDbType.Int),
                            ListCanBoCoQuan,

                      };
                    parameters[0].Value = DotKeKhaiID;
                    parameters[1].Value = table;
                    //parameters[2].Value = table1;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_LIST_KEKHAI_DOTKEKHAICOQUANCANBO, parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("DotKeKhai_DonVi_CanBo");
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

        // Lấy đợt kê khai có trạng thái đang hoạt động
        public List<DotKeKhaiPartial> GetDotKeKhaiActive()
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAI_GETALLACTIVE))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        DotKeKhai.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        DotKeKhai.TuNgay = Utils.ConvertToDateTime(dr[TU_NGAY], DateTime.Now);
                        DotKeKhai.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now);
                        DotKeKhai.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI], false);
                        DotKeKhai.CoQuanTao = Utils.ConvertToInt32(dr[CO_QUAN_TAO], 0);
                        List.Add(DotKeKhai);
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

        //
        public List<DotKeKhaiPartial> GetAllCanBoDotKeKhaiActive(int CanBoID, int CapQuanLy)
        {
            List<DotKeKhaiPartial> List = new List<DotKeKhaiPartial>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                    new SqlParameter(CAP_QUAN_LY,SqlDbType.Int)
                };
                parameters[0].Value = CanBoID;
                parameters[1].Value = CapQuanLy;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DOTKEKHAI_GETALL_CANBODOTKEKHAI_ACTIVE, parameters))
                {
                    while (dr.Read())
                    {
                        DotKeKhaiPartial DotKeKhai = new DotKeKhaiPartial();
                        DotKeKhai.TenDotKeKhai = Utils.ConvertToString(dr[TEN_DOT_KE_KHAI], string.Empty);
                        DotKeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        DotKeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        DotKeKhai.ApDungCho = Utils.ConvertToInt32(dr[AP_DUNG_CHO], 0);
                        DotKeKhai.LoaiDotKeKhai = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        DotKeKhai.CoQuanID = Utils.ConvertToInt32(dr[CO_QUAN_ID], 0);
                        List.Add(DotKeKhai);
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
        // Lấy đợt kê khai phù hợp cho cán bộ sau khi tạo tài khoản
        public int GetDotKeKhaiFitForCanBo_old(int CanBoID, int CoQuanID)
        {
            CheckTrangThai();
            int val = 0;
            if (CanBoID <= 0)
            {
                return val;
            }
            var ListDotKeKhaiOpen = new List<DotKeKhaiPartial>();
            DotKeKhaiPartial DotKeKhaiPartial = new DotKeKhaiPartial();
            DotKeKhaiPartial DotKeKhaiCuoiCung = new DotKeKhaiPartial();
            try
            {
                var CanBoByID = new HeThongCanBoDAL().GetCanBoByID(CanBoID);
                var listdotkekhaiactiveAll = GetDotKeKhaiActive();
                var listdotkekhaiactive = new List<DotKeKhaiPartial>();
                for (int i = 0; i < listdotkekhaiactiveAll.Count; i++)
                {
                    //check thanh tra tinh
                    var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
                    var listDonViCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(listdotkekhaiactiveAll[i].CoQuanTao.Value).Select(x => x.CoQuanID).ToList();
                    if ((listdotkekhaiactiveAll[i].ApDungCho == CanBoByID.CapQuanLy.Value || listdotkekhaiactiveAll[i].ApDungCho == 3)
                        && (listdotkekhaiactiveAll[i].CoQuanTao == 0 || listdotkekhaiactiveAll[i].CoQuanTao == null) || listDonViCon.Contains(CanBoByID.CoQuanID.Value)
                        || laThanhTraTinh)
                    {
                        listdotkekhaiactive.Add(listdotkekhaiactiveAll[i]);
                    }
                }
                var listcanbokekhaiactive = GetAllCanBoDotKeKhaiActive(CanBoID, CanBoByID.CapQuanLy.Value);
                var listdotkekhaiid = new List<int>();
                listcanbokekhaiactive.ForEach(x => listdotkekhaiid.Add(x.DotKeKhaiID));
                if (listdotkekhaiactive.Count == listcanbokekhaiactive.Count)
                {
                    return val;
                }
                ListDotKeKhaiOpen.AddRange(listdotkekhaiactive.Where(x => !listdotkekhaiid.Contains(x.DotKeKhaiID)));
                var ListDotKeKhaiLanDauOpen = ListDotKeKhaiOpen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).ToList();
                if (ListDotKeKhaiLanDauOpen.Count <= 0)
                {
                    var ListDotKeKhaiBoSungOpen = ListDotKeKhaiOpen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).ToList();
                    if (ListDotKeKhaiBoSungOpen.Count <= 0)
                    {
                        var ListDotKeKhaiHangNamOpen = ListDotKeKhaiOpen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).ToList();
                        if (ListDotKeKhaiHangNamOpen.Count <= 0)
                        {
                            return val;
                        }
                        else
                        {
                            DotKeKhaiCuoiCung = ListDotKeKhaiHangNamOpen.OrderByDescending(x => x.DotKeKhaiID).FirstOrDefault();
                            val = DotKeKhai_DonVi_CanBo_Insert(CanBoID, CoQuanID, DotKeKhaiCuoiCung.DotKeKhaiID).Status;
                        }
                    }
                    else
                    {
                        DotKeKhaiCuoiCung = ListDotKeKhaiBoSungOpen.OrderByDescending(x => x.DotKeKhaiID).FirstOrDefault();
                        val = DotKeKhai_DonVi_CanBo_Insert(CanBoID, CoQuanID, DotKeKhaiCuoiCung.DotKeKhaiID).Status;
                    }
                }
                else
                {
                    DotKeKhaiCuoiCung = ListDotKeKhaiLanDauOpen.OrderByDescending(x => x.DotKeKhaiID).FirstOrDefault();
                    val = DotKeKhai_DonVi_CanBo_Insert(CanBoID, CoQuanID, DotKeKhaiCuoiCung.DotKeKhaiID).Status;
                }

                if (val > 0)
                {
                    //thêm thông báo
                    ThongBaoModel ThongBaoModel = new ThongBaoModel();
                    ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                    ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + DotKeKhaiCuoiCung.TuNgay.ToString("dd/MM/yyyy")
                        + " đến ngày " + DotKeKhaiCuoiCung.DenNgay.ToString("dd/MM/yyyy")
                        + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                    ThongBaoModel.ThoiGianBatDau = DotKeKhaiCuoiCung.TuNgay;
                    ThongBaoModel.ThoiGianKetThuc = DotKeKhaiCuoiCung.DenNgay;
                    ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                    ThongBaoModel.NghiepVuID = DotKeKhaiCuoiCung.DotKeKhaiID;
                    ThongBaoModel.TenNghiepVu = DotKeKhaiCuoiCung.TenDotKeKhai;
                    ThongBaoModel.HienThi = DotKeKhaiCuoiCung.TrangThai;
                    ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                    var DoiTuongThongBao = DotKeKhaiDonViCanBo_GetBy_DotKeKhaiID(DotKeKhaiCuoiCung.DotKeKhaiID);
                    if (DoiTuongThongBao != null && DoiTuongThongBao.Count > 0)
                    {
                        foreach (var cb in DoiTuongThongBao)
                        {
                            DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                            var CanBo = new HeThongCanBoDAL().GetCanBoByID(cb.CanBoID);
                            dt.CanBoID = CanBo.CanBoID;
                            dt.CoQuanID = CanBo.CoQuanID;
                            dt.TenCanBo = CanBo.TenCanBo;
                            dt.GioiTinh = CanBo.GioiTinh;
                            dt.Email = CanBo.Email;
                            ThongBaoModel.DoiTuongThongBao.Add(dt);
                        }
                    }
                    new NhacViecDAL().Edit_ThongBao(ThongBaoModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return val;
        }



        /// <summary>
        /// tìm đợt kê khai phù hợp cho cán bộ khi tạo cán bộ mới
        /// update by AnhVH,
        /// 01/03/2021
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public int GetDotKeKhaiFitForCanBo(int CanBoID, int CoQuanID)
        {
            CheckTrangThai();
            int val = 0;
            if (CanBoID <= 0)
            {
                return val;
            }

            // tìm ra đợt kê khai phù hợp
            var listdotkekhaiactiveAll = new List<DotKeKhaiPartial>();
            var dotKeKhaiPhuHop = new DotKeKhaiPartial();
            try
            {
                listdotkekhaiactiveAll = GetDotKeKhaiActive().Where(x => x.LoaiDotKeKhai != EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).ToList();
                if (listdotkekhaiactiveAll != null && listdotkekhaiactiveAll.Count > 0)
                {
                    var listDotKeKhaiLanDau = listdotkekhaiactiveAll.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).ToList();
                    if (listDotKeKhaiLanDau != null && listDotKeKhaiLanDau.Count > 0)
                        dotKeKhaiPhuHop = listDotKeKhaiLanDau.LastOrDefault();
                    else
                    {
                        var listDotKeKhaiBoSung = listdotkekhaiactiveAll.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).ToList();
                        if (listDotKeKhaiBoSung != null && listDotKeKhaiBoSung.Count > 0)
                            dotKeKhaiPhuHop = listDotKeKhaiBoSung.LastOrDefault();
                        else
                        {
                            var CanBoChucVu = new HeThongCanBoDAL().CanBoChucVu_GetBy_CanBoID(CanBoID);
                            if (CanBoChucVu != null && CanBoChucVu.Count > 0 && CanBoChucVu.Any(x => x.KeKhaiHangNam == true))
                            {
                                var listDotKeKhaiHangNam = listdotkekhaiactiveAll.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).ToList();
                                if (listDotKeKhaiHangNam != null && listDotKeKhaiHangNam.Count > 0)
                                    dotKeKhaiPhuHop = listDotKeKhaiHangNam.LastOrDefault();
                            }
                        }
                    }
                }
                // thêm cán bộ vào đợt kê khai phù hợp
                if (dotKeKhaiPhuHop != null && dotKeKhaiPhuHop.DotKeKhaiID > 0)
                    val = DotKeKhai_DonVi_CanBo_Insert(CanBoID, CoQuanID, dotKeKhaiPhuHop.DotKeKhaiID).Status;

                if (val > 0)
                {
                    //thêm thông báo
                    ThongBaoModel ThongBaoModel = new ThongBaoModel();
                    ThongBaoModel.TieuDe = "Thông báo đợt kê khai mới";
                    ThongBaoModel.NoiDung = "Bạn có đợt kê khai từ ngày " + dotKeKhaiPhuHop.TuNgay.ToString("dd/MM/yyyy")
                        + " đến ngày " + dotKeKhaiPhuHop.DenNgay.ToString("dd/MM/yyyy")
                        + ". Vui lòng thực hiện kê khai trong thời gian quy định.";
                    ThongBaoModel.ThoiGianBatDau = dotKeKhaiPhuHop.TuNgay;
                    ThongBaoModel.ThoiGianKetThuc = dotKeKhaiPhuHop.DenNgay;
                    ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.DotKeKhai.GetHashCode();
                    ThongBaoModel.NghiepVuID = dotKeKhaiPhuHop.DotKeKhaiID;
                    ThongBaoModel.TenNghiepVu = dotKeKhaiPhuHop.TenDotKeKhai;
                    ThongBaoModel.HienThi = dotKeKhaiPhuHop.TrangThai;
                    ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                    //var DoiTuongThongBao = DotKeKhaiDonViCanBo_GetBy_DotKeKhaiID(dotKeKhaiPhuHop.DotKeKhaiID);
                    //if (DoiTuongThongBao != null && DoiTuongThongBao.Count > 0)
                    //{
                    //    foreach (var cb in DoiTuongThongBao)
                    //    {
                    //        DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                    //        var CanBo = new HeThongCanBoDAL().GetCanBoByID(cb.CanBoID);
                    //        dt.CanBoID = CanBo.CanBoID;
                    //        dt.CoQuanID = CanBo.CoQuanID;
                    //        dt.TenCanBo = CanBo.TenCanBo;
                    //        dt.GioiTinh = CanBo.GioiTinh;
                    //        dt.Email = CanBo.Email;
                    //        ThongBaoModel.DoiTuongThongBao.Add(dt);
                    //    }
                    //}
                    new NhacViecDAL().Edit_ThongBao_New(ThongBaoModel);
                }

                return val;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion
    }
}
