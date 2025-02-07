using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.BaoCao;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IQuanLyBanKeKhaiBUS
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

    public class QuanLyBanKeKhaiBUS : IQuanLyBanKeKhaiBUS
    {
        private IQuanLyBanKeKhaiDAL _QuanLyBanKeKhaiDAL;
        public QuanLyBanKeKhaiBUS(IQuanLyBanKeKhaiDAL QuanLyBanKeKhaiDAL)
        {
            this._QuanLyBanKeKhaiDAL = QuanLyBanKeKhaiDAL;
        }

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_KeKhaiID(int KeKhaiID)
        {
            return _QuanLyBanKeKhaiDAL.ChiTietThongTinKeKhai_By_KeKhaiID(KeKhaiID);
        }

        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, ref int TotalRow)
        {
            return _QuanLyBanKeKhaiDAL.GetPagingBySearch(p, CoQuanID, CanBoID, ref TotalRow);
        }

        public DataTable ThongTinTaiSan_ThongKeBienDongTaiSan(int CanBoID, int? NhomTaiSanID)
        {
            return _QuanLyBanKeKhaiDAL.ThongTinTaiSan_ThongKeBienDongTaiSan(CanBoID, NhomTaiSanID);
        }

        public BaseResultModel DuyetBanKeKhai(DuyetBanKeKhaiModel DuyetBanKeKhaiModel, int CanBoID, int CoQuanID)
        {
            return _QuanLyBanKeKhaiDAL.DuyetBanKeKhai(DuyetBanKeKhaiModel, CanBoID, CoQuanID);
        }

        public BaseResultModel DuyetBanKeKhai_Multil(List<int> DanhSachKeKhaiID, int CanBoID, int CoQuanID)
        {
            return _QuanLyBanKeKhaiDAL.DuyetBanKeKhai_Multil(DanhSachKeKhaiID, CanBoID, CoQuanID);
        }
        public DuyetBanKeKhaiModelPar GetDuyetBanKeKhaiByKeKhaiID(int? KeKhaiID)
        {
            return _QuanLyBanKeKhaiDAL.GetDuyetBanKeKhaiByKeKhaiID(KeKhaiID);
        }

        public QuanLyBanKeKhaiModel GetQuanLyBanKeKhai(BasePagingParamsForFilter p, int CoQuanID, int CanBoID)
        {
            return _QuanLyBanKeKhaiDAL.GetQuanLyBanKeKhai(p, CoQuanID, CanBoID);
        }

        public QuanLyBanKeKhaiModel GetPagingBySearch_BanKeKhaiThanhTraTinh(BasePagingParamsForFilter p, int CoQuanID, int CanBoID)
        {
            return _QuanLyBanKeKhaiDAL.GetPagingBySearch_BanKeKhaiThanhTraTinh(p, CoQuanID, CanBoID);
        }

        public BaseResultModel DuyetVaGuiBanKeKhai(GuiKeKhaiModel guiKeKhai, int? CanBoID)
        {
            return _QuanLyBanKeKhaiDAL.DuyetVaGuiBanKeKhai(guiKeKhai, CanBoID);
        }

        public BaseResultModel KeKhaiLai_BanKeKhai(DuyetBanKeKhaiModel DuyetBanKeKhaiModel, int CanBoID, int CoQuanID)
        {
            return _QuanLyBanKeKhaiDAL.KeKhaiLai_BanKeKhai(DuyetBanKeKhaiModel, CanBoID, CoQuanID);
        }

        public BaseResultModel ThanhTraTinhTiepNhanBanKeKhai(GuiKeKhaiModel guiKeKhai, int? CoQuanID, int? CanBoID)
        {
            return _QuanLyBanKeKhaiDAL.ThanhTraTinhTiepNhanBanKeKhai(guiKeKhai, CoQuanID, CanBoID);
        }
        
        public BaseResultModel KyVaDuyetKeKhai(int CanBoDuyetID, FileDinhKemModel DataKyDuyet)
        {
            return _QuanLyBanKeKhaiDAL.KyVaDuyetKeKhai(CanBoDuyetID, DataKyDuyet);
        }
        public BaseResultModel UploadFileKySo(int CanBoDuyetID, FileDinhKemModel DataKyDuyet)
        {
            return _QuanLyBanKeKhaiDAL.UploadFileKySo(CanBoDuyetID, DataKyDuyet);
        }
    }
}
