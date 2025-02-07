//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.Models
{
    public class BasePagingParams
    {
        public string Keyword { get; set; } = "";
        public string OrderByOption { get; set; } = "";
        public string OrderByName { get; set; } = "";
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public int Offset { get { return (PageSize == 0 ? 10 : PageSize) * ((PageNumber == 0 ? 1 : PageNumber) - 1); } }
        public int Limit { get { return (PageSize == 0 ? 10 : PageSize); } }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        /// <summary>
        /// format dd/MM/yyyy
        /// </summary> 
        public string TuNgayStr { get; set; }
        /// <summary>
        /// format dd/MM/yyyy
        /// </summary>
        public string DenNgayStr { get; set; }
        public DateTime StartDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(TuNgayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;

                }
            }
        }
        public DateTime EndDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(DenNgayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;

                }
            }
        }

        public int TieuChiThongKe { get; set; }
        public int? Nam { get; set; }
        public DateTime? Ngay { get; set; }
        public int? Truong { get; set; }
        public int? HoiDongID { get; set; }
        public string TenHoiDongThi { get; set; }
        public string SoQuyen { get; set; }
        public int? SoTrang { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? NgayCapBang { get; set; }
        public string SoHieuBang { get; set; }
    }
    public class BasePagingParamsOffset
    {
        public string Keyword { get; set; }
        public string OrderByOption { get; set; }
        public string OrderByName { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Offset { get { return (PageSize == 0 ? 10 : PageSize) * ((PageNumber == 0 ? 1 : PageNumber) - 1); } }
        public int Limit { get { return (PageSize == 0 ? 10 : PageSize); } }

        /// <summary>
        /// format dd/MM/yyyy
        /// </summary> 
        public string TuNgayStr { private get; set; }
        /// <summary>
        /// format dd/MM/yyyy
        /// </summary>
        public string DenNgayStr { private get; set; }
        public DateTime StartDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(TuNgayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
        public DateTime EndDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(DenNgayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
    }
    public class BaseDeleteParams
    {
        public int? ID { get; set; }
        public List<int> ListID { get; set; }
    }

    public class BasePagingParamsForFilter : BasePagingParams
    {
        public int? CoQuanID { get; set; }
        public int? CanBoID { get; set; }
        public int? Nam { get; set; }
        public int? TrangThai { get; set; }
        public int? LoaiDotKeKhai { get; set; }
        public string DotKeKhaiID { get; set; }
        public int? ApDungCho { get; set; }
        public int? GiaTri { get; set; }
        public int? NamKeKhai { get; set; }
        public int? ChucNangID { get; set; }
        public string NhomChucNang { get; set; }

    }
    public class NewParams
    {
        public int CanBoID { get; set; }
        public List<int?> DanhSachNhomTaiSanID { get; set; }
    }

    public class ThongKeParams
    {
        public int? NamKeKhai { get; set; }
        public int? CoQuanID { get; set; }
        public int? DotKekhaiID { get; set; }
        public int? TrangThai { get; set; }
        public int? CapQuanLy { get; set; }
        public int? LoaiKeKhai { get; set; }
        public int? Type { get; set; }
    }

    public class TraCuuParams : BasePagingParams
    {
        public string HoTen { get; set; }
        public int? DonViDaoTao { get; set; }
        public int? TruongTHPT { get; set; }
        public string Lop { get; set; }  
        public string CMND { get; set; }
        public int? NamCapBang { get; set; }
        public int? NamTotNghiep { get; set; }
        public int? HoiDongThiID { get; set; }
        public int? ThiSinhID { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string QuocGia { get; set; }
        public Boolean? GioiTinh { get; set; }
        public string SoBaoDanh { get; set; }
        public string NoiSinh { get; set; }

    }

    public class DuLieuDiemThiParams 
    {
        public int? NamID { get; set; }
        public string SoQuyenID { get; set; }
        public int? Type { get; set; }
    }

    public class CapNhatPhuLucParams
    {
        public int? Nam { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public int Offset { get { return (PageSize == 0 ? 10 : PageSize) * ((PageNumber == 0 ? 1 : PageNumber) - 1); } }
        public int Limit { get { return (PageSize == 0 ? 10 : PageSize); } }
    }

    public class ThongKeSoLuongTotNghiepQuaCacNamParams
    {
        public int? Nam { get; set; }
    } 
    public class DanhSachTotNghiepQuaCacNamParams
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public int? Nam { get; set; }
        public int? TieuChiThongKe { get; set; }
    }

    public class TraCuuVanBangParams
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string CMND { get; set; }
        public string SoHieuBang { get; set; }
        public int? NamTotNghiep { get; set; }
        public int? HoiDongThiID { get; set; }
    }

}
