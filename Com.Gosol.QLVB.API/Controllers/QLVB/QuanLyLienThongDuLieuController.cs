using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.BUS.ThongKe;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.API.Controllers.HeThong.QuanLyLienThongDuLieu
{
    [Route("api/v1/QuanLyLienThongDuLieu")]
    [ApiController]
    public class QuanLyLienThongDuLieuController : BaseApiController
    {
        private IDashBoardBUS _DashBoardBUS;
        private IThongKeBUS _thongKeBUS;
        private IThongTinCapBangBUS _ThongTinCapBangBUS;
        public QuanLyLienThongDuLieuController(IThongTinCapBangBUS ThongTinCapBangBUS, IDashBoardBUS DashBoardBUS, IThongKeBUS thongKeBUS, ILogHelper _LogHelper, ILogger<QuanLyLienThongDuLieuController> logger) : base(_LogHelper, logger)
        {
            this._thongKeBUS = thongKeBUS;
            this._DashBoardBUS = DashBoardBUS;
            this._ThongTinCapBangBUS = ThongTinCapBangBUS;
        }

        [HttpGet]
        [Route("DanhSachTotNghiepQuaCacNam")]
        //[CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult DanhSachTotNghiepQuaCacNam([FromQuery] DanhSachTotNghiepQuaCacNamParams info, [FromHeader] string Ticket)
        {
            try
            {
                if(Ticket != null && Ticket.Length > 0 && _ThongTinCapBangBUS.CheckTicket(Ticket))
                {
                    BasePagingParams p = new BasePagingParams();
                    p.Nam = info.Nam;
                    p.PageSize = info.PageSize;
                    p.PageNumber = info.PageNumber;
                    p.TieuChiThongKe = info.TieuChiThongKe ?? 0;
                    int TotalRow = 0;
                    List<ChiTietDuLieuDiemThiModel> Data;
                    Data = _thongKeBUS.GetThongKe(p, ref TotalRow);
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    if (Data.Count > 0)
                    {
                        base.Data = Data;
                    }
                    else
                    {
                        base.Message = Constant.NO_DATA;
                    }
                    return base.GetActionResult();
                }
                else
                {
                    base.Status = 0;
                    base.Message = Constant.NOT_ACCESS;
                    return base.GetActionResult();
                }
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw ex;
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ThongKeSoLuongTotNghiepQuaCacNam")]
        public IActionResult ThongKeSoLuongTotNghiepQuaCacNam([FromQuery] ThongKeSoLuongTotNghiepQuaCacNamParams info, [FromHeader] string Ticket)
        {
            try
            {
                if (Ticket != null && Ticket.Length > 0 && _ThongTinCapBangBUS.CheckTicket(Ticket))
                {
                    int TotalRow = 0;
                    List<SoLuongThiSinhDuThiVaDo> Data = new List<SoLuongThiSinhDuThiVaDo>();
                    Data = _DashBoardBUS.ThongKeSoLuongTotNghiepQuaCacNam(info);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;

                    return base.GetActionResult();
                }
                else
                {
                    base.Status = 0;
                    base.Message = Constant.NOT_ACCESS;
                    return base.GetActionResult();
                }
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                //base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }
        }

        [HttpGet]
        [Route("TraCuuThongTinVanBang")]
        //[CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult TraCuuThongTinVanBang([FromQuery] TraCuuVanBangParams info, [FromHeader] string Ticket)
        {
            try
            {
                if (Ticket != null && Ticket.Length > 0 && _ThongTinCapBangBUS.CheckTicket(Ticket))
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    if ((info.SoHieuBang != null && info.SoHieuBang.Length > 0)
                    || (info.NgaySinh != null)
                    || (info.CMND != null && info.CMND.Length > 0)
                    || (info.HoTen != null && info.HoTen.Length > 0)
                    || (info.NamTotNghiep != null && info.NamTotNghiep > 0)
                    || (info.HoiDongThiID != null && info.HoiDongThiID > 0))
                    {
                        TraCuuParams p = new TraCuuParams();
                        p.TrangThai = 0;
                        p.Keyword = info.SoHieuBang;
                        p.HoTen = info.HoTen;
                        p.NgaySinh = info.NgaySinh;
                        p.CMND = info.CMND;
                        p.NamTotNghiep = info.NamTotNghiep;
                        p.HoiDongThiID = info.HoiDongThiID;
                        p.PageNumber = info.PageNumber;
                        p.PageSize = info.PageSize;

                        Data = _ThongTinCapBangBUS.TraCuuVanBangChungChi(p, ref TotalRow);

                        if (Data.Count == 0)
                        {
                            base.Status = 1;
                            base.Message = ConstantLogMessage.API_NoData;
                            return base.GetActionResult();
                        }
                        base.Status = 1;
                        base.TotalRow = TotalRow;
                        base.Data = Data;

                        return base.GetActionResult();
                    }
                    else
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                }
                else
                {
                    base.Status = 0;
                    base.Message = Constant.NOT_ACCESS;
                    return base.GetActionResult();
                }
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
            }
        }
    }
}
