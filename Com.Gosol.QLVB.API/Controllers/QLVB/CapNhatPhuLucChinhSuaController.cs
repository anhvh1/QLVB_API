using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.FileDinhKem;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Com.Gosol.QLVB.API.Controllers.QLVB
{
    [Route("api/v1/CapNhatPhuLucChinhSua")]
    [ApiController]
    public class CapNhatPhuLucChinhSuaController : BaseApiController
    {
        private ICapNhatPhuLucChinhSuaBUS _CapNhatPhuLucChinhSuaBUS;
        private IMauPhieuBUS _mauPhieuBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public CapNhatPhuLucChinhSuaController(ICapNhatPhuLucChinhSuaBUS CapNhatPhuLucChinhSuaBUS, IMauPhieuBUS mauPhieuBUS, IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<CapNhatPhuLucChinhSuaController> logger) : base(_LogHelper, logger)
        {
            this._CapNhatPhuLucChinhSuaBUS = CapNhatPhuLucChinhSuaBUS;
            this._mauPhieuBUS = mauPhieuBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery] CapNhatPhuLucParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    int TotalRow = 0;
                    List<CapNhatPhuLucChinhSuaModel> Result = new List<CapNhatPhuLucChinhSuaModel>();
                    var Data = _CapNhatPhuLucChinhSuaBUS.GetBySearchNamTotNghiep(p);   
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    else
                    {
                        if(Data[0].children != null && Data[0].children.Count > 0)
                        {
                            var listPhuLuc = Data[0].children.OrderByDescending(x => x.CapNhatPhuLucID).ToList().Skip(p.Offset).Take(p.Limit);
                            foreach (var item in listPhuLuc)
                            {
                                var tmp = _CapNhatPhuLucChinhSuaBUS.GetByID(item.CapNhatPhuLucID ?? 0);
                                if (tmp != null && tmp.CapNhatPhuLucID > 0)
                                {
                                    var listFileDinhKem = _FileDinhKemBUS.GetByNghiepVuID(tmp.CapNhatPhuLucID, EnumLoaiFileDinhKem.FileCapNhatPhuLuc.GetHashCode());
                                    if (listFileDinhKem != null && listFileDinhKem.Count > 0)
                                    {
                                        var clsCommon = new Commons();
                                        string serverPath = clsCommon.GetServerPath(HttpContext);
                                        foreach (var f in listFileDinhKem)
                                        {
                                            var path = _host.ContentRootPath + "\\" + f.FileUrl;
                                            f.FileUrl = serverPath + f.FileUrl;
                                        }
                                    }
                                    tmp.FileQuyetDinh = listFileDinhKem.FirstOrDefault();
                                }
                                Result.Add(tmp);
                            }
                        }
                    }
                    base.Status = 1;
                    base.TotalRow = Result.Count;
                    base.Data = Result;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Create)]
        [Route("Insert")]
        public async Task<IActionResult> InsertAsync(IFormFile file, [FromForm] string PhuLucChinhSuaStr)
        {
            try
            {
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                var PhuLucChinhSua = JsonConvert.DeserializeObject<CapNhatPhuLucChinhSuaModel>(PhuLucChinhSuaStr);
                var clsCommon = new Commons();
                var Result = _CapNhatPhuLucChinhSuaBUS.Insert(PhuLucChinhSua, CanBoID);
                var ID = Utils.ConvertToInt32(Result.Data, 0);
                if (ID > 0 && file != null)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = ID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileCapNhatPhuLuc.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileCapNhatPhuLuc);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    var url = await clsCommon.InsertFileAsync(file, FileDinhKem, _host, _FileDinhKemBUS);
                    if (url != null && url.Length > 0)
                    {
                        ListFileUrl.Add(url);
                    }
                }

                base.Status = Result.Status;
                base.Message = Result.Message;
                base.Data = Result.Data;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.Message = ConstantLogMessage.API_Error_System;
                //return base.GetActionResult();
                //throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Edit)]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsync(IFormFile file, [FromForm] string PhuLucChinhSuaStr)
        {
            try
            {
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                var PhuLucChinhSua = JsonConvert.DeserializeObject<CapNhatPhuLucChinhSuaModel>(PhuLucChinhSuaStr);
                var clsCommon = new Commons();
                var Result = _CapNhatPhuLucChinhSuaBUS.Update(PhuLucChinhSua, CanBoID);
                if (file != null && PhuLucChinhSua.CapNhatPhuLucID > 0)
                {
                    //delete file cu
                    var data = _CapNhatPhuLucChinhSuaBUS.GetByID(PhuLucChinhSua.CapNhatPhuLucID);
                    if(data != null && data.FileQuyetDinh != null)
                    {
                        List<int> listFile = new List<int>();
                        listFile.Add(data.FileQuyetDinh.FileID);
                        _FileDinhKemBUS.Delete(listFile);
                    }
                    //insert file moi
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = PhuLucChinhSua.CapNhatPhuLucID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileCapNhatPhuLuc.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileCapNhatPhuLuc);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    var url = await clsCommon.InsertFileAsync(file, FileDinhKem, _host, _FileDinhKemBUS);
                    if (url != null && url.Length > 0)
                    {
                        ListFileUrl.Add(url);
                    }
                }
                base.Status = Result.Status;
                base.Message = Result.Message;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.Message = ConstantLogMessage.API_Error_System;
                //return base.GetActionResult();
                //throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete(CapNhatPhuLucChinhSuaModel p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Result = _CapNhatPhuLucChinhSuaBUS.Delete(p.CapNhatPhuLucID);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }

        [HttpGet]
        [Route("GetByID")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery] int? CapNhatPhuLucID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Data = _CapNhatPhuLucChinhSuaBUS.GetByID(CapNhatPhuLucID ?? 0);
                    if (Data != null && Data.CapNhatPhuLucID > 0)
                    {
                        //if(Data.ThongTinThiSinh != null && Data.ThongTinThiSinh.Count > 0)
                        //{
                        //    foreach (var ts in Data.ThongTinThiSinh)
                        //    {
                        //        if(ts.ThongTinChinhSua != null && ts.ThongTinChinhSua.Count > 0)
                        //        {
                        //            var MauPhieu = _mauPhieuBUS.GetByID(ts.MauPhieuID);
                        //            if(MauPhieu.DanhSachChiTietMauPhieu != null)
                        //            {
                        //                foreach (var tt in ts.ThongTinChinhSua)
                        //                {
                        //                    foreach (var item in MauPhieu.DanhSachChiTietMauPhieu)
                        //                    {
                        //                        if(tt.Ma == item.MaCot)
                        //                        {

                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        var listFileDinhKem = _FileDinhKemBUS.GetByNghiepVuID(Data.CapNhatPhuLucID, EnumLoaiFileDinhKem.FileCapNhatPhuLuc.GetHashCode());
                        if (listFileDinhKem != null && listFileDinhKem.Count > 0)
                        {
                            var clsCommon = new Commons();
                            string serverPath = clsCommon.GetServerPath(HttpContext);
                            foreach (var item in listFileDinhKem)
                            {
                                var path = _host.ContentRootPath + "\\" + item.FileUrl;
                                item.FileUrl = serverPath + item.FileUrl;  
                            }
                        }
                        Data.FileQuyetDinh = listFileDinhKem.FirstOrDefault();
                        base.Message = ConstantLogMessage.API_Success;
                        base.Data = Data;
                    }
                    else
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                    }
                    base.Status = 1;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //return base.GetActionResult();
                //throw;
            }
        }


        [HttpGet] 
        [Route("GetNamTotNghiepTree")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetNamTotNghiepTree([FromQuery] CapNhatPhuLucParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    int TotalRow = 0;
                    IList<NamTotNghiepTree> Data;
                    Data = _CapNhatPhuLucChinhSuaBUS.GetBySearchNamTotNghiep(p);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    else
                    {
                        foreach (var item in Data)
                        {
                            item.children = new List<NamTotNghiepTree>();
                        }
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                //base.Message = ex.Message;
                //base.Status = -1;
                //return base.GetActionResult();

                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [Route("GetMaByMauPhieuID")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByID(int MauPhieuID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_SystemConfig_GetByID, EnumLogType.GetByID, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Data = _mauPhieuBUS.GetByID(MauPhieuID);
                    if (Data == null || Data.MauPhieuID == null || Data.MauPhieuID <= 0)
                    {
                        base.Message = "Không có Dữ liệu";
                        base.Status = 0;
                    }
                    else
                    {
                        List<ChiTietMauPhieuModel> chiTietMauPhieuModels = new List<ChiTietMauPhieuModel>();
                        if (Data.DanhSachChiTietMauPhieu != null && Data.DanhSachChiTietMauPhieu.Count > 0)
                        { 
                            chiTietMauPhieuModels = Data.DanhSachChiTietMauPhieu.Where(x => x.MaCot.Contains("BODY_") && x.MaCot != "BODY_ĐL12" 
                                                                                                                      && x.MaCot != "BODY_ĐT"
                                                                                                                      && x.MaCot != "BODY_ĐPK"
                                                                                                                      && x.MaCot != "BODY_ĐBL"
                                                                                                                      && x.MaCot != "BODY_KETLUAN").ToList();
                        }
                       
                        base.Message = " ";
                        base.Status = 1;
                        base.Data = chiTietMauPhieuModels;
                    }

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw ex;
            }

        }
    }
}