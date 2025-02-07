using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Linq;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.DAL.DanhMuc;

namespace Com.Gosol.QLVB.DAL.QLVB
{
    public interface ICapNhatPhuLucChinhSuaDAL
    {
        public List<NamTotNghiepTree> GetBySearchNamTotNghiep(CapNhatPhuLucParams p);
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow);
        public BaseResultModel Insert(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID);
        public BaseResultModel Update(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID);
        public BaseResultModel Delete(int CapNhatPhuLucID);
        public CapNhatPhuLucChinhSuaModel GetByID(int CapNhatPhuLucID);     
    }
    public class CapNhatPhuLucChinhSuaDAL : ICapNhatPhuLucChinhSuaDAL
    {
        public List<NamTotNghiepTree> GetBySearchNamTotNghiep(CapNhatPhuLucParams p)
        {
            List<NamTotNghiepTree> Result = new List<NamTotNghiepTree>();
            List<NamTotNghiepTree> Data = new List<NamTotNghiepTree>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Nam",SqlDbType.Int),
              };
        
            parameters[0].Value = p.Nam ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_CapNhatPhuLuc_GetByNam", parameters))
                {
                    while (dr.Read())
                    {
                        NamTotNghiepTree info = new NamTotNghiepTree();
                        info.CapNhatPhuLucID = Utils.ConvertToInt32(dr["CapNhatPhuLucID"], 0);
                        info.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.SoQuyetDinh = Utils.ConvertToString(dr["SoQuyetDinh"], string.Empty);
                        info.VeViec = Utils.ConvertToString(dr["VeViec"], string.Empty);
                        info.TongSoThiSinh = Utils.ConvertToInt32(dr["TongSoThiSinh"], 0);
                        Data.Add(info);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (Data.Count > 0)
            {
                Result = (from item1 in Data
                          group item1 by item1.Nam into temp1
                          select new NamTotNghiepTree()
                          {
                              Nam = temp1.Key,
                              Name = temp1.Key.ToString(),
                              TongSoThiSinh = Data.Where(x => x.Nam == temp1.Key).Sum(x => x.TongSoThiSinh),
                              children = (from item2 in temp1
                                          select new NamTotNghiepTree()
                                          {
                                              Nam = item2.Nam,
                                              CapNhatPhuLucID = item2.CapNhatPhuLucID,
                                              SoQuyetDinh = item2.SoQuyetDinh,
                                              VeViec = item2.VeViec,
                                              TongSoThiSinh = item2.TongSoThiSinh,
                                          }).ToList()
                          }).OrderByDescending(x => x.Nam).ToList();
            }

            if(Result.Count > 0)
            {
                DuLieuDiemThiParams diemThiParams = new DuLieuDiemThiParams();
                diemThiParams.NamID = p.Nam;
                var list = new DuLieuDiemThiDAL().GetBySearchNamThi(diemThiParams);
                if (list.Count > 0)
                {
                    var listNam = new List<int>();
                    listNam.AddRange(Result.Select(x => x.Nam ?? 0).ToList());
                    listNam.AddRange(list.Select(x => x.NamThi ?? 0).ToList());
                    listNam = listNam.Distinct().ToList();
                    List<NamTotNghiepTree> temp = new List<NamTotNghiepTree>();
                    foreach (var Nam in listNam)
                    {
                        var item = Result.Where(x => x.Nam == Nam).FirstOrDefault();
                        if (item != null)
                        {
                            temp.Add(item);
                        }
                        else
                        {
                            item = new NamTotNghiepTree();
                            item.Nam = Nam;
                            item.Name = Nam.ToString();
                            temp.Add(item);
                        }
                    }

                    Result = temp.OrderByDescending(x=>x.Nam).ToList();
                }
            }
            
            return Result;
        }
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow)
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@NamThi",SqlDbType.Int),
                new SqlParameter("@Truong",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = NamThi;
            parameters[7].Value = Truong;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToBoolean(dr["GioiTinh"], false);
                        info.DanToc = Utils.ConvertToInt32(dr["DanToc"], 0);
                        info.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        info.SoBaoDanh = Utils.ConvertToString(dr["SoBaoDanh"], string.Empty);
                        info.SoDienThoai = Utils.ConvertToString(dr["SoDienThoai"], string.Empty);
                        info.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        info.Lop = Utils.ConvertToString(dr["Lop"], string.Empty);
                        info.TruongTHPT = Utils.ConvertToInt32(dr["TruongTHPT"], 0);
                        info.TenTruongTHPT = Utils.ConvertToString(dr["TenTruongTHPT"], string.Empty);
                        info.LoaiDuThi = Utils.ConvertToString(dr["LoaiDuThi"], string.Empty);
                        info.DonViDKDT = Utils.ConvertToString(dr["DonViDKDT"], string.Empty);
                        info.XepLoaiHanhKiem = Utils.ConvertToInt32(dr["XepLoaiHanhKiem"], 0);
                        info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToInt32(dr["XepLoaiHocLuc"], 0);
                        info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToDecimal(dr["DiemTBLop12"], 0);
                        info.DiemKK = Utils.ConvertToDecimal(dr["DiemKK"], 0);
                        info.DienXTN = Utils.ConvertToInt32(dr["DienXTN"], 0);
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);
                        list.Add(info);
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
        public BaseResultModel Insert(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CapNhatPhuLucID", SqlDbType.Int),
                    new SqlParameter("Nam", SqlDbType.Int),
                    new SqlParameter("SoQuyetDinh", SqlDbType.NVarChar),
                    new SqlParameter("VeViec", SqlDbType.NVarChar),  
                };

                parameters[0].Direction = ParameterDirection.Output;
                parameters[0].Size = 8;
                parameters[1].Value = PhuLucChinhSua.Nam ?? Convert.DBNull;
                parameters[2].Value = PhuLucChinhSua.SoQuyetDinh ?? Convert.DBNull;
                parameters[3].Value = PhuLucChinhSua.VeViec ?? Convert.DBNull;
          
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_CapNhatPhuLucChinhSua_Insert", parameters);
                            int CapNhatPhuLucID = Utils.ConvertToInt32(parameters[0].Value, 0);
                            if (PhuLucChinhSua.ThongTinThiSinh != null && PhuLucChinhSua.ThongTinThiSinh.Count > 0)
                            {
                                foreach (var item in PhuLucChinhSua.ThongTinThiSinh)
                                {
                                    if(item.ThongTinChinhSua != null && item.ThongTinChinhSua.Count > 0)
                                    {
                                        foreach (var info in item.ThongTinChinhSua)
                                        {
                                            SqlParameter[] parms_tt = new SqlParameter[]
                                            {
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("Ma", SqlDbType.NVarChar),
                                                new SqlParameter("GiaTriHienTai", SqlDbType.NVarChar),
                                                new SqlParameter("GiaTriMoi", SqlDbType.NVarChar),
                                                new SqlParameter("LyDo", SqlDbType.NVarChar),
                                                new SqlParameter("CapNhatPhuLucID", SqlDbType.Int),
                                                new SqlParameter("NgaySua", SqlDbType.DateTime),
                                                new SqlParameter("NguoiSua", SqlDbType.Int),
                                            };
                                         
                                            parms_tt[0].Value = item.ThiSinhID;
                                            parms_tt[1].Value = info.Ma ?? Convert.DBNull;
                                            parms_tt[2].Value = info.GiaTriHienTai ?? Convert.DBNull;
                                            parms_tt[3].Value = info.GiaTriMoi ?? Convert.DBNull;
                                            parms_tt[4].Value = info.LyDo ?? Convert.DBNull;
                                            parms_tt[5].Value = CapNhatPhuLucID;
                                            parms_tt[6].Value = info.NgaySua ?? DateTime.Now;
                                            parms_tt[7].Value = info.NguoiSua ?? CanBoID;

                                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinhChinhSua_Insert_New", parms_tt);
                                        }
                                    }
                                }  
                            }
                            trans.Commit();
                            Result.Status = 1;
                            Result.Data = CapNhatPhuLucID;
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("phụ lục");
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

                //update du lieu thi sinh
                foreach (var item in PhuLucChinhSua.ThongTinThiSinh)
                {
                    UpdateThongTinThiSinh(item);
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
        public BaseResultModel Update(CapNhatPhuLucChinhSuaModel PhuLucChinhSua, int CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CapNhatPhuLucID", SqlDbType.Int),
                    new SqlParameter("Nam", SqlDbType.Int),
                    new SqlParameter("SoQuyetDinh", SqlDbType.NVarChar),
                    new SqlParameter("VeViec", SqlDbType.NVarChar),
                };

                parameters[0].Value = PhuLucChinhSua.CapNhatPhuLucID;
                parameters[1].Value = PhuLucChinhSua.Nam ?? Convert.DBNull;
                parameters[2].Value = PhuLucChinhSua.SoQuyetDinh ?? Convert.DBNull;
                parameters[3].Value = PhuLucChinhSua.VeViec ?? Convert.DBNull;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_CapNhatPhuLucChinhSua_Update", parameters);
                            //delete thong tinh chinh sua
                            SqlParameter[] parms_del = new SqlParameter[]
                            {
                                new SqlParameter("CapNhatPhuLucID", SqlDbType.Int),
                            };

                            parms_del[0].Value = PhuLucChinhSua.CapNhatPhuLucID;

                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinhChinhSua_Delete", parms_del);
                            //insert thong tin chinh sua
                            if (PhuLucChinhSua.ThongTinThiSinh != null && PhuLucChinhSua.ThongTinThiSinh.Count > 0)
                            {
                                foreach (var item in PhuLucChinhSua.ThongTinThiSinh)
                                {
                                    if (item.ThongTinChinhSua != null && item.ThongTinChinhSua.Count > 0)
                                    {
                                        foreach (var info in item.ThongTinChinhSua)
                                        {
                                            SqlParameter[] parms_tt = new SqlParameter[]
                                            {
                                                new SqlParameter("ThiSinhID", SqlDbType.Int),
                                                new SqlParameter("Ma", SqlDbType.NVarChar),
                                                new SqlParameter("GiaTriHienTai", SqlDbType.NVarChar),
                                                new SqlParameter("GiaTriMoi", SqlDbType.NVarChar),
                                                new SqlParameter("LyDo", SqlDbType.NVarChar),
                                                new SqlParameter("CapNhatPhuLucID", SqlDbType.Int),
                                                new SqlParameter("NgaySua", SqlDbType.DateTime),
                                                new SqlParameter("NguoiSua", SqlDbType.Int),
                                            };

                                            parms_tt[0].Value = item.ThiSinhID;
                                            parms_tt[1].Value = info.Ma ?? Convert.DBNull;
                                            parms_tt[2].Value = info.GiaTriHienTai ?? Convert.DBNull;
                                            parms_tt[3].Value = info.GiaTriMoi ?? Convert.DBNull;
                                            parms_tt[4].Value = info.LyDo ?? Convert.DBNull;
                                            parms_tt[5].Value = PhuLucChinhSua.CapNhatPhuLucID;
                                            parms_tt[6].Value = info.NgaySua ?? DateTime.Now;
                                            parms_tt[7].Value = info.NguoiSua ?? CanBoID;

                                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinhChinhSua_Insert_New", parms_tt);
                                        }
                                    }
                                }
                            }

                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Update_Success("phụ lục");
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
        public BaseResultModel Delete(int CapNhatPhuLucID)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                  new SqlParameter("CapNhatPhuLucID",SqlDbType.Int)
                };
                parameters[0].Value = CapNhatPhuLucID;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_CapNhatPhuLucChinhSua_Delete", parameters);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Delete_Success("phu lục");
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
        public CapNhatPhuLucChinhSuaModel GetByID(int CapNhatPhuLucID)
        {
            CapNhatPhuLucChinhSuaModel info = new CapNhatPhuLucChinhSuaModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("CapNhatPhuLucID",SqlDbType.Int)
            };
            parameters[0].Value = CapNhatPhuLucID;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_CapNhatPhuLucChinhSua_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        info.CapNhatPhuLucID = Utils.ConvertToInt32(dr["CapNhatPhuLucID"], 0);
                        info.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.SoQuyetDinh = Utils.ConvertToString(dr["SoQuyetDinh"], string.Empty);
                        info.VeViec = Utils.ConvertToString(dr["VeViec"], string.Empty);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            info.ThongTinThiSinh = GetThiSinhByCapNhatPhuLucID(info.CapNhatPhuLucID);

            return info;
        }
        public List<ThongTinThiSinh> GetThiSinhByCapNhatPhuLucID(int CapNhatPhuLucID)
        {
            List<ThongTinThiSinh> Result = new List<ThongTinThiSinh>();
            List<ThongTinChinhSua> Data = new List<ThongTinChinhSua>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@CapNhatPhuLucID",SqlDbType.Int)
            };
            parameters[0].Value = CapNhatPhuLucID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_GetByCapNhatPhuLucID", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinChinhSua info = new ThongTinChinhSua();                 
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0); 
                        info.CapNhatPhuLucID = Utils.ConvertToInt32(dr["CapNhatPhuLucID"], 0); 
                        info.Ma = Utils.ConvertToString(dr["Ma"], string.Empty);
                        info.TenThongTin = Utils.ConvertToString(dr["Ten"], string.Empty);
                        info.GiaTriHienTai = Utils.ConvertToString(dr["GiaTriHienTai"], string.Empty);
                        info.GiaTriMoi = Utils.ConvertToString(dr["GiaTriMoi"], string.Empty);     
                        info.LyDo = Utils.ConvertToString(dr["LyDo"], string.Empty);
                        info.TenNguoiSua = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        info.NguoiSua = Utils.ConvertToInt32(dr["NguoiSua"], 0);
                        info.NgaySua = Utils.ConvertToNullableDateTime(dr["NgaySua"], null);
                        Data.Add(info);             
                    }
                    dr.Close();
                }

                if(Data.Count > 0)
                {
                    var listThiSinh = (from item1 in Data
                              group item1 by item1.ThiSinhID into temp1
                              select new ThongTinThiSinh()
                              {
                                  ThiSinhID = temp1.Key ?? 0,
                                  ThongTinChinhSua = (from item2 in temp1
                                                      select new ThongTinChinhSua()
                                                      {
                                                          ThiSinhID = item2.ThiSinhID,
                                                          TenThongTin = item2.TenThongTin,
                                                          Ma = item2.Ma,
                                                          GiaTriHienTai = item2.GiaTriHienTai,
                                                          GiaTriMoi = item2.GiaTriMoi,
                                                          LyDo = item2.LyDo,
                                                          TenNguoiSua = item2.TenNguoiSua,
                                                          NguoiSua = item2.NguoiSua,
                                                          NgaySua = item2.NgaySua,
                                                      }).ToList()
                              }).ToList();

                    foreach (var item in listThiSinh)
                    {
                        var tmp = new QuanLyThiSinhDAL().GetThongTinThiSinh_New(item.ThiSinhID);
                        tmp.ThongTinChinhSua = item.ThongTinChinhSua;
                        Result.Add(tmp);
                    }
                }

                return Result;
            }
            catch
            {
                throw;
            }
        }
        public List<ThongTinDiemThi> GetDuLieuDiemThi(int ThiSinhID)
        {
            List<ThongTinDiemThi> Result = new List<ThongTinDiemThi>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@ThiSinhID",SqlDbType.Int)
            };
            parameters[0].Value = ThiSinhID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DuLieuDiemThi_GetByThiSinhID", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinDiemThi info = new ThongTinDiemThi();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.DiemThiID = Utils.ConvertToInt32(dr["DiemThiID"], 0);
                        info.NhomID = Utils.ConvertToInt32(dr["NhomID"], 0);
                        info.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                        info.Diem = Utils.ConvertToNullableDecimal(dr["Diem"], null);
                        info.DiemBaiToHop = Utils.ConvertToString(dr["DiemBaiToHop"], string.Empty);
                        info.TenMonThi = Utils.ConvertToString(dr["TenMonThi"], string.Empty);
                        info.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        Result.Add(info);
                    }
                    dr.Close();
                }

                return Result;
            }
            catch
            {
                throw;
            }
        }
        public BaseResultModel UpdateThongTinThiSinh(ThongTinThiSinh ThiSinh)
        {
            var Result = new BaseResultModel();

            var item = new QuanLyThiSinhDAL().GetThongTinThiSinh_New(ThiSinh.ThiSinhID);
            if(ThiSinh.ThongTinChinhSua != null && ThiSinh.ThongTinChinhSua.Count > 0)
            {
                foreach (var tt in ThiSinh.ThongTinChinhSua)
                {
                    if(tt.Ma == "BODY_HT")
                    {
                        item.HoTen = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_NgS")
                    {
                        item.NgaySinh = Utils.ConvertToNullableDateTime(tt.GiaTriMoi, null);
                        item.NgaySinhStr = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_NS")
                    {
                        item.NoiSinh = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_GT")
                    {
                        item.GioiTinh = Utils.ConvertToNullableBoolean(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_DT")
                    {
                        item.DanToc = Utils.ConvertToNullableInt32(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_CMND/CCCD")
                    {
                        item.CMND = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_SBD")
                    {
                        item.SoBaoDanh = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_SĐT")
                    {
                        item.SoDienThoai = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_ĐC")
                    {
                        item.DiaChi = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_L")
                    {
                        item.Lop = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_HST")
                    {
                        if (tt.GiaTriMoi != null && tt.GiaTriMoi.Length > 0)
                        {
                            var Truong = new DanhMucChungDAL().GetByName(item.TenTruongTHPT.Trim(), item.NamThi ?? 0);
                            if (Truong.ID > 0)
                            {
                                item.TruongTHPT = Truong.ID;
                            }
                            else
                            {
                                DanhMucChungModel DanhMucChungModel = new DanhMucChungModel();
                                DanhMucChungModel.Ten = item.TenTruongTHPT.Trim();
                                DanhMucChungModel.Loai = EnumLoaiDanhMuc.DM_Truong.GetHashCode();
                                DanhMucChungModel.Nam = item.NamThi ?? 0;
                                var hd = new DanhMucChungDAL().Insert(DanhMucChungModel);
                                item.TruongTHPT = Utils.ConvertToInt32(hd.Data, 0);
                            }
                        }
                    }
                    else if (tt.Ma == "BODY_LDT")
                    {
                        item.LoaiDuThi = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_ĐVĐKDT")
                    {
                        item.DonViDKDT = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_HK")
                    {
                        item.XepLoaiHanhKiem = Utils.ConvertToNullableInt32(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_HL")
                    {
                        item.XepLoaiHocLuc = Utils.ConvertToNullableInt32(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_ĐL12")
                    {
                        item.DiemTBLop12 = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_ĐKK")
                    {
                        item.DiemKK = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_DUT")
                    {
                        item.DienXTN = Utils.ConvertToNullableInt32(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "")
                    {
                        item.HoiDongThi = Utils.ConvertToNullableInt32(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_ĐXTN")
                    {
                        item.DiemXetTotNghiep = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_XLTN")
                    {
                        item.KetQuaTotNghiep = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_SHB")
                    {
                        item.SoHieuBang = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_VSCBS")
                    {
                        item.VaoSoCapBangSo = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_NCB")
                    {
                        item.NgayCapBang = Utils.ConvertToNullableDateTime(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "")
                    {
                        item.NamThi = Utils.ConvertToNullableInt32(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_Đô")
                    {
                        item.Do = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_ĐôT")
                    {
                        item.DoThem = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_H")
                    {
                        item.Hong = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_LĐ")
                    {
                        item.LaoDong = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_VH")
                    {
                        item.VanHoa = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_RLTT")
                    {
                        item.RLTT = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_TĐT")
                    {
                        item.TongSoDiemThi = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_ĐXL")
                    {
                        item.DiemXL = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_ĐƯT")
                    {
                        item.DiemUT = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_GC")
                    {
                        item.GhiChu = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_HANG")
                    {
                        item.Hang = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_ĐTB")
                    {
                        item.DiemTBCacBaiThi = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_DUT")
                    {
                        item.DienUuTien = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_ĐTBC")
                    {
                        item.DiemTBC = Utils.ConvertToNullableDecimal(tt.GiaTriMoi, null);
                    }
                    else if (tt.Ma == "BODY_QUEQUAN")
                    {
                        item.QueQuan = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_NGHE")
                    {
                        item.ChungNhanNghe = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DTCLS")
                    {
                        item.DTConLietSi = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_GTDKT")
                    {
                        item.GiaiTDKT = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_HOIDONG")
                    {
                        item.HoiDong = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_MONKN")
                    {
                        item.MonKN = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_TBCNKN")
                    {
                        item.TBCNMonKN = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DIEMTHICU")
                    {
                        item.DiemThiCu = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DIEMTHIMOI")
                    {
                        item.DiemThiMoi = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_TONGBQ")
                    {
                        item.TongBQ = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_BQA")
                    {
                        item.BQA = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_BQT")
                    {
                        item.BQT = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DC")
                    {
                        item.DC = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_BAN")
                    {
                        item.Ban = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DAODUC")
                    {
                        item.BODY_DAODUC = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_RLEV")
                    {
                        item.BODY_RLEV = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DIENKK")
                    {
                        item.BODY_DIENKK = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_PHONGTHI")
                    {
                        item.BODY_PHONGTHI = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_DIEMTNC")
                    {
                        item.BODY_DIEMTNC = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_XLTNC")
                    {
                        item.BODY_XLTNC = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_TDTCU")
                    {
                        item.BODY_TDTCU = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_GIAIHSG")
                    {
                        item.BODY_GIAIHSG = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_GIAIHSGK")
                    {
                        item.BODY_GIAIHSGK = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_CHUNGCHINN")
                    {
                        item.BODY_CHUNGCHINN = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_CHUNGCHITH")
                    {
                        item.BODY_CHUNGCHITH = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_TONGDIEMMOI")
                    {
                        item.BODY_TONGDIEMMOI = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_BQAMOI")
                    {
                        item.BODY_BQAMOI = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_BQTMOI")
                    {
                        item.BODY_BQTMOI = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_SOCAPGIAYCN")
                    {
                        item.BODY_SOCAPGIAYCN = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_XLHT")
                    {
                        item.BODY_XLHT = tt.GiaTriMoi;
                    }
                    else if (tt.Ma == "BODY_QUOCGIA")
                    {
                        item.BODY_QUOCGIA = tt.GiaTriMoi;
                    }
                }
            }

            try
            {
                SqlParameter[] parms_ts = new SqlParameter[]{
                      new SqlParameter("ThiSinhID", SqlDbType.Int),
                      new SqlParameter("KyThiID", SqlDbType.Int),
                      new SqlParameter("HoTen", SqlDbType.NVarChar),
                      new SqlParameter("NgaySinh", SqlDbType.DateTime),
                      new SqlParameter("NoiSinh", SqlDbType.NVarChar),
                      new SqlParameter("GioiTinh", SqlDbType.Bit),
                      new SqlParameter("DanToc", SqlDbType.Int),
                      new SqlParameter("CMND", SqlDbType.NVarChar),
                      new SqlParameter("SoBaoDanh", SqlDbType.NVarChar),
                      new SqlParameter("SoDienThoai", SqlDbType.NVarChar),
                      new SqlParameter("DiaChi", SqlDbType.NVarChar),
                      new SqlParameter("Lop", SqlDbType.NVarChar),
                      new SqlParameter("TruongTHPT", SqlDbType.Int),
                      new SqlParameter("LoaiDuThi", SqlDbType.NVarChar),
                      new SqlParameter("DonViDKDT", SqlDbType.NVarChar),
                      new SqlParameter("XepLoaiHanhKiem", SqlDbType.Int),
                      new SqlParameter("XepLoaiHocLuc", SqlDbType.Int),
                      new SqlParameter("DiemTBLop12", SqlDbType.Decimal),
                      new SqlParameter("DiemKK", SqlDbType.Decimal),
                      new SqlParameter("DienXTN", SqlDbType.Int),
                      new SqlParameter("HoiDongThi", SqlDbType.Int),
                      new SqlParameter("DiemXetTotNghiep", SqlDbType.Decimal),
                      new SqlParameter("KetQuaTotNghiep", SqlDbType.NVarChar),
                      new SqlParameter("SoHieuBang", SqlDbType.NVarChar),
                      new SqlParameter("VaoSoCapBangSo", SqlDbType.NVarChar),
                      new SqlParameter("NamThi", SqlDbType.Int),
                      new SqlParameter("Do", SqlDbType.NVarChar),
                      new SqlParameter("DoThem", SqlDbType.NVarChar),
                      new SqlParameter("Hong", SqlDbType.NVarChar),
                      new SqlParameter("LaoDong", SqlDbType.NVarChar),
                      new SqlParameter("VanHoa", SqlDbType.NVarChar),
                      new SqlParameter("RLTT", SqlDbType.NVarChar),
                      new SqlParameter("TongSoDiemThi", SqlDbType.Decimal),
                      new SqlParameter("NgayCapBang", SqlDbType.DateTime),
                      new SqlParameter("DiemXL", SqlDbType.Decimal),
                      new SqlParameter("DiemUT", SqlDbType.Decimal),
                      new SqlParameter("GhiChu", SqlDbType.NVarChar),
                      new SqlParameter("Hang", SqlDbType.NVarChar),
                      new SqlParameter("DiemTBCacBaiThi", SqlDbType.Decimal),
                      new SqlParameter("DienUuTien", SqlDbType.NVarChar),
                      new SqlParameter("DiemTBC", SqlDbType.Decimal),
                      new SqlParameter("QueQuan", SqlDbType.NVarChar),
                      new SqlParameter("ChungNhanNghe", SqlDbType.NVarChar),
                      new SqlParameter("DTConLietSi", SqlDbType.NVarChar),
                      new SqlParameter("GiaiTDKT", SqlDbType.NVarChar),
                      new SqlParameter("HoiDong", SqlDbType.NVarChar),
                      new SqlParameter("MonKN", SqlDbType.NVarChar),
                      new SqlParameter("TBCNMonKN", SqlDbType.NVarChar),
                      new SqlParameter("DiemThiCu", SqlDbType.NVarChar),
                      new SqlParameter("DiemThiMoi", SqlDbType.NVarChar),
                      new SqlParameter("TongBQ", SqlDbType.NVarChar),
                      new SqlParameter("BQA", SqlDbType.NVarChar),
                      new SqlParameter("BQT", SqlDbType.NVarChar),
                      new SqlParameter("DC", SqlDbType.NVarChar),
                      new SqlParameter("Ban", SqlDbType.NVarChar),
                      new SqlParameter("NgaySinhStr", SqlDbType.NVarChar),

                      new SqlParameter("BODY_DAODUC", SqlDbType.NVarChar),
                      new SqlParameter("BODY_RLEV", SqlDbType.NVarChar),
                      new SqlParameter("BODY_DIENKK", SqlDbType.NVarChar),
                      new SqlParameter("BODY_PHONGTHI", SqlDbType.NVarChar),
                      new SqlParameter("BODY_DIEMTNC", SqlDbType.NVarChar),
                      new SqlParameter("BODY_XLTNC", SqlDbType.NVarChar),
                      new SqlParameter("BODY_TDTCU", SqlDbType.NVarChar),
                      new SqlParameter("BODY_GIAIHSG", SqlDbType.NVarChar),
                      new SqlParameter("BODY_GIAIHSGK", SqlDbType.NVarChar),
                      new SqlParameter("BODY_CHUNGCHINN", SqlDbType.NVarChar),
                      new SqlParameter("BODY_CHUNGCHITH", SqlDbType.NVarChar),
                      new SqlParameter("BODY_TONGDIEMMOI", SqlDbType.NVarChar),
                      new SqlParameter("BODY_BQAMOI", SqlDbType.NVarChar),
                      new SqlParameter("BODY_BQTMOI", SqlDbType.NVarChar),
                      new SqlParameter("BODY_SOCAPGIAYCN", SqlDbType.NVarChar),
                      new SqlParameter("BODY_XLHT", SqlDbType.NVarChar),
                      new SqlParameter("BODY_QUOCGIA", SqlDbType.NVarChar),
                };

                parms_ts[0].Value = item.ThiSinhID;
                parms_ts[1].Value = item.KyThiID;
                parms_ts[2].Value = item.HoTen ?? Convert.DBNull;
                parms_ts[3].Value = item.NgaySinh ?? Convert.DBNull;
                parms_ts[4].Value = item.NoiSinh ?? Convert.DBNull;
                parms_ts[5].Value = item.GioiTinh ?? Convert.DBNull;
                parms_ts[6].Value = item.DanToc ?? Convert.DBNull;
                parms_ts[7].Value = item.CMND ?? Convert.DBNull;
                parms_ts[8].Value = item.SoBaoDanh ?? Convert.DBNull;
                parms_ts[9].Value = item.SoDienThoai ?? Convert.DBNull;
                parms_ts[10].Value = item.DiaChi ?? Convert.DBNull;
                parms_ts[11].Value = item.Lop ?? Convert.DBNull;
                parms_ts[12].Value = item.TruongTHPT ?? Convert.DBNull;
                parms_ts[13].Value = item.LoaiDuThi ?? Convert.DBNull;
                parms_ts[14].Value = item.DonViDKDT ?? Convert.DBNull;
                parms_ts[15].Value = item.XepLoaiHanhKiem ?? Convert.DBNull;
                parms_ts[16].Value = item.XepLoaiHocLuc ?? Convert.DBNull;
                parms_ts[17].Value = item.DiemTBLop12 ?? Convert.DBNull;
                parms_ts[18].Value = item.DiemKK ?? Convert.DBNull;
                parms_ts[19].Value = item.DienXTN ?? Convert.DBNull;
                parms_ts[20].Value = item.HoiDongThi ?? Convert.DBNull;
                parms_ts[21].Value = item.DiemXetTotNghiep ?? Convert.DBNull;
                parms_ts[22].Value = item.KetQuaTotNghiep ?? Convert.DBNull;
                parms_ts[23].Value = item.SoHieuBang ?? Convert.DBNull;
                parms_ts[24].Value = item.VaoSoCapBangSo ?? Convert.DBNull;
                parms_ts[25].Value = item.NamThi ?? Convert.DBNull;
                parms_ts[26].Value = item.Do ?? Convert.DBNull;
                parms_ts[27].Value = item.DoThem ?? Convert.DBNull;
                parms_ts[28].Value = item.Hong ?? Convert.DBNull;
                parms_ts[29].Value = item.LaoDong ?? Convert.DBNull;
                parms_ts[30].Value = item.VanHoa ?? Convert.DBNull;
                parms_ts[31].Value = item.RLTT ?? Convert.DBNull;
                parms_ts[32].Value = item.TongSoDiemThi ?? Convert.DBNull;
                parms_ts[33].Value = item.NgayCapBang ?? Convert.DBNull;
                parms_ts[34].Value = item.DiemXL ?? Convert.DBNull;
                parms_ts[35].Value = item.DiemUT ?? Convert.DBNull;
                parms_ts[36].Value = item.GhiChu ?? Convert.DBNull;
                parms_ts[37].Value = item.Hang ?? Convert.DBNull;
                parms_ts[38].Value = item.DiemTBCacBaiThi ?? Convert.DBNull;
                parms_ts[39].Value = item.DienUuTien ?? Convert.DBNull;
                parms_ts[40].Value = item.DiemTBC ?? Convert.DBNull;
                parms_ts[41].Value = item.QueQuan ?? Convert.DBNull;
                parms_ts[42].Value = item.ChungNhanNghe ?? Convert.DBNull;
                parms_ts[43].Value = item.DTConLietSi ?? Convert.DBNull;
                parms_ts[44].Value = item.GiaiTDKT ?? Convert.DBNull;
                parms_ts[45].Value = item.HoiDong ?? Convert.DBNull;
                parms_ts[46].Value = item.MonKN ?? Convert.DBNull;
                parms_ts[47].Value = item.TBCNMonKN ?? Convert.DBNull;
                parms_ts[48].Value = item.DiemThiCu ?? Convert.DBNull;
                parms_ts[49].Value = item.DiemThiMoi ?? Convert.DBNull;
                parms_ts[50].Value = item.TongBQ ?? Convert.DBNull;
                parms_ts[51].Value = item.BQA ?? Convert.DBNull;
                parms_ts[52].Value = item.BQT ?? Convert.DBNull;
                parms_ts[53].Value = item.DC ?? Convert.DBNull;
                parms_ts[54].Value = item.Ban ?? Convert.DBNull;
                parms_ts[55].Value = item.NgaySinhStr ?? Convert.DBNull;

                parms_ts[56].Value = item.BODY_DAODUC ?? Convert.DBNull;
                parms_ts[57].Value = item.BODY_RLEV ?? Convert.DBNull;
                parms_ts[58].Value = item.BODY_DIENKK ?? Convert.DBNull;
                parms_ts[59].Value = item.BODY_PHONGTHI ?? Convert.DBNull;
                parms_ts[60].Value = item.BODY_DIEMTNC ?? Convert.DBNull;
                parms_ts[61].Value = item.BODY_XLTNC ?? Convert.DBNull;
                parms_ts[62].Value = item.BODY_TDTCU ?? Convert.DBNull;
                parms_ts[63].Value = item.BODY_GIAIHSG ?? Convert.DBNull;
                parms_ts[64].Value = item.BODY_GIAIHSGK ?? Convert.DBNull;
                parms_ts[65].Value = item.BODY_CHUNGCHINN ?? Convert.DBNull;
                parms_ts[66].Value = item.BODY_CHUNGCHITH ?? Convert.DBNull;
                parms_ts[67].Value = item.BODY_TONGDIEMMOI ?? Convert.DBNull;
                parms_ts[68].Value = item.BODY_BQAMOI ?? Convert.DBNull;
                parms_ts[69].Value = item.BODY_BQTMOI ?? Convert.DBNull;
                parms_ts[70].Value = item.BODY_SOCAPGIAYCN ?? Convert.DBNull;
                parms_ts[71].Value = item.BODY_XLHT ?? Convert.DBNull;
                parms_ts[72].Value = item.BODY_QUOCGIA ?? Convert.DBNull;

               

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_ThongTinThiSinh_Update", parms_ts);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = ConstantLogMessage.Alert_Update_Success("thi sinh");
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
    }
}
