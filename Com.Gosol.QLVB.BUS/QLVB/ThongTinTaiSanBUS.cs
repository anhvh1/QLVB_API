using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IThongTinTaiSanBUS
    {
        public BaseResultModel Insert(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? BienDong, bool? LaBanTam);
        public BaseResultModel Update(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? LaBanTam);
        public ThongTinTaiSanModelPartial GetByID(int ThongTinTaiSanID);
        public List<KeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow);
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_GetByKeKhaiID(int KeKhaiID);
        public BaseResultModel Delete(List<int> ListThongTinTaiSanID);
        public BaseResultModel Delete_BanKeKhai(List<int> ListKeKhaiID);
        public List<ThongTinTaiSanModelPartial> GetLastThongTinTaiSan(int CanBoID);
        public DataTable ThongKeBienDongTaiSan(int CanBoID);
        public BienDongTaiSanModel ThongKe_BienDongTaiSan(NewParams p);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID(int CanBoID, int? NamKeKhai);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID_BanTam(int CanBoID, int? NamKeKhai, int? TrangThai);
        public BaseResultModel GuiBanKeKhai(int BanKeKhaiID);

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_AndBienDong_By_KeKhaiID(int KeKhaiID);

        public CheckKeKhaiTaiSan CheckKeKhaiTaiSan(int CanBoID);
        public BaseResultModel UpdateTrangThaiBanKeKhai(KeKhaiModel KeKhaiModel);
        public BaseResultModel GetImages(string filename);
        public BaseResultModel UpdateBarcode(KeKhaiModel KeKhaiModel);
        public int CheckBarcode(int KeKhaiID, string Barcode);
    }

    public class ThongTinTaiSanBUS : IThongTinTaiSanBUS
    {
        private IThongTinTaiSanDAL _ThongTinTaiSanDAL;
        public ThongTinTaiSanBUS(IThongTinTaiSanDAL IThongTinTaiSanDAL)
        {
            this._ThongTinTaiSanDAL = IThongTinTaiSanDAL;
        }
        public ThongTinTaiSanModelPartial GetByID(int ThongTinTaiSanID)
        {
            return _ThongTinTaiSanDAL.GetByID(ThongTinTaiSanID);
        }

        public List<KeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow)
        {
            return _ThongTinTaiSanDAL.GetPagingBySearch(p, CanBoID, ref TotalRow);
        }

        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_GetByKeKhaiID(int KeKhaiID)
        {
            return _ThongTinTaiSanDAL.ThongTinTaiSan_GetByKeKhaiID(KeKhaiID);
        }

        public BaseResultModel Delete(List<int> ListThongTinTaiSanID)
        {
            return _ThongTinTaiSanDAL.Delete(ListThongTinTaiSanID);
        }

        public BaseResultModel Delete_BanKeKhai(List<int> ListKeKhaiID)
        {
            return _ThongTinTaiSanDAL.Delete_BanKeKhai(ListKeKhaiID);
        }
        public List<ThongTinTaiSanModelPartial> GetLastThongTinTaiSan(int CanBoID)
        {
            return _ThongTinTaiSanDAL.GetLastThongTinTaiSan(CanBoID);
        }
        public DataTable ThongKeBienDongTaiSan(int CanBoID)
        {
            return _ThongTinTaiSanDAL.ThongKeBienDongTaiSan(CanBoID);
        }

        public BienDongTaiSanModel ThongKe_BienDongTaiSan(NewParams p)
        {
            return _ThongTinTaiSanDAL.ThongKe_BienDongTaiSan(p);
        }

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID(int CanBoID, int? NamKeKhai)
        {
            return _ThongTinTaiSanDAL.ChiTietThongTinKeKhai_By_CanBoID(CanBoID, NamKeKhai);
        }

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID_BanTam(int CanBoID, int? NamKeKhai, int? TrangThai)
        {
            return _ThongTinTaiSanDAL.ChiTietThongTinKeKhai_By_CanBoID_BanTam(CanBoID, NamKeKhai, TrangThai);
        }

        public BaseResultModel Insert(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? BienDong, bool? LaBanTam)
        {
            return _ThongTinTaiSanDAL.Insert(ListThongTinTaiSanModel, CanBoID, BienDong, LaBanTam);
        }

        public BaseResultModel Update(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? LaBanTam)
        {
            return _ThongTinTaiSanDAL.Update(ListThongTinTaiSanModel, CanBoID, LaBanTam);
        }
        //public int Insert(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, ref string Message, int? NamKeKhai)
        //{
        //    return _ThongTinTaiSanDAL.Insert(ListThongTinTaiSanModel, Message, NamKeKhai);
        //}

        public BaseResultModel GuiBanKeKhai(int BanKeKhaiID)
        {
            return _ThongTinTaiSanDAL.GuiBanKeKhai(BanKeKhaiID);
        }

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_AndBienDong_By_KeKhaiID(int KeKhaiID)
        {
            return _ThongTinTaiSanDAL.ChiTietThongTinKeKhai_AndBienDong_By_KeKhaiID(KeKhaiID);
        }

        public CheckKeKhaiTaiSan CheckKeKhaiTaiSan(int CanBoID)
        {
            return _ThongTinTaiSanDAL.CheckKeKhaiTaiSan(CanBoID);
        }
        public BaseResultModel UpdateTrangThaiBanKeKhai(KeKhaiModel KeKhaiModel)
        {
            return _ThongTinTaiSanDAL.UpdateTrangThaiBanKeKhai(KeKhaiModel);
        }
        public BaseResultModel GetImages(string filename)
        {
            return _ThongTinTaiSanDAL.GetImages(filename);
        }
        public BaseResultModel UpdateBarcode(KeKhaiModel KeKhaiModel)
        {
            return _ThongTinTaiSanDAL.UpdateBarcode(KeKhaiModel);
        }
        public int CheckBarcode(int KeKhaiID, string Barcode)
        {
            return _ThongTinTaiSanDAL.CheckBarcode(KeKhaiID, Barcode);
        }
    }
}
