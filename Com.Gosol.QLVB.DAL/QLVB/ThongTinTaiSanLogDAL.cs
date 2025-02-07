using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IThongTinTaiSanLogDAL
    {
        public int InsertEncrypt(ThongTinTaiSanLogModel ThongTinTaiSanLogModel, int ThaoTac, int? LanGui);
        public int Insert(ThongTinTaiSanLogModel ThongTinTaiSanLogModel, int ThaoTac, int? LanGui);

        public List<TaiSanLogModel> GetThongtinTaiSanLogByKeKhaiID(int KeKhaiID);

    }
    public class ThongTinTaiSanLogDAL : IThongTinTaiSanLogDAL
    {
        //tên các store procedure
        private const string INSERT_THONG_TIN_TAI_SAN_LOG = @"v1_KeKhai_ThongTinTaiSanLog_Insert";
        private const string KE_KHAI_THONG_TIN_TAI_SAN_LOG_GET_BY_KEKHAIID = @"v1_KeKhai_ThongTinTaiSanLog_GetByKeKhaiID";

        //Ten các params
        private const string THONG_TIN_TAI_SAN_LOG_ID = "NV01101";
        private const string THONG_TIN_TAI_SAN_ID_THONGTINTAISANLOG = "NV01102";
        private const string KE_KHAI_ID_THONGTINTAISANLOG = "NV01103";
        private const string NHOM_TAI_SAN_ID_THONGTINTAISANLOG = "NV01104";
        private const string TEN_TAI_SAN_CU_THONGTINTAISANLOG = "NV01105";
        private const string DIEN_TICH_CU_THONGTINTAISANLOG = "NV01106";
        private const string GIA_TRI_CU_THONGTINTAISANLOG = "NV01107";
        private const string GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_CU_THONGTINTAISANLOG = "NV01108";
        private const string LOAI_TAI_SAN_ID_CU_THONGTINTAISANLOG = "NV01109";
        private const string CAP_CONG_TRINH_ID_THONGTINTAISANLOG = "NV01110";
        private const string GIAI_TRINH_NGUON_GOC_CU_THONGTINTAISANLOG = "NV01111";
        private const string THONG_TIN_KHAC_CU_THONGTINTAISANLOG = "NV01112";
        private const string CAN_BO_ID_THONGTINTAISANLOG = "NV01113";
        private const string NGUOI_DUNG_TEN_CU_THONGTINTAISANLOG = "NV01114";
        private const string NGUOI_DUNG_TEN_LA_CAN_BO_CU_THONGTINTAISANLOG = "NV01115";
        private const string NAM_KE_KHAI_THONGTINTAISANLOG = "NV01116";
        private const string SO_LUONG_CU_THONGTINTAISANLOG = "NV01117";
        private const string NHOM_TAI_SAN_CON_THONGTINTAISANLOG = "NV01118";
        private const string DIA_CHI_CU_THONGTINTAISANLOG = "NV01119";
        private const string NGAY_CAP_NHAT_THONGTINTAISANLOG = "NV01120";
        private const string THAO_TAC_THONGTINTAISANLOG = "NV01121";
        private const string LAN_GUI_THONGTINTAISANLOG = "NV01122";
        private const string TEN_TAI_SAN_MOI_THONGTINTAISANLOG = "NV01105_1";
        private const string DIEN_TICH_MOI_THONGTINTAISANLOG = "NV01106_1";
        private const string GIA_TRI_MOI_THONGTINTAISANLOG = "NV01107_1";
        private const string GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_MOI_THONGTINTAISANLOG = "NV01108_1";
        private const string LOAI_TAI_SAN_ID_MOI_THONGTINTAISANLOG = "NV01109_1";
        private const string GIAI_TRINH_NGUON_GOC_MOI_THONGTINTAISANLOG = "NV01111_1";
        private const string THONG_TIN_KHAC_MOI_THONGTINTAISANLOG = "NV01112_1";
        private const string NGUOI_DUNG_TEN_MOI_THONGTINTAISANLOG = "NV01114_1";
        private const string NGUOI_DUNG_TEN_LA_CAN_BO_MOI_THONGTINTAISANLOG = "NV01115_1";
        private const string SO_LUONG_MOI_THONGTINTAISANLOG = "NV01117_1";
        private const string DIA_CHI_MOI_THONGTINTAISANLOG = "NV01119_1";

        public int InsertEncrypt(ThongTinTaiSanLogModel ThongTinTaiSanLogModel, int ThaoTac, int? LanGui)
        {
            var result = 0;
            SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter(THONG_TIN_TAI_SAN_ID_THONGTINTAISANLOG,SqlDbType.Int),//NV01102
                        new SqlParameter(KE_KHAI_ID_THONGTINTAISANLOG,SqlDbType.Int),//NV01103
                        new SqlParameter(NHOM_TAI_SAN_ID_THONGTINTAISANLOG,SqlDbType.Int),//NV01104
                        new SqlParameter(TEN_TAI_SAN_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01105
                        new SqlParameter(DIEN_TICH_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01106
                        new SqlParameter(GIA_TRI_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01107
                        new SqlParameter(GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01108
                        new SqlParameter(LOAI_TAI_SAN_ID_CU_THONGTINTAISANLOG,SqlDbType.Int),//NV01109
                        new SqlParameter(CAP_CONG_TRINH_ID_THONGTINTAISANLOG,SqlDbType.Int),//NV01110
                        new SqlParameter(GIAI_TRINH_NGUON_GOC_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01111
                        new SqlParameter(THONG_TIN_KHAC_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01112
                        new SqlParameter(CAN_BO_ID_THONGTINTAISANLOG,SqlDbType.Int),//NV01113
                        new SqlParameter(NGUOI_DUNG_TEN_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01114
                        new SqlParameter(NGUOI_DUNG_TEN_LA_CAN_BO_CU_THONGTINTAISANLOG,SqlDbType.Bit),//NV01115
                        new SqlParameter(NAM_KE_KHAI_THONGTINTAISANLOG,SqlDbType.Int),//NV01116
                        new SqlParameter(SO_LUONG_CU_THONGTINTAISANLOG, SqlDbType.NVarChar),//NV001117
                        new SqlParameter(NHOM_TAI_SAN_CON_THONGTINTAISANLOG, SqlDbType.Int),//NV01118
                        new SqlParameter(DIA_CHI_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01119
                        new SqlParameter(NGAY_CAP_NHAT_THONGTINTAISANLOG,SqlDbType.BigInt),//NV01120
                        new SqlParameter(THAO_TAC_THONGTINTAISANLOG,SqlDbType.Int),//NV01121
                        new SqlParameter(LAN_GUI_THONGTINTAISANLOG,SqlDbType.Int),//NV01122
                        new SqlParameter(TEN_TAI_SAN_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01105_1
                        new SqlParameter(DIEN_TICH_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01106_1
                        new SqlParameter(GIA_TRI_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01107_1
                        new SqlParameter(GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01108_1
                        new SqlParameter(THONG_TIN_KHAC_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01112_1
                        new SqlParameter(NGUOI_DUNG_TEN_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01114_1
                        new SqlParameter(SO_LUONG_MOI_THONGTINTAISANLOG, SqlDbType.NVarChar),//NV001117_1
                        new SqlParameter(DIA_CHI_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01119_1
                        new SqlParameter(GIAI_TRINH_NGUON_GOC_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01111_1
                        new SqlParameter(LOAI_TAI_SAN_ID_MOI_THONGTINTAISANLOG,SqlDbType.Int),//NV01109_1
                        new SqlParameter(NGUOI_DUNG_TEN_LA_CAN_BO_MOI_THONGTINTAISANLOG,SqlDbType.Bit),//NV01115_1
                };
            parameters[0].Value = ThongTinTaiSanLogModel.ThongTinTaiSanID;
            parameters[1].Value = ThongTinTaiSanLogModel.KeKhaiID ?? Convert.DBNull;
            parameters[2].Value = ThongTinTaiSanLogModel.NhomTaiSanID ?? Convert.DBNull;
            parameters[3].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.TenTaiSanCu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.TenTaiSanCu);
            parameters[4].Value = Utils.ConvertToIntDouble(ThongTinTaiSanLogModel.DienTichCu, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.DienTichCu.ToString());
            parameters[5].Value = Utils.ConvertToIntDouble(ThongTinTaiSanLogModel.GiaTriCu, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.GiaTriCu.ToString());
            parameters[6].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungCu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungCu);
            parameters[7].Value = (ThongTinTaiSanLogModel.LoaiTaiSanIDCu is null || ThongTinTaiSanLogModel.LoaiTaiSanIDCu == 0) ? Convert.DBNull : ThongTinTaiSanLogModel.LoaiTaiSanIDCu;
            parameters[8].Value = (ThongTinTaiSanLogModel.CapCongTrinhID is null || ThongTinTaiSanLogModel.CapCongTrinhID == 0) ? Convert.DBNull : ThongTinTaiSanLogModel.CapCongTrinhID;
            parameters[9].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaiTrinhNguonGocCu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.GiaiTrinhNguonGocCu);
            parameters[10].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.ThongTinKhacCu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.ThongTinKhacCu);
            parameters[11].Value = ThongTinTaiSanLogModel.CanBoID is null ? Convert.DBNull : ThongTinTaiSanLogModel.CanBoID.Value;
            parameters[12].Value = Utils.ConvertToInt32(ThongTinTaiSanLogModel.NguoiDungTenCu, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.NguoiDungTenCu.ToString());
            parameters[13].Value = ThongTinTaiSanLogModel.NguoiDungTenLaCanBoCu is null ? Convert.DBNull : ThongTinTaiSanLogModel.NguoiDungTenLaCanBoCu.Value;
            parameters[14].Value = ThongTinTaiSanLogModel.NamKeKhai;
            parameters[15].Value = Utils.ConvertToInt32(ThongTinTaiSanLogModel.SoLuongCu, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.SoLuongCu.ToString());
            parameters[16].Value = (ThongTinTaiSanLogModel.NhomTaiSanConID is null || ThongTinTaiSanLogModel.NhomTaiSanConID == 0) ? Convert.DBNull : ThongTinTaiSanLogModel.NhomTaiSanConID;
            parameters[17].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DiaChiCu) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.DiaChiCu);
            parameters[18].Value = DateTime.Now.Ticks;
            parameters[19].Value = ThaoTac;
            parameters[20].Value = LanGui is null ? Convert.DBNull : LanGui.Value;
            parameters[21].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.TenTaiSanMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.TenTaiSanMoi);
            parameters[22].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DienTichMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.DienTichMoi);
            parameters[23].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaTriMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.GiaTriMoi);
            parameters[24].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungMoi);
            parameters[25].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.ThongTinKhacMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.ThongTinKhacMoi);
            parameters[26].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.NguoiDungTenMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.NguoiDungTenMoi);
            parameters[27].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.SoLuongMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.SoLuongMoi);
            parameters[28].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DiaChiMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.DiaChiMoi);
            parameters[28].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaiTrinhNguonGocMoi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanLogModel.GiaiTrinhNguonGocMoi);
            parameters[30].Value = (ThongTinTaiSanLogModel.LoaiTaiSanIDMoi is null || ThongTinTaiSanLogModel.LoaiTaiSanIDMoi == 0) ? Convert.DBNull : ThongTinTaiSanLogModel.LoaiTaiSanIDMoi;
            parameters[31].Value = (ThongTinTaiSanLogModel.NguoiDungTenLaCanBoMoi is null) ? Convert.DBNull : ThongTinTaiSanLogModel.LoaiTaiSanIDMoi;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_THONG_TIN_TAI_SAN_LOG, parameters);
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

        public int Insert(ThongTinTaiSanLogModel ThongTinTaiSanLogModel, int ThaoTac, int? LanGui)
        {
            var result = 0;
            SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter(THONG_TIN_TAI_SAN_ID_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(KE_KHAI_ID_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(NHOM_TAI_SAN_ID_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(TEN_TAI_SAN_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(DIEN_TICH_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(GIA_TRI_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(LOAI_TAI_SAN_ID_CU_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(CAP_CONG_TRINH_ID_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(GIAI_TRINH_NGUON_GOC_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(THONG_TIN_KHAC_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(CAN_BO_ID_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(NGUOI_DUNG_TEN_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(NGUOI_DUNG_TEN_LA_CAN_BO_CU_THONGTINTAISANLOG,SqlDbType.Bit),
                        new SqlParameter(NAM_KE_KHAI_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(SO_LUONG_CU_THONGTINTAISANLOG, SqlDbType.NVarChar),
                        new SqlParameter(NHOM_TAI_SAN_CON_THONGTINTAISANLOG, SqlDbType.Int),
                        new SqlParameter(DIA_CHI_CU_THONGTINTAISANLOG,SqlDbType.NVarChar),
                        new SqlParameter(NGAY_CAP_NHAT_THONGTINTAISANLOG,SqlDbType.BigInt),
                        new SqlParameter(THAO_TAC_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(LAN_GUI_THONGTINTAISANLOG,SqlDbType.Int),
                        new SqlParameter(TEN_TAI_SAN_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01105_1
                        new SqlParameter(DIEN_TICH_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01106_1
                        new SqlParameter(GIA_TRI_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01107_1
                        new SqlParameter(GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01108_1
                        new SqlParameter(THONG_TIN_KHAC_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01112_1
                        new SqlParameter(NGUOI_DUNG_TEN_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01114_1
                        new SqlParameter(SO_LUONG_MOI_THONGTINTAISANLOG, SqlDbType.NVarChar),//NV001117_1
                        new SqlParameter(DIA_CHI_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01119_1
                        new SqlParameter(GIAI_TRINH_NGUON_GOC_MOI_THONGTINTAISANLOG,SqlDbType.NVarChar),//NV01111_1
                        new SqlParameter(LOAI_TAI_SAN_ID_MOI_THONGTINTAISANLOG,SqlDbType.Int),//NV01109_1
                        new SqlParameter(NGUOI_DUNG_TEN_LA_CAN_BO_MOI_THONGTINTAISANLOG,SqlDbType.Bit),//NV01115_1

                };
            parameters[0].Value = ThongTinTaiSanLogModel.ThongTinTaiSanID;
            parameters[1].Value = ThongTinTaiSanLogModel.KeKhaiID ?? Convert.DBNull;
            parameters[2].Value = ThongTinTaiSanLogModel.NhomTaiSanID ?? Convert.DBNull;
            parameters[3].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.TenTaiSanCu) ? Convert.DBNull : ThongTinTaiSanLogModel.TenTaiSanCu;
            parameters[4].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DienTichCu) ? Convert.DBNull : ThongTinTaiSanLogModel.DienTichCu;
            parameters[5].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaTriCu) ? Convert.DBNull : ThongTinTaiSanLogModel.GiaTriCu;
            parameters[6].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungCu) ? Convert.DBNull : ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungCu;
            parameters[7].Value = (ThongTinTaiSanLogModel.LoaiTaiSanIDCu == 0 || ThongTinTaiSanLogModel.LoaiTaiSanIDCu is null) ? Convert.DBNull : ThongTinTaiSanLogModel.LoaiTaiSanIDCu;
            parameters[8].Value = ThongTinTaiSanLogModel.CapCongTrinhID == 0 ? Convert.DBNull : ThongTinTaiSanLogModel.CapCongTrinhID;
            parameters[9].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaiTrinhNguonGocCu) ? Convert.DBNull : ThongTinTaiSanLogModel.GiaiTrinhNguonGocCu;
            parameters[10].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.ThongTinKhacCu) ? Convert.DBNull : ThongTinTaiSanLogModel.ThongTinKhacCu;
            parameters[11].Value = ThongTinTaiSanLogModel.CanBoID == 0 ? Convert.DBNull : ThongTinTaiSanLogModel.CanBoID;
            parameters[12].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.NguoiDungTenCu) ? Convert.DBNull : ThongTinTaiSanLogModel.NguoiDungTenCu;
            parameters[13].Value = ThongTinTaiSanLogModel.NguoiDungTenLaCanBoCu ?? Convert.DBNull;
            parameters[14].Value = ThongTinTaiSanLogModel.NamKeKhai;
            parameters[15].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.SoLuongCu) ? Convert.DBNull : ThongTinTaiSanLogModel.SoLuongCu;
            parameters[16].Value = ThongTinTaiSanLogModel.NhomTaiSanConID == 0 ? Convert.DBNull : ThongTinTaiSanLogModel.NhomTaiSanConID;
            parameters[17].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DiaChiCu) ? Convert.DBNull : ThongTinTaiSanLogModel.DiaChiCu;
            parameters[18].Value = DateTime.Now.Ticks;
            parameters[19].Value = ThaoTac;
            parameters[20].Value = LanGui ?? Convert.DBNull;
            parameters[21].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.TenTaiSanMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.TenTaiSanMoi;
            parameters[22].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DienTichMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.DienTichMoi;
            parameters[23].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaTriMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.GiaTriMoi;
            parameters[24].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.GiayChungNhanQuyenSuDungMoi;
            parameters[25].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.ThongTinKhacMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.ThongTinKhacMoi;
            parameters[26].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.NguoiDungTenMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.NguoiDungTenMoi;
            parameters[27].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.SoLuongMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.SoLuongMoi;
            parameters[28].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.DiaChiMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.DiaChiMoi;
            parameters[29].Value = string.IsNullOrEmpty(ThongTinTaiSanLogModel.GiaiTrinhNguonGocMoi) ? Convert.DBNull : ThongTinTaiSanLogModel.GiaiTrinhNguonGocMoi;
            parameters[30].Value = (ThongTinTaiSanLogModel.LoaiTaiSanIDMoi == 0 || ThongTinTaiSanLogModel.LoaiTaiSanIDMoi is null) ? Convert.DBNull : ThongTinTaiSanLogModel.LoaiTaiSanIDMoi;
            parameters[31].Value = ThongTinTaiSanLogModel.NguoiDungTenLaCanBoMoi ?? Convert.DBNull;


            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_THONG_TIN_TAI_SAN_LOG, parameters);
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

        public List<TaiSanLogModel> GetThongtinTaiSanLogByKeKhaiID(int KeKhaiID)
        {
            var Result = new List<TaiSanLogModel>();
            //List<ThongTinTaiSanLogModel> lstThongtinTaiSanLog = new List<ThongTinTaiSanLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(KE_KHAI_ID_THONGTINTAISANLOG, SqlDbType.Int),
            };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_THONG_TIN_TAI_SAN_LOG_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {

                        var crThongTinTaiSan = new ThongTinTaiSanLogModel();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.GiaTriCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.GiaTriMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.DienTichCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.DienTichMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.GiaiTrinhNguonGocCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.GiaiTrinhNguonGocMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.GiayChungNhanQuyenSuDungCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.GiayChungNhanQuyenSuDungMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.LoaiTaiSanIDCu = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID_CU_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.LoaiTaiSanIDMoi = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID_MOI_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.NguoiDungTenCu = Utils.ConvertToString(dr["NguoiDungTenCu"], string.Empty);
                        crThongTinTaiSan.NguoiDungTenMoi = Utils.ConvertToString(dr["NguoiDungTenMoi"], string.Empty);
                        crThongTinTaiSan.SoLuongCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.SoLuongMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBoCu = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO_CU_THONGTINTAISANLOG], false);
                        crThongTinTaiSan.NguoiDungTenLaCanBoMoi = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO_MOI_THONGTINTAISANLOG], false);
                        crThongTinTaiSan.TenTaiSanCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.TenTaiSanMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.ThongTinKhacCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.ThongTinKhacMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.DiaChiCu = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI_CU_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.DiaChiMoi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI_MOI_THONGTINTAISANLOG], string.Empty));
                        crThongTinTaiSan.NgayChinhSua = Utils.ConvertLongToDate(Utils.ConvertToInt64(dr[NGAY_CAP_NHAT_THONGTINTAISANLOG], 0));
                        crThongTinTaiSan.ThaoTac = Utils.ConvertToInt32(dr[THAO_TAC_THONGTINTAISANLOG], 0);
                        crThongTinTaiSan.TenNhomTaiSan = Utils.ConvertToString(dr["TenNhomTaiSan"], string.Empty);
                        crThongTinTaiSan.TenLoaiTaiSanCu = Utils.ConvertToString(dr["TenLoaiTaiSanCu"], string.Empty);
                        crThongTinTaiSan.TenLoaiTaiSanMoi = Utils.ConvertToString(dr["TenLoaiTaiSanMoi"], string.Empty);
                        crThongTinTaiSan.TenCanbo = Utils.ConvertToString(dr["TenNguoiChinhSua"], string.Empty);

                        var item = new TaiSanLogModel();
                        item.NgayChinhSua = crThongTinTaiSan.NgayChinhSua;
                        item.NhomTaiSanID = crThongTinTaiSan.NhomTaiSanID.Value;
                        item.TenNhomTaisan = crThongTinTaiSan.TenNhomTaiSan;
                        item.ThongTinTaiSanID = crThongTinTaiSan.ThongTinTaiSanID;
                        item.NguoiChinhSua = crThongTinTaiSan.TenCanbo;
                        item.ThaoTac = crThongTinTaiSan.ThaoTac;
                        if (item.ThaoTac == 1)
                        {
                            item.DanhSachChinhSua = new List<ThongTinChinhSuaModel>();
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.GiaTriMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Giá trị", Cu = crThongTinTaiSan.GiaTriCu, Moi = crThongTinTaiSan.GiaTriMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.DienTichMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Diện tích", Cu = crThongTinTaiSan.DienTichCu, Moi = crThongTinTaiSan.DienTichMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.GiaiTrinhNguonGocMoi) || !string.IsNullOrEmpty(crThongTinTaiSan.GiaiTrinhNguonGocCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Giải trình nguồn gốc", Cu = crThongTinTaiSan.GiaiTrinhNguonGocCu, Moi = crThongTinTaiSan.GiaiTrinhNguonGocMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.GiayChungNhanQuyenSuDungMoi) || !string.IsNullOrEmpty(crThongTinTaiSan.GiayChungNhanQuyenSuDungCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Giấy chứng nhận quyền sử dụng", Cu = crThongTinTaiSan.GiayChungNhanQuyenSuDungCu, Moi = crThongTinTaiSan.GiayChungNhanQuyenSuDungMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.ThongTinKhacMoi) || !string.IsNullOrEmpty(crThongTinTaiSan.ThongTinKhacCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "thông tin khác", Cu = crThongTinTaiSan.ThongTinKhacCu, Moi = crThongTinTaiSan.ThongTinKhacMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.NguoiDungTenMoi))
                            {
                                string NguoiDungTenCuDecrypt = crThongTinTaiSan.NguoiDungTenLaCanBoCu == false ? Encrypt_Decrypt.DecryptString_Aes(crThongTinTaiSan.NguoiDungTenCu) : crThongTinTaiSan.NguoiDungTenCu;
                                string NguoiDungTenMoiDecrypt = crThongTinTaiSan.NguoiDungTenLaCanBoMoi == false ? Encrypt_Decrypt.DecryptString_Aes(crThongTinTaiSan.NguoiDungTenMoi) : crThongTinTaiSan.NguoiDungTenMoi;
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Nguời đứng tên", Cu = NguoiDungTenCuDecrypt, Moi = NguoiDungTenMoiDecrypt });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.TenTaiSanMoi)|| !string.IsNullOrEmpty(crThongTinTaiSan.TenTaiSanMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Tên tài sản", Cu = crThongTinTaiSan.TenTaiSanCu, Moi = crThongTinTaiSan.TenTaiSanMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.SoLuongMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Số lượng", Cu = crThongTinTaiSan.SoLuongCu, Moi = crThongTinTaiSan.SoLuongMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.DiaChiMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Địa chỉ", Cu = crThongTinTaiSan.DiaChiCu, Moi = crThongTinTaiSan.DiaChiMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.TenLoaiTaiSanMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Loại tài sản", Cu = crThongTinTaiSan.TenLoaiTaiSanCu, Moi = crThongTinTaiSan.TenLoaiTaiSanMoi });
                            }
                        }
                        else
                        {
                            item.DanhSachChinhSua = new List<ThongTinChinhSuaModel>();
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.GiaTriCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Giá trị", Cu = crThongTinTaiSan.GiaTriCu, Moi = crThongTinTaiSan.GiaTriMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.DienTichCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Diện tích", Cu = crThongTinTaiSan.DienTichCu, Moi = crThongTinTaiSan.DienTichMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.GiaiTrinhNguonGocMoi) || !string.IsNullOrEmpty(crThongTinTaiSan.GiaiTrinhNguonGocCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Giải trình nguồn gốc", Cu = crThongTinTaiSan.GiaiTrinhNguonGocCu, Moi = crThongTinTaiSan.GiaiTrinhNguonGocMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.GiayChungNhanQuyenSuDungMoi) || !string.IsNullOrEmpty(crThongTinTaiSan.GiayChungNhanQuyenSuDungCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Giấy chứng nhận quyền sử dụng", Cu = crThongTinTaiSan.GiayChungNhanQuyenSuDungCu, Moi = crThongTinTaiSan.GiayChungNhanQuyenSuDungMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.ThongTinKhacMoi) || !string.IsNullOrEmpty(crThongTinTaiSan.ThongTinKhacCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "thông tin khác", Cu = crThongTinTaiSan.ThongTinKhacCu, Moi = crThongTinTaiSan.ThongTinKhacMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.NguoiDungTenCu))
                            {
                                string NguoiDungTenCuDecrypt = crThongTinTaiSan.NguoiDungTenLaCanBoCu == false ? Encrypt_Decrypt.DecryptString_Aes(crThongTinTaiSan.NguoiDungTenCu) : crThongTinTaiSan.NguoiDungTenCu;
                                string NguoiDungTenMoiDecrypt = crThongTinTaiSan.NguoiDungTenLaCanBoMoi == false ? Encrypt_Decrypt.DecryptString_Aes(crThongTinTaiSan.NguoiDungTenMoi) : crThongTinTaiSan.NguoiDungTenMoi;
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Nguời đứng tên", Cu = NguoiDungTenCuDecrypt, Moi = NguoiDungTenMoiDecrypt });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.TenTaiSanCu)|| !string.IsNullOrEmpty(crThongTinTaiSan.TenTaiSanMoi))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Tên tài sản", Cu = crThongTinTaiSan.TenTaiSanCu, Moi = crThongTinTaiSan.TenTaiSanMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.SoLuongCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Số lượng", Cu = crThongTinTaiSan.SoLuongCu, Moi = crThongTinTaiSan.SoLuongMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.DiaChiCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Địa chỉ", Cu = crThongTinTaiSan.DiaChiCu, Moi = crThongTinTaiSan.DiaChiMoi });
                            }
                            if (!string.IsNullOrEmpty(crThongTinTaiSan.TenLoaiTaiSanCu))
                            {
                                item.DanhSachChinhSua.Add(new ThongTinChinhSuaModel() { ThongTinChinhSua = "Loại tài sản", Cu = crThongTinTaiSan.TenLoaiTaiSanCu, Moi = crThongTinTaiSan.TenLoaiTaiSanMoi });
                            }
                        }

                        Result.Add(item);

                        //lstThongtinTaiSanLog.Add(crThongTinTaiSan);
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
