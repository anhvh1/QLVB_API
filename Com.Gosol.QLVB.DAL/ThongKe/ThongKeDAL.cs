using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.QLVB.DAL.ThongKe
{
    public interface IThongKeDAL
    {
        public List<ChiTietDuLieuDiemThiModel> GetThongKe(BasePagingParams p, ref int TotalRow);
    }
    public class ThongKeDAL : IThongKeDAL
    {
        public List<ChiTietDuLieuDiemThiModel> GetThongKe(BasePagingParams p , ref int TotalRow)
        {
            var result = new List<ChiTietDuLieuDiemThiModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@TieuChi",SqlDbType.Int),
                //new SqlParameter("@TotalRow",SqlDbType.Int)
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
             };
            parameters[0].Value = p.Nam ?? DateTime.Now.Year;
            parameters[1].Value = p.TieuChiThongKe;
            //parameters[3].Direction = ParameterDirection.Output;
            //parameters[3].Size = 8;
            parameters[2].Value = p.Limit;
            parameters[3].Value = p.Offset;
            parameters[4].Direction = ParameterDirection.Output;
            parameters[4].Size = 8;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongKe_New", parameters))
                {
                    while (dr.Read())
                    {
                        ChiTietDuLieuDiemThiModel info = new ChiTietDuLieuDiemThiModel();
                        info.KyThiID = Utils.ConvertToInt32(dr["KyThiID"], 0);
                        info.TenKyThi = Utils.ConvertToString(dr["TenKyThi"], string.Empty);
                        info.HoiDongThiID = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.KhoaThiID = Utils.ConvertToInt32(dr["KhoaThiID"], 0);

                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.HoTen = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        info.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        info.NoiSinh = Utils.ConvertToString(dr["NoiSinh"], string.Empty);
                        info.GioiTinh = Utils.ConvertToNullableBoolean(dr["GioiTinh"], null);
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
                        //info.XepLoaiHanhKiemStr = Utils.ConvertToString(dr["XepLoaiHanhKiemStr"], string.Empty);
                        info.XepLoaiHocLuc = Utils.ConvertToInt32(dr["XepLoaiHocLuc"], 0);
                        //info.XepLoaiHocLucStr = Utils.ConvertToString(dr["XepLoaiHocLucStr"], string.Empty);
                        info.DiemTBLop12 = Utils.ConvertToDecimal(dr["DiemTBLop12"], 0);
                        info.DiemKK = Utils.ConvertToDecimal(dr["DiemKK"], 0);
                        info.DienXTN = Utils.ConvertToInt32(dr["DienXTN"], 0);
                        info.HoiDongThi = Utils.ConvertToInt32(dr["HoiDongThi"], 0);
                        info.DiemXetTotNghiep = Utils.ConvertToDecimal(dr["DiemXetTotNghiep"], 0);
                        info.KetQuaTotNghiep = Utils.ConvertToString(dr["KetQuaTotNghiep"], string.Empty);
                        info.SoHieuBang = Utils.ConvertToString(dr["SoHieuBang"], string.Empty);
                        info.VaoSoCapBangSo = Utils.ConvertToString(dr["VaoSoCapBangSo"], string.Empty);
                        info.NamThi = Utils.ConvertToInt32(dr["NamThi"], 0);

                        //info.DiemThiID = Utils.ConvertToInt32(dr["DiemThiID"], 0);
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        //info.MonThiID = Utils.ConvertToInt32(dr["MonThiID"], 0);
                        //info.Diem = Utils.ConvertToDecimal(dr["Diem"], 0);
                        result.Add(info);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[4].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
