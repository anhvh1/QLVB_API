using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Com.Gosol.QLVB.DAL.HeThong
{
    public interface IHeThongCanBoDAL
    {
        public int Insert(HeThongCanBoModel HeThongCanBoModel, ref int CanBoID, ref string Message);
        public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message);
        public List<string> Delete(List<int> CanBoID);
        public HeThongCanBoModel GetCanBoByID(int? CanBoID);
        //public List<HeThongCanBoModel> FilterByName(string TenCanBo, int IsStatus, int CoQuanID);
        public List<HeThongCanBoModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID);
        public List<HeThongCanBoPartialModel> ReadExcelFile_Old(string FilePath, int? CoQuanID);
        public List<HeThongCanBoPartialModel> ReadExcelFile(string FilePath, int? CoQuanID, int? CanBoDangNhapID);
        public int ImportToExel(string FilePath, int? CoQuanID);
        public string GenerationMaCanBo(int CoQuanID);
        public List<HeThongCanBoModel> GetAllCanBoByCoQuanID(int CoQuanID, int CoQuan_ID);
        public List<HeThongCanBoModel> GetAllByCoQuanID(int CoQuanID);
        public List<HeThongCanBoModel> GetAllInCoQuanCha(int CoQuanID);
        public ThongTinDonViModel HeThongCanBo_GetThongTinCoQuan(int CanBoID, int NguoiDungID);
        public List<HeThongCanBoModel> GetAll();
        public List<HeThongCanBoModel> GetAllCanBoByNguoiDung(BasePagingParams p, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID);
    }
    public class HeThongCanBoDAL : IHeThongCanBoDAL


    {
        //private readonly ControllerBase


        //param constant
        private const string PARAM_CanBoID = "@CanBoID";
        private const string PARAM_MaCB = "@MaCB";
        private const string PARAM_TenCanBo = "@TenCanBo";
        private const string PARAM_NgaySinh = "@NgaySinh";
        private const string PARAM_GioiTinh = "@GioiTinh";
        private const string PARAM_DiaChi = "@DiaChi";
        private const string PARAM_ChucVuID = "@ChucVuID";
        private const string PARAM_QuyenKy = "@QuyenKy";
        private const string PARAM_Email = "@Email";
        private const string PARAM_DienThoai = "@DienThoai";
        private const string PARAM_PhongBanID = "@PhongBanID";
        private const string PARAM_CoQuanID = "@CoQuanID";
        private const string PARAM_RoleID = "@RoleID";
        private const string PARAM_QuanTridonVi = "@QuanTridonVi";
        private const string PARAM_CoQuanCuID = "@CoQuanCuID";
        private const string PARAM_CanBoCuID = "@CanBoCuID";
        private const string PARAM_XemTaiLieuMat = "@XemTaiLieuMat";
        private const string PARAM_IsStatus = "@IsStatus";
        private const string PARAM_AnhHoSo = "@AnhHoSo";
        private const string PARAM_HoKhau = "@HoKhau";
        private const string PARAM_MaCQ = "@MaCQ";
        private const string PARAM_CapQuanLy = "@CapQuanLy";
        private const string PARAM_TrangThaiID = "@TrangThaiID";
        private const string PARAM_CMND = "@CMND";
        private const string PARAM_NoiCap = "@NoiCap";
        private const string PARAM_NgayCap = "@NgayCap";


        #region Cán bộ
        public string GenerationMaCanBo(int CoQuanID)
        {
            string maCanBo = "";
            string maCanBoCurr = "";
            string maCoQuan = "";

            SqlParameter[] parameters1 = new SqlParameter[]
       {
                new SqlParameter(PARAM_CoQuanID, SqlDbType.Int)
       };
            parameters1[0].Value = CoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetByID", parameters1))
                {
                    while (dr.Read())
                    {
                        maCoQuan = Utils.ConvertToString(dr["MaCQ"], String.Empty);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (string.IsNullOrEmpty(maCoQuan))
            {
                maCoQuan = "CQ";
            }
            Random oRandom = new Random();
            int MaVach = oRandom.Next(1000, 99999);
            maCanBo = maCoQuan + MaVach;
            //}
            //else
            //{
            //    string s = maCanBoCurr.Substring(maCanBoCurr.IndexOf("_") + 1).ToString();
            //    int STT = Utils.ConvertToInt32((maCanBoCurr.Substring(maCanBoCurr.IndexOf("_") + 1).ToString()), 0);
            //    STT = STT + 1;
            //}

            return maCanBo;
        }

        // Insert Can Bo
        public int Insert(HeThongCanBoModel HeThongCanBoModel, ref int CanBoID, ref string Message)
        {
            int CoQuanID = HeThongCanBoModel.CoQuanID ?? default(int);
            int val = 0;
            if (HeThongCanBoModel.TenCanBo.Trim().Length > 100)
            {

                Message = ConstantLogMessage.API_Error_TooLong;
                return val;
            }
            if (string.IsNullOrEmpty(HeThongCanBoModel.TenCanBo) || HeThongCanBoModel.TenCanBo.Trim().Length <= 0)
            {

                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            if (!Utils.CheckSpecialCharacter(HeThongCanBoModel.TenCanBo))
            {

                Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                return val;
            }
            if (HeThongCanBoModel.CoQuanID != 0)
            {
                var CoQuan = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
                if (CoQuan == null)
                {
                    Message = ConstantLogMessage.Alert_Error_NotExist("Cơ quan");
                    return val;
                }
            }

            //check validate Email/User
            if (HeThongCanBoModel.Email == null || string.IsNullOrEmpty(HeThongCanBoModel.Email))
            {
                if (HeThongCanBoModel.TenNguoiDung == null || string.IsNullOrEmpty(HeThongCanBoModel.TenNguoiDung))
                {
                    Message = "Tên người dùng là bắt buộc!";
                    return val;
                }
            }
            else
            {
                if (HeThongCanBoModel.Email.Contains("@") && HeThongCanBoModel.Email.Contains(".com"))
                {
                    if (!Utils.CheckEmail(HeThongCanBoModel.Email))
                    {
                        Message = "Email không đúng định dạng";
                        return val;
                    }
                }        

                HeThongNguoiDungModel nguoiDung = new HeThongNguoiDungDAL().GetByName(HeThongCanBoModel.Email);
                if (nguoiDung.NguoiDungID > 0)
                {
                    Message = "Email đã được sử dụng!";
                    return val;
                }
                else
                {
                    if (HeThongCanBoModel.TenNguoiDung == null || HeThongCanBoModel.TenNguoiDung == "")
                    {
                        HeThongCanBoModel.TenNguoiDung = HeThongCanBoModel.Email;
                    }
                }
            }

            if (HeThongCanBoModel.TenNguoiDung != null && HeThongCanBoModel.TenNguoiDung.Length > 0)
            {
                HeThongNguoiDungModel nguoiDungModel = new HeThongNguoiDungDAL().GetByName(HeThongCanBoModel.TenNguoiDung);
                if (nguoiDungModel.NguoiDungID > 0)
                {
                    Message = "Tên người dùng đã được sử dụng!";
                    return -3;
                }
            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_NgaySinh, SqlDbType.DateTime),
                    new SqlParameter(PARAM_GioiTinh, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                    new SqlParameter(PARAM_TrangThaiID,  SqlDbType.Int),
                    new SqlParameter(PARAM_MaCB,  SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CanBoID,SqlDbType.Int),
                    new SqlParameter(PARAM_AnhHoSo,SqlDbType.NVarChar),
              };
            parameters[0].Value = HeThongCanBoModel.TenCanBo;
            parameters[1].Value = HeThongCanBoModel.NgaySinh == null ? Convert.DBNull : HeThongCanBoModel.NgaySinh.Value.ToString("yyyy/MM/dd");
            parameters[2].Value = HeThongCanBoModel.GioiTinh ?? Convert.DBNull;
            parameters[3].Value = HeThongCanBoModel.Email ?? Convert.DBNull;
            parameters[4].Value = HeThongCanBoModel.CoQuanID ?? Convert.DBNull;
            parameters[5].Value = HeThongCanBoModel.TrangThaiID ?? Convert.DBNull;
            parameters[8].Value = HeThongCanBoModel.AnhHoSo ?? Convert.DBNull;

            if (string.IsNullOrEmpty(HeThongCanBoModel.MaCB) || HeThongCanBoModel.MaCB.Trim().Length <= 0)
            {
                parameters[6].Value = GenerationMaCanBo(CoQuanID);
            }
            else
            {
                var CanBoByMa = GetByMaCB(HeThongCanBoModel.MaCB);
                if (CanBoByMa.CanBoID > 0)
                {
                    Message = "Mã cán bộ đã tồn tại!";
                    val = 0;
                    return val;
                }
                parameters[6].Value = HeThongCanBoModel.MaCB;
            }
            parameters[7].Direction = ParameterDirection.Output;
            int NguoiDungID;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Insert", parameters), 0);
                        CanBoID = Utils.ConvertToInt32(parameters[8].Value, 0);

                        //thêm người dùng
                        HeThongNguoiDungModel HeThongNguoiDungModel = new HeThongNguoiDungModel();

                        var matKhauMacDinh = new SystemConfigDAL().GetByKey("MatKhau_MacDinh").ConfigValue;
                        HeThongNguoiDungModel.MatKhau = Cryptor.EncryptPasswordUser(HeThongCanBoModel.TenNguoiDung.Trim().ToLower(), matKhauMacDinh ?? "123456");
                        SqlParameter[] paramrs = new SqlParameter[]
                          {
                            new SqlParameter("@TenNguoiDung", SqlDbType.NVarChar),
                            new SqlParameter("@MatKhau", SqlDbType.NVarChar),
                            new SqlParameter("@GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("@TrangThai", SqlDbType.Int),
                            new SqlParameter("@CanBoID", SqlDbType.Int),
                            new SqlParameter("@PublicKeys", SqlDbType.NVarChar),

                          };
                        paramrs[0].Value = HeThongCanBoModel.TenNguoiDung.Trim().ToLower();
                        paramrs[1].Value = HeThongNguoiDungModel.MatKhau.Trim();
                        paramrs[2].Value = HeThongNguoiDungModel.GhiChu ?? Convert.DBNull;
                        paramrs[3].Value = 1;
                        paramrs[4].Value = Utils.ConvertToInt32(val, 0);
                        paramrs[5].Value = HeThongNguoiDungModel.PublicKeys ?? Convert.DBNull;

                        NguoiDungID = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_NguoiDung_Insert", paramrs), 0);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    //Thêm mnguòi dùng đên nhóm mặc định
                    SystemConfigModel sysConfig = new SystemConfigDAL().GetByKey("Nhom_Nguoi_Dung_Mac_Dinh");
                    if (sysConfig != null)
                    {
                        string strValue = sysConfig.ConfigValue;
                        string[] arrValue = strValue.Split(',');
                        foreach (var item in arrValue)
                        {
                            NhomNguoiDungModel nnd = new PhanQuyenDAL().NhomNguoiDung_GetByID(Utils.ConvertToInt32(item.Trim(), 0));
                            if (nnd != null)
                            {
                                NhomNguoiDungModel nhomNguoiDungModel = new PhanQuyenDAL().NhomNguoiDung_GetByCoQuanIDAndNhomTongID(CoQuanID, Utils.ConvertToInt32(item.Trim(), 0));
                                NguoiDungNhomNguoiDungModel nd_nnd = new NguoiDungNhomNguoiDungModel();
                                nd_nnd.NguoiDungID = NguoiDungID;
                                if (nhomNguoiDungModel.NhomNguoiDungID != 0)
                                {
                                    nd_nnd.NhomNguoiDungID = nhomNguoiDungModel.NhomNguoiDungID;
                                }
                                else
                                {
                                    nd_nnd.NhomNguoiDungID = Utils.ConvertToInt32(item.Trim(), 0);
                                }
                                BaseResultModel baseResult = new PhanQuyenDAL().NguoiDung_NhomNguoiDung_InsertOne(nd_nnd);
                            }
                        }
                    }

                    //new DotKeKhaiDAL().GetDotKeKhaiFitForCanBo(Utils.ConvertToInt32(val, 0), HeThongCanBoModel.CoQuanID.Value);
                    Message = ConstantLogMessage.Alert_Insert_Success("cán bộ");
                    return val;
                }
            }

        }

        public int InsertForImportExcel(List<HeThongCanBoPartialModel> DanhSachCanBoImport, ref int CanBoID, ref string Message, int? CoQuanDangNhapID, int? NguoiDungID, int? CanBoDangNhapID)
        {
            int val = 0;
            var matKhauMacDinh = new SystemConfigDAL().GetByKey("MatKhau_MacDinh").ConfigValue;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                foreach (var _heThongCanBoModel in DanhSachCanBoImport)
                {
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            //foreach (var _heThongCanBoModel in DanhSachCanBoImport)
                            //{
                            SqlParameter[] parameters = new SqlParameter[]
                             {
                                       new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                                       new SqlParameter(PARAM_NgaySinh, SqlDbType.DateTime),
                                       new SqlParameter(PARAM_GioiTinh, SqlDbType.NVarChar),
                                       new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
                                       new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                                       new SqlParameter(PARAM_TrangThaiID,  SqlDbType.Int),
                                       new SqlParameter(PARAM_AnhHoSo,  SqlDbType.NVarChar),
                                       new SqlParameter(PARAM_MaCB,  SqlDbType.NVarChar),
                                       new SqlParameter("@PasswordMacDinh", SqlDbType.NVarChar),
                                       new SqlParameter("@TenNguoiDung", SqlDbType.NVarChar),

                             };
                            parameters[0].Value = _heThongCanBoModel.TenCanBo;
                            parameters[1].Value = _heThongCanBoModel.NgaySinh == null ? Convert.DBNull : _heThongCanBoModel.NgaySinh.Value.ToString("yyyy/MM/dd");
                            parameters[2].Value = _heThongCanBoModel.GioiTinh ?? Convert.DBNull;
                            parameters[3].Value = _heThongCanBoModel.Email ?? Convert.DBNull;
                            parameters[4].Value = _heThongCanBoModel.CoQuanID ?? Convert.DBNull;
                            parameters[5].Value = EnumTrangThaiCanBo.DangLamViec.GetHashCode();
                            parameters[6].Value = _heThongCanBoModel.AnhHoSo ?? Convert.DBNull;
                            if (string.IsNullOrEmpty(_heThongCanBoModel.MaCB) || _heThongCanBoModel.MaCB.Trim().Length <= 0)
                            {
                                parameters[7].Value = GenerationMaCanBo(_heThongCanBoModel.CoQuanID.Value);
                            }
                            else
                            {
                                var CanBoByMa = GetByMaCB(_heThongCanBoModel.MaCB);
                                if (CanBoByMa.CanBoID > 0)
                                {
                                    Message = "Mã cán bộ đã tồn tại!";
                                    val = 0;
                                    return val;
                                }
                                parameters[7].Value = _heThongCanBoModel.MaCB;
                            }
                            parameters[8].Value = Cryptor.EncryptPasswordUser(_heThongCanBoModel.TenNguoiDung.Trim().ToLower(), matKhauMacDinh ?? "123456");
                            parameters[9].Value = _heThongCanBoModel.TenNguoiDung.Trim().ToLower();
                            val = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Insert_For_ImportExcel", parameters), 0);
                            trans.Commit();
                            //}
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            return val;
        }
        // Update 
        public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message)
        {
            var CanBoOld = GetCanBoByID(HeThongCanBoModel.CanBoID);
            int val = 0;
          
            if (HeThongCanBoModel.CanBoID == 0)
            {
                Message = "Chưa có cán bộ được chọn!";
                return val;
            }
            if (HeThongCanBoModel.TenCanBo.Trim().Length > 100)
            {

                Message = ConstantLogMessage.API_Error_TooLong;
                return val;
            }
            if (string.IsNullOrEmpty(HeThongCanBoModel.TenCanBo) || HeThongCanBoModel.TenCanBo.Trim().Length <= 0)
            {

                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            if (!Utils.CheckSpecialCharacter(HeThongCanBoModel.TenCanBo))
            {

                Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                return val;
            }
            var crCanBo = GetCanBoByID(HeThongCanBoModel.CanBoID);
            if (crCanBo == null || crCanBo.CanBoID < 1)
            {
                Message = ConstantLogMessage.Alert_Error_NotExist("Cán bộ");
                return val;
            }

            if (HeThongCanBoModel.CoQuanID != null)
            {
                var CoQuan = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
                if (CoQuan == null)
                {
                    Message = ConstantLogMessage.Alert_Error_NotExist("Cơ quan");
                    return val;
                }
                else
                {
                    var CoQuanByID = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
                }
            }
           
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter(PARAM_CanBoID, SqlDbType.Int),
                  new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                  new SqlParameter(PARAM_NgaySinh, SqlDbType.DateTime),
                  new SqlParameter(PARAM_GioiTinh, SqlDbType.Int),
                  new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
                  new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                  new SqlParameter(PARAM_TrangThaiID,  SqlDbType.Int),
                  new SqlParameter(PARAM_AnhHoSo,  SqlDbType.NVarChar),
              };
            parameters[0].Value = HeThongCanBoModel.CanBoID;
            parameters[1].Value = HeThongCanBoModel.TenCanBo ?? Convert.DBNull;
            parameters[2].Value = HeThongCanBoModel.NgaySinh == null ? Convert.DBNull : HeThongCanBoModel.NgaySinh.Value.ToString("yyyy/MM/dd");
            parameters[3].Value = HeThongCanBoModel.GioiTinh ?? Convert.DBNull;
            parameters[4].Value = HeThongCanBoModel.Email ?? Convert.DBNull;
            parameters[5].Value = HeThongCanBoModel.CoQuanID ?? Convert.DBNull;
            parameters[6].Value = HeThongCanBoModel.TrangThaiID ?? Convert.DBNull;
            parameters[7].Value = HeThongCanBoModel.AnhHoSo ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string MessageNew = null;
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Update", parameters);
                        trans.Commit();
                      
                        if (val > 0)
                        {
                            //update lại tài khoản và mật khẩu nếu thay đổi email
                            //if (CanBoOld.Email != HeThongCanBoModel.Email)
                            //{
                            //    new HeThongNguoiDungDAL().UpdateTaiKhoanNguoiDung(CanBoOld.NguoiDungID ?? 0, HeThongCanBoModel.Email);
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Message = ex.Message;
                        return val;
                        throw;
                    }
                    Message = ConstantLogMessage.Alert_Update_Success("cán bộ - chức vụ");
                }
            
                if (HeThongCanBoModel.TrangThaiID == 1) // Đang hoạt động
                {
                    new HeThongNguoiDungDAL().UpdateTrangThai(new List<int>() { HeThongCanBoModel.CanBoID }, 1);
                }
                else if (HeThongCanBoModel.TrangThaiID == 2) // Nghỉ hưu
                {
                    new HeThongNguoiDungDAL().UpdateTrangThai(new List<int>() { HeThongCanBoModel.CanBoID }, 0);
                }
                return val;
            }
        }

        // Delete
        public List<string> Delete(List<int> ListCanBoID)
        {

            List<string> dic = new List<string>();
            string message = "";
            if (ListCanBoID.Count <= 0)
            {
                message = ConstantLogMessage.API_Error_NotExist;
                dic.Add(message);
                return dic;
            }
            else
            {
                int val = 0;
                for (int i = 0; i < ListCanBoID.Count; i++)
                {
                    //var KeKhaiByCanBoID = new KeKhaiDAL().GetKeKhaiByCanBoID(ListCanBoID[i]).ToList();
                    //if (CheckRef(ListCanBoID[i]))
                    //{
                    //    dic.Add("Cán bộ " + GetCanBoByID(ListCanBoID[i]).TenCanBo + " đang được sử dụng. Không thể xóa!");
                    //}
                    //else if (GetCanBoByID(ListCanBoID[i]) == null)
                    //{
                    //    message = "Cán bộ " + GetCanBoByID(ListCanBoID[i]).TenCanBo + " không tồn tại!";
                    //    dic.Add(message);
                    //}
                    //else if (KeKhaiByCanBoID.Count > 0)
                    //{
                    //    dic.Add("Cán bộ " + GetCanBoByID(ListCanBoID[i]).TenCanBo + " đã có kê khai. Không thể xóa!");
                    //}
                    //else
                    //{
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                        new SqlParameter(@"CanBoID", SqlDbType.Int)

                      };
                    parameters[0].Value = ListCanBoID[i];
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Delete", parameters), 0);
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                dic.Add(ex.Message);
                                throw ex;
                            }
                        }
                    }
                    //}
                }
                return dic;
            }

        }

        //public List<DanhMucChucVuModel> GetAll()

        // Get By id
        public HeThongCanBoModel GetCanBoByID(int? CanBoID)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        canBo.TrangThaiTaiKhoan = Utils.ConvertToInt32(dr["TrangThaiTaiKhoan"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        break;
                    }
                    dr.Close();
                    //if (canBo != null && canBo.CanBoID > 0)
                    //{
                    //    canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(CanBoID.Value).Select(x => x.ChucVuID).ToList();
                    //    // lây danh sách chức vụ của cán bộ
                    //}
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }


        /// <summary>
        ///  lấy thông tin tên cơ quan và cơ quan cha của cán bộ đang đăng nhập
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <param name="NguoiDungID"></param>
        /// <returns></returns>
        public ThongTinDonViModel HeThongCanBo_GetThongTinCoQuan(int CanBoID, int NguoiDungID)
        {
            ThongTinDonViModel canBo = new ThongTinDonViModel();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                canBo.TenCoQuan = string.Empty;
                canBo.TenCoQuanCha = string.Empty;
                return canBo;
            }


            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThongCanBo_GetThongTinCoQuan_GetBy_CanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new ThongTinDonViModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                        break;
                    }
                    dr.Close();

                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }
        //GetAll
        public List<HeThongCanBoModel> GetAll()
        {
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAll"))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);


                        list.Add(canBo);
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


        // Get All without nguoidungid
        public List<HeThongCanBoModel> GetAllCanBoWithoutNguoiDung()
        {
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_GetAllCanBoWithoutNguoiDung"))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        list.Add(canBo);
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
        // Get By Name
        public HeThongCanBoModel GetByMaCB(string MaCB)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"MaCB",SqlDbType.NVarChar)
              };
            parameters[0].Value = MaCB ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetByMaCB", parameters))
                {
                    while (dr.Read())
                    {
                        // canBo = new HeThongCanBoModel(Utils.ConvertToInt32(dr["CanBoID"], 0), Utils.ConvertToString(dr["TenCanBo"], string.Empty), Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now), Utils.ConvertToInt32(dr["GioiTinh"], 0), Utils.ConvertToString(dr["DiaChi"], string.Empty), Utils.ConvertToInt32(dr["ChucVuID"], 0), Utils.ConvertToInt32(dr["QuyenKy"], 0), Utils.ConvertToString(dr["Email"], string.Empty), Utils.ConvertToString(dr["DienThoai"], string.Empty), Utils.ConvertToInt32(dr["PhongBanID"], 0), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToInt32(dr["RoleID"], 0), Utils.ConvertToInt32(dr["QuanTridonVi"], 0), Utils.ConvertToInt32(dr["CoQuanCuID"], 0), Utils.ConvertToInt32(dr["CanBoCuID"], 0), Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0), Utils.ConvertToString(dr["AnhHoSo"], string.Empty),
                        //   Utils.ConvertToString(dr["HoKhau"], string.Empty), Utils.ConvertToString(dr["MaCB"], string.Empty), Utils.ConvertToInt32(dr["CapQuanLy"], 0), Utils.ConvertToInt32(dr["TrangThaiID"], 0), Utils.ConvertToInt32(dr["NguoiDungID"], 0));
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        break;

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }
        // Filter By Name
        //public List<HeThongCanBoModel> FilterByName(string TenCanBo, int IsStatus, int CoQuanID)
        //{
        //    List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
        //  SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter(PARAM_TenCanBo,SqlDbType.NVarChar),
        //        new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
        //        new SqlParameter(PARAM_IsStatus,SqlDbType.Int)
        //};
        //    parameters[0].Value = TenCanBo;
        //    parameters[1].Value = CoQuanID == 0 ? (int?)null : CoQuanID;
        //    parameters[2].Value = IsStatus == 0 ? (int?)null : IsStatus;
        //    try
        //    {

        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILTERBYNAME, parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                HeThongCanBoModel canBo = new HeThongCanBoModel(Utils.ConvertToInt32(dr["CanBoID"], 0), Utils.ConvertToString(dr["TenCanBo"], string.Empty), Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now), Utils.ConvertToInt32(dr["GioiTinh"], 0), Utils.ConvertToString(dr["DiaChi"], string.Empty), Utils.ConvertToInt32(dr["ChucVuID"], 0), Utils.ConvertToInt32(dr["QuyenKy"], 0), Utils.ConvertToString(dr["Email"], string.Empty), Utils.ConvertToString(dr["DienThoai"], string.Empty), Utils.ConvertToInt32(dr["PhongBanID"], 0), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToInt32(dr["RoleID"], 0), Utils.ConvertToInt32(dr["QuanTridonVi"], 0), Utils.ConvertToInt32(dr["CoQuanCuID"], 0), Utils.ConvertToInt32(dr["CanBoCuID"], 0), Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0), Utils.ConvertToInt32(dr["IsStatus"], 0));

        //                list.Add(canBo);
        //            }
        //            dr.Close();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return list;
        //}

        // Get list Paging
        public List<HeThongCanBoModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID)
        {
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            //tbCoQuanID.Rows.Add(1);
            //CoQuanID.ForEach(x => table.Rows.Add(x));
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_ID);
            //if (p.CoQuanID != null && p.CoQuanID > 0)
            //{
            //    tbCoQuanID.Rows.Add(p.CoQuanID);
            //} 
            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(crCoQuan.CoQuanID).ToList();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(0).ToList();
            }
            listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                 new SqlParameter("@CoQuanID",SqlDbType.Int),
                  new SqlParameter("@TrangThaiID",SqlDbType.Int),
                  pList
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            //parameters[5].Value = 0;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = CoQuanID ?? Convert.DBNull;
            parameters[7].Value = TrangThaiID ?? Convert.DBNull;
            parameters[8].Value = tbCoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_CanBo_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.TrangThaiTaiKhoan = Utils.ConvertToInt32(dr["TrangThaiTaiKhoan"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);

                        //if (canBo.CanBoID != 1)
                        //{
                        list.Add(canBo);
                        //}

                    }

                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //List<int> List = new List<int>();
            //listCoQuanCon.ForEach(x => List.Add(x.CoQuanID));
            //list.Where(x => List.Contains(x.CoQuanID.Value)).ToList();
            //list = list.OrderBy(x => x.CapCoQuanID).ToList();
            return list;
        }

        public List<HeThongCanBoModel> GetAllCanBoByNguoiDung(BasePagingParams p, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID)
        {
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));

            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_ID);

            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();

            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(0).ToList();
            }
            else listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(crCoQuan.CoQuanID).ToList();
            listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@CoQuanID",SqlDbType.Int),
                new SqlParameter("@TrangThaiID",SqlDbType.Int),
                pList
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = CoQuanID ?? Convert.DBNull;
            parameters[2].Value = TrangThaiID ?? Convert.DBNull;
            parameters[3].Value = tbCoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_CanBo_GetAllCanBoByNguoiDung", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        //canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        //canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        //canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        //canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        //canBo.CapCoQuanID = Utils.ConvertToInt32(dr["CapID"], 0);
                        //canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(canBo.CanBoID).Select(x => x.ChucVuID).ToList();
                        //string ListChucVuID = Utils.ConvertToString(dr["ListChucVuID"], String.Empty);
                        //canBo.DanhSachChucVuID = new List<int>();
                        //if (ListChucVuID != null && ListChucVuID.Length > 0)
                        //{
                        //    var listID = ListChucVuID.Split(',');
                        //    for (int i = 0; i < listID.Length; i++)
                        //    {
                        //        canBo.DanhSachChucVuID.Add(Utils.ConvertToInt32(listID[i], 0));
                        //    }
                        //}
                        list.Add(canBo);
                    }

                    dr.Close();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            //list = list.OrderBy(x => x.CapCoQuanID).ToList();
            return list;
        }
        // Import exel file to db
        public List<HeThongCanBoPartialModel> ReadExcelFile_Old(string FilePath, int? CoQuanID)
        {
            var Result = new List<HeThongCanBoPartialModel>();
            return Result;
        }
        public List<HeThongCanBoPartialModel> ReadExcelFile(string FilePath, int? CoQuanID, int? CanBoDangNhapID)
        {
            List<HeThongCanBoPartialModel> DanhSachHeThongCanBoPartial = new List<HeThongCanBoPartialModel>();
            List<HeThongCanBoPartialModel> DanhSachLoi = new List<HeThongCanBoPartialModel>();
            List<HeThongCanBoPartialModel> DanhSachKhongLoi = new List<HeThongCanBoPartialModel>();
            if (!File.Exists(FilePath))
            {
                //return DanhSachHeThongCanBoPartial;
                return null;
            }
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        List<string> DanhSachNguyenNhan = new List<string>();
                        List<string> DanhSachEmailTrongFileExcel = new List<string>();
                        List<string> DanhSachNguoiDungTrongFileExcel = new List<string>();
                        HeThongCanBoPartialModel HeThongCanBoPartialModel = new HeThongCanBoPartialModel();
                        int RowNoData = 0;
                        int EndRow = 0;
                        string LoiNgaySinh = "";
                        int? GioiTinh = null;
                        DateTime? Ngaysinhnhat = null;
                        int? CoQuanIDTuFile = null;
                        for (int i = 4; i <= workSheet.Dimension.End.Row; i++)
                        {

                            if (Utils.ConvertToInt32(workSheet.Cells[i, 1].Value, 0) <= 0 && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 2].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 3].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 4].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 5].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 6].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 7].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 8].Value, string.Empty)) && Utils.ConvertToInt32(workSheet.Cells[i, 9].Value, 0) <= 0 /*&&*/
                                 /*string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[i, 10].Value, string.Empty))*/)
                            {
                                RowNoData = i;
                                break;
                            }
                            EndRow = i;

                        }
                        if (RowNoData == 4)
                        {
                            DanhSachNguyenNhan.Add("Files không có dữ liệu!");
                            HeThongCanBoPartialModel.NguyenNhan = DanhSachNguyenNhan;
                            DanhSachHeThongCanBoPartial.Add(HeThongCanBoPartialModel);
                            return null;
                        }
                        //List<DanhMucChucVuModel> DanhSachChucVuAll = new DanhMucChucVuDAL().GetAll(CoQuanID);
                        //List<CaLamViecModel> DanhSachCaLamViecAll = new CaLamViecDAL().GetAll(CoQuanID);
                        for (int i = 4; i < EndRow + 1; i++)
                        {
                            HeThongCanBoPartialModel itemCanBo = new HeThongCanBoPartialModel();
                            itemCanBo.NguyenNhan = new List<string>();
                            //itemCanBo.DanhSachChucVuID = new List<int>();
                            //itemCanBo.DanhSachCaLamViecID = new List<int>();
                            for (int j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
                            {
                                //đọc cột mã cán bộ
                                if (j == 2)
                                {
                                    string MaCB = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Trim();
                                    if (!string.IsNullOrEmpty(MaCB))
                                    {
                                        if (GetByMaCB(MaCB).CanBoID > 0)
                                        {
                                            itemCanBo.NguyenNhan.Add("Mã cán bộ đã tồn tại!");
                                            itemCanBo.MaCB = MaCB;
                                        }
                                        else
                                        {
                                            itemCanBo.MaCB = MaCB;
                                        }
                                    }
                                }//đọc cột tên cán bộ
                                if (j == 3)
                                {
                                    string TenCanBo = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Trim();
                                    if (string.IsNullOrEmpty(TenCanBo))
                                    {
                                        itemCanBo.NguyenNhan.Add("Tên cán bộ không được để trống!");
                                    }
                                    else
                                    {
                                        if (!Utils.CheckSpecialCharacter(TenCanBo))
                                        {
                                            itemCanBo.NguyenNhan.Add("Tên cán bộ không được chưa ký tự đặc biệt!");
                                            itemCanBo.TenCanBo = TenCanBo;
                                        }
                                        else
                                        {
                                            itemCanBo.TenCanBo = TenCanBo;
                                        }
                                    }
                                }
                                //đoc cột ngày sinh
                                if (j == 4)
                                {
                                    string NgaySinh = Utils.ConvertToString(workSheet.Cells[i, 4].Value, string.Empty).Trim().ToLower();
                                    if (string.IsNullOrEmpty(NgaySinh))
                                    {
                                        //Ngaysinhnhat = (DateTime?)null;
                                        //LoiNgaySinh = "Ngày sinh đang để trống";
                                        //itemCanBo.NguyenNhan.Add(LoiNgaySinh);
                                    }
                                    else
                                    {
                                        if (Utils.CheckCharacter(NgaySinh.ToString().Trim()))
                                        {
                                            LoiNgaySinh = "Ngày sinh không đúng định dạng";
                                            Ngaysinhnhat = Utils.ConvertToDateTime(NgaySinh, DateTime.Now);
                                            itemCanBo.NguyenNhan.Add(LoiNgaySinh);
                                        }
                                        else
                                        {
                                            LoiNgaySinh = null;
                                            Ngaysinhnhat = Utils.ConvertToDateTime(NgaySinh, DateTime.Now);
                                            itemCanBo.NgaySinh = Ngaysinhnhat;
                                            //if (Ngaysinhnhat == DateTime.Now)
                                            //{
                                            //    LoiNgaySinh = "Ngày sinh không đúng định dạng";
                                            //    itemCanBo.NguyenNhan.Add(LoiNgaySinh);
                                            //}
                                            //else
                                            //{
                                            //    itemCanBo.NgaySinh = Ngaysinhnhat;
                                            //}
                                        }
                                    }
                                }
                                //đọc cột giới tính(liên kết với workbook GioiTinh trong file excel)
                                if (j == 5)
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, 10].Value, "");
                                    if (string.IsNullOrEmpty(temp))
                                    {
                                        //itemCanBo.NguyenNhan.Add("Thiếu giới tính!");
                                    }
                                    else
                                    {
                                        itemCanBo.GioiTinh = Utils.ConvertToInt32(temp, 0);
                                    }
                                }
                                //đọc cột email
                                if (j == 6)
                                {
                                    string Email = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Trim();
                                    if (string.IsNullOrEmpty(Email))
                                    {
                                        //itemCanBo.NguyenNhan.Add("Email/User không được để tróng!");
                                    }
                                    else
                                    {
                                        if (Email.Contains("@") && Email.Contains(".com"))
                                        {
                                            if (!Utils.CheckEmail(Email))
                                            {
                                                itemCanBo.NguyenNhan.Add("Email không đúng định dạng");
                                                itemCanBo.Email = Email;
                                            }
                                            else
                                            {
                                                HeThongNguoiDungModel nguoiDung = new HeThongNguoiDungDAL().GetByName(Email);
                                                if (nguoiDung.NguoiDungID > 0)
                                                {
                                                    itemCanBo.NguyenNhan.Add("Email đã đựoc sử dụng!");
                                                    itemCanBo.Email = Email;
                                                }
                                                else if (DanhSachEmailTrongFileExcel.Contains(Email))
                                                {
                                                    itemCanBo.NguyenNhan.Add("Email trùng với email trong file đang import!");
                                                    itemCanBo.Email = Email;
                                                }
                                                else
                                                {
                                                    itemCanBo.Email = Email;
                                                    DanhSachEmailTrongFileExcel.Add(Email);
                                                }
                                            }
                                        }
                                        //else
                                        //{
                                        //    HeThongNguoiDungModel nguoiDung = new HeThongNguoiDungDAL().GetByName(Email, CoQuanSuDungPhanMemID);
                                        //    if (nguoiDung.NguoiDungID > 0)
                                        //    {
                                        //        itemCanBo.NguyenNhan.Add("Email đã đựoc sử dụng!");
                                        //        itemCanBo.Email = Email;
                                        //    }
                                        //    else
                                        //    {
                                        //        itemCanBo.Email = Email;
                                        //    }
                                        //}
                                    }

                                }
                                //đọc cột tài khoản
                                if (j == 7)
                                {
                                    string TaiKhoan = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Trim().ToLower();
                                    if (string.IsNullOrEmpty(TaiKhoan))
                                    {
                                        itemCanBo.NguyenNhan.Add("Tài khoản người dùng là bắt buộc!");
                                    }
                                    else
                                    {
                                        if (Utils.ConvertToUnSign(TaiKhoan) != TaiKhoan)
                                        {
                                            itemCanBo.NguyenNhan.Add("Tài khoản không được chứa ký tự có dấu!");
                                            itemCanBo.TenNguoiDung = TaiKhoan;
                                        }
                                        HeThongNguoiDungModel nguoiDung = new HeThongNguoiDungDAL().GetByName(TaiKhoan);
                                        if (nguoiDung.NguoiDungID > 0)
                                        {
                                            itemCanBo.NguyenNhan.Add("Tài khoản đã đựoc sử dụng!");
                                            itemCanBo.TenNguoiDung = TaiKhoan;
                                        }
                                        else if (DanhSachNguoiDungTrongFileExcel.Contains(TaiKhoan))
                                        {
                                            itemCanBo.NguyenNhan.Add("Tài khoản trùng với tài khoản trong file đang import!");
                                            itemCanBo.TenNguoiDung = TaiKhoan;
                                        }
                                        else
                                        {
                                            itemCanBo.TenNguoiDung = TaiKhoan;
                                            DanhSachNguoiDungTrongFileExcel.Add(TaiKhoan);
                                        }
                                    }
                                }
                                //đọc cột công ty(liên kết với workbook CongTy trong file excel)
                                if (j == 8)
                                {
                                    CoQuanIDTuFile = Utils.ConvertToInt32(workSheet.Cells[i, 11].Value, 0);
                                    if (CoQuanIDTuFile == 0)
                                    {
                                        itemCanBo.NguyenNhan.Add("Thiếu cơ quan!");
                                    }
                                    else
                                    {
                                        itemCanBo.CoQuanID = CoQuanIDTuFile;
                                    }
                                }

                            }
                            DanhSachHeThongCanBoPartial.Add(itemCanBo);
                        }
                        DanhSachKhongLoi = DanhSachHeThongCanBoPartial.Where(x => x.NguyenNhan.Count == 0).ToList();
                        DanhSachLoi = DanhSachHeThongCanBoPartial.Where(x => x.NguyenNhan.Count > 0).ToList();
                        var CanBoID = 0;
                        var Message = "";
                        if (DanhSachLoi == null || DanhSachLoi.Count == 0)
                        {
                            //foreach (var item in DanhSachKhongLoi)
                            //{
                            //    HeThongCanBoModel heThongCanBo = new HeThongCanBoModel();
                            //    heThongCanBo.MaCB = item.MaCB;
                            //    heThongCanBo.TenCanBo = item.TenCanBo;
                            //    heThongCanBo.NgaySinh = item.NgaySinh;
                            //    heThongCanBo.GioiTinh = item.GioiTinh;
                            //    heThongCanBo.DiaChi = item.DiaChi;
                            //    heThongCanBo.CoQuanID = item.CoQuanID;
                            //    heThongCanBo.Email = item.Email;
                            //    heThongCanBo.DienThoai = item.DienThoai;
                            //    heThongCanBo.TrangThaiID = (int?)EnumTrangThaiNhanVien.DangLam;
                            //    heThongCanBo.DanhSachChucVuID = item.DanhSachChucVuID;
                            //    heThongCanBo.DanhSachCaLamViecID = item.DanhSachCaLamViecID;
                            //    var temp = Insert(heThongCanBo, ref CanBoID, ref Message, null, null);
                            //}
                            //ChungNN đag sửa để import nhanh hơn
                            InsertForImportExcel(DanhSachKhongLoi, ref CanBoID, ref Message, null, null, CanBoDangNhapID);
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
            return DanhSachLoi;
        }

        // Import data to exel 
        public int ImportToExel(string FilePath, int? CoQuanID)
        {
            int val = 0;
            if (!File.Exists(FilePath))
            {
                return val;
            }
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var TotalWorksheet = package.Workbook.Worksheets.Count;
                    if (TotalWorksheet <= 0)
                    {
                        return val;
                    }
                    else
                    {

                        for (int i = 0; i < TotalWorksheet; i++)
                        {
                            DataTable dt = new DataTable();
                            //if (package.Workbook.Worksheets[i].Name.ToString() == "Chuc_Vu")
                            //{
                            //    var ListChucVu = new DanhMucChucVuDAL().GetAllChucVu();
                            //    dt.Columns.Add("STT");
                            //    dt.Columns.Add("TenChucVu");
                            //    foreach (var item in ListChucVu)
                            //    {

                            //        dt.Rows.Add(item.ChucVuID, item.TenChucVu);
                            //    }
                            //    //XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook(FilePath);
                            //    //wbook.Worksheet(package.Workbook.Worksheets[i].Name.ToString()).Clear();
                            //    package.Workbook.Worksheets[i].Cells.Clear();
                            //    int rowIndex = 0;
                            //    foreach (DataRow dr in dt.Rows)
                            //    {
                            //        rowIndex++;
                            //        int colIndex = 0;
                            //        package.Workbook.Worksheets[i].Cells[1, 1].Value = "ID";
                            //        package.Workbook.Worksheets[i].Cells[1, 2].Value = "Chức Vụ";
                            //        foreach (DataColumn dc in dt.Columns)
                            //        {
                            //            colIndex++;
                            //            package.Workbook.Worksheets[i].SetValue(rowIndex + 1, colIndex, dr[dc.ColumnName]);
                            //        }
                            //    }
                            //    package.Save();
                            //    dt.Clear();

                            //}
                            if (package.Workbook.Worksheets[i].Name.ToString() == "CongTy")
                            {
                                var ListCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID.Value);
                                dt.Columns.Add("NoiCongTac");
                                dt.Columns.Add("STT");
                                foreach (var item in ListCoQuan)
                                {
                                    dt.Rows.Add(item.TenCoQuan, item.CoQuanID);
                                }
                                package.Workbook.Worksheets[i].Cells.Clear();
                                int rowIndex = 0;
                                foreach (DataRow dr in dt.Rows)
                                {
                                    rowIndex++;
                                    int colIndex = 0;
                                    //package.Workbook.Worksheets[i].Cells[1, 1].Value = "ID";
                                    //package.Workbook.Worksheets[i].Cells[1, 2].Value = "Nơi công tác";
                                    foreach (DataColumn dc in dt.Columns)
                                    {
                                        colIndex++;
                                        package.Workbook.Worksheets[i].SetValue(rowIndex, colIndex, dr[dc.ColumnName]);
                                    }
                                }
                                package.Save();
                                dt.Clear();
                            }
                            //if (package.Workbook.Worksheets[i].Name.ToString() == "Trang_Thai")
                            //{
                            //    var ListTrangThai = new DanhMucTrangThaiDAL().GetAll().Where(x => x.TrangThaiSuDung == true).ToList();
                            //    dt.Columns.Add("STT");
                            //    dt.Columns.Add("TenTrangThai");
                            //    foreach (var item in ListTrangThai)
                            //    {

                            //        dt.Rows.Add(item.TrangThaiID, item.TenTrangThai);
                            //    }
                            //    package.Workbook.Worksheets[i].Cells.Clear();
                            //    int rowIndex = 0;
                            //    foreach (DataRow dr in dt.Rows)
                            //    {
                            //        rowIndex++;
                            //        int colIndex = 0;
                            //        package.Workbook.Worksheets[i].Cells[1, 1].Value = "ID";
                            //        package.Workbook.Worksheets[i].Cells[1, 2].Value = "Trạng Thái";
                            //        foreach (DataColumn dc in dt.Columns)
                            //        {
                            //            colIndex++;
                            //            package.Workbook.Worksheets[i].SetValue(rowIndex + 1, colIndex, dr[dc.ColumnName]);
                            //        }
                            //    }
                            //    package.Save();
                            //    dt.Clear();
                            //}

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }

        //Chek Reference
        public bool CheckRef(int CanBoID)
        {
            try
            {
                //var CanBoByDotKeKhai = new KeKhaiTaiSanDAL().GetAll().Where(x => x.CanBoID == CanBoID).ToList().Count();
                //var checkThanNhan = new KeKhaiThanNhanDAL().GetByCanBoID(CanBoID);
                //var checkNguoiDung = new HeThongNguoiDungDAL().GetByCanBoID(CanBoID); // đã gộp cán bộ và người dùng thanh 1 màn hình, xóa cán bộ đồng thời xóa người dùng
                //var checkDotKeKhai = new DotKeKhaiDAL().GetDotKeKhaiByCanBoID(CanBoID);
                //if (checkThanNhan.Count > 0 /*|| checkNguoiDung.CanBoID > 0 *//*|| checkDotKeKhai.Count > 0*/)
                //{
                //    return true;
                //}
                return false;
            }
            catch (Exception ex)
            {
                return true;
                throw ex;
            }
        }

        // Convert sang chức vụ id by tên chức vụ
        public int ConvertChucVuIDByName(string TenChucVu)
        {
            return new DanhMucChucVuDAL().GetChucVuByName(TenChucVu).ChucVuID;
        }
        // Convert sang chức vụ id by tên chức vụ
        public int ConvertCoQuanIDByName(string TenCoQuan)
        {
            return new DanhMucCoQuanDonViDAL().GetByName(TenCoQuan).CoQuanID;
        }

        // Convert sang trạng thái id by tên chức vụ
        public int ConvertTrangThaiIDByName(string TenTrangThai)
        {
            return 0;
            //return new DanhMucTrangThaiDAL().GetByName(TenTrangThai).TrangThaiID;
        }

        public List<HeThongCanBoModel> GetCanBoByTrangThaiID(int? CanBoID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TrangThaiID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetByTrangThaiID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        Result.Add(canBo);
                        break;
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

        /// <summary>
        /// lấy toàn bộ cán bộ thuộc danh sách cơ quan
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAllByListCoQuanID(List<int> CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            var table = new DataTable();
            table.Columns.Add("CoQuanID", typeof(string));
            CoQuanID.ForEach(x => table.Rows.Add(x));

            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.id_list";
            SqlParameter[] parameters = new SqlParameter[]
            {
                pList
            };
            parameters[0].Value = table;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInListCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        Result.Add(canBo);
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
        ///  lấy tất cả cán bộ trong cơ quan và cơ quan con
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAllByCoQuanID(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            var DanhSachCoQuanID = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID).Select(x => x.CoQuanID).ToList();

            var table = new DataTable();
            table.Columns.Add("CoQuanID", typeof(string));
            DanhSachCoQuanID.ForEach(x => table.Rows.Add(x));

            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.id_list";
            SqlParameter[] parameters = new SqlParameter[]
            {
                pList
            };
            parameters[0].Value = table;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInListCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        if (canBo.CanBoID != 1)
                            Result.Add(canBo);
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
        /// lấy toàn bộ cán bộ trong 1 cơ quan 
        /// lấy cả danh sách chức vụ, tên chức vụ
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAllInCoQuanID(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CoQuanID", SqlDbType.Int)
        };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        if (Utils.ConvertToInt32(dr["TrangThaiID"], 0) == EnumTrangThaiCanBo.DangLamViec.GetHashCode())
                        {
                            HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
                            canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                            canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                            canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                            //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                            //var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                            //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            Result.Add(canBo);

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

        public List<HeThongCanBoShortModel> GetThanNhanByCanBoID(int CanBoID)
        {
            List<HeThongCanBoShortModel> list = new List<HeThongCanBoShortModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CanBoID",SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetThanNhan_ByCanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoShortModel canBo = new HeThongCanBoShortModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.ThanNhanID = Utils.ConvertToInt32(dr["NV01001"], 0);
                        canBo.HoTenThanNhan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr["NV01003"], string.Empty));
                        list.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        /// <summary>
        /// Lấy chi tiết thông tin cán bộ cho chi tiết bản kê khai  
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <returns></returns>
        public HeThongCanBoPartialModel GetChiTietCanBoByID(int CanBoID)
        {
            HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetChiTiet_ByID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoPartialModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        //canBo.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        //canBo.NgayCap = Utils.ConvertToNullableDateTime(dr["NgayCap"], null);
                        //canBo.NoiCap = Utils.ConvertToString(dr["NoiCap"], string.Empty);
                        //var DanhSachChucVu = CanBoChucVu_GetChucVuCuaCanBo(CanBoID);
                        //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }

        public List<HeThongCanBoModel> GetAllInCoQuanCha(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@CoQuanID",SqlDbType.Int)
             };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInCoQuanCha", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        //var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        Result.Add(canBo);
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
        /// lấy tất cả cán bộ thuộc cấp quản lý và là cán bộ của đơn vị hiện tại và các đơn vị con
        /// </summary>
        /// <param name="CapQuanLy"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(int? CapQuanLy, int? CoQuanID)
        {
            List<HeThongCanBoModel> Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CapQuanLy",SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int)
            };
            parameters[0].Value = CapQuanLy ?? Convert.DBNull;
            parameters[1].Value = CoQuanID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var canbo = Result.Where(x => x.CanBoID == 10).ToList().FirstOrDefault();
            return Result;
        }

        /// <summary>
        /// Lấy cán bộ theo cơ quan và chức năng 
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <param name="ChucNangID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetCanBoByChucNang(int? CoQuanID, int? ChucNangID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CoQuanID", SqlDbType.Int),
                new SqlParameter("@ChucNangID", SqlDbType.Int)
            };
            parameters[0].Value = CoQuanID ?? 0;
            parameters[1].Value = ChucNangID ?? 0;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllByCoQuanAndChucNang", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        //var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        if (canBo.CanBoID != 1)
                            Result.Add(canBo);
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

        public HeThongCanBoModel GetDanhSachChucVu(int? CanBoID)
        {
            var Result = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
          {
                 new SqlParameter("@CanBoID", SqlDbType.Int)
          };
            parameters[0].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMucChucVu_GetByCanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        //Result.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //Result.ChucVuStr = Utils.ConvertToString(dr["DanhSachChucVu"], string.Empty);
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
        #endregion


        #region HT_CanBo_Chuc_Vu

        public int CanBoChucVu_Insert(List<int> ListChucVu, int CanBoID, ref string Message)
        {
            int val = 0;

            var table = new DataTable();
            table.Columns.Add("ID1", typeof(string));
            table.Columns.Add("ID2", typeof(string));
            for (int i = 0; i < ListChucVu.Count; i++)
            {
                var nrow = table.NewRow();
                nrow["ID1"] = CanBoID;
                nrow["ID2"] = ListChucVu[i];
                table.Rows.Add(nrow);
            }

            var pList = new SqlParameter("@list_idCanBo_idChucVu", SqlDbType.Structured);
            pList.TypeName = "dbo.id_id_list";
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
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_Insert", parameters), 0);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                    Message = ConstantLogMessage.Alert_Insert_Success("Cán bộ - chức vụ");
                    return val;
                }
            }

        }

        public List<CanBoChuVu> CanBoChucVu_GetBy_CanBoID(int CanBoID)
        {
            List<CanBoChuVu> Result = new List<CanBoChuVu>();
            SqlParameter[] parameters = new SqlParameter[]
           {
                 new SqlParameter("@CanBoID", SqlDbType.Int)
           };
            parameters[0].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_GetBy_CanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        var item = new CanBoChuVu();
                        item.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        item.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        item.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
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

        public List<DanhMucChucVuModel> CanBoChucVu_GetChucVuCuaCanBo(int CanBoID)
        {
            List<DanhMucChucVuModel> Result = new List<DanhMucChucVuModel>();
            SqlParameter[] parameters = new SqlParameter[]
           {
                 new SqlParameter("@CanBoID", SqlDbType.Int)
           };
            parameters[0].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_GetBy_CanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        var item = new DanhMucChucVuModel();
                        item.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        item.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        Result.Add(item);
                    }
                    dr.Close();
                }
                Result.GroupBy(x => x.ChucVuID).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public int CanBoChucVu_Delete_By_CanBoID(int CanBoID, ref string message)
        {
            message = "";
            var val = 0;

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter(@"CanBoID", SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_Delete_ByCanBoID", parameters);
                        trans.Commit();
                        if (val < 0)
                        {
                            message = "Không thể xóa chức vụ của cán bộ";
                        }
                        else message = ConstantLogMessage.Alert_Delete_Success("Cán bộ - chức vụ");
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return val;

        }

        public int GetCapQuanLyID(string TenCapQuanLy)
        {
            if (TenCapQuanLy == "Toàn tỉnh")
            {
                return 0;
            }
            else if (TenCapQuanLy == "Cấp tỉnh")
            {
                return 1;

            }

            else if (TenCapQuanLy == "Cấp huyện")
            {
                return 2;
            }

            return 0;
        }
        public List<CanBoChuVu> CanBoChucVu_GetAll()
        {
            List<CanBoChuVu> list = new List<CanBoChuVu>();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_GetAll"))
                {
                    while (dr.Read())
                    {
                        CanBoChuVu canBo = new CanBoChuVu();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        list.Add(canBo);
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

        public int InsertCanBoChucVu(Dictionary<int, int> ListCanBoChucVu)
        {
            var val = 0;
            var pList = new SqlParameter("@list_idCanBo_idChucVu", SqlDbType.Structured);
            pList.TypeName = "dbo.id_id_list";
            var tbChucVuCanBo = new DataTable();
            tbChucVuCanBo.Columns.Add("ChucVuID");
            tbChucVuCanBo.Columns.Add("CanBoID");
            foreach (var item in ListCanBoChucVu)
            {
                var newrow = tbChucVuCanBo.NewRow();
                newrow["ChucVuID"] = item.Key;
                newrow["CanBoID"] = item.Value;
                tbChucVuCanBo.Rows.Add(newrow);
            }
            SqlParameter[] parameters = new SqlParameter[]
           {
           pList
           };
            parameters[0].Value = tbChucVuCanBo;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_Insert", parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    return val;
                }
            }
        }

        // Get all cán bộ by coquanid
        public List<HeThongCanBoModel> GetAllCanBoByCoQuanID(int CoQuanID, int CoQuan_ID)
        {
            List<DanhMucCoQuanDonViModel> ListCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            if (CoQuanID <= 0)
            {
                ListCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuan_ID);
            }
            else
            {
                ListCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
            }
            List<int> list = new List<int>();
            ListCoQuanCon.ForEach(x => list.Add(x.CoQuanID));
            List<HeThongCanBoModel> ListCanBoByCoQuanID = new HeThongCanBoDAL().GetAllCanBoWithoutNguoiDung().Where(x => list.Contains(x.CoQuanID.Value)).ToList();
            return ListCanBoByCoQuanID;
        }

        // Get All Cán bộ by CoQuanID and ChucVuID
        public List<HeThongCanBoModel> GetAllCanBoByChucVuIDAndCoQuanID(int? ChucVuID, int? CoQuanID)
        {
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@ChucVuID",SqlDbType.Int),
            new SqlParameter("@CoQuanID",SqlDbType.Int)
            };
            parameters[0].Value = ChucVuID ?? Convert.DBNull;
            parameters[1].Value = CoQuanID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllCanBoByChucVuAndCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        //canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        //canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        list.Add(canBo);
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

        //Get Cán bộ by nguoidungid
        public HeThongCanBoModel GetCanBoByNguoiDungID(int? NguoiDungID)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@NguoiDungID",SqlDbType.Int)

            };
            parameters[0].Value = NguoiDungID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetCanBoByNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        //canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        //canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }

        // get cán bộ by chức vụ id
        public HeThongCanBoModel GetCanBoByChucVuID(int? ChucVuID)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@ChucVuID",SqlDbType.Int)

            };
            parameters[0].Value = ChucVuID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetCanBoByChucVuID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        //canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        //canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        //canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        //canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        //canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        //canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        //canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        //canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        //canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        //canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;

        }

        public List<HeThongCanBoModel> GetListByCoQuanID(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CoQuanID", SqlDbType.Int)
        };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        if (Utils.ConvertToInt32(dr["TrangThaiID"], 0) == EnumTrangThaiCanBo.DangLamViec.GetHashCode())
                        {
                            HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
                            canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                            canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                            canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            //var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                            //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            Result.Add(canBo);

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

        // Lấy danh sách cán bộ đã nghỉ hưu hoặc nghỉ việc
        public int GetListCanBo_Expire(int CanBoID, int CoQuanID)
        {
            var pList = new SqlParameter("@ListCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 2;
            var CoQuanQuanLy = 0;
            if (UserRole.CheckAdmin(CanBoID))
            {
                // TrangThai = 200;
                CapQuanLy = 0;
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
            {

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
            {
                //  TrangThai = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
            {
                // TrangThai = 300;
                CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
            {
                // TrangThai = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanID;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
            {

                //TrangThai = 300;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
            {
                //  TrangThai = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanID;

            }
            int val = 0;
            var listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy);
            var list = listCanBoAll.Where(x => x.TrangThaiID == 2).ToList();
            list.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@TrangThai",SqlDbType.Int),
              pList

           };
            parameters[0].Value = 0;
            parameters[1].Value = tbCanBoID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThongNguoiDung_UpdateTrangThai", parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                    return val;
                }
            }
        }
    }

    #endregion

}
