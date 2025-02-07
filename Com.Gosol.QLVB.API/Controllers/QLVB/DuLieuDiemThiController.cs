using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.DanhMuc;
using Com.Gosol.QLVB.BUS.FileDinhKem;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZXing;
using System.IO;
using Microsoft.Extensions.FileProviders;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using OfficeOpenXml;

namespace Com.Gosol.QLVB.API.Controllers.DanhMuc
{
    [Route("api/v1/DuLieuDiemThi")]
    [ApiController]
    public class DuLieuDiemThiController : BaseApiController
    {
        private IDuLieuDiemThiBUS _DuLieuDiemThiBUS;
        private IQuanLyThiSinhBUS _QuanLyThiSinhBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public DuLieuDiemThiController(IDuLieuDiemThiBUS DuLieuDiemThiBUS, IQuanLyThiSinhBUS QuanLyThiSinhBUS, IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DuLieuDiemThiController> logger) : base(_LogHelper, logger)
        {
            this._DuLieuDiemThiBUS = DuLieuDiemThiBUS;
            this._QuanLyThiSinhBUS = QuanLyThiSinhBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p, int? NamThi)
        {
            try
            {
                int TotalRow = 0;
                p.Nam = p.Nam ?? NamThi;
                IList<ThongTinThiSinh> Data;
                Data = _DuLieuDiemThiBUS.GetThiSinh(p, ref TotalRow);
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
            catch (Exception ex)
            {
                //_logger.LogError(ex.ToString());
                //return base.GetActionResultErrorAPI();

                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
                //throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("DanhSachCongNhanTotNghiep")]
        public IActionResult DanhSachCongNhanTotNghiep([FromQuery] BasePagingParams p, int? NamThi)
        {
            try
            {
                int TotalRow = 0;
                p.Nam = p.Nam ?? NamThi;
                p.PageSize = 999999;
                IList<ThongTinThiSinh> Data;
                p.TrangThai = 1;
                Data = _DuLieuDiemThiBUS.GetThiSinh(p, ref TotalRow);
                if (Data.Count == 0)
                {
                    base.Status = 1;
                    base.Message = ConstantLogMessage.API_NoData;
                    return base.GetActionResult();
                }
                else
                {
                    string rootPath = _host.ContentRootPath;
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext);
                    string pathFile = @"Upload\Temp\DanhSachCongNhanTotNghiep_" + CanBoID + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";

                    string path = rootPath + @"\Upload\ThongTinThiSinh_Template.xlsx";
                    FileInfo fileInfo = new FileInfo(path);
                    FileInfo file = new FileInfo(rootPath + "\\" + pathFile);

                    ExcelPackage package = new ExcelPackage(fileInfo);
                    if (package.Workbook.Worksheets != null)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // get number of rows in the sheet
                        int rows = worksheet.Dimension.Rows;
                        int cols = worksheet.Dimension.Columns;

                        if (Data.Count > 0)
                        {
                            worksheet.InsertRow(5, Data.Count - 1, 4);
                            //worksheet.DeleteRow(data.Count);
                            for (int i = 0; i < Data.Count; i++)
                            {
                                int stt = 1;
                                worksheet.Cells[i + 4, 1].Value = stt.ToString();
                                worksheet.Cells[i + 4, 2].Value = Data[i].NamThi;
                                worksheet.Cells[i + 4, 3].Value = Data[i].HoTen;
                                worksheet.Cells[i + 4, 4].Value = Data[i].NgaySinhStr;
                                worksheet.Cells[i + 4, 5].Value = Data[i].NoiSinh;
                                worksheet.Cells[i + 4, 6].Value = Data[i].TenTruongTHPT;
                                stt++;
                            }

                        }

                        // save changes
                        package.SaveAs(file);
                    }
                    base.Data = serverPath + pathFile;
                }
                base.Status = 1;
                base.TotalRow = TotalRow;
                

                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.ToString());
                //return base.GetActionResultErrorAPI();

                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
                //throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("DanhSachXetTotNghiep")]
        public IActionResult DanhSachXetTotNghiep([FromQuery] BasePagingParams p, int? NamThi)
        {
            try
            {
                int TotalRow = 0;
                p.Nam = p.Nam ?? NamThi;
                p.PageSize = 999999;
                IList<ThongTinThiSinh> Data;
                Data = _DuLieuDiemThiBUS.GetThiSinh(p, ref TotalRow);
                if (Data.Count == 0)
                {
                    base.Status = 1;
                    base.Message = ConstantLogMessage.API_NoData;
                    return base.GetActionResult();
                }
                else
                {
                    string rootPath = _host.ContentRootPath;
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext);
                    string pathFile = @"Upload\Temp\DanhSachXetTotNghiep_" + CanBoID + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";

                    string path = rootPath + @"\Upload\ThongTinThiSinh_Template.xlsx";
                    FileInfo fileInfo = new FileInfo(path);
                    FileInfo file = new FileInfo(rootPath + "\\" + pathFile);

                    ExcelPackage package = new ExcelPackage(fileInfo);
                    if (package.Workbook.Worksheets != null)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // get number of rows in the sheet
                        int rows = worksheet.Dimension.Rows;
                        int cols = worksheet.Dimension.Columns;

                        if (Data.Count > 0)
                        {
                            worksheet.InsertRow(5, Data.Count - 1, 4);
                            //worksheet.DeleteRow(data.Count);
                            for (int i = 0; i < Data.Count; i++)
                            {
                                int stt = 1;
                                worksheet.Cells[i + 4, 1].Value = stt.ToString();
                                worksheet.Cells[i + 4, 2].Value = Data[i].NamThi;
                                worksheet.Cells[i + 4, 3].Value = Data[i].HoTen;
                                worksheet.Cells[i + 4, 4].Value = Data[i].NgaySinhStr;
                                worksheet.Cells[i + 4, 5].Value = Data[i].NoiSinh;
                                worksheet.Cells[i + 4, 6].Value = Data[i].TenTruongTHPT;
                                stt++;
                            }

                        }

                        // save changes
                        package.SaveAs(file);
                    }
                    base.Data = serverPath + pathFile;
                }
                base.Status = 1;
                base.TotalRow = TotalRow;

                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.ToString());
                //return base.GetActionResultErrorAPI();

                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
                //throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetListPaging_New")]
        public IActionResult GetListPaging_New([FromQuery]BasePagingParams p, int? NamThi, int? Truong)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinToChucThi> Data;
                    Data = _DuLieuDiemThiBUS.GetPagingBySearch_New(p, ref TotalRow, CanBoID);
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetListPaging_NNC")]
        public IActionResult GetListPaging_NNC([FromQuery] BasePagingParams p, int? NamThi, int? Truong)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinToChucThi> Data;
                    Data = _DuLieuDiemThiBUS.GetPagingBySearch_NNC(p, ref TotalRow, CanBoID);
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetNamThiTree")]
        public IActionResult GetNamThiTree([FromQuery] DuLieuDiemThiParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<NamThiTree> Data;
                    Data = _DuLieuDiemThiBUS.GetBySearchNamThi(p, CanBoID);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    foreach (var item in Data)
                    {
                        if(item.children != null && item.children.Count > 0 && p.Type > 0)
                        {
                            item.children = item.children.Where(x => x.Type == p.Type).ToList();
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetNamThiTree_VBNN")]
        public IActionResult GetNamThiTree_VBNN([FromQuery] DuLieuDiemThiParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<NamThiTree> Data;
                    Data = _DuLieuDiemThiBUS.GetBySearchNamThiVBNN(p);
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetNamThi")]
        public IActionResult GetNamThi()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<NamThiTree> Data;
                    DuLieuDiemThiParams p = new DuLieuDiemThiParams();
                    Data = _DuLieuDiemThiBUS.GetBySearchNamThi(p, CanBoID);
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
                            item.children = null;
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
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetQuyenByNamThi")]
        public IActionResult GetQuyenByNamThi([FromQuery] DuLieuDiemThiParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    List<NamThiTree> Result = new List<NamThiTree>();
                    if(p.NamID == null || p.NamID == 0)
                    {
                        base.Status = 1;
                        return base.GetActionResult();
                    }
                    var Data = _DuLieuDiemThiBUS.GetBySearchNamThi(p, CanBoID);
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
                            if (item.children != null) Result.AddRange(item.children);
                        }
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Result;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetDanhSachQuyen")]
        public IActionResult GetDanhSachQuyen([FromQuery] DuLieuDiemThiParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    List<NamThiTree> Result = new List<NamThiTree>(); 
                    var Data = _DuLieuDiemThiBUS.GetBySearchNamThi(p, CanBoID);
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
                            if (item.children != null) Result.AddRange(item.children);
                        }
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Result;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DuLieuDiemThiModel DuLieuDiemThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_Them, EnumLogType.Insert, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                    DuLieuDiemThiModel.CoQuanID = CoQuanID;
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var Result = _DuLieuDiemThiBUS.Insert(DuLieuDiemThiModel, CanBoID);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    base.Data = Result.Data;
                    return base.GetActionResult();
                });
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("Insert_New")]
        public async Task<IActionResult> Insert_NewAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                DuLieuDiemThiModel.CoQuanID = CoQuanID;
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                var Result = _DuLieuDiemThiBUS.Insert(DuLieuDiemThiModel, CanBoID);
                var KyThiID = Utils.ConvertToInt32(Result.Data, 0);
                if(KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
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
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("InsertThiSinh")]
        public async Task<IActionResult> InsertThiSinh_NewAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                DuLieuDiemThiModel.CoQuanID = CoQuanID;
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                //check trung cccd
                if(DuLieuDiemThiModel != null && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                {
                    DuLieuDiemThiModel.ListErrorThiSinh = new List<ErrorThongTinThiSinh>();
                    var checktrung = false;
                    int stt = 0;
                    foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                    {
                        if(item.CMND != null && item.CMND.Length > 0)
                        {
                            var Data = _DuLieuDiemThiBUS.CheckTrungCCCD(item.ThiSinhID, item.CMND);
                            if (Data != null && Data.ThiSinhID > 0)
                            {
                                item.DanhSachLoi = "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                checktrung = true;

                                ErrorThongTinThiSinh er = new ErrorThongTinThiSinh("BODY_CMND/CCCD", stt, item.DanhSachLoi, true);
                                DuLieuDiemThiModel.ListErrorThiSinh.Add(er);
                            }
                        }                        
                        stt++;
                    }
                    if (checktrung)
                    {
                        base.Status = 0;
                        base.Message = "Trùng CMND/CCCD với thí sinh trong hệ thống";
                        base.Data = DuLieuDiemThiModel;
                        return base.GetActionResult();
                    }
                }
                var Result = _DuLieuDiemThiBUS.Insert(DuLieuDiemThiModel, CanBoID);
                var KyThiID = Utils.ConvertToInt32(Result.Data, 0);
                if (KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
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
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("InsertForImport_Old")]
        public IActionResult InsertForImport_Old(DuLieuDiemThiModel DuLieuDiemThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_ImportExcel, EnumLogType.Insert, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var Result = _DuLieuDiemThiBUS.InsertForImportExcel(DuLieuDiemThiModel, CanBoID);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    base.Data = Result.Data;
                    return base.GetActionResult();
                });
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("InsertForImport")]
        public async Task<IActionResult> InsertForImportAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                var Result = _DuLieuDiemThiBUS.InsertForImportExcel(DuLieuDiemThiModel, CanBoID);
                var KyThiID = Utils.ConvertToInt32(Result.Data, 0);
                if (KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
                    }
                }
                base.Status = Result.Status;
                base.Message = Result.Message;
                base.Data = Result.Data;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                //return base.GetActionResult();
                _logger.LogError(ex.ToString());
                return base.GetActionResult();
                //return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("InsertForImportExcelAllPages")]
        public async Task<IActionResult> InsertForImportExcelAllPagesAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                var Result = _DuLieuDiemThiBUS.InsertForImportExcelAllPages(DuLieuDiemThiModel, CanBoID);
                var KyThiID = Utils.ConvertToInt32(Result.Data, 0);
                if (KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
                    }
                }
                base.Status = Result.Status;
                base.Message = Result.Message;
                base.Data = Result.Data;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                //return base.GetActionResult();
                _logger.LogError(ex.ToString());
                return base.GetActionResult();
                //return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DuLieuDiemThiModel DuLieuDiemThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Update, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var Result = _DuLieuDiemThiBUS.Update(DuLieuDiemThiModel,CanBoID);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();
                });
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("UpdateThongTinCapBang")]
        public IActionResult UpdateThongTinCapBang(ThongTinThiSinh ThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Update, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var Result = _DuLieuDiemThiBUS.UpdateThongTinCapBang(ThongTinThiSinh, CanBoID);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();
                });
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("Update_New")]
        public async Task<IActionResult> Update_NewAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                var Result = _DuLieuDiemThiBUS.Update(DuLieuDiemThiModel, CanBoID);
                if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
                    }
                }
                if(DuLieuDiemThiModel.DSXoaFileDinhKem != null && DuLieuDiemThiModel.DSXoaFileDinhKem.Count > 0)
                {
                    var listID = DuLieuDiemThiModel.DSXoaFileDinhKem.Select(x => x.FileID).ToList();
                    _FileDinhKemBUS.Delete(listID);
                }
                if(DuLieuDiemThiModel.ListThiSinhDelete != null && DuLieuDiemThiModel.ListThiSinhDelete.Count > 0)
                {
                    foreach (var item in DuLieuDiemThiModel.ListThiSinhDelete)
                    {
                        _QuanLyThiSinhBUS.Delete(item.ThiSinhID);
                    }
                }
                base.Status = Result.Status;
                base.Message = Result.Message;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();

                //_logger.LogError(ex.ToString());
                //return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("UpdateThiSinh")]
        public async Task<IActionResult> UpdateThiSinh_NewAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                //check trung cccd
                if (DuLieuDiemThiModel != null && DuLieuDiemThiModel.ThongTinThiSinh != null && DuLieuDiemThiModel.ThongTinThiSinh.Count > 0)
                {
                    DuLieuDiemThiModel.ListErrorThiSinh = new List<ErrorThongTinThiSinh>();
                    var checktrung = false;
                    int stt = 0;
                    foreach (var item in DuLieuDiemThiModel.ThongTinThiSinh)
                    {
                        if (item.CMND != null && item.CMND.Length > 0)
                        {
                            var Data = _DuLieuDiemThiBUS.CheckTrungCCCD(item.ThiSinhID, item.CMND);
                            if (Data != null && Data.ThiSinhID > 0)
                            {
                                item.DanhSachLoi = "Trùng CMND/CCCD với thí sinh trong hệ thống";
                                checktrung = true;

                                ErrorThongTinThiSinh er = new ErrorThongTinThiSinh("BODY_CMND/CCCD", stt, item.DanhSachLoi, true);
                                DuLieuDiemThiModel.ListErrorThiSinh.Add(er);
                            }
                        }
                        stt++;
                    }
                    if (checktrung)
                    {
                        base.Status = 0;
                        base.Message = "Trùng CMND/CCCD với thí sinh trong hệ thống";
                        base.Data = DuLieuDiemThiModel;
                        return base.GetActionResult();
                    }  
                }
                var Result = _DuLieuDiemThiBUS.Update(DuLieuDiemThiModel, CanBoID);
                if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
                    }
                }
                if (DuLieuDiemThiModel.DSXoaFileDinhKem != null && DuLieuDiemThiModel.DSXoaFileDinhKem.Count > 0)
                {
                    var listID = DuLieuDiemThiModel.DSXoaFileDinhKem.Select(x => x.FileID).ToList();
                    _FileDinhKemBUS.Delete(listID);
                }
                base.Status = Result.Status;
                base.Message = Result.Message;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();

                //_logger.LogError(ex.ToString());
                //return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete(ThongTinThiSinh p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _QuanLyThiSinhBUS.Delete(p.ThiSinhID);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("DeleteThongTinCapBang")]
        public IActionResult DeleteThongTinCapBang(BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    if(p.ListID != null && p.ListID.Count > 0)
                    foreach (var ThiSinhID in p.ListID)
                    {
                            var Result = _QuanLyThiSinhBUS.Delete(ThiSinhID);
                            base.Message = Result.Message;
                            base.Status = Result.Status;
                    }
                   
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [Route("GetByID")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery]int? KyThiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DuLieuDiemThiBUS.GetByID(KyThiID ?? 0);
                    if (Data != null && Data.ThongTinToChucThi != null && Data.ThongTinToChucThi.KyThiID > 0)
                    {
                        var listFileDinhKem = _FileDinhKemBUS.GetByNghiepVuID(Data.ThongTinToChucThi.KyThiID, EnumLoaiFileDinhKem.FileBangDiem.GetHashCode());
                        if(listFileDinhKem != null && listFileDinhKem.Count > 0)
                        {
                            var clsCommon = new Commons();
                            string serverPath = clsCommon.GetServerPath(HttpContext);
                            foreach (var item in listFileDinhKem)
                            {
                                var path = _host.ContentRootPath + "\\" + item.FileUrl;
                                item.FileUrl = serverPath + item.FileUrl;                               
                                //try
                                //{                           
                                //    Byte[] bytes = System.IO.File.ReadAllBytes(path);
                                //    item.Base64String = Convert.ToBase64String(bytes);
                                //    item.TepByte = bytes;
                                //}
                                //catch (Exception) { }
                            }
                        }
                        Data.DSFileDinhKem = listFileDinhKem;
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
        [Route("GetDanhSachFileByDiemThi")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult GetDanhSachFileByDiemThi([FromQuery] int? KyThiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DuLieuDiemThiBUS.GetThongTinKyThi(KyThiID ?? 0);
                    //DuLieuDiemThiModel Data = new DuLieuDiemThiModel();
                    //Data.ThongTinToChucThi = new ThongTinToChucThi();
                    //Data.ThongTinToChucThi.KyThiID = KyThiID ?? 0;
                    if (KyThiID != null && KyThiID > 0)
                    {
                        var listFileDinhKem = _FileDinhKemBUS.GetByNghiepVuID(KyThiID.Value, EnumLoaiFileDinhKem.FileBangDiem.GetHashCode());
                        if (listFileDinhKem != null && listFileDinhKem.Count > 0)
                        {
                            var clsCommon = new Commons();
                            string serverPath = clsCommon.GetServerPath(HttpContext);
                            foreach (var item in listFileDinhKem)
                            {
                                var path = _host.ContentRootPath + "\\" + item.FileUrl;
                                item.FileUrl = serverPath + item.FileUrl;
                                //try
                                //{
                                //    Byte[] bytes = System.IO.File.ReadAllBytes(path);
                                //    item.Base64String = Convert.ToBase64String(bytes);
                                //    item.TepByte = bytes;
                                //}
                                //catch (Exception) { }
                            }
                        }
                        Data.DSFileDinhKem = listFileDinhKem;
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

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("UpdateDanhSachFileByDiemThi")]
        public async Task<IActionResult> UpdateDanhSachFileByDiemThiAsync(IList<IFormFile> files, [FromForm] string DuLieuDiemThiModelStr)
        {
            try
            {
                var DuLieuDiemThiModel = JsonConvert.DeserializeObject<DuLieuDiemThiModel>(DuLieuDiemThiModelStr);
                var clsCommon = new Commons();
                var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                //var Result = _DuLieuDiemThiBUS.Update(DuLieuDiemThiModel, CanBoID);
                if (DuLieuDiemThiModel.ThongTinToChucThi != null && DuLieuDiemThiModel.ThongTinToChucThi.KyThiID > 0 && files != null && files.Count > 0)
                {
                    FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                    FileDinhKem.NghiepVuID = DuLieuDiemThiModel.ThongTinToChucThi.KyThiID;
                    FileDinhKem.FileType = EnumLoaiFileDinhKem.FileBangDiem.GetHashCode();
                    FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    List<String> ListFileUrl = new List<String>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
                    }
                }
                if (DuLieuDiemThiModel.DSXoaFileDinhKem != null && DuLieuDiemThiModel.DSXoaFileDinhKem.Count > 0)
                {
                    var listID = DuLieuDiemThiModel.DSXoaFileDinhKem.Select(x => x.FileID).ToList();
                    _FileDinhKemBUS.Delete(listID);
                }
                base.Status = 1;
                base.Message = "Cập nhật file thành công";
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("Duyet")]
        public IActionResult Update_TrangThai(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Delete, () =>
                {
                    var Result = _DuLieuDiemThiBUS.Update_TrangThaiDiemThi(ListThongTinThiSinh);
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

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("DuyetBangDiemThi")]
        public IActionResult DuyetBangDiemThi(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Delete, () =>
                {
                    var Result = _DuLieuDiemThiBUS.Update_TrangThai(ListThongTinToChucThi);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("UpdateTrangThaiKhoa")]
        public IActionResult UpdateTrangThaiKhoa(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Delete, () =>
                {
                    var Result = _DuLieuDiemThiBUS.Update_TrangThaiKhoa(ListThongTinToChucThi);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("UpdateTrangThaiTrang")]
        public IActionResult UpdateTrangThaiTrang(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Delete, () =>
                {
                    var Result = _DuLieuDiemThiBUS.Update_TrangThaiTrang(ListThongTinToChucThi);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("UpdateTrangThaiQuyen")]
        public IActionResult UpdateTrangThaiQuyen(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            try
            {
                if(ListThongTinToChucThi != null && ListThongTinToChucThi.Count > 0)
                {
                    int TotalRow = 0;
                    foreach (var item in ListThongTinToChucThi)
                    {
                        BasePagingParams p = new BasePagingParams();
                        p.SoQuyen = item.SoQuyen;
                        p.Nam = item.Nam;
                        var data = _DuLieuDiemThiBUS.GetPagingBySearch_New(p, ref TotalRow, CanBoID);
                        if(data != null && data.Count > 0)
                        {
                            foreach (var trang in data)
                            {
                                trang.Type = item.Type;
                            }
                            var Result = _DuLieuDiemThiBUS.Update_TrangThaiTrang(data);
                            base.Message = Result.Message;
                            base.Status = Result.Status;
                        }
                    }
                }

                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [Route("CheckTrungSoQuyenSoTrang")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult CheckTrungSoQuyenSoTrang([FromQuery] int? SoTrang, [FromQuery] string SoQuyen, [FromQuery] string TenHoiDongThi, [FromQuery] DateTime? KhoaThiNgay)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DuLieuDiemThiBUS.CheckTrungSoQuyenSoTrang(SoTrang, SoQuyen, TenHoiDongThi, KhoaThiNgay);
                    if (Data != null && Data.ThongTinToChucThi != null && Data.ThongTinToChucThi.KyThiID > 0)
                    {
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
            }
        }

        //[HttpGet]
        //[Route("GetThiSinhLogByKyThiID")]
        ////[CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        //public IActionResult GetThiSinhLogByKyThiID([FromQuery] int? KyThiID)
        //{
        //    try
        //    {
        //        return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
        //        {
        //            var Data = _DuLieuDiemThiBUS.GetThiSinhLog(KyThiID ?? 0);
        //            if (Data != null)
        //            {
        //                base.Message = ConstantLogMessage.API_Success;
        //                base.Data = Data;
        //            }
        //            else
        //            {
        //                base.Message = ConstantLogMessage.API_NoData;
        //            }
        //            base.Status = 1;

        //            return base.GetActionResult();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        return base.GetActionResultErrorAPI();

        //        //base.Status = -1;
        //        //return base.GetActionResult();
        //        //throw;
        //    }
        //}

        [HttpGet]
        [Route("GetChiTietThiSinhLogByThiSinhID")]
        //[CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult GetThiSinhLogByKyThiID(int ThiSinhID, DateTime? NgayChinhSua)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DuLieuDiemThiBUS.GetChiTietThiSinhLog(ThiSinhID , NgayChinhSua);
                    if (Data != null)
                    {
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetSoTrangByQuyen")]
        public IActionResult GetSoTrangByQuyen([FromQuery] BasePagingParams p)
        {
            try
            {
                int TotalRow = 0;
                IList<ThongTinToChucThi> Data;
                if (p.SoQuyen == null)
                {
                    base.Status = 1;
                    base.Message = ConstantLogMessage.API_NoData;
                    return base.GetActionResult();
                }
                Data = _DuLieuDiemThiBUS.GetPagingBySearch_New(p, ref TotalRow, CanBoID);
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [Route("CheckTrungCCCD")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult CheckTrungCCCD([FromQuery] int? ThiSinhID, [FromQuery] string CCCD)
        {
            try
            {
                var Data = _DuLieuDiemThiBUS.CheckTrungCCCD(ThiSinhID, CCCD);
                if (Data != null && Data.ThiSinhID > 0)
                {
                    base.Status = 0;
                    base.Message = "Trùng CMND/CCCD với thí sinh trong hệ thống";
                    base.Data = Data;
                }
                else
                {
                    base.Status = 1;
                }
              
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

    }
}