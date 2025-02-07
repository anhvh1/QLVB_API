using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.BaoCao;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{

    public interface IQuanLyBanKeKhaiDAL
    {
        public QuanLyBanKeKhaiModel GetQuanLyBanKeKhai(BasePagingParamsForFilter p, int CoQuanID, int CanBoID);
        public QuanLyBanKeKhaiModel GetPagingBySearch_BanKeKhaiThanhTraTinh(BasePagingParamsForFilter p, int CoQuanID, int CanBoID);
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow);
        public DataTable ThongTinTaiSan_ThongKeBienDongTaiSan(int CanBoID, int? NhomTaiSanID);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_KeKhaiID(int KeKhaiID);
        public BaseResultModel DuyetBanKeKhai(DuyetBanKeKhaiModel DuyetBanKeKhaiModel, int CanBoID, int CoQuanID);
        public BaseResultModel DuyetBanKeKhai_Multil(List<int> DanhSachKeKhaiID, int CanBoID, int CoQuanID);
        public DuyetBanKeKhaiModelPar GetDuyetBanKeKhaiByKeKhaiID(int? KeKhaiID);
        public BaseResultModel DuyetVaGuiBanKeKhai(GuiKeKhaiModel guiKeKhai, int? CanBoID);
        public BaseResultModel KeKhaiLai_BanKeKhai(DuyetBanKeKhaiModel DuyetBanKeKhaiModel, int CanBoID, int CoQuanID);
        public BaseResultModel ThanhTraTinhTiepNhanBanKeKhai(GuiKeKhaiModel guiKeKhai, int? CoQuanID, int? CanBoID);
        public BaseResultModel KyVaDuyetKeKhai(int CanBoDuyetID, FileDinhKemModel DataKyDuyet);
        public BaseResultModel UploadFileKySo(int CanBoDuyetID, FileDinhKemModel DataKyDuyet);
    }
    public class QuanLyBanKeKhaiDAL : IQuanLyBanKeKhaiDAL
    {
        //tên các store procedure
        private const string KEKHAI_QUANLYKEKHAI_GETPAGINGBYSEARCH = @"v1_KeKhai_QuanLyBanKeKhai_GetPagingBySearch";
        private const string KEKHAI_QUANLYKEKHAI_GETLISTQUANLYBANKEKHAI = @"v1_KeKhai_QuanLyBanKeKhai_GetListQuanLyBanKeKhai";
        private const string QUANLYKEKHAI_GETPAGINGBYSEARCH = @"v1_QuanLyDotKeKhai_GetPagingBySearch";
        private const string KEKHAI_DUYETBANKEKHAI = @"v1_KeKhai_DuyetBanKeKhai";
        private const string KEKHAI_DUYETBANKEKHAI_MULTIL = @"v1_KeKhai_DuyetBanKeKhai_Multil";
        private const string KEKHAI_DUYETBANKEKHAI_GET_BY_KEKHAIID = @"v1_KeKhai_GetDuyetBanKeKhaiByKeKhaiID";

        //Ten các params
        private const string KE_KHAI_ID = "NV00301";
        private const string DOT_KE_KHAI_ID = "NV00302";
        private const string CAN_BO_ID = "NV00303";
        private const string NAM = "NV00304";
        private const string TRANG_THAI = "NV00305";
        private const string TEN_BAN_KE_KHAI = "NV00306";
        private const string BIEN_DONG = "NV00307";
        private const string TRANG_THAI_NHAC_VIEC_DASBOARD = "NV00308";
        private const string TRANG_THAI_CONG_KHAI = "NV00309";
        private const string LOAI_DOT_KE_KHAI = "NV00106";
        private const string DUYET_BAN_KE_KHAI_ID = "NV00501";
        private const string NGUOI_DUYET_ID = "NV00502";
        private const string NGAY_DUYET = "NV00503";
        private const string GHI_CHU = "NV00504";
        private const string DUYETBANKEKHAI_KEKHAIID = "NV00505";
        private const string PHE_DUYET = "NV00506";
        private const string TRANG_THAI_DUYET = "NV00507";
        private const string TRANG_THAI_NHAC_VIEC = "NV00508";
        private const string NGAY_HET_HAN = "NV00509";

        //Edit by ChungNN 1/4/2021

        public QuanLyBanKeKhaiModel GetQuanLyBanKeKhai(BasePagingParamsForFilter p, int CoQuanID, int CanBoID)
        {
            var Result = new QuanLyBanKeKhaiModel();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));

            //table param DotKeKhaiID 
            var pListDotKeKhai = new SqlParameter("@DanhSachDotKeKhaiID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbDotKeKhaiID = new DataTable();
            tbDotKeKhaiID.Columns.Add("ID", typeof(string));
            List<int> DanhSachDotKeKhaiID = new List<int>();
            if (!string.IsNullOrEmpty(p.DotKeKhaiID))
            {
                DanhSachDotKeKhaiID = p.DotKeKhaiID.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
            }
            DanhSachDotKeKhaiID.ForEach(x => tbDotKeKhaiID.Rows.Add(x));

            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var coQuanChaCuaCoQuanDangNhap = new DanhMucCoQuanDonViDAL().GetByID(crCoQuan.CoQuanChaID);
            List<DanhMucCoQuanDonViModel> DanhSachCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            List<int> DanhSachCoQuanID = new List<int>();
            List<HeThongCanBoModel> DanhSachCanBo = new List<HeThongCanBoModel>();
            //check thanh tra tinh
            //var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
            //if (laThanhTraTinh)
            //{
            //    DanhSachCanBo = new HeThongCanBoDAL().GetAll();
            //    DanhSachCanBo.Where(x=>x.CanBoID != 1).ToList().ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //}
            //else
            //{
            if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())
            {
                DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCoQuanConDaCap(crCoQuan.CoQuanID);
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && coQuanChaCuaCoQuanDangNhap.CapID == EnumCapCoQuan.CapSo.GetHashCode())
            {
                DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCoQuanConDaCap(coQuanChaCuaCoQuanDangNhap.CoQuanID);
            }
            else
            {
                DanhSachCoQuanCon.Add(crCoQuan);
            }
            DanhSachCoQuanCon.ForEach(x => DanhSachCoQuanID.Add(x.CoQuanID));
            DanhSachCanBo = new HeThongCanBoDAL().GetAllByListCoQuanID(DanhSachCoQuanID);
            DanhSachCanBo.Where(x => x.CanBoID != 1).ToList().ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //}
            SqlParameter[] parameters = new SqlParameter[]
              {
                pList,
                pListDotKeKhai,
                new SqlParameter("@CanBoID_Filter",SqlDbType.Int),
                new SqlParameter("@CanBoIDDangNhap",SqlDbType.Int),
                new SqlParameter("@TongSoKeKhai",SqlDbType.Int),
                new SqlParameter("@DaKeKhai",SqlDbType.Int),
                new SqlParameter("@ChuaKeKhai",SqlDbType.Int),
                new SqlParameter("@DaGui",SqlDbType.Int),
                new SqlParameter("@ChuaGui",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
              };
            parameters[0].Value = tbCanBoID;
            parameters[1].Value = tbDotKeKhaiID;
            parameters[2].Value = p.CanBoID ?? Convert.DBNull;
            parameters[3].Value = CanBoID;
            parameters[4].Direction = ParameterDirection.Output;
            parameters[4].Size = 8;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;
            parameters[7].Direction = ParameterDirection.Output;
            parameters[7].Size = 8;
            parameters[8].Direction = ParameterDirection.Output;
            parameters[8].Size = 8;
            parameters[9].Value = p.NamKeKhai;
            parameters[10].Value = p.TrangThai;

            List<KeKhaiModelPartial> DanhSachQuanLyBanKeKhai = new List<KeKhaiModelPartial>();
            Result.DanhSach = new List<KeKhaiModelPartial>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_QuanLyBanKeKhai_LanhDaoDonVi_New", parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr["KekhaiID"], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr["TenBanKeKhai"], string.Empty);
                        item.TrangThai = Utils.ConvertToInt32(dr["TrangThaiBanKeKhai"], 0);
                        item.TenTrangThai = new ThongTinTaiSanDAL().getTenTrangThai(item.TrangThai);
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        item.ChucVuStr = Utils.ConvertToString(dr["DanhSachChucVu"], string.Empty);
                        item.SoCongVan = Utils.ConvertToString(dr["SoCongVan"], string.Empty);
                        FileDinhKemModel FileKekhai = new FileDinhKemModel();
                        FileKekhai.FileID = Utils.ConvertToInt32(dr["FileKeKhai"], 0);
                        item.DanhSachFileDinhKem = new List<FileDinhKemModel>();
                        if (FileKekhai.FileID > 0)
                        {
                            item.DanhSachFileDinhKem.Add(FileKekhai);
                        }

                        FileDinhKemModel FileDuyetVaGui = new FileDinhKemModel();
                        FileDuyetVaGui.FileID = Utils.ConvertToInt32(dr["FileDuyetVaGui"], 0);
                        item.DanhSachFileDuyetDinhKem = new List<FileDinhKemModel>();
                        if (FileDuyetVaGui.FileID > 0)
                        {
                            item.DanhSachFileDuyetDinhKem.Add(FileDuyetVaGui);
                        }

                        FileDinhKemModel FileKekhaiLai = new FileDinhKemModel();
                        FileKekhaiLai.FileID = Utils.ConvertToInt32(dr["FileDuyetKeKhai"], 0);
                        item.DanhSachFileCongVan = new List<FileDinhKemModel>();
                        if (FileKekhaiLai.FileID > 0)
                        {
                            item.DanhSachFileCongVan.Add(FileKekhaiLai);
                        }
                        item.TrangThaiFilter = p.TrangThai;
                        Result.DanhSach.Add(item);
                    }
                    dr.Close();
                }
                Result.TongSoCanKeKhai = Utils.ConvertToInt32(parameters[4].Value, 0);
                Result.DaKeKhai = Utils.ConvertToInt32(parameters[5].Value, 0);
                Result.ChuaKeKhai = Utils.ConvertToInt32(parameters[6].Value, 0);
                Result.DaGui = Utils.ConvertToInt32(parameters[7].Value, 0);
                Result.ChuaGui = Utils.ConvertToInt32(parameters[8].Value, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public QuanLyBanKeKhaiModel GetPagingBySearch_BanKeKhaiThanhTraTinh(BasePagingParamsForFilter p, int CoQuanID, int CanBoID)
        {
            var Result = new QuanLyBanKeKhaiModel();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));

            //List<HeThongCanBoModel> DanhSachCanBo = new List<HeThongCanBoModel>();
            //if (p.CanBoID > 0)
            //{
            //    tbCanBoID.Rows.Add(p.CanBoID);
            //}
            //else if (p.CoQuanID > 0)
            //{
            //    var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCoQuanConDaCap(p.CoQuanID).Select(x => x.CoQuanID).ToList();
            //    DanhSachCanBo = new HeThongCanBoDAL().GetAllByListCoQuanID(listCoQuan);
            //    DanhSachCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //}
            //else
            //{
            //    DanhSachCanBo = new HeThongCanBoDAL().GetAll();
            //    DanhSachCanBo.Where(x => x.CanBoID != 1).ToList().ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //}
            SqlParameter[] parameters = new SqlParameter[]
              {
                pList,
                new SqlParameter("@TrangThai",SqlDbType.Int),
                new SqlParameter("@CanBoID_Filter",SqlDbType.Int),
                new SqlParameter("@CanBoIDDangNhap",SqlDbType.Int),
                new SqlParameter("@TongSoKeKhai",SqlDbType.Int),
                new SqlParameter("@DaGui",SqlDbType.Int),
                new SqlParameter("@ChuaGui",SqlDbType.Int),
                new SqlParameter("@DaTiepNhan",SqlDbType.Int),
                new SqlParameter("@ChuaTiepNhan",SqlDbType.Int),
                new SqlParameter("@Nam",SqlDbType.Int),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("@CoQuanID_Filter",SqlDbType.Int),
              };
            parameters[0].Value = tbCanBoID;
            parameters[1].Value = p.TrangThai;
            parameters[2].Value = p.CanBoID ?? Convert.DBNull;
            parameters[3].Value = CanBoID;
            parameters[4].Direction = ParameterDirection.Output;
            parameters[4].Size = 8;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;
            parameters[7].Direction = ParameterDirection.Output;
            parameters[7].Size = 8;
            parameters[8].Direction = ParameterDirection.Output;
            parameters[8].Size = 8;
            parameters[9].Value = p.NamKeKhai ?? Convert.DBNull;
            parameters[10].Value = p.Limit;
            parameters[11].Value = p.Offset;
            parameters[12].Value = p.CoQuanID ?? Convert.DBNull;

            List<KeKhaiModelPartial> DanhSachQuanLyBanKeKhai = new List<KeKhaiModelPartial>();
            Result.DanhSach = new List<KeKhaiModelPartial>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_QuanLyBanKeKhai_ThanhTraTinh_New", parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr["KekhaiID"], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr["TenBanKeKhai"], string.Empty);
                        item.TrangThai = Utils.ConvertToInt32(dr["TrangThaiBanKeKhai"], 0);
                        item.TenTrangThai = new ThongTinTaiSanDAL().getTenTrangThai(item.TrangThai);
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        item.ChucVuStr = Utils.ConvertToString(dr["DanhSachChucVu"], string.Empty);
                        item.SoCongVan = Utils.ConvertToString(dr["SoCongVan"], string.Empty);
                        FileDinhKemModel FileKekhai = new FileDinhKemModel();
                        FileKekhai.FileID = Utils.ConvertToInt32(dr["FileKeKhai"], 0);
                        item.DanhSachFileDinhKem = new List<FileDinhKemModel>();
                        if (FileKekhai.FileID > 0)
                        {
                            item.DanhSachFileDinhKem.Add(FileKekhai);
                        }

                        FileDinhKemModel FileDuyetVaGui = new FileDinhKemModel();
                        FileDuyetVaGui.FileID = Utils.ConvertToInt32(dr["FileDuyetVaGui"], 0);
                        item.DanhSachFileDuyetDinhKem = new List<FileDinhKemModel>();
                        if (FileDuyetVaGui.FileID > 0)
                        {
                            item.DanhSachFileDuyetDinhKem.Add(FileDuyetVaGui);
                        }

                        FileDinhKemModel FileKekhaiLai = new FileDinhKemModel();
                        FileKekhaiLai.FileID = Utils.ConvertToInt32(dr["FileDuyetKeKhai"], 0);
                        item.DanhSachFileCongVan = new List<FileDinhKemModel>();
                        if (FileKekhaiLai.FileID > 0)
                        {
                            item.DanhSachFileCongVan.Add(FileKekhaiLai);
                        }
                        item.TrangThaiFilter = p.TrangThai;
                        Result.DanhSach.Add(item);
                    }
                    dr.Close();
                }
                Result.TongSoCanKeKhai = Utils.ConvertToInt32(parameters[4].Value, 0);
                Result.DaGui = Utils.ConvertToInt32(parameters[5].Value, 0);
                Result.ChuaGui = Utils.ConvertToInt32(parameters[6].Value, 0);
                Result.DaTiepNhan = Utils.ConvertToInt32(parameters[7].Value, 0);
                Result.ChuaTiepNhan = Utils.ConvertToInt32(parameters[8].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow)
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();

            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var TrangThai = 200;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var coQuanChaCuaCoQuanDangNhap = new DanhMucCoQuanDonViDAL().GetByID(crCoQuan.CoQuanChaID);
            List<DanhMucCoQuanDonViModel> DanhSachCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            List<int> DanhSachCoQuanID = new List<int>();
            List<HeThongCanBoModel> DanhSachCanBo = new List<HeThongCanBoModel>();
            //check thanh tra tinh
            var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
            if (laThanhTraTinh)
            {
                TrangThai = 300;
                DanhSachCanBo = new HeThongCanBoDAL().GetAll();
                DanhSachCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            }
            else
            {
                TrangThai = 200;
                if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {
                    DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCoQuanConDaCap(crCoQuan.CoQuanID);
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode() && coQuanChaCuaCoQuanDangNhap.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {
                    DanhSachCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCoQuanConDaCap(coQuanChaCuaCoQuanDangNhap.CoQuanID);
                }
                else
                {
                    DanhSachCoQuanCon.Add(crCoQuan);
                }
                DanhSachCoQuanCon.ForEach(x => DanhSachCoQuanID.Add(x.CoQuanID));
                DanhSachCanBo = new HeThongCanBoDAL().GetAllByListCoQuanID(DanhSachCoQuanID);
                DanhSachCanBo.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter(NAM,SqlDbType.Int),
                new SqlParameter("@TrangThai",SqlDbType.Int),
                pList,
                new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                new SqlParameter("@CanBoIDDangNhap",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.NamKeKhai ?? Convert.DBNull;
            parameters[7].Value = TrangThai;
            parameters[8].Value = tbCanBoID;
            parameters[9].Value = p.LoaiDotKeKhai ?? Convert.DBNull;
            parameters[10].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_KeKhai_QuanLyBanKeKhai_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TenTrangThai = new ThongTinTaiSanDAL().getTenTrangThai(Utils.ConvertToInt32(dr[TRANG_THAI], 0));
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0);
                        item.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        item.LoaiDotKeKhaiID = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
                        {
                            item.TenLoai = "Kê khai bổ nhiệm";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.BoSung.GetHashCode())
                        {
                            item.TenLoai = "Kê khai bổ sung";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.HangNam.GetHashCode())
                        {
                            item.TenLoai = "Kê khai hàng năm";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.LanDau.GetHashCode())
                        {
                            item.TenLoai = "Kê khai lần đầu";
                        }
                        if (laThanhTraTinh)
                        {

                            if (item.TrangThai == 310)
                            {
                                item.isPheDuyet = true;
                                item.TenTrangThai = "Chưa tiếp nhận";
                            }
                            if (item.TrangThai == 400)
                            {
                                item.TenTrangThai = "Đã tiếp nhận";
                                item.isPheDuyet = false;
                            }
                            list.Add(item);
                        }
                        else
                        {
                            if (item.TrangThai == 200)
                            {
                                item.isPheDuyet = true;
                                item.TenTrangThai = "Chưa tiếp nhận";
                            }
                            if (item.TrangThai == 300)
                            {
                                item.TenTrangThai = "Đã tiếp nhận";
                                item.isPheDuyet = false;
                            }
                            list.Add(item);
                        }
                        var DuyetKeKhai = GetDuyetBanKeKhaiByKeKhaiID(item.KeKhaiID);
                        item.DanhSachFileDinhKem = new List<FileDinhKemModel>();
                        var DanhSachFileDinhKem = new FileDinhKemDAL().GetAllField_FileDinhKem_ByNghiepVuID_AndType(item.KeKhaiID, EnumLoaiFileDinhKem.FileKeKhai.GetHashCode()).Where(x => x.TrangThai > 0).ToList();
                        if (DuyetKeKhai != null && DuyetKeKhai.DuyetBanKeKhaiID > 0)
                        {
                            item.DanhSachFileDuyetDinhKem = new List<FileDinhKemModel>();
                            var DanhSachFileDuyetDinhKem = new FileDinhKemDAL().GetAllField_FileDinhKem_ByNghiepVuID_AndType(DuyetKeKhai.DuyetBanKeKhaiID, EnumLoaiFileDinhKem.FileDuyetKeKhai.GetHashCode()).Where(x => x.TrangThai > 0).ToList();
                            if (DanhSachFileDuyetDinhKem.Count > 0)
                            {
                                item.DanhSachFileDuyetDinhKem.Add(DanhSachFileDuyetDinhKem[0]);
                            }
                        }
                        if (DanhSachFileDinhKem.Count > 0)
                        {
                            item.DanhSachFileDinhKem.Add(DanhSachFileDinhKem[0]);
                        }
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

        public List<KeKhaiModelPartial> GetPagingBySearch_old(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow)
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();

            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var GiaTri = 200;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 2;
            var CoQuanQuanLy = 0;
            //check thanh tra tinh
            var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
            if (p.CanBoID == null || p.CanBoID < 1)
            {
                if (UserRole.CheckAdmin(CanBoID) || laThanhTraTinh)
                {
                    GiaTri = 200;
                    CapQuanLy = 0;
                    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                    else
                        CoQuanQuanLy = 0;

                }
                //else if (laThanhTraTinh)
                //{
                //    GiaTri = 300;
                //    CapQuanLy = 0;
                //    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                //    else
                //        CoQuanQuanLy = 0;
                //}
                else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
                {

                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
                {
                    GiaTri = 200;
                    CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                    else CoQuanQuanLy = 0;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
                {
                    GiaTri = 300;
                    CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                    else
                        CoQuanQuanLy = 0;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
                {
                    GiaTri = 200;
                    CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                    else
                        CoQuanQuanLy = crCoQuan.CoQuanID;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
                {

                    GiaTri = 300;
                    CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                    else
                        CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
                {
                    GiaTri = 200;
                    CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                    if (p.CoQuanID != null && p.CoQuanID > 0) CoQuanQuanLy = p.CoQuanID.Value;
                    else
                        CoQuanQuanLy = crCoQuan.CoQuanID;
                }
                var listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy);
                listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));

            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                new SqlParameter(NAM,SqlDbType.Int),
                new SqlParameter("@GiaTri",SqlDbType.Int),
                pList,
                new SqlParameter(TRANG_THAI,SqlDbType.Int),
                new SqlParameter("@CanBoIDDangNhap",SqlDbType.Int),
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.CanBoID ?? Convert.DBNull;
            parameters[7].Value = p.NamKeKhai ?? Convert.DBNull;
            parameters[8].Value = GiaTri;
            parameters[9].Value = tbCanBoID;
            parameters[10].Value = p.TrangThai ?? Convert.DBNull;
            parameters[11].Value = CanBoID;
            var listThanhTraTinh = new List<int>();
            try
            {
                listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
                var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_QUANLYKEKHAI_GETPAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TenTrangThai = new ThongTinTaiSanDAL().getTenTrangThai(Utils.ConvertToInt32(dr[TRANG_THAI], 0));
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0);
                        item.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        item.LoaiDotKeKhaiID = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
                        {
                            item.TenLoai = "Kê khai bổ nhiệm";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.BoSung.GetHashCode())
                        {
                            item.TenLoai = "Kê khai bổ sung";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.HangNam.GetHashCode())
                        {
                            item.TenLoai = "Kê khai hàng năm";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.LanDau.GetHashCode())
                        {
                            item.TenLoai = "Kê khai lần đầu";
                        }
                        if (UserRole.CheckAdmin(CanBoID) || laThanhTraTinh)
                        {
                            item.isPheDuyet = false;
                            if (item.TrangThai < 300 && item.TrangThai >= 200) item.TenTrangThai = "Chưa tiếp nhận";
                            else if (item.TrangThai == 300) item.TenTrangThai = "Đã tiếp nhận";
                            else if (item.TrangThai >= 400) item.TenTrangThai = "Đã tiếp nhận";
                            if (laThanhTraTinh)
                            {
                                if (item.TrangThai == 200)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 201)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê Khai lại";
                                }
                                else
                                {
                                    item.TenTrangThai = "Đã tiếp nhận";
                                    item.isPheDuyet = false;
                                }
                            }
                            list.Add(item);
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
                        {
                            list.Add(item);
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
                        {
                            if (crCoQuan.CoQuanID == item.CoQuanID)
                            {
                                if (item.TrangThai == 200)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 201)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                else
                                {
                                    item.TenTrangThai = "Đã tiếp nhận";
                                    item.isPheDuyet = false;
                                }
                                list.Add(item);
                            }

                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
                        {
                            if (crCoQuan.CoQuanID == item.CoQuanID)
                            {
                                if (item.TrangThai == 200)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 201)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                else if (item.TrangThai == 300)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";

                                }
                                else if (item.TrangThai == 301)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }

                                list.Add(item);

                            }
                            else
                            {
                                if (listThanhTraTinh.Contains(crCoQuan.CoQuanID) && item.TrangThai >= 300 && item.CapQuanLy == EnumCapQuanLyCanBo.CapTinh.GetHashCode())
                                {
                                    if (item.TrangThai == 300)
                                    {
                                        item.isPheDuyet = true;
                                        item.TenTrangThai = "Chưa tiếp nhận";
                                    }
                                    else if (item.TrangThai == 301)
                                    {
                                        item.isPheDuyet = true;
                                        item.TenTrangThai = "Kê khai lại";
                                    }
                                    else
                                    {
                                        item.isPheDuyet = false;
                                        item.TenTrangThai = "Đã tiếp nhận";
                                    }
                                    list.Add(item);
                                }

                            }

                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
                        {
                            if (crCoQuan.CoQuanID == item.CoQuanID)
                            {
                                if (item.TrangThai == 200)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 201)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                else
                                {
                                    item.TenTrangThai = "Đã tiếp nhận";
                                    item.isPheDuyet = false;
                                }
                                list.Add(item);
                            }

                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
                        {
                            if (crCoQuan.CoQuanID == item.CoQuanID)
                            {
                                if (item.TrangThai == 200)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 201)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                if (item.TrangThai == 300)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 301)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                list.Add(item);

                            }
                            else
                            {
                                if (listThanhTraHuyen.Contains(crCoQuan.CoQuanID) && item.TrangThai >= 300 && item.CapQuanLy == EnumCapQuanLyCanBo.CapHuyen.GetHashCode())
                                {
                                    if (item.TrangThai == 300)
                                    {
                                        item.isPheDuyet = true;
                                        item.TenTrangThai = "Chưa tiếp nhận";
                                    }
                                    else if (item.TrangThai == 301)
                                    {
                                        item.isPheDuyet = true;
                                        item.TenTrangThai = "Kê khai lại";
                                    }
                                    else
                                    {
                                        item.isPheDuyet = false;
                                        item.TenTrangThai = "Đã tiếp nhận";
                                    }
                                    list.Add(item);
                                }
                            }

                        }
                        else                                                             // cấp xã
                        {
                            if (crCoQuan.CoQuanID == item.CoQuanID)
                            {
                                if (item.TrangThai == 200)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else if (item.TrangThai == 201)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                else if (item.TrangThai == 101)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Kê khai lại";
                                }
                                else
                                {
                                    item.TenTrangThai = "Đã tiếp nhận";
                                }
                                list.Add(item);
                            }

                        }
                        var DuyetKeKhai = GetDuyetBanKeKhaiByKeKhaiID(item.KeKhaiID);
                        item.DanhSachFileDinhKem = new List<FileDinhKemModel>();
                        var DanhSachFileDinhKem = new FileDinhKemDAL().GetAllField_FileDinhKem_ByNghiepVuID_AndType(item.KeKhaiID, EnumLoaiFileDinhKem.FileKeKhai.GetHashCode()).Where(x => x.TrangThai > 0).ToList();
                        if (DuyetKeKhai != null && DuyetKeKhai.DuyetBanKeKhaiID > 0)
                        {
                            item.DanhSachFileDuyetDinhKem = new List<FileDinhKemModel>();
                            var DanhSachFileDuyetDinhKem = new FileDinhKemDAL().GetAllField_FileDinhKem_ByNghiepVuID_AndType(DuyetKeKhai.DuyetBanKeKhaiID, EnumLoaiFileDinhKem.FileDuyetKeKhai.GetHashCode()).Where(x => x.TrangThai > 0).ToList();
                            if (DanhSachFileDuyetDinhKem.Count > 0)
                            {
                                item.DanhSachFileDuyetDinhKem.Add(DanhSachFileDuyetDinhKem[0]);
                            }
                        }
                        if (DanhSachFileDinhKem.Count > 0)
                        {
                            item.DanhSachFileDinhKem.Add(DanhSachFileDinhKem[0]);
                        }
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (laThanhTraTinh)
            {
                list = list.Where(x => x.TrangThai >= 300 || listThanhTraTinh.Contains(x.CoQuanID)).ToList();
                foreach (var item in list)
                {
                    if (item.TrangThai == 400)
                    {
                        item.TenTrangThai = "đã duyệt";
                    }
                    else
                    {
                        item.TenTrangThai = "chưa duyệt";
                    }
                }
            }
            return list;
        }

        // Get Quan lý bản kê khai không phân trang
        public List<KeKhaiModelPartial> GetListQuanLyBanKeKhai(int CoQuanID, int CanBoID)
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var GiaTri = 200;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 2;
            var CoQuanQuanLy = 0;
            var QuyenDuyet = new PhanQuyenDAL().CheckQuyenDuyetBanKeKhai(CanBoID);

            if (UserRole.CheckAdmin(CanBoID))
            {
                GiaTri = 200;
                CapQuanLy = 0;
                CoQuanQuanLy = 0;

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
            {

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
            {
                GiaTri = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
            {
                GiaTri = 300;
                CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
            {
                GiaTri = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanID;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
            {

                GiaTri = 300;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
            {
                GiaTri = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanID;
            }
            var listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy);
            listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //////////////////////////////////

            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("GiaTri",SqlDbType.Int),
                pList

              };
            parameters[0].Value = GiaTri;
            parameters[1].Value = tbCanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_QUANLYKEKHAI_GETLISTQUANLYBANKEKHAI, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TenTrangThai = new ThongTinTaiSanDAL().getTenTrangThai(Utils.ConvertToInt32(dr[TRANG_THAI], 0));
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0);
                        item.LoaiDotKeKhaiID = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.BoNhiem.GetHashCode())
                        {
                            item.TenLoai = "Kê khai bổ nhiệm";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.BoSung.GetHashCode())
                        {
                            item.TenLoai = "Kê khai bổ sung";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.HangNam.GetHashCode())
                        {
                            item.TenLoai = "Kê khai hàng năm";
                        }
                        else if (item.LoaiDotKeKhaiID == EnumLoaiDotKeKhai.LanDau.GetHashCode())
                        {
                            item.TenLoai = "Kê khai lần đầu";
                        }
                        if (UserRole.CheckAdmin(CanBoID))
                        {
                            item.isPheDuyet = false;
                            if (item.TrangThai < 300 && item.TrangThai >= 200) item.TenTrangThai = "Chưa tiếp nhận";
                            else if (item.TrangThai == 300) item.TenTrangThai = "Đã tiếp nhận";
                            else if (item.TrangThai >= 400) item.TenTrangThai = "Đã tiếp nhận";
                            list.Add(item);
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
                        {
                            list.Add(item);
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
                        {
                            if (item.TrangThai == 200 || item.TrangThai == 201)
                            {
                                if (item.CapCoQuan == EnumCapCoQuan.CapHuyen.GetHashCode())
                                    item.isPheDuyet = false;
                                else item.isPheDuyet = true;
                                item.TenTrangThai = "Chưa tiếp nhận";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã tiếp nhận";
                                item.isPheDuyet = false;
                            }
                            list.Add(item);
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
                        {
                            if (item.TrangThai >= 300)
                            {
                                if (item.TrangThai == 300)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else
                                {
                                    item.isPheDuyet = false;
                                    item.TenTrangThai = "Đã tiếp nhận";
                                }
                                list.Add(item);
                            }
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
                        {
                            if (item.TrangThai == 200 || item.TrangThai == 201)
                            {
                                if (item.CapCoQuan == EnumCapCoQuan.CapXa.GetHashCode())
                                    item.isPheDuyet = false;
                                else item.isPheDuyet = true;
                                item.TenTrangThai = "Chưa tiếp nhận";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã tiếp nhận";
                                item.isPheDuyet = false;
                            }
                            list.Add(item);
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
                        {
                            if (item.TrangThai >= 300)
                            {
                                if (item.TrangThai == 300)
                                {
                                    item.isPheDuyet = true;
                                    item.TenTrangThai = "Chưa tiếp nhận";
                                }
                                else
                                {
                                    item.isPheDuyet = false;
                                    item.TenTrangThai = "Đã tiếp nhận";
                                }
                                list.Add(item);
                            }
                        }
                        else                                                             // cấp xã
                        {
                            if (item.TrangThai == 200 || item.TrangThai == 201)
                            {
                                item.isPheDuyet = true;
                                item.TenTrangThai = "Chưa tiếp nhận";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã tiếp nhận";
                            }
                            list.Add(item);
                        }
                        if (!QuyenDuyet)
                        {
                            item.isPheDuyet = false;
                        }
                        item.DanhSachFileDinhKem = new FileDinhKemDAL().GetListFileDinhKemByKeKhaiID(item.KeKhaiID).Where(x => x.TrangThai > 0).ToList();
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

        //đang không dùng
        public List<KeKhaiModelPartial> GetPagingBySearch1(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow)
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
            var pList = new SqlParameter("@DanhSachDonViTrucThuoc", SqlDbType.Structured);
            pList.TypeName = "dbo.id_list";
            var TrangThai = 200;
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            if (p.CoQuanID != null && p.CoQuanID > 0)
            {
                tbCoQuanID.Rows.Add(p.CoQuanID);
            }

            else if (UserRole.CheckAdmin(CanBoID))
            {
                TrangThai = 200;
                var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(0).ToList();
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
            {

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
            {
                TrangThai = 200;
                var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID).Where(x => x.CapID != EnumCapCoQuan.CapHuyen.GetHashCode()).ToList();
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
                tbCoQuanID.Rows.Add(CoQuanID);
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
            {
                TrangThai = 300;
                var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(crCoQuan.CoQuanChaID.Value).Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).ToList();
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
            {
                TrangThai = 200;
                var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID).Where(x => x.CapID != EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
            {

                TrangThai = 300;
                var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID).Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
                listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            else                                                             // cấp xã
            {
                TrangThai = 200;
                tbCoQuanID.Rows.Add(CoQuanID);

            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                new SqlParameter(NAM,SqlDbType.Int),
                pList,
                new SqlParameter(TRANG_THAI,SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
              };
            parameters[9].Direction = ParameterDirection.Output;
            parameters[9].Size = 8;
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = p.CanBoID ?? Convert.DBNull;
            parameters[6].Value = p.Nam ?? Convert.DBNull;
            parameters[7].Value = tbCoQuanID;
            parameters[8].Value = TrangThai;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, QUANLYKEKHAI_GETPAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TenTrangThai = new ThongTinTaiSanDAL().getTenTrangThai(Utils.ConvertToInt32(dr[TRANG_THAI], 0));
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);

                        if (UserRole.CheckAdmin(CanBoID))
                        {
                            TrangThai = 200;
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
                        {

                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
                        {
                            //TrangThai = 200;
                            if (item.TrangThai == 200) item.isPheDuyet = true;
                            else item.isPheDuyet = false;
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
                        {
                            //TrangThai = 300;
                            if (item.TrangThai == 300) item.isPheDuyet = true;
                            else item.isPheDuyet = false;
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
                        {
                            //TrangThai = 200;
                            if (item.TrangThai == 200) item.isPheDuyet = true;
                            else item.isPheDuyet = false;
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
                        {
                            //TrangThai = 300;
                            if (item.TrangThai == 300) item.isPheDuyet = true;
                            else item.isPheDuyet = false;
                        }
                        else                                                             // cấp xã
                        {
                            //TrangThai = 200;
                            if (item.TrangThai == 200) item.isPheDuyet = true;
                            else item.isPheDuyet = false;
                        }
                        item.DanhSachFileDinhKem = new FileDinhKemDAL().GetListFileDinhKemByKeKhaiID(item.KeKhaiID).Where(x => x.TrangThai > 0).ToList();

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

        // thống kê biến động tài sản
        public DataTable ThongTinTaiSan_ThongKeBienDongTaiSan(int CanBoID, int? NhomTaiSanID)
        {
            var Result = new DataTable();
            try
            {
                var query = new ThongTinTaiSanDAL().ThongKeBienDongTaiSan(CanBoID);
                Result = query.AsEnumerable().Where(x => Utils.ConvertToInt32(x.Field<string>("NhomTaiSanID"), 0) == NhomTaiSanID || NhomTaiSanID == null || NhomTaiSanID == 0).CopyToDataTable();
            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        // Chi tiết thông tin bản kê khai by kekhaiID
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_KeKhaiID(int KeKhaiID)
        {
            var Result = new ChiTietThongTinKeKhaiModel();
            if (KeKhaiID < 1)
            {
                return Result;
            }
            try
            {
                var crKeKhai = new KeKhaiDAL().GetByID(KeKhaiID);
                if (crKeKhai == null || crKeKhai.KeKhaiID < 1)
                {
                    return Result;
                }
                // thông tin bản thân
                var ThongTinBanThan = new HeThongCanBoDAL().GetChiTietCanBoByID(crKeKhai.CanBoID);
                Result.BanThan = ThongTinBanThan;
                // thông tin vợ chồng
                var ThongTinThanNhan = new KeKhaiThanNhanDAL().GetThanNhanCanBo_By_CanBoID(crKeKhai.CanBoID);
                Result.VoChong = ThongTinThanNhan.VoChong;
                // thông tin con chưa thành niên
                Result.ConChuaThanhNien = ThongTinThanNhan.ConChuaThanhNien;

                // thông tin tài sản
                Result.ThongTinTaiSan = new ThongTinTaiSanDAL().ThongTinTaiSan_GetByKeKhaiID_ForQuanLyBanKeKhai(KeKhaiID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// Duyệt nhiều cấp
        /// </summary>
        /// <param name="DuyetBanKeKhaiModel"></param>
        /// <param name=CAN_BO_ID></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>

        public BaseResultModel DuyetBanKeKhai(DuyetBanKeKhaiModel DuyetBanKeKhaiModel, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DuyetBanKeKhaiModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập dữ liệu trước khi tiếp nhận";
                    return Result;
                }
                else if (DuyetBanKeKhaiModel.PheDuyet == null || DuyetBanKeKhaiModel.PheDuyet < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Phê duyệt không được trống";
                    return Result;
                }
                else if (DuyetBanKeKhaiModel.NgayDuyet == null)
                {
                    Result.Status = 0;
                    Result.Message = "Ngày tiếp nhận không được trống";
                    return Result;
                }
                else if (DuyetBanKeKhaiModel.KeKhaiID == null || DuyetBanKeKhaiModel.KeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Bản kê khai không tồn tại";
                    return Result;
                }
                else
                {
                    var crObj = new KeKhaiDAL().GetByID(DuyetBanKeKhaiModel.KeKhaiID);
                    if (crObj == null || crObj.KeKhaiID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Bản kê khai không tồn tại";
                        return Result;
                    }
                    else
                    {
                        //var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
                        var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
                        DuyetBanKeKhaiModel.NguoiDuyetID = CanBoID;
                        if (DuyetBanKeKhaiModel.PheDuyet == 1)
                        {
                            DuyetBanKeKhaiModel.TrangThaiDuyet = laThanhTraTinh == true ? (int)EnumTrangThaiDuyet.ThanhTraTinhDuyet : (int)EnumTrangThaiDuyet.DuyetCap1;
                        }
                        else
                        {
                            DuyetBanKeKhaiModel.TrangThaiDuyet = (int)EnumTrangThaiDuyet.KekhaiLai;
                        }
                        SqlParameter[] parameters = new SqlParameter[]
                         {
                            new SqlParameter(NGUOI_DUYET_ID, SqlDbType.Int),
                            new SqlParameter(DUYETBANKEKHAI_KEKHAIID, SqlDbType.Int),
                            new SqlParameter(PHE_DUYET, SqlDbType.Int),
                            new SqlParameter(NGAY_DUYET, SqlDbType.DateTime2),
                            new SqlParameter(TRANG_THAI_DUYET, SqlDbType.Int),
                            new SqlParameter(GHI_CHU, SqlDbType.NText),
                         };
                        parameters[0].Value = DuyetBanKeKhaiModel.NguoiDuyetID;
                        parameters[1].Value = DuyetBanKeKhaiModel.KeKhaiID;
                        parameters[2].Value = DuyetBanKeKhaiModel.PheDuyet;
                        parameters[3].Value = DuyetBanKeKhaiModel.NgayDuyet;
                        parameters[4].Value = DuyetBanKeKhaiModel.TrangThaiDuyet;
                        parameters[5].Value = DuyetBanKeKhaiModel.GhiChu ?? Convert.DBNull;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI, parameters), 0);
                                    trans.Commit();
                                    if (query > 0)
                                    {
                                        Result.Status = 1;
                                        Result.Message = "tiếp nhận bản kê khai thành công";
                                        Result.Data = query;
                                    }
                                    else
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                    }
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
                    if (Result.Status == 1)
                    {
                        try
                        {
                            if (DuyetBanKeKhaiModel.PheDuyet == 1)
                            {
                                //thêm thông báo cho cán bộ kê khai
                                ThongBaoModel ThongBaoModel = new ThongBaoModel();
                                ThongBaoModel.TieuDe = "Thông báo lưu hồ sơ kê khai";
                                ThongBaoModel.NoiDung = "Bạn có " + crObj.TenBanKeKhai + " đã được tiếp nhận và lưu thành hồ sơ pháp lý";
                                ThongBaoModel.ThoiGianBatDau = DateTime.Now;
                                ThongBaoModel.ThoiGianKetThuc = DateTime.Now;
                                ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.LuuHoSoKeKhai.GetHashCode();
                                ThongBaoModel.NghiepVuID = crObj.KeKhaiID;
                                ThongBaoModel.TenNghiepVu = crObj.TenBanKeKhai;
                                ThongBaoModel.HienThi = true;
                                ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                                DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                                var CanBo = new HeThongCanBoDAL().GetCanBoByID(crObj.CanBoID);
                                dt.CanBoID = crObj.CanBoID;
                                dt.CoQuanID = CanBo.CoQuanID;
                                dt.TenCanBo = CanBo.TenCanBo;
                                dt.GioiTinh = CanBo.GioiTinh;
                                dt.Email = CanBo.Email;
                                ThongBaoModel.DoiTuongThongBao.Add(dt);
                                new NhacViecDAL().Edit_ThongBao(ThongBaoModel);
                                //thông báo cho lãnh đạo đơn vị
                                ThongBaoModel TBDuyetCongKhai = new ThongBaoModel();
                                TBDuyetCongKhai.TieuDe = "Thông báo duyệt công khai hồ sơ kê khai";
                                TBDuyetCongKhai.NoiDung = crObj.TenBanKeKhai + " cần duyệt công khai tại đơn vị";
                                TBDuyetCongKhai.ThoiGianBatDau = DateTime.Now;
                                TBDuyetCongKhai.ThoiGianKetThuc = DateTime.Now;
                                TBDuyetCongKhai.LoaiThongBao = EnumLoaiThongBao.DuyetCongKhai.GetHashCode();
                                TBDuyetCongKhai.NghiepVuID = crObj.KeKhaiID;
                                TBDuyetCongKhai.TenNghiepVu = crObj.TenBanKeKhai;
                                TBDuyetCongKhai.HienThi = true;
                                TBDuyetCongKhai.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                                //lấy đối tượng là lãnh đạo đơn vị
                                var listCanBo = new HeThongCanBoDAL().GetCanBoByChucNang(CanBo.CoQuanID, ChucNangEnum.KeKhai_DuyetKeKhaiCongKhai.GetHashCode());
                                if (listCanBo != null && listCanBo.Count > 0)
                                {
                                    foreach (var cb in listCanBo)
                                    {
                                        DoiTuongThongBaoModel doituong = new DoiTuongThongBaoModel();
                                        doituong.CanBoID = cb.CanBoID;
                                        doituong.CoQuanID = cb.CoQuanID;
                                        doituong.TenCanBo = cb.TenCanBo;
                                        doituong.GioiTinh = cb.GioiTinh;
                                        doituong.Email = cb.Email;
                                        TBDuyetCongKhai.DoiTuongThongBao.Add(doituong);
                                    }
                                }
                                new NhacViecDAL().Edit_ThongBao(TBDuyetCongKhai);
                            }
                            else
                            {
                                ThongBaoModel ThongBaoModel = new ThongBaoModel();
                                ThongBaoModel.TieuDe = "Thông báo yêu cầu kê khai bổ sung";
                                ThongBaoModel.NoiDung = "Bạn có " + crObj.TenBanKeKhai + " được yêu cầu kê khai bổ sung";
                                ThongBaoModel.ThoiGianBatDau = DateTime.Now;
                                ThongBaoModel.ThoiGianKetThuc = DateTime.Now;
                                ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.BoSungHoSoKeKhai.GetHashCode();
                                ThongBaoModel.NghiepVuID = crObj.KeKhaiID;
                                ThongBaoModel.TenNghiepVu = crObj.TenBanKeKhai;
                                ThongBaoModel.HienThi = true;
                                ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                                DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                                var CanBo = new HeThongCanBoDAL().GetCanBoByID(crObj.CanBoID);
                                dt.CanBoID = crObj.CanBoID;
                                dt.CoQuanID = CanBo.CoQuanID;
                                dt.TenCanBo = CanBo.TenCanBo;
                                dt.GioiTinh = CanBo.GioiTinh;
                                dt.Email = CanBo.Email;
                                ThongBaoModel.DoiTuongThongBao.Add(dt);

                                new NhacViecDAL().Edit_ThongBao(ThongBaoModel);
                            }
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public BaseResultModel DuyetBanKeKhai_Multil(List<int> DanhSachKeKhaiID, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhSachKeKhaiID == null || DanhSachKeKhaiID.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập dữ liệu trước khi tiếp nhận";
                    return Result;
                }
                else
                {
                    //// check tồn tại bản kê khai
                    //for (int i = 0; i < DanhSachKeKhaiID.Count; i++)
                    //{
                    //    var crObj = new KeKhaiDAL().GetByID(DanhSachKeKhaiID[i]);
                    //    if (crObj == null || crObj.KeKhaiID < 1)
                    //    {
                    //        Result.Status = 0;
                    //        Result.Message = "Bản kê khai không tồn tại";
                    //        return Result;
                    //    }
                    //}
                    //var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
                    //var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
                    int TrangThaiDuyet = EnumTrangThaiDuyet.ThanhTraTinhDuyet.GetHashCode();
                    var table = new DataTable();
                    table.Columns.Add("ID", typeof(string));
                    DanhSachKeKhaiID.ForEach(x => table.Rows.Add(x));
                    var ListKeKhaiID = new SqlParameter("@DanhSachKeKhaiID", SqlDbType.Structured);
                    ListKeKhaiID.TypeName = "dbo.list_ID";
                    SqlParameter[] parameters = new SqlParameter[]
                     {
                            new SqlParameter(NGUOI_DUYET_ID, SqlDbType.Int),
                            new SqlParameter(PHE_DUYET, SqlDbType.Int),
                            new SqlParameter(NGAY_DUYET, SqlDbType.DateTime2),
                            new SqlParameter(TRANG_THAI_DUYET, SqlDbType.Int),
                            new SqlParameter(GHI_CHU, SqlDbType.NText),
                             ListKeKhaiID
                     };
                    parameters[0].Value = CanBoID;
                    parameters[1].Value = EnumPheDuyetBanKeKhai.PheDuyet.GetHashCode();
                    parameters[2].Value = DateTime.Now.Date;
                    parameters[3].Value = TrangThaiDuyet;
                    parameters[4].Value = Convert.DBNull;
                    parameters[5].Value = table;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI_MULTIL, parameters), 0);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1;
                                    Result.Message = "tiếp nhận bản kê khai thành công";
                                    Result.Data = query;
                                }
                                else
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                }
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
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public DuyetBanKeKhaiModelPar GetDuyetBanKeKhaiByKeKhaiID(int? KeKhaiID)
        {
            DuyetBanKeKhaiModelPar DuyetBanKeKhaiModelPar = new DuyetBanKeKhaiModelPar();
            SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter(DUYETBANKEKHAI_KEKHAIID, SqlDbType.Int)
                        };
            parameters[0].Value = KeKhaiID ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        DuyetBanKeKhaiModelPar = new DuyetBanKeKhaiModelPar();
                        DuyetBanKeKhaiModelPar.DuyetBanKeKhaiID = Utils.ConvertToInt32(dr[DUYET_BAN_KE_KHAI_ID], 0);
                        DuyetBanKeKhaiModelPar.TrangThaiDuyet = Utils.ConvertToInt32(dr[TRANG_THAI_DUYET], 0);
                        DuyetBanKeKhaiModelPar.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        DuyetBanKeKhaiModelPar.TrangThaiNhacViec = Utils.ConvertToBoolean(dr[TRANG_THAI_NHAC_VIEC], true);
                        DuyetBanKeKhaiModelPar.PheDuyet = Utils.ConvertToInt32(dr[PHE_DUYET], 0);
                        break;
                    }
                    dr.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DuyetBanKeKhaiModelPar;
        }

        public BaseResultModel DuyetVaGuiBanKeKhai(GuiKeKhaiModel guiKeKhai, int? CanBoID)
        {
            var Result = new BaseResultModel();
            var pList = new SqlParameter("@DanhSachKeKhaiID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbKeKhaiID = new DataTable();
            tbKeKhaiID.Columns.Add("ID", typeof(string));

            try
            {
                if (guiKeKhai == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập dữ liệu trước khi tiếp nhận";
                    return Result;
                }
                else if (guiKeKhai.DanhSachBanKekhaiID is null || guiKeKhai.DanhSachBanKekhaiID == null)
                {
                    Result.Status = 0;
                    Result.Message = "không có bản kê khai cần duyệt và gửi!";
                    return Result;
                }
                else
                {
                    guiKeKhai.DanhSachBanKekhaiID.ForEach(x => tbKeKhaiID.Rows.Add(x));

                    SqlParameter[] parameters = new SqlParameter[]
                     {
                            pList,
                            new SqlParameter("@SoCongVan", SqlDbType.NVarChar),
                            new SqlParameter("@NgayDuyetVaGui", SqlDbType.DateTime),
                            new SqlParameter("@CanBoID", SqlDbType.Int),
                            new SqlParameter("@FileID", SqlDbType.Int),
                     };
                    parameters[0].Value = tbKeKhaiID;
                    parameters[1].Value = string.IsNullOrEmpty(guiKeKhai.SoCongVan) ? Convert.DBNull : guiKeKhai.SoCongVan;
                    parameters[2].Value = DateTime.Now;
                    parameters[3].Value = CanBoID ?? Convert.DBNull;
                    parameters[4].Value = guiKeKhai.FileID ?? Convert.DBNull;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_QuanLyBanKeKhai_DuyetVaGui", parameters), 0);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1;
                                    Result.Message = "Tiếp nhận và gửi bản kê khai thành công";
                                    Result.Data = query / 3;
                                }
                                else
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                }
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
                if (Result.Status == 1)
                {
                    try
                    {
                        foreach (var item in guiKeKhai.DanhSachBanKekhaiID)
                        {
                            var crObj = new KeKhaiDAL().GetByID(item);
                            //thêm thông báo cho cán bộ kê khai
                            ThongBaoModel ThongBaoModel = new ThongBaoModel();
                            ThongBaoModel.TieuDe = "Thông báo lưu hồ sơ kê khai";
                            ThongBaoModel.NoiDung = "Bạn có " + crObj.TenBanKeKhai + " đã được tiếp nhận và lưu thành hồ sơ pháp lý";
                            ThongBaoModel.ThoiGianBatDau = DateTime.Now;
                            ThongBaoModel.ThoiGianKetThuc = DateTime.Now;
                            ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.LuuHoSoKeKhai.GetHashCode();
                            ThongBaoModel.NghiepVuID = crObj.KeKhaiID;
                            ThongBaoModel.TenNghiepVu = crObj.TenBanKeKhai;
                            ThongBaoModel.HienThi = true;
                            ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                            DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                            var CanBo = new HeThongCanBoDAL().GetCanBoByID(crObj.CanBoID);
                            dt.CanBoID = crObj.CanBoID;
                            dt.CoQuanID = CanBo.CoQuanID;
                            dt.TenCanBo = CanBo.TenCanBo;
                            dt.GioiTinh = CanBo.GioiTinh;
                            dt.Email = CanBo.Email;
                            ThongBaoModel.DoiTuongThongBao.Add(dt);
                            new NhacViecDAL().Edit_ThongBao(ThongBaoModel);
                            ////thông báo cho lãnh đạo đơn vị
                            //ThongBaoModel TBDuyetCongKhai = new ThongBaoModel();
                            //TBDuyetCongKhai.TieuDe = "Thông báo duyệt công khai hồ sơ kê khai";
                            //TBDuyetCongKhai.NoiDung = crObj.TenBanKeKhai + " cần duyệt công khai tại đơn vị";
                            //TBDuyetCongKhai.ThoiGianBatDau = DateTime.Now;
                            //TBDuyetCongKhai.ThoiGianKetThuc = DateTime.Now;
                            //TBDuyetCongKhai.LoaiThongBao = EnumLoaiThongBao.DuyetCongKhai.GetHashCode();
                            //TBDuyetCongKhai.NghiepVuID = crObj.KeKhaiID;
                            //TBDuyetCongKhai.TenNghiepVu = crObj.TenBanKeKhai;
                            //TBDuyetCongKhai.HienThi = true;
                            //TBDuyetCongKhai.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                            ////lấy đối tượng là lãnh đạo đơn vị
                            //var listCanBo = new HeThongCanBoDAL().GetCanBoByChucNang(CanBo.CoQuanID, ChucNangEnum.KeKhai_DuyetKeKhaiCongKhai.GetHashCode());
                            //if (listCanBo != null && listCanBo.Count > 0)
                            //{
                            //    foreach (var cb in listCanBo)
                            //    {
                            //        DoiTuongThongBaoModel doituong = new DoiTuongThongBaoModel();
                            //        doituong.CanBoID = cb.CanBoID;
                            //        doituong.CoQuanID = cb.CoQuanID;
                            //        doituong.TenCanBo = cb.TenCanBo;
                            //        doituong.GioiTinh = cb.GioiTinh;
                            //        doituong.Email = cb.Email;
                            //        TBDuyetCongKhai.DoiTuongThongBao.Add(doituong);
                            //    }
                            //}
                            //new NhacViecDAL().Edit_ThongBao(TBDuyetCongKhai);
                            //thông báo cho thanh tra tỉnh
                            ThongBaoModel ThongBaoForTTTModel = new ThongBaoModel();
                            ThongBaoForTTTModel.TieuDe = "Thông báo tiếp nhân bản kê khai";
                            ThongBaoForTTTModel.NoiDung = "Bạn có bản kê khai cần tiếp nhận";
                            ThongBaoForTTTModel.ThoiGianBatDau = DateTime.Now;
                            ThongBaoForTTTModel.ThoiGianKetThuc = DateTime.Now;
                            ThongBaoForTTTModel.LoaiThongBao = EnumLoaiThongBao.DuyetHoSoKeKhai.GetHashCode();
                            ThongBaoForTTTModel.NghiepVuID = crObj.KeKhaiID;
                            ThongBaoForTTTModel.TenNghiepVu = crObj.TenBanKeKhai;
                            ThongBaoForTTTModel.HienThi = true;
                            ThongBaoForTTTModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                            var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
                            foreach (var ID in listThanhTraTinh)
                            {
                                var DanhSachCanBoThucoThanhTraTinh = new HeThongCanBoDAL().GetAllByCoQuanID(ID);
                                foreach (var CanBoTTT in DanhSachCanBoThucoThanhTraTinh)
                                {
                                    DoiTuongThongBaoModel doiTuongModel = new DoiTuongThongBaoModel();
                                    doiTuongModel.CanBoID = CanBoTTT.CanBoID;
                                    doiTuongModel.CoQuanID = CanBoTTT.CoQuanID;
                                    doiTuongModel.TenCanBo = CanBoTTT.TenCanBo;
                                    doiTuongModel.GioiTinh = CanBoTTT.GioiTinh;
                                    doiTuongModel.Email = CanBoTTT.Email;
                                    ThongBaoForTTTModel.DoiTuongThongBao.Add(doiTuongModel);
                                }
                            }
                            new NhacViecDAL().Edit_ThongBao(ThongBaoForTTTModel);
                        }
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public BaseResultModel KeKhaiLai_BanKeKhai(DuyetBanKeKhaiModel DuyetBanKeKhaiModel, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DuyetBanKeKhaiModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập dữ liệu trước khi tiếp nhận";
                    return Result;
                }
                else if (DuyetBanKeKhaiModel.NgayHetHan == null)
                {
                    Result.Status = 0;
                    Result.Message = "Ngày hết hạn không được trống";
                    return Result;
                }
                else if (DuyetBanKeKhaiModel.NgayHetHan != null && DuyetBanKeKhaiModel.NgayHetHan < DateTime.Now.Date)
                {
                    Result.Status = 0;
                    Result.Message = "Ngày hết hạn không được nhỏ hơn ngày hiện tại";
                    return Result;
                }
                else if (DuyetBanKeKhaiModel.KeKhaiID == null || DuyetBanKeKhaiModel.KeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Bản kê khai không tồn tại";
                    return Result;
                }
                else
                {
                    var crObj = new KeKhaiDAL().GetByID(DuyetBanKeKhaiModel.KeKhaiID);
                    if (crObj == null || crObj.KeKhaiID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Bản kê khai không tồn tại";
                        return Result;
                    }
                    else
                    {
                        //var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
                        //var laThanhTraTinh = new PhanQuyenDAL().CheckThanhTraTinh(CoQuanID);
                        DuyetBanKeKhaiModel.NguoiDuyetID = CanBoID;
                        DuyetBanKeKhaiModel.PheDuyet = 0;
                        DuyetBanKeKhaiModel.TrangThaiDuyet = EnumTrangThaiDuyet.KekhaiLai.GetHashCode();
                        SqlParameter[] parameters = new SqlParameter[]
                         {
                            new SqlParameter(NGUOI_DUYET_ID, SqlDbType.Int),
                            new SqlParameter(DUYETBANKEKHAI_KEKHAIID, SqlDbType.Int),
                            new SqlParameter(PHE_DUYET, SqlDbType.Int),
                            new SqlParameter(NGAY_DUYET, SqlDbType.DateTime2),
                            new SqlParameter(TRANG_THAI_DUYET, SqlDbType.Int),
                            new SqlParameter(GHI_CHU, SqlDbType.NText),
                            new SqlParameter(NGAY_HET_HAN, SqlDbType.DateTime2),
                         };
                        parameters[0].Value = DuyetBanKeKhaiModel.NguoiDuyetID;
                        parameters[1].Value = DuyetBanKeKhaiModel.KeKhaiID;
                        parameters[2].Value = DuyetBanKeKhaiModel.PheDuyet;
                        parameters[3].Value = DateTime.Now;
                        parameters[4].Value = DuyetBanKeKhaiModel.TrangThaiDuyet;
                        parameters[5].Value = DuyetBanKeKhaiModel.GhiChu ?? Convert.DBNull;
                        parameters[6].Value = DuyetBanKeKhaiModel.NgayHetHan ?? Convert.DBNull;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, "v1_KeKhai_KeKhaiLai", parameters), 0);
                                    trans.Commit();
                                    if (query > 0)
                                    {
                                        Result.Status = 1;
                                        Result.Message = "Yêu cầu kê khai lại thành công";
                                        Result.Data = query;
                                    }
                                    else
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                    }
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
                    if (Result.Status > 0)
                    {
                        ThongBaoModel ThongBaoModel = new ThongBaoModel();
                        ThongBaoModel.TieuDe = "Thông báo yêu cầu kê khai bổ sung";
                        ThongBaoModel.NoiDung = "Bạn có " + crObj.TenBanKeKhai + " được yêu cầu kê khai bổ sung";
                        ThongBaoModel.ThoiGianBatDau = DateTime.Now;
                        ThongBaoModel.ThoiGianKetThuc = DateTime.Now;
                        ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.BoSungHoSoKeKhai.GetHashCode();
                        ThongBaoModel.NghiepVuID = crObj.KeKhaiID;
                        ThongBaoModel.TenNghiepVu = crObj.TenBanKeKhai;
                        ThongBaoModel.HienThi = true;
                        ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                        DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                        var CanBo = new HeThongCanBoDAL().GetCanBoByID(crObj.CanBoID);
                        dt.CanBoID = crObj.CanBoID;
                        dt.CoQuanID = CanBo.CoQuanID;
                        dt.TenCanBo = CanBo.TenCanBo;
                        dt.GioiTinh = CanBo.GioiTinh;
                        dt.Email = CanBo.Email;
                        ThongBaoModel.DoiTuongThongBao.Add(dt);
                        new NhacViecDAL().Edit_ThongBao(ThongBaoModel);
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public BaseResultModel ThanhTraTinhTiepNhanBanKeKhai(GuiKeKhaiModel guiKeKhai, int? CoQuanID, int? CanBoID)
        {
            var Result = new BaseResultModel();
            if (guiKeKhai is null || guiKeKhai == null)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng nhập dữ liệu trước khi tiếp nhận";
                return Result;
            }
            if (guiKeKhai.DanhSachBanKekhaiID != null)
            {
                if (guiKeKhai.DanhSachBanKekhaiID.Count == 0 && guiKeKhai.TiepNhanAll == 0)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn bản kê khai cần tiếp nhận";
                    return Result;
                }
            }
            var pList = new SqlParameter("@DanhSachKeKhaiID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbKeKhaiID = new DataTable();
            tbKeKhaiID.Columns.Add("ID", typeof(string));
            List<int> DanhSachNghiepVuID = new List<int>();
            if (guiKeKhai.TiepNhanAll == 1)
            {
                BasePagingParamsForFilter p = new BasePagingParamsForFilter();
                p.PageSize = 10000;
                p.TrangThai = 4;
                p.NamKeKhai = guiKeKhai.Nam.Value;
                QuanLyBanKeKhaiModel quanLyBanKeKhai = GetPagingBySearch_BanKeKhaiThanhTraTinh(p, CoQuanID.Value, CanBoID.Value);
                quanLyBanKeKhai.DanhSach.ForEach(x => tbKeKhaiID.Rows.Add(x.KeKhaiID));
                quanLyBanKeKhai.DanhSach.ForEach(x => DanhSachNghiepVuID.Add(x.KeKhaiID));
            }
            else
            {
                guiKeKhai.DanhSachBanKekhaiID.ForEach(x => tbKeKhaiID.Rows.Add(x));
                DanhSachNghiepVuID = guiKeKhai.DanhSachBanKekhaiID;
            }
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {
                            pList,
                             new SqlParameter(NGUOI_DUYET_ID, SqlDbType.Int),
                            new SqlParameter(PHE_DUYET, SqlDbType.Int),
                            new SqlParameter(NGAY_DUYET, SqlDbType.DateTime2),
                            new SqlParameter(TRANG_THAI_DUYET, SqlDbType.Int),
                            new SqlParameter(GHI_CHU, SqlDbType.NText),

                 };
                parameters[0].Value = tbKeKhaiID;
                parameters[1].Value = CanBoID;
                parameters[2].Value = EnumPheDuyetBanKeKhai.PheDuyet.GetHashCode();
                parameters[3].Value = DateTime.Now.Date;
                parameters[4].Value = EnumTrangThaiDuyet.ThanhTraTinhDuyet.GetHashCode();
                parameters[5].Value = Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var query = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_QuanLyBanKeKhai_ThanhTraTinhTiepNhan", parameters), 0);
                            trans.Commit();
                            if (query > 0)
                            {
                                Result.Status = 1;
                                Result.Message = "Tiếp nhận thành công";
                                Result.Data = query;
                                new NhacViecDAL().UpdateTrangThaiHienThiThongBao(EnumLoaiThongBao.DuyetHoSoKeKhai.GetHashCode(), DanhSachNghiepVuID);
                            }
                            else
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                            }
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public List<KeKhaiModelPartial> GetBanKeKhaiHetHanKeKhaiLai()
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_KeKhai_GetBanKeKhaiHetHanChinhSua", null))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        list.Add(item);
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

        public BaseResultModel KyVaDuyetKeKhai(int CanBoDuyetID, FileDinhKemModel DataKyDuyet)
        {
            var Result = new BaseResultModel();
            int KeKhaiID = DataKyDuyet.KeKhaiID.Value;
            if (KeKhaiID > 0)
            {
                KeKhaiModel banKeKhai = new KeKhaiDAL().GetByID(KeKhaiID);
                if (banKeKhai.TrangThai >= EnumTrangThaiDuyet.DuyetCap1.GetHashCode())
                {
                    Result.Message = "Kê khai đã được duyệt và gửi";
                    Result.Status = 0;
                    return Result;
                }
                if (DataKyDuyet.FileUrl == "" || DataKyDuyet.TenFileGoc == "")
                {
                    Result.Message = "Không có tệp ký số";
                    Result.Status = 0;
                    return Result;
                }
                try
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                       new SqlParameter(NGUOI_DUYET_ID, SqlDbType.Int),
                       new SqlParameter(DUYETBANKEKHAI_KEKHAIID, SqlDbType.Int),
                       new SqlParameter(PHE_DUYET, SqlDbType.Int),
                       new SqlParameter(NGAY_DUYET, SqlDbType.DateTime2),
                       new SqlParameter(TRANG_THAI_DUYET, SqlDbType.Int),
                       new SqlParameter(GHI_CHU, SqlDbType.NText),
                    };
                    parameters[0].Value = CanBoDuyetID;
                    parameters[1].Value = KeKhaiID;
                    parameters[2].Value = EnumPheDuyetBanKeKhai.PheDuyet;
                    parameters[3].Value = DateTime.Now;
                    parameters[4].Value = EnumTrangThaiDuyet.GuiThanhTraTinh;
                    parameters[5].Value = Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {

                            var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI, parameters), 0);
                            trans.Commit();
                            if (query > 0)
                            {
                                FileDinhKemModel fileDinhKemModel = new FileDinhKemModel();
                                fileDinhKemModel.NghiepVuID = KeKhaiID;
                                fileDinhKemModel.FileUrl = DataKyDuyet.FileUrl;
                                fileDinhKemModel.TenFileGoc = DataKyDuyet.TenFileGoc;
                                fileDinhKemModel.TenFileHeThong = DataKyDuyet.TenFileGoc;
                                fileDinhKemModel.NgayCapNhat = DateTime.Now;
                                fileDinhKemModel.NguoiCapNhat = CanBoDuyetID;
                                fileDinhKemModel.FileType = EnumLoaiFileDinhKem.FileDuyetVaGui.ToString();
                                var resultfile = new FileDinhKemDAL().Insert(fileDinhKemModel);
                                if(resultfile.Status > 0)
                                {
                                    Result.Status = resultfile.Status;
                                    Result.Message = "Ký và duyệt thành công";
                                    List<int> DanhSachNghiepVuID = new List<int>();
                                    DanhSachNghiepVuID.Add(KeKhaiID);
                                    new NhacViecDAL().UpdateTrangThaiHienThiThongBao(EnumLoaiThongBao.DuyetHoSoKeKhai.GetHashCode(), DanhSachNghiepVuID);
                                }
                                else
                                {
                                    Result.Status = resultfile.Status;
                                    Result.Message = resultfile.Message;
                                }
                            }
                            else
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Result.Status = -1;
                    Result.Message = ConstantLogMessage.API_Error_System;
                    throw;
                }
            }
            return Result;
        }

        public BaseResultModel UploadFileKySo(int CanBoDuyetID, FileDinhKemModel DataKyDuyet)
        {
            var Result = new BaseResultModel();
            int KeKhaiID = DataKyDuyet.KeKhaiID.Value;
            if (KeKhaiID > 0)
            {
                KeKhaiModel banKeKhai = new KeKhaiDAL().GetByID(KeKhaiID);
                if (banKeKhai.TrangThai >= EnumTrangThaiDuyet.DuyetCap1.GetHashCode())
                {
                    Result.Message = "Kê khai đã được duyệt và gửi";
                    Result.Status = 0;
                    return Result;
                }
                if (DataKyDuyet.FileUrl == "" || DataKyDuyet.TenFileGoc == "")
                {
                    Result.Message = "Không có tệp ký số";
                    Result.Status = 0;
                    return Result;
                }
                try
                {
                    //SqlParameter[] parameters = new SqlParameter[]
                    //{
                    //   new SqlParameter(NGUOI_DUYET_ID, SqlDbType.Int),
                    //   new SqlParameter(DUYETBANKEKHAI_KEKHAIID, SqlDbType.Int),
                    //   new SqlParameter(PHE_DUYET, SqlDbType.Int),
                    //   new SqlParameter(NGAY_DUYET, SqlDbType.DateTime2),
                    //   new SqlParameter(TRANG_THAI_DUYET, SqlDbType.Int),
                    //   new SqlParameter(GHI_CHU, SqlDbType.NText),
                    //};
                    //parameters[0].Value = CanBoDuyetID;
                    //parameters[1].Value = KeKhaiID;
                    //parameters[2].Value = EnumPheDuyetBanKeKhai.PheDuyet;
                    //parameters[3].Value = DateTime.Now;
                    //parameters[4].Value = EnumTrangThaiDuyet.GuiThanhTraTinh;
                    //parameters[5].Value = Convert.DBNull;

                    //using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    //{
                    //    conn.Open();
                    //    using (SqlTransaction trans = conn.BeginTransaction())
                    //    {

                    //        var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI, parameters), 0);
                    //        trans.Commit();
                    //        if (query > 0)
                    //        {
                    //            FileDinhKemModel fileDinhKemModel = new FileDinhKemModel();
                    //            fileDinhKemModel.NghiepVuID = KeKhaiID;
                    //            fileDinhKemModel.FileUrl = DataKyDuyet.FileUrl;
                    //            fileDinhKemModel.TenFileGoc = DataKyDuyet.TenFileGoc;
                    //            fileDinhKemModel.TenFileHeThong = DataKyDuyet.TenFileGoc;
                    //            fileDinhKemModel.NgayCapNhat = DateTime.Now;
                    //            fileDinhKemModel.NguoiCapNhat = CanBoDuyetID;
                    //            fileDinhKemModel.FileType = EnumLoaiFileDinhKem.FileDuyetVaGui.ToString();
                    //            var resultfile = new FileDinhKemDAL().Insert(fileDinhKemModel);
                    //            if (resultfile.Status > 0)
                    //            {
                    //                Result.Status = resultfile.Status;
                    //                Result.Message = "Ký và duyệt thành công";
                    //                List<int> DanhSachNghiepVuID = new List<int>();
                    //                DanhSachNghiepVuID.Add(KeKhaiID);
                    //                new NhacViecDAL().UpdateTrangThaiHienThiThongBao(EnumLoaiThongBao.DuyetHoSoKeKhai.GetHashCode(), DanhSachNghiepVuID);
                    //            }
                    //            else
                    //            {
                    //                Result.Status = resultfile.Status;
                    //                Result.Message = resultfile.Message;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Result.Status = -1;
                    //            Result.Message = ConstantLogMessage.API_Error_System;
                    //        }
                    //    }
                    //}
                    FileDinhKemModel fileDinhKemModel = new FileDinhKemModel();
                    fileDinhKemModel.NghiepVuID = KeKhaiID;
                    fileDinhKemModel.FileUrl = DataKyDuyet.FileUrl;
                    fileDinhKemModel.TenFileGoc = DataKyDuyet.TenFileGoc;
                    fileDinhKemModel.TenFileHeThong = DataKyDuyet.TenFileGoc;
                    fileDinhKemModel.NgayCapNhat = DateTime.Now;
                    fileDinhKemModel.NguoiCapNhat = CanBoDuyetID;
                    fileDinhKemModel.FileType = EnumLoaiFileDinhKem.FileDuyetVaGui.ToString();
                    var resultfile = new FileDinhKemDAL().Insert(fileDinhKemModel);
                    if (resultfile.Status > 0)
                    {
                        Result.Status = resultfile.Status;
                        Result.Message = "Ký và lưu tệp thành công";
                    }
                    else
                    {
                        Result.Status = resultfile.Status;
                        Result.Message = resultfile.Message;
                    }
                }
                catch (Exception)
                {
                    Result.Status = -1;
                    Result.Message = ConstantLogMessage.API_Error_System;
                    throw;
                }
            }
            return Result;
        }
    }
}


