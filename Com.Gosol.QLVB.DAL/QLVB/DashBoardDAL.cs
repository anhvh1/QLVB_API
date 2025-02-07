using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.QLVB
{
    public interface IDashBoardDAL
    {
        public SoLuong5NamGanDay SoLuong5NamGanDay();
        public List<SoLuongThiSinhDuThiVaDo> SoLuong10NamGanDay();
        public List<SoLuongThiSinhDuThiVaDo> SoLuongThiSinhDuThiVaDo(int TuNam, int DenNam);
        public List<SoLuongThiSinhDuThiVaDo> ThongKeSoLuongTotNghiepQuaCacNam(ThongKeSoLuongTotNghiepQuaCacNamParams p);
    }
    public class DashBoardDAL : IDashBoardDAL
    {
        public SoLuong5NamGanDay SoLuong5NamGanDay()
        {
            SoLuong5NamGanDay Result = new SoLuong5NamGanDay();
            Result.SoLieuChiTiet = new List<SoLieuThongKe>();
            List<SoLieuThongKe> SoLieuChiTiet = new List<SoLieuThongKe>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TuNam",SqlDbType.Int),
                new SqlParameter("@DenNam",SqlDbType.Int),
              };

            parameters[0].Value = DateTime.Now.Year - 4;
            parameters[1].Value = DateTime.Now.Year;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DashBoard_SoLuong5NamGanDay", parameters))
                {
                    while (dr.Read())
                    {
                        SoLieuThongKe info = new SoLieuThongKe();
                        info.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.TotNghiepLoaiGioi = Utils.ConvertToInt32(dr["TotNghiepLoaiGioi"], 0);
                        info.TotNghiepLoaiKha = Utils.ConvertToInt32(dr["TotNghiepLoaiKha"], 0);
                        info.TotNghiepTrungBinh = Utils.ConvertToInt32(dr["TotNghiepTrungBinh"], 0); 
                        int Tong = Utils.ConvertToInt32(dr["Tong"], 0);
                        info.Truot = Tong - info.TotNghiepLoaiGioi - info.TotNghiepLoaiKha - info.TotNghiepTrungBinh;
                        SoLieuChiTiet.Add(info);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if(SoLieuChiTiet.Count > 0)
            {
                Result.TongTotNghiepLoaiGioi = SoLieuChiTiet.Sum(x => x.TotNghiepLoaiGioi);
                Result.TongTotNghiepLoaiKha = SoLieuChiTiet.Sum(x => x.TotNghiepLoaiKha);
                Result.TongTotNghiepTrungBinh = SoLieuChiTiet.Sum(x => x.TotNghiepTrungBinh);
                Result.TongTruot = SoLieuChiTiet.Sum(x => x.Truot);
                Result.SoLieuChiTiet = SoLieuChiTiet;
            }
            
            return Result;
        }

        public List<SoLuongThiSinhDuThiVaDo> SoLuongThiSinhDuThiVaDo(int TuNam, int DenNam)
        {
            List<SoLuongThiSinhDuThiVaDo> Result = new List<SoLuongThiSinhDuThiVaDo>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TuNam",SqlDbType.Int),
                new SqlParameter("@DenNam",SqlDbType.Int),
              };

            parameters[0].Value = TuNam;
            parameters[1].Value = DenNam;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DashBoard_SoLuongThiSinhDuThiVaDo", parameters))
                {
                    while (dr.Read())
                    {
                        SoLuongThiSinhDuThiVaDo info = new SoLuongThiSinhDuThiVaDo();
                        info.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.ThiSinhDuThi = Utils.ConvertToInt32(dr["ThiSinhDuThi"], 0);
                        info.ThiSinhDo = Utils.ConvertToInt32(dr["ThiSinhDo"], 0);
                        Result.Add(info);
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

        public List<SoLuongThiSinhDuThiVaDo> SoLuong10NamGanDay()
        {
            List<SoLuongThiSinhDuThiVaDo> Result = new List<SoLuongThiSinhDuThiVaDo>();
   
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DashBoard_SoLuong10NamGanDay", null))
                {
                    while (dr.Read())
                    {
                        SoLuongThiSinhDuThiVaDo info = new SoLuongThiSinhDuThiVaDo();
                        info.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.ThiSinhDuThi = Utils.ConvertToInt32(dr["ThiSinhDuThi"], 0);
                        info.ThiSinhDo = Utils.ConvertToInt32(dr["ThiSinhDo"], 0);
                        info.ThiSinhDuocCapBang = Utils.ConvertToInt32(dr["ThiSinhDuocCapBang"], 0);
                        Result.Add(info);
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

        public List<SoLuongThiSinhDuThiVaDo> ThongKeSoLuongTotNghiepQuaCacNam(ThongKeSoLuongTotNghiepQuaCacNamParams p)
        {
            List<SoLuongThiSinhDuThiVaDo> Result = new List<SoLuongThiSinhDuThiVaDo>();
            SqlParameter[] parameters = new SqlParameter[]
            {
              new SqlParameter("@Nam",SqlDbType.Int),
            };
            parameters[0].Value = p.Nam ?? 0;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongKeSoLuongTotNghiepQuaCacNam", parameters))
                {
                    while (dr.Read())
                    {
                        SoLuongThiSinhDuThiVaDo info = new SoLuongThiSinhDuThiVaDo();
                        info.Nam = Utils.ConvertToInt32(dr["Nam"], 0);
                        info.ThiSinhDuThi = Utils.ConvertToInt32(dr["ThiSinhDuThi"], 0);
                        info.ThiSinhDo = Utils.ConvertToInt32(dr["ThiSinhDo"], 0);
                        info.ThiSinhDuocCapBang = Utils.ConvertToInt32(dr["ThiSinhDuocCapBang"], 0);
                        Result.Add(info);
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
