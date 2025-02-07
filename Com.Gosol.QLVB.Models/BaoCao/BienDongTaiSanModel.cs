using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class BienDongTaiSanModel
    {
        public DataTable Data { get; set; }

        public List<int> DanhSachNam { get; set; }

        public BienDongTaiSanModel()
        {

        }

    }
    public class BienDongTaiSanNew : BienDongTaiSanModel
    {
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public int? CoQuanID { get; set; }
        public BienDongTaiSanNew()
        {

        }
        public BienDongTaiSanNew(int CanBoID,string TenCanBo,int CoQuanID)
        {
            this.CanBoID = CanBoID;
            this.TenCanBo = TenCanBo;
            this.CoQuanID = CoQuanID;
        }
    }
    public class BienDongTaiSanPartial
    {
        public int? CoQuanID { get; set; }
        public List<int> ListCanBoID { get; set; }
        public int TuNam { get; set; }
        public int DenNam { get; set; }
        public int? MucBienDong_Cli { get; set; }
        public int? GiaTriTu { get; set; }
        public int? GiaTriDen { get; set; }
        public int? GiaTri { get; set; }
        public BienDongTaiSanPartial()
        {

        }
        public BienDongTaiSanPartial(int CoQuanID, List<int> ListCanBoID, int TuNam, int DenNam, int MucBienDong_Cli, int GiaTriTu, int GiaTriDen, int GiaTri)
        {
            this.CoQuanID = CoQuanID;
            this.ListCanBoID = ListCanBoID;
            this.TuNam = TuNam;
            this.DenNam = DenNam;
            this.MucBienDong_Cli = MucBienDong_Cli;
            this.GiaTriTu = GiaTriTu;
            this.GiaTriDen = GiaTriDen;
            this.GiaTri = GiaTri;
        }
    }

}
