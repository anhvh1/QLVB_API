using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.Ultilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Com.Gosol.QLVB.DAL.QLVB
{
    public interface IMauPhieuDAL
    {
        public BaseResultModel Insert(MauPhieuModel _mauPhieuModel);
        public BaseResultModel Update(MauPhieuModel _mauPhieuModel);
        public List<string> Delete(List<int> DanhSachMauPhieuID);
        public List<MauPhieuModel> GetListPaging(BasePagingParams p, ref int TotalRow);
        public MauPhieuModel GetByID(int? MauPhieuID);
        public MauPhieuModel GetChiTietByNam(int? Nam, string TenMauPhieu);
        public string ImportDataToExcel(string rootpath, int? MauPhieuID);
        public DuLieuDiemThiModel ReadFileExcel(string FilePath, ref string Mess, int? NamTotNghiep);
        public DuLieuDiemThiModel ReadFileExcelUpdate(string FilePath, ref string Mess);
        public DuLieuDiemThiModel ReadFileExcel_AllPages(string FilePath, ref string Mess, int? NamTotNghiep);
        public List<MauPhieuModel> GetAll(ref int TotalRow);
        public List<MauPhieuModel> GetAll_VBNN(ref int TotalRow);
    }

    public class MauPhieuDAL : IMauPhieuDAL
    {
        public BaseResultModel Insert(MauPhieuModel _mauPhieuModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (_mauPhieuModel.TenMauPhieu == null || string.IsNullOrEmpty(_mauPhieuModel.TenMauPhieu))
                {
                    Result.Status = 0;
                    Result.Message = "Tên mẫu phiếu không được trống";
                    return Result;
                }
                if (_mauPhieuModel.TenMauPhieu.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên mẫu phiếu không được quá 200 ký tự";
                    return Result;
                }

                if (_mauPhieuModel.Nam == null || _mauPhieuModel.Nam == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Năm áp dụng mẫu phiếu không được đê trống!";
                    return Result;
                }
                //else
                //{
                //    var MauPhieuByNam = GetByNam(_mauPhieuModel.Nam);
                //    if (MauPhieuByNam != null && MauPhieuByNam.MauPhieuID > 0)
                //    {
                //        Result.Status = 0;
                //        Result.Message = "Năm bạn chọn đã có mẫu phiếu!";
                //        return Result;
                //    }
                //}

                if (_mauPhieuModel.DanhSachChiTietMauPhieu == null || _mauPhieuModel.DanhSachChiTietMauPhieu.Count <= 0)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn cột mẫu!";
                    return Result;
                }

                var crMauPhieu = GetMauPhieuByName(_mauPhieuModel.TenMauPhieu);
                if (crMauPhieu != null && crMauPhieu.MauPhieuID > 0)
                {
                    Result.Status = 0;
                    Result.Message = "Tên mẫu phiếu đã tồn tại!";
                    return Result;
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                       {
                            new SqlParameter("TenMauPhieu", SqlDbType.NVarChar),
                            new SqlParameter("GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("Nam", SqlDbType.Int),
                            new SqlParameter("MauPhieuID", SqlDbType.Int)

                       };
                    parameters[0].Value = _mauPhieuModel.TenMauPhieu.Trim();
                    parameters[1].Value = string.IsNullOrEmpty(_mauPhieuModel.GhiChu) ? Convert.DBNull : _mauPhieuModel.GhiChu.Trim(); parameters[2].Value = _mauPhieuModel.Nam ?? Convert.DBNull;
                    parameters[3].Direction = ParameterDirection.Output;
                    parameters[3].Size = 8;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_Insert", parameters);
                                if (Result.Status > 0)
                                {
                                    var tbChiTietMauPhieu = new DataTable();
                                    tbChiTietMauPhieu.Columns.Add("CotID");
                                    tbChiTietMauPhieu.Columns.Add("ThuTu");
                                    tbChiTietMauPhieu.Columns.Add("NhomID");
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.ForEach(x =>
                                    {
                                        tbChiTietMauPhieu.Rows.Add(x.CotID, x.ThuTu, x.NhomID);
                                    });
                                    SqlParameter[] param = new SqlParameter[]
                                    {
                                          new SqlParameter("MauPhieuID", SqlDbType.Int),
                                           new SqlParameter("DanhSachMauPhieuChiTiet", SqlDbType.Structured),
                                    };
                                    param[0].Value = Utils.ConvertToInt32(parameters[3].Value, 0);
                                    param[1].Value = tbChiTietMauPhieu;

                                    SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_ChiTietMauPhieu_Insert", param);
                                }
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("mẫu phiếu");
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

        public BaseResultModel Update(MauPhieuModel _mauPhieuModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (_mauPhieuModel.MauPhieuID == null || _mauPhieuModel.MauPhieuID.Value == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn mẫu phiếu cần chỉnh sửa!";
                    return Result;
                }
                var crMauPhieu = GetByID(_mauPhieuModel.MauPhieuID);
                if (crMauPhieu.TenMauPhieu.Trim() != _mauPhieuModel.TenMauPhieu.Trim())
                {
                    if (_mauPhieuModel == null || string.IsNullOrEmpty(_mauPhieuModel.TenMauPhieu))
                    {
                        Result.Status = 0;
                        Result.Message = "Tên mẫu phiếu không được trống";
                        return Result;
                    }
                    else
                    {
                        var _mauPhieu = GetMauPhieuByName(_mauPhieuModel.TenMauPhieu.Trim());
                        if (_mauPhieu.MauPhieuID > 0)
                        {
                            Result.Status = 0;
                            Result.Message = "Tên mẫu phiếu đã tồn tại!";
                            return Result;
                        }
                        else
                        {
                            if (_mauPhieuModel.TenMauPhieu.Trim().Length > 200)
                            {
                                Result.Status = 0;
                                Result.Message = "Tên mẫu phiếu không được quá 200 ký tự";
                                return Result;
                            }
                        }
                    }
                }

                //if (crMauPhieu.Nam != _mauPhieuModel.Nam)
                //{
                //    var _mauPhieu = GetByNam(_mauPhieuModel.Nam);
                //    if (_mauPhieu.MauPhieuID > 0)
                //    {
                //        Result.Status = 0;
                //        Result.Message = "Năm bạn chọn đã có mẫu phiếu!";
                //        return Result;
                //    }
                //}

                var tbChiTietMauPhieu = new DataTable();
                tbChiTietMauPhieu.Columns.Add("CotID");
                tbChiTietMauPhieu.Columns.Add("ThuTu");
                tbChiTietMauPhieu.Columns.Add("NhomID");
                _mauPhieuModel.DanhSachChiTietMauPhieu.ForEach(x =>
                {
                    tbChiTietMauPhieu.Rows.Add(x.CotID, x.ThuTu, x.NhomID);
                });
                SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter("TenMauPhieu", SqlDbType.NVarChar),
                        new SqlParameter("GhiChu", SqlDbType.NVarChar),
                        new SqlParameter("Nam", SqlDbType.Int),
                        new SqlParameter("MauPhieuID", SqlDbType.Int),
                        new SqlParameter("@DanhSachChiTietMauPhieu", SqlDbType.Structured),

                };

                parameters[0].Value = _mauPhieuModel.TenMauPhieu.Trim() ?? Convert.DBNull;
                parameters[1].Value = string.IsNullOrEmpty(_mauPhieuModel.GhiChu) ? Convert.DBNull : _mauPhieuModel.GhiChu.Trim();
                parameters[2].Value = _mauPhieuModel.Nam ?? Convert.DBNull;
                parameters[3].Value = _mauPhieuModel.MauPhieuID ?? Convert.DBNull;
                parameters[4].Value = tbChiTietMauPhieu;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_Update", parameters);
                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Update_Success("mẫu phiếu");
                            return Result;
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
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
        }

        public bool CheckMauPhieuDuocSuDung(int MauPhieuID)
        {
            var result = false;
            var ThiSinhID = 0;
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"MauPhieuID",SqlDbType.Int)
              };
            parameters[0].Value = MauPhieuID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThiSinh_GetByMauPhieuID", parameters))
                {
                    while (dr.Read())
                    {
                        ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        if (ThiSinhID > 0)
                        {
                            result = true;
                            break;
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<string> Delete(List<int> DanhSachMauPhieuID)
        {
            List<string> dic = new List<string>();

            string message = "";
            if (DanhSachMauPhieuID.Count <= 0)
            {
                message = "Vui lòng chọn mẫu phiếu cần xóa!";
                dic.Add(message);
                //return dic;
            }
            else
            {
                //var Seccess = 0;
                for (int i = 0; i < DanhSachMauPhieuID.Count; i++)
                {
                    var MauPhieuCuaBoID = Utils.ConvertToInt32(new SystemConfigDAL().GetByKey("FileExportExcelBo").ConfigValue, 0);
                    if (CheckMauPhieuDuocSuDung(DanhSachMauPhieuID[i]))
                    {
                        MauPhieuModel mauPhieuModel = GetByID(DanhSachMauPhieuID[i]);
                        message = "'" + mauPhieuModel.TenMauPhieu + "'" + " đang được sử dụng! Không thể xoá!";
                        dic.Add(message);
                    }
                    else if (MauPhieuCuaBoID > 0 && MauPhieuCuaBoID == DanhSachMauPhieuID[i])
                    {
                        MauPhieuModel mauPhieuModel = GetByID(MauPhieuCuaBoID);
                        message = "'" + mauPhieuModel.TenMauPhieu + "'" + " là mẫu phiếu của bộ! Không thể xoá!";
                        dic.Add(message);
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                         {
                             new SqlParameter("MauPhieuID", SqlDbType.Int)

                        };
                        parameters[0].Value = DanhSachMauPhieuID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_Delete", parameters);
                                    trans.Commit();
                                    //if (val == 0)
                                    //{
                                    //    message = "Không thể xóa từ Chức vụ  " + GetChucVuByID(ListChucVuID[i]).TenChucVu;
                                    //    dic.Add(0, message);
                                    //    return dic;
                                    //}

                                }
                                catch
                                {
                                    trans.Rollback();
                                    throw;
                                }
                            }
                        }
                    }
                }

            }
            return dic;
        }

        public List<MauPhieuModel> GetListPaging(BasePagingParams p, ref int TotalRow)
        {
            List<MauPhieuModel> list = new List<MauPhieuModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = p.Nam ?? Convert.DBNull;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetListPaging", parameters))
                {
                    while (dr.Read())
                    {
                        MauPhieuModel item = new MauPhieuModel();
                        item.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        item.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        item.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        list.Add(item);
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

        public MauPhieuModel GetMauPhieuByName(string TenMauPhieu)
        {
            if (string.IsNullOrEmpty(TenMauPhieu))
            {
                return new MauPhieuModel();
            }
            MauPhieuModel mauPhieu = new MauPhieuModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenMauPhieu",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenMauPhieu.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetByName", parameters))
                {
                    while (dr.Read())
                    {
                        mauPhieu.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        mauPhieu.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        mauPhieu.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        mauPhieu.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mauPhieu;
        }

        public MauPhieuModel GetByNam(int? Nam)
        {
            if (Nam == null || Nam == 0)
            {
                return new MauPhieuModel();
            }
            MauPhieuModel mauPhieu = new MauPhieuModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"Nam",SqlDbType.Int)

              };
            parameters[0].Value = Nam;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetByNam", parameters))
                {
                    while (dr.Read())
                    {
                        mauPhieu.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        mauPhieu.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        mauPhieu.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        mauPhieu.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mauPhieu;
        }

        public MauPhieuModel GetByID(int? MauPhieuID)
        {
            if (MauPhieuID == null || MauPhieuID == 0)
            {
                return new MauPhieuModel();
            }
            MauPhieuModel mauPhieu = new MauPhieuModel();
            var DanhSachChiTietTemp = new List<ChiTietMauPhieuTemp>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"MauPhieuID",SqlDbType.Int)
              };
            parameters[0].Value = MauPhieuID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietMauPhieuTemp chiTietMauPhieuModel = new ChiTietMauPhieuTemp();
                        chiTietMauPhieuModel.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        chiTietMauPhieuModel.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        chiTietMauPhieuModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        chiTietMauPhieuModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        chiTietMauPhieuModel.CotID = Utils.ConvertToInt32(dr["CotID"], 0);
                        chiTietMauPhieuModel.TieuDeCot = Utils.ConvertToString(dr["Ten"], string.Empty);
                        chiTietMauPhieuModel.MaCot = Utils.ConvertToString(dr["Ma"], string.Empty);
                        chiTietMauPhieuModel.Loai = Utils.ConvertToNullableInt32(dr["Loai"], null);
                        chiTietMauPhieuModel.ViTri = Utils.ConvertToString(dr["ViTri"], null);
                        chiTietMauPhieuModel.NhomID = Utils.ConvertToInt32(dr["NhomID"], 0);
                        DanhSachChiTietTemp.Add(chiTietMauPhieuModel);
                    }
                    dr.Close();
                }
                mauPhieu = (from gr1 in DanhSachChiTietTemp
                            group gr1 by new { gr1.MauPhieuID, gr1.TenMauPhieu, gr1.Nam, gr1.GhiChu } into temp
                            select new MauPhieuModel()
                            {
                                MauPhieuID = temp.Key.MauPhieuID,
                                TenMauPhieu = temp.Key.TenMauPhieu,
                                Nam = temp.Key.Nam,
                                GhiChu = temp.Key.GhiChu,
                                DanhSachChiTietMauPhieu = (from gr1 in temp
                                                           where gr1.Loai != EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()
                                                           select new ChiTietMauPhieuModel()
                                                           {
                                                               CotID = gr1.CotID,
                                                               TieuDeCot = gr1.TieuDeCot,
                                                               MaCot = gr1.MaCot,
                                                               ThuTu = gr1.ThuTu,
                                                               Loai = gr1.Loai,
                                                               ViTri = gr1.ViTri,
                                                               NhomID = gr1.NhomID,
                                                               DanhSachCon = (from gr2 in temp
                                                                              where gr1.NhomID != 0 && gr2.NhomID == gr1.NhomID && (gr1.MaCot.Contains("ĐL12") || gr1.MaCot.Contains("ĐT") || gr1.MaCot.Contains("ĐPK") || gr1.MaCot.Contains("ĐBL") || gr1.MaCot.Contains("BODY_KETLUAN") || gr1.MaCot.Contains("BODY_DIEMTHICUCHA")) && gr1.CotID != gr2.CotID
                                                                              select new ChiTietMauPhieuModel()
                                                                              {
                                                                                  CotID = gr2.CotID,
                                                                                  TieuDeCot = gr2.TieuDeCot,
                                                                                  MaCot = gr2.MaCot + "||" + gr2.NhomID,
                                                                                  ThuTu = gr2.ThuTu,
                                                                                  Loai = gr2.Loai,
                                                                                  ViTri = gr2.ViTri,
                                                                                  NhomID = gr2.NhomID
                                                                              }).ToList()
                                                           }).OrderBy(x => x.ThuTu).ToList()
                            }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mauPhieu;
        }

        public MauPhieuModel GetChiTietByNam(int? Nam, string TenMauPhieu)
        {
            if (Nam == null || Nam == 0)
            {
                return new MauPhieuModel();
            }
            MauPhieuModel mauPhieu = new MauPhieuModel();
            var DanhSachChiTietTemp = new List<ChiTietMauPhieuTemp>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"Nam",SqlDbType.Int),
                new SqlParameter(@"TenMauPhieu",SqlDbType.NVarChar)
              };
            parameters[0].Value = Nam;
            parameters[1].Value = TenMauPhieu.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetChiTietByNam", parameters))
                {
                    while (dr.Read())
                    {
                        var chiTietMauPhieuModel = new ChiTietMauPhieuTemp();
                        chiTietMauPhieuModel.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        chiTietMauPhieuModel.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        chiTietMauPhieuModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        chiTietMauPhieuModel.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        chiTietMauPhieuModel.CotID = Utils.ConvertToInt32(dr["CotID"], 0);
                        chiTietMauPhieuModel.TieuDeCot = Utils.ConvertToString(dr["Ten"], string.Empty);
                        chiTietMauPhieuModel.MaCot = Utils.ConvertToString(dr["Ma"], string.Empty);
                        chiTietMauPhieuModel.Loai = Utils.ConvertToNullableInt32(dr["Loai"], null);
                        chiTietMauPhieuModel.ViTri = Utils.ConvertToString(dr["ViTri"], null);
                        chiTietMauPhieuModel.NhomID = Utils.ConvertToInt32(dr["NhomID"], 0);
                        DanhSachChiTietTemp.Add(chiTietMauPhieuModel);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //mauPhieu = (from gr1 in DanhSachChiTietTemp
            //            group gr1 by new { gr1.MauPhieuID, gr1.TenMauPhieu, gr1.Nam, gr1.GhiChu } into temp
            //            select new MauPhieuModel()
            //            {
            //                MauPhieuID = temp.Key.MauPhieuID,
            //                TenMauPhieu = temp.Key.TenMauPhieu,
            //                Nam = temp.Key.Nam,
            //                GhiChu = temp.Key.GhiChu,
            //                DanhSachChiTietMauPhieu = (from gr1 in temp
            //                                           select new ChiTietMauPhieuModel()
            //                                           {
            //                                               CotID = gr1.CotID,
            //                                               TieuDeCot = gr1.TieuDeCot,
            //                                               MaCot = gr1.MaCot,
            //                                               ThuTu = gr1.ThuTu,
            //                                               Loai = gr1.Loai,
            //                                               ViTri = gr1.ViTri
            //                                           }).OrderBy(x => x.ThuTu).ToList()
            //            }).FirstOrDefault();
            mauPhieu = (from gr1 in DanhSachChiTietTemp
                        group gr1 by new { gr1.MauPhieuID, gr1.TenMauPhieu, gr1.Nam, gr1.GhiChu } into temp
                        select new MauPhieuModel()
                        {
                            MauPhieuID = temp.Key.MauPhieuID,
                            TenMauPhieu = temp.Key.TenMauPhieu,
                            Nam = temp.Key.Nam,
                            GhiChu = temp.Key.GhiChu,
                            DanhSachChiTietMauPhieu = (from gr1 in temp
                                                       where gr1.Loai != EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()
                                                       select new ChiTietMauPhieuModel()
                                                       {
                                                           CotID = gr1.CotID,
                                                           TieuDeCot = gr1.TieuDeCot,
                                                           MaCot = gr1.MaCot,
                                                           ThuTu = gr1.ThuTu,
                                                           Loai = gr1.Loai,
                                                           ViTri = gr1.ViTri,
                                                           NhomID = gr1.NhomID,
                                                           DanhSachCon = (from gr2 in temp
                                                                          where gr1.NhomID != 0 && gr2.NhomID == gr1.NhomID && (gr1.MaCot.Contains("ĐL12") || gr1.MaCot.Contains("ĐT") || gr1.MaCot.Contains("ĐPK") || gr1.MaCot.Contains("ĐBL")) && gr1.CotID != gr2.CotID
                                                                          select new ChiTietMauPhieuModel()
                                                                          {
                                                                              CotID = gr2.CotID,
                                                                              TieuDeCot = gr2.TieuDeCot,
                                                                              MaCot = gr2.MaCot + "||" + gr2.NhomID,
                                                                              ThuTu = gr2.ThuTu,
                                                                              Loai = gr2.Loai,
                                                                              ViTri = gr2.ViTri,
                                                                              NhomID = gr2.NhomID
                                                                          }).ToList()
                                                       }).OrderBy(x => x.ThuTu).ToList()
                        }).FirstOrDefault();
            return mauPhieu;
        }

        public string ImportDataToExcel_Old(string rootPath, int? MauPhieuID)
        {
            var Data = GetByID(MauPhieuID);
            if (Data == null || Data.MauPhieuID == 0 || Data.MauPhieuID == null)
            {
                return "";
            }

            bool isFolder = Directory.Exists(rootPath + "\\Upload\\Temp\\");
            if (!isFolder)
            {
                Directory.CreateDirectory(rootPath + "\\Upload\\Temp\\");
            }

            string path = rootPath + @"\Upload\BieuMau.xlsx";
            FileInfo fileInfo = new FileInfo(path);
            string FilePath = @"Upload\Temp\" + Utils.RemoveSpecialCharacters(Data.TenMauPhieu) + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            FileInfo file = new FileInfo(rootPath + "\\" + FilePath);
            ExcelPackage package = new ExcelPackage(fileInfo);

            if (package.Workbook.Worksheets != null)
            {
                var DanhSachNguoiKy = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode());

                package.Workbook.Worksheets.Add("CanBoKy");
                ExcelWorksheet pkCanBoKy = package.Workbook.Worksheets["CanBoKy"];
                if (DanhSachNguoiKy != null && DanhSachNguoiKy.Count > 0)
                {
                    int index = 1;
                    for (int i = 0; i < DanhSachNguoiKy.Count; i++)
                    {
                        pkCanBoKy.Cells[index, 1].Value = DanhSachNguoiKy[i].Ten;
                        pkCanBoKy.Cells[index, 2].Value = DanhSachNguoiKy[i].ID;
                        index++;
                    }
                }
                package.Workbook.Worksheets["CanBoKy"].Hidden = eWorkSheetHidden.Hidden;


                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                if (Data.DanhSachChiTietMauPhieu != null && Data.DanhSachChiTietMauPhieu.Count > 0)
                {
                    var DanhSachDau = Data.DanhSachChiTietMauPhieu.Where(x => x.Loai == EnumLoaiDanhMuc.DM_BieuMau_HEAD.GetHashCode()).ToList();
                    var DanhSachNoiDungBangDiem = Data.DanhSachChiTietMauPhieu.Where(x => x.Loai == EnumLoaiDanhMuc.DM_BieuMau_BODY.GetHashCode()).ToList();
                    var DanhSachCuoi = Data.DanhSachChiTietMauPhieu.Where(x => x.Loai == EnumLoaiDanhMuc.DM_BieuMau_FOOT.GetHashCode()).ToList();
                    var CountCotNoiDung = DanhSachNoiDungBangDiem.Count;
                    //generate phần đầu
                    int rowHead_T = 1;
                    int colHead_T = 1;



                    foreach (var item in DanhSachDau.Where(x => x.ViTri == "T").ToList())
                    {

                        if (item.MaCot.Contains("HEAD_HD"))
                        {
                            if (!package.Workbook.Worksheets.Contains(package.Workbook.Worksheets["HoiDongThi"]))
                            {
                                var DanhSachHoiDong = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_HoiDong.GetHashCode());
                                package.Workbook.Worksheets.Add("HoiDongThi");
                                ExcelWorksheet pkHoiDongThi = package.Workbook.Worksheets["HoiDongThi"];
                                if (DanhSachHoiDong != null && DanhSachHoiDong.Count > 0)
                                {
                                    int index = 1;
                                    for (int i = 0; i < DanhSachHoiDong.Count; i++)
                                    {
                                        pkHoiDongThi.Cells[index, 1].Value = DanhSachHoiDong[i].Ten;
                                        pkHoiDongThi.Cells[index, 2].Value = DanhSachHoiDong[i].ID;
                                        index++;
                                    }
                                }
                                package.Workbook.Worksheets["HoiDongThi"].Hidden = eWorkSheetHidden.Hidden;
                            }

                            worksheet.Cells[rowHead_T, colHead_T].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_T, colHead_T].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_T, colHead_T].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 1].Merge = true;

                            var temp = worksheet.Cells[rowHead_T, colHead_T + 2].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            //DanhSachHoiDong.ForEach(x => temp.Formula.Values.Add(x.Ten));
                            temp.Formula.ExcelFormula = "=HoiDongThi!$A$1:$A$1000";
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 2].Style.Locked = false;

                        }
                        else if (item.MaCot.Contains("HEAD_KT"))
                        {
                            worksheet.Cells[rowHead_T, colHead_T].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_T, colHead_T].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_T, colHead_T].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 1].Merge = true;
                            if (!package.Workbook.Worksheets.Contains(package.Workbook.Worksheets["KhoaThi"]))
                            {
                                var DanHSachKhoaThiTheoNam = new DanhMucKhoaThiDAL().GetByNam(Data.Nam);
                                package.Workbook.Worksheets.Add("KhoaThi");
                                ExcelWorksheet pkKhoaThi = package.Workbook.Worksheets["KhoaThi"];
                                if (DanHSachKhoaThiTheoNam != null && DanHSachKhoaThiTheoNam.Count > 0)
                                {
                                    int index = 1;
                                    for (int i = 0; i < DanHSachKhoaThiTheoNam.Count; i++)
                                    {
                                        pkKhoaThi.Cells[index, 1].Value = DanHSachKhoaThiTheoNam[i].TenKhoaThi;
                                        pkKhoaThi.Cells[index, 2].Value = DanHSachKhoaThiTheoNam[i].KhoaThiID;
                                        index++;
                                    }
                                }
                                package.Workbook.Worksheets["KhoaThi"].Hidden = eWorkSheetHidden.Hidden;
                            }

                            var temp = worksheet.Cells[rowHead_T, colHead_T + 2].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            //DanHSachKhoaThiTheoNam.ForEach(x => temp.Formula.Values.Add(x.TenKhoaThi));
                            temp.Formula.ExcelFormula = "=KhoaThi!$A$1:$A$1000";
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 2].Style.Locked = false;
                        }
                        else if (item.MaCot.Contains("HEAD_B"))
                        {
                            worksheet.Cells[rowHead_T, colHead_T].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_T, colHead_T].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_T, colHead_T].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 1].Merge = true;
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 2].Style.Locked = false;
                        }

                        else
                        {
                            worksheet.Cells[rowHead_T, colHead_T].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_T, colHead_T].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_T, colHead_T].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 1].Merge = true;
                            worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 2].Style.Locked = false;
                        }
                        rowHead_T++;
                    }
                    int rowHead_P = 1;
                    int colHead_P = 1;

                    colHead_P = CountCotNoiDung > 0 ? CountCotNoiDung - 2 : colHead_P + 2;
                    worksheet.Cells[rowHead_P, colHead_P].Value = "Số quyển: ";
                    worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                    worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;

                    worksheet.Cells[rowHead_P + 1, colHead_P].Value = "Trang: ";
                    worksheet.Cells[rowHead_P + 1, colHead_P].Style.Font.Bold = true;
                    worksheet.Cells[rowHead_P + 1, colHead_P, rowHead_P + 1, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowHead_P + 1, colHead_P, rowHead_P + 1, colHead_P + 1].Merge = true;
                    rowHead_P = rowHead_P + 2;

                    foreach (var item in DanhSachDau.Where(x => x.ViTri == "P").ToList())
                    {
                        if (item.MaCot.Contains("HEAD_PT"))
                        {
                            colHead_P = CountCotNoiDung > 0 ? CountCotNoiDung - 2 : colHead_P + 2;
                            worksheet.Cells[rowHead_P, colHead_P].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;
                        }
                        else if (item.MaCot.Contains("SBD"))
                        {
                            colHead_P = CountCotNoiDung > 0 ? CountCotNoiDung - 2 : colHead_P + 2;
                            worksheet.Cells[rowHead_P, colHead_P].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;
                        }
                        else
                        {
                            worksheet.Cells[rowHead_P, colHead_P].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;
                        }
                        rowHead_P++;
                    }
                    worksheet.Cells[1, colHead_P + 1, rowHead_P, colHead_P + 2].Style.Locked = false;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //generate phần bảng điểm 
                    int rowBody = rowHead_T > rowHead_P ? rowHead_T + 1 : rowHead_P + 1;
                    int colBody = 1;
                    int colBodyTemp = 0;
                    int CountSoCotGom = 0;
                    int CountTemp = 0;//đếm số cột có danh sách con
                    int RowTempForLock = 0;//sử dụng để đánh dấu phần ko cho chỉnh sửa
                    bool HasDanhSachCon = DanhSachNoiDungBangDiem.Where(x => x.DanhSachCon.Count > 0).FirstOrDefault() != null ? true : false;
                    foreach (var item in DanhSachNoiDungBangDiem.OrderBy(x => x.ThuTu).ToList())
                    {
                        if (item.MaCot.Contains("STT"))
                        {
                            worksheet.Column(colBody).Width = 7;
                        }
                        else if (item.MaCot.Contains("SBD"))
                        {
                            worksheet.Column(colBody).Width = 14.3;
                        }
                        else if (item.MaCot.Contains("BODY_HT"))
                        {
                            worksheet.Column(colBody).Width = 29;
                            worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }
                        else if (item.MaCot.Contains("BODY_NgS"))
                        {
                            worksheet.Column(colBody).Width = 14.29;
                        }
                        else if (item.MaCot.Contains("BODY_NS"))
                        {
                            worksheet.Column(colBody).Width = 17;
                        }
                        else if (item.MaCot.Equals("BODY_GT"))
                        {
                            worksheet.Column(colBody).Width = 8.43;
                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            temp.Formula.Values.Add("Nam");
                            temp.Formula.Values.Add("Nữ");
                        }
                        else if (item.MaCot.Equals("BODY_DT"))
                        {
                            if (!package.Workbook.Worksheets.Contains(package.Workbook.Worksheets["DanToc"]))
                            {
                                var DanhSachDanToc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_DanToc.GetHashCode());
                                package.Workbook.Worksheets.Add("DanToc");
                                ExcelWorksheet pkDanToc = package.Workbook.Worksheets["DanToc"];
                                if (DanhSachDanToc != null && DanhSachDanToc.Count > 0)
                                {
                                    int index = 1;
                                    for (int i = 0; i < DanhSachDanToc.Count; i++)
                                    {
                                        pkDanToc.Cells[index, 1].Value = DanhSachDanToc[i].Ten;
                                        pkDanToc.Cells[index, 2].Value = DanhSachDanToc[i].ID;
                                        index++;
                                    }
                                }
                                package.Workbook.Worksheets["DanToc"].Hidden = eWorkSheetHidden.Hidden;
                            }

                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            temp.Formula.ExcelFormula = "=DanToc!$A$1:$A$100";
                        }
                        else if (item.MaCot.Contains("BODY_HST"))
                        {
                            worksheet.Column(colBody).Width = 33;
                            if (!package.Workbook.Worksheets.Contains(package.Workbook.Worksheets["Truong"]))
                            {
                                var DanhSachTruong = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_Truong.GetHashCode());
                                package.Workbook.Worksheets.Add("Truong");
                                ExcelWorksheet pkTruong = package.Workbook.Worksheets["Truong"];
                                if (DanhSachTruong != null && DanhSachTruong.Count > 0)
                                {
                                    int index = 1;
                                    for (int i = 0; i < DanhSachTruong.Count; i++)
                                    {
                                        pkTruong.Cells[index, 1].Value = DanhSachTruong[i].Ten;
                                        pkTruong.Cells[index, 2].Value = DanhSachTruong[i].ID;
                                        index++;
                                    }
                                }
                                package.Workbook.Worksheets["Truong"].Hidden = eWorkSheetHidden.Hidden;
                            }

                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            temp.Formula.ExcelFormula = "=Truong!$A$1:$A$1000";
                        }
                        else if (item.MaCot.Contains("BODY_HK"))
                        {
                            var DanhSachHanhKiem = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode());
                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            DanhSachHanhKiem.OrderBy(x => x.Ten).ToList().ForEach(x => temp.Formula.Values.Add(x.Ten));
                        }
                        else if (item.MaCot.Contains("BODY_GC"))
                        {
                            worksheet.Column(colBody).Width = 18;
                        }
                        else if (item.MaCot.Contains("BODY_CMND/CCCD"))
                        {
                            worksheet.Column(colBody).Width = 21;
                        }
                        else if (item.MaCot.Contains("BODY_SĐT"))
                        {
                            worksheet.Column(colBody).Width = 18;
                        }
                        else if (item.MaCot.Contains("BODY_ĐKK"))
                        {
                            worksheet.Column(colBody).Width = 9.5;
                        }
                        else if (item.MaCot.Contains("BODY_XLTN") || item.MaCot.Contains("BODY_HL") || item.MaCot.Contains("BODY_HANG"))
                        {
                            var DanhSachXepLoaiTN = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_XepLoai.GetHashCode());
                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            DanhSachXepLoaiTN.OrderBy(x => x.Ten).ToList().ForEach(x => temp.Formula.Values.Add(x.Ten));
                        }
                        worksheet.Cells[rowBody, colBody].Value = item.TieuDeCot;
                        if (HasDanhSachCon)
                        {
                            worksheet.Cells[rowBody + 2, colBody].Value = item.MaCot;
                            worksheet.Row(rowBody + 2).Hidden = true;
                            RowTempForLock = rowBody + 2;
                            worksheet.Row(rowBody + 1).Style.Font.Bold = true;
                        }
                        else
                        {
                            worksheet.Cells[rowBody + 1, colBody].Value = item.MaCot;
                            worksheet.Row(rowBody + 1).Hidden = true;
                            RowTempForLock = rowBody + 1;
                        }
                        worksheet.Row(rowBody).Height = 52;
                        worksheet.Row(rowBody).Style.WrapText = true;
                        worksheet.Row(rowBody).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Row(rowBody).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(rowBody).Style.Font.Bold = true;
                        if (item.DanhSachCon.Count > 0)
                        {
                            CountSoCotGom += item.DanhSachCon.Count;
                            CountTemp++;
                            colBodyTemp = colBody;
                            foreach (var itemCon in item.DanhSachCon)
                            {
                                worksheet.Cells[rowBody + 1, colBody].Value = itemCon.TieuDeCot;
                                worksheet.Cells[rowBody + 2, colBody].Value = itemCon.MaCot;
                                colBody++;
                            }
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Merge = true;
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Style.WrapText = true;
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        }
                        else
                        {
                            if (HasDanhSachCon)
                            {
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Merge = true;
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Style.WrapText = true;
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                            colBody++;
                        }
                    }

                    //format cell border cho phần bảng điểm
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Top.Color.SetColor(Color.Black);
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Bottom.Color.SetColor(Color.Black);
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Right.Color.SetColor(Color.Black);
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Left.Color.SetColor(Color.Black);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Căn giữa text cho tất cả phần băng điểm
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //định dạng giá trịn của tất cả các ô trong bảng điểm là dạng text
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Numberformat.Format = "@";
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //trừ phần băng điểm được chỉnh sửa, nhữg phần khác ko được chỉnh sửa
                    //worksheet.Cells[rowBody + 3, 1, rowBody + 28, worksheet.Dimension.End.Column].Style.Locked = false;
                    worksheet.Cells[RowTempForLock, 1, RowTempForLock + 28, worksheet.Dimension.End.Column].Style.Locked = false;
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //generate phần cuối
                    //đánh dấu mốc là đến Foot để hàm ReadExcel nhận biết
                    worksheet.Cells[rowBody + 29, 1].Value = "FOOT";
                    worksheet.Cells[rowBody + 29, 2].Value = Data.Nam;
                    worksheet.Cells[rowBody + 29, 3].Value = Data.TenMauPhieu;
                    worksheet.Row(rowBody + 29).Hidden = true;
                    int rowFoot = rowBody + 30;
                    int colFoot = 1;
                    int CountFoot = DanhSachCuoi.Count;
                    //Chia đôi phần cuôi
                    int totalCol = CountCotNoiDung + CountSoCotGom;
                    //int colChiaTotal = totalCol / 2;
                    var DanhSachFootNhap = DanhSachCuoi.Where(x => x.NhomID == EnumLoaiDanhMuc.Nhom_CuoiMau_NhapTay.GetHashCode()).OrderBy(x => x.TieuDeCot.Length).ToList();
                    var DanhSachKy = DanhSachCuoi.Where(x => x.NhomID == 0 || x.NhomID == null).ToList();
                    //int RowFootTemp = 0;
                    foreach (var item in DanhSachFootNhap)
                    {
                        if (item.MaCot.Contains("FOOT_ĐCNTN"))
                        {
                            worksheet.Cells[rowFoot, colFoot].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Merge = true;
                            worksheet.Row(rowFoot).Height = 35.25;
                            worksheet.Row(rowFoot).Style.WrapText = true;
                            worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowFoot, colFoot + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowFoot, colFoot + 2].Style.Locked = false;
                        }
                        else if (item.MaCot.Contains("FOOT_KĐCNTN"))
                        {
                            worksheet.Cells[rowFoot, colFoot].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Merge = true;
                            worksheet.Row(rowFoot).Height = 35.25;
                            worksheet.Row(rowFoot).Style.WrapText = true;
                            worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowFoot, colFoot + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowFoot, colFoot + 2].Style.Locked = false;
                        }
                        else
                        {
                            worksheet.Cells[rowFoot, colFoot].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Merge = true;
                            worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowFoot, colFoot + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowFoot, colFoot + 2].Style.Locked = false;
                        }
                        rowFoot++;
                    }
                    rowFoot = rowBody + 30;
                    int khoangCach = (totalCol - 3) / (DanhSachKy.Count + 1);
                    if (DanhSachFootNhap != null && DanhSachFootNhap.Count > 0)
                    {
                        colFoot = colFoot + 4;
                    }

                    //var DanhSachNguoiKy = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode());
                    int cotTemp = 0;
                    foreach (var item in DanhSachKy)
                    {
                        cotTemp = colFoot + khoangCach;
                    }
                    if (cotTemp < totalCol)
                        cotTemp = totalCol;
                    else
                    {
                        if (DanhSachKy != null && DanhSachKy.Count > 0)
                        {
                            cotTemp = cotTemp + DanhSachKy.Count;
                        }
                    }

                    foreach (var item in DanhSachKy)
                    {
                        worksheet.Cells[rowFoot, colFoot].Value = item.TieuDeCot;
                        worksheet.Cells[rowFoot, colFoot].Style.Font.Bold = true;
                        var temp = worksheet.Cells[rowFoot + 2, colFoot].DataValidation.AddListDataValidation();
                        //DanhSachNguoiKy.ForEach(x => temp.Formula.Values.Add(x.Ten));
                        temp.PromptTitle = "Thông báo lỗi!";
                        temp.Prompt = "Thông báo lỗi!";
                        temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                        temp.ShowErrorMessage = true;
                        worksheet.Cells[rowFoot + 2, colFoot].Style.Locked = false;
                        temp.Formula.ExcelFormula = "=CanBoKy!$A$1:$A$1000";

                        //tạo cột ẩn để so sánh lấy ra cán bộ ID từ sheet ẩn với hàm Vlookup
                        worksheet.Cells[rowFoot, cotTemp + 1].Value = item.MaCot;
                        worksheet.Cells[rowFoot + 2, cotTemp + 1].FormulaR1C1 = "VLOOKUP(R" + (rowFoot + 2) + "C" + colFoot + ", CanBoKy!$A$1:$B$100000,2,0)";//VLOOKUP(TRIM(G12),DM_DoanhNghiep!$A$1:$B$100000,2,0)
                        worksheet.Column(cotTemp + 1).Hidden = true;
                        worksheet.Column(cotTemp + 1).Style.Locked = true;
                        cotTemp++;

                        colFoot = colFoot + (khoangCach == 0 ? 1 : khoangCach);
                    }

                    worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Font.Size = 14;
                }
                worksheet.Protection.IsProtected = true;
                worksheet.Protection.AllowDeleteRows = true;
                worksheet.Protection.AllowEditObject = true;
                worksheet.Protection.AllowFormatColumns = true;
                worksheet.Protection.AllowFormatRows = true;
                worksheet.Protection.AllowInsertRows = true;
                worksheet.Protection.AllowSort = true;
                worksheet.Protection.AllowEditScenarios = true;
                worksheet.Protection.AllowFormatCells = true;
                worksheet.Protection.AllowFormatColumns = true;
                worksheet.Protection.AllowPivotTables = true;
                package.SaveAs(file);
            }
            return FilePath;
        }

        public string ImportDataToExcel(string rootPath, int? MauPhieuID)
        {
            var Data = GetByID(MauPhieuID);
            if (Data == null || Data.MauPhieuID == 0 || Data.MauPhieuID == null)
            {
                return "";
            }

            bool isFolder = Directory.Exists(rootPath + "\\Upload\\Temp\\");
            if (!isFolder)
            {
                Directory.CreateDirectory(rootPath + "\\Upload\\Temp\\");
            }

            string path = rootPath + @"\Upload\BieuMau.xlsx";
            FileInfo fileInfo = new FileInfo(path);
            var TenMauPhieu = Regex.Replace(Data.TenMauPhieu, "[@,\\.\":'\\\\]", string.Empty);
            string FilePath = @"Upload\Temp\" + Utils.RemoveSpecialCharacters(TenMauPhieu) + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            FileInfo file = new FileInfo(rootPath + "\\" + FilePath);
            ExcelPackage package = new ExcelPackage(fileInfo);
            //ExcelPackage package = new ExcelPackage(file);
            //package.Workbook.Worksheets.Add(Data.TenMauPhieu);
            if (package.Workbook.Worksheets != null)
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                if (Data.DanhSachChiTietMauPhieu != null && Data.DanhSachChiTietMauPhieu.Count > 0)
                {
                    var DanhSachDau = Data.DanhSachChiTietMauPhieu.Where(x => x.Loai == EnumLoaiDanhMuc.DM_BieuMau_HEAD.GetHashCode()).ToList();
                    var DanhSachNoiDungBangDiem = Data.DanhSachChiTietMauPhieu.Where(x => x.Loai == EnumLoaiDanhMuc.DM_BieuMau_BODY.GetHashCode()).ToList();
                    var DanhSachCuoi = Data.DanhSachChiTietMauPhieu.Where(x => x.Loai == EnumLoaiDanhMuc.DM_BieuMau_FOOT.GetHashCode()).ToList();
                    var CountCotNoiDung = DanhSachNoiDungBangDiem.Count;

                    //đánh dấu ID của mẫu phiếu
                    worksheet.Cells[1, 1].Value = Data.MauPhieuID;
                    worksheet.Cells[1, 2].Value = Data.TenMauPhieu;
                    worksheet.Row(1).Hidden = true;
                    worksheet.Row(1).Style.Locked = true;

                    //generate phần đầu
                    int rowHead_T = 2;
                    int colHead_T = 1;


                    var DanhSachDauBenTrai = DanhSachDau.Where(x => x.ViTri == "T").ToList();
                    //foreach (var item in DanhSachDau.Where(x => x.ViTri == "T").ToList())
                    foreach (var item in DanhSachDauBenTrai)
                    {
                        worksheet.Cells[rowHead_T, colHead_T].Value = item.TieuDeCot + ": ";
                        worksheet.Cells[rowHead_T, colHead_T].Style.Font.Bold = true;
                        worksheet.Cells[rowHead_T, colHead_T].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 1].Merge = true;
                        worksheet.Cells[rowHead_T, colHead_T, rowHead_T, colHead_T + 2].Style.Locked = false;
                        worksheet.Cells[rowHead_T, colHead_T].Style.Locked = true;
                        rowHead_T++;
                    }
                    //if (DanhSachDauBenTrai != null && DanhSachDauBenTrai.Count <= 2)
                    //{
                    //    worksheet.Cells[3, colHead_T].Value = "";
                    //}
                    int rowHead_P = 2;
                    int colHead_P = 1;

                    colHead_P = CountCotNoiDung > 0 ? CountCotNoiDung - 2 : colHead_P + 2;
                    worksheet.Cells[rowHead_P, colHead_P].Value = "Số quyển: ";
                    worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                    worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;

                    worksheet.Cells[rowHead_P + 1, colHead_P].Value = "Trang: ";
                    worksheet.Cells[rowHead_P + 1, colHead_P].Style.Font.Bold = true;
                    worksheet.Cells[rowHead_P + 1, colHead_P, rowHead_P + 1, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowHead_P + 1, colHead_P, rowHead_P + 1, colHead_P + 1].Merge = true;
                    rowHead_P = rowHead_P + 2;

                    foreach (var item in DanhSachDau.Where(x => x.ViTri == "P").ToList())
                    {
                        if (item.MaCot.Contains("HEAD_PT"))
                        {
                            colHead_P = CountCotNoiDung > 0 ? CountCotNoiDung - 2 : colHead_P + 2;
                            worksheet.Cells[rowHead_P, colHead_P].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;
                        }
                        else if (item.MaCot.Contains("SBD"))
                        {
                            colHead_P = CountCotNoiDung > 0 ? CountCotNoiDung - 2 : colHead_P + 2;
                            worksheet.Cells[rowHead_P, colHead_P].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;
                        }
                        else
                        {
                            worksheet.Cells[rowHead_P, colHead_P].Value = item.TieuDeCot + ": ";
                            worksheet.Cells[rowHead_P, colHead_P].Style.Font.Bold = true;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowHead_P, colHead_P, rowHead_P, colHead_P + 1].Merge = true;
                        }
                        rowHead_P++;
                    }
                    worksheet.Cells[1, colHead_P + 1, rowHead_P, colHead_P + 2].Style.Locked = false;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //generate phần bảng điểm 
                    int rowBody = rowHead_T > rowHead_P ? rowHead_T + 1 : rowHead_P + 1;
                    int colBody = 1;
                    int colBodyTemp = 0;
                    int CountSoCotGom = 0;
                    int CountTemp = 0;//đếm số cột có danh sách con
                    int RowTempForLock = 0;//sử dụng để đánh dấu phần ko cho chỉnh sửa
                    int colHoTen = 0;
                    bool HasDanhSachCon = DanhSachNoiDungBangDiem.Where(x => x.DanhSachCon.Count > 0).FirstOrDefault() != null ? true : false;
                    foreach (var item in DanhSachNoiDungBangDiem.OrderBy(x => x.ThuTu).ToList())
                    {
                        if (item.MaCot.Contains("STT"))
                        {
                            worksheet.Column(colBody).Width = 7;
                        }
                        else if (item.MaCot.Contains("SBD"))
                        {
                            worksheet.Column(colBody).Width = 14.3;
                        }
                        else if (item.MaCot.Contains("BODY_HT"))
                        {
                            worksheet.Column(colBody).Width = 29;
                            worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            colHoTen = colBody;
                        }
                        else if (item.MaCot.Contains("BODY_NgS"))
                        {
                            worksheet.Column(colBody).Width = 14.29;
                        }
                        else if (item.MaCot.Contains("BODY_NS"))
                        {
                            worksheet.Column(colBody).Width = 17;
                        }
                        else if (item.MaCot.Equals("BODY_GT"))
                        {
                            worksheet.Column(colBody).Width = 8.43;
                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            temp.Formula.Values.Add("Nam");
                            temp.Formula.Values.Add("Nữ");
                        }
                        else if (item.MaCot.Equals("BODY_DT"))
                        {
                            if (!package.Workbook.Worksheets.Contains(package.Workbook.Worksheets["DanToc"]))
                            {
                                var DanhSachDanToc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_DanToc.GetHashCode());
                                package.Workbook.Worksheets.Add("DanToc");
                                ExcelWorksheet pkDanToc = package.Workbook.Worksheets["DanToc"];
                                if (DanhSachDanToc != null && DanhSachDanToc.Count > 0)
                                {
                                    int index = 1;
                                    for (int i = 0; i < DanhSachDanToc.Count; i++)
                                    {
                                        pkDanToc.Cells[index, 1].Value = DanhSachDanToc[i].Ten;
                                        pkDanToc.Cells[index, 2].Value = DanhSachDanToc[i].ID;
                                        index++;
                                    }
                                }
                                package.Workbook.Worksheets["DanToc"].Hidden = eWorkSheetHidden.Hidden;
                            }

                            var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            temp.PromptTitle = "Thông báo lỗi!";
                            temp.Prompt = "Thông báo lỗi!";
                            temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            temp.ShowErrorMessage = true;
                            temp.Formula.ExcelFormula = "=DanToc!$A$1:$A$100";
                        }
                        else if (item.MaCot.Contains("BODY_HST"))
                        {
                            worksheet.Column(colBody).Width = 33;
                            //if (!package.Workbook.Worksheets.Contains(package.Workbook.Worksheets["Truong"]))
                            //{
                            //    var DanhSachTruong = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_Truong.GetHashCode());
                            //    package.Workbook.Worksheets.Add("Truong");
                            //    ExcelWorksheet pkTruong = package.Workbook.Worksheets["Truong"];
                            //    if (DanhSachTruong != null && DanhSachTruong.Count > 0)
                            //    {
                            //        int index = 1;
                            //        for (int i = 0; i < DanhSachTruong.Count; i++)
                            //        {
                            //            pkTruong.Cells[index, 1].Value = DanhSachTruong[i].Ten;
                            //            pkTruong.Cells[index, 2].Value = DanhSachTruong[i].ID;
                            //            index++;
                            //        }
                            //    }
                            //    package.Workbook.Worksheets["Truong"].Hidden = eWorkSheetHidden.Hidden;
                            //}

                            //var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                            //temp.PromptTitle = "Thông báo lỗi!";
                            //temp.Prompt = "Thông báo lỗi!";
                            //temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                            //temp.ShowErrorMessage = true;
                            //temp.Formula.ExcelFormula = "=Truong!$A$1:$A$1000";
                        }
                        else if (item.MaCot.Contains("BODY_HK"))
                        {
                            var DanhSachHanhKiem = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode());
                            if (DanhSachHanhKiem != null && DanhSachHanhKiem.Count > 0)
                            {
                                var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                                temp.PromptTitle = "Thông báo lỗi!";
                                temp.Prompt = "Thông báo lỗi!";
                                temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                                temp.ShowErrorMessage = true;
                                DanhSachHanhKiem.OrderBy(x => x.Ten).ToList().ForEach(x => temp.Formula.Values.Add(x.Ten));
                            }

                        }
                        else if (item.MaCot.Contains("BODY_GC"))
                        {
                            worksheet.Column(colBody).Width = 18;
                        }
                        else if (item.MaCot.Contains("BODY_CMND/CCCD"))
                        {
                            worksheet.Column(colBody).Width = 21;
                        }
                        else if (item.MaCot.Contains("BODY_SĐT"))
                        {
                            worksheet.Column(colBody).Width = 18;
                        }
                        else if (item.MaCot.Contains("BODY_ĐKK"))
                        {
                            worksheet.Column(colBody).Width = 9.5;
                        }
                        else if (item.MaCot.Contains("BODY_XLTN") || item.MaCot.Contains("BODY_HL") || item.MaCot.Contains("BODY_HANG"))
                        {
                            var DanhSachXepLoaiTN = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_XepLoai.GetHashCode());
                            if (DanhSachXepLoaiTN != null && DanhSachXepLoaiTN.Count > 0)
                            {
                                var temp = worksheet.Cells[rowBody + 2, colBody, rowBody + 28, colBody].DataValidation.AddListDataValidation();
                                temp.PromptTitle = "Thông báo lỗi!";
                                temp.Prompt = "Thông báo lỗi!";
                                temp.Error = "Vui lòng chọn dữ liệu trong danh sách!";
                                temp.ShowErrorMessage = true;
                                DanhSachXepLoaiTN.OrderBy(x => x.Ten).ToList().ForEach(x => temp.Formula.Values.Add(x.Ten));
                            }
                        }
                        else if (item.MaCot.Contains("BODY_SHB"))
                        {
                            worksheet.Column(colBody).Width = 19.5;
                        }
                        else if (item.MaCot.Contains("BODY_VSCBS"))
                        {
                            worksheet.Column(colBody).Width = 20;
                        }
                        else if (item.MaCot.Contains("BODY_NCB"))
                        {
                            worksheet.Column(colBody).Width = 13.6;
                        }
                        else if (item.MaCot.Contains("BODY_ĐVĐKDT"))
                        {
                            worksheet.Column(colBody).Width = 30;
                        }
                        worksheet.Cells[rowBody, colBody].Value = item.TieuDeCot;
                        if (HasDanhSachCon)
                        {
                            worksheet.Cells[rowBody + 2, colBody].Value = item.MaCot;
                            worksheet.Row(rowBody + 2).Hidden = true;
                            RowTempForLock = rowBody + 2;
                            worksheet.Row(rowBody + 1).Style.Font.Bold = true;
                        }
                        else
                        {
                            worksheet.Cells[rowBody + 1, colBody].Value = item.MaCot;
                            worksheet.Row(rowBody + 1).Hidden = true;
                            RowTempForLock = rowBody + 1;
                        }
                        worksheet.Row(rowBody).Height = 52;
                        worksheet.Row(rowBody).Style.WrapText = true;
                        worksheet.Row(rowBody).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Row(rowBody).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(rowBody).Style.Font.Bold = true;
                        if (item.DanhSachCon.Count > 0)
                        {
                            CountSoCotGom += item.DanhSachCon.Count;
                            CountTemp++;
                            colBodyTemp = colBody;
                            foreach (var itemCon in item.DanhSachCon)
                            {
                                worksheet.Cells[rowBody + 1, colBody].Value = itemCon.TieuDeCot;
                                worksheet.Cells[rowBody + 2, colBody].Value = itemCon.MaCot;
                                colBody++;
                            }
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Merge = true;
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Style.WrapText = true;
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[rowBody, colBodyTemp, rowBody, colBody - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        }
                        else
                        {
                            if (HasDanhSachCon)
                            {
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Merge = true;
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Style.WrapText = true;
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells[rowBody, colBody, rowBody + 1, colBody].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                            colBody++;
                        }
                    }

                    //format cell border cho phần bảng điểm
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Top.Color.SetColor(Color.Black);
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Bottom.Color.SetColor(Color.Black);
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Right.Color.SetColor(Color.Black);
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Border.Left.Color.SetColor(Color.Black);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Căn giữa text cho tất cả phần băng điểm
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rowBody + 2, colHoTen, rowBody + 28, colHoTen].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //định dạng giá trịn của tất cả các ô trong bảng điểm là dạng text
                    worksheet.Cells[rowBody, 1, rowBody + 28, CountCotNoiDung + CountSoCotGom - CountTemp].Style.Numberformat.Format = "@";
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //trừ phần băng điểm được chỉnh sửa, nhữg phần khác ko được chỉnh sửa
                    //worksheet.Cells[rowBody + 3, 1, rowBody + 28, worksheet.Dimension.End.Column].Style.Locked = false;
                    worksheet.Cells[RowTempForLock, 1, RowTempForLock + 28, worksheet.Dimension.End.Column].Style.Locked = false;
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //generate phần cuối
                    //đánh dấu mốc là đến Foot để hàm ReadExcel nhận biết
                    worksheet.Cells[rowBody + 29, 1].Value = "FOOT";
                    worksheet.Cells[rowBody + 29, 2].Value = Data.Nam;
                    worksheet.Cells[rowBody + 29, 3].Value = Data.TenMauPhieu;
                    worksheet.Row(rowBody + 29).Hidden = true;
                    int rowFoot = rowBody + 30;
                    int colFoot = 1;
                    int CountFoot = DanhSachCuoi.Count;
                    //Chia đôi phần cuôi
                    int totalCol = CountCotNoiDung + CountSoCotGom;
                    foreach (var item in DanhSachCuoi)
                    {
                        worksheet.Cells[rowFoot, colFoot].Value = item.TieuDeCot + ": ";
                        worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Merge = true;
                        if (item.MaCot == "FOOT_ĐCNTN" || item.MaCot == "FOOT_KĐCNTN" || item.MaCot == "FOOT_CTHĐ_COITHI")
                        {
                            worksheet.Row(rowFoot).Height = 35.25;
                        }
                        worksheet.Row(rowFoot).Style.WrapText = true;
                        worksheet.Cells[rowFoot, colFoot, rowFoot, colFoot + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowFoot, colFoot + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowFoot, colFoot + 2].Style.Locked = false;
                        worksheet.Cells[rowFoot, colFoot].Style.Locked = true;
                        rowFoot++;
                    }

                    worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].Style.Font.Size = 14;
                }
                worksheet.Protection.IsProtected = true;
                worksheet.Protection.AllowDeleteRows = true;
                worksheet.Protection.AllowEditObject = true;
                worksheet.Protection.AllowFormatColumns = true;
                worksheet.Protection.AllowFormatRows = true;
                worksheet.Protection.AllowInsertRows = true;
                worksheet.Protection.AllowSort = true;
                worksheet.Protection.AllowEditScenarios = true;
                worksheet.Protection.AllowFormatCells = true;
                worksheet.Protection.AllowFormatColumns = true;
                worksheet.Protection.AllowPivotTables = true;
                package.SaveAs(file);
            }
            return FilePath;
        }

        //public DuLieuDiemThiModel ReadFileExcel_Old(string FilePath, ref string Mess)
        //{
        //    var result = new DuLieuDiemThiModel();
        //    result.ThongTinThiSinh = new List<ThongTinThiSinh>();
        //    if (!File.Exists(FilePath))
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
        //        {
        //            var totalWorksheets = package.Workbook.Worksheets.Count;
        //            if (totalWorksheets <= 0)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
        //                int StartDataBody = 0;
        //                int EndDataBody = 0;
        //                var row = workSheet.Dimension.End.Row;
        //                var col = workSheet.Dimension.End.Column;
        //                int Nam = 0;
        //                string TenMauPhieu = "";
        //                for (int i = 1; i <= workSheet.Dimension.End.Row; i++)
        //                {

        //                    for (int j = 1; j < workSheet.Dimension.End.Column; j++)
        //                    {
        //                        if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("BODY"))
        //                        {
        //                            StartDataBody = i + 1;
        //                            break;
        //                        }
        //                    }

        //                    for (int j = 1; j < workSheet.Dimension.End.Column; j++)
        //                    {
        //                        if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT"))
        //                        {
        //                            EndDataBody = i - 2;
        //                            Nam = Utils.ConvertToInt32(workSheet.Cells[i, j + 1].Value, 0);
        //                            TenMauPhieu = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            break;
        //                        }
        //                    }

        //                }

        //                result.ChiTietMauPhieu = GetChiTietByNam(Nam, TenMauPhieu);

        //                #region đọc data đầu
        //                //var DanhSachHoiDong = new List<DanhMucChungModel>();
        //                //DanhSachHoiDong = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_HoiDong.GetHashCode());
        //                var DanhMucChung = new List<DanhMucChungModel>();
        //                DanhMucChung = new DanhMucChungDAL().GetAll(0);
        //                var DanhSachKhoaNgayThi = new List<DanhMucKhoaThiModel>();
        //                BasePagingParams p = new BasePagingParams();
        //                p.PageSize = 100000;
        //                int total = 0;
        //                DanhSachKhoaNgayThi = new DanhMucKhoaThiDAL().GetPagingBySearch(p, ref total);

        //                //int HoiDongThiID = 0;
        //                //int HoiDongChamThiID = 0;
        //                //int HoiDongCoiThiID = 0;
        //                //int HoiDongGiamThiID = 0;
        //                //int HoiDongGiamKhaoID = 0;
        //                //int KhoaThiID = 0;
        //                var ThongTinToChucThiModel = new ThongTinToChucThi();
        //                for (int i = 1; i < StartDataBody - 1; i++)
        //                {
        //                    for (int j = 1; j < workSheet.Dimension.End.Column; j++)
        //                    {
        //                        if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng thi"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            if (!string.IsNullOrEmpty(temp))
        //                            {
        //                                var hoiDongModel = DanhMucChung.Where(x => x.Ten.ToLower().Contains(temp.ToLower()) && x.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode()).FirstOrDefault();
        //                                if (hoiDongModel != null && hoiDongModel.ID > 0)
        //                                {
        //                                    ThongTinToChucThiModel.HoiDongThiID = hoiDongModel.ID;
        //                                }
        //                                else
        //                                {
        //                                    ////nếu hội đồng chưa có trong DB thì insert vào DB
        //                                    //var DanhMucChungModel = new DanhMucChungModel();
        //                                    //DanhMucChungModel.Ten = temp;
        //                                    //DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
        //                                    //var insertTemp = new DanhMucChungDAL().Insert(DanhMucChungModel);
        //                                    //ThongTinToChucThiModel.HoiDongThiID = Utils.ConvertToInt32(insertTemp.Data, 0);
        //                                }
        //                            }

        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng chấm thi"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            if (!string.IsNullOrEmpty(temp))
        //                            {
        //                                var hoiDongModel = DanhMucChung.Where(x => x.Ten.ToLower().Contains(temp.ToLower()) && x.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode()).FirstOrDefault();
        //                                if (hoiDongModel != null && hoiDongModel.ID > 0)
        //                                {
        //                                    ThongTinToChucThiModel.HoiDongChamThiID = hoiDongModel.ID;
        //                                }
        //                                else
        //                                {
        //                                    ////nếu hội đồng chưa có trong DB thì insert vào DB
        //                                    //var DanhMucChungModel = new DanhMucChungModel();
        //                                    //DanhMucChungModel.Ten = temp;
        //                                    //DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
        //                                    //var insertTemp = new DanhMucChungDAL().Insert(DanhMucChungModel);
        //                                    //ThongTinToChucThiModel.HoiDongChamThiID = Utils.ConvertToInt32(insertTemp.Data, 0);
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng coi thi"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            if (!string.IsNullOrEmpty(temp))
        //                            {
        //                                var hoiDongModel = DanhMucChung.Where(x => x.Ten.ToLower().Contains(temp.ToLower()) && x.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode()).FirstOrDefault();
        //                                if (hoiDongModel != null && hoiDongModel.ID > 0)
        //                                {
        //                                    ThongTinToChucThiModel.HoiDongCoiThiID = hoiDongModel.ID;
        //                                }
        //                                else
        //                                {
        //                                    //    //nếu hội đồng chưa có trong DB thì insert vào DB
        //                                    //    var DanhMucChungModel = new DanhMucChungModel();
        //                                    //    DanhMucChungModel.Ten = temp;
        //                                    //    DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
        //                                    //    var insertTemp = new DanhMucChungDAL().Insert(DanhMucChungModel);
        //                                    //    ThongTinToChucThiModel.HoiDongCoiThiID = Utils.ConvertToInt32(insertTemp.Data, 0);
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám thị"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            if (!string.IsNullOrEmpty(temp))
        //                            {
        //                                var hoiDongModel = DanhMucChung.Where(x => x.Ten.ToLower().Contains(temp.ToLower()) && x.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode()).FirstOrDefault();
        //                                if (hoiDongModel != null && hoiDongModel.ID > 0)
        //                                {
        //                                    ThongTinToChucThiModel.HoiDongGiamThiID = hoiDongModel.ID;
        //                                }
        //                                else
        //                                {
        //                                    ////nếu hội đồng chưa có trong DB thì insert vào DB
        //                                    //var DanhMucChungModel = new DanhMucChungModel();
        //                                    //DanhMucChungModel.Ten = temp;
        //                                    //DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
        //                                    //var insertTemp = new DanhMucChungDAL().Insert(DanhMucChungModel);
        //                                    //ThongTinToChucThiModel.HoiDongGiamThiID = Utils.ConvertToInt32(insertTemp.Data, 0);
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám khảo"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            if (!string.IsNullOrEmpty(temp))
        //                            {
        //                                var hoiDongModel = DanhMucChung.Where(x => x.Ten.ToLower().Contains(temp.ToLower()) && x.Loai == EnumLoaiDanhMuc.DM_HoiDong.GetHashCode()).FirstOrDefault();
        //                                if (hoiDongModel != null && hoiDongModel.ID > 0)
        //                                {
        //                                    ThongTinToChucThiModel.HoiDongGiamKhaoID = hoiDongModel.ID;
        //                                }
        //                                else
        //                                {
        //                                    ////nếu hội đồng chưa có trong DB thì insert vào DB
        //                                    //var DanhMucChungModel = new DanhMucChungModel();
        //                                    //DanhMucChungModel.Ten = temp;
        //                                    //DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
        //                                    //var insertTemp = new DanhMucChungDAL().Insert(DanhMucChungModel);
        //                                    //ThongTinToChucThiModel.HoiDongGiamKhaoID = Utils.ConvertToInt32(insertTemp.Data, 0);
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("khoá ngày thi"))
        //                        {

        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            var khoaNgayThi = DanhSachKhoaNgayThi.Where(x => x.TenKhoaThi.ToLower().Contains(temp.ToLower())).FirstOrDefault();
        //                            if (khoaNgayThi != null && khoaNgayThi.KhoaThiID > 0)
        //                            {
        //                                ThongTinToChucThiModel.KhoaThiID = khoaNgayThi.KhoaThiID;
        //                            }
        //                            else
        //                            {
        //                                ////nếu khoá ngày thi chưa có trong DB thì insert vào DB
        //                                //var DanhMucChungModel = new DanhMucChungModel();
        //                                //DanhMucChungModel.Ten = temp;
        //                                //DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_HoiDong.GetHashCode();
        //                                //var insertTemp = new DanhMucChungDAL().Insert(DanhMucChungModel);
        //                                //ThongTinToChucThiModel.HoiDongGiamKhaoID = Utils.ConvertToInt32(insertTemp.Data, 0);
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ban"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            ThongTinToChucThiModel.Ban = temp;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("phòng thi"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            ThongTinToChucThiModel.PhongThi = temp;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("từ sbd"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            ThongTinToChucThiModel.SBDDau = temp;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("đến sbd"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            ThongTinToChucThiModel.SBDCuoi = temp;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số quyển"))
        //                        {
        //                            var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
        //                            ThongTinToChucThiModel.SoQuyen = temp;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("trang"))
        //                        {
        //                            var temp = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].Value, null);
        //                            ThongTinToChucThiModel.SoTrang = temp;
        //                        }
        //                    }
        //                }

        //                #endregion

        //                #region đọc data bảng điểm

        //                //var DanhSachDanToc = new List<DanhMucChungModel>();
        //                //DanhSachDanToc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_DanToc.GetHashCode());

        //                //var DanhSachMonHoc = new List<DanhMucChungModel>();
        //                //DanhSachMonHoc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_MonHoc.GetHashCode());

        //                for (int i = StartDataBody; i <= EndDataBody; i++)
        //                {
        //                    var ThiSinh = new ThongTinThiSinh();
        //                    ThiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
        //                    ThiSinh.HoiDongThi = ThongTinToChucThiModel.HoiDongThiID;
        //                    for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
        //                    {
        //                        //var test = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
        //                        if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SBD"))
        //                        {
        //                            ThiSinh.SoBaoDanh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_STT"))
        //                        {
        //                            ThiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HT"))
        //                        {
        //                            ThiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NgS"))
        //                        {
        //                            ThiSinh.NgaySinh = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NS"))
        //                        {
        //                            ThiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GT"))
        //                        {
        //                            var _gioiTinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                            if (_gioiTinh.ToLower() == "nam")
        //                            {
        //                                ThiSinh.GioiTinh = false;
        //                            }
        //                            else if (_gioiTinh.ToLower() == "nữ")
        //                            {
        //                                ThiSinh.GioiTinh = true;
        //                            }
        //                            //ThiSinh.GioiTinh = Utils.ConvertToBoolean(workSheet.Cells[i, j].RichText.Text, false);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DT"))
        //                        {
        //                            //ThiSinh.DanToc = Utils.ConvertToInt32(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                            string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                            if (!string.IsNullOrEmpty(DT))
        //                            {
        //                                //var DanToc = DanhSachDanToc.Where(x => x.Ten.ToLower().Contains(DT)).FirstOrDefault();
        //                                var DanToc = DanhMucChung.Where(x => x.Ten.Contains(DT) && x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).FirstOrDefault();
        //                                if (DanToc != null && DanToc.ID > 0)
        //                                {
        //                                    ThiSinh.DanToc = DanToc.ID;
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HST"))
        //                        {
        //                            var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                            var TruongModel = DanhMucChung.Where(x => x.Ten.Contains(TenTruong) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
        //                            if (TruongModel != null && TruongModel.ID > 0)
        //                            {
        //                                ThiSinh.TruongTHPT = TruongModel.ID;
        //                                ThiSinh.TenTruongTHPT = TruongModel.Ten;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HK"))
        //                        {
        //                            var TenHanhKiem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                            if (!string.IsNullOrEmpty(TenHanhKiem))
        //                            {
        //                                var HanhKiemModel = DanhMucChung.Where(x => x.Ten.Contains(TenHanhKiem) && x.Loai == EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode()).FirstOrDefault();
        //                                if (HanhKiemModel != null && HanhKiemModel.ID > 0)
        //                                {
        //                                    ThiSinh.XepLoaiHanhKiem = HanhKiemModel.ID;
        //                                    ThiSinh.XepLoaiHanhKiemStr = HanhKiemModel.Ten;
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HL"))
        //                        {
        //                            var TenHocLuc = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                            if (!string.IsNullOrEmpty(TenHocLuc))
        //                            {
        //                                var HocLucModel = DanhMucChung.Where(x => x.Ten.Contains(TenHocLuc) && x.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode()).FirstOrDefault();
        //                                if (HocLucModel != null && HocLucModel.ID > 0)
        //                                {
        //                                    ThiSinh.XepLoaiHocLuc = HocLucModel.ID;
        //                                    ThiSinh.XepLoaiHocLucStr = HocLucModel.Ten;
        //                                }
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXTN"))
        //                        {
        //                            ThiSinh.DiemXetTotNghiep = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐKK"))
        //                        {
        //                            ThiSinh.DiemKK = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
        //                        {
        //                            ThiSinh.TongSoDiemThi = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXL"))
        //                        {
        //                            ThiSinh.DiemXL = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐL12"))
        //                        {
        //                            ThiSinh.DiemTBLop12 = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐƯT"))
        //                        {
        //                            ThiSinh.DiemUT = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_Đô"))
        //                        {
        //                            ThiSinh.Do = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐôT"))
        //                        {
        //                            ThiSinh.DoThem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_H"))
        //                        {
        //                            ThiSinh.Hong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLTN") /*|| Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG")*/)
        //                        {
        //                            ThiSinh.KetQuaTotNghiep = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GC"))
        //                        {
        //                            ThiSinh.GhiChu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CMND/CCCD"))
        //                        {
        //                            ThiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SĐT"))
        //                        {
        //                            ThiSinh.SoDienThoai = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_L"))
        //                        {
        //                            ThiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LDT"))
        //                        {
        //                            ThiSinh.LoaiDuThi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐVĐKDT"))
        //                        {
        //                            ThiSinh.DonViDKDT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LĐ"))
        //                        {
        //                            ThiSinh.LaoDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VH"))
        //                        {
        //                            ThiSinh.VanHoa = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_RLTT"))
        //                        {
        //                            ThiSinh.RLTT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SHB"))
        //                        {
        //                            ThiSinh.SoHieuBang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
        //                        {
        //                            ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NCB"))
        //                        {
        //                            ThiSinh.NgayCapBang = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
        //                        {
        //                            ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
        //                        {
        //                            ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG"))
        //                        {
        //                            ThiSinh.Hang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, null);
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Contains("MH"))
        //                        {
        //                            var thongTinDiemMonHoc = new ThongTinDiemThi();
        //                            thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
        //                            if (thongTinDiemMonHoc.Diem != null)
        //                            {
        //                                var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
        //                                var arrTemp = temp.Split("||");
        //                                thongTinDiemMonHoc.NhomID = arrTemp.Length > 0 ? Utils.ConvertToNullableInt32(arrTemp[1], null) : null;
        //                                var TenMonHoc = Utils.ConvertToString(workSheet.Cells[StartDataBody - 2, j].Value, string.Empty);
        //                                var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
        //                                if (MonHocModel != null && MonHocModel.ID > 0)
        //                                {
        //                                    thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
        //                                }

        //                                ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
        //                            }
        //                        }
        //                    }
        //                    if (/*string.IsNullOrEmpty(ThiSinh.SoBaoDanh) && */!string.IsNullOrEmpty(ThiSinh.HoTen))
        //                    {
        //                        result.ThongTinThiSinh.Add(ThiSinh);
        //                    }

        //                }

        //                #endregion

        //                #region đọc data cuối mẫu phiếu
        //                //var DanhSachNguoiKy = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode());
        //                for (int i = EndDataBody + 2; i <= workSheet.Dimension.End.Row; i++)
        //                {
        //                    for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
        //                    {
        //                        //var test = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower();
        //                        //if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("người đọc điểm"))
        //                        //{
        //                        //    var tenNguoiDocDiem = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var nguoiDocDiemModel = DanhSachNguoiKy.Where(x => x.Ten == tenNguoiDocDiem).FirstOrDefault();
        //                        //    if (nguoiDocDiemModel != null && nguoiDocDiemModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.NguoiDocDiem = nguoiDocDiemModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("người nhập điểm"))
        //                        //{
        //                        //    var tenNguoiNhapDiem = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var nguoiNhapDiemModel = DanhSachNguoiKy.Where(x => x.Ten == tenNguoiNhapDiem).FirstOrDefault();
        //                        //    if (nguoiNhapDiemModel != null && nguoiNhapDiemModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.NguoiNhapVaInDiem = nguoiNhapDiemModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("người đọc soát"))
        //                        //{
        //                        //    var tenNguoiDocSoat = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var nguoiDocSoatModel = DanhSachNguoiKy.Where(x => x.Ten == tenNguoiDocSoat).FirstOrDefault();
        //                        //    if (nguoiDocSoatModel != null && nguoiDocSoatModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.NguoiDocSoatBanGhi = nguoiDocSoatModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("cthđ chấm thi"))
        //                        //{
        //                        //    var tenChuTichChamThi = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var chuTichChamThiModel = DanhSachNguoiKy.Where(x => x.Ten == tenChuTichChamThi).FirstOrDefault();
        //                        //    if (chuTichChamThiModel != null && chuTichChamThiModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.ChuTichHoiDong = chuTichChamThiModel.ID;
        //                        //    }
        //                        //}
        //                        if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_NĐĐ"))
        //                        {
        //                            var NguoiDocDiemID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (NguoiDocDiemID != 0)
        //                            {
        //                                ThongTinToChucThiModel.NguoiDocDiem = NguoiDocDiemID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_NNĐ"))
        //                        {
        //                            var NguoiNhapDiemID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (NguoiNhapDiemID != 0)
        //                            {
        //                                ThongTinToChucThiModel.NguoiNhapVaInDiem = NguoiNhapDiemID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_NĐS"))
        //                        {
        //                            var NguoiDocSoatID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (NguoiDocSoatID != 0)
        //                            {
        //                                ThongTinToChucThiModel.NguoiDocSoatBanGhi = NguoiDocSoatID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_CTHĐCT"))
        //                        {
        //                            var ChuTichHoiDongID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (ChuTichHoiDongID != 0)
        //                            {
        //                                ThongTinToChucThiModel.ChuTichHoiDong = ChuTichHoiDongID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày duyệt chấm"))
        //                        {
        //                            var NgayDuyetCham = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
        //                            ThongTinToChucThiModel.NgayDuyetCham = NgayDuyetCham;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số thí sinh dự thi"))
        //                        {
        //                            var SoThiSinhDuThi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.SoThiSinhDuThi = SoThiSinhDuThi;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("được công nhận tốt nghiệp"))
        //                        {
        //                            var DuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.DuocCongNhanTotNghiep = DuocCongNhanTN;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("không được công nhận tốt nghiệp"))
        //                        {
        //                            var KhongDuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.KhongDuocCongNhanTotNghiep = KhongDuocCongNhanTN;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại giỏi"))
        //                        {
        //                            var SoLoaiGioi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.TNLoaiGioi = SoLoaiGioi;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại khá"))
        //                        {
        //                            var SoLoaiKha = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.TNLoaiKha = SoLoaiKha;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại tb"))
        //                        {
        //                            var SoLoaiTB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.TNLoaiTB = SoLoaiTB;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện a"))
        //                        {
        //                            var TotNghiepDienA = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.TotNghiepDienA = TotNghiepDienA;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện b"))
        //                        {
        //                            var TotNghiepDienB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.TotNghiepDienB = TotNghiepDienB;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,5"))
        //                        {
        //                            var DienTN4_5 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.DienTotNghiep4_5 = DienTN4_5;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,75"))
        //                        {
        //                            var DienTN4_75 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.DienTotNghiep4_75 = DienTN4_75;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 2"))
        //                        {
        //                            var DienTN2 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.DienTotNghiep2 = DienTN2;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 3"))
        //                        {
        //                            var DienTN3 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
        //                            ThongTinToChucThiModel.DienTotNghiep3 = DienTN3;
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày sở duyệt"))
        //                        {
        //                            var NgaySoDuyet = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
        //                            ThongTinToChucThiModel.NgaySoDuyet = NgaySoDuyet;
        //                        }
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("cán bộ sở kt"))
        //                        //{
        //                        //    var TenCanBoSoKT = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var CanBoSoKTModel = DanhSachNguoiKy.Where(x => x.Ten == TenCanBoSoKT).FirstOrDefault();
        //                        //    if (CanBoSoKTModel != null && CanBoSoKTModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.CanBoSoKT = CanBoSoKTModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("giám đốc sở gd"))
        //                        //{
        //                        //    var TenGiamDocSo = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var GiamDocSoModel = DanhSachNguoiKy.Where(x => x.Ten == TenGiamDocSo).FirstOrDefault();
        //                        //    if (GiamDocSoModel != null && GiamDocSoModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.GiamDocSo = GiamDocSoModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("thư ký"))
        //                        //{
        //                        //    var TenThuKy = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var ThuKyModel = DanhSachNguoiKy.Where(x => x.Ten == TenThuKy).FirstOrDefault();
        //                        //    if (ThuKyModel != null && ThuKyModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.ThuKy = ThuKyModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("phó chủ khảo"))
        //                        //{
        //                        //    var PhoChuKhao = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var PhoChuKhaoModel = DanhSachNguoiKy.Where(x => x.Ten == PhoChuKhao).FirstOrDefault();
        //                        //    if (PhoChuKhaoModel != null && PhoChuKhaoModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.PhoChuKhao = PhoChuKhaoModel.ID;
        //                        //    }
        //                        //}
        //                        //else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("chánh chủ khảo"))
        //                        //{
        //                        //    var ChanhChuKhao = Utils.ConvertToString(workSheet.Cells[i + 2, j].RichText.Text, string.Empty);
        //                        //    var ChanhChuKhaoModel = DanhSachNguoiKy.Where(x => x.Ten == ChanhChuKhao).FirstOrDefault();
        //                        //    if (ChanhChuKhaoModel != null && ChanhChuKhaoModel.ID > 0)
        //                        //    {
        //                        //        ThongTinToChucThiModel.ChanhChuKhao = ChanhChuKhaoModel.ID;
        //                        //    }
        //                        //}
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_CBSKT"))
        //                        {
        //                            var CanBoSoKT_ID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (CanBoSoKT_ID != 0)
        //                            {
        //                                ThongTinToChucThiModel.CanBoSoKT = CanBoSoKT_ID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_GĐSGD"))
        //                        {
        //                            var GiamDocSoID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (GiamDocSoID != 0)
        //                            {
        //                                ThongTinToChucThiModel.GiamDocSo = GiamDocSoID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_TK"))
        //                        {
        //                            var ThuKyID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (ThuKyID != 0)
        //                            {
        //                                ThongTinToChucThiModel.ThuKy = ThuKyID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_PCK"))
        //                        {
        //                            var PhoChuKhaoID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (PhoChuKhaoID != 0)
        //                            {
        //                                ThongTinToChucThiModel.PhoChuKhao = PhoChuKhaoID;
        //                            }
        //                        }
        //                        else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT_CCK"))
        //                        {
        //                            var ChanhChuKhaoID = Utils.ConvertToInt32(workSheet.Cells[i + 2, j].Value, 0);
        //                            if (ChanhChuKhaoID != 0)
        //                            {
        //                                ThongTinToChucThiModel.ChanhChuKhao = ChanhChuKhaoID;
        //                            }
        //                        }
        //                    }
        //                }
        //                #endregion

        //                result.ThongTinToChucThi = ThongTinToChucThiModel;
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        Mess = ex.Message;
        //        throw ex;
        //    }
        //    return result;
        //}

        public DuLieuDiemThiModel ReadFileExcel(string FilePath, ref string Mess, int? NamTotNghiep)
        {
            var result = new DuLieuDiemThiModel();
            result.DuLieuCuaNam = NamTotNghiep;
            result.ThongTinThiSinh = new List<ThongTinThiSinh>();
            result.ListErrorThiSinh = new List<ErrorThongTinThiSinh>();
            if (!File.Exists(FilePath))
            {
                Mess = "File không tồn tại! Vui lòng kiểm tra lại!";
                return null;
            }
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        Mess = "File không có dữ liệu! Vui lòng kiểm tra lại!";
                        return null;
                    }
                    else
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        int StartDataBody = 0;
                        int EndDataBody = 0;
                        var row = workSheet.Dimension.End.Row;
                        var col = workSheet.Dimension.End.Column;
                        int Nam = 0;
                        string TenMauPhieu = "";
                        var totalRow = workSheet.Dimension.End.Row;
                        var totalCol = workSheet.Dimension.End.Column;

                        var _mauPhieuID = Utils.ConvertToInt32(workSheet.Cells[1, 1].RichText.Text, 0);
                        var _tenMauPhieu = Utils.ConvertToString(workSheet.Cells[1, 2].RichText.Text, string.Empty);

                        var DanhSachTieuDe_Str = "";
                        //result.ChiTietMauPhieu = GetChiTietByNam(Nam, TenMauPhieu);
                        if (_mauPhieuID != 0 && !string.IsNullOrEmpty(_tenMauPhieu))
                        {
                            result.ChiTietMauPhieu = GetByID(_mauPhieuID);
                            ChiTietMauPhieuModel colDanhSachLoi = new ChiTietMauPhieuModel();
                            colDanhSachLoi.TieuDeCot = "Thông tin lỗi";
                            colDanhSachLoi.Loai = 9;
                            colDanhSachLoi.MaCot = "BODY_Loi";
                            colDanhSachLoi.NhomID = 0;
                            colDanhSachLoi.DanhSachCon = new List<ChiTietMauPhieuModel>();
                            result.ChiTietMauPhieu.DanhSachChiTietMauPhieu.Add(colDanhSachLoi);
                        }

                        if (result.ChiTietMauPhieu == null || result.ChiTietMauPhieu.MauPhieuID == null || result.ChiTietMauPhieu.MauPhieuID == 0)
                        {
                            for (int i = 1; i <= totalRow; i++)
                            {
                                for (int j = 1; j <= totalCol; j++)
                                {
                                    var test = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                    if ((Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("stt") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("số thứ tự") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("thứ tự") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("tt"))
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("cmnd")
                                        || (Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("họ và tên") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("họ tên"))
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("số hiệu bằng")
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("số hiệu bằng")
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("vào sổ cấp bằng số")
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("điểm")
                                        )
                                    {
                                        DanhSachTieuDe_Str += Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower() + ",";
                                    }
                                }
                                if (!string.IsNullOrEmpty(DanhSachTieuDe_Str)) break;
                            }
                            if (DanhSachTieuDe_Str.Contains("cmnd") && (DanhSachTieuDe_Str.Contains("họ và tên") || DanhSachTieuDe_Str.Contains("họ tên")) && (DanhSachTieuDe_Str.Contains("số hiệu bằng") || DanhSachTieuDe_Str.Contains("vào sổ cấp bằng số")))
                            {
                                var DanhSachBody = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_BieuMau_BODY.GetHashCode());
                                MauPhieuModel _mauPhieuModel = new MauPhieuModel();
                                //_mauPhieuModel.MauPhieuID = int.MaxValue;
                                _mauPhieuModel.DanhSachChiTietMauPhieu = new List<ChiTietMauPhieuModel>();

                                TenMauPhieu = "Mẫu export excel của bộ";
                                //var MauPhieu = GetChiTietByNam(2015, TenMauPhieu);
                                var MauPhieu = new MauPhieuModel();
                                if (MauPhieu != null && MauPhieu.MauPhieuID > 0) _mauPhieuModel.MauPhieuID = MauPhieu.MauPhieuID;
                                try
                                {
                                    var MauPhieuID = Utils.ConvertToInt32(new SystemConfigDAL().GetByKey("FileExportExcelBo").ConfigValue, 0);
                                    if (MauPhieuID > 0)
                                    {
                                        _mauPhieuModel.MauPhieuID = MauPhieuID;
                                    }
                                    else
                                    {
                                        _mauPhieuModel.MauPhieuID = int.MaxValue;
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                #region Tạo bảng theo file cấp bằng của bộ
                                var col_STT = DanhSachBody.Where(x => x.Ma == "BODY_STT").FirstOrDefault();
                                if (col_STT != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_STT = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_STT.CotID = col_STT.ID;
                                    _chiTietMauPhieu_STT.MaCot = col_STT.Ma;
                                    _chiTietMauPhieu_STT.TieuDeCot = col_STT.Ten;
                                    _chiTietMauPhieu_STT.ThuTu = 1;
                                    _chiTietMauPhieu_STT.Loai = col_STT.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_STT);
                                }

                                var col_CMTND = DanhSachBody.Where(x => x.Ma == "BODY_CMND/CCCD").FirstOrDefault();
                                if (col_CMTND != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_CMND = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_CMND.CotID = col_CMTND.ID;
                                    _chiTietMauPhieu_CMND.MaCot = col_CMTND.Ma;
                                    _chiTietMauPhieu_CMND.TieuDeCot = col_CMTND.Ten;
                                    _chiTietMauPhieu_CMND.ThuTu = 2;
                                    _chiTietMauPhieu_CMND.Loai = col_CMTND.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_CMND);
                                }

                                var col_HoTen = DanhSachBody.Where(x => x.Ma == "BODY_HT").FirstOrDefault();
                                if (col_HoTen != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_HoTen = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_HoTen.CotID = col_HoTen.ID;
                                    _chiTietMauPhieu_HoTen.MaCot = col_HoTen.Ma;
                                    _chiTietMauPhieu_HoTen.TieuDeCot = col_HoTen.Ten;
                                    _chiTietMauPhieu_HoTen.ThuTu = 3;
                                    _chiTietMauPhieu_HoTen.Loai = col_HoTen.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HoTen);
                                }

                                var col_GioiTinh = DanhSachBody.Where(x => x.Ma == "BODY_GT").FirstOrDefault();
                                if (col_GioiTinh != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_GioiTinh = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_GioiTinh.CotID = col_GioiTinh.ID;
                                    _chiTietMauPhieu_GioiTinh.MaCot = col_GioiTinh.Ma;
                                    _chiTietMauPhieu_GioiTinh.TieuDeCot = col_GioiTinh.Ten;
                                    _chiTietMauPhieu_GioiTinh.ThuTu = 4;
                                    _chiTietMauPhieu_GioiTinh.Loai = col_GioiTinh.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_GioiTinh);
                                }

                                var col_NgaySinh = DanhSachBody.Where(x => x.Ma == "BODY_NgS").FirstOrDefault();
                                if (col_NgaySinh != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_NgaySinh = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_NgaySinh.CotID = col_NgaySinh.ID;
                                    _chiTietMauPhieu_NgaySinh.MaCot = col_NgaySinh.Ma;
                                    _chiTietMauPhieu_NgaySinh.TieuDeCot = col_NgaySinh.Ten;
                                    _chiTietMauPhieu_NgaySinh.ThuTu = 5;
                                    _chiTietMauPhieu_NgaySinh.Loai = col_NgaySinh.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NgaySinh);
                                }

                                var col_NoiSinh = DanhSachBody.Where(x => x.Ma == "BODY_NS").FirstOrDefault();
                                if (col_NoiSinh != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_NoiSinh = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_NoiSinh.CotID = col_NoiSinh.ID;
                                    _chiTietMauPhieu_NoiSinh.MaCot = col_NoiSinh.Ma;
                                    _chiTietMauPhieu_NoiSinh.TieuDeCot = col_NoiSinh.Ten;
                                    _chiTietMauPhieu_NoiSinh.ThuTu = 6;
                                    _chiTietMauPhieu_NoiSinh.Loai = col_NoiSinh.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NoiSinh);
                                }

                                var col_DanToc = DanhSachBody.Where(x => x.Ma == "BODY_DT").FirstOrDefault();
                                if (col_DanToc != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_DanToc = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_DanToc.CotID = col_DanToc.ID;
                                    _chiTietMauPhieu_DanToc.MaCot = col_DanToc.Ma;
                                    _chiTietMauPhieu_DanToc.TieuDeCot = col_DanToc.Ten;
                                    _chiTietMauPhieu_DanToc.ThuTu = 7;
                                    _chiTietMauPhieu_DanToc.Loai = col_DanToc.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_DanToc);
                                }

                                var col_Lop = DanhSachBody.Where(x => x.Ma == "BODY_L").FirstOrDefault();
                                if (col_Lop != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_Lop = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_Lop.CotID = col_Lop.ID;
                                    _chiTietMauPhieu_Lop.MaCot = col_Lop.Ma;
                                    _chiTietMauPhieu_Lop.TieuDeCot = col_Lop.Ten;
                                    _chiTietMauPhieu_Lop.ThuTu = 8;
                                    _chiTietMauPhieu_Lop.Loai = col_Lop.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_Lop);
                                }

                                var col_SoHieuBang = DanhSachBody.Where(x => x.Ma == "BODY_SHB").FirstOrDefault();
                                if (col_SoHieuBang != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_SoHieuBang = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_SoHieuBang.CotID = col_SoHieuBang.ID;
                                    _chiTietMauPhieu_SoHieuBang.MaCot = col_SoHieuBang.Ma;
                                    _chiTietMauPhieu_SoHieuBang.TieuDeCot = col_SoHieuBang.Ten;
                                    _chiTietMauPhieu_SoHieuBang.ThuTu = 9;
                                    _chiTietMauPhieu_SoHieuBang.Loai = col_SoHieuBang.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_SoHieuBang);
                                }

                                var col_VaoSoCapBang = DanhSachBody.Where(x => x.Ma == "BODY_VSCBS").FirstOrDefault();
                                if (col_VaoSoCapBang != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_VaoSoCapBang = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_VaoSoCapBang.CotID = col_VaoSoCapBang.ID;
                                    _chiTietMauPhieu_VaoSoCapBang.MaCot = col_VaoSoCapBang.Ma;
                                    _chiTietMauPhieu_VaoSoCapBang.TieuDeCot = col_VaoSoCapBang.Ten;
                                    _chiTietMauPhieu_VaoSoCapBang.ThuTu = 10;
                                    _chiTietMauPhieu_VaoSoCapBang.Loai = col_VaoSoCapBang.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_VaoSoCapBang);
                                }

                                var col_NgayCapBang = DanhSachBody.Where(x => x.Ma == "BODY_NCB").FirstOrDefault();
                                if (col_NgayCapBang != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_NgayCapBang = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_NgayCapBang.CotID = col_NgayCapBang.ID;
                                    _chiTietMauPhieu_NgayCapBang.MaCot = col_NgayCapBang.Ma;
                                    _chiTietMauPhieu_NgayCapBang.TieuDeCot = col_NgayCapBang.Ten;
                                    _chiTietMauPhieu_NgayCapBang.ThuTu = 11;
                                    _chiTietMauPhieu_NgayCapBang.Loai = col_NgayCapBang.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NgayCapBang);
                                }

                                var col_HocSinhTruong = DanhSachBody.Where(x => x.Ma == "BODY_HST").FirstOrDefault();
                                if (col_HocSinhTruong != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_HocSinhTruong = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_HocSinhTruong.CotID = col_HocSinhTruong.ID;
                                    _chiTietMauPhieu_HocSinhTruong.MaCot = col_HocSinhTruong.Ma;
                                    _chiTietMauPhieu_HocSinhTruong.TieuDeCot = col_HocSinhTruong.Ten;
                                    _chiTietMauPhieu_HocSinhTruong.ThuTu = 12;
                                    _chiTietMauPhieu_HocSinhTruong.Loai = col_HocSinhTruong.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HocSinhTruong);
                                }
                                #endregion

                                result.ChiTietMauPhieu = _mauPhieuModel;

                                var _DanhMucChung = new List<DanhMucChungModel>();
                                _DanhMucChung = new DanhMucChungDAL().GetAll(0);
                                var DanhMucDanToc = new List<DanhMucChungModel>();
                                DanhMucDanToc = _DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).ToList();
                                var DanhMucTruongHoc = new List<DanhMucChungModel>();
                                DanhMucTruongHoc = _DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).ToList();
                                if(NamTotNghiep > 0)
                                {
                                    DanhMucTruongHoc = DanhMucTruongHoc.Where(x => x.Nam == NamTotNghiep).ToList();
                                }
                                //đọc dữ liệu file của bộ
                                int rowTieuDe = 0;
                                for (int i = 1; i <= totalRow; i++)
                                {
                                    for (int j = 1; j <= totalCol; j++)
                                    {
                                        var tempTieuDe = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                        var tempTieuDeUnsign = Utils.ConvertToUnSign(tempTieuDe);
                                        if ((tempTieuDe.Equals("stt") || tempTieuDe.Equals("số thứ tự") || tempTieuDe.Equals("thứ tự") || tempTieuDe.Equals("tt"))
                                            || tempTieuDe.Equals("cmnd") || tempTieuDe.Equals("số cmnd") || tempTieuDeUnsign.Equals("so cmnd")
                                            || tempTieuDe.Equals("họ và tên thí sinh") || tempTieuDeUnsign.Equals("ho va ten thi sinh") || tempTieuDe.Equals("họ và tên") || tempTieuDeUnsign.Equals("ho va ten")
                                            || tempTieuDe.Equals("số hiệu bằng") || tempTieuDeUnsign.Equals("so hieu bang")
                                            || tempTieuDe.Equals("vào sổ cấp bằng số") || tempTieuDeUnsign.Equals("vao so cap bang so")
                                            )
                                        {
                                            rowTieuDe = i;
                                            break;
                                        }
                                    }
                                    if (rowTieuDe != 0) break;
                                }
                                int STT = 1;
                                int rowData = rowTieuDe + 1;
                                for (int i = rowData; i <= totalRow; i++)
                                {
                                    ThongTinThiSinh _thiSinh = new ThongTinThiSinh();
                                    _thiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                                    for (int j = 1; j <= totalCol; j++)
                                    {
                                        //if (j == 1)
                                        var TieuDeCot = Utils.ConvertToString(workSheet.Cells[rowTieuDe, j].RichText.Text, string.Empty).ToLower();
                                        var TieuDeCotUnsign = Utils.ConvertToUnSign(TieuDeCot);
                                        if (TieuDeCot.Equals("stt"))
                                        {
                                            _thiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        else if (TieuDeCot.Equals("số cmnd") || TieuDeCot.Equals("cmnd") || TieuDeCotUnsign.Equals("so cmnd"))
                                        {
                                            _thiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 4)
                                        else if (TieuDeCot.Equals("họ và tên thí sinh") || TieuDeCotUnsign.Equals("ho va ten thi sinh") || TieuDeCot.Equals("họ và tên") || TieuDeCot.Equals("họ tên") || TieuDeCotUnsign.Equals("ho ten") || TieuDeCotUnsign.Equals("ho ten thi sinh"))
                                        {
                                            _thiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 5)
                                        else if (TieuDeCot.Equals("giới tính") || TieuDeCotUnsign.Equals("gioi tinh"))
                                        {
                                            var temp = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                            if ((!string.IsNullOrEmpty(temp)) && temp.ToLower() == "nam")
                                                _thiSinh.GioiTinh = false;
                                            else
                                                _thiSinh.GioiTinh = true;
                                        }
                                        //else if (j == 6)
                                        else if (TieuDeCot.Equals("ngày sinh") || TieuDeCotUnsign.Equals("ngay sinh"))
                                        {
                                            _thiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                        }
                                        //else if (j == 7)
                                        else if (TieuDeCot.Equals("nơi sinh") || TieuDeCotUnsign.Equals("noi sinh"))
                                        {
                                            _thiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 8)
                                        else if (TieuDeCot.Equals("dân tộc") || TieuDeCotUnsign.Equals("dan toc"))
                                        {
                                            string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                            if (!string.IsNullOrEmpty(DT))
                                            {
                                                var DanToc = DanhMucDanToc.Where(x => x.Ten.ToLower().Equals(DT) /*&& x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()*/).FirstOrDefault();
                                                if (DanToc != null && DanToc.ID > 0)
                                                {
                                                    _thiSinh.DanToc = DanToc.ID;
                                                }
                                            }
                                        }
                                        //else if (j == 9)
                                        else if (TieuDeCot.Equals("lớp") || TieuDeCotUnsign.Equals("lop"))
                                        {
                                            _thiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 10)
                                        else if (TieuDeCot.Equals("số hiệu bằng") || TieuDeCotUnsign.Equals("so hieu bang"))
                                        {
                                            _thiSinh.SoHieuBang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 11)
                                        else if (TieuDeCot.Equals("vào sổ cấp bằng số") || TieuDeCotUnsign.Equals("vao so cap bang so"))
                                        {
                                            _thiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 12)
                                        else if (TieuDeCot.Equals("ngày cấp") || TieuDeCotUnsign.Equals("ngay cap") || TieuDeCotUnsign.Equals("ngay cap bang"))
                                        {
                                            _thiSinh.NgayCapBang = Utils.ConvertToNullableDateTimeFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                        }
                                        //else if (j == 13)
                                        else if (TieuDeCot.Equals("trường") || TieuDeCotUnsign.Equals("truong") || TieuDeCot.Equals("đơn vị") || TieuDeCotUnsign.Equals("don vi") || TieuDeCot.Equals("trường học") || TieuDeCotUnsign.Equals("truong hoc"))
                                        {
                                            var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                            var TenTruongUnsign = Utils.ConvertToUnSign(TenTruong);
                                            if (!string.IsNullOrEmpty(TenTruong))
                                            {
                                                TenTruong = TenTruong.Trim();
                                                var TruongModel = DanhMucTruongHoc.Where(x => x.Ten.ToLower().Contains(TenTruong)
                                                                                    || TenTruong.Contains(x.Ten.ToLower())
                                                                                     || TenTruongUnsign.Contains(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenTruongUnsign)
                                                                                    /*&& x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()*/)
                                                                            .FirstOrDefault();
                                                if (TruongModel != null && TruongModel.ID > 0)
                                                {
                                                    _thiSinh.TruongTHPT = TruongModel.ID;
                                                    _thiSinh.TenTruongTHPT = TruongModel.Ten;
                                                }
                                                else
                                                {
                                                    //DanhMucChungModel DM_Truong = new DanhMucChungModel();
                                                    //DM_Truong.Ten = TenTruong;
                                                    //DM_Truong.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                                    //var TempInsert = new DanhMucChungDAL().Insert(DM_Truong);
                                                    //_thiSinh.TruongTHPT = Utils.ConvertToNullableInt32(TempInsert.Data, null);
                                                    _thiSinh.TenTruongTHPT = TenTruong;
                                                }
                                            }
                                        }
                                        else if (TieuDeCot.Equals("ghi chú") || TieuDeCotUnsign.Equals("ghi chu"))
                                        {
                                            _thiSinh.GhiChu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }

                                    }

                                    if (_thiSinh.STT == null || _thiSinh.STT == "0")
                                    {
                                        _thiSinh.STT = STT.ToString();
                                        STT++;
                                    }

                                    //check thi sinh theo cmnd
                                    //if (_thiSinh.CMND != null && _thiSinh.CMND.Length > 0)
                                    //{
                                    //    var info = new QuanLyThiSinhDAL().GetByCMND(_thiSinh.CMND);
                                    //    if (info.ThiSinhID > 0)
                                    //    {
                                    //        info.SoHieuBang = _thiSinh.SoHieuBang;
                                    //        info.NgayCapBang = _thiSinh.NgayCapBang;
                                    //        info.VaoSoCapBangSo = _thiSinh.VaoSoCapBangSo;

                                    //        result.ThongTinThiSinh.Add(info);
                                    //    }
                                    //    else result.ThongTinThiSinh.Add(_thiSinh);
                                    //    ////var info = DanhSachTatCaThiSinh.Where(x => x.CMND == _thiSinh.CMND.Trim()).FirstOrDefault();
                                    //    //if (info != null && info.ThiSinhID > 0)
                                    //    //{
                                    //    //    _thiSinh.ThiSinhID = info.ThiSinhID;
                                    //    //    result.ThongTinThiSinh.Add(_thiSinh);
                                    //    //}
                                    //    //else result.ThongTinThiSinh.Add(_thiSinh);
                                    //}

                                    if (!string.IsNullOrEmpty(_thiSinh.CMND))
                                    {
                                        result.ThongTinThiSinh.Add(_thiSinh);
                                    }
                                }
                                result.ThongTinToChucThi = new ThongTinToChucThi();
                                result.ThongTinToChucThi.TenHoiDongThi = "Phần mềm thi của bộ";
                                return result;
                            }
                            else if (DanhSachTieuDe_Str.Contains("cmnd") && DanhSachTieuDe_Str.Contains("điểm"))
                            {
                                return ReadFileExcelBo(workSheet);
                            }
                            else
                            {
                                Mess = "File không đúng định dạng! Vui lòng thử lại!";
                                return null;
                            }
                        }

                        for (int i = 1; i <= totalRow; i++)
                        {

                            for (int j = 1; j <= totalCol; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("BODY"))
                                {
                                    StartDataBody = i + 1;
                                    break;
                                }
                            }

                            for (int j = 1; j <= totalCol; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT"))
                                {
                                    EndDataBody = i - 2;
                                    //Nam = Utils.ConvertToInt32(workSheet.Cells[i, j + 1].Value, 0);
                                    //TenMauPhieu = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    break;
                                }
                            }

                        }

                        #region đọc data đầu
                        //var DanhSachHoiDong = new List<DanhMucChungModel>();
                        //DanhSachHoiDong = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_HoiDong.GetHashCode());
                        var DanhMucChung = new List<DanhMucChungModel>();
                        DanhMucChung = new DanhMucChungDAL().GetAll(0);
                        var DanhSachKhoaNgayThi = new List<DanhMucKhoaThiModel>();
                        BasePagingParams p = new BasePagingParams();
                        p.PageSize = 100000;
                        int total = 0;
                        DanhSachKhoaNgayThi = new DanhMucKhoaThiDAL().GetPagingBySearch(p, ref total);

                        var ThongTinToChucThiModel = new ThongTinToChucThi();
                        for (int i = 1; i < StartDataBody - 1; i++)
                        {
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng thi"))
                                {
                                    var TenHoiDongThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    if(ThongTinToChucThiModel.TenHoiDongThi == null || ThongTinToChucThiModel.TenHoiDongThi == "")
                                    ThongTinToChucThiModel.TenHoiDongThi = TenHoiDongThi;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HD", -1, "", true);
                                    if (TenHoiDongThi.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                  
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng chấm thi"))
                                {
                                    var TenHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    if (ThongTinToChucThiModel.TenHoiDongChamThi == null || ThongTinToChucThiModel.TenHoiDongChamThi == "")
                                    {
                                        ThongTinToChucThiModel.TenHoiDongChamThi = TenHoiDongChamThi;
                                        ThongTinToChucThiModel.HEAD_HDCT = TenHoiDongChamThi;

                                        ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDCT", -1, "", true);
                                        if (TenHoiDongChamThi.Length > EnumMaxLength.Text.GetHashCode())
                                        {
                                            loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                    
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng coi thi"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongCoiThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HĐCT", -1, "", true);
                                    if (ThongTinToChucThiModel.TenHoiDongCoiThi.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                   
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám thị"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongGiamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDGT", -1, "", true);
                                    if (ThongTinToChucThiModel.TenHoiDongGiamThi.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                  
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám khảo"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongGiamKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDGK", -1, "", true);
                                    if (ThongTinToChucThiModel.TenHoiDongGiamKhao.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                   
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("khoá ngày thi"))
                                {
                                    ThongTinToChucThiModel.KhoaThiNgay = Utils.ConvertToNullableDateTime(workSheet.Cells[i, j + 2].Value, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("ban:"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.Ban = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("phòng thi"))
                                {
                                    if(ThongTinToChucThiModel.PhongThi == null || ThongTinToChucThiModel.PhongThi == "")
                                    {
                                        var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                        ThongTinToChucThiModel.PhongThi = temp;

                                        if (temp.Length > EnumMaxLength.Text.GetHashCode())
                                        {
                                            ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_PT", -1, "", true);
                                            loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                    } 
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("từ sbd"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDDau = temp;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_SBD_START", -1, "", true);
                                    if (temp.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("đến sbd"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDCuoi = temp;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_SBD_END", -1, "", true);
                                    if (temp.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số quyển"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SoQuyen = temp;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("QUYEN", -1, "", true);
                                    if (temp.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("trang"))
                                {
                                    var temp = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].Value, null);
                                    ThongTinToChucThiModel.SoTrang = temp;

                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tỉnh"))
                                {
                                    var Tinh = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    if (ThongTinToChucThiModel.Tinh == null || ThongTinToChucThiModel.Tinh == "")
                                        ThongTinToChucThiModel.Tinh = Tinh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("kỳ thi"))
                                {
                                    var KyThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.TenKyThi = KyThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("trường") || Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("trường:"))
                                {
                                    var HEAD_TRUONG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HEAD_TRUONG = HEAD_TRUONG;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_TRUONG", -1, "", true);
                                    if (HEAD_TRUONG.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng chấm lại"))
                                {
                                    var HEAD_HDCL = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HEAD_HDCL = HEAD_HDCL;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDCL", -1, "", true);
                                    if (HEAD_HDCL.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region đọc data bảng điểm

                        //var DanhSachDanToc = new List<DanhMucChungModel>();
                        //DanhSachDanToc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_DanToc.GetHashCode());

                        //var DanhSachMonHoc = new List<DanhMucChungModel>();
                        //DanhSachMonHoc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_MonHoc.GetHashCode());
                        int index = 0;
                        for (int i = StartDataBody; i <= EndDataBody + 1; i++)
                        {
                            var ThiSinh = new ThongTinThiSinh();
                            ThiSinh.DanhSachLoi = "";
                            ThiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                            ThiSinh.HoiDongThi = ThongTinToChucThiModel.HoiDongThiID;
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                //var test = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SBD"))
                                {
                                    ThiSinh.SoBaoDanh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_STT"))
                                {
                                    ThiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HT"))
                                {
                                    ThiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    //if (ThiSinh.HoTen == string.Empty) ThiSinh.DanhSachLoi += "Tên thí sinh không được để trống; ";
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NgS"))
                                {
                                    //ThiSinh.NgaySinh = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                    //ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                    //if (ThiSinh.NgaySinh == null && workSheet.Cells[i, j].RichText.Text != null && workSheet.Cells[i, j].RichText.Text.Length > 0)
                                    //{
                                    //    ThiSinh.DanhSachLoi += "Ngày sinh không đúng định dạng; ";
                                    //}

                                    var NgaySinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (NgaySinh.Length > 0)
                                    {
                                        if (NgaySinh.Length == 6 && !NgaySinh.Contains("/") && !NgaySinh.Contains("-"))
                                        {
                                            var Ngay = NgaySinh.Substring(0, 2);
                                            var Thang = NgaySinh.Substring(2, 2);
                                            var NamSinh = NgaySinh.Substring(4, 2);
                                            ThiSinh.NgaySinhStr = Ngay + "/" + Thang + "/19" + NamSinh;
                                        }
                                        else if (NgaySinh.Length == 8 && !NgaySinh.Contains("/") && !NgaySinh.Contains("-"))
                                        {
                                            var Ngay = NgaySinh.Substring(0, 2);
                                            var Thang = NgaySinh.Substring(2, 2);
                                            var NamSinh = NgaySinh.Substring(4, 4);
                                            ThiSinh.NgaySinhStr = Ngay + "/" + Thang + "/" + NamSinh;
                                        }                               
                                        else ThiSinh.NgaySinhStr = NgaySinh;
                                        ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(ThiSinh.NgaySinhStr, null);
                                        if (ThiSinh.NgaySinh != null && ThiSinh.NgaySinhStr.Length > 10)
                                        {
                                            ThiSinh.NgaySinhStr = ThiSinh.NgaySinh.Value.ToString("dd/MM/yyyy");
                                        }

                                        ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("BODY_NgS", index, "", true);
                                        if (!Utils.CheckSpecialCharacter(NgaySinh) || NgaySinh.Length > EnumMaxLength.NgaySinh.GetHashCode())
                                        {
                                            loi.DanhSachLoi = "Ngày sinh không hợp lệ";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                      
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NS"))
                                {
                                    ThiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GT"))
                                {
                                    var _gioiTinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (_gioiTinh.ToLower() == "nam")
                                    {
                                        ThiSinh.GioiTinh = false;
                                    }
                                    else if (_gioiTinh.ToLower() == "nữ")
                                    {
                                        ThiSinh.GioiTinh = true;
                                    }
                                    //ThiSinh.GioiTinh = Utils.ConvertToBoolean(workSheet.Cells[i, j].RichText.Text, false);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DT"))
                                {
                                    //ThiSinh.DanToc = Utils.ConvertToInt32(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(DT))
                                    {
                                        //var DanToc = DanhSachDanToc.Where(x => x.Ten.ToLower().Contains(DT)).FirstOrDefault();
                                        var DanToc = DanhMucChung.Where(x => x.Ten.Contains(DT) && x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).FirstOrDefault();
                                        if (DanToc != null && DanToc.ID > 0)
                                        {
                                            ThiSinh.DanToc = DanToc.ID;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HST"))
                                {
                                    var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenTruong))
                                    {
                                        TenTruong = TenTruong.Trim();
                                        //var TruongModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenTruong.ToLower()) || TenTruong.ToLower().Contains(x.Ten.ToLower())) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
                                        var DanhMucTruong = DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).ToList();
                                        var TruongModel = DanhMucTruong.Where(x => ((x.Ten.ToLower().Equals(TenTruong.ToLower())
                                                                                    || TenTruong.ToLower().Equals(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(TenTruong.ToLower()).Equals(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Equals(Utils.ConvertToUnSign(TenTruong.ToLower())))
                                                                                    && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                                                            .FirstOrDefault();
                                        if (TruongModel != null && TruongModel.ID > 0)
                                        {
                                            ThiSinh.TruongTHPT = TruongModel.ID;
                                            ThiSinh.TenTruongTHPT = TruongModel.Ten;
                                        }
                                        else
                                        {
                                            ThiSinh.TenTruongTHPT = TenTruong;
                                            //thêm mới dm trường
                                            DanhMucChungModel danhMucChungModel = new DanhMucChungModel();
                                            danhMucChungModel.Ten = TenTruong;
                                            danhMucChungModel.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                            danhMucChungModel.Nam = NamTotNghiep;
                                            var baseResult = new DanhMucChungDAL().Insert(danhMucChungModel);
                                            int ID = Utils.ConvertToInt32(baseResult.Data, 0);
                                            if(ID > 0)
                                            {
                                                danhMucChungModel.ID = ID;
                                                DanhMucChung.Add(danhMucChungModel);
                                                ThiSinh.TruongTHPT = ID;
                                            }
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HK"))
                                {
                                    var TenHanhKiem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenHanhKiem))
                                    {
                                        var HanhKiemModel = DanhMucChung.Where(x => x.Ten.Contains(TenHanhKiem) && x.Loai == EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode()).FirstOrDefault();
                                        if (HanhKiemModel != null && HanhKiemModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHanhKiem = HanhKiemModel.ID;
                                            ThiSinh.XepLoaiHanhKiemStr = HanhKiemModel.Ten;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HL"))
                                {
                                    var TenHocLuc = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenHocLuc))
                                    {
                                        var HocLucModel = DanhMucChung.Where(x => x.Ten.Contains(TenHocLuc) && x.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode()).FirstOrDefault();
                                        if (HocLucModel != null && HocLucModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHocLuc = HocLucModel.ID;
                                            ThiSinh.XepLoaiHocLucStr = HocLucModel.Ten;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXTN"))
                                {
                                    //ThiSinh.DiemXetTotNghiep = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemXetTotNghiep = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐKK"))
                                {
                                    //ThiSinh.DiemKK = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemKK = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
                                {
                                    //ThiSinh.TongSoDiemThi = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimalFromIntForTongDiemThi(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXL"))
                                {
                                    //ThiSinh.DiemXL = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemXL = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐL12"))
                                {
                                    //ThiSinh.DiemTBLop12 = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐƯT"))
                                {
                                    //ThiSinh.DiemUT = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemUT = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_Đô"))
                                {
                                    ThiSinh.Do = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐôT"))
                                {
                                    ThiSinh.DoThem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_H"))
                                {
                                    ThiSinh.Hong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLTN") /*|| Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG")*/)
                                {
                                    ThiSinh.KetQuaTotNghiep = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GC"))
                                {
                                    ThiSinh.GhiChu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CMND/CCCD"))
                                {
                                    ThiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("BODY_CMND/CCCD", index, "", true);
                                    if (!Utils.CheckSpecialCharacter(ThiSinh.CMND) || ThiSinh.CMND.Length > EnumMaxLength.CCCD.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "CMND/CCCD không hợp lệ";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SĐT"))
                                {
                                    ThiSinh.SoDienThoai = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_L"))
                                {
                                    ThiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LDT"))
                                {
                                    ThiSinh.LoaiDuThi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐVĐKDT"))
                                {
                                    ThiSinh.DonViDKDT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LĐ"))
                                {
                                    ThiSinh.LaoDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VH"))
                                {
                                    ThiSinh.VanHoa = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_RLTT"))
                                {
                                    ThiSinh.RLTT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SHB"))
                                {
                                    ThiSinh.SoHieuBang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
                                {
                                    ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NCB"))
                                {
                                    ThiSinh.NgayCapBang = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
                                {
                                    ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
                                {
                                    ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimalFromIntForTongDiemThi(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG"))
                                {
                                    ThiSinh.Hang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Contains("MH"))
                                {
                                    var thongTinDiemMonHoc = new ThongTinDiemThi();                                   
                                    //thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    thongTinDiemMonHoc.DiemBaiToHop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (thongTinDiemMonHoc.Diem != null || thongTinDiemMonHoc.DiemBaiToHop != string.Empty)
                                    {
                                        var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                        var arrTemp = temp.Split("||");
                                        thongTinDiemMonHoc.NhomID = arrTemp.Length > 0 ? Utils.ConvertToNullableInt32(arrTemp[1], null) : null;
                                        var TenMonHoc = Utils.ConvertToString(workSheet.Cells[StartDataBody - 2, j].Value, string.Empty);
                                        var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                        if (MonHocModel != null && MonHocModel.ID > 0)
                                        {
                                            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                        }

                                        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐTB"))
                                {
                                    //var DiemTrungBinh = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                                    var DiemTrungBinh = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBCacBaiThi = DiemTrungBinh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DUT"))
                                {
                                    var DienUuTien = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.DienUuTien = DienUuTien;
                                }                            
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐTBC"))
                                {
                                    //var DiemTBC = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                                    var DiemTBC = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBC = DiemTBC;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUEQUAN"))
                                {
                                    var QueQuan = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.QueQuan = QueQuan;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TB12"))
                                {                 
                                    var DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBLop12 = DiemTBLop12;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NGHE"))
                                {
                                    var ChungNhanNghe = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.ChungNhanNghe = ChungNhanNghe;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DTCLS"))
                                {
                                    var DTConLietSi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.DTConLietSi = DTConLietSi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GTDKT"))
                                {
                                    var GiaiTDKT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.GiaiTDKT = GiaiTDKT;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐC"))
                                {
                                    ThiSinh.DiaChi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HOIDONG"))
                                {
                                    ThiSinh.HoiDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_MONKN"))
                                {
                                    ThiSinh.MonKN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TBCNKN"))
                                {
                                    ThiSinh.TBCNMonKN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHICU"))
                                {
                                    ThiSinh.DiemThiCu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHIMOI"))
                                {
                                    ThiSinh.DiemThiMoi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TONGBQ"))
                                {
                                    ThiSinh.TongBQ = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQA"))
                                {
                                    ThiSinh.BQA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQT"))
                                {
                                    ThiSinh.BQT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DC"))
                                {
                                    ThiSinh.DC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BAN"))
                                {
                                    ThiSinh.Ban = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DAODUC"))
                                {
                                    ThiSinh.BODY_DAODUC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_RLEV"))
                                {
                                    ThiSinh.BODY_RLEV = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIENKK"))
                                {
                                    ThiSinh.BODY_DIENKK = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_PHONGTHI"))
                                {
                                    ThiSinh.BODY_PHONGTHI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTNC"))
                                {
                                    ThiSinh.BODY_DIEMTNC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLTNC"))
                                {
                                    ThiSinh.BODY_XLTNC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TDTCU"))
                                {
                                    ThiSinh.BODY_TDTCU = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_KETLUAN"))
                                {
                                    ThiSinh.BODY_KETLUAN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHICUCHA"))
                                {
                                    ThiSinh.BODY_DIEMTHICUCHA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GIAIHSG"))
                                {
                                    ThiSinh.BODY_GIAIHSG = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GIAIHSGK"))
                                {
                                    ThiSinh.BODY_GIAIHSGK = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CHUNGCHINN"))
                                {
                                    ThiSinh.BODY_CHUNGCHINN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CHUNGCHITH"))
                                {
                                    ThiSinh.BODY_CHUNGCHITH = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TONGDIEMMOI"))
                                {
                                    ThiSinh.BODY_TONGDIEMMOI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQAMOI"))
                                {
                                    ThiSinh.BODY_BQAMOI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQTMOI"))
                                {
                                    ThiSinh.BODY_BQTMOI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SOCAPGIAYCN"))
                                {
                                    ThiSinh.BODY_SOCAPGIAYCN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLHT"))
                                {
                                    ThiSinh.BODY_XLHT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUOCGIA"))
                                {
                                    ThiSinh.BODY_QUOCGIA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                            }

                            if (!string.IsNullOrEmpty(ThiSinh.HoTen))
                            {
                                ThiSinh.Index = index;
                                result.ThongTinThiSinh.Add(ThiSinh);
                                index++;
                            }

                        }

                        #endregion

                        #region đọc data cuối mẫu phiếu
                        //var DanhSachNguoiKy = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode());
                        for (int i = EndDataBody + 2; i <= workSheet.Dimension.End.Row; i++)
                        {
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                var test = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower();
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người đọc điểm"))
                                {
                                    ThongTinToChucThiModel.NguoiDocDiem = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người nhập điểm"))
                                {
                                    ThongTinToChucThiModel.NguoiNhapVaInDiem = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người đọc soát"))
                                {
                                    ThongTinToChucThiModel.NguoiDocSoatBanGhi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("CTHĐ chấm thi"))
                                {
                                    ThongTinToChucThiModel.ChuTichHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày duyệt chấm"))
                                {
                                    var NgayDuyetCham = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
                                    ThongTinToChucThiModel.NgayDuyetCham = NgayDuyetCham;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số thí sinh dự thi"))
                                {
                                    var SoThiSinhDuThi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.SoThiSinhDuThi = SoThiSinhDuThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("được công nhận tốt nghiệp:"))
                                {
                                    var DuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DuocCongNhanTotNghiep = DuocCongNhanTN;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("không được công nhận tốt nghiệp:"))
                                {
                                    var KhongDuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.KhongDuocCongNhanTotNghiep = KhongDuocCongNhanTN;
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại Giỏi"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Loai_Gioi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại Khá"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Loai_Kha = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại TB"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Loai_TB = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại giỏi"))
                                {
                                    var SoLoaiGioi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiGioi = SoLoaiGioi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại khá"))
                                {
                                    var SoLoaiKha = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiKha = SoLoaiKha;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại tb"))
                                {
                                    var SoLoaiTB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiTB = SoLoaiTB;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện a"))
                                {
                                    var TotNghiepDienA = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienA = TotNghiepDienA;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện b"))
                                {
                                    var TotNghiepDienB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienB = TotNghiepDienB;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện c"))
                                {
                                    var TotNghiepDienC = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienC = TotNghiepDienC;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,5"))
                                {
                                    var DienTN4_5 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep4_5 = DienTN4_5;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,75"))
                                {
                                    var DienTN4_75 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep4_75 = DienTN4_75;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 2"))
                                {
                                    var DienTN2 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep2 = DienTN2;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 3"))
                                {
                                    var DienTN3 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep3 = DienTN3;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày sở duyệt"))
                                {
                                    var NgaySoDuyet = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
                                    ThongTinToChucThiModel.NgaySoDuyet = NgaySoDuyet;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Cán bộ sở KT"))
                                {
                                    ThongTinToChucThiModel.CanBoSoKT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Giám đốc sở GD"))
                                {
                                    ThongTinToChucThiModel.GiamDocSo = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thư ký"))
                                {
                                    ThongTinToChucThiModel.ThuKy = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.FOOT_THUKY = ThongTinToChucThiModel.ThuKy;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Phó chủ khảo"))
                                {
                                    ThongTinToChucThiModel.PhoChuKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chánh chủ khảo"))
                                {
                                    ThongTinToChucThiModel.ChanhChuKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tổ trưởng hồi phách"))
                                {
                                    var ToTruongHoiPhach = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ToTruongHoiPhach = ToTruongHoiPhach;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Hiệu trưởng"))
                                {
                                    var HieuTruong = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HieuTruong = HieuTruong;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng coi thi"))
                                {
                                    var CTHĐCT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDongCoiThi = CTHĐCT;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng:"))
                                {
                                    var CTHĐ = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDong = CTHĐ;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Cán bộ xét duyệt"))
                                {
                                    var CanBoXetDuyet = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.CanBoXetDuyet = CanBoXetDuyet;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("CTHĐ chấm thi"))
                                {
                                    var ChuTichHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDongChamThi = ChuTichHoiDongChamThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Địa danh"))
                                {
                                    var DiaDanh = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.DiaDanh = DiaDanh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ghi chú"))
                                {
                                    var GhiChuCuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.GhiChuCuoiTrang = GhiChuCuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Từ SBD"))
                                {
                                    var SBDDau_CuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDDau_CuoiTrang = SBDDau_CuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Đến SBD"))
                                {
                                    var SBDDau_CuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDCuoi_CuoiTrang = SBDDau_CuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh đỗ thẳng"))
                                {
                                    var TSDoThang = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSDoThang = TSDoThang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh đỗ thêm"))
                                {
                                    var TSDoThem = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSDoThem = TSDoThem;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh thi hỏng"))
                                {
                                    var TSThiHong = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSThiHong = TSThiHong;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Phó Giám đốc Sở Giáo dục"))
                                {
                                    var PGiamDoc = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.PGiamDoc = PGiamDoc;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người kiểm tra hồ sơ"))
                                {
                                    ThongTinToChucThiModel.FOOT_NGUOIKIEMTRAHS = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người kiểm tra"))
                                {
                                    var NguoiKiemTra = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.NguoiKiemTra = NguoiKiemTra;
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ráp phách, đọc điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_RPDD = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Kiểm tra đọc"))
                                {
                                    ThongTinToChucThiModel.FOOT_KTD = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Dò trên bảng điểm in sẵn"))
                                {
                                    ThongTinToChucThiModel.FOOT_DTBDIS = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thư ký"))
                                {
                                    ThongTinToChucThiModel.FOOT_THUKY = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Giám sát"))
                                {
                                    ThongTinToChucThiModel.FOOT_GIAMSAT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Được công nhận tốt nghiệp"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_DCNTN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Diện bq 4.5"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Dien45 = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Diện bq 4.75"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Dien475 = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm 1 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEM1DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm 1,5 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEM15DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm 2 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEM2DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm trên 2 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEMTREN2DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS vắng mặt khi thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_VANGMATKHITHI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS vi pham quy chế thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_VIPHAMQUYCHETHI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS thuộc diện ưu tiên"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSDIENUUTIEN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS có giấy chứng nhận nghề hợp lệ"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSCOCHUNGNHANNGHE = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                               
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Không được công nhận tốt nghiệp"))
                                {
                                    ThongTinToChucThiModel.FOOT_SKĐCNTN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Số thí sinh dự thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_SSTSDT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Số thí sinh dự thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_STSDT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }                             
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tốt nghiệp diện D"))
                                {
                                    ThongTinToChucThiModel.FOOT_TND_D = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tốt nghiệp diện E"))
                                {
                                    ThongTinToChucThiModel.FOOT_TND_E = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                              
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại Thường"))
                                {
                                    ThongTinToChucThiModel.FOOT_SLTHUONG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("Thường") || Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("Thường:"))
                                {
                                    ThongTinToChucThiModel.FOOT_LTHUONG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS con liệt sĩ, dân tộc ít người"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSCONLIETSI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS các diện khác"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSCACDIENKHAC = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người lập bảng"))
                                {
                                    ThongTinToChucThiModel.FOOT_NGUOILAPBANG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ngày xác nhận lập bảng"))
                                {
                                    String FOOT_NXNLAPBANG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    FOOT_NXNLAPBANG = FOOT_NXNLAPBANG.Replace("12:00:00 AM", "");
                                    ThongTinToChucThiModel.FOOT_NXNLAPBANG = FOOT_NXNLAPBANG;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ngày xác nhận chủ tịch hội đồng coi thi"))
                                {
                                    String FOOT_NXNHOIDONGCOITHI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    FOOT_NXNHOIDONGCOITHI = FOOT_NXNHOIDONGCOITHI.Replace("12:00:00 AM", "");
                                    ThongTinToChucThiModel.FOOT_NXNHOIDONGCOITHI = FOOT_NXNHOIDONGCOITHI;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ngày xác nhận chấm thi và xét tốt nghiệp"))
                                {
                                    String FOOT_NXNCHAMTHIXTN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    FOOT_NXNCHAMTHIXTN = FOOT_NXNCHAMTHIXTN.Replace("12:00:00 AM", "");
                                    ThongTinToChucThiModel.FOOT_NXNCHAMTHIXTN = FOOT_NXNCHAMTHIXTN;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Vụ trưởng Vụ Giáo dục thường xuyên"))
                                {
                                    ThongTinToChucThiModel.FOOT_VTVGDTX = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng phúc khảo"))
                                {
                                    ThongTinToChucThiModel.FOOT_CTHDPHUCKHAO = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                
                            }
                        }
                        #endregion

                        result.ThongTinToChucThi = ThongTinToChucThiModel;

                        if(result.ThongTinThiSinh != null && result.ThongTinThiSinh.Count > 0)
                        {
                            DataTable tableThiSinh = new DataTable();
                            tableThiSinh.Columns.Add("ThiSinhID_Str");
                            tableThiSinh.Columns.Add("KyThiID");
                            tableThiSinh.Columns.Add("HoTen");
                            tableThiSinh.Columns.Add("NgaySinh");
                            tableThiSinh.Columns.Add("NoiSinh");
                            tableThiSinh.Columns.Add("GioiTinh");
                            tableThiSinh.Columns.Add("DanToc");
                            tableThiSinh.Columns.Add("CMND");
                            tableThiSinh.Columns.Add("SoBaoDanh");
                            tableThiSinh.Columns.Add("SoDienThoai");
                            tableThiSinh.Columns.Add("DiaChi");
                            tableThiSinh.Columns.Add("Lop");
                            tableThiSinh.Columns.Add("TruongTHPT");
                            tableThiSinh.Columns.Add("LoaiDuThi");
                            tableThiSinh.Columns.Add("DonViDKDT");
                            tableThiSinh.Columns.Add("XepLoaiHanhKiem");
                            tableThiSinh.Columns.Add("XepLoaiHocLuc");
                            tableThiSinh.Columns.Add("DiemTBLop12");
                            tableThiSinh.Columns.Add("DiemKK");
                            tableThiSinh.Columns.Add("DienXTN");
                            tableThiSinh.Columns.Add("HoiDongThi");
                            tableThiSinh.Columns.Add("DiemXetTotNghiep");
                            tableThiSinh.Columns.Add("KetQuaTotNghiep");
                            tableThiSinh.Columns.Add("SoHieuBang");
                            tableThiSinh.Columns.Add("VaoSoCapBangSo");
                            tableThiSinh.Columns.Add("NamThi");
                            tableThiSinh.Columns.Add("Do");
                            tableThiSinh.Columns.Add("DoThem");
                            tableThiSinh.Columns.Add("Hong");
                            tableThiSinh.Columns.Add("LaoDong");
                            tableThiSinh.Columns.Add("VanHoa");
                            tableThiSinh.Columns.Add("RLTT");
                            tableThiSinh.Columns.Add("TongSoDiemThi");
                            tableThiSinh.Columns.Add("NgayCapBang");
                            tableThiSinh.Columns.Add("DiemXL");
                            tableThiSinh.Columns.Add("DiemUT");
                            tableThiSinh.Columns.Add("GhiChu");
                            tableThiSinh.Columns.Add("Hang");
                            tableThiSinh.Columns.Add("DiemTBCacBaiThi");
                            tableThiSinh.Columns.Add("DienUuTien");
                            tableThiSinh.Columns.Add("DiemTBC");

                            foreach (var item in result.ThongTinThiSinh)
                            {
                                var ThiSinhID_Str = Guid.NewGuid().ToString();
                                tableThiSinh.Rows.Add(ThiSinhID_Str,
                                                        item.KyThiID,
                                                        item.HoTen,
                                                        item.NgaySinh,
                                                        item.NoiSinh,
                                                        item.GioiTinh,
                                                        item.DanToc,
                                                        item.CMND,
                                                        item.SoBaoDanh,
                                                        item.SoDienThoai,
                                                        item.DiaChi,
                                                        item.Lop,
                                                        item.TruongTHPT,
                                                        item.LoaiDuThi,
                                                        item.DonViDKDT,
                                                        item.XepLoaiHanhKiem,
                                                        item.XepLoaiHocLuc,
                                                        item.DiemTBLop12,
                                                        item.DiemKK,
                                                        item.DienXTN,
                                                        item.HoiDongThi,
                                                        item.DiemXetTotNghiep,
                                                        item.KetQuaTotNghiep,
                                                        item.SoHieuBang,
                                                        item.VaoSoCapBangSo,
                                                        item.NamThi,
                                                        item.Do,
                                                        item.DoThem,
                                                        item.Hong,
                                                        item.LaoDong,
                                                        item.VanHoa,
                                                        item.RLTT,
                                                        item.TongSoDiemThi,
                                                        item.NgayCapBang,
                                                        item.DiemXL,
                                                        item.DiemUT,
                                                        item.GhiChu,
                                                        item.Hang,
                                                        item.DiemTBCacBaiThi,
                                                        item.DienUuTien,
                                                        item.DiemTBC
                                                        );
                            }

                            List<string> listThiSinhTrungCMNN = new List<string>();

                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter("TableThiSinh", SqlDbType.Structured),
                            };
                            parameters[0].Value = tableThiSinh;

                            try
                            {
                                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_SLTrungCMND_New", parameters))
                                {
                                    while (dr.Read())
                                    {
                                        var CMND = Utils.ConvertToString(dr["CMND"], String.Empty);
                                        listThiSinhTrungCMNN.Add(CMND);
                                        //var SLTrungCMND = Utils.ConvertToInt32(dr["SL"], 0);
                                        //if (SLTrungCMND > 0) Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";
                                    }
                                    dr.Close();
                                }

                            }
                            catch { }

                            if(listThiSinhTrungCMNN.Count > 0)
                            {
                                //foreach (var item in result.ThongTinThiSinh)
                                //{
                                //    for (int i = 0; i < listThiSinhTrungCMNN.Count; i++)
                                //    {
                                //        if (item.CMND != "" && item.CMND == listThiSinhTrungCMNN[i])
                                //        {
                                //            item.DanhSachLoi += "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                //            Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";

                                        
                                //        }
                                //    }

                                //}

                                for (int i = 0; i < result.ThongTinThiSinh.Count; i++)
                                {
                                    var item = result.ThongTinThiSinh[i];
                                    for (int j = 0; j < listThiSinhTrungCMNN.Count; j++)
                                    {
                                        if (item.CMND != "" && item.CMND == listThiSinhTrungCMNN[j])
                                        {
                                            item.DanhSachLoi += "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                            Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";

                                            var check = false;
                                            foreach (var err in result.ListErrorThiSinh)
                                            {
                                                if(err.Index == item.Index && err.MaCot == "BODY_CMND/CCCD")
                                                {
                                                    err.DanhSachLoi += "; Trùng CMND/CCCD với thí sinh trong hệ thống";
                                                    check = true;
                                                }
                                            }
                                            if (!check)
                                            {
                                                ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("BODY_CMND/CCCD", item.Index ?? 0, "Trùng CMND/CCCD với thí sinh trong hệ thống", true);
                                                result.ListErrorThiSinh.Add(loi);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Mess = ex.Message;
                //throw ex;
            }
            return result;
        }

        public DuLieuDiemThiModel ReadFileExcelUpdate(string FilePath, ref string Mess)
        {
            var result = new DuLieuDiemThiModel();
            result.ThongTinThiSinh = new List<ThongTinThiSinh>();
            if (!File.Exists(FilePath))
            {
                Mess = "File không tồn tại! Vui lòng kiểm tra lại!";
                return null;
            }
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        Mess = "File không có dữ liệu! Vui lòng kiểm tra lại!";
                        return null;
                    }
                    else
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        int StartDataBody = 0;
                        int EndDataBody = 0;
                        var row = workSheet.Dimension.End.Row;
                        var col = workSheet.Dimension.End.Column;
                        int Nam = 0;
                        string TenMauPhieu = "";
                        var totalRow = workSheet.Dimension.End.Row;
                        var totalCol = workSheet.Dimension.End.Column;

                        var _mauPhieuID = Utils.ConvertToInt32(workSheet.Cells[1, 1].RichText.Text, 0);
                        var _tenMauPhieu = Utils.ConvertToString(workSheet.Cells[1, 2].RichText.Text, string.Empty);

                        var DanhSachTieuDe_Str = "";

                        #region đọc data đầu
                       
                        var DanhMucChung = new List<DanhMucChungModel>();
                        DanhMucChung = new DanhMucChungDAL().GetAll(0);
                        var DanhSachKhoaNgayThi = new List<DanhMucKhoaThiModel>();
                        BasePagingParams p = new BasePagingParams();
                        p.PageSize = 100000;
                        int total = 0;
                        DanhSachKhoaNgayThi = new DanhMucKhoaThiDAL().GetPagingBySearch(p, ref total);

                        var ThongTinToChucThiModel = new ThongTinToChucThi();
                        for (int i = 1; i < StartDataBody - 1; i++)
                        {
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng thi"))
                                {
                                    var TenHoiDongThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.TenHoiDongThi = TenHoiDongThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng chấm thi"))
                                {
                                    var TenHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.TenHoiDongChamThi = TenHoiDongChamThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng coi thi"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongCoiThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám thị"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongGiamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám khảo"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongGiamKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("khoá ngày thi"))
                                {
                                    ThongTinToChucThiModel.KhoaThiNgay = Utils.ConvertToNullableDateTime(workSheet.Cells[i, j + 2].Value, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("ban:"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.Ban = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("phòng thi"))
                                {
                                    if (ThongTinToChucThiModel.PhongThi == null || ThongTinToChucThiModel.PhongThi == "")
                                    {
                                        var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                        ThongTinToChucThiModel.PhongThi = temp;
                                    }   
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("từ sbd"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDDau = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("đến sbd"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDCuoi = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số quyển"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SoQuyen = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("trang"))
                                {
                                    var temp = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].Value, null);
                                    ThongTinToChucThiModel.SoTrang = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tỉnh"))
                                {
                                    var Tinh = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.Tinh = Tinh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("kỳ thi"))
                                {
                                    var KyThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.TenKyThi = KyThi;
                                }

                            }
                        }

                        #endregion

                        #region đọc data bảng điểm
                        for (int i = StartDataBody; i <= EndDataBody + 1; i++)
                        {
                            var ThiSinh = new ThongTinThiSinh();
                            ThiSinh.DanhSachLoi = "";
                            ThiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                            ThiSinh.HoiDongThi = ThongTinToChucThiModel.HoiDongThiID;
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                
                                if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SBD"))
                                {
                                    ThiSinh.SoBaoDanh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_STT"))
                                {
                                    ThiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HT"))
                                {
                                    ThiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NgS"))
                                {
                                    var NgaySinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (NgaySinh.Length > 0)
                                    {
                                        if (NgaySinh.Length == 6 && !NgaySinh.Contains("/"))
                                        {
                                            var Ngay = NgaySinh.Substring(0, 2);
                                            var Thang = NgaySinh.Substring(2, 2);
                                            var NamSinh = NgaySinh.Substring(4, 2);
                                            ThiSinh.NgaySinhStr = Ngay + "/" + Thang + "/19" + NamSinh;
                                        }
                                        else if (NgaySinh.Length == 8 && !NgaySinh.Contains("/"))
                                        {
                                            var Ngay = NgaySinh.Substring(0, 2);
                                            var Thang = NgaySinh.Substring(2, 2);
                                            var NamSinh = NgaySinh.Substring(4, 4);
                                            ThiSinh.NgaySinhStr = Ngay + "/" + Thang + "/" + NamSinh;
                                        }
                                        else ThiSinh.NgaySinhStr = NgaySinh;
                                        ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(ThiSinh.NgaySinhStr, null);
                                        if(ThiSinh.NgaySinh != null && ThiSinh.NgaySinhStr.Length > 10)
                                        {
                                            ThiSinh.NgaySinhStr = ThiSinh.NgaySinh.Value.ToString("dd/MM/yyyy");
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NS"))
                                {
                                    ThiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GT"))
                                {
                                    var _gioiTinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (_gioiTinh.ToLower() == "nam")
                                    {
                                        ThiSinh.GioiTinh = false;
                                    }
                                    else if (_gioiTinh.ToLower() == "nữ")
                                    {
                                        ThiSinh.GioiTinh = true;
                                    }
                                    
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DT"))
                                {
                                    
                                    string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(DT))
                                    {
                                        
                                        var DanToc = DanhMucChung.Where(x => x.Ten.Contains(DT) && x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).FirstOrDefault();
                                        if (DanToc != null && DanToc.ID > 0)
                                        {
                                            ThiSinh.DanToc = DanToc.ID;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HST"))
                                {
                                    var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenTruong))
                                    {

                                        var DanhMucTruong = DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).ToList();
                                        var TruongModel = DanhMucTruong.Where(x => ((x.Ten.ToLower().Equals(TenTruong.ToLower())
                                                                                    || TenTruong.ToLower().Equals(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(TenTruong.ToLower()).Equals(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Equals(Utils.ConvertToUnSign(TenTruong.ToLower())))
                                                                                    && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                                                            .FirstOrDefault();
                                        if (TruongModel != null && TruongModel.ID > 0)
                                        {
                                            ThiSinh.TruongTHPT = TruongModel.ID;
                                            ThiSinh.TenTruongTHPT = TruongModel.Ten;
                                        }
                                        else
                                        {
                                            ThiSinh.TenTruongTHPT = TenTruong;
                                            //thêm mới dm trường
                                            DanhMucChungModel danhMucChungModel = new DanhMucChungModel();
                                            danhMucChungModel.Ten = TenTruong;
                                            danhMucChungModel.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                            var baseResult = new DanhMucChungDAL().Insert(danhMucChungModel);
                                            int ID = Utils.ConvertToInt32(baseResult.Data, 0);
                                            if (ID > 0)
                                            {
                                                danhMucChungModel.ID = ID;
                                                DanhMucChung.Add(danhMucChungModel);
                                                ThiSinh.TruongTHPT = ID;
                                            }
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HK"))
                                {
                                    var TenHanhKiem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenHanhKiem))
                                    {
                                        var HanhKiemModel = DanhMucChung.Where(x => x.Ten.Contains(TenHanhKiem) && x.Loai == EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode()).FirstOrDefault();
                                        if (HanhKiemModel != null && HanhKiemModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHanhKiem = HanhKiemModel.ID;
                                            ThiSinh.XepLoaiHanhKiemStr = HanhKiemModel.Ten;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HL"))
                                {
                                    var TenHocLuc = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenHocLuc))
                                    {
                                        var HocLucModel = DanhMucChung.Where(x => x.Ten.Contains(TenHocLuc) && x.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode()).FirstOrDefault();
                                        if (HocLucModel != null && HocLucModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHocLuc = HocLucModel.ID;
                                            ThiSinh.XepLoaiHocLucStr = HocLucModel.Ten;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXTN"))
                                {
                                    //ThiSinh.DiemXetTotNghiep = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemXetTotNghiep = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐKK"))
                                {
                                    //ThiSinh.DiemKK = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemKK = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
                                {
                                    //ThiSinh.TongSoDiemThi = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimalFromIntForTongDiemThi(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXL"))
                                {
                                    //ThiSinh.DiemXL = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemXL = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐL12"))
                                {
                                    //ThiSinh.DiemTBLop12 = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐƯT"))
                                {
                                    //ThiSinh.DiemUT = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemUT = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_Đô"))
                                {
                                    ThiSinh.Do = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐôT"))
                                {
                                    ThiSinh.DoThem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_H"))
                                {
                                    ThiSinh.Hong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLTN") /*|| Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG")*/)
                                {
                                    ThiSinh.KetQuaTotNghiep = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GC"))
                                {
                                    ThiSinh.GhiChu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CMND/CCCD"))
                                {
                                    ThiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SĐT"))
                                {
                                    ThiSinh.SoDienThoai = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_L"))
                                {
                                    ThiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LDT"))
                                {
                                    ThiSinh.LoaiDuThi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐVĐKDT"))
                                {
                                    ThiSinh.DonViDKDT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LĐ"))
                                {
                                    ThiSinh.LaoDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VH"))
                                {
                                    ThiSinh.VanHoa = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_RLTT"))
                                {
                                    ThiSinh.RLTT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SHB"))
                                {
                                    ThiSinh.SoHieuBang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
                                {
                                    ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NCB"))
                                {
                                    ThiSinh.NgayCapBang = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
                                {
                                    ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
                                {
                                    ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimalFromIntForTongDiemThi(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG"))
                                {
                                    ThiSinh.Hang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Contains("MH"))
                                {
                                    var thongTinDiemMonHoc = new ThongTinDiemThi();                                  
                                    thongTinDiemMonHoc.DiemBaiToHop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (thongTinDiemMonHoc.Diem != null || thongTinDiemMonHoc.DiemBaiToHop != string.Empty)
                                    {
                                        var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                        var arrTemp = temp.Split("||");
                                        thongTinDiemMonHoc.NhomID = arrTemp.Length > 0 ? Utils.ConvertToNullableInt32(arrTemp[1], null) : null;
                                        var TenMonHoc = Utils.ConvertToString(workSheet.Cells[StartDataBody - 2, j].Value, string.Empty);
                                        var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                        if (MonHocModel != null && MonHocModel.ID > 0)
                                        {
                                            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                        }

                                        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐTB"))
                                {                                 
                                    var DiemTrungBinh = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBCacBaiThi = DiemTrungBinh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DUT"))
                                {
                                    var DienUuTien = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.DienUuTien = DienUuTien;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐTBC"))
                                {                              
                                    var DiemTBC = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBC = DiemTBC;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUEQUAN"))
                                {
                                    var QueQuan = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.QueQuan = QueQuan;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TB12"))
                                {
                                    var DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBLop12 = DiemTBLop12;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NGHE"))
                                {
                                    var ChungNhanNghe = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.ChungNhanNghe = ChungNhanNghe;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DTCLS"))
                                {
                                    var DTConLietSi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.DTConLietSi = DTConLietSi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GTDKT"))
                                {
                                    var GiaiTDKT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.GiaiTDKT = GiaiTDKT;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐC"))
                                {
                                    ThiSinh.DiaChi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HOIDONG"))
                                {
                                    ThiSinh.HoiDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_MONKN"))
                                {
                                    ThiSinh.MonKN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TBCNKN"))
                                {
                                    ThiSinh.TBCNMonKN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHICU"))
                                {
                                    ThiSinh.DiemThiCu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHIMOI"))
                                {
                                    ThiSinh.DiemThiMoi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TONGBQ"))
                                {
                                    ThiSinh.TongBQ = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQA"))
                                {
                                    ThiSinh.BQA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQT"))
                                {
                                    ThiSinh.BQT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DC"))
                                {
                                    ThiSinh.DC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BAN"))
                                {
                                    ThiSinh.Ban = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUOCGIA"))
                                {
                                    ThiSinh.BODY_QUOCGIA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                            }

                            if (!string.IsNullOrEmpty(ThiSinh.HoTen))
                            {
                                result.ThongTinThiSinh.Add(ThiSinh);
                            }

                        }

                        #endregion

                        #region đọc data cuối mẫu phiếu
                       
                        for (int i = EndDataBody + 2; i <= workSheet.Dimension.End.Row; i++)
                        {
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                var test = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower();
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người đọc điểm"))
                                {
                                    ThongTinToChucThiModel.NguoiDocDiem = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người nhập điểm"))
                                {
                                    ThongTinToChucThiModel.NguoiNhapVaInDiem = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người đọc soát"))
                                {
                                    ThongTinToChucThiModel.NguoiDocSoatBanGhi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("CTHĐ chấm thi"))
                                {
                                    ThongTinToChucThiModel.ChuTichHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày duyệt chấm"))
                                {
                                    var NgayDuyetCham = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
                                    ThongTinToChucThiModel.NgayDuyetCham = NgayDuyetCham;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số thí sinh dự thi"))
                                {
                                    var SoThiSinhDuThi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.SoThiSinhDuThi = SoThiSinhDuThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("được công nhận tốt nghiệp:"))
                                {
                                    var DuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DuocCongNhanTotNghiep = DuocCongNhanTN;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("không được công nhận tốt nghiệp:"))
                                {
                                    var KhongDuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.KhongDuocCongNhanTotNghiep = KhongDuocCongNhanTN;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại giỏi"))
                                {
                                    var SoLoaiGioi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiGioi = SoLoaiGioi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại khá"))
                                {
                                    var SoLoaiKha = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiKha = SoLoaiKha;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại tb"))
                                {
                                    var SoLoaiTB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiTB = SoLoaiTB;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện a"))
                                {
                                    var TotNghiepDienA = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienA = TotNghiepDienA;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện b"))
                                {
                                    var TotNghiepDienB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienB = TotNghiepDienB;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện c"))
                                {
                                    var TotNghiepDienC = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienC = TotNghiepDienC;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,5"))
                                {
                                    var DienTN4_5 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep4_5 = DienTN4_5;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,75"))
                                {
                                    var DienTN4_75 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep4_75 = DienTN4_75;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 2"))
                                {
                                    var DienTN2 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep2 = DienTN2;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 3"))
                                {
                                    var DienTN3 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep3 = DienTN3;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày sở duyệt"))
                                {
                                    var NgaySoDuyet = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
                                    ThongTinToChucThiModel.NgaySoDuyet = NgaySoDuyet;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Cán bộ sở KT"))
                                {
                                    ThongTinToChucThiModel.CanBoSoKT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Giám đốc sở GD"))
                                {
                                    ThongTinToChucThiModel.GiamDocSo = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thư ký"))
                                {
                                    ThongTinToChucThiModel.ThuKy = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Phó chủ khảo"))
                                {
                                    ThongTinToChucThiModel.PhoChuKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chánh chủ khảo"))
                                {
                                    ThongTinToChucThiModel.ChanhChuKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tổ trưởng hồi phách"))
                                {
                                    var ToTruongHoiPhach = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ToTruongHoiPhach = ToTruongHoiPhach;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Hiệu trưởng"))
                                {
                                    var HieuTruong = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HieuTruong = HieuTruong;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng coi thi"))
                                {
                                    var CTHĐCT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDongCoiThi = CTHĐCT;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng:"))
                                {
                                    var CTHĐ = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDong = CTHĐ;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Cán bộ xét duyệt"))
                                {
                                    var CanBoXetDuyet = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.CanBoXetDuyet = CanBoXetDuyet;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("CTHĐ chấm thi"))
                                {
                                    var ChuTichHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDongChamThi = ChuTichHoiDongChamThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Địa danh"))
                                {
                                    var DiaDanh = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.DiaDanh = DiaDanh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ghi chú"))
                                {
                                    var GhiChuCuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.GhiChuCuoiTrang = GhiChuCuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Từ SBD"))
                                {
                                    var SBDDau_CuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDDau_CuoiTrang = SBDDau_CuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Đến SBD"))
                                {
                                    var SBDDau_CuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDCuoi_CuoiTrang = SBDDau_CuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh đỗ thẳng"))
                                {
                                    var TSDoThang = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSDoThang = TSDoThang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh đỗ thêm"))
                                {
                                    var TSDoThem = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSDoThem = TSDoThem;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh thi hỏng"))
                                {
                                    var TSThiHong = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSThiHong = TSThiHong;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Phó Giám đốc Sở Giáo dục"))
                                {
                                    var PGiamDoc = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.PGiamDoc = PGiamDoc;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người kiểm tra"))
                                {
                                    var NguoiKiemTra = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.NguoiKiemTra = NguoiKiemTra;
                                }
                            }
                        }
                        #endregion

                        result.ThongTinToChucThi = ThongTinToChucThiModel;

                        if (result.ThongTinThiSinh != null && result.ThongTinThiSinh.Count > 0)
                        {
                            DataTable tableThiSinh = new DataTable();
                            tableThiSinh.Columns.Add("ThiSinhID_Str");
                            tableThiSinh.Columns.Add("KyThiID");
                            tableThiSinh.Columns.Add("HoTen");
                            tableThiSinh.Columns.Add("NgaySinh");
                            tableThiSinh.Columns.Add("NoiSinh");
                            tableThiSinh.Columns.Add("GioiTinh");
                            tableThiSinh.Columns.Add("DanToc");
                            tableThiSinh.Columns.Add("CMND");
                            tableThiSinh.Columns.Add("SoBaoDanh");
                            tableThiSinh.Columns.Add("SoDienThoai");
                            tableThiSinh.Columns.Add("DiaChi");
                            tableThiSinh.Columns.Add("Lop");
                            tableThiSinh.Columns.Add("TruongTHPT");
                            tableThiSinh.Columns.Add("LoaiDuThi");
                            tableThiSinh.Columns.Add("DonViDKDT");
                            tableThiSinh.Columns.Add("XepLoaiHanhKiem");
                            tableThiSinh.Columns.Add("XepLoaiHocLuc");
                            tableThiSinh.Columns.Add("DiemTBLop12");
                            tableThiSinh.Columns.Add("DiemKK");
                            tableThiSinh.Columns.Add("DienXTN");
                            tableThiSinh.Columns.Add("HoiDongThi");
                            tableThiSinh.Columns.Add("DiemXetTotNghiep");
                            tableThiSinh.Columns.Add("KetQuaTotNghiep");
                            tableThiSinh.Columns.Add("SoHieuBang");
                            tableThiSinh.Columns.Add("VaoSoCapBangSo");
                            tableThiSinh.Columns.Add("NamThi");
                            tableThiSinh.Columns.Add("Do");
                            tableThiSinh.Columns.Add("DoThem");
                            tableThiSinh.Columns.Add("Hong");
                            tableThiSinh.Columns.Add("LaoDong");
                            tableThiSinh.Columns.Add("VanHoa");
                            tableThiSinh.Columns.Add("RLTT");
                            tableThiSinh.Columns.Add("TongSoDiemThi");
                            tableThiSinh.Columns.Add("NgayCapBang");
                            tableThiSinh.Columns.Add("DiemXL");
                            tableThiSinh.Columns.Add("DiemUT");
                            tableThiSinh.Columns.Add("GhiChu");
                            tableThiSinh.Columns.Add("Hang");
                            tableThiSinh.Columns.Add("DiemTBCacBaiThi");
                            tableThiSinh.Columns.Add("DienUuTien");
                            tableThiSinh.Columns.Add("DiemTBC");

                            foreach (var item in result.ThongTinThiSinh)
                            {
                                var ThiSinhID_Str = Guid.NewGuid().ToString();
                                tableThiSinh.Rows.Add(ThiSinhID_Str,
                                                        item.KyThiID,
                                                        item.HoTen,
                                                        item.NgaySinh,
                                                        item.NoiSinh,
                                                        item.GioiTinh,
                                                        item.DanToc,
                                                        item.CMND,
                                                        item.SoBaoDanh,
                                                        item.SoDienThoai,
                                                        item.DiaChi,
                                                        item.Lop,
                                                        item.TruongTHPT,
                                                        item.LoaiDuThi,
                                                        item.DonViDKDT,
                                                        item.XepLoaiHanhKiem,
                                                        item.XepLoaiHocLuc,
                                                        item.DiemTBLop12,
                                                        item.DiemKK,
                                                        item.DienXTN,
                                                        item.HoiDongThi,
                                                        item.DiemXetTotNghiep,
                                                        item.KetQuaTotNghiep,
                                                        item.SoHieuBang,
                                                        item.VaoSoCapBangSo,
                                                        item.NamThi,
                                                        item.Do,
                                                        item.DoThem,
                                                        item.Hong,
                                                        item.LaoDong,
                                                        item.VanHoa,
                                                        item.RLTT,
                                                        item.TongSoDiemThi,
                                                        item.NgayCapBang,
                                                        item.DiemXL,
                                                        item.DiemUT,
                                                        item.GhiChu,
                                                        item.Hang,
                                                        item.DiemTBCacBaiThi,
                                                        item.DienUuTien,
                                                        item.DiemTBC
                                                        );
                            }

                            List<string> listThiSinhTrungCMNN = new List<string>();

                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter("TableThiSinh", SqlDbType.Structured),
                            };
                            parameters[0].Value = tableThiSinh;

                            try
                            {
                                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_SLTrungCMND_New", parameters))
                                {
                                    while (dr.Read())
                                    {
                                        var CMND = Utils.ConvertToString(dr["CMND"], String.Empty);
                                        listThiSinhTrungCMNN.Add(CMND);
                                        //var SLTrungCMND = Utils.ConvertToInt32(dr["SL"], 0);
                                        //if (SLTrungCMND > 0) Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";
                                    }
                                    dr.Close();
                                }

                            }
                            catch { }

                            if (listThiSinhTrungCMNN.Count > 0)
                            {
                                foreach (var item in result.ThongTinThiSinh)
                                {
                                    for (int i = 0; i < listThiSinhTrungCMNN.Count; i++)
                                    {
                                        if (item.CMND != "" && item.CMND == listThiSinhTrungCMNN[i])
                                        {
                                            item.DanhSachLoi += "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                            Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Mess = ex.Message;
                throw ex;
            }
            return result;
        }

        public DuLieuDiemThiModel ReadFileExcelBo_Old(ExcelWorksheet workSheet)
        {
            var result = new DuLieuDiemThiModel();
            result.ThongTinThiSinh = new List<ThongTinThiSinh>();
            var DanhMucChung = new List<DanhMucChungModel>();
            DanhMucChung = new DanhMucChungDAL().GetAll(0);
            try
            {
                int StartDataBody = 0;
                int EndDataBody = 0;
                int rowTieuDe = 0;
                EndDataBody = workSheet.Dimension.End.Row;
                var totalRow = workSheet.Dimension.End.Row;
                var totalColumn = workSheet.Dimension.End.Column;
                int Nam = 0;
                string TenMauPhieu = "";
                for (int i = 1; i <= totalRow; i++)
                {
                    for (int j = 1; j <= totalColumn; j++)
                    {
                        var temp = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty);
                        if (temp.Contains("STT") || temp.ToLower().Contains("họ tên") || temp.ToLower().Contains("họ và tên"))
                        {
                            rowTieuDe = i;
                            StartDataBody = i + 1;
                            break;
                        }
                    }
                }

                //lấy ra danh sách môn học trong file bẳng điểm
                var DanhSachMonHocTrongFile = new List<string>();
                for (int j = 0; j <= totalColumn; j++)
                {
                    var temp = Utils.ConvertToString(workSheet.Cells[rowTieuDe, j].Value, string.Empty);
                    var tempKhongDau = Utils.ConvertToUnSign(temp.ToLower());
                    if (temp.ToLower().Contains("điểm thi") || temp.ToLower().Contains("điểm lớp 12") || temp.ToLower().Contains("điểm các bài thi") || temp.ToLower().Contains("điểm bài thi")
                           || tempKhongDau.Contains("diem thi") || tempKhongDau.Contains("diem lop 12") || tempKhongDau.Contains("diem cac bai thi") || tempKhongDau.Contains("diem bai thi"))
                    {
                        //rowTieuDe = i;
                        //StartDataBody = i + 1;
                        //break;
                    }
                }

                #region đọc data bảng điểm
                var ThongTinToChucThiModel = new ThongTinToChucThi();
                result.ThongTinToChucThi.TenHoiDongThi = "Phần mềm thi của bộ";

                for (int i = StartDataBody; i <= EndDataBody; i++)
                {
                    var ThiSinh = new ThongTinThiSinh();
                    ThiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                    //ThiSinh.HoiDongThi = ThongTinToChucThiModel.HoiDongThiID;
                    for (int j = 1; j <= totalColumn; j++)
                    {
                        var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).ToLower();
                        if (temp.Equals("Số báo danh"))
                        {
                            ThiSinh.SoBaoDanh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("STT"))
                        {
                            ThiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Họ Tên"))
                        {
                            ThiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Ngày sinh"))
                        {
                            ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Nơi sinh"))
                        {
                            ThiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Giới tính"))
                        {
                            var _gioiTinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (_gioiTinh.ToLower() == "nam")
                            {
                                ThiSinh.GioiTinh = false;
                            }
                            else if (_gioiTinh.ToLower() == "nữ")
                            {
                                ThiSinh.GioiTinh = true;
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Dân tộc"))
                        {
                            string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (!string.IsNullOrEmpty(DT))
                            {
                                var DanToc = DanhMucChung.Where(x => x.Ten.Contains(DT) && x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).FirstOrDefault();
                                if (DanToc != null && DanToc.ID > 0)
                                {
                                    ThiSinh.DanToc = DanToc.ID;
                                }
                            }
                        }


                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Trường THPT"))
                        {
                            //var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            //var TruongModel = DanhMucChung.Where(x => x.Ten.Equals(TenTruong) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
                            //if (TruongModel != null && TruongModel.ID > 0)
                            //{
                            //    ThiSinh.TruongTHPT = TruongModel.ID;
                            //    ThiSinh.TenTruongTHPT = TruongModel.Ten;
                            //}
                            var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (!string.IsNullOrEmpty(TenTruong))
                            {
                                //var TruongModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenTruong.ToLower()) || TenTruong.ToLower().Contains(x.Ten.ToLower())) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
                                var TruongModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenTruong.ToLower())
                                                                                    || TenTruong.ToLower().Contains(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(TenTruong.ToLower()).Contains(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(Utils.ConvertToUnSign(TenTruong.ToLower()))
                                                                                    && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                                                            .FirstOrDefault();
                                if (TruongModel != null && TruongModel.ID > 0)
                                {
                                    ThiSinh.TruongTHPT = TruongModel.ID;
                                    ThiSinh.TenTruongTHPT = TruongModel.Ten;
                                }
                                else
                                {
                                    //DanhMucChungModel DM_Truong = new DanhMucChungModel();
                                    //DM_Truong.Ten = TenTruong;
                                    //DM_Truong.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                    //var TempInsert = new DanhMucChungDAL().Insert(DM_Truong);
                                    //_thiSinh.TruongTHPT = Utils.ConvertToNullableInt32(TempInsert.Data, null);
                                    ThiSinh.TenTruongTHPT = TenTruong;
                                }
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Equals("HK"))
                        {
                            var TenHanhKiem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (!string.IsNullOrEmpty(TenHanhKiem))
                            {
                                var HanhKiemModel = DanhMucChung.Where(x => x.Ten.Contains(TenHanhKiem) && x.Loai == EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode()).FirstOrDefault();
                                if (HanhKiemModel != null && HanhKiemModel.ID > 0)
                                {
                                    ThiSinh.XepLoaiHanhKiem = HanhKiemModel.ID;
                                    ThiSinh.XepLoaiHanhKiemStr = HanhKiemModel.Ten;
                                }
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Equals("HL"))
                        {
                            var TenHocLuc = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (!string.IsNullOrEmpty(TenHocLuc))
                            {
                                var HocLucModel = DanhMucChung.Where(x => x.Ten.Contains(TenHocLuc) && x.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode()).FirstOrDefault();
                                if (HocLucModel != null && HocLucModel.ID > 0)
                                {
                                    ThiSinh.XepLoaiHocLuc = HocLucModel.ID;
                                    ThiSinh.XepLoaiHocLucStr = HocLucModel.Ten;
                                }
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Điểm xét tốt nghiệp"))
                        {
                            ThiSinh.DiemXetTotNghiep = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Điểm KK"))
                        {
                            ThiSinh.DiemKK = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Điểm TB lớp 12"))
                        {
                            ThiSinh.DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                        }


                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Kết quả tốt nghiệp") /*|| Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG")*/)
                        {
                            ThiSinh.KetQuaTotNghiep = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }

                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Số CMND"))
                        {
                            ThiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Số điện thoại"))
                        {
                            ThiSinh.SoDienThoai = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Lớp"))
                        {
                            ThiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Loại dự thi"))
                        {
                            ThiSinh.LoaiDuThi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Đơn vị ĐKDT"))
                        {
                            ThiSinh.DonViDKDT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }


                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Contains("Toán"))
                        {
                            var thongTinDiemMonHoc = new ThongTinDiemThi();
                            //thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                            thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                            if (thongTinDiemMonHoc.Diem != null)
                            {
                                //var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                //var arrTemp = temp.Split("||");
                                //thongTinDiemMonHoc.NhomID = arrTemp.Length > 0 ? Utils.ConvertToNullableInt32(arrTemp[1], null) : null;
                                var TenMonHoc = "Toán";
                                var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                if (MonHocModel != null && MonHocModel.ID > 0)
                                {
                                    thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    thongTinDiemMonHoc.NhomID = 1;
                                }

                                ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Contains("Văn"))
                        {
                            var thongTinDiemMonHoc = new ThongTinDiemThi();
                            thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                            if (thongTinDiemMonHoc.Diem != null)
                            {
                                var TenMonHoc = "Văn";
                                var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                if (MonHocModel != null && MonHocModel.ID > 0)
                                {
                                    thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    thongTinDiemMonHoc.NhomID = 1;
                                }

                                ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Contains("Ngoại ngữ"))
                        {
                            var thongTinDiemMonHoc = new ThongTinDiemThi();
                            thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                            if (thongTinDiemMonHoc.Diem != null)
                            {
                                var TenMonHoc = "Ngoại ngữ";
                                var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                if (MonHocModel != null && MonHocModel.ID > 0)
                                {
                                    thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    thongTinDiemMonHoc.NhomID = 1;
                                }

                                ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Contains("Bài thi tổ hợp"))
                        {
                            var thongTinDiemMonHoc = new ThongTinDiemThi();
                            thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                            if (thongTinDiemMonHoc.Diem != null)
                            {
                                var TenMonHoc = "Bài thi tổ hợp";
                                var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                if (MonHocModel != null && MonHocModel.ID > 0)
                                {
                                    thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    thongTinDiemMonHoc.NhomID = 1;
                                }

                                ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                            }
                        }
                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, j].Value, string.Empty).Contains("Tên bài tổ hợp"))
                        {
                            var thongTinDiemMonHoc = new ThongTinDiemThi();
                            //thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                            thongTinDiemMonHoc.DiemBaiToHop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (thongTinDiemMonHoc.DiemBaiToHop != null && thongTinDiemMonHoc.DiemBaiToHop.Length > 0)
                            {
                                var TenMonHoc = "Tên bài tổ hợp";
                                var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                if (MonHocModel != null && MonHocModel.ID > 0)
                                {
                                    thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    thongTinDiemMonHoc.NhomID = 1;
                                }

                                ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                            }
                        }

                        else if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Diện XTN"))
                        {
                            var DienUuTien = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            ThiSinh.DienUuTien = DienUuTien;
                        }

                    }

                    //check thi sinh theo cmnd
                    if (ThiSinh.CMND != null && ThiSinh.CMND.Length > 0)
                    {
                        var info = new QuanLyThiSinhDAL().GetByCMND(ThiSinh.CMND);
                        if (info.ThiSinhID > 0)
                        {
                            ThiSinh.ThiSinhID = info.ThiSinhID;
                            ThiSinh.SoHieuBang = info.SoHieuBang;
                            ThiSinh.NgayCapBang = info.NgayCapBang;
                            ThiSinh.VaoSoCapBangSo = info.VaoSoCapBangSo;
                        }
                    }


                    if (!string.IsNullOrEmpty(ThiSinh.HoTen))
                    {
                        result.ThongTinThiSinh.Add(ThiSinh);
                    }

                }

                #endregion

                TenMauPhieu = "Mẫu export excel của bộ";
                result.ChiTietMauPhieu = GetChiTietByNam(2015, TenMauPhieu);
                try
                {
                    var MauPhieuID = Utils.ConvertToInt32(new SystemConfigDAL().GetByKey("FileExportExcelBo").ConfigValue, 0);
                    if (MauPhieuID > 0)
                    {
                        result.ChiTietMauPhieu = GetByID(MauPhieuID);
                    }
                    else
                    {
                        var DanhSachBody = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_BieuMau_BODY.GetHashCode());
                        MauPhieuModel _mauPhieuModel = new MauPhieuModel();
                        _mauPhieuModel.MauPhieuID = int.MaxValue;
                        _mauPhieuModel.DanhSachChiTietMauPhieu = new List<ChiTietMauPhieuModel>();

                        #region Tạo bảng theo file cấp bằng của bộ
                        var col_STT = DanhSachBody.Where(x => x.Ma == "BODY_STT").FirstOrDefault();
                        if (col_STT != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_STT = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_STT.CotID = col_STT.ID;
                            _chiTietMauPhieu_STT.MaCot = col_STT.Ma;
                            _chiTietMauPhieu_STT.TieuDeCot = col_STT.Ten;
                            _chiTietMauPhieu_STT.ThuTu = 1;
                            _chiTietMauPhieu_STT.Loai = col_STT.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_STT);
                        }

                        var col_HoTen = DanhSachBody.Where(x => x.Ma == "BODY_HT").FirstOrDefault();
                        if (col_HoTen != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_HoTen = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_HoTen.CotID = col_HoTen.ID;
                            _chiTietMauPhieu_HoTen.MaCot = col_HoTen.Ma;
                            _chiTietMauPhieu_HoTen.TieuDeCot = col_HoTen.Ten;
                            _chiTietMauPhieu_HoTen.ThuTu = 2;
                            _chiTietMauPhieu_HoTen.Loai = col_HoTen.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HoTen);
                        }

                        var col_GioiTinh = DanhSachBody.Where(x => x.Ma == "BODY_GT").FirstOrDefault();
                        if (col_GioiTinh != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_GioiTinh = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_GioiTinh.CotID = col_GioiTinh.ID;
                            _chiTietMauPhieu_GioiTinh.MaCot = col_GioiTinh.Ma;
                            _chiTietMauPhieu_GioiTinh.TieuDeCot = col_GioiTinh.Ten;
                            _chiTietMauPhieu_GioiTinh.ThuTu = 3;
                            _chiTietMauPhieu_GioiTinh.Loai = col_GioiTinh.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_GioiTinh);
                        }

                        var col_NgaySinh = DanhSachBody.Where(x => x.Ma == "BODY_NgS").FirstOrDefault();
                        if (col_NgaySinh != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_NgaySinh = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_NgaySinh.CotID = col_NgaySinh.ID;
                            _chiTietMauPhieu_NgaySinh.MaCot = col_NgaySinh.Ma;
                            _chiTietMauPhieu_NgaySinh.TieuDeCot = col_NgaySinh.Ten;
                            _chiTietMauPhieu_NgaySinh.ThuTu = 4;
                            _chiTietMauPhieu_NgaySinh.Loai = col_NgaySinh.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NgaySinh);
                        }

                        var col_NoiSinh = DanhSachBody.Where(x => x.Ma == "BODY_NS").FirstOrDefault();
                        if (col_NoiSinh != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_NoiSinh = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_NoiSinh.CotID = col_NoiSinh.ID;
                            _chiTietMauPhieu_NoiSinh.MaCot = col_NoiSinh.Ma;
                            _chiTietMauPhieu_NoiSinh.TieuDeCot = col_NoiSinh.Ten;
                            _chiTietMauPhieu_NoiSinh.ThuTu = 5;
                            _chiTietMauPhieu_NoiSinh.Loai = col_NoiSinh.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NoiSinh);
                        }

                        var col_DanToc = DanhSachBody.Where(x => x.Ma == "BODY_DT").FirstOrDefault();
                        if (col_DanToc != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_DanToc = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_DanToc.CotID = col_DanToc.ID;
                            _chiTietMauPhieu_DanToc.MaCot = col_DanToc.Ma;
                            _chiTietMauPhieu_DanToc.TieuDeCot = col_DanToc.Ten;
                            _chiTietMauPhieu_DanToc.ThuTu = 6;
                            _chiTietMauPhieu_DanToc.Loai = col_DanToc.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_DanToc);
                        }

                        var col_SoBaoDanh = DanhSachBody.Where(x => x.Ma == "BODY_SBD").FirstOrDefault();
                        if (col_SoBaoDanh != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_SoBaoDanh = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_SoBaoDanh.CotID = col_SoBaoDanh.ID;
                            _chiTietMauPhieu_SoBaoDanh.MaCot = col_SoBaoDanh.Ma;
                            _chiTietMauPhieu_SoBaoDanh.TieuDeCot = col_SoBaoDanh.Ten;
                            _chiTietMauPhieu_SoBaoDanh.ThuTu = 7;
                            _chiTietMauPhieu_SoBaoDanh.Loai = col_SoBaoDanh.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_SoBaoDanh);
                        }

                        var col_CMND = DanhSachBody.Where(x => x.Ma == "BODY_CMND/CCCD").FirstOrDefault();
                        if (col_CMND != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_CMND = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_CMND.CotID = col_CMND.ID;
                            _chiTietMauPhieu_CMND.MaCot = col_CMND.Ma;
                            _chiTietMauPhieu_CMND.TieuDeCot = col_CMND.Ten;
                            _chiTietMauPhieu_CMND.ThuTu = 8;
                            _chiTietMauPhieu_CMND.Loai = col_CMND.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_CMND);
                        }

                        var col_SDT = DanhSachBody.Where(x => x.Ma == "BODY_SĐT").FirstOrDefault();
                        if (col_SDT != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_SDT = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_SDT.CotID = col_SDT.ID;
                            _chiTietMauPhieu_SDT.MaCot = col_SDT.Ma;
                            _chiTietMauPhieu_SDT.TieuDeCot = col_SDT.Ten;
                            _chiTietMauPhieu_SDT.ThuTu = 9;
                            _chiTietMauPhieu_SDT.Loai = col_SDT.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_SDT);
                        }

                        var col_DiaChi = DanhSachBody.Where(x => x.Ma == "BODY_ĐC").FirstOrDefault();
                        if (col_DiaChi != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_DiaChi = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_DiaChi.CotID = col_DiaChi.ID;
                            _chiTietMauPhieu_DiaChi.MaCot = col_DiaChi.Ma;
                            _chiTietMauPhieu_DiaChi.TieuDeCot = col_DiaChi.Ten;
                            _chiTietMauPhieu_DiaChi.ThuTu = 10;
                            _chiTietMauPhieu_DiaChi.Loai = col_DiaChi.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_DiaChi);
                        }

                        var col_Lop = DanhSachBody.Where(x => x.Ma == "BODY_L").FirstOrDefault();
                        if (col_Lop != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_Lop = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_Lop.CotID = col_Lop.ID;
                            _chiTietMauPhieu_Lop.MaCot = col_Lop.Ma;
                            _chiTietMauPhieu_Lop.TieuDeCot = col_Lop.Ten;
                            _chiTietMauPhieu_Lop.ThuTu = 11;
                            _chiTietMauPhieu_Lop.Loai = col_Lop.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_Lop);
                        }

                        var col_Truong = DanhSachBody.Where(x => x.Ma == "BODY_HST").FirstOrDefault();
                        if (col_Truong != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_Truong = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_Truong.CotID = col_Truong.ID;
                            _chiTietMauPhieu_Truong.MaCot = col_Truong.Ma;
                            _chiTietMauPhieu_Truong.TieuDeCot = col_Truong.Ten;
                            _chiTietMauPhieu_Truong.ThuTu = 12;
                            _chiTietMauPhieu_Truong.Loai = col_Truong.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_Truong);
                        }

                        var col_LoaiDuThi = DanhSachBody.Where(x => x.Ma == "BODY_LDT").FirstOrDefault();
                        if (col_LoaiDuThi != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_LoaiDuThi = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_LoaiDuThi.CotID = col_LoaiDuThi.ID;
                            _chiTietMauPhieu_LoaiDuThi.MaCot = col_LoaiDuThi.Ma;
                            _chiTietMauPhieu_LoaiDuThi.TieuDeCot = col_LoaiDuThi.Ten;
                            _chiTietMauPhieu_LoaiDuThi.ThuTu = 13;
                            _chiTietMauPhieu_LoaiDuThi.Loai = col_LoaiDuThi.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_LoaiDuThi);
                        }

                        var col_DonViDkDT = DanhSachBody.Where(x => x.Ma == "BODY_ĐVĐKDT").FirstOrDefault();
                        if (col_DonViDkDT != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_DonViDKDT = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_DonViDKDT.CotID = col_DonViDkDT.ID;
                            _chiTietMauPhieu_DonViDKDT.MaCot = col_DonViDkDT.Ma;
                            _chiTietMauPhieu_DonViDKDT.TieuDeCot = col_DonViDkDT.Ten;
                            _chiTietMauPhieu_DonViDKDT.ThuTu = 14;
                            _chiTietMauPhieu_DonViDKDT.Loai = col_DonViDkDT.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_DonViDKDT);
                        }

                        var col_HanhKiem = DanhSachBody.Where(x => x.Ma == "BODY_HK").FirstOrDefault();
                        if (col_HanhKiem != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_HanhKiem = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_HanhKiem.CotID = col_HanhKiem.ID;
                            _chiTietMauPhieu_HanhKiem.MaCot = col_HanhKiem.Ma;
                            _chiTietMauPhieu_HanhKiem.TieuDeCot = col_HanhKiem.Ten;
                            _chiTietMauPhieu_HanhKiem.ThuTu = 15;
                            _chiTietMauPhieu_HanhKiem.Loai = col_HanhKiem.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HanhKiem);
                        }

                        var col_HocLuc = DanhSachBody.Where(x => x.Ma == "BODY_HL").FirstOrDefault();
                        if (col_HocLuc != null)
                        {
                            ChiTietMauPhieuModel _chiTietMauPhieu_HocLuc = new ChiTietMauPhieuModel();
                            _chiTietMauPhieu_HocLuc.CotID = col_HocLuc.ID;
                            _chiTietMauPhieu_HocLuc.MaCot = col_HocLuc.Ma;
                            _chiTietMauPhieu_HocLuc.TieuDeCot = col_HocLuc.Ten;
                            _chiTietMauPhieu_HocLuc.ThuTu = 16;
                            _chiTietMauPhieu_HocLuc.Loai = col_HocLuc.Loai;
                            _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HocLuc);
                        }

                        //var col_HocLuc = DanhSachBody.Where(x => x.Ma == "BODY_HL").FirstOrDefault();
                        //if (col_HocLuc != null)
                        //{
                        //    ChiTietMauPhieuModel _chiTietMauPhieu_HocLuc = new ChiTietMauPhieuModel();
                        //    _chiTietMauPhieu_HocLuc.CotID = col_HocLuc.ID;
                        //    _chiTietMauPhieu_HocLuc.MaCot = col_HocLuc.Ma;
                        //    _chiTietMauPhieu_HocLuc.TieuDeCot = col_HocLuc.Ten;
                        //    _chiTietMauPhieu_HocLuc.ThuTu = 16;
                        //    _chiTietMauPhieu_HocLuc.Loai = col_HocLuc.Loai;
                        //    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HocLuc);
                        //}

                        #endregion
                    }
                }
                catch (Exception)
                {

                }



                result.ThongTinToChucThi = ThongTinToChucThiModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public DuLieuDiemThiModel ReadFileExcelBo(ExcelWorksheet workSheet)
        {
            var result = new DuLieuDiemThiModel();
            result.ThongTinThiSinh = new List<ThongTinThiSinh>();
            var DanhMucChung = new List<DanhMucChungModel>();
            DanhMucChung = new DanhMucChungDAL().GetAll(0);
            try
            {
                int StartDataBody = 0;
                int EndDataBody = 0;
                EndDataBody = workSheet.Dimension.End.Row;
                var row = workSheet.Dimension.End.Row;
                var col = workSheet.Dimension.End.Column;
                int Nam = 0;
                string TenMauPhieu = "";
                for (int i = 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    for (int j = 1; j < workSheet.Dimension.End.Column; j++)
                    {
                        if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("STT"))
                        {
                            StartDataBody = i;
                            break;
                        }
                    }
                }

                TenMauPhieu = "Mẫu export excel của bộ";
                result.ChiTietMauPhieu = GetChiTietByNam(2015, TenMauPhieu);
                try
                {
                    var MauPhieuID = Utils.ConvertToInt32(new SystemConfigDAL().GetByKey("FileExportExcelBo").ConfigValue, 0);
                    if (MauPhieuID > 0)
                    {
                        result.ChiTietMauPhieu = GetByID(MauPhieuID);
                    }
                }
                catch (Exception)
                {

                }

                #region đọc data bảng điểm
                var ThongTinToChucThiModel = new ThongTinToChucThi();
                for (int i = 3; i < EndDataBody; i++)
                {
                    for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                    {
                        if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).ToLower().Contains("hội đồng thi"))
                        {
                            var TenHoiDongThi = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty);
                            if (TenHoiDongThi != null && TenHoiDongThi.Length > 0)
                            {
                                ThongTinToChucThiModel.TenHoiDongThi = TenHoiDongThi;
                            }
                            break;
                        }
                    }
                }

                for (int i = 3; i <= EndDataBody; i++)
                {
                    var ThiSinh = new ThongTinThiSinh();
                    ThiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                    ThiSinh.HoiDongThi = ThongTinToChucThiModel.HoiDongThiID;
                    for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                    {
                        var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).ToLower();
                        var tempUnsign = Utils.ConvertToUnSign(temp);
                        //if (Utils.ConvertToString(workSheet.Cells[StartDataBody, j].Value, string.Empty).Equals("Số báo danh"))
                        if (temp.Equals("số báo danh") || tempUnsign.Equals("so bao danh") || temp.Equals("sbd"))
                        {
                            ThiSinh.SoBaoDanh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("stt"))
                        {
                            ThiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("họ tên") || temp.Equals("họ và tên") || temp.Equals("họ và tên thí sinh") || tempUnsign.Equals("ho ten") || tempUnsign.Equals("ho va ten") || tempUnsign.Equals("ho va ten thi sinh"))
                        {
                            ThiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("ngày sinh") || tempUnsign.Equals("ngay sinh"))
                        {
                            ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                        }
                        else if (temp.Equals("nơi sinh") || tempUnsign.Equals("noi sinh"))
                        {
                            ThiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("giới tính") || tempUnsign.Equals("gioi tinh"))
                        {
                            var _gioiTinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (_gioiTinh.ToLower() == "nam")
                            {
                                ThiSinh.GioiTinh = false;
                            }
                            else if (_gioiTinh.ToLower() == "nữ")
                            {
                                ThiSinh.GioiTinh = true;
                            }
                        }
                        else if (temp.Equals("dân tộc") || tempUnsign.Equals("dan toc"))
                        {
                            string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (!string.IsNullOrEmpty(DT))
                            {
                                var DanToc = DanhMucChung.Where(x => x.Ten.Contains(DT) && x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).FirstOrDefault();
                                if (DanToc != null && DanToc.ID > 0)
                                {
                                    ThiSinh.DanToc = DanToc.ID;
                                }
                            }
                        }


                        else if (temp.Equals("trường thpt") || tempUnsign.Equals("truong thpt"))
                        {
                            //var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            //var TruongModel = DanhMucChung.Where(x => x.Ten.Equals(TenTruong) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
                            //if (TruongModel != null && TruongModel.ID > 0)
                            //{
                            //    ThiSinh.TruongTHPT = TruongModel.ID;
                            //    ThiSinh.TenTruongTHPT = TruongModel.Ten;
                            //}
                            var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            if (!string.IsNullOrEmpty(TenTruong))
                            {
                                //var TruongModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenTruong.ToLower()) || TenTruong.ToLower().Contains(x.Ten.ToLower())) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
                                var TruongModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenTruong.ToLower())
                                                                                    || TenTruong.ToLower().Contains(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(TenTruong.ToLower()).Contains(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(Utils.ConvertToUnSign(TenTruong.ToLower()))
                                                                                    && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                                                            .FirstOrDefault();
                                if (TruongModel != null && TruongModel.ID > 0)
                                {
                                    ThiSinh.TruongTHPT = TruongModel.ID;
                                    ThiSinh.TenTruongTHPT = TruongModel.Ten;
                                }
                                else
                                {
                                    //DanhMucChungModel DM_Truong = new DanhMucChungModel();
                                    //DM_Truong.Ten = TenTruong;
                                    //DM_Truong.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                    //var TempInsert = new DanhMucChungDAL().Insert(DM_Truong);
                                    //_thiSinh.TruongTHPT = Utils.ConvertToNullableInt32(TempInsert.Data, null);
                                    ThiSinh.TenTruongTHPT = TenTruong;
                                }
                            }
                        }
                        else if (temp.Equals("xếp loại") || tempUnsign.Equals("xep loai"))
                        {
                            for (int m = j; m <= j + 1; m++)
                            {
                                var tempXepLoai = Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, m].Value, string.Empty).ToLower();
                                var tempXepLoaiUnsign = Utils.ConvertToUnSign(tempXepLoai);
                                if (tempXepLoai.Equals("hk") || tempXepLoai.Equals("hạnh kiểm") || tempXepLoaiUnsign.Equals("hanh kiem"))
                                {
                                    var TenHanhKiem = Utils.ConvertToString(workSheet.Cells[i, m].RichText.Text, string.Empty).ToLower();
                                    var TenHanhKiemUnsign = Utils.ConvertToUnSign(TenHanhKiem);
                                    if (!string.IsNullOrEmpty(TenHanhKiem))
                                    {
                                        var HanhKiemModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenHanhKiem) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenHanhKiemUnsign)) && x.Loai == EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode()).FirstOrDefault();
                                        if (HanhKiemModel != null && HanhKiemModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHanhKiem = HanhKiemModel.ID;
                                            ThiSinh.XepLoaiHanhKiemStr = HanhKiemModel.Ten;
                                        }
                                    }
                                }
                                else if (tempXepLoai.Equals("hl") || tempXepLoai.Equals("học lực") || tempXepLoaiUnsign.Equals("hoc luc"))
                                {
                                    var TenHocLuc = Utils.ConvertToString(workSheet.Cells[i, m].RichText.Text, string.Empty).ToLower();
                                    var TenHocLucUnsign = Utils.ConvertToUnSign(TenHocLuc);
                                    if (!string.IsNullOrEmpty(TenHocLuc))
                                    {
                                        var HocLucModel = DanhMucChung.Where(x => (x.Ten.Contains(TenHocLuc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenHocLucUnsign)) && x.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode()).FirstOrDefault();
                                        if (HocLucModel != null && HocLucModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHocLuc = HocLucModel.ID;
                                            ThiSinh.XepLoaiHocLucStr = HocLucModel.Ten;
                                        }
                                    }
                                }
                            }

                        }
                        else if (temp.Equals("điểm xét tốt nghiệp") || tempUnsign.Equals("diem xet tot nghiep") || temp.Equals("điểm xtn") || tempUnsign.Equals("diem xtn"))
                        {
                            ThiSinh.DiemXetTotNghiep = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                        }
                        else if (temp.Equals("điểm kk") || tempUnsign.Equals("diem kk"))
                        {
                            ThiSinh.DiemKK = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                        }
                        else if (temp.Equals("điểm tb lớp 12") || tempUnsign.Equals("diem tb lop 12"))
                        {
                            ThiSinh.DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                        }
                        else if (temp.Equals("kết quả tốt nghiệp") || tempUnsign.Equals("ket qua tot nghiep")/*|| Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG")*/)
                        {
                            ThiSinh.KetQuaTotNghiep = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("số cmnd") || temp.Equals("cmnd") || tempUnsign.Equals("so cmnd"))
                        {
                            ThiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("số điện thoại") || tempUnsign.Equals("so dien thoai") || temp.Equals("sđt"))
                        {
                            ThiSinh.SoDienThoai = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("lớp") || tempUnsign.Equals("lop"))
                        {
                            ThiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("loại dự thi") || tempUnsign.Equals("loai du thi"))
                        {
                            ThiSinh.LoaiDuThi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("đơn vị đkdt") || tempUnsign.Equals("don vi dkdt"))
                        {
                            ThiSinh.DonViDKDT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                        }
                        else if (temp.Equals("điểm lớp 12") || tempUnsign.Equals("diem lop 12"))
                        {
                            var DanhSachMonHoc = DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).ToList();
                            for (int k = j; k <= col; k++)
                            {
                                var tempMonHoc = Utils.ConvertToString(workSheet.Cells[StartDataBody + 1, k].Value, string.Empty).ToLower();
                                var tempMonHocUnsign = Utils.ConvertToUnSign(tempMonHoc);
                                if (!string.IsNullOrEmpty(tempMonHoc))
                                {
                                    //if (tempMonHoc.Contains("toán") || tempMonHocUnsign.Equals("toan"))
                                    //{
                                    //    var thongTinDiemMonHoc = new ThongTinDiemThi();
                                    //    //thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                                    //    thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, k].RichText.Text, null);
                                    //    if (thongTinDiemMonHoc.Diem != null)
                                    //    {
                                    //        //var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                    //        //var arrTemp = temp.Split("||");
                                    //        //thongTinDiemMonHoc.NhomID = arrTemp.Length > 0 ? Utils.ConvertToNullableInt32(arrTemp[1], null) : null;
                                    //        var TenMonHoc = "Toán";
                                    //        var TenMonHocUnsign = Utils.ConvertToUnSign(TenMonHoc.ToLower());
                                    //        var MonHocModel = DanhSachMonHoc.Where(x => x.Ten.Contains(TenMonHoc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenMonHocUnsign)).FirstOrDefault();
                                    //        if (MonHocModel != null && MonHocModel.ID > 0)
                                    //        {
                                    //            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    //            thongTinDiemMonHoc.NhomID = 1;
                                    //        }

                                    //        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    //    }
                                    //}
                                    //else if (tempMonHoc.Contains("văn") || tempMonHocUnsign.Contains("van"))
                                    //{
                                    //    var thongTinDiemMonHoc = new ThongTinDiemThi();
                                    //    thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, k].RichText.Text, null);
                                    //    if (thongTinDiemMonHoc.Diem != null)
                                    //    {
                                    //        var TenMonHoc = "Văn";
                                    //        var TenMonHocUnsign = Utils.ConvertToUnSign(TenMonHoc.ToLower());
                                    //        var MonHocModel = DanhSachMonHoc.Where(x => x.Ten.Contains(TenMonHoc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenMonHocUnsign)).FirstOrDefault();
                                    //        if (MonHocModel != null && MonHocModel.ID > 0)
                                    //        {
                                    //            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    //            thongTinDiemMonHoc.NhomID = 1;
                                    //        }

                                    //        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    //    }
                                    //}
                                    //else if (tempMonHoc.Contains("ngoại ngữ") || tempMonHocUnsign.Contains("ngoai ngu"))
                                    //{
                                    //    var thongTinDiemMonHoc = new ThongTinDiemThi();
                                    //    thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, k].RichText.Text, null);
                                    //    if (thongTinDiemMonHoc.Diem != null)
                                    //    {
                                    //        var TenMonHoc = "Ngoại ngữ";
                                    //        var TenMonHocUnsign = Utils.ConvertToUnSign(TenMonHoc.ToLower());
                                    //        var MonHocModel = DanhSachMonHoc.Where(x => x.Ten.Contains(TenMonHoc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenMonHocUnsign)).FirstOrDefault();
                                    //        if (MonHocModel != null && MonHocModel.ID > 0)
                                    //        {
                                    //            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    //            thongTinDiemMonHoc.NhomID = 1;
                                    //        }

                                    //        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    //    }
                                    //}
                                    //else if (tempMonHoc.Equals("bài thi tổ hợp") || tempMonHocUnsign.Equals("bai thi to hop"))
                                    //{
                                    //    var thongTinDiemMonHoc = new ThongTinDiemThi();
                                    //    thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, k].RichText.Text, null);
                                    //    if (thongTinDiemMonHoc.Diem != null)
                                    //    {
                                    //        var TenMonHoc = "Bài thi tổ hợp";
                                    //        var TenMonHocUnsign = Utils.ConvertToUnSign(TenMonHoc.ToLower());
                                    //        var MonHocModel = DanhSachMonHoc.Where(x => x.Ten.Contains(TenMonHoc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenMonHocUnsign) /*&& x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()*/).FirstOrDefault();
                                    //        if (MonHocModel != null && MonHocModel.ID > 0)
                                    //        {
                                    //            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                    //            thongTinDiemMonHoc.NhomID = 1;
                                    //        }

                                    //        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    //    }
                                    //}
                                    if (tempMonHoc.Equals("tên bài tổ hợp") || tempMonHocUnsign.Equals("ten bai to hop"))
                                    {
                                        var thongTinDiemMonHoc = new ThongTinDiemThi();
                                        //thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                        thongTinDiemMonHoc.DiemBaiToHop = Utils.ConvertToString(workSheet.Cells[i, k].RichText.Text, string.Empty);
                                        if (thongTinDiemMonHoc.DiemBaiToHop != null && thongTinDiemMonHoc.DiemBaiToHop.Length > 0)
                                        {
                                            var TenMonHoc = "Tên bài tổ hợp";
                                            var TenMonHocUnsign = Utils.ConvertToUnSign(TenMonHoc.ToLower());
                                            var MonHocModel = DanhSachMonHoc.Where(x => x.Ten.Contains(TenMonHoc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenMonHocUnsign)/* && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()*/).FirstOrDefault();
                                            if (MonHocModel != null && MonHocModel.ID > 0)
                                            {
                                                thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                                thongTinDiemMonHoc.NhomID = 1;
                                            }

                                            ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                        }
                                    }
                                    else
                                    {
                                        var thongTinDiemMonHoc = new ThongTinDiemThi();
                                        thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, k].RichText.Text, null);
                                        if (thongTinDiemMonHoc.Diem != null)
                                        {
                                            var MonHocModel = DanhSachMonHoc.Where(x => x.Ten.Contains(tempMonHoc) || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(tempMonHocUnsign) /*&& x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()*/).FirstOrDefault();
                                            if (MonHocModel != null && MonHocModel.ID > 0)
                                            {
                                                thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                                thongTinDiemMonHoc.NhomID = 1;
                                            }

                                            ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                        }
                                    }
                                }
                                else break;
                            }
                        }

                        else if (temp.Equals("diện xtn") || tempUnsign.Equals("dien xtn"))
                        {
                            var DienUuTien = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            ThiSinh.DienUuTien = DienUuTien;
                        }
                        else if (temp.Equals("địa chỉ") || tempUnsign.Equals("dia chi"))
                        {
                            var DiaChi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                            ThiSinh.DiaChi = DiaChi;
                        }

                    }

                    ////check thi sinh theo cmnd
                    //if (ThiSinh.CMND != null && ThiSinh.CMND.Length > 0)
                    //{
                    //    var info = new QuanLyThiSinhDAL().GetByCMND(ThiSinh.CMND);
                    //    if (info.ThiSinhID > 0)
                    //    {
                    //        ThiSinh.ThiSinhID = info.ThiSinhID;
                    //        ThiSinh.SoHieuBang = info.SoHieuBang;
                    //        ThiSinh.NgayCapBang = info.NgayCapBang;
                    //        ThiSinh.VaoSoCapBangSo = info.VaoSoCapBangSo;
                    //    }
                    //}


                    if (!string.IsNullOrEmpty(ThiSinh.HoTen) && (ThiSinh.CMND != null && ThiSinh.CMND.Length > 0))
                    {
                        result.ThongTinThiSinh.Add(ThiSinh);
                    }

                }

                #endregion

                result.ThongTinToChucThi = ThongTinToChucThiModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<MauPhieuModel> GetAll(ref int TotalRow)
        {
            var Result = new List<MauPhieuModel>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    new SqlParameter("@TotalRow",SqlDbType.Int)
                };
                parameters[0].Direction = ParameterDirection.Output;
                parameters[0].Size = 8;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetAll", parameters))
                {
                    while (dr.Read())
                    {
                        MauPhieuModel item = new MauPhieuModel();
                        item.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        item.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        item.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        Result.Add(item);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[0].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public List<MauPhieuModel> GetAll_VBNN(ref int TotalRow)
        {
            var Result = new List<MauPhieuModel>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                    new SqlParameter("@TotalRow",SqlDbType.Int)
                };
                parameters[0].Direction = ParameterDirection.Output;
                parameters[0].Size = 8;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_MauPhieu_GetAll_VBNN", parameters))
                {
                    while (dr.Read())
                    {
                        MauPhieuModel item = new MauPhieuModel();
                        item.MauPhieuID = Utils.ConvertToInt32(dr["MauPhieuID"], 0);
                        item.TenMauPhieu = Utils.ConvertToString(dr["TenMauPhieu"], string.Empty);
                        item.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        Result.Add(item);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[0].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public DuLieuDiemThiModel ReadFileExcel_AllPages(string FilePath, ref string Mess, int? NamTotNghiep)
        {
            var result = new DuLieuDiemThiModel();
            result.DuLieuCuaNam = NamTotNghiep;
            result.ThongTinThiSinh = new List<ThongTinThiSinh>();
            result.ListErrorThiSinh = new List<ErrorThongTinThiSinh>();
            if (!File.Exists(FilePath))
            {
                Mess = "File không tồn tại! Vui lòng kiểm tra lại!";
                return null;
            }
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        Mess = "File không có dữ liệu! Vui lòng kiểm tra lại!";
                        return null;
                    }
                    else
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        int StartDataBody = 0;
                        int EndDataBody = 0;
                        var row = workSheet.Dimension.End.Row;
                        var col = workSheet.Dimension.End.Column;
                        int Nam = 0;
                        string TenMauPhieu = "";
                        var totalRow = workSheet.Dimension.End.Row;
                        var totalCol = workSheet.Dimension.End.Column;

                        var _mauPhieuID = Utils.ConvertToInt32(workSheet.Cells[1, 1].RichText.Text, 0);
                        var _tenMauPhieu = Utils.ConvertToString(workSheet.Cells[1, 2].RichText.Text, string.Empty);

                        var DanhSachTieuDe_Str = "";
                        //result.ChiTietMauPhieu = GetChiTietByNam(Nam, TenMauPhieu);
                        if (_mauPhieuID != 0 && !string.IsNullOrEmpty(_tenMauPhieu))
                        {
                            result.ChiTietMauPhieu = GetByID(_mauPhieuID);
                            ChiTietMauPhieuModel colDanhSachLoi = new ChiTietMauPhieuModel();
                            colDanhSachLoi.TieuDeCot = "Thông tin lỗi";
                            colDanhSachLoi.Loai = 9;
                            colDanhSachLoi.MaCot = "BODY_Loi";
                            colDanhSachLoi.NhomID = 0;
                            colDanhSachLoi.DanhSachCon = new List<ChiTietMauPhieuModel>();
                            result.ChiTietMauPhieu.DanhSachChiTietMauPhieu.Add(colDanhSachLoi);
                        }

                        if (result.ChiTietMauPhieu == null || result.ChiTietMauPhieu.MauPhieuID == null || result.ChiTietMauPhieu.MauPhieuID == 0)
                        {
                            for (int i = 1; i <= totalRow; i++)
                            {
                                for (int j = 1; j <= totalCol; j++)
                                {
                                    var test = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                    if ((Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("stt") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("số thứ tự") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("thứ tự") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("tt"))
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("cmnd")
                                        || (Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("họ và tên") || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("họ tên"))
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("số hiệu bằng")
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("số hiệu bằng")
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("vào sổ cấp bằng số")
                                        || Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower().Contains("điểm")
                                        )
                                    {
                                        DanhSachTieuDe_Str += Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower() + ",";
                                    }
                                }
                                if (!string.IsNullOrEmpty(DanhSachTieuDe_Str)) break;
                            }
                            if (DanhSachTieuDe_Str.Contains("cmnd") && (DanhSachTieuDe_Str.Contains("họ và tên") || DanhSachTieuDe_Str.Contains("họ tên")) && (DanhSachTieuDe_Str.Contains("số hiệu bằng") || DanhSachTieuDe_Str.Contains("vào sổ cấp bằng số")))
                            {
                                var DanhSachBody = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_BieuMau_BODY.GetHashCode());
                                MauPhieuModel _mauPhieuModel = new MauPhieuModel();
                                //_mauPhieuModel.MauPhieuID = int.MaxValue;
                                _mauPhieuModel.DanhSachChiTietMauPhieu = new List<ChiTietMauPhieuModel>();

                                TenMauPhieu = "Mẫu export excel của bộ";
                                //var MauPhieu = GetChiTietByNam(2015, TenMauPhieu);
                                var MauPhieu = new MauPhieuModel();
                                if (MauPhieu != null && MauPhieu.MauPhieuID > 0) _mauPhieuModel.MauPhieuID = MauPhieu.MauPhieuID;
                                try
                                {
                                    var MauPhieuID = Utils.ConvertToInt32(new SystemConfigDAL().GetByKey("FileExportExcelBo").ConfigValue, 0);
                                    if (MauPhieuID > 0)
                                    {
                                        _mauPhieuModel.MauPhieuID = MauPhieuID;
                                    }
                                    else
                                    {
                                        _mauPhieuModel.MauPhieuID = int.MaxValue;
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                #region Tạo bảng theo file cấp bằng của bộ
                                var col_STT = DanhSachBody.Where(x => x.Ma == "BODY_STT").FirstOrDefault();
                                if (col_STT != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_STT = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_STT.CotID = col_STT.ID;
                                    _chiTietMauPhieu_STT.MaCot = col_STT.Ma;
                                    _chiTietMauPhieu_STT.TieuDeCot = col_STT.Ten;
                                    _chiTietMauPhieu_STT.ThuTu = 1;
                                    _chiTietMauPhieu_STT.Loai = col_STT.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_STT);
                                }

                                var col_CMTND = DanhSachBody.Where(x => x.Ma == "BODY_CMND/CCCD").FirstOrDefault();
                                if (col_CMTND != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_CMND = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_CMND.CotID = col_CMTND.ID;
                                    _chiTietMauPhieu_CMND.MaCot = col_CMTND.Ma;
                                    _chiTietMauPhieu_CMND.TieuDeCot = col_CMTND.Ten;
                                    _chiTietMauPhieu_CMND.ThuTu = 2;
                                    _chiTietMauPhieu_CMND.Loai = col_CMTND.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_CMND);
                                }

                                var col_HoTen = DanhSachBody.Where(x => x.Ma == "BODY_HT").FirstOrDefault();
                                if (col_HoTen != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_HoTen = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_HoTen.CotID = col_HoTen.ID;
                                    _chiTietMauPhieu_HoTen.MaCot = col_HoTen.Ma;
                                    _chiTietMauPhieu_HoTen.TieuDeCot = col_HoTen.Ten;
                                    _chiTietMauPhieu_HoTen.ThuTu = 3;
                                    _chiTietMauPhieu_HoTen.Loai = col_HoTen.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HoTen);
                                }

                                var col_GioiTinh = DanhSachBody.Where(x => x.Ma == "BODY_GT").FirstOrDefault();
                                if (col_GioiTinh != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_GioiTinh = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_GioiTinh.CotID = col_GioiTinh.ID;
                                    _chiTietMauPhieu_GioiTinh.MaCot = col_GioiTinh.Ma;
                                    _chiTietMauPhieu_GioiTinh.TieuDeCot = col_GioiTinh.Ten;
                                    _chiTietMauPhieu_GioiTinh.ThuTu = 4;
                                    _chiTietMauPhieu_GioiTinh.Loai = col_GioiTinh.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_GioiTinh);
                                }

                                var col_NgaySinh = DanhSachBody.Where(x => x.Ma == "BODY_NgS").FirstOrDefault();
                                if (col_NgaySinh != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_NgaySinh = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_NgaySinh.CotID = col_NgaySinh.ID;
                                    _chiTietMauPhieu_NgaySinh.MaCot = col_NgaySinh.Ma;
                                    _chiTietMauPhieu_NgaySinh.TieuDeCot = col_NgaySinh.Ten;
                                    _chiTietMauPhieu_NgaySinh.ThuTu = 5;
                                    _chiTietMauPhieu_NgaySinh.Loai = col_NgaySinh.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NgaySinh);
                                }

                                var col_NoiSinh = DanhSachBody.Where(x => x.Ma == "BODY_NS").FirstOrDefault();
                                if (col_NoiSinh != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_NoiSinh = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_NoiSinh.CotID = col_NoiSinh.ID;
                                    _chiTietMauPhieu_NoiSinh.MaCot = col_NoiSinh.Ma;
                                    _chiTietMauPhieu_NoiSinh.TieuDeCot = col_NoiSinh.Ten;
                                    _chiTietMauPhieu_NoiSinh.ThuTu = 6;
                                    _chiTietMauPhieu_NoiSinh.Loai = col_NoiSinh.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NoiSinh);
                                }

                                var col_DanToc = DanhSachBody.Where(x => x.Ma == "BODY_DT").FirstOrDefault();
                                if (col_DanToc != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_DanToc = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_DanToc.CotID = col_DanToc.ID;
                                    _chiTietMauPhieu_DanToc.MaCot = col_DanToc.Ma;
                                    _chiTietMauPhieu_DanToc.TieuDeCot = col_DanToc.Ten;
                                    _chiTietMauPhieu_DanToc.ThuTu = 7;
                                    _chiTietMauPhieu_DanToc.Loai = col_DanToc.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_DanToc);
                                }

                                var col_Lop = DanhSachBody.Where(x => x.Ma == "BODY_L").FirstOrDefault();
                                if (col_Lop != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_Lop = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_Lop.CotID = col_Lop.ID;
                                    _chiTietMauPhieu_Lop.MaCot = col_Lop.Ma;
                                    _chiTietMauPhieu_Lop.TieuDeCot = col_Lop.Ten;
                                    _chiTietMauPhieu_Lop.ThuTu = 8;
                                    _chiTietMauPhieu_Lop.Loai = col_Lop.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_Lop);
                                }

                                var col_SoHieuBang = DanhSachBody.Where(x => x.Ma == "BODY_SHB").FirstOrDefault();
                                if (col_SoHieuBang != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_SoHieuBang = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_SoHieuBang.CotID = col_SoHieuBang.ID;
                                    _chiTietMauPhieu_SoHieuBang.MaCot = col_SoHieuBang.Ma;
                                    _chiTietMauPhieu_SoHieuBang.TieuDeCot = col_SoHieuBang.Ten;
                                    _chiTietMauPhieu_SoHieuBang.ThuTu = 9;
                                    _chiTietMauPhieu_SoHieuBang.Loai = col_SoHieuBang.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_SoHieuBang);
                                }

                                var col_VaoSoCapBang = DanhSachBody.Where(x => x.Ma == "BODY_VSCBS").FirstOrDefault();
                                if (col_VaoSoCapBang != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_VaoSoCapBang = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_VaoSoCapBang.CotID = col_VaoSoCapBang.ID;
                                    _chiTietMauPhieu_VaoSoCapBang.MaCot = col_VaoSoCapBang.Ma;
                                    _chiTietMauPhieu_VaoSoCapBang.TieuDeCot = col_VaoSoCapBang.Ten;
                                    _chiTietMauPhieu_VaoSoCapBang.ThuTu = 10;
                                    _chiTietMauPhieu_VaoSoCapBang.Loai = col_VaoSoCapBang.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_VaoSoCapBang);
                                }

                                var col_NgayCapBang = DanhSachBody.Where(x => x.Ma == "BODY_NCB").FirstOrDefault();
                                if (col_NgayCapBang != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_NgayCapBang = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_NgayCapBang.CotID = col_NgayCapBang.ID;
                                    _chiTietMauPhieu_NgayCapBang.MaCot = col_NgayCapBang.Ma;
                                    _chiTietMauPhieu_NgayCapBang.TieuDeCot = col_NgayCapBang.Ten;
                                    _chiTietMauPhieu_NgayCapBang.ThuTu = 11;
                                    _chiTietMauPhieu_NgayCapBang.Loai = col_NgayCapBang.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_NgayCapBang);
                                }

                                var col_HocSinhTruong = DanhSachBody.Where(x => x.Ma == "BODY_HST").FirstOrDefault();
                                if (col_HocSinhTruong != null)
                                {
                                    ChiTietMauPhieuModel _chiTietMauPhieu_HocSinhTruong = new ChiTietMauPhieuModel();
                                    _chiTietMauPhieu_HocSinhTruong.CotID = col_HocSinhTruong.ID;
                                    _chiTietMauPhieu_HocSinhTruong.MaCot = col_HocSinhTruong.Ma;
                                    _chiTietMauPhieu_HocSinhTruong.TieuDeCot = col_HocSinhTruong.Ten;
                                    _chiTietMauPhieu_HocSinhTruong.ThuTu = 12;
                                    _chiTietMauPhieu_HocSinhTruong.Loai = col_HocSinhTruong.Loai;
                                    _mauPhieuModel.DanhSachChiTietMauPhieu.Add(_chiTietMauPhieu_HocSinhTruong);
                                }
                                #endregion

                                result.ChiTietMauPhieu = _mauPhieuModel;

                                var _DanhMucChung = new List<DanhMucChungModel>();
                                _DanhMucChung = new DanhMucChungDAL().GetAll(0);
                                var DanhMucDanToc = new List<DanhMucChungModel>();
                                DanhMucDanToc = _DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).ToList();
                                var DanhMucTruongHoc = new List<DanhMucChungModel>();
                                DanhMucTruongHoc = _DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).ToList();
                                //đọc dữ liệu file của bộ
                                int rowTieuDe = 0;
                                for (int i = 1; i <= totalRow; i++)
                                {
                                    for (int j = 1; j <= totalCol; j++)
                                    {
                                        var tempTieuDe = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                        var tempTieuDeUnsign = Utils.ConvertToUnSign(tempTieuDe);
                                        if ((tempTieuDe.Equals("stt") || tempTieuDe.Equals("số thứ tự") || tempTieuDe.Equals("thứ tự") || tempTieuDe.Equals("tt"))
                                            || tempTieuDe.Equals("cmnd") || tempTieuDe.Equals("số cmnd") || tempTieuDeUnsign.Equals("so cmnd")
                                            || tempTieuDe.Equals("họ và tên thí sinh") || tempTieuDeUnsign.Equals("ho va ten thi sinh") || tempTieuDe.Equals("họ và tên") || tempTieuDeUnsign.Equals("ho va ten")
                                            || tempTieuDe.Equals("số hiệu bằng") || tempTieuDeUnsign.Equals("so hieu bang")
                                            || tempTieuDe.Equals("vào sổ cấp bằng số") || tempTieuDeUnsign.Equals("vao so cap bang so")
                                            )
                                        {
                                            rowTieuDe = i;
                                            break;
                                        }
                                    }
                                    if (rowTieuDe != 0) break;
                                }
                                int STT = 1;
                                int rowData = rowTieuDe + 1;
                                for (int i = rowData; i <= totalRow; i++)
                                {
                                    ThongTinThiSinh _thiSinh = new ThongTinThiSinh();
                                    _thiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                                    for (int j = 1; j <= totalCol; j++)
                                    {
                                        //if (j == 1)
                                        var TieuDeCot = Utils.ConvertToString(workSheet.Cells[rowTieuDe, j].RichText.Text, string.Empty).ToLower();
                                        var TieuDeCotUnsign = Utils.ConvertToUnSign(TieuDeCot);
                                        if (TieuDeCot.Equals("stt"))
                                        {
                                            _thiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        else if (TieuDeCot.Equals("số cmnd") || TieuDeCot.Equals("cmnd") || TieuDeCotUnsign.Equals("so cmnd"))
                                        {
                                            _thiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 4)
                                        else if (TieuDeCot.Equals("họ và tên thí sinh") || TieuDeCotUnsign.Equals("ho va ten thi sinh") || TieuDeCot.Equals("họ và tên") || TieuDeCot.Equals("họ tên") || TieuDeCotUnsign.Equals("ho ten") || TieuDeCotUnsign.Equals("ho ten thi sinh"))
                                        {
                                            _thiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 5)
                                        else if (TieuDeCot.Equals("giới tính") || TieuDeCotUnsign.Equals("gioi tinh"))
                                        {
                                            var temp = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                            if ((!string.IsNullOrEmpty(temp)) && temp.ToLower() == "nam")
                                                _thiSinh.GioiTinh = false;
                                            else
                                                _thiSinh.GioiTinh = true;
                                        }
                                        //else if (j == 6)
                                        else if (TieuDeCot.Equals("ngày sinh") || TieuDeCotUnsign.Equals("ngay sinh"))
                                        {
                                            _thiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                        }
                                        //else if (j == 7)
                                        else if (TieuDeCot.Equals("nơi sinh") || TieuDeCotUnsign.Equals("noi sinh"))
                                        {
                                            _thiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 8)
                                        else if (TieuDeCot.Equals("dân tộc") || TieuDeCotUnsign.Equals("dan toc"))
                                        {
                                            string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                            if (!string.IsNullOrEmpty(DT))
                                            {
                                                var DanToc = DanhMucDanToc.Where(x => x.Ten.ToLower().Equals(DT) /*&& x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()*/).FirstOrDefault();
                                                if (DanToc != null && DanToc.ID > 0)
                                                {
                                                    _thiSinh.DanToc = DanToc.ID;
                                                }
                                            }
                                        }
                                        //else if (j == 9)
                                        else if (TieuDeCot.Equals("lớp") || TieuDeCotUnsign.Equals("lop"))
                                        {
                                            _thiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 10)
                                        else if (TieuDeCot.Equals("số hiệu bằng") || TieuDeCotUnsign.Equals("so hieu bang"))
                                        {
                                            _thiSinh.SoHieuBang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 11)
                                        else if (TieuDeCot.Equals("vào sổ cấp bằng số") || TieuDeCotUnsign.Equals("vao so cap bang so"))
                                        {
                                            _thiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }
                                        //else if (j == 12)
                                        else if (TieuDeCot.Equals("ngày cấp") || TieuDeCotUnsign.Equals("ngay cap") || TieuDeCotUnsign.Equals("ngay cap bang"))
                                        {
                                            _thiSinh.NgayCapBang = Utils.ConvertToNullableDateTimeFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                        }
                                        //else if (j == 13)
                                        else if (TieuDeCot.Equals("trường") || TieuDeCotUnsign.Equals("truong") || TieuDeCot.Equals("đơn vị") || TieuDeCotUnsign.Equals("don vi") || TieuDeCot.Equals("trường học") || TieuDeCotUnsign.Equals("truong hoc"))
                                        {
                                            var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty).ToLower();
                                            var TenTruongUnsign = Utils.ConvertToUnSign(TenTruong);
                                            if (!string.IsNullOrEmpty(TenTruong))
                                            {
                                                var TruongModel = DanhMucTruongHoc.Where(x => x.Ten.ToLower().Contains(TenTruong)
                                                                                    || TenTruong.Contains(x.Ten.ToLower())
                                                                                     || TenTruongUnsign.Contains(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Contains(TenTruongUnsign)
                                                                                    /*&& x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()*/)
                                                                            .FirstOrDefault();
                                                if (TruongModel != null && TruongModel.ID > 0)
                                                {
                                                    _thiSinh.TruongTHPT = TruongModel.ID;
                                                    _thiSinh.TenTruongTHPT = TruongModel.Ten;
                                                }
                                                else
                                                {
                                                    //DanhMucChungModel DM_Truong = new DanhMucChungModel();
                                                    //DM_Truong.Ten = TenTruong;
                                                    //DM_Truong.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                                    //var TempInsert = new DanhMucChungDAL().Insert(DM_Truong);
                                                    //_thiSinh.TruongTHPT = Utils.ConvertToNullableInt32(TempInsert.Data, null);
                                                    _thiSinh.TenTruongTHPT = TenTruong;
                                                }
                                            }
                                        }
                                        else if (TieuDeCot.Equals("ghi chú") || TieuDeCotUnsign.Equals("ghi chu"))
                                        {
                                            _thiSinh.GhiChu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                        }

                                    }

                                    if (_thiSinh.STT == null || _thiSinh.STT == "0")
                                    {
                                        _thiSinh.STT = STT.ToString();
                                        STT++;
                                    }

                                    //check thi sinh theo cmnd
                                    //if (_thiSinh.CMND != null && _thiSinh.CMND.Length > 0)
                                    //{
                                    //    var info = new QuanLyThiSinhDAL().GetByCMND(_thiSinh.CMND);
                                    //    if (info.ThiSinhID > 0)
                                    //    {
                                    //        info.SoHieuBang = _thiSinh.SoHieuBang;
                                    //        info.NgayCapBang = _thiSinh.NgayCapBang;
                                    //        info.VaoSoCapBangSo = _thiSinh.VaoSoCapBangSo;

                                    //        result.ThongTinThiSinh.Add(info);
                                    //    }
                                    //    else result.ThongTinThiSinh.Add(_thiSinh);
                                    //    ////var info = DanhSachTatCaThiSinh.Where(x => x.CMND == _thiSinh.CMND.Trim()).FirstOrDefault();
                                    //    //if (info != null && info.ThiSinhID > 0)
                                    //    //{
                                    //    //    _thiSinh.ThiSinhID = info.ThiSinhID;
                                    //    //    result.ThongTinThiSinh.Add(_thiSinh);
                                    //    //}
                                    //    //else result.ThongTinThiSinh.Add(_thiSinh);
                                    //}

                                    if (!string.IsNullOrEmpty(_thiSinh.CMND))
                                    {
                                        result.ThongTinThiSinh.Add(_thiSinh);
                                    }
                                }
                                result.ThongTinToChucThi = new ThongTinToChucThi();
                                result.ThongTinToChucThi.TenHoiDongThi = "Phần mềm thi của bộ";
                                return result;
                            }
                            else if (DanhSachTieuDe_Str.Contains("cmnd") && DanhSachTieuDe_Str.Contains("điểm"))
                            {
                                return ReadFileExcelBo(workSheet);
                            }
                            else
                            {
                                Mess = "File không đúng định dạng! Vui lòng thử lại!";
                                return null;
                            }
                        }

                        for (int i = 1; i <= totalRow; i++)
                        {

                            for (int j = 1; j <= totalCol; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("BODY"))
                                {
                                    StartDataBody = i + 1;
                                    break;
                                }
                            }

                            for (int j = 1; j <= totalCol; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("FOOT"))
                                {
                                    EndDataBody = i - 2;
                                    //Nam = Utils.ConvertToInt32(workSheet.Cells[i, j + 1].Value, 0);
                                    //TenMauPhieu = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    break;
                                }
                            }

                        }

                        #region đọc data đầu
                        //var DanhSachHoiDong = new List<DanhMucChungModel>();
                        //DanhSachHoiDong = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_HoiDong.GetHashCode());
                        var DanhMucChung = new List<DanhMucChungModel>();
                        DanhMucChung = new DanhMucChungDAL().GetAll(0);
                        var DanhSachKhoaNgayThi = new List<DanhMucKhoaThiModel>();
                        BasePagingParams p = new BasePagingParams();
                        p.PageSize = 100000;
                        int total = 0;
                        DanhSachKhoaNgayThi = new DanhMucKhoaThiDAL().GetPagingBySearch(p, ref total);

                        var ThongTinToChucThiModel = new ThongTinToChucThi();
                        for (int i = 1; i < StartDataBody - 1; i++)
                        {
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng thi"))
                                {
                                    var TenHoiDongThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    if (ThongTinToChucThiModel.TenHoiDongThi == null || ThongTinToChucThiModel.TenHoiDongThi == "")
                                    {
                                        ThongTinToChucThiModel.TenHoiDongThi = TenHoiDongThi;

                                        ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HD", -1, "", true);
                                        if (TenHoiDongThi.Length > EnumMaxLength.Text.GetHashCode())
                                        {
                                            loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                       
                                    }                 
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng chấm thi"))
                                {
                                    var TenHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    if (ThongTinToChucThiModel.TenHoiDongChamThi == null || ThongTinToChucThiModel.TenHoiDongChamThi == "")
                                    {
                                        ThongTinToChucThiModel.TenHoiDongChamThi = TenHoiDongChamThi;
                                        ThongTinToChucThiModel.HEAD_HDCT = TenHoiDongChamThi;

                                        ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDCT", -1, "", true);
                                        if (TenHoiDongChamThi.Length > EnumMaxLength.Text.GetHashCode())
                                        {
                                            loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                       
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng coi thi"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongCoiThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HĐCT", -1, "", true);
                                    if (ThongTinToChucThiModel.TenHoiDongCoiThi.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                  
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám thị"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongGiamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDGT", -1, "", true);
                                    if (ThongTinToChucThiModel.TenHoiDongGiamThi.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                    
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng giám khảo"))
                                {
                                    ThongTinToChucThiModel.TenHoiDongGiamKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDGK", -1, "", true);
                                    if (ThongTinToChucThiModel.TenHoiDongGiamKhao.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                    
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("khoá ngày thi"))
                                {
                                    ThongTinToChucThiModel.KhoaThiNgay = Utils.ConvertToNullableDateTime(workSheet.Cells[i, j + 2].Value, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("ban:"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.Ban = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("phòng thi"))
                                {
                                    if (ThongTinToChucThiModel.PhongThi == null || ThongTinToChucThiModel.PhongThi == "")
                                    {
                                        var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                        ThongTinToChucThiModel.PhongThi = temp;

                                        ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_PT", -1, "", true);
                                        if (temp.Length > EnumMaxLength.Text.GetHashCode())
                                        {
                                            loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("từ sbd"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDDau = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("đến sbd"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDCuoi = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số quyển"))
                                {
                                    var temp = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SoQuyen = temp;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("QUYEN", -1, "", true);
                                    if (temp.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("trang"))
                                {
                                    var temp = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].Value, null);
                                    ThongTinToChucThiModel.SoTrang = temp;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tỉnh"))
                                {
                                    var Tinh = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    if (ThongTinToChucThiModel.Tinh == null || ThongTinToChucThiModel.Tinh == "")
                                        ThongTinToChucThiModel.Tinh = Tinh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("kỳ thi"))
                                {
                                    var KyThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.TenKyThi = KyThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("trường") || Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("trường:"))
                                {
                                    var HEAD_TRUONG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HEAD_TRUONG = HEAD_TRUONG;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_TRUONG", -1, "", true);
                                    if (HEAD_TRUONG.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("hội đồng chấm lại"))
                                {
                                    var HEAD_HDCL = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HEAD_HDCL = HEAD_HDCL;

                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("HEAD_HDCL", -1, "", true);
                                    if (HEAD_HDCL.Length > EnumMaxLength.Text.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "Không nhập quá " + EnumMaxLength.Text.GetHashCode() + " ký tự";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region đọc data bảng điểm

                        //var DanhSachDanToc = new List<DanhMucChungModel>();
                        //DanhSachDanToc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_DanToc.GetHashCode());

                        //var DanhSachMonHoc = new List<DanhMucChungModel>();
                        //DanhSachMonHoc = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_MonHoc.GetHashCode());
                        int index = 0;
                        for (int i = StartDataBody; i <= EndDataBody + 1; i++)
                        {
                            var ThiSinh = new ThongTinThiSinh();
                            ThiSinh.DanhSachLoi = "";
                            ThiSinh.ListThongTinDiemThi = new List<ThongTinDiemThi>();
                            ThiSinh.HoiDongThi = ThongTinToChucThiModel.HoiDongThiID;
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                //var test = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SBD"))
                                {
                                    ThiSinh.SoBaoDanh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_STT"))
                                {
                                    ThiSinh.STT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HT"))
                                {
                                    ThiSinh.HoTen = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    //if (ThiSinh.HoTen == string.Empty) ThiSinh.DanhSachLoi += "Tên thí sinh không được để trống; ";
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NgS"))
                                {
                                    //ThiSinh.NgaySinh = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                    //ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                    //if (ThiSinh.NgaySinh == null && workSheet.Cells[i, j].RichText.Text != null && workSheet.Cells[i, j].RichText.Text.Length > 0)
                                    //{
                                    //    ThiSinh.DanhSachLoi += "Ngày sinh không đúng định dạng; ";
                                    //}

                                    var NgaySinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (NgaySinh.Length > 0)
                                    {
                                        if (NgaySinh.Length == 6 && !NgaySinh.Contains("/") && !NgaySinh.Contains("-"))
                                        {
                                            var Ngay = NgaySinh.Substring(0, 2);
                                            var Thang = NgaySinh.Substring(2, 2);
                                            var NamSinh = NgaySinh.Substring(4, 2);
                                            ThiSinh.NgaySinhStr = Ngay + "/" + Thang + "/19" + NamSinh;
                                        }
                                        else if (NgaySinh.Length == 8 && !NgaySinh.Contains("/") && !NgaySinh.Contains("-"))
                                        {
                                            var Ngay = NgaySinh.Substring(0, 2);
                                            var Thang = NgaySinh.Substring(2, 2);
                                            var NamSinh = NgaySinh.Substring(4, 4);
                                            ThiSinh.NgaySinhStr = Ngay + "/" + Thang + "/" + NamSinh;
                                        }
                                        else ThiSinh.NgaySinhStr = NgaySinh;
                                        ThiSinh.NgaySinh = Utils.ConvertToNullableDateTimeFromInt(ThiSinh.NgaySinhStr, null);
                                        if (ThiSinh.NgaySinh != null && ThiSinh.NgaySinhStr.Length > 10)
                                        {
                                            ThiSinh.NgaySinhStr = ThiSinh.NgaySinh.Value.ToString("dd/MM/yyyy");
                                        }

                                        ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("BODY_NgS", index, "", true);
                                        if (!Utils.CheckSpecialCharacter(NgaySinh) || NgaySinh.Length > EnumMaxLength.NgaySinh.GetHashCode())
                                        {
                                            loi.DanhSachLoi = "Ngày sinh không hợp lệ";
                                            result.ListErrorThiSinh.Add(loi);
                                        }
                                       
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NS"))
                                {
                                    ThiSinh.NoiSinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GT"))
                                {
                                    var _gioiTinh = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (_gioiTinh.ToLower() == "nam")
                                    {
                                        ThiSinh.GioiTinh = false;
                                    }
                                    else if (_gioiTinh.ToLower() == "nữ")
                                    {
                                        ThiSinh.GioiTinh = true;
                                    }
                                    //ThiSinh.GioiTinh = Utils.ConvertToBoolean(workSheet.Cells[i, j].RichText.Text, false);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DT"))
                                {
                                    //ThiSinh.DanToc = Utils.ConvertToInt32(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    string DT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(DT))
                                    {
                                        //var DanToc = DanhSachDanToc.Where(x => x.Ten.ToLower().Contains(DT)).FirstOrDefault();
                                        var DanToc = DanhMucChung.Where(x => x.Ten.Contains(DT) && x.Loai == EnumLoaiDanhMuc.DM_DanToc.GetHashCode()).FirstOrDefault();
                                        if (DanToc != null && DanToc.ID > 0)
                                        {
                                            ThiSinh.DanToc = DanToc.ID;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HST"))
                                {
                                    var TenTruong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenTruong))
                                    {
                                        //var TruongModel = DanhMucChung.Where(x => (x.Ten.ToLower().Contains(TenTruong.ToLower()) || TenTruong.ToLower().Contains(x.Ten.ToLower())) && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).FirstOrDefault();
                                        var DanhMucTruong = DanhMucChung.Where(x => x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode()).ToList();
                                        var TruongModel = DanhMucTruong.Where(x => ((x.Ten.ToLower().Equals(TenTruong.ToLower())
                                                                                    || TenTruong.ToLower().Equals(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(TenTruong.ToLower()).Equals(Utils.ConvertToUnSign(x.Ten.ToLower()))
                                                                                     || Utils.ConvertToUnSign(x.Ten.ToLower()).Equals(Utils.ConvertToUnSign(TenTruong.ToLower())))
                                                                                    && x.Loai == EnumLoaiDanhMuc.DM_Truong.GetHashCode())
                                                                            .FirstOrDefault();
                                        if (TruongModel != null && TruongModel.ID > 0)
                                        {
                                            ThiSinh.TruongTHPT = TruongModel.ID;
                                            ThiSinh.TenTruongTHPT = TruongModel.Ten;
                                        }
                                        else
                                        {
                                            ThiSinh.TenTruongTHPT = TenTruong;
                                            //thêm mới dm trường
                                            DanhMucChungModel danhMucChungModel = new DanhMucChungModel();
                                            danhMucChungModel.Ten = TenTruong;
                                            danhMucChungModel.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                            var baseResult = new DanhMucChungDAL().Insert(danhMucChungModel);
                                            int ID = Utils.ConvertToInt32(baseResult.Data, 0);
                                            if (ID > 0)
                                            {
                                                danhMucChungModel.ID = ID;
                                                DanhMucChung.Add(danhMucChungModel);
                                                ThiSinh.TruongTHPT = ID;
                                            }
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HK"))
                                {
                                    var TenHanhKiem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenHanhKiem))
                                    {
                                        var HanhKiemModel = DanhMucChung.Where(x => x.Ten.Contains(TenHanhKiem) && x.Loai == EnumLoaiDanhMuc.DM_HanhKiem.GetHashCode()).FirstOrDefault();
                                        if (HanhKiemModel != null && HanhKiemModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHanhKiem = HanhKiemModel.ID;
                                            ThiSinh.XepLoaiHanhKiemStr = HanhKiemModel.Ten;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HL"))
                                {
                                    var TenHocLuc = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (!string.IsNullOrEmpty(TenHocLuc))
                                    {
                                        var HocLucModel = DanhMucChung.Where(x => x.Ten.Contains(TenHocLuc) && x.Loai == EnumLoaiDanhMuc.DM_XepLoai.GetHashCode()).FirstOrDefault();
                                        if (HocLucModel != null && HocLucModel.ID > 0)
                                        {
                                            ThiSinh.XepLoaiHocLuc = HocLucModel.ID;
                                            ThiSinh.XepLoaiHocLucStr = HocLucModel.Ten;
                                        }
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXTN"))
                                {
                                    //ThiSinh.DiemXetTotNghiep = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemXetTotNghiep = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐKK"))
                                {
                                    //ThiSinh.DiemKK = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemKK = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
                                {
                                    //ThiSinh.TongSoDiemThi = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimalFromIntForTongDiemThi(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐXL"))
                                {
                                    //ThiSinh.DiemXL = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemXL = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐL12"))
                                {
                                    //ThiSinh.DiemTBLop12 = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐƯT"))
                                {
                                    //ThiSinh.DiemUT = Utils.ConvertToDecimal(workSheet.Cells[i, j].RichText.Text, 0);
                                    ThiSinh.DiemUT = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, 0);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_Đô"))
                                {
                                    ThiSinh.Do = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐôT"))
                                {
                                    ThiSinh.DoThem = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_H"))
                                {
                                    ThiSinh.Hong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLTN") /*|| Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG")*/)
                                {
                                    ThiSinh.KetQuaTotNghiep = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GC"))
                                {
                                    ThiSinh.GhiChu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CMND/CCCD"))
                                {
                                    ThiSinh.CMND = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("BODY_CMND/CCCD", index, "", true);
                                    if (!Utils.CheckSpecialCharacter(ThiSinh.CMND) || ThiSinh.CMND.Length > EnumMaxLength.CCCD.GetHashCode())
                                    {
                                        loi.DanhSachLoi = "CMND/CCCD không hợp lệ";
                                        result.ListErrorThiSinh.Add(loi);
                                    }
                                    
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SĐT"))
                                {
                                    ThiSinh.SoDienThoai = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_L"))
                                {
                                    ThiSinh.Lop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LDT"))
                                {
                                    ThiSinh.LoaiDuThi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐVĐKDT"))
                                {
                                    ThiSinh.DonViDKDT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_LĐ"))
                                {
                                    ThiSinh.LaoDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VH"))
                                {
                                    ThiSinh.VanHoa = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_RLTT"))
                                {
                                    ThiSinh.RLTT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SHB"))
                                {
                                    ThiSinh.SoHieuBang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
                                {
                                    ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NCB"))
                                {
                                    ThiSinh.NgayCapBang = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty), null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_VSCBS"))
                                {
                                    ThiSinh.VaoSoCapBangSo = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TĐT"))
                                {
                                    ThiSinh.TongSoDiemThi = Utils.ConvertToNullableDecimalFromIntForTongDiemThi(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HANG"))
                                {
                                    ThiSinh.Hang = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, null);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Contains("MH"))
                                {
                                    var thongTinDiemMonHoc = new ThongTinDiemThi();
                                    //thongTinDiemMonHoc.Diem = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    thongTinDiemMonHoc.DiemBaiToHop = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    if (thongTinDiemMonHoc.Diem != null || thongTinDiemMonHoc.DiemBaiToHop != string.Empty)
                                    {
                                        var temp = Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty);
                                        var arrTemp = temp.Split("||");
                                        thongTinDiemMonHoc.NhomID = arrTemp.Length > 0 ? Utils.ConvertToNullableInt32(arrTemp[1], null) : null;
                                        var TenMonHoc = Utils.ConvertToString(workSheet.Cells[StartDataBody - 2, j].Value, string.Empty);
                                        var MonHocModel = DanhMucChung.Where(x => x.Ten.Contains(TenMonHoc) && x.Loai == EnumLoaiDanhMuc.DM_MonHoc.GetHashCode()).FirstOrDefault();
                                        if (MonHocModel != null && MonHocModel.ID > 0)
                                        {
                                            thongTinDiemMonHoc.MonThiID = MonHocModel.ID;
                                        }

                                        ThiSinh.ListThongTinDiemThi.Add(thongTinDiemMonHoc);
                                    }
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐTB"))
                                {
                                    //var DiemTrungBinh = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                                    var DiemTrungBinh = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBCacBaiThi = DiemTrungBinh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DUT"))
                                {
                                    var DienUuTien = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.DienUuTien = DienUuTien;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐTBC"))
                                {
                                    //var DiemTBC = Utils.ConvertToNullableDecimal(workSheet.Cells[i, j].RichText.Text, null);
                                    var DiemTBC = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBC = DiemTBC;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUEQUAN"))
                                {
                                    var QueQuan = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.QueQuan = QueQuan;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TB12"))
                                {
                                    var DiemTBLop12 = Utils.ConvertToNullableDecimalFromInt(workSheet.Cells[i, j].RichText.Text, null);
                                    ThiSinh.DiemTBLop12 = DiemTBLop12;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_NGHE"))
                                {
                                    var ChungNhanNghe = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.ChungNhanNghe = ChungNhanNghe;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DTCLS"))
                                {
                                    var DTConLietSi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.DTConLietSi = DTConLietSi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GTDKT"))
                                {
                                    var GiaiTDKT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                    ThiSinh.GiaiTDKT = GiaiTDKT;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_ĐC"))
                                {
                                    ThiSinh.DiaChi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_HOIDONG"))
                                {
                                    ThiSinh.HoiDong = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_MONKN"))
                                {
                                    ThiSinh.MonKN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TBCNKN"))
                                {
                                    ThiSinh.TBCNMonKN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHICU"))
                                {
                                    ThiSinh.DiemThiCu = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHIMOI"))
                                {
                                    ThiSinh.DiemThiMoi = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TONGBQ"))
                                {
                                    ThiSinh.TongBQ = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQA"))
                                {
                                    ThiSinh.BQA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQT"))
                                {
                                    ThiSinh.BQT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DC"))
                                {
                                    ThiSinh.DC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BAN"))
                                {
                                    ThiSinh.Ban = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DAODUC"))
                                {
                                    ThiSinh.BODY_DAODUC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_RLEV"))
                                {
                                    ThiSinh.BODY_RLEV = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIENKK"))
                                {
                                    ThiSinh.BODY_DIENKK = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_PHONGTHI"))
                                {
                                    ThiSinh.BODY_PHONGTHI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTNC"))
                                {
                                    ThiSinh.BODY_DIEMTNC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLTNC"))
                                {
                                    ThiSinh.BODY_XLTNC = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TDTCU"))
                                {
                                    ThiSinh.BODY_TDTCU = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_KETLUAN"))
                                {
                                    ThiSinh.BODY_KETLUAN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_DIEMTHICUCHA"))
                                {
                                    ThiSinh.BODY_DIEMTHICUCHA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GIAIHSG"))
                                {
                                    ThiSinh.BODY_GIAIHSG = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_GIAIHSGK"))
                                {
                                    ThiSinh.BODY_GIAIHSGK = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CHUNGCHINN"))
                                {
                                    ThiSinh.BODY_CHUNGCHINN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_CHUNGCHITH"))
                                {
                                    ThiSinh.BODY_CHUNGCHITH = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TONGDIEMMOI"))
                                {
                                    ThiSinh.BODY_TONGDIEMMOI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQAMOI"))
                                {
                                    ThiSinh.BODY_BQAMOI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_BQTMOI"))
                                {
                                    ThiSinh.BODY_BQTMOI = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_SOCAPGIAYCN"))
                                {
                                    ThiSinh.BODY_SOCAPGIAYCN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_XLHT"))
                                {
                                    ThiSinh.BODY_XLHT = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUOCGIA"))
                                {
                                    ThiSinh.BODY_QUOCGIA = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_QUYEN"))
                                {
                                    ThiSinh.BODY_QUYEN = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[StartDataBody - 1, j].Value, string.Empty).Equals("BODY_TRANG"))
                                {
                                    ThiSinh.BODY_TRANG = Utils.ConvertToString(workSheet.Cells[i, j].RichText.Text, string.Empty);
                                }

                            }

                            if (!string.IsNullOrEmpty(ThiSinh.HoTen))
                            {
                                ThiSinh.Index = index;
                                result.ThongTinThiSinh.Add(ThiSinh);
                                index++;
                            }

                        }

                        #endregion

                        #region đọc data cuối mẫu phiếu
                        //var DanhSachNguoiKy = new DanhMucChungDAL().GetAll(EnumLoaiDanhMuc.DM_NguoiDuyetKetQua.GetHashCode());
                        for (int i = EndDataBody + 2; i <= workSheet.Dimension.End.Row; i++)
                        {
                            for (int j = 1; j <= workSheet.Dimension.End.Column; j++)
                            {
                                var test = Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower();
                                if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người đọc điểm"))
                                {
                                    ThongTinToChucThiModel.NguoiDocDiem = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người nhập điểm"))
                                {
                                    ThongTinToChucThiModel.NguoiNhapVaInDiem = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người đọc soát"))
                                {
                                    ThongTinToChucThiModel.NguoiDocSoatBanGhi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("CTHĐ chấm thi"))
                                {
                                    ThongTinToChucThiModel.ChuTichHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày duyệt chấm"))
                                {
                                    var NgayDuyetCham = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
                                    ThongTinToChucThiModel.NgayDuyetCham = NgayDuyetCham;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("số thí sinh dự thi"))
                                {
                                    var SoThiSinhDuThi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.SoThiSinhDuThi = SoThiSinhDuThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("được công nhận tốt nghiệp:"))
                                {
                                    var DuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DuocCongNhanTotNghiep = DuocCongNhanTN;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Equals("không được công nhận tốt nghiệp:"))
                                {
                                    var KhongDuocCongNhanTN = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.KhongDuocCongNhanTotNghiep = KhongDuocCongNhanTN;
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại Giỏi"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Loai_Gioi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại Khá"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Loai_Kha = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại TB"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Loai_TB = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại giỏi"))
                                {
                                    var SoLoaiGioi = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiGioi = SoLoaiGioi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại khá"))
                                {
                                    var SoLoaiKha = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiKha = SoLoaiKha;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("loại tb"))
                                {
                                    var SoLoaiTB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TNLoaiTB = SoLoaiTB;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện a"))
                                {
                                    var TotNghiepDienA = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienA = TotNghiepDienA;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện b"))
                                {
                                    var TotNghiepDienB = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienB = TotNghiepDienB;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("tốt nghiệp diện c"))
                                {
                                    var TotNghiepDienC = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TotNghiepDienC = TotNghiepDienC;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,5"))
                                {
                                    var DienTN4_5 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep4_5 = DienTN4_5;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 4,75"))
                                {
                                    var DienTN4_75 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep4_75 = DienTN4_75;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 2"))
                                {
                                    var DienTN2 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep2 = DienTN2;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("diện tốt nghiệp 3"))
                                {
                                    var DienTN3 = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.DienTotNghiep3 = DienTN3;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).ToLower().Contains("ngày sở duyệt"))
                                {
                                    var NgaySoDuyet = Utils.ConvertToNullableDateTime(Utils.ConvertToString(workSheet.Cells[i, j + 2].RichText.Text, string.Empty), null);
                                    ThongTinToChucThiModel.NgaySoDuyet = NgaySoDuyet;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Cán bộ sở KT"))
                                {
                                    ThongTinToChucThiModel.CanBoSoKT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Giám đốc sở GD"))
                                {
                                    ThongTinToChucThiModel.GiamDocSo = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thư ký"))
                                {
                                    ThongTinToChucThiModel.ThuKy = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.FOOT_THUKY = ThongTinToChucThiModel.ThuKy;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Phó chủ khảo"))
                                {
                                    ThongTinToChucThiModel.PhoChuKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chánh chủ khảo"))
                                {
                                    ThongTinToChucThiModel.ChanhChuKhao = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tổ trưởng hồi phách"))
                                {
                                    var ToTruongHoiPhach = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ToTruongHoiPhach = ToTruongHoiPhach;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Hiệu trưởng"))
                                {
                                    var HieuTruong = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.HieuTruong = HieuTruong;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng coi thi"))
                                {
                                    var CTHĐCT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDongCoiThi = CTHĐCT;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng:"))
                                {
                                    var CTHĐ = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDong = CTHĐ;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Cán bộ xét duyệt"))
                                {
                                    var CanBoXetDuyet = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.CanBoXetDuyet = CanBoXetDuyet;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("CTHĐ chấm thi"))
                                {
                                    var ChuTichHoiDongChamThi = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.ChuTichHoiDongChamThi = ChuTichHoiDongChamThi;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Địa danh"))
                                {
                                    var DiaDanh = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.DiaDanh = DiaDanh;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ghi chú"))
                                {
                                    var GhiChuCuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.GhiChuCuoiTrang = GhiChuCuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Từ SBD"))
                                {
                                    var SBDDau_CuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDDau_CuoiTrang = SBDDau_CuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Đến SBD"))
                                {
                                    var SBDDau_CuoiTrang = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.SBDCuoi_CuoiTrang = SBDDau_CuoiTrang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh đỗ thẳng"))
                                {
                                    var TSDoThang = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSDoThang = TSDoThang;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh đỗ thêm"))
                                {
                                    var TSDoThem = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSDoThem = TSDoThem;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thí sinh thi hỏng"))
                                {
                                    var TSThiHong = Utils.ConvertToNullableInt32(workSheet.Cells[i, j + 2].RichText.Text, null);
                                    ThongTinToChucThiModel.TSThiHong = TSThiHong;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Phó Giám đốc Sở Giáo dục"))
                                {
                                    var PGiamDoc = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.PGiamDoc = PGiamDoc;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người kiểm tra hồ sơ"))
                                {
                                    ThongTinToChucThiModel.FOOT_NGUOIKIEMTRAHS = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người kiểm tra"))
                                {
                                    var NguoiKiemTra = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    ThongTinToChucThiModel.NguoiKiemTra = NguoiKiemTra;
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ráp phách, đọc điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_RPDD = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Kiểm tra đọc"))
                                {
                                    ThongTinToChucThiModel.FOOT_KTD = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Dò trên bảng điểm in sẵn"))
                                {
                                    ThongTinToChucThiModel.FOOT_DTBDIS = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Thư ký"))
                                {
                                    ThongTinToChucThiModel.FOOT_THUKY = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Giám sát"))
                                {
                                    ThongTinToChucThiModel.FOOT_GIAMSAT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Được công nhận tốt nghiệp"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_DCNTN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Diện bq 4.5"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Dien45 = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Diện bq 4.75"))
                                {
                                    ThongTinToChucThiModel.FOOT_So_Dien475 = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm 1 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEM1DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm 1,5 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEM15DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm 2 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEM2DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Được cộng thêm trên 2 điểm"))
                                {
                                    ThongTinToChucThiModel.FOOT_CONGTHEMTREN2DIEM = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS vắng mặt khi thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_VANGMATKHITHI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS vi pham quy chế thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_VIPHAMQUYCHETHI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS thuộc diện ưu tiên"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSDIENUUTIEN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS có giấy chứng nhận nghề hợp lệ"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSCOCHUNGNHANNGHE = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Không được công nhận tốt nghiệp"))
                                {
                                    ThongTinToChucThiModel.FOOT_SKĐCNTN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Số thí sinh dự thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_SSTSDT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Số thí sinh dự thi"))
                                {
                                    ThongTinToChucThiModel.FOOT_STSDT = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tốt nghiệp diện D"))
                                {
                                    ThongTinToChucThiModel.FOOT_TND_D = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Tốt nghiệp diện E"))
                                {
                                    ThongTinToChucThiModel.FOOT_TND_E = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Sở: Loại Thường"))
                                {
                                    ThongTinToChucThiModel.FOOT_SLTHUONG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("Thường") || Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Equals("Thường:"))
                                {
                                    ThongTinToChucThiModel.FOOT_LTHUONG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS con liệt sĩ, dân tộc ít người"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSCONLIETSI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("HS các diện khác"))
                                {
                                    ThongTinToChucThiModel.FOOT_HSCACDIENKHAC = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Người lập bảng"))
                                {
                                    ThongTinToChucThiModel.FOOT_NGUOILAPBANG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ngày xác nhận lập bảng"))
                                {
                                    String FOOT_NXNLAPBANG = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    FOOT_NXNLAPBANG = FOOT_NXNLAPBANG.Replace("12:00:00 AM", "");
                                    ThongTinToChucThiModel.FOOT_NXNLAPBANG = FOOT_NXNLAPBANG;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ngày xác nhận chủ tịch hội đồng coi thi"))
                                {
                                    String FOOT_NXNHOIDONGCOITHI = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    FOOT_NXNHOIDONGCOITHI = FOOT_NXNHOIDONGCOITHI.Replace("12:00:00 AM", "");
                                    ThongTinToChucThiModel.FOOT_NXNHOIDONGCOITHI = FOOT_NXNHOIDONGCOITHI;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Ngày xác nhận chấm thi và xét tốt nghiệp"))
                                {
                                    String FOOT_NXNCHAMTHIXTN = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                    FOOT_NXNCHAMTHIXTN = FOOT_NXNCHAMTHIXTN.Replace("12:00:00 AM", "");
                                    ThongTinToChucThiModel.FOOT_NXNCHAMTHIXTN = FOOT_NXNCHAMTHIXTN;
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Vụ trưởng Vụ Giáo dục thường xuyên"))
                                {
                                    ThongTinToChucThiModel.FOOT_VTVGDTX = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }
                                else if (Utils.ConvertToString(workSheet.Cells[i, j].Value, string.Empty).Contains("Chủ tịch hội đồng phúc khảo"))
                                {
                                    ThongTinToChucThiModel.FOOT_CTHDPHUCKHAO = Utils.ConvertToString(workSheet.Cells[i, j + 2].Value, string.Empty);
                                }

                            }
                        }
                        #endregion

                        result.ThongTinToChucThi = ThongTinToChucThiModel;

                        if (result.ThongTinThiSinh != null && result.ThongTinThiSinh.Count > 0)
                        {
                            DataTable tableThiSinh = new DataTable();
                            tableThiSinh.Columns.Add("ThiSinhID_Str");
                            tableThiSinh.Columns.Add("KyThiID");
                            tableThiSinh.Columns.Add("HoTen");
                            tableThiSinh.Columns.Add("NgaySinh");
                            tableThiSinh.Columns.Add("NoiSinh");
                            tableThiSinh.Columns.Add("GioiTinh");
                            tableThiSinh.Columns.Add("DanToc");
                            tableThiSinh.Columns.Add("CMND");
                            tableThiSinh.Columns.Add("SoBaoDanh");
                            tableThiSinh.Columns.Add("SoDienThoai");
                            tableThiSinh.Columns.Add("DiaChi");
                            tableThiSinh.Columns.Add("Lop");
                            tableThiSinh.Columns.Add("TruongTHPT");
                            tableThiSinh.Columns.Add("LoaiDuThi");
                            tableThiSinh.Columns.Add("DonViDKDT");
                            tableThiSinh.Columns.Add("XepLoaiHanhKiem");
                            tableThiSinh.Columns.Add("XepLoaiHocLuc");
                            tableThiSinh.Columns.Add("DiemTBLop12");
                            tableThiSinh.Columns.Add("DiemKK");
                            tableThiSinh.Columns.Add("DienXTN");
                            tableThiSinh.Columns.Add("HoiDongThi");
                            tableThiSinh.Columns.Add("DiemXetTotNghiep");
                            tableThiSinh.Columns.Add("KetQuaTotNghiep");
                            tableThiSinh.Columns.Add("SoHieuBang");
                            tableThiSinh.Columns.Add("VaoSoCapBangSo");
                            tableThiSinh.Columns.Add("NamThi");
                            tableThiSinh.Columns.Add("Do");
                            tableThiSinh.Columns.Add("DoThem");
                            tableThiSinh.Columns.Add("Hong");
                            tableThiSinh.Columns.Add("LaoDong");
                            tableThiSinh.Columns.Add("VanHoa");
                            tableThiSinh.Columns.Add("RLTT");
                            tableThiSinh.Columns.Add("TongSoDiemThi");
                            tableThiSinh.Columns.Add("NgayCapBang");
                            tableThiSinh.Columns.Add("DiemXL");
                            tableThiSinh.Columns.Add("DiemUT");
                            tableThiSinh.Columns.Add("GhiChu");
                            tableThiSinh.Columns.Add("Hang");
                            tableThiSinh.Columns.Add("DiemTBCacBaiThi");
                            tableThiSinh.Columns.Add("DienUuTien");
                            tableThiSinh.Columns.Add("DiemTBC");

                            foreach (var item in result.ThongTinThiSinh)
                            {
                                var ThiSinhID_Str = Guid.NewGuid().ToString();
                                tableThiSinh.Rows.Add(ThiSinhID_Str,
                                                        item.KyThiID,
                                                        item.HoTen,
                                                        item.NgaySinh,
                                                        item.NoiSinh,
                                                        item.GioiTinh,
                                                        item.DanToc,
                                                        item.CMND,
                                                        item.SoBaoDanh,
                                                        item.SoDienThoai,
                                                        item.DiaChi,
                                                        item.Lop,
                                                        item.TruongTHPT,
                                                        item.LoaiDuThi,
                                                        item.DonViDKDT,
                                                        item.XepLoaiHanhKiem,
                                                        item.XepLoaiHocLuc,
                                                        item.DiemTBLop12,
                                                        item.DiemKK,
                                                        item.DienXTN,
                                                        item.HoiDongThi,
                                                        item.DiemXetTotNghiep,
                                                        item.KetQuaTotNghiep,
                                                        item.SoHieuBang,
                                                        item.VaoSoCapBangSo,
                                                        item.NamThi,
                                                        item.Do,
                                                        item.DoThem,
                                                        item.Hong,
                                                        item.LaoDong,
                                                        item.VanHoa,
                                                        item.RLTT,
                                                        item.TongSoDiemThi,
                                                        item.NgayCapBang,
                                                        item.DiemXL,
                                                        item.DiemUT,
                                                        item.GhiChu,
                                                        item.Hang,
                                                        item.DiemTBCacBaiThi,
                                                        item.DienUuTien,
                                                        item.DiemTBC
                                                        );
                            }

                            List<string> listThiSinhTrungCMNN = new List<string>();

                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter("TableThiSinh", SqlDbType.Structured),
                            };
                            parameters[0].Value = tableThiSinh;

                            try
                            {
                                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_SLTrungCMND_New", parameters))
                                {
                                    while (dr.Read())
                                    {
                                        var CMND = Utils.ConvertToString(dr["CMND"], String.Empty);
                                        listThiSinhTrungCMNN.Add(CMND);
                                        //var SLTrungCMND = Utils.ConvertToInt32(dr["SL"], 0);
                                        //if (SLTrungCMND > 0) Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";
                                    }
                                    dr.Close();
                                }

                            }
                            catch { }

                            if (listThiSinhTrungCMNN.Count > 0)
                            {
                                //foreach (var item in result.ThongTinThiSinh)
                                //{
                                //    for (int i = 0; i < listThiSinhTrungCMNN.Count; i++)
                                //    {
                                //        if (item.CMND != "" && item.CMND == listThiSinhTrungCMNN[i])
                                //        {
                                //            item.DanhSachLoi += "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                //            Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";
                                //        }
                                //    }
                                //}

                                for (int i = 0; i < result.ThongTinThiSinh.Count; i++)
                                {
                                    var item = result.ThongTinThiSinh[i];
                                    for (int j = 0; j < listThiSinhTrungCMNN.Count; j++)
                                    {
                                        if (item.CMND != "" && item.CMND == listThiSinhTrungCMNN[j])
                                        {
                                            item.DanhSachLoi += "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                            Mess = "Có thí sinh trùng CMND/CCCD với thí sinh trong hệ thống";

                                            var check = false;
                                            foreach (var err in result.ListErrorThiSinh)
                                            {
                                                if (err.Index == item.Index && err.MaCot == "BODY_CMND/CCCD")
                                                {
                                                    err.DanhSachLoi += "; Trùng CMND/CCCD với thí sinh trong hệ thống";
                                                    check = true;
                                                }
                                            }
                                            if (!check)
                                            {
                                                ErrorThongTinThiSinh loi = new ErrorThongTinThiSinh("BODY_CMND/CCCD", item.Index ?? 0, "Trùng CMND/CCCD với thí sinh trong hệ thống", true);
                                                result.ListErrorThiSinh.Add(loi);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Mess = ex.Message;
                //throw ex;
            }
            return result;
        }

    }
}
