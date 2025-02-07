using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models.HeThong;
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

namespace Com.Gosol.QLVB.DAL.HeThong
{
    public interface INguoiDungDAL
    {
        NguoiDungModel GetInfoByLogin(string UserName, string Password);
        NguoiDungModel GetInfoByLoginCAS(string Mail);
        public NguoiDungModel GetByTenNguoiDung(string TenNguoiDung);
        public int UpdateThoiGianlogin(NguoiDungModel nguoiDungModel, ref string message);
    }

    public class NguoiDungDAL : INguoiDungDAL
    {
        public NguoiDungModel GetInfoByLogin(string UserName, string Password)
        {
            NguoiDungModel user = null;
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return user;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"TenNguoiDung",SqlDbType.NVarChar),
                new SqlParameter(@"MatKhau",SqlDbType.NVarChar),
            };

            parameters[0].Value = UserName.Trim();
            parameters[1].Value = Password.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_NguoiDung_GetByLogin", parameters))
                {
                    if (dr.Read())
                    {
                        user = new NguoiDungModel();
                        user.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], "");
                        user.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        user.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        user.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        user.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        user.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        user.CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0);
                        user.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        user.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], "");
                        //// nếu người dùng có quyền quản lý cán bộ thì có quyền quản lý thân nhân
                        //var QuyenCuaCanBo = new ChucNangDAL().GetListChucNangByNguoiDungID(user.NguoiDungID);
                        //if (QuyenCuaCanBo.Any(x => x.ChucNangID == ChucNangEnum.HeThong_QuanLy_CanBo.GetHashCode())) user.QuanLyThanNhan = EnumVaiTroCanBo.LanhDao.GetHashCode();
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return user;
        }

        public NguoiDungModel GetInfoByLoginCAS(string Email)
        {
            NguoiDungModel user = null;
            if (string.IsNullOrEmpty(Email))
            {
                return user;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Email",SqlDbType.NVarChar)
            };

            parameters[0].Value = Email.Trim();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_NguoiDung_GetByLogin_CAS", parameters))
                {
                    if (dr.Read())
                    {
                        user = new NguoiDungModel();
                        user.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], "");
                        user.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        user.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        user.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        //user.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], "");
                        user.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        user.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        user.CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0);
                        //user.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        user.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], "");
                        //user.QuanLyThanNhan = user.VaiTro;
                        // nếu người dùng có quyền quản lý cán bộ thì có quyền quản lý thân nhân
                        var QuyenCuaCanBo = new ChucNangDAL().GetListChucNangByNguoiDungID(user.NguoiDungID);
                        //if (QuyenCuaCanBo.Any(x => x.ChucNangID == ChucNangEnum.HeThong_QuanLy_CanBo.GetHashCode())) user.QuanLyThanNhan = EnumVaiTroCanBo.LanhDao.GetHashCode();
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return user;
        }

        public NguoiDungModel GetByTenNguoiDung(string TenNguoiDung)
        {
            var NguoiDungModel = new NguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"TenNguoiDung",SqlDbType.NVarChar),
            };

            parameters[0].Value = TenNguoiDung.ToLower().Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_NguoiDung_GetByTenNguoiDung", parameters))
                {
                    if (dr.Read())
                    {
                        NguoiDungModel.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        NguoiDungModel.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], "");
                        NguoiDungModel.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        NguoiDungModel.ThoiGianLogin = Utils.ConvertToNullableDateTime(dr["ThoiGianLogin"], null);
                        NguoiDungModel.SoLanLogin = Utils.ConvertToInt32(dr["SoLanLogin"], 0);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NguoiDungModel;
        }

        public int UpdateThoiGianlogin(NguoiDungModel nguoiDungModel, ref string message)
        {
            var Result = 0;
            SqlParameter[] parameters = new SqlParameter[]
              {
                            new SqlParameter("@NguoiDungID", SqlDbType.Int),
                            new SqlParameter("@ThoiGianLogin", SqlDbType.DateTime),
                            new SqlParameter("@SoLanLogin", SqlDbType.Int),
                            new SqlParameter("@TrangThai", SqlDbType.TinyInt),

              };
            parameters[0].Value = nguoiDungModel.NguoiDungID;
            parameters[1].Value = nguoiDungModel.ThoiGianLogin ?? Convert.DBNull;
            parameters[2].Value = nguoiDungModel.SoLanLogin ?? Convert.DBNull;
            parameters[3].Value = nguoiDungModel.TrangThai;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        Result = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HT_NguoiDung_UpdateThoiGianLogin", parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            return Result;
        }
    }
}
