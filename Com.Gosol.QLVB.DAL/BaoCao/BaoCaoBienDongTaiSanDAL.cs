using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models.BaoCao;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.BaoCao
{
    public interface IBaoCaoBienDongTaiSanDAL
    {
        public List<BaoCaoBienDongTaiSanModelPartial> BaoCaoBienDongTaiSan(int? CoQuan_ID, int CoQuanID, List<int> ListCanBoID, int TuNam, int DenNam, int CanBoID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri);
        public List<BienDongTaiSanNew> BaoCaoBienDongTaiSanChiTiet(int? CoQuan_ID, List<int> ListCanBoID, int TuNam, int DenNam, int CoQuanID, int CanBo_ID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri);
    }
    public class BaoCaoBienDongTaiSanDAL : IBaoCaoBienDongTaiSanDAL
    {
        // Báo cáo biến động tài sản của các cán bộ trực thuộc
        public List<BaoCaoBienDongTaiSanModelPartial> BaoCaoBienDongTaiSan(int? CoQuan_ID, int CoQuanID, List<int> ListCanBoID, int TuNam, int DenNam, int CanBoID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri)
        {
            List<BaoCaoBienDongTaiSanModelPartial> listBienDong = new List<BaoCaoBienDongTaiSanModelPartial>();
            List<BaoCaoBienDongTaiSanModelPartial> listBienDong_New = new List<BaoCaoBienDongTaiSanModelPartial>();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            List<BaoCaoBienDongTaiSanModel> list = new List<BaoCaoBienDongTaiSanModel>();
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 5;
            var CoQuanQuanLy = 0;
            if (UserRole.CheckAdmin(CanBoID))
            {
                CapQuanLy = 0;
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
            {

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
            {
                //  TrangThai = 200;
                CapQuanLy = 0;
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
            {
                // TrangThai = 300;
                CapQuanLy = 0;
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
            {
                // TrangThai = 200;
                CapQuanLy = 0;
                CoQuanQuanLy = crCoQuan.CoQuanID;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
            {

                //TrangThai = 300;
                CapQuanLy = 0;
                CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
            {
                //  TrangThai = 200;
                CapQuanLy = 0;
                CoQuanQuanLy = crCoQuan.CoQuanID;

            }
            if (/*ListCanBoID.Count <= 0 ||*/ ListCanBoID == null || ListCanBoID.Count == 0)
            {
                List<HeThongCanBoModel> listCanBoAll = new List<HeThongCanBoModel>();
                if (CoQuan_ID == null)
                {
                    listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy).ToList();
                }
                else
                {
                    listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy).Where(x => x.CoQuanID == CoQuan_ID).ToList();
                }
                ListCanBoID = new List<int>();
                listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
                listCanBoAll.ForEach(x => ListCanBoID.Add(x.CanBoID));
            }
            else
            {
                ListCanBoID.ForEach(x => tbCanBoID.Rows.Add(x));
            };

            var listNamKeKhai = new List<int>();
            SqlParameter[] parameters = new SqlParameter[]
                 {
                new SqlParameter("@TuNam",SqlDbType.Int),
                new SqlParameter("@DenNam",SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int),
                pList,

                 };
            parameters[0].Value = TuNam;
            parameters[1].Value = DenNam;
            parameters[2].Value = CoQuan_ID ?? Convert.DBNull;
            parameters[3].Value = tbCanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_BaoCao_BaoCaoMucBienDongTaiSan", parameters))
                {
                    while (dr.Read())
                    {
                        BaoCaoBienDongTaiSanModel item = new BaoCaoBienDongTaiSanModel();
                        item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        item.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        item.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        item.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr["NV00415"], 0);
                        //item.GiaTri = Utils.ConvertToIntDouble(dr["GiaTri"], double.MinValue);
                        item.KeKhaiID = Utils.ConvertToInt32(dr["NV00301"], 0);
                        string lstGiaTri = Utils.ConvertToString(dr["GiaTri"], string.Empty);
                        string[] arrGiaTri = lstGiaTri.Split(',');
                        double tongGiaTri = 0;
                        for (int i = 0; i < arrGiaTri.Length; i++)
                        {
                            double temp = Utils.ConvertToIntDouble(Encrypt_Decrypt.DecryptString_Aes(arrGiaTri[i]), 0);
                            tongGiaTri = tongGiaTri + temp;
                        }
                        item.GiaTri = tongGiaTri;
                        list.Add(item);

                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            int NamKeKhai = TuNam - 1;
            while (NamKeKhai < DenNam)
            {
                NamKeKhai++;
                listNamKeKhai.Add(NamKeKhai);
            }

            //Check TuNam-DenNam
            if (listNamKeKhai.Count <= 0)
            {

            }
            else if (listNamKeKhai.Count == 1)
            {
                foreach (var item in ListCanBoID)
                {
                    List<int> ListKeKhaiID = new List<int>();
                    ListKeKhaiID.AddRange(list.Where(x => x.CanBoID == item && x.NamKeKhai == DenNam).Select(y => y.KeKhaiID.Value));
                    if (ListKeKhaiID.Count == 1)
                    {
                        var CanBo = new HeThongCanBoDAL().GetCanBoByID(item);
                        BaoCaoBienDongTaiSanModelPartial BaoCaoBienDongTaiSanModelPartial1 = new BaoCaoBienDongTaiSanModelPartial();
                        BaoCaoBienDongTaiSanModelPartial1.CanBoID = item;
                        BaoCaoBienDongTaiSanModelPartial1.TenCanBo = CanBo.TenCanBo;
                        BaoCaoBienDongTaiSanModelPartial1.CoQuanID = new DanhMucCoQuanDonViDAL().GetByID(CanBo.CoQuanID).CoQuanID;
                        BaoCaoBienDongTaiSanModelPartial1.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CanBo.CoQuanID).TenCoQuan;
                        BaoCaoBienDongTaiSanModelPartial1.NgaySinh = Utils.ConvertToDateTime(CanBo.NgaySinh, DateTime.Now);
                        BaoCaoBienDongTaiSanModelPartial1.TrangThai = 0;
                        BaoCaoBienDongTaiSanModelPartial1.DiaChi = CanBo.DiaChi;
                        BaoCaoBienDongTaiSanModelPartial1.NamKeKhai = DenNam;
                        BaoCaoBienDongTaiSanModelPartial1.TuNam = TuNam;
                        BaoCaoBienDongTaiSanModelPartial1.DenNam = DenNam;
                        BaoCaoBienDongTaiSanModelPartial1.HoKhau = CanBo.HoKhau;
                        BaoCaoBienDongTaiSanModelPartial1.MucBienDong = null;
                        listBienDong.Add(BaoCaoBienDongTaiSanModelPartial1);
                    }
                    else if (ListKeKhaiID.Count < 1)
                    {
                    }
                    else
                    {
                        var CanBo = new HeThongCanBoDAL().GetCanBoByID(item);
                        BaoCaoBienDongTaiSanModelPartial BaoCaoBienDongTaiSanModelPartial = new BaoCaoBienDongTaiSanModelPartial();
                        BaoCaoBienDongTaiSanModelPartial.CanBoID = item;
                        BaoCaoBienDongTaiSanModelPartial.TenCanBo = CanBo.TenCanBo;
                        BaoCaoBienDongTaiSanModelPartial.CoQuanID = CanBo.CoQuanID.Value;
                        BaoCaoBienDongTaiSanModelPartial.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CanBo.CoQuanID).TenCoQuan;
                        BaoCaoBienDongTaiSanModelPartial.NgaySinh = CanBo.NgaySinh.Value;
                        BaoCaoBienDongTaiSanModelPartial.TrangThai = 0;
                        BaoCaoBienDongTaiSanModelPartial.DiaChi = CanBo.DiaChi;
                        BaoCaoBienDongTaiSanModelPartial.NamKeKhai = TuNam;
                        BaoCaoBienDongTaiSanModelPartial.HoKhau = CanBo.HoKhau;
                        BaoCaoBienDongTaiSanModelPartial.TuNam = TuNam;
                        BaoCaoBienDongTaiSanModelPartial.DenNam = DenNam;
                        double? MucBienDong = 0;
                        var ListKeKhai = list.Where(x => x.CanBoID == item && x.NamKeKhai == DenNam).ToList();
                        ListKeKhai.OrderBy(x => x);
                        MucBienDong = ListKeKhai.LastOrDefault().GiaTri - ListKeKhai.FirstOrDefault().GiaTri;
                        //}
                        BaoCaoBienDongTaiSanModelPartial.MucBienDong = MucBienDong;
                        listBienDong.Add(BaoCaoBienDongTaiSanModelPartial);

                    }

                }
            }
            else
            {
                List<int> list1 = new List<int>();
                list1.AddRange(list.Where(x => !list1.Contains(x.CanBoID)).Select(y => y.CanBoID));
                foreach (var item in ListCanBoID)
                {
                    List<int> ListKeKhaiID = new List<int>();
                    ListKeKhaiID.AddRange(list.Where(x => x.CanBoID == item).Select(y => y.KeKhaiID.Value));
                    var BaoCaoBienDongTaiSanModel = list.Where(x => x.CanBoID == item).FirstOrDefault();
                    var BaoCaoBienDongTaiSanModelTuNam = list.Where(x => x.CanBoID == item && x.NamKeKhai == TuNam).ToList();
                    var BaoCaoBienDongTaiSanModelDenNam = list.Where(x => x.CanBoID == item && x.NamKeKhai == DenNam).ToList();
                    if (BaoCaoBienDongTaiSanModel == null)
                    {
                        var CanBo = new HeThongCanBoDAL().GetCanBoByID(item);
                        BaoCaoBienDongTaiSanModelPartial BaoCaoBienDongTaiSanModelPartial1 = new BaoCaoBienDongTaiSanModelPartial();
                        BaoCaoBienDongTaiSanModelPartial1.CanBoID = item;
                        BaoCaoBienDongTaiSanModelPartial1.TenCanBo = CanBo.TenCanBo;
                        BaoCaoBienDongTaiSanModelPartial1.CoQuanID = CanBo.CoQuanID.Value;
                        BaoCaoBienDongTaiSanModelPartial1.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CanBo.CoQuanID).TenCoQuan;
                        BaoCaoBienDongTaiSanModelPartial1.NgaySinh = Utils.ConvertToDateTime(CanBo.NgaySinh, DateTime.Now);
                        BaoCaoBienDongTaiSanModelPartial1.TrangThai = 0;
                        BaoCaoBienDongTaiSanModelPartial1.DiaChi = CanBo.DiaChi;
                        BaoCaoBienDongTaiSanModelPartial1.NamKeKhai = DenNam;
                        BaoCaoBienDongTaiSanModelPartial1.TuNam = TuNam;
                        BaoCaoBienDongTaiSanModelPartial1.DenNam = DenNam;
                        BaoCaoBienDongTaiSanModelPartial1.HoKhau = CanBo.HoKhau;
                        BaoCaoBienDongTaiSanModelPartial1.MucBienDong = 0;
                    }
                    else if (BaoCaoBienDongTaiSanModelTuNam.Count <= 0 || BaoCaoBienDongTaiSanModelDenNam.Count <= 0)
                    {
                        var CanBo = new HeThongCanBoDAL().GetCanBoByID(item);
                        BaoCaoBienDongTaiSanModelPartial BaoCaoBienDongTaiSanModelPartial1 = new BaoCaoBienDongTaiSanModelPartial();
                        BaoCaoBienDongTaiSanModelPartial1.CanBoID = item;
                        BaoCaoBienDongTaiSanModelPartial1.CoQuanID = CanBo.CoQuanID.Value;
                        BaoCaoBienDongTaiSanModelPartial1.TenCanBo = CanBo.TenCanBo;
                        BaoCaoBienDongTaiSanModelPartial1.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CanBo.CoQuanID).TenCoQuan;
                        BaoCaoBienDongTaiSanModelPartial1.NgaySinh = Utils.ConvertToDateTime(CanBo.NgaySinh, DateTime.Now);
                        BaoCaoBienDongTaiSanModelPartial1.TrangThai = 0;
                        BaoCaoBienDongTaiSanModelPartial1.DiaChi = CanBo.DiaChi;
                        BaoCaoBienDongTaiSanModelPartial1.NamKeKhai = DenNam;
                        BaoCaoBienDongTaiSanModelPartial1.TuNam = TuNam;
                        BaoCaoBienDongTaiSanModelPartial1.DenNam = DenNam;
                        BaoCaoBienDongTaiSanModelPartial1.HoKhau = CanBo.HoKhau;
                        BaoCaoBienDongTaiSanModelPartial1.MucBienDong = null;
                        listBienDong.Add(BaoCaoBienDongTaiSanModelPartial1);
                    }
                    else
                    {
                        BaoCaoBienDongTaiSanModelPartial BaoCaoBienDongTaiSanModelPartial = new BaoCaoBienDongTaiSanModelPartial();
                        BaoCaoBienDongTaiSanModelPartial.CanBoID = Utils.ConvertToInt32(BaoCaoBienDongTaiSanModel.CanBoID, 0);
                        BaoCaoBienDongTaiSanModelPartial.TenCanBo = BaoCaoBienDongTaiSanModel.TenCanBo;
                        BaoCaoBienDongTaiSanModelPartial.CoQuanID = BaoCaoBienDongTaiSanModel.CoQuanID;
                        BaoCaoBienDongTaiSanModelPartial.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(BaoCaoBienDongTaiSanModel.CoQuanID).TenCoQuan;
                        BaoCaoBienDongTaiSanModelPartial.TenCoQuan = BaoCaoBienDongTaiSanModel.TenCoQuan;
                        BaoCaoBienDongTaiSanModelPartial.NgaySinh = BaoCaoBienDongTaiSanModel.NgaySinh;
                        BaoCaoBienDongTaiSanModelPartial.TrangThai = BaoCaoBienDongTaiSanModel.TrangThai;
                        BaoCaoBienDongTaiSanModelPartial.DiaChi = BaoCaoBienDongTaiSanModel.DiaChi;
                        BaoCaoBienDongTaiSanModelPartial.NamKeKhai = BaoCaoBienDongTaiSanModel.NamKeKhai;
                        BaoCaoBienDongTaiSanModelPartial.HoKhau = BaoCaoBienDongTaiSanModel.HoKhau;
                        BaoCaoBienDongTaiSanModelPartial.TuNam = TuNam;
                        BaoCaoBienDongTaiSanModelPartial.DenNam = DenNam;
                        double? MucBienDong = 0;
                        MucBienDong = BaoCaoBienDongTaiSanModelDenNam.OrderBy(x => x.KeKhaiID).LastOrDefault().GiaTri - BaoCaoBienDongTaiSanModelTuNam.OrderBy(x => x.KeKhaiID).LastOrDefault().GiaTri;
                        BaoCaoBienDongTaiSanModelPartial.MucBienDong = MucBienDong;
                        listBienDong.Add(BaoCaoBienDongTaiSanModelPartial);
                    }
                }
            }
            foreach (var item in listBienDong)
            {
                if (item.MucBienDong == null)
                {
                    item.MucBienDong = 0;
                }
            }
            if (MucBienDong_Cli == 1)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(Utils.ConvertToIntDouble(x.MucBienDong.Value, 0)) >= GiaTriTu && Math.Abs(Utils.ConvertToIntDouble(x.MucBienDong.Value, 0)) <= GiaTriDen).ToList();
            }
            else if (MucBienDong_Cli == 2)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(Utils.ConvertToIntDouble(x.MucBienDong.Value, 0)) < GiaTriTu || Math.Abs(x.MucBienDong.Value) > GiaTriDen).ToList();
            }
            else if (MucBienDong_Cli == 3)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(x.MucBienDong.Value) == GiaTri).ToList();
            }
            else if (MucBienDong_Cli == 4)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(x.MucBienDong.Value) > GiaTri).ToList();
            }
            else if (MucBienDong_Cli == 5)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(x.MucBienDong.Value) < GiaTri).ToList();
            }
            else if (MucBienDong_Cli == 6)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(Utils.ConvertToIntDouble(x.MucBienDong.Value, 0)) >= GiaTri).ToList();
            }
            else if (MucBienDong_Cli == 7)
            {
                listBienDong_New = listBienDong.Where(x => Math.Abs(x.MucBienDong.Value) <= GiaTri).ToList();
            }
            else
            {
                listBienDong_New = listBienDong;
            }


            return listBienDong_New;
        }

        // Báo cáo biến động tài sản chi tiết của các cán bộ trực thuộc
        public List<BienDongTaiSanNew> BaoCaoBienDongTaiSanChiTiet(int? CoQuan_ID, List<int> ListCanBoID, int TuNam, int DenNam, int CoQuanID, int CanBo_ID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri)
        {
            List<BienDongTaiSanNew> ListBienDongTaiSanModel = new List<BienDongTaiSanNew>();
            List<BienDongTaiSanNew> ListBienDongTaiSanModel_New = new List<BienDongTaiSanNew>();
            try
            {
                var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
                var CapQuanLy = 2;
                var CoQuanQuanLy = 0;
                if (UserRole.CheckAdmin(CanBo_ID))
                {
                    CapQuanLy = 0;
                    CoQuanQuanLy = 0;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())   // cấp trung ương
                {
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
                {
                    CapQuanLy = 0;
                    CoQuanQuanLy = 0;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
                {
                    CapQuanLy = 0;
                    CoQuanQuanLy = 0;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
                {
                    CapQuanLy = 0;
                    CoQuanQuanLy = crCoQuan.CoQuanID;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
                {
                    CapQuanLy = 0;
                    CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
                }
                else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
                {
                    CapQuanLy = 0;
                    CoQuanQuanLy = crCoQuan.CoQuanID;

                }
                if (ListCanBoID.Count <= 0)
                {
                    new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy).ForEach(x => ListCanBoID.Add(x.CanBoID));
                }
                var listNamKeKhai = new List<int>();

                int NamKeKhai = TuNam - 1;
                while (NamKeKhai < DenNam)
                {
                    NamKeKhai++;
                    listNamKeKhai.Add(NamKeKhai);
                }


                //Check TuNam-DenNam
                if (listNamKeKhai.Count <= 0)
                {

                }
                else if (listNamKeKhai.Count == 1)
                {
                    foreach (var CanBoID in ListCanBoID)
                    {
                        BienDongTaiSanNew BienDongTaiSanModel = new BienDongTaiSanNew();
                        var Result = new DataTable();
                        Result.Columns.Add("ChiTieu");
                        Result.Columns.Add("NhomTaiSanID");
                        try
                        {
                            //if()
                            List<int> ListKeKhaiID = new List<int>();
                            var listTaiSan = new ThongTinTaiSanDAL().ThongTinTaiSan_GetAll_By_CanBoID(CanBoID).Where(x => x.TrangThaiBanKeKhai >= 400).ToList();
                            ListKeKhaiID.AddRange(listTaiSan.Where(x => !ListKeKhaiID.Contains(x.KeKhaiID.Value) && x.NamKeKhai == listNamKeKhai.FirstOrDefault()).Select(y => y.KeKhaiID.Value));
                            if (ListKeKhaiID.Count <= 0)
                            {

                            }
                            else
                            {
                                if (ListKeKhaiID.Count > 1)
                                {
                                    ListKeKhaiID = new List<int>();
                                    ListKeKhaiID.AddRange(listTaiSan.Where(x => !ListKeKhaiID.Contains(x.KeKhaiID.Value) && x.NamKeKhai == listNamKeKhai.FirstOrDefault()).Select(y => y.KeKhaiID.Value));
                                    listNamKeKhai.OrderBy(x => x);
                                    ListKeKhaiID.OrderBy(x => x);
                                    BienDongTaiSanModel.DanhSachNam = listNamKeKhai;
                                    //ListNamKeKhai = listNamKeKhai;
                                    var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
                                    //listNamKeKhai.ForEach(x => Result.Columns.Add(x.ToString()));
                                    Result.Columns.Add("MucBienDong");

                                    Result.Columns.Add(listNamKeKhai.FirstOrDefault().ToString() + "1");
                                    Result.Columns.Add(listNamKeKhai.FirstOrDefault().ToString() + "2");

                                    for (int i = 0; i < listNhomTaiSan.Count; i++)
                                    {
                                        var crRow = Result.NewRow();
                                        crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                                        if (listNhomTaiSan[i].NhomTaiSanChaID == null || listNhomTaiSan[i].NhomTaiSanChaID < 1)
                                            crRow["NhomTaiSanID"] = listNhomTaiSan[i].NhomTaiSanID;
                                        else crRow["NhomTaiSanID"] = listNhomTaiSan[i].NhomTaiSanChaID;
                                        var KeKhaiIDFirst = listTaiSan.Where(x => (x.KeKhaiID == ListKeKhaiID.FirstOrDefault() && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri);
                                        var KeKhaiIDLast = listTaiSan.Where(x => (x.KeKhaiID == ListKeKhaiID.LastOrDefault() && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri);
                                        crRow[listNamKeKhai.FirstOrDefault().ToString() + "1"] = KeKhaiIDFirst.ToString();
                                        crRow[listNamKeKhai.FirstOrDefault().ToString() + "2"] = KeKhaiIDLast.ToString();
                                        crRow["MucBienDong"] =
                                            KeKhaiIDLast - KeKhaiIDFirst;

                                        int Sum = 0;

                                        Result.Rows.Add(crRow);
                                    }
                                    BienDongTaiSanModel.Data = Result;
                                    BienDongTaiSanModel.CanBoID = CanBoID;
                                    BienDongTaiSanModel.CoQuanID = new HeThongCanBoDAL().GetCanBoByID(CanBoID).CoQuanID;
                                    BienDongTaiSanModel.TenCanBo = new HeThongCanBoDAL().GetCanBoByID(CanBoID).TenCanBo;
                                    BienDongTaiSanModel.DanhSachNam = new List<int>();
                                    BienDongTaiSanModel.DanhSachNam.Add(Utils.ConvertToInt32(listNamKeKhai.FirstOrDefault().ToString() + "1", 0));
                                    BienDongTaiSanModel.DanhSachNam.Add(Utils.ConvertToInt32(listNamKeKhai.FirstOrDefault().ToString() + "2", 0));
                                    ListBienDongTaiSanModel.Add(BienDongTaiSanModel);
                                }
                                else
                                {
                                    listNamKeKhai.OrderBy(x => x);
                                    BienDongTaiSanModel.DanhSachNam = listNamKeKhai;
                                    var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
                                    Result.Columns.Add(listNamKeKhai.FirstOrDefault().ToString() + "1");
                                    Result.Columns.Add("MucBienDong");
                                    for (int i = 0; i < listNhomTaiSan.Count; i++)
                                    {
                                        var crRow = Result.NewRow();
                                        crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                                        if (listNhomTaiSan[i].NhomTaiSanChaID == null || listNhomTaiSan[i].NhomTaiSanChaID < 1)
                                            crRow["NhomTaiSanID"] = listNhomTaiSan[i].NhomTaiSanID;
                                        else crRow["NhomTaiSanID"] = listNhomTaiSan[i].NhomTaiSanChaID;
                                        //List<int> ListGiaTri = new List<int>();                                  
                                        for (int j = 0; j < listNamKeKhai.Count; j++)
                                        {
                                            crRow[listNamKeKhai.FirstOrDefault().ToString() + "1"] = listTaiSan.Where(x => (x.NamKeKhai == listNamKeKhai[j]) && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID)).ToList().Sum(y => y.GiaTri).ToString();
                                            crRow["MucBienDong"] = "";
                                        }
                                        Result.Rows.Add(crRow);
                                    }
                                    BienDongTaiSanModel.Data = Result;
                                    BienDongTaiSanModel.CanBoID = CanBoID;
                                    BienDongTaiSanModel.CoQuanID = new HeThongCanBoDAL().GetCanBoByID(CanBoID).CoQuanID;
                                    BienDongTaiSanModel.TenCanBo = new HeThongCanBoDAL().GetCanBoByID(CanBoID).TenCanBo;
                                    BienDongTaiSanModel.DanhSachNam = new List<int>();
                                    BienDongTaiSanModel.DanhSachNam.Add(Utils.ConvertToInt32(listNamKeKhai.FirstOrDefault().ToString() + "1", 0));
                                    ListBienDongTaiSanModel.Add(BienDongTaiSanModel);
                                }

                            }



                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
                else
                {
                    if (ListCanBoID.Count <= 0)
                    {

                    }
                    foreach (var CanBoID in ListCanBoID)
                    {
                        BienDongTaiSanNew BienDongTaiSanModel = new BienDongTaiSanNew();
                        var Result = new DataTable();
                        Result.Columns.Add("ChiTieu");
                        Result.Columns.Add("NhomTaiSanID");

                        try
                        {
                            var listTaiSan = new ThongTinTaiSanDAL().ThongTinTaiSan_GetAll_By_CanBoID(CanBoID).Where(x => x.TrangThaiBanKeKhai >= 400).ToList();
                            if (listTaiSan.Count <= 0)
                            {

                            }
                            else
                            {
                                listNamKeKhai.OrderBy(x => x);
                                BienDongTaiSanModel.DanhSachNam = listNamKeKhai;
                                var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
                                listNamKeKhai.ForEach(x => Result.Columns.Add(x.ToString()));
                                Result.Columns.Add("MucBienDong");
                                for (int i = 0; i < listNhomTaiSan.Count; i++)
                                {
                                    var crRow = Result.NewRow();
                                    crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                                    if (listNhomTaiSan[i].NhomTaiSanChaID == null || listNhomTaiSan[i].NhomTaiSanChaID < 1)
                                        crRow["NhomTaiSanID"] = listNhomTaiSan[i].NhomTaiSanID;
                                    else crRow["NhomTaiSanID"] = listNhomTaiSan[i].NhomTaiSanChaID;
                                    List<int> ListGiaTri = new List<int>();
                                    List<int> ListTongGiaTriHangNam = new List<int>();
                                    var KeKhaiIDMuonNhatDenNam = listTaiSan.Where(x => x.NamKeKhai == DenNam).OrderBy(x => x.KeKhaiID).Select(x => x.KeKhaiID).LastOrDefault();
                                    var KeKhaiIDSomNhatTuNam = listTaiSan.Where(x => x.NamKeKhai == TuNam).OrderBy(x => x.KeKhaiID).Select(x => x.KeKhaiID).LastOrDefault();
                                    var GiaTriKeKhaiTuNam = listTaiSan.Where(x =>
                                         x.KeKhaiID == KeKhaiIDSomNhatTuNam &&
                                        (x.NamKeKhai == TuNam) && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID)).ToList().Sum(y => y.GiaTri);
                                    var GiaTriKeKhaiDenNam = listTaiSan.Where(x =>
                                        x.KeKhaiID == KeKhaiIDMuonNhatDenNam &&
                                       (x.NamKeKhai == DenNam) && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID)).ToList().Sum(y => y.GiaTri);
                                    for (int j = 0; j < listNamKeKhai.Count; j++)
                                    {
                                        int? KeKhaiID = 0;
                                        if (listNamKeKhai[j] == TuNam)

                                        { KeKhaiID = KeKhaiIDSomNhatTuNam == null ? (int?)null : KeKhaiIDSomNhatTuNam; }
                                        else { KeKhaiID = KeKhaiIDMuonNhatDenNam == null ? (int?)null : KeKhaiIDMuonNhatDenNam; }
                                        crRow[listNamKeKhai[j].ToString()] = listTaiSan.Where(x =>
                                         x.KeKhaiID == KeKhaiID &&
                                        (x.NamKeKhai == listNamKeKhai[j]) && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID)).ToList().Sum(y => y.GiaTri).ToString();

                                        ListGiaTri.Add(Utils.ConvertToInt32(listTaiSan.Where(x => (x.NamKeKhai == listNamKeKhai[j]) && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID)).ToList().Sum(y => y.GiaTri).ToString(), 0));
                                    }
                                    crRow["MucBienDong"] = GiaTriKeKhaiDenNam - GiaTriKeKhaiTuNam;
                                    for (int a = 0; a < listNamKeKhai.Count; a++)
                                    {
                                        int Sum = 0;
                                        for (int k = 0; k < listNhomTaiSan.Count; k++)
                                        {

                                            Sum += Utils.ConvertToInt32(listTaiSan.Where(x => (x.NamKeKhai == listNamKeKhai[a] && (x.NhomTaiSanChaID == listNhomTaiSan[k].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[k].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString(), 0);
                                        }
                                        ListTongGiaTriHangNam.Add(Sum);
                                        if (Sum == 0)
                                        {
                                            crRow[listNamKeKhai[a].ToString()] = "Chưa có";
                                        }
                                    }
                                    ListGiaTri.OrderBy(x => x);
                                    var CheckKeKhaiTaiSan = ListTongGiaTriHangNam.Where(x => x <= 0).ToList();
                                    if (CheckKeKhaiTaiSan.Count > 0)
                                    {
                                        crRow["MucBienDong"] = " ";
                                    }
                                    else
                                    {
                                    }
                                    Result.Rows.Add(crRow);
                                }
                                BienDongTaiSanModel.Data = Result;
                                BienDongTaiSanModel.CanBoID = CanBoID;
                                BienDongTaiSanModel.CoQuanID = new HeThongCanBoDAL().GetCanBoByID(CanBoID).CoQuanID;
                                BienDongTaiSanModel.TenCanBo = new HeThongCanBoDAL().GetCanBoByID(CanBoID).TenCanBo;
                                BienDongTaiSanModel.DanhSachNam = listNamKeKhai;
                                ListBienDongTaiSanModel.Add(BienDongTaiSanModel);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                foreach (var item in ListBienDongTaiSanModel)
                {
                    int Sum = 0;
                    foreach (DataRow dr in item.Data.Rows)
                    {
                        int MucBienDong = Utils.ConvertToInt32(dr["MucBienDong"], 0);
                        Sum = Sum + MucBienDong;

                    }
                    if (MucBienDong_Cli == 1)
                    {
                        if (Math.Abs(Sum) >= GiaTriTu && Math.Abs(Sum) <= GiaTriDen)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }
                    }
                    else if (MucBienDong_Cli == 2)
                    {
                        if (Math.Abs(Sum) < GiaTriTu || Math.Abs(Sum) > GiaTriDen)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }
                    }
                    else if (MucBienDong_Cli == 3)
                    {
                        if (Math.Abs(Sum) == GiaTri)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }
                    }
                    else if (MucBienDong_Cli == 4)
                    {
                        if (Math.Abs(Sum) > GiaTri)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }

                    }
                    else if (MucBienDong_Cli == 5)
                    {
                        if (Math.Abs(Sum) < GiaTri)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }
                    }
                    else if (MucBienDong_Cli == 6)
                    {
                        if (Math.Abs(Sum) >= GiaTri)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }

                    }
                    else if (MucBienDong_Cli == 7)
                    {
                        if (Math.Abs(Sum) <= GiaTri)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }

                    }
                    else
                    {
                        if (Math.Abs(Sum) >= GiaTriTu && Math.Abs(Sum) <= GiaTriDen)
                        {
                            ListBienDongTaiSanModel_New.Add(item);
                        }
                        ListBienDongTaiSanModel_New = ListBienDongTaiSanModel;
                    }
                }
                if (CoQuan_ID != null)
                {
                    ListBienDongTaiSanModel_New = ListBienDongTaiSanModel_New.Where(x => x.CoQuanID == CoQuan_ID).ToList();
                }

            }
            catch
            {
                throw;
            }
            return ListBienDongTaiSanModel_New;
        }

    }
}
