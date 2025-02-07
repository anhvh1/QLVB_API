using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface ITongHopBanKeKhaiDAL
    {
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow);
        public string ExportForwardFile(int FileDinhKemID);
        public List<BienDongTaiSanModelNew> BienDongTaiSan(int KeKhaiID, int CanBoID);
    }
    public class TongHopBanKeKhaiDAL : ITongHopBanKeKhaiDAL
    {
        //tên các store procedure
        private const string KE_KHAI_TONGHOPBANKEKHAI_GETPAGINGBYSEARCH = @"v1_KeKhai_TongHopBanKeKhai_GetPagingBySearch";

        //Ten các params
        private const string KE_KHAI_ID = "NV00301";
        private const string DOT_KE_KHAI_ID_KEKHAI = "NV00302";
        private const string CAN_BO_ID_KEKHAI = "NV00303";
        private const string NAM_KE_KHAI_KEKHAI = "NV00304";
        private const string TRANG_THAI = "NV00305";
        private const string TEN_BAN_KE_KHAI = "NV00306";
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow)
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 2;
            var CoQuanQuanLy = 0;
            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            if (UserRole.CheckAdmin(CanBoID) || listThanhTraTinh.Contains(CoQuanID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(0);
            }
            else if ((listThanhTraHuyen.Contains(CoQuanID)))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
            var listCanBoAll = new HeThongCanBoDAL().GetAllByListCoQuanID(listCoQuanCon.Select(x => x.CoQuanID).ToList());
            listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("CoQuanID",SqlDbType.Int),
                new SqlParameter(CAN_BO_ID_KEKHAI,SqlDbType.Int),               
                new SqlParameter(NAM_KE_KHAI_KEKHAI,SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                   pList,
              };
            parameters[8].Direction = ParameterDirection.Output;
            parameters[8].Size = 8;
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = p.CoQuanID ?? Convert.DBNull;
            parameters[6].Value = p.CanBoID ?? Convert.DBNull;
            parameters[7].Value = p.Nam ?? Convert.DBNull;
            parameters[9].Value = tbCanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KE_KHAI_TONGHOPBANKEKHAI_GETPAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID_KEKHAI], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID_KEKHAI], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI_KEKHAI], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.DanhSachFileDinhKem = new FileDinhKemDAL().GetListFileDinhKemByKeKhaiID(item.KeKhaiID).Where(x => x.TrangThai > 0).ToList();

                        list.Add(item);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[8].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public string ExportForwardFile(int FileDinhKemID)
        {
            try
            {
                var ServerPath = new FileDinhKemDAL().GetByID(FileDinhKemID).TenFileHeThong.ToString();
                Byte[] bytes = System.IO.File.ReadAllBytes(ServerPath);
                String file = Convert.ToBase64String(bytes);
                file = string.Concat("data:application/vnd.ms-excel;base64,", file);
                return file;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BienDongTaiSanModelNew> BienDongTaiSan(int KeKhaiID, int CanBoID)
        {
            return new List<BienDongTaiSanModelNew>();
            //    List<int> ListKeKhaiID = new List<int>();
            //    List<BienDongTaiSanModelNew> ListBienDongTaiSan = new List<BienDongTaiSanModelNew>();
            //    SqlParameter[] parameters = new SqlParameter[]
            //     {
            //        new SqlParameter(@"KeKhaiID",SqlDbType.Int),
            //         new SqlParameter(@CAN_BO_ID_KEKHAI,SqlDbType.Int)
            //     };
            //    parameters[0].Value = KeKhaiID;
            //    parameters[1].Value = CanBoID;
            //    try
            //    {
            //        //List<BienDongTaiSanModelNew> list = new List<BienDongTaiSanModelNew>();
            //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_KeKhai_BienDongTaiSan", parameters))
            //        {

            //            while (dr.Read())
            //            {

            //                ListKeKhaiID.Add(Utils.ConvertToInt32(dr["KeKhaiID"], 0));
            //            }
            //            dr.Close();
            //            if (ListKeKhaiID.Count < 2)
            //            {
            //                var listNhomTaiSan1 = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
            //                foreach (var item in listNhomTaiSan1)
            //                {                                                     
            //                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                        BienDongTaiSanModelNew.TenTaiSan = item.TenNhomTaiSan;
            //                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = "Chưa có";
            //                        BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);

            //                }
            //                return ListBienDongTaiSan;
            //            }
            //            var ThongTinKekhaiNow = new List<ThongTinTaiSanModelPartial>();
            //            for (int i = 0; i < ListKeKhaiID.Count; i++)
            //            {
            //                ThongTinKekhaiNow = new ThongTinTaiSanDAL().ThongTinTaiSan_GetByKeKhaiID(ListKeKhaiID[i]);
            //                var ThongTinKeKhaiBefore = new ThongTinTaiSanDAL().ThongTinTaiSan_GetByKeKhaiID(ListKeKhaiID[i + 1]);
            //                foreach (var a in ThongTinKekhaiNow)
            //                {
            //                    foreach (var j in ThongTinKeKhaiBefore)
            //                    {
            //                        switch (a.NhomTaiSanID.Value)
            //                        {
            //                            case 1:     // nhà ở
            //                                {

            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 11:     // cong trinh xay dung
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 12:     // dat 
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 13:     // cac loai dat khac
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 6:     // tien
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 3:     // phuong tien di chuyen
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 2:     // vang bac
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 8:     // tai san khac
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 10:     // Tai khoan nuoc ngoai
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 7:     // cac khoan no
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            case 9:     // Tong thiu nhap trong nam
            //                                {
            //                                    if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri > j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Up";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    else if (a.NhomTaiSanID == j.NhomTaiSanID && a.GiaTri < j.GiaTri)
            //                                    {
            //                                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        BienDongTaiSanModelNew.TrangThai = "Down";
            //                                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                    }
            //                                    else
            //                                    {
            //                                        //BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                                        //BienDongTaiSanModelNew.TenTaiSan = j.TenTaiSan;
            //                                        //BienDongTaiSanModelNew.GiaiTrinhNguonGoc = a.GiaiTrinhNguonGoc;
            //                                        //BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                                        //ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                                        break;
            //                                    }
            //                                    break;
            //                                }
            //                            default:
            //                                {
            //                                    return new List<BienDongTaiSanModelNew>();
            //                                }


            //                        }

            //                    }

            //                }
            //                break;
            //            }
            //            var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNTS();
            //            List<int> list = new List<int>();
            //            List<int> list1 = new List<int>();
            //            ThongTinKekhaiNow.ForEach(x => list.Add(x.NhomTaiSanID.Value));
            //            foreach (var item in listNhomTaiSan)
            //            {
            //                if (!list.Contains(item.NhomTaiSanID))
            //                {

            //                    BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                    BienDongTaiSanModelNew.TenTaiSan = item.TenNhomTaiSan;
            //                    BienDongTaiSanModelNew.GiaiTrinhNguonGoc = "Chưa có";
            //                    BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                    ListBienDongTaiSan.Add(BienDongTaiSanModelNew);
            //                    list1.Add(item.NhomTaiSanID);
            //                }
            //            }
            //            if(ListBienDongTaiSan.Count < 10)
            //            {
            //                foreach (var item in listNhomTaiSan)
            //                {
            //                    if (!list1.Contains(item.NhomTaiSanIDEquals))
            //                    {
            //                        var NhomTaiSan = new DanhMucNhomTaiSanDAL().GetNTSByID(item);
            //                        BienDongTaiSanModelNew BienDongTaiSanModelNew = new BienDongTaiSanModelNew();
            //                        BienDongTaiSanModelNew.TenTaiSan = NhomTaiSan.TenNhomTaiSan;
            //                        BienDongTaiSanModelNew.GiaiTrinhNguonGoc = "Chưa có";
            //                        BienDongTaiSanModelNew.TrangThai = "Chưa có";
            //                        ListBienDongTaiSan.Add(BienDongTaiSanModelNew);                            
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //    return ListBienDongTaiSan;
        }

    }
}
