using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Com.Gosol.QLVB.Models;

namespace Com.Gosol.QLVB.DAL.HeThong
{
    public interface IChucNangDAL
    {
        List<ChucNangModel> GetListChucNangByNguoiDungID(int NguoidungID);
        public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
    }
    public class ChucNangDAL : IChucNangDAL
    {
        public List<ChucNangModel> GetListChucNangByNguoiDungID(int NguoidungID)
        {
            List<ChucNangModel> Result = new List<ChucNangModel>();
            List<ChucNangModel> list = new List<ChucNangModel>();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(@"NguoiDungID", SqlDbType.Int),
            };
            parameters[0].Value = NguoidungID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_ChucNang_GetByNguoidungID", parameters))
                {

                    while (dr.Read())
                    {
                        ChucNangModel item = new ChucNangModel(Utils.ConvertToInt32(dr["ChucNangID"], 0)
                            , Utils.ConvertToString(dr["TenChucNang"], String.Empty)
                             , Utils.ConvertToString(dr["MaChucNang"], String.Empty)
                            , Utils.ConvertToInt32(dr["Quyen"], 0))
                            ;

                        list.Add(item);
                    }
                    dr.Close();
                    Result = (from m in list
                              group m by new { m.ChucNangID, m.TenChucNang, m.MaChucNang } into chucNang
                              select new ChucNangModel(chucNang.Key.ChucNangID, chucNang.Key.TenChucNang, chucNang.Key.MaChucNang, list.Where(x => x.ChucNangID == chucNang.Key.ChucNangID).ToList().Max(x => x.Quyen))
                              ).ToList();

                }
            }
            catch
            {
                throw;
            }
            return Result;
        }


        public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<ChucNangModel> list = new List<ChucNangModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),

            };
            parameters[0].Value = p.Keyword != null ? p.Keyword : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_ChucNang_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ChucNangModel item = new ChucNangModel();
                        item.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        item.TenChucNang = Utils.ConvertToString(dr["TenChucNang"], string.Empty);
                        item.ChucNangChaID = Utils.ConvertToInt32(dr["ChucNangChaID"], 0);
                        item.TenChucNangCha = Utils.ConvertToString(dr["TenChucNangCha"], string.Empty);
                        list.Add(item);
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

    }


}
