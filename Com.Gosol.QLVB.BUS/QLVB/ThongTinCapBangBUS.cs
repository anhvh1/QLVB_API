using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.QLVB
{
    public interface IThongTinCapBangBUS
    {
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, DateTime? NgayCapBang, int DonViDaoTao, ref int TotalRow);
        public List<ThongTinThiSinh> DanhSachThiSinhCapBang(BasePagingParams p, ref int TotalRow);
        public BaseResultModel Insert(ThongTinThiSinh ThongTinThiSinh);
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh);
        public ThongTinThiSinh GetByID(int ThiSinhID);
        public ThongTinThiSinh GetThongTinCapBang(int ThiSinhID);
        public BaseResultModel Update_TrangThaiCapBang(List<ThongTinThiSinh> ListThongTinThiSinh);
        public List<ThongTinThiSinh> DanhSachThiSinhTrung(TraCuuParams p, ref int TotalRow);
        public List<ThongTinThiSinh> TraCuuVanBangChungChi(TraCuuParams p, ref int TotalRow);
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh ThongTinThiSinh, int? CanBoID);
        public List<ThongTinThiSinh> TraCuuVanBangChungChiNuocNgoaiCap(TraCuuParams p, ref int TotalRow);
        public Boolean CheckTicket(string Ticket);
    }
    public class ThongTinCapBangBUS : IThongTinCapBangBUS
    {
        private IThongTinCapBangDAL _ThongTinCapBangDAL;
        public ThongTinCapBangBUS(IThongTinCapBangDAL ThongTinCapBangDAL)
        {
            _ThongTinCapBangDAL = ThongTinCapBangDAL;
        }
        public List<ThongTinThiSinh> GetPagingBySearch(BasePagingParams p, DateTime? NgayCapBang, int DonViDaoTao, ref int TotalRow)
        {
            return _ThongTinCapBangDAL.GetPagingBySearch(p, NgayCapBang, DonViDaoTao, ref TotalRow);
        } 
        public List<ThongTinThiSinh> DanhSachThiSinhCapBang(BasePagingParams p, ref int TotalRow)
        {
            return _ThongTinCapBangDAL.DanhSachThiSinhCapBang(p, ref TotalRow);
        }
        public BaseResultModel Insert(ThongTinThiSinh ThongTinThiSinh)
        {
            return _ThongTinCapBangDAL.Insert(ThongTinThiSinh);
        }
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh)
        {
            return _ThongTinCapBangDAL.Update(ThongTinThiSinh);
        }
        public ThongTinThiSinh GetByID(int ThiSinhID)
        {
            return _ThongTinCapBangDAL.GetByID(ThiSinhID);
        }
        public ThongTinThiSinh GetThongTinCapBang(int ThiSinhID)
        {
            return _ThongTinCapBangDAL.GetThongTinCapBang(ThiSinhID);
        }
        public BaseResultModel Update_TrangThaiCapBang(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            return _ThongTinCapBangDAL.Update_TrangThaiCapBang(ListThongTinThiSinh);
        }
        public List<ThongTinThiSinh> TraCuuVanBangChungChi(TraCuuParams p, ref int TotalRow)
        {
            return _ThongTinCapBangDAL.TraCuuVanBangChungChi(p, ref TotalRow);
        }
        public List<ThongTinThiSinh> DanhSachThiSinhTrung(TraCuuParams p, ref int TotalRow)
        {
            return _ThongTinCapBangDAL.DanhSachThiSinhTrung(p, ref TotalRow);
        }
        public List<ThongTinThiSinh> TraCuuVanBangChungChiNuocNgoaiCap(TraCuuParams p, ref int TotalRow)
        {
            return _ThongTinCapBangDAL.TraCuuVanBangChungChiNuocNgoaiCap(p, ref TotalRow);
        }
        public BaseResultModel UpdateThongTinCapBang(ThongTinThiSinh ThongTinThiSinh, int? CanBoID)
        {
            ThongTinThiSinh ts = _ThongTinCapBangDAL.GetThongTinCapBang(ThongTinThiSinh.ThiSinhID);
            var Result = _ThongTinCapBangDAL.UpdateThongTinCapBang(ThongTinThiSinh, CanBoID);
            if (Result.Status == 1)
            {
                List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();
                if (ThongTinThiSinh.ThiSinhMaped > 0)
                {
                    ThongTinThiSinh.ThiSinhID = ThongTinThiSinh.ThiSinhMaped ?? 0;
                    ThongTinThiSinh.Maped = 1;
                    list.Add(ThongTinThiSinh);
                }
                else
                {
                    ThongTinThiSinh info = new ThongTinThiSinh();
                    info.ThiSinhID = ts.ThiSinhTrung1 ?? 0;
                    info.Maped = 0;
                    list.Add(info);
                }

                new QuanLyThiSinhDAL().Update_TrangThai(list);
            }
            return Result;
        }
        public Boolean CheckTicket(string Ticket)
        {
            return _ThongTinCapBangDAL.CheckTicket(Ticket);
        }
    }
}
