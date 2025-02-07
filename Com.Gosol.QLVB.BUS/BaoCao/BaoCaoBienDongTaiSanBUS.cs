using Com.Gosol.QLVB.DAL.BaoCao;
using Com.Gosol.QLVB.Models.BaoCao;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Com.Gosol.QLVB.BUS.BaoCao
{
    public interface IBaoCaoBienDongTaiSanBUS
    {
        public List<BaoCaoBienDongTaiSanModelPartial> BaoCaoBienDongTaiSan(int? CoQuan_ID, int CoQuanID, List<int> ListCanBoID, int TuNam, int DenNam, int CanBoID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri);
        public List<BienDongTaiSanNew> BaoCaoBienDongTaiSanChiTiet(int? CoQuan_ID,List<int> ListCanBoID, int TuNam, int DenNam, int CoQuanID, int CanBo_ID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri);
    }
    public class BaoCaoBienDongTaiSanBUS : IBaoCaoBienDongTaiSanBUS
    {
        private IBaoCaoBienDongTaiSanDAL _IBaoCaoBienDongTaiSanDAL;
        public BaoCaoBienDongTaiSanBUS(IBaoCaoBienDongTaiSanDAL IBaoCaoBienDongTaiSanDAL)
        {
            this._IBaoCaoBienDongTaiSanDAL = IBaoCaoBienDongTaiSanDAL;
        }
        public List<BaoCaoBienDongTaiSanModelPartial> BaoCaoBienDongTaiSan(int? CoQuan_ID, int CoQuanID, List<int> ListCanBoID, int TuNam, int DenNam, int CanBoID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri)
        {
            return _IBaoCaoBienDongTaiSanDAL.BaoCaoBienDongTaiSan(CoQuan_ID, CoQuanID,  ListCanBoID,  TuNam,  DenNam,CanBoID,MucBienDong_Cli,GiaTriTu,GiaTriDen,GiaTri);
        }
        public List<BienDongTaiSanNew> BaoCaoBienDongTaiSanChiTiet(int? CoQuan_ID, List<int> ListCanBoID, int TuNam, int DenNam,int CoQuanID,int CanBo_ID, int? MucBienDong_Cli, int? GiaTriTu, int? GiaTriDen, int? GiaTri)
        {
            return _IBaoCaoBienDongTaiSanDAL.BaoCaoBienDongTaiSanChiTiet(CoQuan_ID, ListCanBoID,TuNam,DenNam,CoQuanID,CanBo_ID, MucBienDong_Cli,  GiaTriTu, GiaTriDen,GiaTri);
        }
    }
}
