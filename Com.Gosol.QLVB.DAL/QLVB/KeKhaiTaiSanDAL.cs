using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IKeKhaiTaiSanDAL
    {
        public int Insert(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message);
        public int Update(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message);
        public KeKhaiTaiSanModel GetByID(int KeKhaiID);
        public List<KeKhaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? NamKeKhai);
        public string Export_Exel(string PathFile);
    }
    public class KeKhaiTaiSanDAL : IKeKhaiTaiSanDAL
    {
        //tên các store procedure
        private const string KE_KHAI_INSERT = @"v1_KeKhai_Insert";
        private const string KE_KHAI_UPDATE = @"v1_KeKhai_Update";
        private const string KE_KHAI_GET_BY_ID = @"v1_KeKhai_GetByID";
        private const string KE_KHAI_GET_ALL = @"v1_KeKhai_GetAll";

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
        private const string LOAI_DOT_KE_KHAI = "NV00106";

        // Insert
        public int Insert(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message)
        {
            int val = 0;
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,SqlDbType.NVarChar),
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                new SqlParameter(TRANG_THAI,SqlDbType.Int)
              };
            parameters[0].Value = KeKhaiTaiSanModel.DotKeKhaiID ?? Convert.DBNull;
            parameters[1].Value = KeKhaiTaiSanModel.CanBoID;
            parameters[2].Value = KeKhaiTaiSanModel.NamKeKhai ?? Convert.DBNull;
            parameters[3].Value = KeKhaiTaiSanModel.TrangThaiID ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {

                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KE_KHAI_INSERT, parameters);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            Message = ConstantLogMessage.Alert_Insert_Success("Kê Khai");
            return val;
        }

        // Update
        public int Update(KeKhaiTaiSanModel KeKhaiTaiSanModel, ref string Message)
        {
            int val = 0;
            if (KeKhaiTaiSanModel.KeKhaiID == 0)
            {
                Message = "Chưa có kê khai được chọn!";
                return val;
            }
  
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(KE_KHAI_ID,SqlDbType.Int),
                    new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int),
                    new SqlParameter(CAN_BO_ID,SqlDbType.NVarChar),
                    new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                    new SqlParameter(TRANG_THAI,SqlDbType.Int)
              };
            parameters[0].Value = KeKhaiTaiSanModel.KeKhaiID;
            parameters[1].Value = KeKhaiTaiSanModel.DotKeKhaiID ?? Convert.DBNull;
            parameters[2].Value = KeKhaiTaiSanModel.CanBoID ?? Convert.DBNull;
            parameters[3].Value = KeKhaiTaiSanModel.NamKeKhai ?? Convert.DBNull;
            parameters[4].Value = KeKhaiTaiSanModel.TrangThaiID ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KE_KHAI_UPDATE, parameters);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            Message = ConstantLogMessage.Alert_Update_Success("Kê Khai");
            return val;
        }
        // Delete

        public KeKhaiTaiSanModel GetByID(int KeKhaiID)
        {
            if (KeKhaiID <= 0)
            {
                return new KeKhaiTaiSanModel();
            }
            KeKhaiTaiSanModel KeKhai = new KeKhaiTaiSanModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(KE_KHAI_ID,SqlDbType.Int)
              };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KE_KHAI_GET_BY_ID, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhai = new KeKhaiTaiSanModel();
                        KeKhai.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        KeKhai.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        KeKhai.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        KeKhai.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                       KeKhai.TrangThaiID = Utils.ConvertToInt32(dr[TRANG_THAI], 0);

                    }
                    dr.Close();
                }
                return KeKhai;
            }
            catch
            {
                throw;
            }
        }

        public List<KeKhaiModel> GetAll()
        {
            List<KeKhaiModel> List = new List<KeKhaiModel>();
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
                        List.Add(KeKhai);
                    }
                    dr.Close();
                }
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }     

        // Get List Paging
        public List<KeKhaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? NamKeKhai)
        {
            List<KeKhaiTaiSanModel> list = new List<KeKhaiTaiSanModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter(NAM_KE_KHAI,SqlDbType.Int)

              };
            parameters[0].Value = p.Keyword != null ? p.Keyword.Trim() : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            //parameters[5].Value = 0;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = NamKeKhai ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_KeKhai_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiTaiSanModel KeKhai = new KeKhaiTaiSanModel(Utils.ConvertToInt32(dr[KE_KHAI_ID], 0), Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0), Utils.ConvertToInt32(dr[CAN_BO_ID], 0), Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0), Utils.ConvertToInt32(dr[TRANG_THAI], 0));
                        list.Add(KeKhai);

                    }

                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return list;
        }
        //Export exel
        public string Export_Exel(string PathFile)
        {
            try
            {
                string file = "";
                using (ExcelPackage package = new ExcelPackage(new FileInfo(PathFile)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Ke_Khai_Tai_San");
                    worksheet.TabColor = System.Drawing.Color.Black;
                    worksheet.DefaultRowHeight = 15;
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "Tên bản kê khai";
                    worksheet.Cells[1, 3].Value = "Năm kê khai";
                    worksheet.Cells[1, 4].Value = "Tên cán bộ";
                    worksheet.Cells[1, 5].Value = "Nơi công tác";
                    int recordIndex = 2;
                    List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
                    foreach (var item in GetAll().ToList())
                    {
                        KeKhaiModelPartial keKhaiModelPartial = new KeKhaiModelPartial
                        {
                            KeKhaiID = item.KeKhaiID,
                            DotKeKhaiID = item.DotKeKhaiID,
                            CanBoID = item.CanBoID,
                            NamKeKhai = item.NamKeKhai,
                            TrangThai = item.TrangThai,
                            TenBanKeKhai = string.Concat("Bản kê khai năm ", item.NamKeKhai.ToString()),
                            TenCanBo = new HeThongCanBoDAL().GetCanBoByID(item.CanBoID).TenCanBo.ToString(),
                            TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(new HeThongCanBoDAL().GetCanBoByID(item.CanBoID).CoQuanID).TenCoQuan.ToString()
                        };
                        list.Add(keKhaiModelPartial);
                    };
                    foreach (var item in list)
                    {
                        worksheet.Cells[recordIndex, 1].Value = item.KeKhaiID;
                        worksheet.Cells[recordIndex, 2].Value = item.TenBanKeKhai;
                        worksheet.Cells[recordIndex, 3].Value = item.NamKeKhai;
                        worksheet.Cells[recordIndex, 4].Value = item.TenCanBo;
                        worksheet.Cells[recordIndex, 5].Value = item.TenCoQuan;
                        recordIndex++;
                    }
                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    //package.Save();
                    Byte[] bytes = package.GetAsByteArray();
                    file = Convert.ToBase64String(bytes);
                    file = Convert.ToBase64String(bytes);
                    file = string.Concat("data:application/vnd.ms-excel;base64,", file);
                }

                return file;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}