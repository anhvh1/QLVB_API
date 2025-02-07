using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.QLVB
{
    public interface IQuanLyThiSinhBUS
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow);
        public BaseResultModel Insert(List<ThongTinThiSinh> ListThongTinThiSinh);
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh);
        public BaseResultModel Delete(int ThiSinhID);
        public ThongTinThiSinh GetByID(int ThiSinhID);
    }
    public class QuanLyThiSinhBUS : IQuanLyThiSinhBUS
    {
        private IQuanLyThiSinhDAL _QuanLyThiSinhDAL;
        public QuanLyThiSinhBUS(IQuanLyThiSinhDAL quanLyThiSinhDAL)
        {
            _QuanLyThiSinhDAL = quanLyThiSinhDAL;
        }
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, int NamThi, int Truong, ref int TotalRow)
        {
            return _QuanLyThiSinhDAL.GetPagingBySearch(p, NamThi, Truong, ref TotalRow);
        }
        public BaseResultModel Insert(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            return _QuanLyThiSinhDAL.Insert(ListThongTinThiSinh);
        }
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh)
        {
            return _QuanLyThiSinhDAL.Update(ThongTinThiSinh);
        }
        public BaseResultModel Delete(int ThiSinhID)
        {
            return _QuanLyThiSinhDAL.Delete(ThiSinhID);
        }
        public ThongTinThiSinh GetByID(int ThiSinhID)
        {
            return _QuanLyThiSinhDAL.GetThongTinThiSinh_New(ThiSinhID);
        }
    }
}
