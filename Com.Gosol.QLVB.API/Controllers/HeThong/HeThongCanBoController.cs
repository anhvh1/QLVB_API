using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Diagnostics;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.API.Authorization;
using Microsoft.Extensions.Logging;
using Com.Gosol.QLVB.BUS.KeKhai;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.BUS.FileDinhKem;

namespace Com.Gosol.QLVB.API.Controllers.HeThong
{
    [Route("api/v1/HeThongCanBo")]
    [ApiController]
    public class HeThongCanBoController : BaseApiController
    {
        private IHeThongCanBoBUS _HeThongCanBoBUS;
        //private IKeKhaiThanNhanBUS _KeKhaiThanNhanBUS;
        private IPhanQuyenBUS _PhanQuyenBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHostingEnvironment _host;
        public HeThongCanBoController(IHeThongCanBoBUS HeThongCanBoBUS, IPhanQuyenBUS PhanQuyenBUS, IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment HostingEnvironment, ILogHelper _LogHelper, ILogger<HeThongCanBoController> logger) : base(_LogHelper, logger)
        {

            this._HeThongCanBoBUS = HeThongCanBoBUS;
            this._PhanQuyenBUS = PhanQuyenBUS;
            this._host = HostingEnvironment;
            //this._KeKhaiThanNhanBUS = keKhaiThanNhanBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Create)]
        [Route("Insert")]
        public async Task<IActionResult> InsertAsync(IFormFile file, [FromForm] string HeThongCanBoStr)
        {
            try
            {

                //return CreateActionResult(ConstantLogMessage.HT_CanBo_ThemCanBo, EnumLogType.Insert, async () =>
                //{
                var HeThongCanBoModel = JsonConvert.DeserializeObject<HeThongCanBoModel>(HeThongCanBoStr);

                if (file != null)
                {
                    var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    var clsCommon = new Commons();
                    {
                        string url = await clsCommon.InsertAnhDaiDienAsync(file, _host, crCanBoID, HeThongCanBoModel);
                        if (url != null && url.Length > 0)
                        {
                            HeThongCanBoModel.AnhHoSo = url;
                        }
                    }
                }

                string Message = null;
                int val = 0;
                int CanBoID = 0;
                val = _HeThongCanBoBUS.Insert(HeThongCanBoModel, ref CanBoID, ref Message);
                if (val > 0)
                {
                    base.Status = 1;
                }
                else if (val == -3)
                {
                    base.Status = -3;
                }
                base.Message = Message;
                base.Data = val;
                return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }

        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Create)]
        [Route("Insert_Temp")]
        public async Task<IActionResult> Insert(HeThongCanBoModel HeThongCanBoModel)
        {
            try
            {

                string Message = null;
                int val = 0;
                int CanBoID = 0;
                val = _HeThongCanBoBUS.Insert(HeThongCanBoModel, ref CanBoID, ref Message);
                base.Message = Message;
                base.Data = val;
                return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }

        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Edit)]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsync(IFormFile file, [FromForm] string HeThongCanBoStr)
        {
            try
            {
                //return CreateActionResult(ConstantLogMessage.HT_CanBo_SuaCanBo, EnumLogType.Update, () =>
                //{
                var HeThongCanBoModel = JsonConvert.DeserializeObject<HeThongCanBoModel>(HeThongCanBoStr);

                if (file != null)
                {
                    var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    var clsCommon = new Commons();
                    {
                        string url = await clsCommon.InsertAnhDaiDienAsync(file, _host, crCanBoID, HeThongCanBoModel);
                        if (url != null && url.Length > 0)
                        {
                            HeThongCanBoModel.AnhHoSo = url;
                        }
                    }
                }

                string Message = null;
                int val = 0;
                val = _HeThongCanBoBUS.Update(HeThongCanBoModel, ref Message);
                base.Message = Message;
                base.Status = val > 0 ? 1 : 0;
                base.Data = val;
                return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }

        }
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_XoaCanBo, EnumLogType.Delete, () =>
                 {

                     var Result = _HeThongCanBoBUS.Delete(p.ListID);
                     if (Result.Count > 0)
                     {
                         base.Message = "Lỗi!";
                         base.Data = Result;
                         base.Status = 0;
                         return base.GetActionResult();
                     }
                     else
                     {
                         base.Message = "Xóa thành công!";
                         base.Data = Result;
                         base.Status = 1;
                         return base.GetActionResult();
                     }
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }

        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetByID")]
        public IActionResult GetCanBoByID([FromQuery] int CanBoID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetByID, EnumLogType.GetByID, () =>
                 {
                     var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                     HeThongCanBoModel Data;
                     Data = _HeThongCanBoBUS.GetCanBoByID(CanBoID);
                     if (Data != null && Data.CanBoID > 0)
                     {
                         if (!string.IsNullOrEmpty(Data.AnhHoSo))
                         {
                             var clsCommon = new Commons();
                             Data.AnhHoSo = clsCommon.GetServerPath(HttpContext) + Data.AnhHoSo;
                         }
                     }
                     base.Status = Data.CanBoID > 0 ? 1 : 0;
                     base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetPagingBySearch([FromQuery] BasePagingParams p, int? CoQuanID, int? TrangThaiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Insert, () =>
                 {
                     IList<HeThongCanBoModel> Data;
                     int TotalRow = 0;
                     Data = _HeThongCanBoBUS.GetPagingBySearch(p, ref TotalRow, CoQuanID, TrangThaiID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CoQuanID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0));
                     int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                     if (Data.Count == 0)
                     {
                         base.Message = ConstantLogMessage.API_NoData;
                         base.Status = 1;
                         base.TotalRow = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     //if (NguoiDungID == 1)
                     //{
                     //    TotalRow = TotalRow - 1;
                     //}
                     //Check quyền quản lý
                     //var ListChucNang = _PhanQuyenBUS.GetListChucNangByNguoiDungID(NguoiDungID);
                     //var quyenQuanLy = ListChucNang.Where(x => x.MaChucNang == "quan-ly").ToList();
                     //if (quyenQuanLy.Count > 0)
                     //{
                     //    if (quyenQuanLy[0].Xem == 0)
                     //    {
                     //        TotalRow = 1;
                     //        Data = Data.Where(x => x.NguoiDungID == NguoiDungID).ToList();
                     //    }
                     //}
                     //else
                     //{
                     //    TotalRow = 1;
                     //    Data = Data.Where(x => x.NguoiDungID == NguoiDungID).ToList();
                     //}
                     base.Status = TotalRow > 0 ? 1 : 0;
                     base.Data = Data;
                     base.TotalRow = TotalRow;
                     return base.GetActionResult();

                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        //[HttpGet]
        //[Route("FilterByName")]
        //public IActionResult FilterByName(string TenCanBo, int IsStatus, int CoQuanID)
        //{
        //    return CreateActionResult(ConstantLogMessage.HT_CanBo_FilterByName, () =>
        //    {
        //        IList<HeThongCanBoModel> Data;
        //        Data = _HeThongCanBoBUS.FilterByName(TenCanBo, IsStatus, CoQuanID);
        //        int totalRow = Data.Count();
        //        base.Status = totalRow > 0 ? 1 : 0;
        //        base.Data = Data;
        //        base.TotalRow = totalRow;
        //        return base.GetActionResult();
        //    });
        //}
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("ReadExcelFileOld")]
        public async Task<IActionResult> ReadExcelFileOld([FromBody] Files file)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_ImportFile, EnumLogType.Other, () =>
                {
                    string SavePath = _host.ContentRootPath + "\\Upload\\" + "CanBo.xlsx";
                    if (System.IO.File.Exists(SavePath))
                    {
                        System.IO.File.Delete(SavePath);
                    }
                    using (FileStream stream = System.IO.File.Create(SavePath))
                    {
                        byte[] byteArray = Convert.FromBase64String(file.files);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }

                    var Result = _HeThongCanBoBUS.ReadExcelFile_Old(SavePath, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
                    //var ResultNew = Result.Where(x => x.NguyenNhan.Where(y=>y.Contains("Files không có dữ liệu!")).Select(x=>x)).ToList();
                    //if (ResultNew.Count > 0)
                    //{
                    //    base.Message = "Files không có dữ liệu!";
                    //    base.Status = 0;
                    //    return base.GetActionResult();
                    //}
                    if (Result.Where(x => x.NguyenNhan.Count > 0).ToList().Count <= 0)
                    {
                        base.Message = "Import thành công!";
                        base.Status = 1;
                        return base.GetActionResult();
                    }

                    else
                    {
                        foreach (var item in Result)
                        {
                            var FileNo = item.NguyenNhan.Where(x => x.ToString().Trim() == "Files không có dữ liệu!").ToList();
                            if (FileNo.Count > 0)
                            {
                                base.Message = "Files không có dữ liệu!";
                                base.Status = 0;
                                return base.GetActionResult();
                            }
                        }
                        base.Message = "Import không thành công!" + "<br>" + " Danh sách lỗi :" + "<br>";
                        base.Status = 0;
                        base.Data = Result;
                        return base.GetActionResult();
                    }
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("ReadExcelFile")]
        public async Task<IActionResult> ReadExcelFile(IList<IFormFile> files)
        {
            try
            {
                //return CreateActionResult(ConstantLogMessage.HT_CanBo_ImportFile, EnumLogType.Other, async () =>
                //{
                string SavePath = _host.ContentRootPath + "\\Upload\\" + "CanBo.xlsx";
                if (System.IO.File.Exists(SavePath))
                {
                    System.IO.File.Delete(SavePath);
                }
                foreach (IFormFile source in files)
                {
                    using (FileStream output = System.IO.File.Create(SavePath))
                        await source.CopyToAsync(output);
                }
                var coQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                var CanBoDangNhapID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                var Result = _HeThongCanBoBUS.ReadExcelFile(SavePath, coQuanID, CanBoDangNhapID);
                if (Result == null || Result is null)
                {
                    base.Message = "Files không có dữ liệu!";
                    base.Status = 0;
                }
                else
                {
                    if (Result.Count > 0)
                    {
                        base.Message = "Danh sách nhân viên lỗi!";
                        base.Status = 0;
                        base.Data = Result;
                    }
                    else
                    {
                        base.Message = "Import thành công!";
                        base.Status = 1;
                    }
                }

                return base.GetActionResult();
                //});

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("DowloadExelOld")]
        public async Task<IActionResult> DowloadExelOld()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_ExportFile, EnumLogType.Other, () =>
                 {

                     var host = _host.ContentRootPath;
                     var expath = host + "\\Upload\\CanBo_Template.xlsm";
                     _HeThongCanBoBUS.ImportToExel(expath, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
                     var memory = new MemoryStream();
                     Byte[] bytes = System.IO.File.ReadAllBytes(expath);
                     String file = Convert.ToBase64String(bytes);
                     file = string.Concat("data:application/vnd.ms-excel;base64,", file);
                     //httpResponseMessage.StatusCode = HttpStatusCode.OK;
                     base.Data = file;
                     base.Status = 1;
                     return base.GetActionResult();

                 });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [Route("DowloadExel")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public async Task<IActionResult> DowloadExel()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_ExportFile, EnumLogType.Other, () =>
                {
                    //var host = _host.ContentRootPath;
                    var clsCommon = new Commons();
                    var host = clsCommon.GetServerPath(HttpContext);
                    var expath = "Upload\\CanBo_Template.xlsx";
                    //var expath = host + "\\Upload\\CanBo_Template.xlsm";
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    _HeThongCanBoBUS.ImportToExel(expath, CoQuanID);

                    base.Data = host + expath;
                    //base.Data = expath;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetThanNhanByCanBoID")]
        public IActionResult GetThanNhanByCanBoID() // lấy thân nhân của cán bộ đang đăng nhập
        {
            try
            {
                return CreateActionResult("Lấy thân nhân theo cán bộ", EnumLogType.Insert, () =>
                {
                    List<HeThongCanBoShortModel> list = new List<HeThongCanBoShortModel>();
                    list = _HeThongCanBoBUS.GetThanNhanByCanBoID(Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0));
                    base.Status = 1;
                    Data = list;
                    base.Message = "Danh sách thân nhân";
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }

        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAllCanBoByCoQuanID")]
        public IActionResult GetAllCanBoByCoQuanID(int CoQuanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.GetAllCanBoByCoQuanID(CoQuanID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0));
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAllCanBoTrongCoQuan")]
        public IActionResult GetAllCanBoTrongCoQuan()
        {
            try
            {
                return CreateActionResult("Lấy tất cả cán bộ trong cơ quan", EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.GetAllByCoQuanID(Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0));
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GenerationMaCanBo")]
        public IActionResult GenerationMaCanBo([FromQuery] int CoQuanID)
        {
            try
            {
                return CreateActionResult("Tạo mã bởi cơ quan", EnumLogType.GetByID, () =>
                {
                    var Data = _HeThongCanBoBUS.GenerationMaCanBo(CoQuanID);
                    if (string.IsNullOrEmpty(Data))
                    {
                        base.Status = 0;
                        base.Data = Data;
                        return base.GetActionResult();

                    }
                    base.Status = 1;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetAllInCoQuanCha")]
        public IActionResult GetAllInCoQuanCha([FromQuery] int? CoQuanID)
        {
            try
            {
                return CreateActionResult("Lấy tất cả cán bộ trong cơ quan", EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.GetAllInCoQuanCha(CoQuanID.Value);
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.FullAccess)]
        [Route("HeThongCanBo_GetThongTinCoQuan")]
        public IActionResult HeThongCanBo_GetThongTinCoQuan()
        {
            try
            {
                return CreateActionResult("Lấy thông tin cơ quan của cán bộ", EnumLogType.Other, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("NguoiDungID")).Value, 0);
                    var Result = _HeThongCanBoBUS.HeThongCanBo_GetThongTinCoQuan(CanBoID, NguoiDungID);
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Lấy thông tin cán bộ và thân nhân cán bộ theo CanBoID
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ThongTinCanBo_GetThongTinCanBo")]
        public IActionResult HeThongCanBo_GetThongTinCoQuan(int CanBoID)
        {
            try
            {
                return CreateActionResult("Lấy thông tin của cán bộ", EnumLogType.GetByID, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("NguoiDungID")).Value, 0);
                    ThongTinCanBoModel Result = new ThongTinCanBoModel();
                    Result.ThongTinCanBo = _HeThongCanBoBUS.GetCanBoByID(CanBoID);
                    if (Result.ThongTinCanBo != null && Result.ThongTinCanBo.CanBoID > 0)
                    {
                        var clsCommon = new Commons();
                        var listFile = _FileDinhKemBUS.GetAllField_FileDinhKem_ByNghiepVuID_AndType(CanBoID, EnumLoaiFileDinhKem.AnhHoSo.GetHashCode());
                        if (listFile.Count > 0)
                            Result.ThongTinCanBo.AnhHoSo = clsCommon.GetServerPath(HttpContext) + listFile[0].FileUrl;
                    }
                    //Result.ThongTinThanNhan = _KeKhaiThanNhanBUS.GetThanNhanCanBo_Byu_CanBoID(CanBoID);
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [Route("ThongTinCanBo_UpdateThongTinCanBo")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public async Task<IActionResult> ThongTinCanBo_UpdateThongTinCanBo(IFormFile AnhHoSo, [FromForm] string NghiepVuID, [FromForm] string ThongTinCanBo, [FromForm] string ListThongTinThanNhan, [FromForm] string ListDeleteConCai)
        {
            var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("NguoiDungID")).Value, 0);
            int resCanBo = 1;
            var resThanNhan = new BaseResultModel();
            var resDeleteConCai = new BaseResultModel();
            var resAnhHoSo = "";
            string Message = string.Empty;
            if (ThongTinCanBo != null)
            {
                var HeThongCanBoModel = JsonConvert.DeserializeObject<HeThongCanBoModel>(ThongTinCanBo);
                resCanBo = _HeThongCanBoBUS.Update(HeThongCanBoModel, ref Message);
            }
            if (ListThongTinThanNhan != null)
            {
                //var ListThanNhanCanBoModel = JsonConvert.DeserializeObject<List<KeKhaiThanNhanModel>>(ListThongTinThanNhan);
                //var listInsert = ListThanNhanCanBoModel.Where(x => x.ThanNhanID == null || x.ThanNhanID < 1).ToList();
                //var listUpdate = ListThanNhanCanBoModel.Where(x => x.ThanNhanID != null && x.ThanNhanID > 0).ToList();
                //if (listInsert.Count > 0) { resThanNhan = _KeKhaiThanNhanBUS.InsertAll(listInsert); }
                //if (listUpdate.Count > 0) { resThanNhan = _KeKhaiThanNhanBUS.UpdateAll(listUpdate); }

                if (resThanNhan.Status < 1)
                {
                    Message = resThanNhan.Message;
                }
            }
            else
            {
                resThanNhan.Status = 1;
            }
            if (ListDeleteConCai != null)
            {
                List<int> listDeleteConCai = JsonConvert.DeserializeObject<List<int>>(ListDeleteConCai);
                //resDeleteConCai = _KeKhaiThanNhanBUS.Delete(listDeleteConCai);
            }
            else
            {
                resDeleteConCai.Status = 1;
            }
            if (AnhHoSo != null)
            {
                var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);

                var listFile = _FileDinhKemBUS.GetAllField_FileDinhKem_ByNghiepVuID_AndType(Utils.ConvertToInt32(NghiepVuID, 0), EnumLoaiFileDinhKem.AnhHoSo.GetHashCode());
                if (listFile.Count > 0)
                {
                    _FileDinhKemBUS.Delete(listFile);
                }

                FileDinhKemModel FileDinhKem = new FileDinhKemModel();
                FileDinhKem.NghiepVuID = Utils.ConvertToInt32(NghiepVuID, 0);
                FileDinhKem.NguoiCapNhat = crCanBoID;
                FileDinhKem.FileType = EnumLoaiFileDinhKem.AnhHoSo.GetHashCode();
                FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.AnhHoSo);
                var clsCommon = new Commons();
                resAnhHoSo = await clsCommon.InsertFileAsync(AnhHoSo, FileDinhKem, _host, _FileDinhKemBUS);
            }
            else
            {
                resAnhHoSo = "some random string";//random string for check api
            }
            base.Message = Message;
            base.Status = (resCanBo > 0 && resThanNhan.Status > 0 && resDeleteConCai.Status > 0 && resAnhHoSo != "") ? 1 : 0;
            base.Data = Data;
            return base.GetActionResult();
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAllCanBoByCoQuanDangNhap")]
        public IActionResult GetAllCanBoByCoQuanDangNhap()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Other, () =>
                {
                    var CoQuanDangNhap = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                    List<int> ListTemp = new List<int>();
                    ListTemp.Add(CoQuanDangNhap);
                    var Result = _HeThongCanBoBUS.GetAllByListCoQuanID(ListTemp);
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetAll")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetAll()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Insert, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    IList<HeThongCanBoModel> Data;
                    Data = _HeThongCanBoBUS.GetAll();
                    base.Status = 1;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllCanBoByNguoiDung")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetAllCanBoByNguoiDung([FromQuery] BasePagingParams p, int? CoQuanID, int? TrangThaiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Insert, () =>
                {
                    IList<HeThongCanBoModel> Data;
                    Data = _HeThongCanBoBUS.GetAllCanBoByNguoiDung(p, CoQuanID, TrangThaiID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CoQuanID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0));
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    int TotalRow = Data.Count;
                    if (Data.Count == 0)
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                        base.Status = 1;
                        base.TotalRow = 0;
                        base.Data = Data;
                        return base.GetActionResult();
                    }

                    base.Status = TotalRow > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = TotalRow;
                    return base.GetActionResult();

                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
    }
}
