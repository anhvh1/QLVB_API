using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.QLVB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.QLVB
{
    public interface IDashBoardBUS
    {
        public SoLuong5NamGanDay SoLuong5NamGanDay();
        public List<SoLuongThiSinhDuThiVaDo> SoLuong10NamGanDay();
        public List<SoLuongThiSinhDuThiVaDo> SoLuongThiSinhDuThiVaDo(int TuNam, int DenNam);
        public List<SoLuongThiSinhDuThiVaDo> ThongKeSoLuongTotNghiepQuaCacNam(ThongKeSoLuongTotNghiepQuaCacNamParams p);
    }
    public class DashBoardBUS : IDashBoardBUS
    {
        private IDashBoardDAL _DashBoardDAL;
        public DashBoardBUS(IDashBoardDAL DashBoardDAL)
        {
            _DashBoardDAL = DashBoardDAL;
        }
        public SoLuong5NamGanDay SoLuong5NamGanDay()
        {
            return _DashBoardDAL.SoLuong5NamGanDay();
        }
        public List<SoLuongThiSinhDuThiVaDo> SoLuong10NamGanDay()
        {
            return _DashBoardDAL.SoLuong10NamGanDay();
        }
        public List<SoLuongThiSinhDuThiVaDo> SoLuongThiSinhDuThiVaDo(int TuNam, int DenNam)
        {
            return _DashBoardDAL.SoLuongThiSinhDuThiVaDo(TuNam, DenNam);
        }
        public List<SoLuongThiSinhDuThiVaDo> ThongKeSoLuongTotNghiepQuaCacNam(ThongKeSoLuongTotNghiepQuaCacNamParams p)
        {
            return _DashBoardDAL.ThongKeSoLuongTotNghiepQuaCacNam(p);
        }
    }
}
