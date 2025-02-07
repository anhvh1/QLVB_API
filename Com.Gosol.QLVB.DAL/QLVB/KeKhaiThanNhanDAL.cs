using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IKeKhaiThanNhanDAL
    {
        public BaseResultModel Insert(KeKhaiThanNhanModel KeKhaiThanNhanModel);
        public BaseResultModel Update(KeKhaiThanNhanModel KeKhaiThanNhanModel);
        public BaseResultModel Delete(List<int> ListThanNhanID);
        public KeKhaiThanNhanModel GetByID(int ThanNhanID);
        public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CanBoID, int CoQuanID, int NguoiDungID, int VaiTro);
        public BaseResultModel InsertAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel);
        public BaseResultModel UpdateAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel);
        public ThanNhanCanBoModel GetThanNhanCanBo_By_CanBoID(int CanBoID);

    }
    public class KeKhaiThanNhanDAL : IKeKhaiThanNhanDAL
    {
        //tên các store procedure
        private const string KE_KHAI_THANNHAN_GET_BY_THANNHANID = @"v1_KeKhai_ThanNhan_GetByID";
        private const string INSERT_KE_KHAI_THANNHAN = @"v1_KeKhai_ThanNhan_Insert";
        private const string UPDATE_KE_KHAI_THANNHAN = @"v1_KeKhai_ThanNhan_Update";
        private const string DELETE_KE_KHAI_THANNHAN = @"v1_KeKhai_ThanNhan_Delete";
        private const string KE_KHAI_THANNHAN_GET_BY_CANBOID = @"v1_KeKhai_ThanNhan_GetByCanBoID";
        private const string KE_KHAI_THANNHAN_GET_PAGINGBYSEARCH = @"v1_KeKhai_ThanNhan_GetPagingBySearch";
        private const string KE_KHAI_THANNHAN_GET_ALL = @"v1_KeKhai_ThanNhan_GetAll";

        //Ten các params
        private const string THAN_NHAN_ID = "NV01001";
        private const string CAN_BO_ID_THANNHAN = "NV01002";
        private const string HO_TEN = "NV01003";
        private const string NAM_SINH = "NV01004";
        private const string HO_KHAU_THUONG_TRU = "NV01005";
        private const string CHO_O_HIEN_NAY = "NV01006";
        private const string CHUC_VU = "NV01007";
        private const string NOI_CONG_TAC = "NV01008";
        private const string QUAN_HE = "NV01009";
        private const string NGAY_SINH = "NV01010";
        private const string TRANG_THAI = "NV01011";
        private const string CMND = "NV01012";
        private const string NGAY_CAP = "NV01013";
        private const string NOI_CAP = "NV01014";

        public KeKhaiThanNhanModel GetByID(int ThanNhanID)
        {
            KeKhaiThanNhanModel ThanNhan = new KeKhaiThanNhanModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(THAN_NHAN_ID,SqlDbType.Int)
              };
            parameters[0].Value = ThanNhanID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_THANNHAN_GET_BY_THANNHANID, parameters))
                {
                    while (dr.Read())
                    {
                        ThanNhan = new KeKhaiThanNhanModel();
                        ThanNhan.ThanNhanID = Utils.ConvertToInt32(dr[THAN_NHAN_ID], 0);
                        ThanNhan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID_THANNHAN], 0);
                        ThanNhan.HoTen = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_TEN], string.Empty));
                        ThanNhan.NamSinh = Utils.ConvertToInt32(dr[NAM_SINH], 0);
                        ThanNhan.HoKhauThuongTru = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_KHAU_THUONG_TRU], string.Empty));
                        ThanNhan.ChoOHienNay = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHO_O_HIEN_NAY], string.Empty));
                        ThanNhan.ChucVu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHUC_VU], string.Empty));
                        ThanNhan.NoiCongTac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[NOI_CONG_TAC], string.Empty));
                        ThanNhan.QuanHe = Utils.ConvertToInt32(dr[QUAN_HE], 0);
                        ThanNhan.NgaySinh = Utils.ConvertToNullableDateTime(dr[NGAY_SINH], null);
                        ThanNhan.CMND = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CMND], string.Empty));
                        ThanNhan.NoiCap = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[NOI_CAP], string.Empty));
                        ThanNhan.NgayCap = Utils.ConvertToNullableDateTime(dr[NGAY_CAP], null);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return ThanNhan;
        }

        public BaseResultModel Insert(KeKhaiThanNhanModel KeKhaiThanNhanModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (!Utils.CheckSpecialCharacter(KeKhaiThanNhanModel.HoTen))
                {
                    Result.Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                    Result.Status = 0;
                    return Result;
                }
                else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen))
                {
                    Result.Message = "Họ tên không được trống!";
                    Result.Status = 0;
                    return Result;
                }
                else if (KeKhaiThanNhanModel.HoTen.Length > 100)
                {
                    Result.Message = "Họ tên không được quá 100 ký tự!";
                    Result.Status = 0;
                    return Result;
                }
                else if (KeKhaiThanNhanModel.NgaySinh == null)
                {
                    Result.Message = "Ngày sinh không được trống!";
                    Result.Status = 0;
                    return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac))
                {
                    Result.Message = "Nơi công tác không được trống!";
                    Result.Status = 0;
                    return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && KeKhaiThanNhanModel.NoiCongTac.Trim().Length > 255)
                {
                    Result.Message = "Nơi công tác không được quá 255 ký tự!";
                    Result.Status = 0;
                    return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu))
                {
                    Result.Message = "Chức vụ không được trống!";
                    Result.Status = 0;
                    return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && KeKhaiThanNhanModel.ChucVu.Trim().Length > 255)
                {
                    Result.Message = "Chức vụ không được quá 255 ký tự!";
                    Result.Status = 0; return Result;
                }
                //else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru))
                //{
                //    Result.Message = "Hộ khẩu thường trú không được trống!";
                //    Result.Status = 0;
                //    return Result;
                //}
                else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay))
                {
                    Result.Message = "Chỗ ở hiện nay không được để trống!";
                    Result.Status = 0;
                    return Result;
                }
                else
                {
                    var CanBo = new HeThongCanBoDAL().GetCanBoByID(KeKhaiThanNhanModel.CanBoID);
                    if (CanBo == null || CanBo.CanBoID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Cán bộ không tồn tại";
                        return Result;
                    }
                    else
                    {
                        // sét trạng thái cho vợ, chồng cũ
                        if (KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode())
                        {
                            var crVoChong = GetByCanBoID(KeKhaiThanNhanModel.CanBoID.Value).Where(x => (x.QuanHe == EnumQuanHe.Chong.GetHashCode() || x.QuanHe == EnumQuanHe.Vo.GetHashCode()) && (x.TrangThai == null || x.TrangThai == 0)).FirstOrDefault();
                            if (crVoChong != null && crVoChong.ThanNhanID > 0)
                            {
                                crVoChong.TrangThai = 1;
                                var updateTrangThaiVoChong = Update(crVoChong);
                                if (updateTrangThaiVoChong.Status < 1) return updateTrangThaiVoChong;
                            }
                        }

                        ////////////////////////////////
                        SqlParameter[] parameters = new SqlParameter[]
                       {
                              new SqlParameter(CAN_BO_ID_THANNHAN, SqlDbType.Int),
                              new SqlParameter(HO_TEN, SqlDbType.NVarChar),
                              new SqlParameter(NAM_SINH, SqlDbType.Int),
                              new SqlParameter(HO_KHAU_THUONG_TRU, SqlDbType.NText),
                              new SqlParameter(CHO_O_HIEN_NAY, SqlDbType.NText),
                              new SqlParameter(CHUC_VU, SqlDbType.NVarChar),
                              new SqlParameter(NOI_CONG_TAC, SqlDbType.NVarChar),
                              new SqlParameter(QUAN_HE, SqlDbType.Int),
                              new SqlParameter(NGAY_SINH, SqlDbType.DateTime2),
                              new SqlParameter(CMND, SqlDbType.NVarChar),
                              new SqlParameter(NGAY_CAP, SqlDbType.DateTime2),
                              new SqlParameter(NOI_CAP, SqlDbType.NVarChar),
                       };
                        parameters[0].Value = KeKhaiThanNhanModel.CanBoID ?? Convert.DBNull;
                        parameters[1].Value = KeKhaiThanNhanModel.HoTen == null ? Convert.DBNull : KeKhaiThanNhanModel.HoTen.Trim();
                        parameters[1].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.HoTen.Trim());
                        parameters[2].Value = KeKhaiThanNhanModel.NamSinh ?? Convert.DBNull;
                        parameters[3].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.HoKhauThuongTru.Trim());
                        parameters[4].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.ChoOHienNay.Trim());
                        parameters[5].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.ChucVu.Trim());
                        parameters[6].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.NoiCongTac.Trim());
                        parameters[7].Value = KeKhaiThanNhanModel.QuanHe ?? Convert.DBNull;
                        parameters[8].Value = (KeKhaiThanNhanModel.NgaySinh == null || KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd");
                        parameters[9].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.CMND) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.CMND.Trim());
                        parameters[10].Value = (KeKhaiThanNhanModel.NgayCap == null || KeKhaiThanNhanModel.NgayCap.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgayCap.Value.ToString("yyyy/MM/dd");
                        parameters[11].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCap) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.NoiCap.Trim());
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_KE_KHAI_THANNHAN, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Thân nhân");
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    return Result;
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
                return Result;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel InsertAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel)
        {
            var Result = new BaseResultModel();
            if (ListKeKhaiThanNhanModel == null)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng nhập thông tin trước khi thêm mới con chưa thành niên";
                return Result;
            }
            try
            {
                for (int i = 0; i < ListKeKhaiThanNhanModel.Count; i++)
                {
                    var crThanhNhan = ListKeKhaiThanNhanModel[i];

                    Result = Insert(crThanhNhan);
                    if (Result.Status < 1)
                    {
                        return Result;
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

        public BaseResultModel Update(KeKhaiThanNhanModel KeKhaiThanNhanModel)
        {

            var Result = new BaseResultModel();
            try
            {
                if (!Utils.CheckSpecialCharacter(KeKhaiThanNhanModel.HoTen))
                {
                    Result.Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                    Result.Status = 0; return Result;
                }
                else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen))
                {
                    Result.Message = "Họ tên không được trống!";
                    Result.Status = 0; return Result;
                }
                else if (KeKhaiThanNhanModel.HoTen.Length > 100)
                {
                    Result.Message = "Họ tên không được quá 100 ký tự!";
                    Result.Status = 0; return Result;
                }
                else if (KeKhaiThanNhanModel.NgaySinh == null)
                {
                    Result.Message = "Ngày sinh không được trống!";
                    Result.Status = 0; return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac))
                {
                    Result.Message = "Nơi công tác không được trống!";
                    Result.Status = 0; return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && KeKhaiThanNhanModel.NoiCongTac.Trim().Length > 255)
                {
                    Result.Message = "Nơi công tác không được quá 255 ký tự!";
                    Result.Status = 0; return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu))
                {
                    Result.Message = "Chức vụ không được trống!";
                    Result.Status = 0; return Result;
                }
                else if ((KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode()) && KeKhaiThanNhanModel.ChucVu.Trim().Length > 255)
                {
                    Result.Message = "Chức vụ không được quá 255 ký tự!";
                    Result.Status = 0; return Result;
                }
                //else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru))
                //{
                //    Result.Message = "Hộ khẩu thường trú không được trống!";
                //    Result.Status = 0; return Result;
                //}
                else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay))
                {
                    Result.Message = "Chỗ ở hiện nay không được để trống!";
                    Result.Status = 0; return Result;
                }
                else
                {
                    var ThanNhan = GetByID(KeKhaiThanNhanModel.ThanNhanID);
                    if (KeKhaiThanNhanModel.CanBoID != null && KeKhaiThanNhanModel.CanBoID > 0)
                    {
                        var CanBo = new HeThongCanBoDAL().GetCanBoByID(KeKhaiThanNhanModel.CanBoID);
                        if (CanBo == null || CanBo.CanBoID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ không tồn tại";
                            return Result;
                        }
                    }
                    if (ThanNhan == null || ThanNhan.ThanNhanID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Thân nhân không tồn tại";
                    }
                    else
                    {
                        // sét trạng thái cho vợ, chồng cũ
                        if (KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode())
                        {
                            var crVoChong = GetByCanBoID(KeKhaiThanNhanModel.CanBoID.Value).Where(x => (x.QuanHe == EnumQuanHe.Chong.GetHashCode() || x.QuanHe == EnumQuanHe.Vo.GetHashCode()) && (x.TrangThai == null || x.TrangThai == 0)).FirstOrDefault();
                            if (crVoChong != null && crVoChong.ThanNhanID > 0)
                            {
                                crVoChong.TrangThai = 1;
                                var updateTrangThaiVoChong = Update_VoChong(crVoChong);
                                if (updateTrangThaiVoChong.Status < 1) return updateTrangThaiVoChong;
                            }
                        }

                        SqlParameter[] parameters = new SqlParameter[]
                       {
                              new SqlParameter(CAN_BO_ID_THANNHAN, SqlDbType.Int),
                              new SqlParameter(HO_TEN, SqlDbType.NVarChar),
                              new SqlParameter(NAM_SINH, SqlDbType.Int),
                              new SqlParameter(HO_KHAU_THUONG_TRU, SqlDbType.NText),
                              new SqlParameter(CHO_O_HIEN_NAY, SqlDbType.NText),
                              new SqlParameter(CHUC_VU, SqlDbType.NVarChar),
                              new SqlParameter(NOI_CONG_TAC, SqlDbType.NVarChar),
                              new SqlParameter(QUAN_HE, SqlDbType.Int),
                              new SqlParameter(THAN_NHAN_ID, SqlDbType.Int),
                              new SqlParameter(NGAY_SINH, SqlDbType.DateTime2),
                              new SqlParameter(TRANG_THAI, SqlDbType.Int),
                              new SqlParameter(CMND, SqlDbType.NVarChar),
                              new SqlParameter(NGAY_CAP, SqlDbType.DateTime2),
                              new SqlParameter(NOI_CAP, SqlDbType.NVarChar)
                       };
                        parameters[0].Value = KeKhaiThanNhanModel.CanBoID ?? Convert.DBNull;
                        parameters[1].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.HoTen.Trim());
                        parameters[2].Value = KeKhaiThanNhanModel.NamSinh ?? Convert.DBNull;
                        parameters[3].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.HoKhauThuongTru.Trim());
                        parameters[4].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.ChoOHienNay.Trim());
                        parameters[5].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.ChucVu.Trim());
                        parameters[6].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.NoiCongTac.Trim());
                        parameters[7].Value = KeKhaiThanNhanModel.QuanHe ?? Convert.DBNull;
                        parameters[8].Value = KeKhaiThanNhanModel.ThanNhanID;
                        parameters[9].Value = (KeKhaiThanNhanModel.NgaySinh == null || KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd");
                        parameters[10].Value = KeKhaiThanNhanModel.TrangThai ?? Convert.DBNull;
                        parameters[11].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.CMND) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.CMND.Trim());
                        parameters[12].Value = (KeKhaiThanNhanModel.NgayCap == null || KeKhaiThanNhanModel.NgayCap.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgayCap.Value.ToString("yyyy/MM/dd");
                        parameters[13].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCap) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.NoiCap.Trim());

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_KE_KHAI_THANNHAN, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("Thân nhân");
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    return Result;
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
                return Result;
                throw ex;
            }
            return Result;

        }

        public BaseResultModel Update_VoChong(KeKhaiThanNhanModel KeKhaiThanNhanModel)
        {
            var Result = new BaseResultModel();
            try
            {
                // sét trạng thái cho vợ, chồng cũ
                if (KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Chong.GetHashCode() || KeKhaiThanNhanModel.QuanHe == EnumQuanHe.Vo.GetHashCode())
                {
                    var crVoChong = GetByCanBoID(KeKhaiThanNhanModel.CanBoID.Value).Where(x => (x.QuanHe == EnumQuanHe.Chong.GetHashCode() || x.QuanHe == EnumQuanHe.Vo.GetHashCode()) && (x.TrangThai == null || x.TrangThai == 0)).FirstOrDefault();
                    if (crVoChong != null && crVoChong.ThanNhanID > 0)
                    {
                        //crVoChong.TrangThai = 1;
                        SqlParameter[] parameters = new SqlParameter[]
                      {
                              new SqlParameter(CAN_BO_ID_THANNHAN, SqlDbType.Int),
                              new SqlParameter(HO_TEN, SqlDbType.NVarChar),
                              new SqlParameter(NAM_SINH, SqlDbType.Int),
                              new SqlParameter(HO_KHAU_THUONG_TRU, SqlDbType.NText),
                              new SqlParameter(CHO_O_HIEN_NAY, SqlDbType.NText),
                              new SqlParameter(CHUC_VU, SqlDbType.NVarChar),
                              new SqlParameter(NOI_CONG_TAC, SqlDbType.NVarChar),
                              new SqlParameter(QUAN_HE, SqlDbType.Int),
                              new SqlParameter(THAN_NHAN_ID, SqlDbType.Int),
                              new SqlParameter(NGAY_SINH, SqlDbType.DateTime2),
                              new SqlParameter(TRANG_THAI, SqlDbType.Int),
                              new SqlParameter(CMND, SqlDbType.NVarChar),
                              new SqlParameter(NGAY_CAP, SqlDbType.DateTime2),
                              new SqlParameter(NOI_CAP, SqlDbType.NVarChar)

                      };
                        parameters[0].Value = KeKhaiThanNhanModel.CanBoID ?? Convert.DBNull;
                        parameters[1].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.HoTen.Trim());
                        parameters[2].Value = KeKhaiThanNhanModel.NamSinh ?? Convert.DBNull;
                        parameters[3].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.HoKhauThuongTru.Trim());
                        parameters[4].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.ChoOHienNay.Trim());
                        parameters[5].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.ChucVu.Trim());
                        parameters[6].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.NoiCongTac.Trim());
                        parameters[7].Value = KeKhaiThanNhanModel.QuanHe ?? Convert.DBNull;
                        parameters[8].Value = KeKhaiThanNhanModel.ThanNhanID;
                        parameters[9].Value = (KeKhaiThanNhanModel.NgaySinh == null || KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd");
                        parameters[10].Value = 1;
                        parameters[11].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.CMND) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.CMND.Trim());
                        parameters[12].Value = (KeKhaiThanNhanModel.NgayCap == null || KeKhaiThanNhanModel.NgayCap.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgayCap.Value.ToString("yyyy/MM/dd");
                        parameters[13].Value = string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCap) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(KeKhaiThanNhanModel.NoiCap.Trim());

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_KE_KHAI_THANNHAN, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("Thân nhân");
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    return Result;
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
                return Result;
                throw ex;
            }
            return Result;

        }

        public BaseResultModel UpdateAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel)
        {
            var Result = new BaseResultModel();
            if (ListKeKhaiThanNhanModel == null)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng nhập thông tin trước khi thêm mới con chưa thành niên";
                return Result;
            }
            try
            {
                for (int i = 0; i < ListKeKhaiThanNhanModel.Count; i++)
                {
                    var crThanhNhan = ListKeKhaiThanNhanModel[i];

                    Result = Update(crThanhNhan);
                    if (Result.Status < 1)
                    {
                        return Result;
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

        public BaseResultModel Delete(List<int> ListThanNhanID)
        {
            var Result = new BaseResultModel();
            if (ListThanNhanID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                for (int i = 0; i < ListThanNhanID.Count; i++)
                {
                    int val = 0;
                    if (!int.TryParse(ListThanNhanID[i].ToString(), out val))
                    {
                        Result.Status = 0;
                        Result.Message = "ThanNhanID '" + ListThanNhanID[i].ToString() + "' không đúng định dạng";
                        return Result;
                    }
                    else
                    {
                        var crThanThan = GetByID(ListThanNhanID[i]);
                        if (crThanThan == null || crThanThan.ThanNhanID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "ThanNhanID '" + ListThanNhanID[i].ToString() + "' không tồn tại";
                            return Result;
                        }
                        else
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter(THAN_NHAN_ID, SqlDbType.Int)
                            };
                            parameters[0].Value = ListThanNhanID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_KE_KHAI_THANNHAN, parameters);
                                        trans.Commit();
                                        if (val < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Không thể xóa Thân nhân có TrangThaiID = " + ListThanNhanID[i];
                                            return Result;
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
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Danh mục trạng thái");
                return Result;
            }
        }

        public List<KeKhaiThanNhanModel> GetByCanBoID(int CanBoID)
        {
            var Result = new List<KeKhaiThanNhanModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(CAN_BO_ID_THANNHAN,SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_THANNHAN_GET_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var ThanNhan = new KeKhaiThanNhanModel();
                        ThanNhan.ThanNhanID = Utils.ConvertToInt32(dr[THAN_NHAN_ID], 0);
                        ThanNhan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID_THANNHAN], 0);
                        ThanNhan.HoTen = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_TEN], string.Empty));
                        ThanNhan.NamSinh = Utils.ConvertToInt32(dr[NAM_SINH], 0);
                        ThanNhan.HoKhauThuongTru = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_KHAU_THUONG_TRU], string.Empty));
                        ThanNhan.ChoOHienNay = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHO_O_HIEN_NAY], string.Empty));
                        ThanNhan.ChucVu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHUC_VU], string.Empty));
                        ThanNhan.NoiCongTac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[NOI_CONG_TAC], string.Empty));
                        ThanNhan.QuanHe = Utils.ConvertToInt32(dr[QUAN_HE], 0);
                        ThanNhan.NgaySinh = Utils.ConvertToNullableDateTime(dr[NGAY_SINH], null);
                        ThanNhan.TrangThai = Utils.ConvertToNullableInt32(dr[TRANG_THAI], null);
                        ThanNhan.CMND = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CMND], string.Empty));
                        ThanNhan.NgayCap = Utils.ConvertToNullableDateTime(dr[NGAY_CAP], null);
                        ThanNhan.NoiCap = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[NOI_CAP], string.Empty));
                        Result.Add(ThanNhan);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }

        public ThanNhanCanBoModel GetThanNhanCanBo_By_CanBoID(int CanBoID)
        {
            var Result = new ThanNhanCanBoModel();
            try
            {
                var DanhSachThanNhan = GetByCanBoID(CanBoID);
                Result.VoChong = DanhSachThanNhan.Where(x => x.QuanHe == EnumQuanHe.Vo.GetHashCode() || x.QuanHe == EnumQuanHe.Chong.GetHashCode() && (x.TrangThai == null || x.TrangThai == 0)).FirstOrDefault();
                if (Result.VoChong == null)
                {
                    Result.VoChong = new KeKhaiThanNhanModel();
                    Result.VoChong.TrangThai = null;
                }
                Result.ConChuaThanhNien = DanhSachThanNhan.Where(x => x.QuanHe == EnumQuanHe.ConChuaThanhNien.GetHashCode()).ToList();
            }
            catch
            {
                throw;
            }
            return Result;
        }

        public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow, int CanBoID, int CoQuanID, int NguoiDungID, int VaiTro)
        {
            List<KeKhaiThanNhanPartial_New> list = new List<KeKhaiThanNhanPartial_New>();
            var danhSachCoQuan = new DanhMucCoQuanDonViDAL().GetListByUser_Phang(CoQuanID, NguoiDungID);
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("ID", typeof(string));
            danhSachCoQuan.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter(CAN_BO_ID_THANNHAN,SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int),
                 pList,

              };
            parameters[0].Value = p.Keyword;
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = VaiTro == EnumVaiTroCanBo.ChuyenVien.GetHashCode() ? CanBoID : p.CanBoID ?? Convert.DBNull;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;
            parameters[7].Value = p.CoQuanID ?? Convert.DBNull;
            parameters[8].Value = tbCoQuanID;

            var QuyenCuaCanBo = new ChucNangDAL().GetListChucNangByNguoiDungID(NguoiDungID);
            if (QuyenCuaCanBo.Any(x => x.ChucNangID == ChucNangEnum.HeThong_QuanLy_CanBo.GetHashCode())) parameters[5].Value = p.CanBoID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KE_KHAI_THANNHAN_GET_PAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiThanNhanPartial_New ThanNhan = new KeKhaiThanNhanPartial_New();
                        ThanNhan.ThanNhanID = Utils.ConvertToInt32(dr[THAN_NHAN_ID], 0);//
                        ThanNhan.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        ThanNhan.HoTen = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_TEN], string.Empty));
                        ThanNhan.NamSinh = Utils.ConvertToInt32(dr[NAM_SINH], 0);//
                        ThanNhan.HoKhauThuongTru = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_KHAU_THUONG_TRU], string.Empty));
                        ThanNhan.ChoOHienNay = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHO_O_HIEN_NAY], string.Empty));
                        ThanNhan.ChucVu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHUC_VU], string.Empty));
                        ThanNhan.NoiCongTac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[NOI_CONG_TAC], string.Empty));
                        ThanNhan.QuanHe = Utils.ConvertToInt32(dr[QUAN_HE], 0);
                        ThanNhan.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);//
                        ThanNhan.NgaySinh = Utils.ConvertToNullableDateTime(dr[NGAY_SINH], null);
                        list.Add(ThanNhan);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        #region old code (trước ngày 04/12/2019 - thay đổi BA)

        //GetAll
        public List<KeKhaiThanNhanModel> GetAll()
        {
            List<KeKhaiThanNhanModel> list = new List<KeKhaiThanNhanModel>();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_THANNHAN_GET_ALL))
                {
                    while (dr.Read())
                    {
                        KeKhaiThanNhanModel ThanNhan = new KeKhaiThanNhanModel(
                            Utils.ConvertToInt32(dr[THAN_NHAN_ID], 0),
                            Utils.ConvertToInt32(dr[CAN_BO_ID_THANNHAN], 0),
                            Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_TEN], string.Empty)),
                            Utils.ConvertToInt32(dr[NAM_SINH], 0),
                            Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[HO_KHAU_THUONG_TRU], string.Empty)),
                            Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHO_O_HIEN_NAY], string.Empty)),
                            Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[CHUC_VU], string.Empty)),
                            Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[NOI_CONG_TAC], string.Empty)),
                            Utils.ConvertToInt32(dr[QUAN_HE], 0)
                        );
                        list.Add(ThanNhan);

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return list;
        }

        //public BaseResultModel Insert(KeKhaiThanNhanModel KeKhaiThanNhanModel, EnumQuanHe quanHe)
        //{
        //    var Result = new BaseResultModel();
        //    try
        //    {
        //        if (!Utils.CheckSpecialCharacter(KeKhaiThanNhanModel.HoTen))
        //        {
        //            Result.Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen))
        //        {
        //            Result.Message = "Họ tên không được trống!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (KeKhaiThanNhanModel.HoTen.Length > 100)
        //        {
        //            Result.Message = "Họ tên không được quá 100 ký tự!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && KeKhaiThanNhanModel.NamSinh == null)
        //        {
        //            Result.Message = "Năm sinh không được trống!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac))
        //        {
        //            Result.Message = "Nơi công tác không được trống!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && KeKhaiThanNhanModel.NoiCongTac.Trim().Length > 255)
        //        {
        //            Result.Message = "Nơi công tác không được quá 255 ký tự!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu))
        //        {
        //            Result.Message = "Chức vụ không được trống!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && KeKhaiThanNhanModel.ChucVu.Trim().Length > 255)
        //        {
        //            Result.Message = "Chức vụ không được quá 255 ký tự!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru))
        //        {
        //            Result.Message = "Hộ khẩu thường trú không được trống!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay))
        //        {
        //            Result.Message = "Chỗ ở hiện nay không được để trống!";
        //            Result.Status = 0;
        //            return Result;
        //        }
        //        else
        //        {
        //            var CanBo = new HeThongCanBoDAL().GetCanBoByID(KeKhaiThanNhanModel.CanBoID);
        //            if (CanBo == null || CanBo.CanBoID < 1)
        //            {
        //                Result.Status = 0;
        //                Result.Message = "Cán bộ không tồn tại";
        //                return Result;
        //            }
        //            else
        //            {
        //                SqlParameter[] parameters = new SqlParameter[]
        //               {
        //                      new SqlParameter(CAN_BO_ID_THANNHAN, SqlDbType.Int),
        //                      new SqlParameter(HO_TEN, SqlDbType.NVarChar),
        //                      new SqlParameter(NAM_SINH, SqlDbType.Int),
        //                      new SqlParameter(HO_KHAU_THUONG_TRU, SqlDbType.NText),
        //                      new SqlParameter(CHO_O_HIEN_NAY, SqlDbType.NText),
        //                      new SqlParameter(CHUC_VU, SqlDbType.NVarChar),
        //                      new SqlParameter(NOI_CONG_TAC, SqlDbType.NVarChar),
        //                      new SqlParameter(QUAN_HE, SqlDbType.Int),
        //                      new SqlParameter(NGAY_SINH, SqlDbType.DateTime2),
        //               };
        //                parameters[0].Value = KeKhaiThanNhanModel.CanBoID ?? Convert.DBNull;
        //                parameters[1].Value = KeKhaiThanNhanModel.HoTen == null ? Convert.DBNull : KeKhaiThanNhanModel.HoTen.Trim();
        //                parameters[2].Value = KeKhaiThanNhanModel.NamSinh ?? Convert.DBNull;
        //                parameters[3].Value = KeKhaiThanNhanModel.HoKhauThuongTru == null ? Convert.DBNull : KeKhaiThanNhanModel.HoKhauThuongTru.Trim();
        //                parameters[4].Value = KeKhaiThanNhanModel.ChoOHienNay == null ? Convert.DBNull : KeKhaiThanNhanModel.ChoOHienNay.Trim();
        //                parameters[5].Value = KeKhaiThanNhanModel.ChucVu == null ? Convert.DBNull : KeKhaiThanNhanModel.ChucVu.Trim();
        //                parameters[6].Value = KeKhaiThanNhanModel.NoiCongTac == null ? Convert.DBNull : KeKhaiThanNhanModel.NoiCongTac.Trim();
        //                parameters[7].Value = quanHe.GetHashCode();
        //                //parameters[8].Value = KeKhaiThanNhanModel.NgaySinh != null ? Utils.ConvertToDateTime(KeKhaiThanNhanModel.NgaySinh, DateTime.Now) : Convert.DBNull;
        //                parameters[8].Value = (KeKhaiThanNhanModel.NgaySinh == null || KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd");
        //                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
        //                {
        //                    conn.Open();
        //                    using (SqlTransaction trans = conn.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_KeKhai_ThanNhan_Insert", parameters);
        //                            trans.Commit();
        //                            Result.Message = ConstantLogMessage.Alert_Insert_Success("Thân nhân");
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Result.Status = -1;
        //                            Result.Message = ConstantLogMessage.API_Error_System;
        //                            trans.Rollback();
        //                            return Result;
        //                            throw ex;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Status = -1;
        //        Result.Message = ConstantLogMessage.API_Error_System;
        //        return Result;
        //        throw ex;
        //    }
        //    return Result;
        //}

        //public BaseResultModel InsertAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel, EnumQuanHe quanHe)
        //{
        //    var Result = new BaseResultModel();
        //    if (ListKeKhaiThanNhanModel == null)
        //    {
        //        Result.Status = 0;
        //        Result.Message = "Vui lòng nhập thông tin trước khi thêm mới con chưa thành niên";
        //        return Result;
        //    }
        //    try
        //    {
        //        for (int i = 0; i < ListKeKhaiThanNhanModel.Count; i++)
        //        {
        //            var crThanhNhan = ListKeKhaiThanNhanModel[i];

        //            Result = Insert(crThanhNhan, quanHe);
        //            if (Result.Status < 1)
        //            {
        //                return Result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Status = -1;
        //        Result.Message = ConstantLogMessage.API_Error_System;
        //        throw ex;
        //    }
        //    return Result;
        //}


        //public BaseResultModel Update(KeKhaiThanNhanModel KeKhaiThanNhanModel, EnumQuanHe quanHe)
        //{

        //    var Result = new BaseResultModel();
        //    try
        //    {
        //        if (!Utils.CheckSpecialCharacter(KeKhaiThanNhanModel.HoTen))
        //        {
        //            Result.Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
        //            Result.Status = 0; return Result;
        //        }
        //        else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoTen))
        //        {
        //            Result.Message = "Họ tên không được trống!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (KeKhaiThanNhanModel.HoTen.Length > 100)
        //        {
        //            Result.Message = "Họ tên không được quá 100 ký tự!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && KeKhaiThanNhanModel.NamSinh == null)
        //        {
        //            Result.Message = "Năm sinh không được trống!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && string.IsNullOrEmpty(KeKhaiThanNhanModel.NoiCongTac))
        //        {
        //            Result.Message = "Nơi công tác không được trống!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && KeKhaiThanNhanModel.NoiCongTac.Trim().Length > 255)
        //        {
        //            Result.Message = "Nơi công tác không được quá 255 ký tự!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && string.IsNullOrEmpty(KeKhaiThanNhanModel.ChucVu))
        //        {
        //            Result.Message = "Chức vụ không được trống!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (quanHe == EnumQuanHe.Vo && KeKhaiThanNhanModel.ChucVu.Trim().Length > 255)
        //        {
        //            Result.Message = "Chức vụ không được quá 255 ký tự!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.HoKhauThuongTru))
        //        {
        //            Result.Message = "Hộ khẩu thường trú không được trống!";
        //            Result.Status = 0; return Result;
        //        }
        //        else if (string.IsNullOrEmpty(KeKhaiThanNhanModel.ChoOHienNay))
        //        {
        //            Result.Message = "Chỗ ở hiện nay không được để trống!";
        //            Result.Status = 0; return Result;
        //        }
        //        else
        //        {
        //            var ThanNhan = GetByID(KeKhaiThanNhanModel.ThanNhanID);
        //            if (KeKhaiThanNhanModel.CanBoID != null && KeKhaiThanNhanModel.CanBoID > 0)
        //            {
        //                var CanBo = new HeThongCanBoDAL().GetCanBoByID(KeKhaiThanNhanModel.CanBoID);
        //                if (CanBo == null || CanBo.CanBoID < 1)
        //                {
        //                    Result.Status = 0;
        //                    Result.Message = "Cán bộ không tồn tại";
        //                    return Result;
        //                }
        //            }
        //            if (ThanNhan == null || ThanNhan.ThanNhanID < 1)
        //            {
        //                Result.Status = 0;
        //                Result.Message = "Thân nhân không tồn tại";
        //            }
        //            else
        //            {
        //                SqlParameter[] parameters = new SqlParameter[]
        //               {
        //                      new SqlParameter(CAN_BO_ID_THANNHAN, SqlDbType.Int),
        //                      new SqlParameter(HO_TEN, SqlDbType.NVarChar),
        //                      new SqlParameter(NAM_SINH, SqlDbType.Int),
        //                      new SqlParameter(HO_KHAU_THUONG_TRU, SqlDbType.NText),
        //                      new SqlParameter(CHO_O_HIEN_NAY, SqlDbType.NText),
        //                      new SqlParameter(CHUC_VU, SqlDbType.NVarChar),
        //                      new SqlParameter(NOI_CONG_TAC, SqlDbType.NVarChar),
        //                      new SqlParameter(QUAN_HE, SqlDbType.Int),
        //                    new SqlParameter(THAN_NHAN_ID, SqlDbType.Int),
        //                     new SqlParameter(NGAY_SINH, SqlDbType.DateTime2)
        //               };
        //                parameters[0].Value = KeKhaiThanNhanModel.CanBoID ?? Convert.DBNull;
        //                parameters[1].Value = KeKhaiThanNhanModel.HoTen == null ? Convert.DBNull : KeKhaiThanNhanModel.HoTen.Trim();
        //                parameters[2].Value = KeKhaiThanNhanModel.NamSinh ?? Convert.DBNull;
        //                parameters[3].Value = KeKhaiThanNhanModel.HoKhauThuongTru == null ? Convert.DBNull : KeKhaiThanNhanModel.HoKhauThuongTru.Trim();
        //                parameters[4].Value = KeKhaiThanNhanModel.ChoOHienNay == null ? Convert.DBNull : KeKhaiThanNhanModel.ChoOHienNay.Trim();
        //                parameters[5].Value = KeKhaiThanNhanModel.ChucVu == null ? Convert.DBNull : KeKhaiThanNhanModel.ChucVu.Trim();
        //                parameters[6].Value = KeKhaiThanNhanModel.NoiCongTac == null ? Convert.DBNull : KeKhaiThanNhanModel.NoiCongTac.Trim();
        //                parameters[7].Value = quanHe.GetHashCode();
        //                parameters[8].Value = KeKhaiThanNhanModel.ThanNhanID;
        //                parameters[9].Value = (KeKhaiThanNhanModel.NgaySinh == null || KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd") == "") ? Convert.DBNull : KeKhaiThanNhanModel.NgaySinh.Value.ToString("yyyy/MM/dd");

        //                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
        //                {
        //                    conn.Open();
        //                    using (SqlTransaction trans = conn.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_KeKhai_ThanNhan_Update", parameters);
        //                            trans.Commit();
        //                            Result.Message = ConstantLogMessage.Alert_Update_Success("Thân nhân");
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Result.Status = -1;
        //                            Result.Message = ConstantLogMessage.API_Error_System;
        //                            trans.Rollback();
        //                            return Result;
        //                            throw ex;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Status = -1;
        //        Result.Message = ConstantLogMessage.API_Error_System;
        //        return Result;
        //        throw ex;
        //    }
        //    return Result;

        //}

        //public BaseResultModel UpdateAll(List<KeKhaiThanNhanModel> ListKeKhaiThanNhanModel, EnumQuanHe quanHe)
        //{
        //    var Result = new BaseResultModel();
        //    if (ListKeKhaiThanNhanModel == null)
        //    {
        //        Result.Status = 0;
        //        Result.Message = "Vui lòng nhập thông tin trước khi thêm mới con chưa thành niên";
        //        return Result;
        //    }
        //    try
        //    {
        //        for (int i = 0; i < ListKeKhaiThanNhanModel.Count; i++)
        //        {
        //            var crThanhNhan = ListKeKhaiThanNhanModel[i];

        //            Result = Update(crThanhNhan, quanHe);
        //            if (Result.Status < 1)
        //            {
        //                return Result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Status = -1;
        //        Result.Message = ConstantLogMessage.API_Error_System;
        //        throw ex;
        //    }
        //    return Result;
        //}     

        //public List<KeKhaiThanNhanPartial_New> GetPagingBySearch(BasePagingParamsForFilter p, EnumQuanHe quanHe, ref int TotalRow)
        //{
        //    List<KeKhaiThanNhanPartial_New> list = new List<KeKhaiThanNhanPartial_New>();
        //    SqlParameter[] parameters = new SqlParameter[]
        //      {
        //        new SqlParameter("@Keyword",SqlDbType.NVarChar),
        //        new SqlParameter("@OrderByName",SqlDbType.NVarChar),
        //        new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
        //        new SqlParameter("@pLimit",SqlDbType.Int),
        //        new SqlParameter("@pOffset",SqlDbType.Int),
        //        new SqlParameter(CAN_BO_ID_THANNHAN,SqlDbType.Int),
        //        new SqlParameter("@TotalRow",SqlDbType.Int),
        //        new SqlParameter(QUAN_HE,SqlDbType.Int),
        //        new SqlParameter("@CoQuanID",SqlDbType.Int)

        //      };
        //    parameters[0].Value = p.Keyword;
        //    parameters[1].Value = p.OrderByName;
        //    parameters[2].Value = p.OrderByOption;
        //    parameters[3].Value = p.Limit;
        //    parameters[4].Value = p.Offset;
        //    parameters[5].Value = p.CanBoID ?? Convert.DBNull;
        //    parameters[6].Direction = ParameterDirection.Output;
        //    parameters[6].Size = 8;
        //    parameters[7].Value = quanHe.GetHashCode();
        //    parameters[8].Value = p.CoQuanID ?? Convert.DBNull;

        //    try
        //    {
        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_KeKhai_ThanNhan_GetPagingBySearch", parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                KeKhaiThanNhanPartial_New ThanNhan = new KeKhaiThanNhanPartial_New();
        //                ThanNhan.ThanNhanID = Utils.ConvertToInt32(dr[THAN_NHAN_ID], 0);//
        //                ThanNhan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID_THANNHAN], 0);
        //                ThanNhan.HoTen = Utils.ConvertToString(dr[HO_TEN], string.Empty);//
        //                ThanNhan.NamSinh = Utils.ConvertToInt32(dr[NAM_SINH], 0);//
        //                ThanNhan.HoKhauThuongTru = Utils.ConvertToString(dr[HO_KHAU_THUONG_TRU], string.Empty);//
        //                ThanNhan.ChoOHienNay = Utils.ConvertToString(dr[CHO_O_HIEN_NAY], string.Empty);//
        //                ThanNhan.ChucVu = Utils.ConvertToString(dr[CHUC_VU], string.Empty);//
        //                ThanNhan.NoiCongTac = Utils.ConvertToString(dr[NOI_CONG_TAC], string.Empty);//
        //                ThanNhan.QuanHe = Utils.ConvertToInt32(dr[QUAN_HE], 0);
        //                ThanNhan.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);//
        //                ThanNhan.NgaySinh = Utils.ConvertToNullableDateTime(dr[NGAY_SINH], null);
        //                list.Add(ThanNhan);
        //            }
        //            dr.Close();
        //        }
        //        TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return list;
        //}


        #endregion

    }
}
