using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{

    public interface ICongKhaiBanKeKhaiDAL
    {
        public List<CongKhaiBanKeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_And_DanhSachBanKeKhai_KeKhaiID(int KeKhaiID, int CanBoXemKeKhai);
    }
    public class CongKhaiBanKeKhaiDAL : ICongKhaiBanKeKhaiDAL
    {
        //tên các store procedure
        private const string KEKHAI_CONGKHAIBANKEKHAI_GETPAGINGBYSEARCH = @"v1_KeKhai_CongKhaiBanKeKhai_GetPagingBySearch_New";
        private const string KEKHAI_CONGKHAIBANKEKHAI_GET_BY_CANBOXEMID= @"v1_KeKhai_CongKhaiBanKeKhai_GetBy_CanBoXemID";
        private const string KEKHAI_CONGKHAIBANKEKHAI_CHECKHETHAN= @"v1_KeKhai_CheckHetHanCongKhaiBanKeKhai";
        private const string GET_TRANG_THAI_CONG_KHAI_BAN_KE_KHAI= @"v1_GetTrangThaiCongKhaiBanKeKhai";

        //Ten các params
        private const string KE_KHAI_ID = "NV00301";
        private const string CAN_BO_ID = "NV00303";
        private const string NAM = "NV00304";
        private const string TEN_BAN_KE_KHAI = "NV00306";
        private const string TRANG_THAI = "NV00305";
        private const string TRANG_THAI_NHAC_VIEC = "NV00308";
        private const string TRANG_THAI_CONG_KHAI = "NV00309";
        private const string CONG_KHAI_BAN_KE_KHAI_BAN_KE_KHAI_ID = "NV00801";
        private const string CONG_KHAI_BAN_KE_KHAI_ID = "NV00701";
        private const string NGUOI_DUYET_ID_CONG_KHAI = "NV00702";
        private const string NGAY_DUYET_CONG_KHAI = "NV00703";
        private const string NGAY_HET_HAN_CONG_KHAI = "NV00704";
        private const string TRANG_THAI_CUA_CONG_KHAI = "NV00705";
        private const string GHI_CHU_CUA_CONG_KHAI = "NV00706";
        private const string KEKHAIID_CONG_KHAI = "NV00802";

        /// <summary>
        /// lấy danh sách bản kê khai mà cán bộ đang đăng nhập được xem công khai - đã duyệt công khai
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CanBoID"></param>
        /// <param name="TotalRow"></param>
        /// <returns></returns>
        public List<CongKhaiBanKeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow)
        {
            CheckHetHanCongKhaiBanKeKhai();
            List<CongKhaiBanKeKhaiModel> Result = new List<CongKhaiBanKeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter("CoQuanID",SqlDbType.Int),
                new SqlParameter("CanBoID",SqlDbType.Int),
                new SqlParameter("Nam",SqlDbType.Int),
                new SqlParameter("@NguoiXemID",SqlDbType.Int),

              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.CoQuanID ?? Convert.DBNull;
            parameters[7].Value = p.CanBoID ?? Convert.DBNull;
            parameters[8].Value = p.Nam ?? Convert.DBNull;
            parameters[9].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_CONGKHAIBANKEKHAI_GETPAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        CongKhaiBanKeKhaiModel item = new CongKhaiBanKeKhaiModel();
                        item.NgayDuyet = Utils.ConvertToDateTime(dr[NGAY_DUYET_CONG_KHAI], DateTime.Now.Date);
                        item.NgayHetHan = Utils.ConvertToDateTime(dr[NGAY_HET_HAN_CONG_KHAI], DateTime.Now.Date);
                        if (item.NgayDuyet.Date <= DateTime.Now.Date && item.NgayHetHan.Date >= DateTime.Now.Date)
                        {
                            item.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                            item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            item.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                            item.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                            item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                            var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(item.CanBoID);
                            item.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            item.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            Result.Add(item);
                        }
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public List<KeKhaiModel> CongKhaiBanKeKhai_GetBy_CanBoXemID(int CanBoID, int NguoiXemID)
        {
            List<KeKhaiModel> Result = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int),
                new SqlParameter("@NguoiXemID",SqlDbType.Int),
              };


            parameters[0].Value = CanBoID;
            parameters[1].Value = NguoiXemID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_CONGKHAIBANKEKHAI_GET_BY_CANBOXEMID, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModel item = new KeKhaiModel();
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.CongKhaiBanKeKhaiID = Utils.ConvertToInt32(dr[CONG_KHAI_BAN_KE_KHAI_ID], 0);
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
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

        /// <summary>
        /// lấy chi tiết thông tin kê khai
        /// 1. thông tin người kê khai
        /// 2. thông tin vợ, chồng
        /// 3. thôg tin con cái
        /// 4 danh sách bản kê khai của cán bộ mà người đăng nhập được xem (có trong duyệt công khai)
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <param name="CanBoXemKeKhai"></param> cán bộ đang đăng nhập
        /// <returns></returns>
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_And_DanhSachBanKeKhai_KeKhaiID(int CanBoID, int CanBoXemKeKhai)
        {
            var Result = new ChiTietThongTinKeKhaiModel();
            if (CanBoID < 1)
            {
                return Result;
            }
            try
            {
                var crCanBo = new HeThongCanBoDAL().GetCanBoByID(CanBoID);
                if (crCanBo == null || crCanBo.CanBoID < 1) return Result;
                // thông tin bản thân
                var ThongTinBanThan = new HeThongCanBoDAL().GetChiTietCanBoByID(crCanBo.CanBoID);
                Result.BanThan = ThongTinBanThan;
                // thông tin vợ chồng
                var ThongTinThanNhan = new KeKhaiThanNhanDAL().GetThanNhanCanBo_By_CanBoID(crCanBo.CanBoID);
                Result.VoChong = ThongTinThanNhan.VoChong;
                // thông tin con chưa thành niên
                Result.ConChuaThanhNien = ThongTinThanNhan.ConChuaThanhNien;
                // danh sách bản kê khai được phép xem (đã duyệt bản kê khai và người xem có quyền xem)
                Result.DanhSachBanKeKhai = CongKhaiBanKeKhai_GetBy_CanBoXemID(crCanBo.CanBoID, CanBoXemKeKhai);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public void CheckHetHanCongKhaiBanKeKhai()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, KEKHAI_CONGKHAIBANKEKHAI_CHECKHETHAN);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        // Get bản kê khai đẻ lấy trạng thái  nhăc việc
        public List<KeKhaiModel> CongKhaiBanKeKhai_GetBy_CanBoIDAndKeKhaiID(int? CanBoID, int? KeKhaiID)
        {
            List<KeKhaiModel> Result = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                new SqlParameter(KE_KHAI_ID,SqlDbType.Int),
              };

            parameters[0].Value = CanBoID ?? Convert.DBNull;
            parameters[1].Value = KeKhaiID ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, GET_TRANG_THAI_CONG_KHAI_BAN_KE_KHAI, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModel item = new KeKhaiModel();
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.TrangThaiCongKhai = Utils.ConvertToInt32(dr[TRANG_THAI_CONG_KHAI], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TrangThaiNhacViec = Utils.ConvertToBoolean(dr[TRANG_THAI_NHAC_VIEC], false);
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
    }
}

