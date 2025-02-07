using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.EFCore;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.BaoCao;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.BaoCao
{
    public interface IThongKeBanKeKhaiDAL
    {
        public List<ThongKeBanKeKhaiModel> ThongKeBanKeKhai(ThongKeParams p, int CanBoID, int CoQuanID, int NguoiDungID);
        public List<DotKeKhaiModel> ListDotKeKhai(int? CanBoID, int? CoQuanIDCuaCanBoDangNhap, int? CoQuanFilter, int? NamKeKhai);
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_ID, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan);
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSan(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_ID, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhai);
        public List<HeThongCanBoShortModel> GetCanBoByCoQuanAndLoaiKeKhai(int? CoQuanID, int? NamKeKhai, int? CapQuanLy, int? LoaiKeKhai, int CoQuan_ID, int NguoiDungID);
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeByCoQuanID(int? CoQuanID, int? CapID, int? CapQuanLy, int? CoQuanIDFilter, int? NamKeKhai, int? CoQuanCuaCanBoDangNhap, int NguoiDungID);
        public List<NhacViecModel> Dasboard_Notification(int? NguoiDungID, int? CanBoID, int? CoQuanID);
        public ThongKeGuiThanhTraTinhModel ThongKeGuiThanhTraTinh(int CanBoID, int CoQuanID, int NguoiDungID);
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart_New(int? CoQuanID_Filter, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan);
        public List<HeThongCanBoShortModel> GetCanBoByCoQuanAndLoaiKeKhai_New(int? CoQuanID, int? NamKeKhai, int? CapQuanLy, int? LoaiKeKhai, int CoQuan_CuaCanBoDangNhap, int NguoiDungID);
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart_New_v2(int? Type, int? CoQuanID_Filter, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan);
    }
    public class ThongKeBanKeKhaiDAL : IThongKeBanKeKhaiDAL
    {
        //tên các store procedure
        private const string SELECT_CANBO_BY_COQUAN_AND_LOAIKEKHAI = @"v1_SelectCanBoByCoQuanAndLoaiKeKhai";
        private const string THONG_KE_CHI_TIET_TAI_SAN = @"v1_ThongKeChiTietKeKhaiTaiSan";
        private const string BAO_CAO_THONG_KE_BAN_KE_KHAI = @"v1_BaoCao_ThongKeBanKeKhai";

        //Ten các params
        private const string CO_QUAN_ID = "NV00203";
        private const string DOT_KE_KHAI_ID = "NV00101";
        private const string NAM_KE_KHAI_DOTKEKHAI = "NV00105";

        public List<DotKeKhaiModel> ListDotKeKhai(int? CanBoID, int? CoQuanIDCuaCanBoDangNhap, int? CoQuanFilter, int? NamKeKhai)
        {
            List<DotKeKhaiModel> ListDotKeKhai = new List<DotKeKhaiModel>();
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            //var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanIDCuaCanBoDangNhap);
            //if (p.CoQuanID != null && p.CoQuanID > 0)
            //{
            //    tbCanBoID.Rows.Add(p.CoQuanID);
            //}
            //var CapQuanLy = 2;
            //var CoQuanQuanLy = 0;
            //if (UserRole.CheckAdmin(CanBoID.Value))
            //{
            //    CapQuanLy = 0;
            //    CoQuanQuanLy = 0;

            //}
            //else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
            //{

            //}
            //else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
            //{
            //    CapQuanLy = 0;
            //    CoQuanQuanLy = 0;
            //    //var listCanBo = listCanBoAll.Where(x => x.CapQuanLy == EnumCapQuanLyCanBo.CapTinh.GetHashCode()).ToList();
            //    //listCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //    //tbCanBoID.Rows.Add(CoQuanID);
            //}
            //else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
            //{
            //    CapQuanLy = 0;
            //    CoQuanQuanLy = 0;
            //    //var listCanBo = listCanBoAll.Where(x => x.CapQuanLy == EnumCapQuanLyCanBo.CapTinh.GetHashCode()).ToList();
            //    //listCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //}
            //else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
            //{
            //    CapQuanLy = 0;
            //    CoQuanQuanLy = crCoQuan.CoQuanID;
            //    //var listCanBo = new DanhMucCoQuanDonViDAL()
            //    //listCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));

            //}
            //else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
            //{
            //    CapQuanLy = 0;
            //    CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
            //    //var listCanBo = listCanBoAll.Where(x => x.CapQuanLy == EnumCapQuanLyCanBo.CapHuyen.GetHashCode()).ToList();
            //    //listCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CoQuanID));
            //}
            //else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
            //{
            //    CapQuanLy = 0;
            //    CoQuanQuanLy = crCoQuan.CoQuanID;
            //    //tbCanBoID.Rows.Add(CoQuanID);

            //}

            //var listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy);
            //if (CoQuanFilter != null && CoQuanFilter > 0)
            //{
            //    listCanBoAll = new HeThongCanBoDAL().GetListByCoQuanID(CoQuanFilter.Value);
            //}


            var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanIDCuaCanBoDangNhap.Value);

            if (UserRole.CheckAdmin(CanBoID.Value))
            {
                listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0);

            }
            var listCanBo = new HeThongCanBoDAL().GetAllByListCoQuanID(listCoQuan.Select(x => x.CoQuanID).ToList());
            ListDotKeKhai = new DotKeKhaiDAL().GetList_ByListCanBoID(listCanBo.Select(x => x.CanBoID).ToList(), NamKeKhai.Value);
            //ListDotKeKhai = new DotKeKhaiDAL().GetList_ByListCanBoID(listCanBoAll.Select(x => x.CanBoID).ToList(), NamKeKhai.Value);
            //List<int> List = new List<int>();
            //foreach (var item in listCanBoAll)
            //{
            //    var ListDotKeKhaiID = new DotKeKhaiDAL().GetDotKeKhaiByCanBoID(item.CanBoID);
            //    List.AddRange(ListDotKeKhaiID.Where(x => !List.Contains(x.DotKeKhaiID)).Select(y => y.DotKeKhaiID));
            //}
            //foreach (var item in List)
            //{

            //    var DotKeKhaiByID = new DotKeKhaiDAL().GetBy_ID(item);
            //    var TenDotKeKhai = CommonDAL.getTenLoaiDotKeKhai(DotKeKhaiByID.LoaiDotKeKhai).ToString();
            //    DotKeKhaiByID.TenDotKeKhai = TenDotKeKhai;
            //    ListDotKeKhai.Add(DotKeKhaiByID);
            //}
            return ListDotKeKhai;

        }

        public List<ThongKeBanKeKhaiModel> ThongKeBanKeKhai(ThongKeParams p, int CanBoID, int CoQuanID, int NguoiDungID)
        {
            List<ThongKeBanKeKhaiModel> Result = new List<ThongKeBanKeKhaiModel>();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var listCoQuanTrucThuoc = new DanhMucCoQuanDonViDAL().GetListByUser_Phang(CoQuanID, NguoiDungID);
            var listCanBoAll = new HeThongCanBoDAL().GetAllByListCoQuanID(listCoQuanTrucThuoc.Select(x => x.CoQuanID).ToList());
            listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(NAM_KE_KHAI_DOTKEKHAI,SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int),
                new SqlParameter(DOT_KE_KHAI_ID,SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
                pList

              };
            //param TrangThai là 0: chưa gửi hoặc 1: đã gửi
            parameters[0].Value = p.NamKeKhai ?? Convert.DBNull;
            parameters[1].Value = p.CoQuanID ?? Convert.DBNull;
            parameters[2].Value = p.DotKekhaiID ?? Convert.DBNull;
            parameters[3].Value = p.TrangThai ?? Convert.DBNull;
            parameters[4].Value = tbCanBoID;
            try
            {
                var list = new List<ThongKeBanKeKhaiPartialModel>();
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, BAO_CAO_THONG_KE_BAN_KE_KHAI, parameters))
                {
                    while (dr.Read())
                    {
                        ThongKeBanKeKhaiPartialModel item = new ThongKeBanKeKhaiPartialModel();
                        item.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        item.DenNgay = Utils.ConvertToDateTime(dr["NV00103"], DateTime.Now);
                        item.DiaChi = Utils.ConvertToString(dr["DiaChi"], "");
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr["NV00101"], 0);
                        item.HoKhau = Utils.ConvertToString(dr["HoKhau"], "");
                        item.NamKeKhai = Utils.ConvertToInt32(dr["NV00105"], 0);
                        item.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        item.MaCoQuan = Utils.ConvertToString(dr["MaCQ"], "");
                        item.MaCanBo = Utils.ConvertToString(dr["MaCB"], "");
                        item.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        item.TuNgay = Utils.ConvertToDateTime(dr["NV00102"], DateTime.Now);
                        var listChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(item.CanBoID);
                        item.DanhSachChucVuID = listChucVu.Select(x => x.ChucVuID).ToList();
                        item.DanhSachTenChucVu = listChucVu.Select(x => x.TenChucVu).ToList();
                        list.Add(item);
                    }
                    dr.Close();
                }
                Result = (from m in list
                          group m by new { m.CoQuanID, m.TenCoQuan } into coquan
                          select new ThongKeBanKeKhaiModel()
                          {
                              CoQuanID = coquan.Key.CoQuanID,
                              TenCoQuan = coquan.Key.TenCoQuan,
                          }
                          ).ToList();
                Result.ForEach(x => x.DanhSachBanKeKhai = list.Where(n => n.CoQuanID == x.CoQuanID).ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        // DashBoard gọi chung 
        //public List<ThongKeChiTietKeKhaiTaiSanPar> DashBoard_Common(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap)
        // Dashboard biểu đồ
        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan)
        {
            ThongKeTaiSanModel ThongKeTaiSanModel = new ThongKeTaiSanModel();
            //    ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSanPar = new ThongKeChiTietKeKhaiTaiSanPar();
            List<ThongKeChiTietKeKhaiTaiSanPar> ListThongKeChiTietKeKhaiTaiSanPar = new List<ThongKeChiTietKeKhaiTaiSanPar>();
            //List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan = new List<ThongKeChiTietKeKhaiTaiSan>();
            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            var ThongKeChiTiet = ThongKeChiTietKeKhaiTaiSan(CoQuanID, CapQuanLy, NamKeKhai, NguoiDungID, CoQuan_CuaCanBoDangNhap, ref ListThongKeChiTietKeKhaiTaiSan);
            var CapTinh = ThongKeChiTiet.ListThongKeCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).ToList().FirstOrDefault();
            var CapSo = ThongKeChiTiet.ListThongKeCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapSo.GetHashCode()).ToList().FirstOrDefault();
            var CapHuyen = ThongKeChiTiet.ListThongKeCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).ToList().FirstOrDefault();
            var CapPhong = ThongKeChiTiet.ListThongKeCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapPhong.GetHashCode()).ToList().FirstOrDefault();
            var CapXa = ThongKeChiTiet.ListThongKeCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList().FirstOrDefault();
            var DaKeKhaiCapTinh = CapTinh.KeKhaiBoNhiemDaKeKhai + CapTinh.KeKhaiBoSungDaKeKhai + CapTinh.KeKhaiHangNamDaKeKhai + CapTinh.KeKhaiLanDauDaKeKhai + CapSo.KeKhaiLanDauDaKeKhai
                  + CapSo.KeKhaiBoNhiemDaKeKhai + CapSo.KeKhaiHangNamDaKeKhai + CapSo.KeKhaiBoSungDaKeKhai;
            var ChuaKeKhaiCapTinh = CapTinh.KeKhaiBoNhiemChuaKeKhai + CapTinh.KeKhaiBoSungChuaKeKhai + CapTinh.KeKhaiHangNamChuaKeKhai + CapTinh.KeKhaiLanDauChuaKeKhai + CapSo.KeKhaiBoNhiemChuaKeKhai
          + CapSo.KeKhaiBoSungChuaKeKhai + CapSo.KeKhaiHangNamChuaKeKhai + CapSo.KeKhaiLanDauChuaKeKhai;
            var DaKeKhaiCapHuyen = CapHuyen.KeKhaiBoNhiemDaKeKhai + CapHuyen.KeKhaiBoSungDaKeKhai + CapHuyen.KeKhaiHangNamDaKeKhai + CapHuyen.KeKhaiLanDauDaKeKhai;
            var ChuaKeKhaiCapHuyen = CapHuyen.KeKhaiBoNhiemChuaKeKhai + CapHuyen.KeKhaiBoSungChuaKeKhai + CapHuyen.KeKhaiHangNamChuaKeKhai + CapHuyen.KeKhaiLanDauChuaKeKhai;
            var DaKeKhaiCapXa = CapXa.KeKhaiBoNhiemDaKeKhai + CapXa.KeKhaiBoSungDaKeKhai + CapXa.KeKhaiHangNamDaKeKhai + CapXa.KeKhaiLanDauDaKeKhai;
            var ChuaKeKhaiCapXa = CapXa.KeKhaiBoNhiemChuaKeKhai + CapXa.KeKhaiBoSungChuaKeKhai + CapXa.KeKhaiHangNamChuaKeKhai + CapXa.KeKhaiLanDauChuaKeKhai;

            try
            {
                foreach (var item in ListThongKeChiTietKeKhaiTaiSan)
                {
                    var ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                    //ThongKeChiTietKeKhaiTaiSan ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                    ThongKeChiTietKeKhaiTaiSan.CoQuanID = item.CoQuanID;
                    ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = item.CoQuanChaID;
                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = item.TenCoQuan;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai = item.KeKhaiHangNamDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai = item.KeKhaiBoSungDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = item.KeKhaiBoNhiemDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = item.KeKhaiBoNhiemDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai = item.KeKhaiBoSungChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai = item.KeKhaiBoNhiemChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai = item.KeKhaiHangNamChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai = item.KeKhaiLanDauChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai = item.KeKhaiLanDauDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.TongSoChuaKeKhai = (item.KeKhaiBoSungChuaKeKhai + item.KeKhaiBoNhiemChuaKeKhai + item.KeKhaiHangNamChuaKeKhai + item.KeKhaiLanDauChuaKeKhai);
                    ThongKeChiTietKeKhaiTaiSan.TongSoDaKeKhai = (item.KeKhaiHangNamDaKeKhai + item.KeKhaiBoSungDaKeKhai + item.KeKhaiBoNhiemDaKeKhai + item.KeKhaiLanDauDaKeKhai);
                    ThongKeChiTietKeKhaiTaiSan.CapID = item.CapID;
                    ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).Count;
                    if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapTinh.GetHashCode()) // Cấp tỉnh
                    {
                        var CoQuanCapTinh = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault(); // kiểm tra cơ quan cấp tỉnh
                        if (CoQuanCapTinh == null || string.IsNullOrEmpty(CoQuanCapTinh.TenCapCoQuan))
                        {
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            CoQuanCapTinh = new ThongKeChiTietKeKhaiTaiSanPar();
                            //ThongKeChiTietKeKhaiTaiSanPar_New.TenCoQuan = ThongKeChiTietKeKhaiTaiSan.TenCoQuan;
                            CoQuanCapTinh.TenCapCoQuan = "Cấp tỉnh";
                            CoQuanCapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                            CoQuanCapTinh.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                            CoQuanCapTinh.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            CoQuanCapTinh.TongSoChuaKeKhai = ChuaKeKhaiCapTinh;
                            CoQuanCapTinh.TongSoDaKeKhai = DaKeKhaiCapTinh;
                            ListThongKeChiTietKeKhaiTaiSanPar.Add(CoQuanCapTinh);
                        }
                        else
                        {   // check cơ quan đã tồn tại chưa
                            var CheckCoQuan = CoQuanCapTinh.ListThongKeChiTiet.Where(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                            if (CheckCoQuan == null || CheckCoQuan.CoQuanID < 1)
                            {
                                //ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = ThongKeChiTietKeKhaiTaiSan.TenCoQuan;
                                if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                                {
                                    string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                                }
                                ThongKeChiTietKeKhaiTaiSan.CoQuanID = ThongKeChiTietKeKhaiTaiSan.CoQuanID;
                                ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = ThongKeChiTietKeKhaiTaiSan.CoQuanChaID;
                                ThongKeChiTietKeKhaiTaiSan.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                                ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault().ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                            else
                            {
                                CheckCoQuan.TongSoChuaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                                CheckCoQuan.TongSoDaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                                //ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault().TongSoChuaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                                //ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault().TongSoDaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                                var ListDanhSachTheoCap = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault().ListThongKeChiTiet;
                                ListDanhSachTheoCap.RemoveAll(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID);
                                ListDanhSachTheoCap.Add(CheckCoQuan);
                            }
                        }
                    }
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) // cấp sở
                    {
                        var CoQuanCapTinh = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault(); // Kiểm tra trong danh sách có cơ quan cấp tỉnh nào chưa
                        var ListCoQuanCha = new DanhMucCoQuanDonViDAL().GetAllByCapCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        var CoQuanCapChaPhuHop = ListCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).ToList().FirstOrDefault();
                        if (CoQuanCapChaPhuHop == null || CoQuanCapChaPhuHop.CoQuanID <= 0)
                        {
                            CoQuanCapChaPhuHop = new DanhMucCoQuanDonViDAL().GetByID1(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        }
                        if (CoQuanCapTinh == null || string.IsNullOrEmpty(CoQuanCapTinh.TenCapCoQuan)) // chưa có
                        {
                            CoQuanCapTinh = new ThongKeChiTietKeKhaiTaiSanPar();
                            //ThongKeChiTietKeKhaiTaiSanPar_New.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(ThongKeChiTietKeKhaiTaiSan.CoQuanChaID).TenCoQuan.ToString();
                            CoQuanCapTinh.TenCapCoQuan = "Cấp tỉnh";
                            CoQuanCapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            ThongKeChiTietKeKhaiTaiSan.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                            CoQuanCapTinh.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                            CoQuanCapTinh.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            CoQuanCapTinh.TongSoChuaKeKhai = ChuaKeKhaiCapTinh;
                            CoQuanCapTinh.TongSoDaKeKhai = DaKeKhaiCapTinh;
                            ListThongKeChiTietKeKhaiTaiSanPar.Add(CoQuanCapTinh);
                        }
                        else
                        {
                            var CoQuanCha = CoQuanCapTinh.ListThongKeChiTiet.Where(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault(); // kiểm tra có cơ quan cha của nó tồn tại chưa
                            if (CoQuanCha == null || CoQuanCha.CoQuanID < 1)
                            {
                                //ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                                if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                                {
                                    string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                                }
                                ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                                ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                                ThongKeChiTietKeKhaiTaiSan.CapID = CoQuanCapChaPhuHop.CapID;
                                ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault().ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                            else
                            {
                                CoQuanCha.TongSoChuaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                                CoQuanCha.TongSoDaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                                var ListDanhSachTheoCap = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).FirstOrDefault().ListThongKeChiTiet;
                                ListDanhSachTheoCap.RemoveAll(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == CoQuanCapChaPhuHop.CapID);
                                ListDanhSachTheoCap.Add(CoQuanCha);
                            }
                        }
                    }
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()) // Cấp huyện
                    {
                        var CoQuanCapHuyen = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).FirstOrDefault();
                        if (CoQuanCapHuyen == null || string.IsNullOrEmpty(CoQuanCapHuyen.TenCapCoQuan))
                        {
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            CoQuanCapHuyen = new ThongKeChiTietKeKhaiTaiSanPar();
                            //ThongKeChiTietKeKhaiTaiSanPar_New.TenCoQuan = ThongKeChiTietKeKhaiTaiSan.TenCoQuan;
                            CoQuanCapHuyen.TenCapCoQuan = "Cấp huyện";
                            CoQuanCapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                            CoQuanCapHuyen.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                            CoQuanCapHuyen.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            CoQuanCapHuyen.TongSoChuaKeKhai = ChuaKeKhaiCapHuyen;
                            CoQuanCapHuyen.TongSoDaKeKhai = DaKeKhaiCapHuyen;
                            ListThongKeChiTietKeKhaiTaiSanPar.Add(CoQuanCapHuyen);
                        }
                        else
                        {
                            // check nó đã có trong danh sách chưa
                            var CoQuan = CoQuanCapHuyen.ListThongKeChiTiet.Where(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                            if (CoQuan == null || CoQuan.CoQuanID < 1)
                            {
                                //ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = ThongKeChiTietKeKhaiTaiSan.TenCoQuan;
                                if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                                {
                                    string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                                }
                                ThongKeChiTietKeKhaiTaiSan.CoQuanID = ThongKeChiTietKeKhaiTaiSan.CoQuanID;
                                ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = ThongKeChiTietKeKhaiTaiSan.CoQuanChaID;
                                ThongKeChiTietKeKhaiTaiSan.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                                ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).FirstOrDefault().ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                            else
                            {
                                CoQuan.TongSoChuaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                                CoQuan.TongSoDaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                                //Lấy danh sách cơ quan theo cấp cơ quan .Xóa và update lại
                                var ListDanhSachTheoCap = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault().ListThongKeChiTiet;
                                ListDanhSachTheoCap.RemoveAll(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID);
                                ListDanhSachTheoCap.Add(CoQuan);
                            }
                        }
                    }
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) // Cấp phòng
                    {
                        var ListCoQuanCha = new DanhMucCoQuanDonViDAL().GetAllByCapCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        var CoQuanCapChaPhuHop = ListCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).ToList().FirstOrDefault();
                        if (CoQuanCapChaPhuHop == null || CoQuanCapChaPhuHop.CoQuanID <= 0)
                        {
                            CoQuanCapChaPhuHop = new DanhMucCoQuanDonViDAL().GetByID1(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        }
                        // Kiểm tra cơ quan cấp huyện đã tồn tại chưa
                        var CoQuanCapHuyen = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == (CoQuanCapChaPhuHop.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ? EnumCapCoQuan.CapHuyen.GetHashCode() : EnumCapCoQuan.CapTinh.GetHashCode())).FirstOrDefault();
                        if (CoQuanCapHuyen == null || string.IsNullOrEmpty(CoQuanCapHuyen.TenCapCoQuan))
                        {
                            CoQuanCapHuyen = new ThongKeChiTietKeKhaiTaiSanPar();
                            //ThongKeChiTietKeKhaiTaiSanPar_New.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(ThongKeChiTietKeKhaiTaiSan.CoQuanChaID).TenCoQuan.ToString();
                            CoQuanCapHuyen.TenCapCoQuan = CoQuanCapChaPhuHop.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ? "Cấp huyện" : "Cấp tỉnh";
                            CoQuanCapHuyen.CapID = CoQuanCapChaPhuHop.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ? EnumCapCoQuan.CapHuyen.GetHashCode() : EnumCapCoQuan.CapTinh.GetHashCode();
                            CoQuanCapHuyen.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            ThongKeChiTietKeKhaiTaiSan.CapID = CoQuanCapChaPhuHop.CapID;
                            CoQuanCapHuyen.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            CoQuanCapHuyen.TongSoChuaKeKhai = ChuaKeKhaiCapHuyen;
                            CoQuanCapHuyen.TongSoDaKeKhai = DaKeKhaiCapHuyen;
                            ListThongKeChiTietKeKhaiTaiSanPar.Add(CoQuanCapHuyen);
                        }
                        else
                        {
                            // check cơ quan cha đã tồn tại chưa
                            var CoQuanCapCha = CoQuanCapHuyen.ListThongKeChiTiet.Where(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == CoQuanCapChaPhuHop.CapID).FirstOrDefault();
                            if (CoQuanCapCha == null || CoQuanCapCha.CoQuanID < 1)
                            {
                                //ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                                if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                                {
                                    string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                                }
                                ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                                ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                                ThongKeChiTietKeKhaiTaiSan.CapID = CoQuanCapChaPhuHop.CapID;
                                ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == (CoQuanCapChaPhuHop.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ? EnumCapCoQuan.CapHuyen.GetHashCode() : EnumCapCoQuan.CapTinh.GetHashCode())).FirstOrDefault().ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                            else
                            {
                                CoQuanCapCha.TongSoChuaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                                CoQuanCapCha.TongSoDaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                                var ListDanhsachCoQuanTheoCap = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == (CoQuanCapChaPhuHop.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ? EnumCapCoQuan.CapHuyen.GetHashCode() : EnumCapCoQuan.CapTinh.GetHashCode())).FirstOrDefault().ListThongKeChiTiet;
                                ListDanhsachCoQuanTheoCap.RemoveAll(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == CoQuanCapChaPhuHop.CapID);
                                ListDanhsachCoQuanTheoCap.Add(CoQuanCapCha);
                            }
                        }
                    }
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapXa.GetHashCode()) // Cấp xã
                    {
                        var CoQuanCapXa = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.TenCapCoQuan == "Cấp xã").FirstOrDefault();
                        var ListCoQuanCha = new DanhMucCoQuanDonViDAL().GetAllByCapCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        var CoQuanCapChaPhuHop = ListCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).ToList().FirstOrDefault();
                        if (CoQuanCapChaPhuHop == null || CoQuanCapChaPhuHop.CoQuanID <= 0)
                        {
                            CoQuanCapChaPhuHop = new DanhMucCoQuanDonViDAL().GetByID1(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        }
                        if (CoQuanCapXa == null || string.IsNullOrEmpty(CoQuanCapXa.TenCapCoQuan))
                        {
                            CoQuanCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                            //ThongKeChiTietKeKhaiTaiSanPar_New.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(ThongKeChiTietKeKhaiTaiSan.CoQuanChaID).TenCoQuan.ToString();
                            CoQuanCapXa.TenCapCoQuan = "Cấp xã";
                            CoQuanCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            ThongKeChiTietKeKhaiTaiSan.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                            CoQuanCapXa.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                            CoQuanCapXa.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            CoQuanCapXa.TongSoChuaKeKhai = ChuaKeKhaiCapXa;
                            CoQuanCapXa.TongSoDaKeKhai = DaKeKhaiCapXa;
                            ListThongKeChiTietKeKhaiTaiSanPar.Add(CoQuanCapXa);
                        }
                        else
                        {
                            // check cơ quan cha tồn tại chưa
                            var CoQuanCha = CoQuanCapXa.ListThongKeChiTiet.Where(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                            if (CoQuanCha == null || CoQuanCha.CoQuanID < 1)
                            {
                                //ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                                if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                                {
                                    string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                                }
                                ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                                ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                                ThongKeChiTietKeKhaiTaiSan.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                                ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).FirstOrDefault().ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                            else
                            {
                                CoQuanCha.TongSoChuaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                                CoQuanCha.TongSoDaKeKhai += (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                                var ListDanhSachTheoCap = ListThongKeChiTietKeKhaiTaiSanPar.Where(x => x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault().ListThongKeChiTiet;
                                ListDanhSachTheoCap.RemoveAll(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID);
                                ListDanhSachTheoCap.Add(CoQuanCha);

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            //List<int> List = new List<int>();
            //listCoQuanCon.ForEach(x => List.Add(x.CoQuanID));
            //list.Where(x => List.Contains(x.CoQuanID.Value)).ToList();
            ThongKeTaiSanModel.ThongKeTaiSan_Chart = ListThongKeChiTietKeKhaiTaiSanPar;
            ThongKeTaiSanModel.ThongKeTaiSan_Table = ThongKeChiTiet;
            return ThongKeTaiSanModel;
        }

        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart_New(int? CoQuanID_Filter, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan)
        {
            ThongKeTaiSanModel ThongKeTaiSanModel = new ThongKeTaiSanModel();
            try
            {
                int CapID = 0;
                var crCoQuan = new DanhMucCoQuanDonViModel();
                crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_CuaCanBoDangNhap);
                CapID = crCoQuan.CapID.Value;
                ThongKeTaiSanModel.ThongKeTaiSan_Chart = new List<ThongKeChiTietKeKhaiTaiSanPar>();
                ThongKeTaiSanModel.ThongKeTaiSan_Table = new ThongKeChiTietKeKhaiTaiSanPar();
                ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha = new List<ThongKeChiTietKeKhaiTaiSan>();
                List<int> DanhSachCapID = new List<int>();
                if (CapID == EnumCapCoQuan.CapTinh.GetHashCode() || CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {
                    DanhSachCapID.Add(EnumCapCoQuan.CapTinh.GetHashCode());
                    DanhSachCapID.Add(EnumCapCoQuan.CapSo.GetHashCode());
                    DanhSachCapID.Add(EnumCapCoQuan.CapHuyen.GetHashCode());
                    DanhSachCapID.Add(EnumCapCoQuan.CapPhong.GetHashCode());
                    DanhSachCapID.Add(EnumCapCoQuan.CapXa.GetHashCode());
                }
                else if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                {
                    DanhSachCapID.Add(EnumCapCoQuan.CapHuyen.GetHashCode());
                    DanhSachCapID.Add(EnumCapCoQuan.CapPhong.GetHashCode());
                    DanhSachCapID.Add(EnumCapCoQuan.CapXa.GetHashCode());
                }
                else if (CapID == EnumCapCoQuan.CapPhong.GetHashCode())
                {

                }
                else if (CapID == EnumCapCoQuan.CapXa.GetHashCode())
                {
                    DanhSachCapID.Add(EnumCapCoQuan.CapXa.GetHashCode());
                }
                var pList = new SqlParameter("@DanhSachCapID", SqlDbType.Structured);
                pList.TypeName = "dbo.list_ID";
                var tbCapID = new DataTable();
                tbCapID.Columns.Add("CapID", typeof(string));
                DanhSachCapID.ForEach(x => tbCapID.Rows.Add(x));
                SqlParameter[] parameters = new SqlParameter[]
               {
                new SqlParameter("CoQuanID",SqlDbType.Int),
                new SqlParameter("Nam",SqlDbType.Int),
                  pList
               };
                parameters[0].Value = CoQuanID_Filter ?? Convert.DBNull;
                parameters[1].Value = NamKeKhai ?? Convert.DBNull;
                parameters[2].Value = tbCapID;
                var listThongKeChiTietFromDB = new List<ThongKeChiTietKeKhaiTaiSan>();
                try
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_ThongKeDashboard", parameters))
                    {
                        while (dr.Read())
                        {
                            ThongKeChiTietKeKhaiTaiSan ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                            ThongKeChiTietKeKhaiTaiSan.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            ThongKeChiTietKeKhaiTaiSan.SLChuaKeKhai = Utils.ConvertToInt32(dr["SLChuaKeKhai"], 0);
                            ThongKeChiTietKeKhaiTaiSan.LoaiDotKeKhai = Utils.ConvertToInt32(dr["LoaiDotKeKhai"], 0);
                            ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = Utils.ConvertToInt32(dr["TongSoDotKeKhai"], 0);
                            listThongKeChiTietFromDB.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (CapID == EnumCapCoQuan.CapTinh.GetHashCode() || CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {
                    #region CấpTỉnh
                    //Cap tinh
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapTinh = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapTinh.TenCapCoQuan = "Cấp Tỉnh";
                    thongkeChiTietCapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                    var DanhSachCoQuanIDCapSo = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapSo.GetHashCode()).Select(x => x.CoQuanID.Value);
                    var DanhSachThongkeCapTinh = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || x.CapID == EnumCapCoQuan.CapSo.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapSo.Contains(x.CoQuanChaID.Value))).ToList();
                    thongkeChiTietCapTinh.TongSoDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0).Count();
                    thongkeChiTietCapTinh.TongSoChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1).Count();
                    thongkeChiTietCapTinh.ListThongKeChiTiet = (from item in DanhSachThongkeCapTinh
                                                                where item.CapID == EnumCapCoQuan.CapTinh.GetHashCode()
                                                                group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                                                                select new ThongKeChiTietKeKhaiTaiSan()
                                                                {
                                                                    CoQuanID = gr.Key.CoQuanID,
                                                                    CapID = gr.Key.CapID,
                                                                    CoQuanChaID = gr.Key.CoQuanChaID,
                                                                    TenCoQuan = gr.Key.TenCoQuan.Remove(0, 4).Trim(),
                                                                    TongSoChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 /*&& x.CoQuanID == gr.Key.CoQuanID*/).Count(),
                                                                    TongSoDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 /*&& x.CoQuanID == gr.Key.CoQuanID*/).Count(),
                                                                }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapTinh);

                    //thêm dữ liệu vào thốg kê bảng
                    (from item in thongkeChiTietCapTinh.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         TongSoDotKeKhai = item.TongSoDotKeKhai,
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count(),
                         Children = (from item2 in DanhSachThongkeCapTinh
                                     where item2.CapID == EnumCapCoQuan.CapTinh.GetHashCode()
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         TongSoDotKeKhai = gr2.Key.TongSoDotKeKhai,
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CapID = gr2.Key.CapID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()

                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    ThongKeChiTietKeKhaiTaiSan CapTinh = new ThongKeChiTietKeKhaiTaiSan();
                    CapTinh.TenCoQuan = "UBND Cấp Tỉnh";
                    CapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                    var DanhSachCoQuanIDCapTinh = DanhSachThongkeCapTinh.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Select(x => x.CoQuanID.Value);
                    //CapTinh.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapTinh.ToList(), NamKeKhai.Value).ToList().Count();
                    CapTinh.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    CapTinh.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapTinh);
                    #endregion

                    #region CâpSở
                    //Thêm dữ liệu thống kê bảng cấp sở
                    var DanhSachThongkeCapSo = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapSo.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapSo.Contains(x.CoQuanChaID.Value))).ToList();
                    (from item in DanhSachThongkeCapSo
                     where (item.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                     group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.CoQuanChaID, item.TongSoDotKeKhai } into gr
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                         TongSoDotKeKhai = gr.Key.TongSoDotKeKhai,
                         CoQuanID = gr.Key.CoQuanID,
                         CapID = gr.Key.CapID,
                         TenCoQuan = gr.Key.TenCoQuan,
                         CoQuanChaID = gr.Key.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Count(),
                         Children = (from item2 in DanhSachThongkeCapSo
                                     where /*item2.CapID == EnumCapCoQuan.CapSo.GetHashCode() ||*/ (item2.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && gr.Key.CoQuanID == item2.CoQuanChaID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         TongSoDotKeKhai = gr2.Key.TongSoDotKeKhai,
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    ThongKeChiTietKeKhaiTaiSan CapSo = new ThongKeChiTietKeKhaiTaiSan();
                    CapSo.TenCoQuan = "Cấp Sở, Ngành";
                    CapSo.CapID = EnumCapCoQuan.CapSo.GetHashCode();
                    //CapSo.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapSo.ToList(), NamKeKhai.Value).ToList().Count();
                    CapSo.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapSo.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapSo.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapSo.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapSo.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapSo.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapSo.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    CapSo.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapSo.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapSo);
                    ///
                    #endregion

                    #region CapHuyện
                    //cap huyen
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapHuyen = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapHuyen.TenCapCoQuan = "Cấp Huyện";
                    thongkeChiTietCapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    var DanhSachCoQuanIDCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).Select(x => x.CoQuanID.Value);
                    var DanhSachThongkeCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapHuyen.Contains(x.CoQuanChaID.Value))).ToList();
                    thongkeChiTietCapHuyen.TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Count(x => x.SLChuaKeKhai == 1);
                    thongkeChiTietCapHuyen.TongSoDaKeKhai = DanhSachThongkeCapHuyen.Count(x => x.SLChuaKeKhai == 0);
                    thongkeChiTietCapHuyen.ListThongKeChiTiet = (from item in DanhSachThongkeCapHuyen
                                                                 where item.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (item.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && item.CoQuanChaID == item.CoQuanID)
                                                                 group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID, item.TongSoDotKeKhai } into gr
                                                                 select new ThongKeChiTietKeKhaiTaiSan()
                                                                 {
                                                                     TongSoDotKeKhai = gr.Key.TongSoDotKeKhai,
                                                                     CoQuanID = gr.Key.CoQuanID,
                                                                     TenCoQuan = gr.Key.TenCoQuan.Remove(0, 4).Trim(),
                                                                     CapID = gr.Key.CapID,
                                                                     CoQuanChaID = gr.Key.CoQuanChaID,
                                                                     TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),
                                                                     TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),

                                                                 }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapHuyen);

                    //thêm dữ liệu vào thốg kê bảng cấp  huyện
                    (from item in thongkeChiTietCapHuyen.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { item.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                         TongSoDotKeKhai = item.TongSoDotKeKhai,
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == item.CoQuanID /*|| item.CoQuanID == x.CoQuanChaID)*/).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         Children = (from item2 in DanhSachThongkeCapHuyen
                                     where /*item2.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ||*/ (item2.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && item2.CoQuanChaID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.TongSoChuaKeKhai, item2.TongSoDaKeKhai, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         TongSoDotKeKhai = gr2.Key.TongSoDotKeKhai,
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    ThongKeChiTietKeKhaiTaiSan CapHuyen = new ThongKeChiTietKeKhaiTaiSan();
                    CapHuyen.TenCoQuan = "Cấp UBND Huyện";
                    CapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    //CapHuyen.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapHuyen.ToList(), NamKeKhai.Value).ToList().Count();
                    CapHuyen.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapHuyen.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapHuyen.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    CapHuyen.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapHuyen);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                    #region CấpXã
                    //cap xa
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapXa.TenCapCoQuan = "Cấp Xã";
                    thongkeChiTietCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    var DanhSachThongkeCapXa = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                    thongkeChiTietCapXa.TongSoChuaKeKhai = DanhSachThongkeCapXa.Count(x => x.SLChuaKeKhai == 1);
                    thongkeChiTietCapXa.TongSoDaKeKhai = DanhSachThongkeCapXa.Count(x => x.SLChuaKeKhai == 0);
                    thongkeChiTietCapXa.ListThongKeChiTiet = (from item in DanhSachThongkeCapXa
                                                                  //where item.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                                                              group item by new { item.CoQuanChaID/*, item.CoQuanID, item.TenCoQuan,item.TongSoChuaKeKhai,item.TongSoDaKeKhai */} into gr
                                                              select new ThongKeChiTietKeKhaiTaiSan()
                                                              {
                                                                  CoQuanID = new DanhMucCoQuanDonViDAL().GetByID(gr.Key.CoQuanChaID).CoQuanID,
                                                                  TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(gr.Key.CoQuanChaID).TenCoQuan.Remove(0, 4).Trim(),
                                                                  //CoQuanID = DanhSachThongkeCapHuyen.Where(x=>x.CoQuanID==gr.Key.CoQuanChaID).FirstOrDefault().CoQuanID,
                                                                  //TenCoQuan = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr.Key.CoQuanChaID).FirstOrDefault().TenCoQuan,
                                                                  CapID = EnumCapCoQuan.CapXa.GetHashCode(),
                                                                  TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr.Key.CoQuanChaID).Count(),
                                                                  TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr.Key.CoQuanChaID).Count()
                                                              }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapXa);

                    //thêm dữ liệu vào thốg kê bảng cấp xã
                    (from item in thongkeChiTietCapXa.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(item.CoQuanID.Value, NamKeKhai.Value).ToList().Count(),
                         TongSoDotKeKhai = item.TongSoDotKeKhai,
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         Children = (from item2 in DanhSachThongkeCapXa
                                     where item2.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         TongSoDotKeKhai = gr2.Key.TongSoDotKeKhai,
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    var DanhSachCoQuanIDCapXa = DanhSachThongkeCapXa.Select(x => x.CoQuanID.Value);
                    ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
                    CapXa.TenCoQuan = "Cấp UBND Xã";
                    CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    //CapXa.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapXa.ToList(), NamKeKhai.Value).ToList().Count();
                    CapXa.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapXa.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapXa.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapXa.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapXa.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapXa.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapXa.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    CapXa.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapXa);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                }
                else if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                {
                    #region CapHuyện
                    //cap huyen
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapHuyen = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapHuyen.TenCapCoQuan = "Cấp Huyện";
                    thongkeChiTietCapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    var DanhSachCoQuanIDCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).Select(x => x.CoQuanID.Value);
                    var DanhSachThongkeCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapHuyen.Contains(x.CoQuanChaID.Value))).ToList();
                    thongkeChiTietCapHuyen.TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Count(x => x.SLChuaKeKhai == 1);
                    thongkeChiTietCapHuyen.TongSoDaKeKhai = DanhSachThongkeCapHuyen.Count(x => x.SLChuaKeKhai == 0);
                    thongkeChiTietCapHuyen.ListThongKeChiTiet = (from item in DanhSachThongkeCapHuyen
                                                                 where item.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (item.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && item.CoQuanChaID == item.CoQuanID)
                                                                 group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                                                                 select new ThongKeChiTietKeKhaiTaiSan()
                                                                 {
                                                                     CoQuanID = gr.Key.CoQuanID,
                                                                     TenCoQuan = gr.Key.TenCoQuan.Remove(0, 4).Trim(),
                                                                     CapID = gr.Key.CapID,
                                                                     CoQuanChaID = gr.Key.CoQuanChaID,
                                                                     TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),
                                                                     TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),

                                                                 }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapHuyen);

                    //thêm dữ liệu vào thốg kê bảng cấp  huyện
                    (from item in thongkeChiTietCapHuyen.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { item.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == item.CoQuanID /*|| item.CoQuanID == x.CoQuanChaID)*/).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == item.CoQuanID).Count(),
                         Children = (from item2 in DanhSachThongkeCapHuyen
                                     where /*item2.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() ||*/ (item2.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && item2.CoQuanChaID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.TongSoChuaKeKhai, item2.TongSoDaKeKhai, item2.CoQuanChaID } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    ThongKeChiTietKeKhaiTaiSan CapHuyen = new ThongKeChiTietKeKhaiTaiSan();
                    CapHuyen.TenCoQuan = "Cấp UBND Huyện";
                    CapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    //CapHuyen.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapHuyen.ToList(), NamKeKhai.Value).ToList().Count();
                    CapHuyen.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapHuyen.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapHuyen.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapHuyen.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    CapHuyen.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapHuyen);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                    #region CấpXã
                    //cap xa
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapXa.TenCapCoQuan = "Cấp Xã";
                    thongkeChiTietCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    var DanhSachThongkeCapXa = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                    thongkeChiTietCapXa.TongSoChuaKeKhai = DanhSachThongkeCapXa.Count(x => x.SLChuaKeKhai == 1);
                    thongkeChiTietCapXa.TongSoDaKeKhai = DanhSachThongkeCapXa.Count(x => x.SLChuaKeKhai == 0);
                    thongkeChiTietCapXa.ListThongKeChiTiet = (from item in DanhSachThongkeCapXa
                                                                  //where item.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                                                              group item by new { item.CoQuanChaID/*, item.CoQuanID, item.TenCoQuan,item.TongSoChuaKeKhai,item.TongSoDaKeKhai */} into gr
                                                              select new ThongKeChiTietKeKhaiTaiSan()
                                                              {
                                                                  CoQuanID = new DanhMucCoQuanDonViDAL().GetByID(gr.Key.CoQuanChaID).CoQuanID,
                                                                  TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(gr.Key.CoQuanChaID).TenCoQuan.Remove(0, 4).Trim(),
                                                                  CapID = EnumCapCoQuan.CapXa.GetHashCode(),
                                                                  TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr.Key.CoQuanChaID).Count(),
                                                                  TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr.Key.CoQuanChaID).Count()
                                                              }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapXa);

                    //thêm dữ liệu vào thốg kê bảng cấp xã
                    (from item in thongkeChiTietCapXa.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(item.CoQuanID.Value, NamKeKhai.Value).ToList().Count(),
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         Children = (from item2 in DanhSachThongkeCapXa
                                     where item2.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    var DanhSachCoQuanIDCapXa = DanhSachThongkeCapXa.Select(x => x.CoQuanID.Value);
                    ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
                    CapXa.TenCoQuan = "Cấp UBND Xã";
                    CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    //CapXa.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapXa.ToList(), NamKeKhai.Value).ToList().Count();
                    CapXa.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapXa.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapXa.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapXa.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapXa.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapXa.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapXa.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    CapXa.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapXa);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion
                }

                else if (CapID == EnumCapCoQuan.CapXa.GetHashCode())
                {
                    #region CấpXã
                    //cap xa
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapXa.TenCapCoQuan = "Cấp Xã";
                    thongkeChiTietCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    var DanhSachThongkeCapXa = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                    thongkeChiTietCapXa.TongSoChuaKeKhai = DanhSachThongkeCapXa.Count(x => x.SLChuaKeKhai == 1);
                    thongkeChiTietCapXa.TongSoDaKeKhai = DanhSachThongkeCapXa.Count(x => x.SLChuaKeKhai == 0);
                    thongkeChiTietCapXa.ListThongKeChiTiet = (from item in DanhSachThongkeCapXa
                                                                  //where item.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                                                              group item by new { item.CoQuanChaID/*, item.CoQuanID, item.TenCoQuan,item.TongSoChuaKeKhai,item.TongSoDaKeKhai */} into gr
                                                              select new ThongKeChiTietKeKhaiTaiSan()
                                                              {
                                                                  CoQuanID = new DanhMucCoQuanDonViDAL().GetByID(gr.Key.CoQuanChaID).CoQuanID,
                                                                  TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(gr.Key.CoQuanChaID).TenCoQuan.Remove(0, 4).Trim(),
                                                                  CapID = EnumCapCoQuan.CapXa.GetHashCode(),
                                                                  TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr.Key.CoQuanChaID).Count(),
                                                                  TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr.Key.CoQuanChaID).Count()
                                                              }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapXa);

                    //thêm dữ liệu vào thốg kê bảng cấp xã
                    (from item in thongkeChiTietCapXa.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         //TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(item.CoQuanID.Value, NamKeKhai.Value).ToList().Count(),
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Count(),
                         Children = (from item2 in DanhSachThongkeCapXa
                                     where item2.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(new List<int>() { gr2.Key.CoQuanID.Value }, NamKeKhai.Value).ToList().Count(),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count(),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count(),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count(),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.CoQuanChaID == gr2.Key.CoQuanChaID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count(),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    var DanhSachCoQuanIDCapXa = DanhSachThongkeCapXa.Select(x => x.CoQuanID.Value);
                    ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
                    CapXa.TenCoQuan = "Cấp UBND Xã";
                    CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    //CapXa.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapXa.ToList(), NamKeKhai.Value).ToList().Count();
                    CapXa.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapXa.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Count();
                    CapXa.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapXa.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Count();
                    CapXa.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapXa.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Count();
                    CapXa.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 1 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    CapXa.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.SLChuaKeKhai == 0 && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Count();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapXa);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion
                }

                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiHangNamChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiHangNamChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiHangNamDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiHangNamDaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoSungChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoSungChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoSungDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoSungDaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoNhiemChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoNhiemChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoNhiemDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoNhiemDaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiLanDauChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiLanDauChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiLanDauDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiLanDauDaKeKhai);

                ThongKeTaiSanModel.ThongKeTaiSan_Table.ListCapID = new List<int>()
                {
                    EnumCapCoQuan.CapTrungUong.GetHashCode(),
                    EnumCapCoQuan.CapTinh.GetHashCode(),
                    EnumCapCoQuan.CapSo.GetHashCode(),
                    EnumCapCoQuan.CapHuyen.GetHashCode(),
                    EnumCapCoQuan.CapPhong.GetHashCode(),
                    EnumCapCoQuan.CapXa.GetHashCode()
                };

                if (!(CoQuanID_Filter is null || CoQuanID_Filter == null))
                {
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTiet = new ThongKeChiTietKeKhaiTaiSanPar();
                    var CoQuanFilter = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID_Filter);
                    if (CoQuanFilter.CapID == EnumCapCoQuan.CapPhong.GetHashCode() || CoQuanFilter.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                    {
                        CapID = new DanhMucCoQuanDonViDAL().GetByID(CoQuanFilter.CoQuanChaID).CapID.Value;

                    }
                    else
                    {
                        CapID = CoQuanFilter.CapID.Value;
                    }
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Clear();
                    thongkeChiTiet.CapID = CapID;
                    if (CapID == EnumCapCoQuan.CapTinh.GetHashCode())
                    {
                        thongkeChiTiet.TenCapCoQuan = "Cấp Tỉnh";
                    }
                    if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                    {
                        thongkeChiTiet.TenCapCoQuan = "Cấp Huyện";
                    }
                    if (CapID == EnumCapCoQuan.CapXa.GetHashCode())
                    {
                        thongkeChiTiet.TenCapCoQuan = "Cấp Xã";
                    }
                    thongkeChiTiet.TongSoChuaKeKhai = listThongKeChiTietFromDB.Count(x => x.SLChuaKeKhai == 1);
                    thongkeChiTiet.TongSoDaKeKhai = listThongKeChiTietFromDB.Count(x => x.SLChuaKeKhai == 0);
                    thongkeChiTiet.ListThongKeChiTiet = (from item in listThongKeChiTietFromDB
                                                         group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                                                         select new ThongKeChiTietKeKhaiTaiSan()
                                                         {
                                                             CoQuanID = gr.Key.CoQuanID,
                                                             TenCoQuan = gr.Key.TenCoQuan.Trim(),
                                                             CapID = gr.Key.CapID,
                                                             CoQuanChaID = gr.Key.CoQuanChaID,
                                                             TongSoChuaKeKhai = listThongKeChiTietFromDB.Where(x => x.SLChuaKeKhai == 1 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),
                                                             TongSoDaKeKhai = listThongKeChiTietFromDB.Where(x => x.SLChuaKeKhai == 0 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),

                                                         }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTiet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ThongKeTaiSanModel;
        }

        public ThongKeTaiSanModel ThongKeChiTietKeKhaiTaiSan_Chart_New_v2(int? Type, int? CoQuanID_Filter, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan)
        {
            ThongKeTaiSanModel ThongKeTaiSanModel = new ThongKeTaiSanModel();
            try
            {
                int CapID = 0;
                var crCoQuan = new DanhMucCoQuanDonViModel();
                crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_CuaCanBoDangNhap);
                CapID = crCoQuan.CapID.Value;
                ThongKeTaiSanModel.ThongKeTaiSan_Chart = new List<ThongKeChiTietKeKhaiTaiSanPar>();
                ThongKeTaiSanModel.ThongKeTaiSan_Table = new ThongKeChiTietKeKhaiTaiSanPar();
                ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha = new List<ThongKeChiTietKeKhaiTaiSan>();
                List<int> DanhSachCapID = new List<int>();
                List<int> DanhSachCoQuan = new List<int>();
                if (CapID == EnumCapCoQuan.CapTinh.GetHashCode() || CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {    
                    if (CoQuanID_Filter > 0)
                    {
                        DanhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID_Filter).Select(x => x.CoQuanID).ToList();
                    }
                    else DanhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).Select(x => x.CoQuanID).ToList();
                }
                else if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                {
                    if (CoQuanID_Filter > 0)
                    {
                        DanhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID_Filter).Select(x => x.CoQuanID).ToList();
                    }
                    else DanhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuan_CuaCanBoDangNhap).Select(x => x.CoQuanID).ToList();
                }
                else if (CapID == EnumCapCoQuan.CapPhong.GetHashCode())
                {

                }
                else if (CapID == EnumCapCoQuan.CapXa.GetHashCode())
                {
                    if (CoQuanID_Filter > 0)
                    {
                        DanhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID_Filter).Select(x => x.CoQuanID).ToList();
                    }
                    else DanhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuan_CuaCanBoDangNhap).Select(x => x.CoQuanID).ToList();
                }
                
                var pList = new SqlParameter("@DanhSachID", SqlDbType.Structured);
                pList.TypeName = "dbo.list_ID";
                var tbCapID = new DataTable();
                tbCapID.Columns.Add("CapID", typeof(string));
                DanhSachCoQuan.ForEach(x => tbCapID.Rows.Add(x));
                SqlParameter[] parameters = new SqlParameter[]
               {
                new SqlParameter("CoQuanID",SqlDbType.Int),
                new SqlParameter("Nam",SqlDbType.Int),
                  pList
               };
                parameters[0].Value = CoQuanID_Filter ?? Convert.DBNull;
                parameters[1].Value = NamKeKhai ?? Convert.DBNull;
                parameters[2].Value = tbCapID;
                var listThongKeChiTietFromDB = new List<ThongKeChiTietKeKhaiTaiSan>();
                try
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_ThongKeDashboard_New", parameters))
                    {
                        while (dr.Read())
                        {
                            ThongKeChiTietKeKhaiTaiSan ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                            ThongKeChiTietKeKhaiTaiSan.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                            ThongKeChiTietKeKhaiTaiSan.SLChuaKeKhai = Utils.ConvertToInt32(dr["SLChuaKeKhai"], 0);
                            ThongKeChiTietKeKhaiTaiSan.SLDaKeKhai = Utils.ConvertToInt32(dr["SLDaKeKhai"], 0);
                            ThongKeChiTietKeKhaiTaiSan.LoaiDotKeKhai = Utils.ConvertToInt32(dr["LoaiDotKeKhai"], 0);
                            ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = Utils.ConvertToInt32(dr["TongSoDotKeKhai"], 0);
                            listThongKeChiTietFromDB.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (CapID == EnumCapCoQuan.CapTinh.GetHashCode() || CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {
                    #region CấpTỉnh
                    //Cap tinh
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapTinh = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapTinh.TenCapCoQuan = "Cấp Tỉnh";
                    thongkeChiTietCapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                    var DanhSachCoQuanIDCapSo = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapSo.GetHashCode()).Select(x => x.CoQuanID.Value);
                    var DanhSachThongkeCapTinh = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || x.CapID == EnumCapCoQuan.CapSo.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapSo.Contains(x.CoQuanChaID.Value))).ToList();
                    thongkeChiTietCapTinh.TongSoDaKeKhai = DanhSachThongkeCapTinh.Sum(x => x.SLDaKeKhai);
                    thongkeChiTietCapTinh.TongSoChuaKeKhai = DanhSachThongkeCapTinh.Sum(x => x.SLChuaKeKhai);
                    thongkeChiTietCapTinh.ListThongKeChiTiet = (from item in DanhSachThongkeCapTinh
                                                                where item.CapID == EnumCapCoQuan.CapSo.GetHashCode() || (item.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapSo.Contains(item.CoQuanChaID.Value))
                                                                group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                                                                select new ThongKeChiTietKeKhaiTaiSan()
                                                                {
                                                                    CoQuanID = gr.Key.CoQuanID,
                                                                    CapID = gr.Key.CapID,
                                                                    CoQuanChaID = gr.Key.CoQuanChaID,
                                                                    TenCoQuan = gr.Key.TenCoQuan,
                                                                    TongSoChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                                                    TongSoDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                                                }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapTinh);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    ThongKeChiTietKeKhaiTaiSan CapTinh = new ThongKeChiTietKeKhaiTaiSan();
                    CapTinh.TenCoQuan = "UBND Cấp Tỉnh";
                    CapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
                    var DanhSachCoQuanIDCapTinh = DanhSachThongkeCapTinh.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Select(x => x.CoQuanID.Value);
                    CapTinh.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapTinh.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapTinh.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapTinh.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapTinh.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapTinh.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapTinh.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapTinh.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    var ListThongKeChiTietUBNDCapTinh = (from item in DanhSachThongkeCapTinh
                                                                where item.CapID == EnumCapCoQuan.CapTinh.GetHashCode() 
                                                                group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.CoQuanChaID, item.TongSoDotKeKhai } into gr
                                                                select new ThongKeChiTietKeKhaiTaiSan()
                                                                {
                                                                    CoQuanID = gr.Key.CoQuanID,
                                                                    CapID = gr.Key.CapID,
                                                                    CoQuanChaID = gr.Key.CoQuanChaID,
                                                                    TenCoQuan = gr.Key.TenCoQuan,
                                                                    TongSoChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                                                    TongSoDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                                                    KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                                                    KeKhaiHangNamDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                                                    KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                                                    KeKhaiBoSungDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                                                    KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                                                    KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                                                    KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                                                    KeKhaiLanDauDaKeKhai = DanhSachThongkeCapTinh.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                                                }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapTinh);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.AddRange(ListThongKeChiTietUBNDCapTinh);
                    #endregion

                    #region CâpSở
                    //Thêm dữ liệu thống kê bảng cấp sở
                    var DanhSachThongkeCapSo = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapSo.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapSo.Contains(x.CoQuanChaID.Value))).ToList();
                    (from item in DanhSachThongkeCapSo
                     where (item.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                     group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.CoQuanChaID } into gr
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         TongSoDotKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                         CoQuanID = gr.Key.CoQuanID,
                         CapID = gr.Key.CapID,
                         TenCoQuan = gr.Key.TenCoQuan,
                         CoQuanChaID = gr.Key.CoQuanChaID,
                         TongSoChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         TongSoDaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                         Children = (from item2 in DanhSachThongkeCapSo
                                     where (item2.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && gr.Key.CoQuanID == item2.CoQuanChaID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = gr2.Key.TongSoDotKeKhai,
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         TongSoChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                         TongSoDaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapSo.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    ThongKeChiTietKeKhaiTaiSan CapSo = new ThongKeChiTietKeKhaiTaiSan();
                    CapSo.TenCoQuan = "Cấp Sở, Ngành";
                    CapSo.CapID = EnumCapCoQuan.CapSo.GetHashCode();
                    //CapSo.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapSo.ToList(), NamKeKhai.Value).ToList().Count();
                    CapSo.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapSo.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapSo.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapSo.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapSo.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapSo.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapSo.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapSo.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapSo.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapSo);
                    ///
                    #endregion

                    #region CapHuyện
                    //cap huyen
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapHuyen = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapHuyen.TenCapCoQuan = "Cấp Huyện";
                    thongkeChiTietCapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    var DanhSachCoQuanIDCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).Select(x => x.CoQuanID.Value);
                    var DanhSachThongkeCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapHuyen.Contains(x.CoQuanChaID.Value))).ToList();
                    thongkeChiTietCapHuyen.TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Sum(x => x.SLChuaKeKhai);
                    thongkeChiTietCapHuyen.TongSoDaKeKhai = DanhSachThongkeCapHuyen.Sum(x => x.SLDaKeKhai);
                    thongkeChiTietCapHuyen.ListThongKeChiTiet = (from item in DanhSachThongkeCapHuyen
                                                                 where item.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (item.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && item.CoQuanChaID == item.CoQuanID)
                                                                 group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                                                                 select new ThongKeChiTietKeKhaiTaiSan()
                                                                 {
                                                                     CoQuanID = gr.Key.CoQuanID,
                                                                     TenCoQuan = gr.Key.TenCoQuan.Remove(0, 4).Trim(),
                                                                     CapID = gr.Key.CapID,
                                                                     CoQuanChaID = gr.Key.CoQuanChaID,
                                                                     TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                                                                     TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Sum(x => x.SLDaKeKhai),
                                                                 }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapHuyen);

                    //thêm dữ liệu vào thốg kê bảng cấp  huyện
                    (from item in thongkeChiTietCapHuyen.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         TongSoDotKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         Children = (from item2 in DanhSachThongkeCapHuyen
                                     where (/*item2.CapID == EnumCapCoQuan.CapPhong.GetHashCode() &&*/ item2.CoQuanChaID == item.CoQuanID || item2.CoQuanID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.TongSoChuaKeKhai, item2.TongSoDaKeKhai, item2.CoQuanChaID, item.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                         TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    ThongKeChiTietKeKhaiTaiSan CapHuyen = new ThongKeChiTietKeKhaiTaiSan();
                    CapHuyen.TenCoQuan = "Cấp UBND Huyện";
                    CapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    //CapHuyen.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapHuyen.ToList(), NamKeKhai.Value).ToList().Count();
                    CapHuyen.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapHuyen.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapHuyen.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapHuyen.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapHuyen);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                    #region CấpXã
                    //cap xa
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapXa.TenCapCoQuan = "Cấp Xã";
                    thongkeChiTietCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    var DanhSachThongkeCapXa = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                    thongkeChiTietCapXa.TongSoChuaKeKhai = DanhSachThongkeCapXa.Sum(x => x.SLChuaKeKhai);
                    thongkeChiTietCapXa.TongSoDaKeKhai = DanhSachThongkeCapXa.Sum(x => x.SLDaKeKhai);
                    thongkeChiTietCapXa.ListThongKeChiTiet = (from item in DanhSachThongkeCapXa      
                                                              group item by new { item.CoQuanChaID, item.TenCoQuanCha } into gr
                                                              select new ThongKeChiTietKeKhaiTaiSan()
                                                              {
                                                                  CoQuanID = gr.Key.CoQuanChaID,
                                                                  TenCoQuan = gr.Key.TenCoQuanCha.Remove(0, 4).Trim(),
                                                                  CapID = EnumCapCoQuan.CapXa.GetHashCode(),
                                                                  TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanChaID == gr.Key.CoQuanChaID).Sum(x => x.SLChuaKeKhai),
                                                                  TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanChaID == gr.Key.CoQuanChaID).Sum(x => x.SLDaKeKhai)
                                                              }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapXa);

                    //thêm dữ liệu vào thốg kê bảng cấp xã
                    (from item in thongkeChiTietCapXa.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {        
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         Children = (from item2 in DanhSachThongkeCapXa
                                     where (item2.CapID == EnumCapCoQuan.CapXa.GetHashCode() && item2.CoQuanChaID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                         TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    var DanhSachCoQuanIDCapXa = DanhSachThongkeCapXa.Select(x => x.CoQuanID.Value);
                    ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
                    CapXa.TenCoQuan = "Cấp UBND Xã";
                    CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    CapXa.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapXa);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                }
                else if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                {
                    #region CapHuyện
                    //cap huyen
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapHuyen = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapHuyen.TenCapCoQuan = "Cấp Huyện";
                    thongkeChiTietCapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    var DanhSachCoQuanIDCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).Select(x => x.CoQuanID.Value);
                    var DanhSachThongkeCapHuyen = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (x.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && DanhSachCoQuanIDCapHuyen.Contains(x.CoQuanChaID.Value))).ToList();
                    thongkeChiTietCapHuyen.TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Sum(x => x.SLChuaKeKhai);
                    thongkeChiTietCapHuyen.TongSoDaKeKhai = DanhSachThongkeCapHuyen.Sum(x => x.SLDaKeKhai);
                    thongkeChiTietCapHuyen.ListThongKeChiTiet = (from item in DanhSachThongkeCapHuyen
                                                                 where item.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || (item.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && item.CoQuanChaID == item.CoQuanID)
                                                                 group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                                                                 select new ThongKeChiTietKeKhaiTaiSan()
                                                                 {
                                                                     CoQuanID = gr.Key.CoQuanID,
                                                                     TenCoQuan = gr.Key.TenCoQuan.Remove(0, 4).Trim(),
                                                                     CapID = gr.Key.CapID,
                                                                     CoQuanChaID = gr.Key.CoQuanChaID,
                                                                     TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                                                                     TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Sum(x => x.SLDaKeKhai),
                                                                 }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapHuyen);

                    //thêm dữ liệu vào thốg kê bảng cấp  huyện
                    (from item in thongkeChiTietCapHuyen.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         TongSoDotKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanChaID == item.CoQuanID || x.CoQuanID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         Children = (from item2 in DanhSachThongkeCapHuyen
                                     where (/*item2.CapID == EnumCapCoQuan.CapPhong.GetHashCode() &&*/ item2.CoQuanChaID == item.CoQuanID || item2.CoQuanID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.TongSoChuaKeKhai, item2.TongSoDaKeKhai, item2.CoQuanChaID, item.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         TongSoChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                         TongSoDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    ThongKeChiTietKeKhaiTaiSan CapHuyen = new ThongKeChiTietKeKhaiTaiSan();
                    CapHuyen.TenCoQuan = "Cấp UBND Huyện";
                    CapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
                    //CapHuyen.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID_New(DanhSachCoQuanIDCapHuyen.ToList(), NamKeKhai.Value).ToList().Count();
                    CapHuyen.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapHuyen.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapHuyen.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapHuyen.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapHuyen.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapHuyen.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapHuyen);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                    #region CấpXã
                    //cap xa
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapXa.TenCapCoQuan = "Cấp Xã";
                    thongkeChiTietCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    var DanhSachThongkeCapXa = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                    thongkeChiTietCapXa.TongSoChuaKeKhai = DanhSachThongkeCapXa.Sum(x => x.SLChuaKeKhai);
                    thongkeChiTietCapXa.TongSoDaKeKhai = DanhSachThongkeCapXa.Sum(x => x.SLDaKeKhai);
                    thongkeChiTietCapXa.ListThongKeChiTiet = (from item in DanhSachThongkeCapXa
                                                              group item by new { item.CoQuanChaID, item.TenCoQuanCha } into gr
                                                              select new ThongKeChiTietKeKhaiTaiSan()
                                                              {
                                                                  CoQuanID = gr.Key.CoQuanChaID,
                                                                  TenCoQuan = gr.Key.TenCoQuanCha.Remove(0, 4).Trim(),
                                                                  CapID = EnumCapCoQuan.CapXa.GetHashCode(),
                                                                  TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanChaID == gr.Key.CoQuanChaID).Sum(x => x.SLChuaKeKhai),
                                                                  TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanChaID == gr.Key.CoQuanChaID).Sum(x => x.SLDaKeKhai)
                                                              }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapXa);

                    //thêm dữ liệu vào thốg kê bảng cấp xã
                    (from item in thongkeChiTietCapXa.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         Children = (from item2 in DanhSachThongkeCapXa
                                     where (item2.CapID == EnumCapCoQuan.CapXa.GetHashCode() && item2.CoQuanChaID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                         TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    var DanhSachCoQuanIDCapXa = DanhSachThongkeCapXa.Select(x => x.CoQuanID.Value);
                    ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
                    CapXa.TenCoQuan = "Cấp UBND Xã";
                    CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    CapXa.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapXa);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion
                }

                else if (CapID == EnumCapCoQuan.CapXa.GetHashCode())
                {
                    #region CấpXã
                    //cap xa
                    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTietCapXa = new ThongKeChiTietKeKhaiTaiSanPar();
                    thongkeChiTietCapXa.TenCapCoQuan = "Cấp Xã";
                    thongkeChiTietCapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    var DanhSachThongkeCapXa = listThongKeChiTietFromDB.Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                    thongkeChiTietCapXa.TongSoChuaKeKhai = DanhSachThongkeCapXa.Sum(x => x.SLChuaKeKhai);
                    thongkeChiTietCapXa.TongSoDaKeKhai = DanhSachThongkeCapXa.Sum(x => x.SLDaKeKhai);
                    thongkeChiTietCapXa.ListThongKeChiTiet = (from item in DanhSachThongkeCapXa
                                                              group item by new { item.CoQuanChaID, item.TenCoQuanCha } into gr
                                                              select new ThongKeChiTietKeKhaiTaiSan()
                                                              {
                                                                  CoQuanID = gr.Key.CoQuanChaID,
                                                                  TenCoQuan = gr.Key.TenCoQuanCha.Remove(0, 4).Trim(),
                                                                  CapID = EnumCapCoQuan.CapXa.GetHashCode(),
                                                                  TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanChaID == gr.Key.CoQuanChaID).Sum(x => x.SLChuaKeKhai),
                                                                  TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanChaID == gr.Key.CoQuanChaID).Sum(x => x.SLDaKeKhai)
                                                              }).ToList();
                    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTietCapXa);

                    //thêm dữ liệu vào thốg kê bảng cấp xã
                    (from item in thongkeChiTietCapXa.ListThongKeChiTiet
                     select new ThongKeChiTietKeKhaiTaiSan()
                     {
                         CoQuanID = item.CoQuanID,
                         CapID = item.CapID,
                         TenCoQuan = item.TenCoQuan,
                         CoQuanChaID = item.CoQuanChaID,
                         TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLChuaKeKhai),
                         TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == item.CoQuanID).Sum(x => x.SLDaKeKhai),
                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLChuaKeKhai),
                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode() && (x.CoQuanID == item.CoQuanID || x.CoQuanChaID == item.CoQuanID)).Sum(x => x.SLDaKeKhai),
                         Children = (from item2 in DanhSachThongkeCapXa
                                     where (item2.CapID == EnumCapCoQuan.CapXa.GetHashCode() && item2.CoQuanChaID == item.CoQuanID)
                                     group item2 by new { item2.CoQuanID, item2.TenCoQuan, item2.CapID, item2.CoQuanChaID, item2.TongSoDotKeKhai } into gr2
                                     select new ThongKeChiTietKeKhaiTaiSan()
                                     {
                                         TongSoDotKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.TongSoDotKeKhai),
                                         CoQuanID = gr2.Key.CoQuanID,
                                         CoQuanChaID = gr2.Key.CoQuanChaID,
                                         CapID = gr2.Key.CapID,
                                         TenCoQuan = gr2.Key.TenCoQuan,
                                         TongSoChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLChuaKeKhai),
                                         TongSoDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID).Sum(x => x.SLDaKeKhai),
                                         KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                         KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai),
                                         KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.CoQuanID == gr2.Key.CoQuanID && x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai),
                                     }).ToList()
                     }).ToList().ForEach(x => ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeChiTiet.Add(x));

                    var DanhSachCoQuanIDCapXa = DanhSachThongkeCapXa.Select(x => x.CoQuanID.Value);
                    ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
                    CapXa.TenCoQuan = "Cấp UBND Xã";
                    CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
                    CapXa.KeKhaiHangNamChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiHangNamDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.HangNam.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiBoSungChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiBoSungDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiBoNhiemChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiBoNhiemDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoNhiem.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    CapXa.KeKhaiLanDauChuaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLChuaKeKhai);
                    CapXa.KeKhaiLanDauDaKeKhai = DanhSachThongkeCapXa.Where(x => x.LoaiDotKeKhai == EnumLoaiDotKeKhai.LanDau.GetHashCode()).Sum(x => x.SLDaKeKhai);
                    ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Add(CapXa);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion
                }

                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiHangNamChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiHangNamChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiHangNamDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiHangNamDaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoSungChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoSungChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoSungDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoSungDaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoNhiemChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoNhiemChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiBoNhiemDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiBoNhiemDaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiLanDauChuaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiLanDauChuaKeKhai);
                ThongKeTaiSanModel.ThongKeTaiSan_Table.TongKeKhaiLanDauDaKeKhai = ThongKeTaiSanModel.ThongKeTaiSan_Table.ListThongKeCoQuanCha.Sum(x => x.KeKhaiLanDauDaKeKhai);

                ThongKeTaiSanModel.ThongKeTaiSan_Table.ListCapID = new List<int>()
                {
                    EnumCapCoQuan.CapTrungUong.GetHashCode(),
                    EnumCapCoQuan.CapTinh.GetHashCode(),
                    EnumCapCoQuan.CapSo.GetHashCode(),
                    EnumCapCoQuan.CapHuyen.GetHashCode(),
                    EnumCapCoQuan.CapPhong.GetHashCode(),
                    EnumCapCoQuan.CapXa.GetHashCode()
                };

                //if (!(CoQuanID_Filter is null || CoQuanID_Filter == null))
                //{
                //    ThongKeChiTietKeKhaiTaiSanPar thongkeChiTiet = new ThongKeChiTietKeKhaiTaiSanPar();
                //    var CoQuanFilter = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID_Filter);
                //    if (CoQuanFilter.CapID == EnumCapCoQuan.CapPhong.GetHashCode() || CoQuanFilter.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                //    {
                //        CapID = new DanhMucCoQuanDonViDAL().GetByID(CoQuanFilter.CoQuanChaID).CapID.Value;

                //    }
                //    else
                //    {
                //        CapID = CoQuanFilter.CapID.Value;
                //    }
                //    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Clear();
                //    thongkeChiTiet.CapID = CapID;
                //    if (CapID == EnumCapCoQuan.CapTinh.GetHashCode())
                //    {
                //        thongkeChiTiet.TenCapCoQuan = "Cấp Tỉnh";
                //    }
                //    if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                //    {
                //        thongkeChiTiet.TenCapCoQuan = "Cấp Huyện";
                //    }
                //    if (CapID == EnumCapCoQuan.CapXa.GetHashCode())
                //    {
                //        thongkeChiTiet.TenCapCoQuan = "Cấp Xã";
                //    }
                //    thongkeChiTiet.TongSoChuaKeKhai = listThongKeChiTietFromDB.Count(x => x.SLChuaKeKhai == 1);
                //    thongkeChiTiet.TongSoDaKeKhai = listThongKeChiTietFromDB.Count(x => x.SLChuaKeKhai == 0);
                //    thongkeChiTiet.ListThongKeChiTiet = (from item in listThongKeChiTietFromDB
                //                                         group item by new { item.CoQuanID, item.CapID, item.TenCoQuan, item.TongSoChuaKeKhai, item.TongSoDaKeKhai, item.CoQuanChaID } into gr
                //                                         select new ThongKeChiTietKeKhaiTaiSan()
                //                                         {
                //                                             CoQuanID = gr.Key.CoQuanID,
                //                                             TenCoQuan = gr.Key.TenCoQuan.Trim(),
                //                                             CapID = gr.Key.CapID,
                //                                             CoQuanChaID = gr.Key.CoQuanChaID,
                //                                             TongSoChuaKeKhai = listThongKeChiTietFromDB.Where(x => x.SLChuaKeKhai == 1 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),
                //                                             TongSoDaKeKhai = listThongKeChiTietFromDB.Where(x => x.SLChuaKeKhai == 0 && (x.CoQuanID == gr.Key.CoQuanID || x.CoQuanChaID == gr.Key.CoQuanID)).Count(),

                //                                         }).ToList();
                //    ThongKeTaiSanModel.ThongKeTaiSan_Chart.Add(thongkeChiTiet);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ThongKeTaiSanModel;
        }

        public ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSan(int? CoQuanID, int? CapQuanLy, int? NamKeKhai, int? NguoiDungID, int? CoQuan_CuaCanBoDangNhap, ref List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhai)
        {
            ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSanPar = new ThongKeChiTietKeKhaiTaiSanPar();
            ThongKeChiTietKeKhaiTaiSan CapTinh = new ThongKeChiTietKeKhaiTaiSan();
            //List<int>  ListDotKeKhaiCapTinh  = new l
            CapTinh.CapID = EnumCapCoQuan.CapTinh.GetHashCode();
            CapTinh.TenCoQuan = "UBND Cấp Tỉnh";
            ThongKeChiTietKeKhaiTaiSan CapSo = new ThongKeChiTietKeKhaiTaiSan();
            CapSo.CapID = EnumCapCoQuan.CapSo.GetHashCode();
            CapSo.TenCoQuan = "Cấp Sở,Ngành";
            ThongKeChiTietKeKhaiTaiSan CapHuyen = new ThongKeChiTietKeKhaiTaiSan();
            CapHuyen.CapID = EnumCapCoQuan.CapHuyen.GetHashCode();
            CapHuyen.TenCoQuan = "Cấp UBND Huyện";
            ThongKeChiTietKeKhaiTaiSan CapPhong = new ThongKeChiTietKeKhaiTaiSan();
            CapPhong.CapID = EnumCapCoQuan.CapPhong.GetHashCode();
            CapPhong.TenCoQuan = "Cấp Phòng Ban ";
            ThongKeChiTietKeKhaiTaiSan CapXa = new ThongKeChiTietKeKhaiTaiSan();
            CapXa.CapID = EnumCapCoQuan.CapXa.GetHashCode();
            CapXa.TenCoQuan = "Cấp UBND Xã ";
            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            //tbCoQuanID.Rows.Add(1);
            //CoQuanID.ForEach(x => table.Rows.Add(x));
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_CuaCanBoDangNhap);

            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan = new List<ThongKeChiTietKeKhaiTaiSan>();
            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            if (UserRole.CheckAdmin(NguoiDungID.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
            }
            else if (listThanhTraTinh.Contains(CoQuan_CuaCanBoDangNhap.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else if (listThanhTraHuyen.Contains(CoQuan_CuaCanBoDangNhap.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID).ToList();
            }
            if (listCoQuanCon != null && listCoQuanCon.Count > 0)
            {
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            SqlParameter[] parameters = new SqlParameter[]
               {
                new SqlParameter(CO_QUAN_ID,SqlDbType.Int),
                new SqlParameter("@CapQuanLy",SqlDbType.Int),
                new SqlParameter(NAM_KE_KHAI_DOTKEKHAI,SqlDbType.Int),
                  pList
               };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            parameters[1].Value = CapQuanLy ?? Convert.DBNull;
            parameters[2].Value = NamKeKhai ?? Convert.DBNull;
            parameters[3].Value = tbCoQuanID;
            try
            {
                var listThongKeChiTietFromDB = new List<ThongKeChiTietKeKhaiTaiSan>();
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, THONG_KE_CHI_TIET_TAI_SAN, parameters))
                {
                    while (dr.Read())
                    {
                        ThongKeChiTietKeKhaiTaiSan ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                        ThongKeChiTietKeKhaiTaiSan.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        ThongKeChiTietKeKhaiTaiSan.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        ThongKeChiTietKeKhaiTaiSan.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiHangNamDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoSungDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoNhiemDaKeKhai"], 0);
                        //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoNhiemDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoSungChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoNhiemChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiHangNamChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiLanDauChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiLanDauDaKeKhai"], 0);
                        //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).Count;
                        listThongKeChiTietFromDB.Add(ThongKeChiTietKeKhaiTaiSan);

                    }
                    dr.Close();
                }
                var DanhSachCoQuan = listThongKeChiTietFromDB.Select(x => new DanhMucCoQuanDonViModel()
                {
                    CoQuanID = x.CoQuanID.Value,
                    TenCoQuan = x.TenCoQuan,
                    CoQuanChaID = x.CoQuanChaID,
                    CapID = x.CapID
                }
                ).ToList().Distinct().ToList();
                ListThongKeChiTietKeKhai = listThongKeChiTietFromDB;
                for (int i = 0; i < listThongKeChiTietFromDB.Count; i++)
                {
                    var ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                    ThongKeChiTietKeKhaiTaiSan.CapID = listThongKeChiTietFromDB[i].CapID;
                    ThongKeChiTietKeKhaiTaiSan.Children = listThongKeChiTietFromDB[i].Children;
                    ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = listThongKeChiTietFromDB[i].CoQuanChaID;
                    ThongKeChiTietKeKhaiTaiSan.CoQuanID = listThongKeChiTietFromDB[i].CoQuanID;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai = listThongKeChiTietFromDB[i].KeKhaiBoNhiemChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = listThongKeChiTietFromDB[i].KeKhaiBoNhiemDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai = listThongKeChiTietFromDB[i].KeKhaiBoSungChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai = listThongKeChiTietFromDB[i].KeKhaiBoSungDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai = listThongKeChiTietFromDB[i].KeKhaiHangNamChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai = listThongKeChiTietFromDB[i].KeKhaiHangNamDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai = listThongKeChiTietFromDB[i].KeKhaiLanDauChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai = listThongKeChiTietFromDB[i].KeKhaiLanDauDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.TenCoQuan = listThongKeChiTietFromDB[i].TenCoQuan;
                    ThongKeChiTietKeKhaiTaiSan.TongSoChuaKeKhai = listThongKeChiTietFromDB[i].TongSoChuaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.TongSoDaKeKhai = listThongKeChiTietFromDB[i].TongSoDaKeKhai;
                    ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = listThongKeChiTietFromDB[i].TongSoDotKeKhai;

                    if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())        // Cấp phòng    
                    {

                        var ListCoQuanCha = new DanhMucCoQuanDonViDAL().GetAllByCapCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        // lấy ra cơ quan cha có cấp = cấp huyện để hiển thị
                        var CoQuanCapChaPhuHop = ListCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).ToList().FirstOrDefault();
                        if (CoQuanCapChaPhuHop == null || CoQuanCapChaPhuHop.CoQuanID <= 0)
                        {
                            //CoQuanCapChaPhuHop = new DanhMucCoQuanDonViDAL().GetByID1(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                            CoQuanCapChaPhuHop = DanhSachCoQuan.FirstOrDefault(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        }
                        if (CoQuanCapChaPhuHop.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())
                        {
                            CapHuyen.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CapHuyen.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CapHuyen.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan;
                            CapHuyen.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CapHuyen.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CapHuyen.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CapHuyen.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CapHuyen.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                        }
                        else
                        {
                            CapSo.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CapSo.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CapSo.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan;
                            CapSo.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CapSo.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CapSo.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CapSo.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CapSo.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                        }
                        // check cơ quan này đã tồn tại chưa
                        var CoQuanCapHuyen = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == CoQuanCapChaPhuHop.CapID).FirstOrDefault();
                        if (CoQuanCapHuyen == null || CoQuanCapHuyen.CoQuanID <= 0)
                        {
                            //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count();
                            var TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).ToList();
                            if (NamKeKhai != null && TongSoDotKeKhai.Count > 0)
                            {
                                ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = TongSoDotKeKhai.Where(x => x.NamKeKhai == NamKeKhai).ToList().GroupBy(x => x.DotKeKhaiID).Count();
                            }
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            //var ListThongKeChiTiet = ThongKeByCoQuanID(CoQuanCapChaPhuHop.CoQuanID, CoQuanCapChaPhuHop.CapID, CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value).ListThongKeChiTiet;
                            var ListThongKeChiTiet = ThongKeBanKeKhaiTheoCoQuanCha(CoQuanCapChaPhuHop.CoQuanID, CoQuanCapChaPhuHop.CapID, CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value, listThongKeChiTietFromDB).ListThongKeChiTiet;
                            if (ListThongKeChiTiet != null)
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = ListThongKeChiTiet.ToList();
                            }
                            else
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = new List<ThongKeChiTietKeKhaiTaiSan>();
                            }
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                            ThongKeChiTietKeKhaiTaiSan.CapID = CoQuanCapChaPhuHop.CapID;
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                            ListThongKeChiTietKeKhaiTaiSan.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                        else
                        {
                            //lấy ra chính nó update số liệu
                            var CoQuanChinhNoDauTien = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID.Value == CoQuanCapChaPhuHop.CoQuanID && x.CapID == CoQuanCapChaPhuHop.CapID)
                                  .FirstOrDefault();
                            CoQuanChinhNoDauTien.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                            CoQuanChinhNoDauTien.TongSoDotKeKhai = CoQuanCapHuyen.TongSoDotKeKhai;
                        }
                    }  // Cấp phòng
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())  // cấp tỉnh 
                    {
                        CapTinh.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                        CapTinh.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                        CapTinh.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                        //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan;
                        CapTinh.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                        CapTinh.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                        CapTinh.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                        CapTinh.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                        CapTinh.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                        //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count();
                        // check nó đã tồn tại chưa
                        var CoQuanCapTinh = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                        if (CoQuanCapTinh == null || CoQuanCapTinh.CoQuanID <= 0)
                        {
                            //var ListThongKeChiTiet = ThongKeByCoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, EnumCapCoQuan.CapTinh.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value).ListThongKeChiTiet;
                            var ListThongKeChiTiet = ThongKeBanKeKhaiTheoCoQuanCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, EnumCapCoQuan.CapTinh.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value, listThongKeChiTietFromDB).ListThongKeChiTiet;
                            ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanIDAndNamKeKhai(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, NamKeKhai).ToList().Count();
                            if (ListThongKeChiTiet != null)
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = ListThongKeChiTiet.Where(x => x.CapID == EnumCapCoQuan.CapTinh.GetHashCode()).ToList();
                            }
                            else
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = new List<ThongKeChiTietKeKhaiTaiSan>();
                            }

                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            ListThongKeChiTietKeKhaiTaiSan.Add(ThongKeChiTietKeKhaiTaiSan);
                        }

                        else
                        {
                            // lấy ra chính nó để update số liệu
                            var CoQuanChinhNoDauTien = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID.Value == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                            CoQuanChinhNoDauTien.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;

                        }
                    }  // Cấp tỉnh
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()) // Cấp huyện
                    {
                        CapHuyen.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                        CapHuyen.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                        CapHuyen.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                        //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan;
                        CapHuyen.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                        CapHuyen.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                        CapHuyen.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                        CapHuyen.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                        CapHuyen.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;

                        //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count();
                        var CoQuanCapHuyen = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                        if (CoQuanCapHuyen == null || CoQuanCapHuyen.CoQuanID <= 0)
                        {
                            //var ListThongKeChiTiet1 = ThongKeByCoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, EnumCapCoQuan.CapHuyen.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value).ListThongKeChiTiet;
                            var ListThongKeChiTiet = ThongKeBanKeKhaiTheoCoQuanCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, EnumCapCoQuan.CapHuyen.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value, listThongKeChiTietFromDB).ListThongKeChiTiet;
                            ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanIDAndNamKeKhai(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, NamKeKhai).ToList().Count;
                            if (ListThongKeChiTiet != null)
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = ListThongKeChiTiet.ToList();
                            }
                            else
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = new List<ThongKeChiTietKeKhaiTaiSan>();
                            }
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            ListThongKeChiTietKeKhaiTaiSan.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                        else
                        {
                            // lấy ra chính nó để update số liệu
                            var CoQuanChinhNoDauTien = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID.Value == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                            CoQuanChinhNoDauTien.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                        }
                    }  // Cấp huyện
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) // Cấp sở
                    {
                        CapSo.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                        CapSo.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                        CapSo.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                        //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan;
                        CapSo.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                        CapSo.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                        CapSo.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                        CapSo.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                        CapSo.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;

                        //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count();
                        ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanIDAndNamKeKhai(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, NamKeKhai).ToList().Count;
                        var CoQuanCapSo = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                        if (CoQuanCapSo == null || CoQuanCapSo.CoQuanID <= 0)
                        {
                            //var ListThongKeChiTiet = ThongKeByCoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, EnumCapCoQuan.CapSo.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value).ListThongKeChiTiet;
                            var ListThongKeChiTiet = ThongKeBanKeKhaiTheoCoQuanCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, EnumCapCoQuan.CapSo.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value, listThongKeChiTietFromDB).ListThongKeChiTiet;
                            if (ListThongKeChiTiet != null)
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = ListThongKeChiTiet.ToList();
                            }
                            else
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = new List<ThongKeChiTietKeKhaiTaiSan>();
                            }

                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }
                            ListThongKeChiTietKeKhaiTaiSan.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                        else
                        {
                            // lấy ra chính nó để update số liệu
                            var CoQuanChinhNoDauTien = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID.Value == ThongKeChiTietKeKhaiTaiSan.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                            CoQuanChinhNoDauTien.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;

                        }
                    }  // Cấp sở
                    else if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapXa.GetHashCode()) // Cấp xã
                    {
                        CapXa.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                        CapXa.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                        CapXa.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                        //ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan;
                        CapXa.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                        CapXa.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                        CapXa.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                        CapXa.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                        CapXa.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                        var ListCoQuanCha = new DanhMucCoQuanDonViDAL().GetAllByCapCha(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        var CoQuanCapChaPhuHop = ListCoQuanCha.Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).ToList().FirstOrDefault();
                        if (CoQuanCapChaPhuHop == null || CoQuanCapChaPhuHop.CoQuanID <= 0)
                        {
                            //CoQuanCapChaPhuHop = new DanhMucCoQuanDonViDAL().GetByID1(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                            CoQuanCapChaPhuHop = DanhSachCoQuan.FirstOrDefault(x => x.CoQuanID == ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value);
                        }
                        var CoQuanCha = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID == CoQuanCapChaPhuHop.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID).FirstOrDefault();
                        if (CoQuanCha == null || CoQuanCha.CoQuanID <= 0)
                        {
                            var ListThongKeChiTiet = ThongKeBanKeKhaiTheoCoQuanCha(CoQuanCapChaPhuHop.CoQuanID, EnumCapCoQuan.CapXa.GetHashCode(), CapQuanLy, CoQuanID, NamKeKhai, CoQuan_CuaCanBoDangNhap, NguoiDungID.Value, listThongKeChiTietFromDB).ListThongKeChiTiet;
                            if (ListThongKeChiTiet != null)
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = ListThongKeChiTiet.ToList();
                            }
                            else
                            {
                                ThongKeChiTietKeKhaiTaiSan.Children = new List<ThongKeChiTietKeKhaiTaiSan>();
                            }
                            //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count();
                            ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanIDAndNamKeKhai(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value, NamKeKhai).ToList().Count;
                            ThongKeChiTietKeKhaiTaiSan.TenCoQuan = CoQuanCapChaPhuHop.TenCoQuan;
                            ThongKeChiTietKeKhaiTaiSan.CoQuanID = CoQuanCapChaPhuHop.CoQuanID;
                            ThongKeChiTietKeKhaiTaiSan.CapID = ThongKeChiTietKeKhaiTaiSan.CapID;
                            ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = CoQuanCapChaPhuHop.CoQuanChaID;
                            if (ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Contains("UBND") == true && !string.IsNullOrEmpty(ThongKeChiTietKeKhaiTaiSan.TenCoQuan))
                            {
                                string[] replace = ThongKeChiTietKeKhaiTaiSan.TenCoQuan.Split("UBND");
                                ThongKeChiTietKeKhaiTaiSan.TenCoQuan = replace[1].ToString().Trim();
                            }

                            ListThongKeChiTietKeKhaiTaiSan.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                        else
                        {
                            var CoQuanChinhNoDauTien = ListThongKeChiTietKeKhaiTaiSan.Where(x => x.CoQuanID.Value == CoQuanCapChaPhuHop.CoQuanID && x.CapID == ThongKeChiTietKeKhaiTaiSan.CapID)
                                  .FirstOrDefault();
                            CoQuanChinhNoDauTien.KeKhaiHangNamDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoSungChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiBoNhiemChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiHangNamChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauChuaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai;
                            CoQuanChinhNoDauTien.KeKhaiLanDauDaKeKhai += ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai;
                            CoQuanChinhNoDauTien.TongSoDotKeKhai = CoQuanCha.TongSoDotKeKhai;

                        }
                    } // Cấp xã
                }
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha = new List<ThongKeChiTietKeKhaiTaiSan>();
                CapTinh.TongSoDotKeKhai = new DotKeKhaiDAL().GetDotKeKhai_By_CapCoQuan(EnumCapCoQuan.CapTinh.GetHashCode(), NamKeKhai).Count;
                CapSo.TongSoDotKeKhai = new DotKeKhaiDAL().GetDotKeKhai_By_CapCoQuan(EnumCapCoQuan.CapSo.GetHashCode(), NamKeKhai).Count;
                CapHuyen.TongSoDotKeKhai = new DotKeKhaiDAL().GetDotKeKhai_By_CapCoQuan(EnumCapCoQuan.CapHuyen.GetHashCode(), NamKeKhai).Count;
                CapPhong.TongSoDotKeKhai = new DotKeKhaiDAL().GetDotKeKhai_By_CapCoQuan(EnumCapCoQuan.CapPhong.GetHashCode(), NamKeKhai).Count;
                CapXa.TongSoDotKeKhai = new DotKeKhaiDAL().GetDotKeKhai_By_CapCoQuan(EnumCapCoQuan.CapXa.GetHashCode(), NamKeKhai).Count;
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha.Add(CapTinh);
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha.Add(CapSo);
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha.Add(CapHuyen);
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha.Add(CapPhong);
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha.Add(CapXa);
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID = new List<int>();
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID.Add(EnumCapCoQuan.CapTrungUong.GetHashCode());
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID.Add(EnumCapCoQuan.CapTinh.GetHashCode());
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID.Add(EnumCapCoQuan.CapSo.GetHashCode());
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID.Add(EnumCapCoQuan.CapHuyen.GetHashCode());
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID.Add(EnumCapCoQuan.CapPhong.GetHashCode());
                ThongKeChiTietKeKhaiTaiSanPar.ListCapID.Add(EnumCapCoQuan.CapXa.GetHashCode());
                ThongKeChiTietKeKhaiTaiSanPar.TongSoDotKeKhai = new DotKeKhaiDAL().GetAll().GroupBy(x => x.DotKeKhaiID).Count();
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeChiTiet = ListThongKeChiTietKeKhaiTaiSan;
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiBoNhiemDaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiBoNhiemDaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiBoNhiemChuaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiBoNhiemChuaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiBoSungChuaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiBoSungChuaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiBoSungDaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiBoSungDaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiHangNamChuaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiHangNamChuaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiHangNamDaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiHangNamDaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiLanDauChuaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiLanDauChuaKeKhai);
                ThongKeChiTietKeKhaiTaiSanPar.TongKeKhaiLanDauDaKeKhai = ListThongKeChiTietKeKhaiTaiSan.Sum(x => x.KeKhaiLanDauDaKeKhai);


            }
            catch (Exception ex)
            {

                throw ex;
            }
            if (CapQuanLy != null || CapQuanLy > 0)
            {
                ThongKeChiTietKeKhaiTaiSanPar.ListThongKeCoQuanCha.Where(x => x.CapID == CapQuanLy).ToList();
            }
            return ThongKeChiTietKeKhaiTaiSanPar;
        }
        // Lấy cán bộ theo cơ quan và loại kê khai
        public List<HeThongCanBoShortModel> GetCanBoByCoQuanAndLoaiKeKhai(int? CoQuanID, int? NamKeKhai, int? CapQuanLy, int? LoaiKeKhai, int CoQuan_CuaCanBoDangNhap, int NguoiDungID)
        {
            List<HeThongCanBoShortModel> ListCanBo = new List<HeThongCanBoShortModel>();
            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_CuaCanBoDangNhap);
            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan = new List<ThongKeChiTietKeKhaiTaiSan>();
            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
            }
            else if (listThanhTraTinh.Contains(CoQuan_CuaCanBoDangNhap))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else if (listThanhTraHuyen.Contains(CoQuan_CuaCanBoDangNhap))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID).ToList();
            }
            if (listCoQuanCon != null && listCoQuanCon.Count > 0)
            {
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            var ListCanBoTrongCoQuan = new HeThongCanBoDAL().GetAllByListCoQuanID(new List<int>() { CoQuanID.Value });
            HeThongCanBoShortModel HeThongCanBoShortModel = new HeThongCanBoShortModel();
            foreach (var item in ListCanBoTrongCoQuan)
            {


                if (LoaiKeKhai == 6 || LoaiKeKhai == 7 || LoaiKeKhai == 5 || LoaiKeKhai == 8)
                {
                    int LoaiKeKhaiNew = 0;
                    if (LoaiKeKhai == 6) { LoaiKeKhaiNew = EnumLoaiDotKeKhai.BoSung.GetHashCode(); }
                    else if (LoaiKeKhai == 7) { LoaiKeKhaiNew = EnumLoaiDotKeKhai.BoNhiem.GetHashCode(); }
                    else if (LoaiKeKhai == 8) { LoaiKeKhaiNew = EnumLoaiDotKeKhai.LanDau.GetHashCode(); }
                    else if (LoaiKeKhai == 5) { LoaiKeKhaiNew = EnumLoaiDotKeKhai.HangNam.GetHashCode(); }
                    var DotKeKhaiByCanBoID = new DotKeKhaiDAL().GetDotKeKhaiByCanBoID(item.CanBoID).Where(x => x.LoaiDotKeKhai == LoaiKeKhaiNew).ToList();
                    foreach (var i in DotKeKhaiByCanBoID)
                    {
                        var KeKhaiByDotKeKhaiIDAndCanBoID = new KeKhaiDAL().GetKeKhaiByCanBoID(item.CanBoID).Where(x => x.DotKeKhaiID == i.DotKeKhaiID).ToList();
                        if (KeKhaiByDotKeKhaiIDAndCanBoID.Count <= 0)
                        {
                            HeThongCanBoShortModel = new HeThongCanBoShortModel();
                            HeThongCanBoShortModel.TenCanBo = item.TenCanBo;
                            HeThongCanBoShortModel.CanBoID = item.CanBoID;
                            HeThongCanBoShortModel.MaCB = item.MaCB;
                            HeThongCanBoShortModel.CoQuanID = item.CoQuanID;
                            HeThongCanBoShortModel.TenCoQuan = item.TenCoQuan;
                            HeThongCanBoShortModel.NgaySinh = item.NgaySinh;
                            HeThongCanBoShortModel.TenDotKeKhai = i.TenDotKeKhai;
                            var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(HeThongCanBoShortModel.CanBoID);
                            HeThongCanBoShortModel.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            HeThongCanBoShortModel.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            ListCanBo.Add(HeThongCanBoShortModel);
                        }
                    }
                }
            }
            if (LoaiKeKhai == 1 || LoaiKeKhai == 2 || LoaiKeKhai == 3 || LoaiKeKhai == 4)
            {
                if (LoaiKeKhai == 1) { LoaiKeKhai = EnumLoaiDotKeKhai.HangNam.GetHashCode(); }
                else if (LoaiKeKhai == 2) { LoaiKeKhai = EnumLoaiDotKeKhai.BoSung.GetHashCode(); }
                else if (LoaiKeKhai == 3) { LoaiKeKhai = EnumLoaiDotKeKhai.BoNhiem.GetHashCode(); }
                else if (LoaiKeKhai == 4) { LoaiKeKhai = EnumLoaiDotKeKhai.LanDau.GetHashCode(); }

                SqlParameter[] parameters = new SqlParameter[]
                    {
                new SqlParameter(CO_QUAN_ID , SqlDbType.Int),
                new SqlParameter("@CapQuanLy",SqlDbType.Int),
                new SqlParameter(NAM_KE_KHAI_DOTKEKHAI,SqlDbType.Int),
                  pList,
                   new SqlParameter("@LoaiKeKhai",SqlDbType.Int)
                    };
                parameters[0].Value = CoQuanID ?? Convert.DBNull;
                parameters[1].Value = CapQuanLy ?? Convert.DBNull;
                parameters[2].Value = NamKeKhai ?? Convert.DBNull;
                parameters[3].Value = tbCoQuanID;
                parameters[4].Value = LoaiKeKhai ?? Convert.DBNull;
                try
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, SELECT_CANBO_BY_COQUAN_AND_LOAIKEKHAI, parameters))
                    {
                        while (dr.Read())
                        {

                            HeThongCanBoShortModel = new HeThongCanBoShortModel();
                            HeThongCanBoShortModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                            HeThongCanBoShortModel.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                            HeThongCanBoShortModel.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            HeThongCanBoShortModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            HeThongCanBoShortModel.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            HeThongCanBoShortModel.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                            HeThongCanBoShortModel.TenDotKeKhai = Utils.ConvertToString(dr["NV00108"], string.Empty);
                            var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(HeThongCanBoShortModel.CanBoID);
                            HeThongCanBoShortModel.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            HeThongCanBoShortModel.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            ListCanBo.Add(HeThongCanBoShortModel);
                        }

                        dr.Close();
                    }
                }

                catch (Exception ex)
                {

                    throw ex;
                }


            }

            ListCanBo.OrderBy(x => x.CanBoID);
            return ListCanBo;
        }

        public List<HeThongCanBoShortModel> GetCanBoByCoQuanAndLoaiKeKhai_New(int? CoQuanID, int? NamKeKhai, int? CapQuanLy, int? LoaiKeKhai, int CoQuan_CuaCanBoDangNhap, int NguoiDungID)
        {
            List<HeThongCanBoShortModel> ListCanBo = new List<HeThongCanBoShortModel>();
            try
            {
                int loaiDotKeKhai = 0;
                int trangThai = 0; // Chua ke khai thì trang thái bằng 0; ngược laj đã kê khai bằng 1
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiHangNamChuaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.HangNam.GetHashCode();
                    trangThai = 1;
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiHangNamKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.HangNam.GetHashCode();
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiBoNhiemChuaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.BoNhiem.GetHashCode();
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiBoNhiemDaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.BoNhiem.GetHashCode();
                    trangThai = 1;
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiBoSungChuaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.BoSung.GetHashCode();
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiBoSungDaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.BoSung.GetHashCode();
                    trangThai = 1;
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiLanDauChuaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.LanDau.GetHashCode();
                }
                if (LoaiKeKhai == EnumLoaiKeKhai.KeKhaiLanDauDaKeKhai.GetHashCode())
                {
                    loaiDotKeKhai = EnumLoaiDotKeKhai.LanDau.GetHashCode();
                    trangThai = 1;
                }
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CoQuanID" , SqlDbType.Int),
                    new SqlParameter("LoaiDotKeKhai",SqlDbType.Int),
                    new SqlParameter("NamKeKhai",SqlDbType.Int),
                    new SqlParameter("TrangThai",SqlDbType.Int),

                };
                parameters[0].Value = CoQuanID ?? Convert.DBNull;
                parameters[1].Value = loaiDotKeKhai;
                parameters[2].Value = NamKeKhai ?? Convert.DBNull;
                parameters[3].Value = trangThai;
                try
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_ThongKeCanBoKeKhai_ByCoQuanID_LoaiDotKeKhai", parameters))
                    {
                        while (dr.Read())
                        {

                            HeThongCanBoShortModel HeThongCanBoShortModel = new HeThongCanBoShortModel();
                            HeThongCanBoShortModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                            HeThongCanBoShortModel.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                            HeThongCanBoShortModel.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            HeThongCanBoShortModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            HeThongCanBoShortModel.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            HeThongCanBoShortModel.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                            HeThongCanBoShortModel.TenDotKeKhai = Utils.ConvertToString(dr["NV00108"], string.Empty);
                            var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(HeThongCanBoShortModel.CanBoID);
                            HeThongCanBoShortModel.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            HeThongCanBoShortModel.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            ListCanBo.Add(HeThongCanBoShortModel);
                        }

                        dr.Close();
                    }
                }

                catch (Exception ex)
                {

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListCanBo;
        }

        // code cũ
        // Thống kê theo cơ quan
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeByCoQuanID(int? CoQuanID, int? CapID, int? CapQuanLy, int? CoQuanIDFilter, int? NamKeKhai, int? CoQuanCuaCanBoDangNhap, int NguoiDungID)
        {
            ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
            //var ListCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID.Value);
            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanCuaCanBoDangNhap);

            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan = new List<ThongKeChiTietKeKhaiTaiSan>();
            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
            }
            else if (listThanhTraTinh.Contains(CoQuanCuaCanBoDangNhap.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else if (listThanhTraHuyen.Contains(CoQuanCuaCanBoDangNhap.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }

            else
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID).ToList();
            }
            if (listCoQuanCon != null && listCoQuanCon.Count > 0)
            {
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            SqlParameter[] parameters = new SqlParameter[]
           {

                pList,
                new SqlParameter(CO_QUAN_ID,SqlDbType.Int),
                new SqlParameter("@CapQuanLy",SqlDbType.Int),
                new SqlParameter(NAM_KE_KHAI_DOTKEKHAI,SqlDbType.Int)

           };
            parameters[0].Value = tbCoQuanID;
            parameters[1].Value = CoQuanIDFilter ?? Convert.DBNull;
            parameters[2].Value = CapQuanLy ?? Convert.DBNull;
            parameters[3].Value = NamKeKhai ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, THONG_KE_CHI_TIET_TAI_SAN, parameters))
                {
                    while (dr.Read())
                    {
                        ThongKeChiTietKeKhaiTaiSan ThongKeChiTietKeKhaiTaiSan = new ThongKeChiTietKeKhaiTaiSan();
                        ThongKeChiTietKeKhaiTaiSan.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        ThongKeChiTietKeKhaiTaiSan.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        ThongKeChiTietKeKhaiTaiSan.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiHangNamDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoSungDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoNhiemDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoNhiemDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoSungChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiBoNhiemChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiHangNamChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai = Utils.ConvertToInt32(dr["KeKhaiLanDauChuaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai = Utils.ConvertToInt32(dr["KeKhaiLanDauDaKeKhai"], 0);
                        ThongKeChiTietKeKhaiTaiSan.TongSoChuaKeKhai = (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                        ThongKeChiTietKeKhaiTaiSan.TongSoDaKeKhai = (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                        ThongKeChiTietKeKhaiTaiSan.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count(); ;
                        //ThongKeChiTietKeKhaiTaiSanPar_New.TenCoQuan = ThongKeChiTietKeKhaiTaiSan.TenCoQuan;
                        if (ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet == null)
                        {
                            ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                        }
                        if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode()) // Cấp huyện
                        {
                            if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                            {
                                ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                        }
                        else if (CapID == EnumCapCoQuan.CapPhong.GetHashCode()) // Cấp phòng
                        {
                            if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                            {
                                ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                        }
                        else if (CapID == EnumCapCoQuan.CapSo.GetHashCode()) // Cấp sở
                        {
                            if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapSo.GetHashCode() || ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                            {
                                ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                        }
                        else if (CapID == EnumCapCoQuan.CapTinh.GetHashCode()) // Cấp tỉnh
                        {
                            if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                            {
                                ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                        }
                        else if (CapID == EnumCapCoQuan.CapXa.GetHashCode()) // cấp xã
                        {
                            if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapXa.GetHashCode() && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                            {
                                ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                            }
                        }

                        //ThongKeChiTietKeKhaiTaiSanPar_New.TongSoChuaKeKhai = (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                        //ThongKeChiTietKeKhaiTaiSanPar_New.TongSoDaKeKhai = (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                        //ListThongKeChiTietKeKhaiTaiSanPar.Add(ThongKeChiTietKeKhaiTaiSanPar_New);

                    }
                    dr.Close();
                }




            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ThongKeChiTietKeKhaiTaiSanPar_New;
        }

        // code mới -đang sử  dụng
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeBanKeKhaiTheoCoQuanCha(int? CoQuanID, int? CapID, int? CapQuanLy, int? CoQuanIDFilter, int? NamKeKhai, int? CoQuanCuaCanBoDangNhap, int NguoiDungID, List<ThongKeChiTietKeKhaiTaiSan> listData)
        {
            ThongKeChiTietKeKhaiTaiSanPar ThongKeChiTietKeKhaiTaiSanPar_New = new ThongKeChiTietKeKhaiTaiSanPar();
            //var ListCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID.Value);
            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanCuaCanBoDangNhap);

            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTietKeKhaiTaiSan = new List<ThongKeChiTietKeKhaiTaiSan>();
            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
            }
            else if (listThanhTraTinh.Contains(CoQuanCuaCanBoDangNhap.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else if (listThanhTraHuyen.Contains(CoQuanCuaCanBoDangNhap.Value))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).ToList();
            }
            else
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID).ToList();
            }
            if (listCoQuanCon != null && listCoQuanCon.Count > 0)
            {
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }

            try
            {
                var lst = listData.Where(x =>
                listCoQuanCon.Select(y => y.CoQuanID).ToList().Contains(x.CoQuanID.Value)
                && (CoQuanIDFilter == null || CoQuanIDFilter == 0 || x.CoQuanID == CoQuanIDFilter)
                && (CapQuanLy == null || CapQuanLy == 0 || x.CapID == CapQuanLy)
                ).ToList();
                for (int i = 0; i < lst.Count; i++)
                {

                    var ThongKeChiTietKeKhaiTaiSan = lst[i];
                    ThongKeChiTietKeKhaiTaiSan.TongSoChuaKeKhai = (ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamChuaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauChuaKeKhai);
                    ThongKeChiTietKeKhaiTaiSan.TongSoDaKeKhai = (ThongKeChiTietKeKhaiTaiSan.KeKhaiHangNamDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoSungDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiBoNhiemDaKeKhai + ThongKeChiTietKeKhaiTaiSan.KeKhaiLanDauDaKeKhai);
                    //ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).GroupBy(x => x.DotKeKhaiID).Count(); ;
                    var TongSoDotKeKhai = new DotKeKhaiDAL().DotKeKhaiDonViCanBo_GetBy_CoQuanID(ThongKeChiTietKeKhaiTaiSan.CoQuanID.Value).ToList();
                    if (NamKeKhai != null && TongSoDotKeKhai.Count > 0)
                    {
                        ThongKeChiTietKeKhaiTaiSan.TongSoDotKeKhai = TongSoDotKeKhai.Where(x => x.NamKeKhai == NamKeKhai).ToList().GroupBy(x => x.DotKeKhaiID).Count();
                    }
                    if (ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet == null)
                    {
                        ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet = new List<ThongKeChiTietKeKhaiTaiSan>();
                    }
                    if (CapID == EnumCapCoQuan.CapHuyen.GetHashCode()) // Cấp huyện
                    {
                        if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                        {
                            ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                    }
                    else if (CapID == EnumCapCoQuan.CapPhong.GetHashCode()) // Cấp phòng
                    {
                        if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                        {
                            ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                    }
                    else if (CapID == EnumCapCoQuan.CapSo.GetHashCode()) // Cấp sở
                    {
                        if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapSo.GetHashCode() || ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapPhong.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                        {
                            ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                    }
                    else if (CapID == EnumCapCoQuan.CapTinh.GetHashCode()) // Cấp tỉnh
                    {
                        if ((ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapSo.GetHashCode()) && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                        {
                            ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                    }
                    else if (CapID == EnumCapCoQuan.CapXa.GetHashCode()) // cấp xã
                    {
                        if (ThongKeChiTietKeKhaiTaiSan.CapID == EnumCapCoQuan.CapXa.GetHashCode() && (ThongKeChiTietKeKhaiTaiSan.CoQuanID == CoQuanID || ThongKeChiTietKeKhaiTaiSan.CoQuanChaID == CoQuanID))
                        {
                            ThongKeChiTietKeKhaiTaiSanPar_New.ListThongKeChiTiet.Add(ThongKeChiTietKeKhaiTaiSan);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ThongKeChiTietKeKhaiTaiSanPar_New;
        }

        // Thông báo dashboard
        public List<NhacViecModel> Dasboard_Notification(int? NguoiDungID, int? CanBoID, int? CoQuanID)
        {
            var DuyetCongKhai = "duyet-cong-khai-ban-ke-khai";
            var QuanLyBanKeKhai = "quan-ly-ban-ke-khai";
            var keKhaiTaiSan = "ke-khai-tai-san";
            var BackUpDB = "quan-tri-du-lieu";
            var QuanLyDotKeKhai = "quan-ly-dot-ke-khai";
            var CongKhaiBanKeKhai = "cong-khai-ban-ke-khai";
            BackupService BackupService = new BackupService();
            List<NhacViecModel> ListThongBao = new List<NhacViecModel>();
            ListThongBao = new NhacViecDAL().GetViecLam(NguoiDungID, CanBoID, CoQuanID.Value);
            if (UserRole.CheckAdmin(NguoiDungID.Value))
            {

            }
            else
            {
                var GetAllKeKhaiByCanBoID = new KeKhaiDAL().GetList_By_CanBoID(CanBoID.Value);
                var KeKhaiChuaNhacViec = GetAllKeKhaiByCanBoID.Where(x => x.TrangThaiNhacViec == true).ToList();
                var KeKhaiChuaPheDuyetLan1 = KeKhaiChuaNhacViec.Where(x => x.TrangThai == 101 /*|| x.TrangThai == 201*/).ToList();
                var KeKhaiChuaPheDuyetLan2 = KeKhaiChuaNhacViec.Where(x => x.TrangThai == 201 /*|| x.TrangThai == 201*/).ToList();
                var KeKhaiPheDuyetLan1 = KeKhaiChuaNhacViec.Where(x => x.TrangThai == 300).ToList();
                var KeKhaiPheDuyetLan2 = KeKhaiChuaNhacViec.Where(x => x.TrangThai == 400).ToList();
                if (KeKhaiChuaNhacViec.Count > 0 && KeKhaiChuaPheDuyetLan1.Count > 0)
                {
                    foreach (var item in KeKhaiChuaPheDuyetLan1)
                    {
                        NhacViecModel NhacViecModel = new NhacViecModel();
                        NhacViecModel.Name = item.TenBanKeKhai + " chưa được duyệt lần 1";
                        NhacViecModel.Key = keKhaiTaiSan;
                        ListThongBao.Add(NhacViecModel);
                    }
                }
                else if (KeKhaiChuaNhacViec.Count > 0 && KeKhaiChuaPheDuyetLan2.Count > 0)
                {
                    foreach (var item in KeKhaiChuaPheDuyetLan2)
                    {
                        NhacViecModel NhacViecModel = new NhacViecModel();
                        NhacViecModel.Name = item.TenBanKeKhai + " chưa được duyệt lần 2";
                        NhacViecModel.Key = QuanLyBanKeKhai;
                        ListThongBao.Add(NhacViecModel);
                    }
                }
                else if (KeKhaiChuaNhacViec.Count > 0 && KeKhaiPheDuyetLan1.Count > 0)
                {
                    foreach (var item in KeKhaiPheDuyetLan1)
                    {
                        NhacViecModel NhacViecModel = new NhacViecModel();
                        NhacViecModel.Name = item.TenBanKeKhai + " đã được phê duyệt lần 1";
                        NhacViecModel.Key = QuanLyBanKeKhai;
                        ListThongBao.Add(NhacViecModel);
                    }
                }
                else if (KeKhaiChuaNhacViec.Count > 0 && KeKhaiPheDuyetLan2.Count > 0)
                {
                    foreach (var item in KeKhaiPheDuyetLan2)
                    {
                        var GetCongKhaiBanKeKhai = new CongKhaiBanKeKhaiDAL().CongKhaiBanKeKhai_GetBy_CanBoIDAndKeKhaiID(CanBoID, item.KeKhaiID).ToList().Where(x => x.TrangThaiCongKhai == 1 || x.TrangThaiCongKhai == 2).ToList();
                        if (GetCongKhaiBanKeKhai.Count <= 0)
                        {
                            NhacViecModel NhacViecModel = new NhacViecModel();
                            NhacViecModel.Name = item.TenBanKeKhai + " đã được phê duyệt lần 2";
                            NhacViecModel.Key = QuanLyBanKeKhai;
                            ListThongBao.Add(NhacViecModel);
                        }
                    }
                }
                foreach (var item in GetAllKeKhaiByCanBoID)
                {
                    var GetDuyetKeKhaiByKeKhaiID = new CongKhaiBanKeKhaiDAL().CongKhaiBanKeKhai_GetBy_CanBoIDAndKeKhaiID(item.CanBoID, item.KeKhaiID);
                    var BanKeKhaiChuaCongKhai = GetDuyetKeKhaiByKeKhaiID.Where(x => x.TrangThai == 400 && x.TrangThaiCongKhai == 2 && x.TrangThaiNhacViec == true).ToList();
                    var BanKeKhaiDaCongKhai = GetDuyetKeKhaiByKeKhaiID.Where(x => x.TrangThai == 500 && x.TrangThaiCongKhai == 1 && x.TrangThaiNhacViec == true).ToList();
                    if (BanKeKhaiChuaCongKhai.Count > 0)
                    {
                        foreach (var item1 in BanKeKhaiChuaCongKhai)
                        {
                            NhacViecModel NhacViecModel = new NhacViecModel();
                            NhacViecModel.Name = item1.TenBanKeKhai + " chưa được duyệt công khai";
                            NhacViecModel.Key = DuyetCongKhai;
                            ListThongBao.Add(NhacViecModel);


                        }
                    }
                    if (BanKeKhaiDaCongKhai.Count > 0)
                    {
                        foreach (var item1 in BanKeKhaiDaCongKhai)
                        {
                            NhacViecModel NhacViecModel = new NhacViecModel();
                            NhacViecModel.Name = item1.TenBanKeKhai + " đã được duyệt công khai";
                            NhacViecModel.Key = CongKhaiBanKeKhai;
                            ListThongBao.Add(NhacViecModel);


                        }
                    }
                }
                var DotKeKhaiByCanBo = new DotKeKhaiDAL().GetDotKeKhaiByCanBoID(CanBoID.Value).Where(x => x.TrangThai == true).ToList();
                List<KeKhaiModel> ListKeKhai = new List<KeKhaiModel>();
                DotKeKhaiByCanBo.ForEach(x => ListKeKhai.Add(new KeKhaiDAL().GetByDotKeKhaiIDAndCanBoID(x.DotKeKhaiID, x.CanBoID.Value)));
                ListKeKhai = ListKeKhai.Where(x => x.KeKhaiID > 0).ToList();
                var ChucNangByNguoiDungID = new ChucNangDAL().GetListChucNangByNguoiDungID(NguoiDungID.Value);
                var QuyenXemQuanLyDotKeKhai = ChucNangByNguoiDungID.Where(x => x.MaChucNang == "quan-ly-dot-ke-khai" && x.Quyen >= 1).ToList();
                if (ListKeKhai.Count < DotKeKhaiByCanBo.Count)
                {
                    var DotKeKhaiCuoiCung = DotKeKhaiByCanBo.OrderByDescending(x => x.DotKeKhaiID).FirstOrDefault();
                    TimeSpan time = DotKeKhaiCuoiCung.DenNgay.Subtract(DateTime.Now);
                    if (time.TotalDays <= int.Parse(new SystemConfigDAL().GetByKey("Alert_Exdate").ConfigValue) && QuyenXemQuanLyDotKeKhai.Count > 0)
                    {
                        NhacViecModel NhacViecModel = new NhacViecModel();
                        NhacViecModel.Name = "Có đợt kê khai sắp hết hạn";
                        NhacViecModel.Key = QuanLyDotKeKhai;
                        ListThongBao.Add(NhacViecModel);
                    }

                }
            }
            return ListThongBao;


        }

        public ThongKeGuiThanhTraTinhModel ThongKeGuiThanhTraTinh(int CanBoID, int CoQuanID, int NguoiDungID)
        {
            ThongKeGuiThanhTraTinhModel Result = new ThongKeGuiThanhTraTinhModel();
            var pList = new SqlParameter("@DanhSachCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            tbCoQuanID.Rows.Add(CoQuanID);
            ////var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            //listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCoQuanConDaCap(CoQuanID).ToList();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(0).Where(x => x.CoQuanID != CoQuanID).ToList();
            }
            listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            SqlParameter[] parameters = new SqlParameter[]
              {
                    pList,
                  new SqlParameter("@TongSoKeKhai",SqlDbType.Int),
                  new SqlParameter("@DaKeKhai",SqlDbType.Int),
                  new SqlParameter("@ChuaKeKhai",SqlDbType.Int),
                  new SqlParameter("@TongDaGui",SqlDbType.Int),
              };
            //param TrangThai là 0: chưa gửi hoặc 1: đã gửi
            parameters[0].Value = tbCoQuanID;
            parameters[1].Direction = ParameterDirection.Output;
            parameters[1].Size = 8;
            parameters[2].Direction = ParameterDirection.Output;
            parameters[2].Size = 8;
            parameters[3].Direction = ParameterDirection.Output;
            parameters[3].Size = 8;
            parameters[4].Direction = ParameterDirection.Output;
            parameters[4].Size = 8;
            List<HeThongCanBoThongKeModel> DanhSachCanBo = new List<HeThongCanBoThongKeModel>();
            try
            {
                var list = new List<ThongKeBanKeKhaiPartialModel>();
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_ThongKeSoLuong_BanKeKhai", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoThongKeModel CanBo = new HeThongCanBoThongKeModel();
                        CanBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        CanBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        CanBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        CanBo.TrangThaiBanKeKhai = Utils.ConvertToInt32(dr["TrangThaiBanKeKhai"], 0);
                        DanhSachCanBo.Add(CanBo);
                    }
                    dr.Close();
                }
                Result.TongSo = Utils.ConvertToInt32(parameters[1].Value, 0);
                Result.DaKeKhai = Utils.ConvertToInt32(parameters[2].Value, 0);
                Result.ChuaKeKhai = Utils.ConvertToInt32(parameters[3].Value, 0);
                Result.DaGui = Utils.ConvertToInt32(parameters[4].Value, 0);
                Result.DanhSachCanBoChuaKeKhai = DanhSachCanBo.Where(x => x.TrangThaiBanKeKhai == 0).ToList();
                Result.DanhSachCanBoDaKeKhai = DanhSachCanBo.Where(x => x.TrangThaiBanKeKhai != 0).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

    }
}
