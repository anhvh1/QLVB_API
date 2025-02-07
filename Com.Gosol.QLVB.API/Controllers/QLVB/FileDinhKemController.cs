using Com.Gosol.QLVB.API.Config;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.FileDinhKem;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Linq;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.Security;

namespace Com.Gosol.QLVB.API.Controllers.QLVB
{
    [Route("api/v1/FileDinhKem")]
    [ApiController]
    public class FileDinhKemController : BaseApiController
    {
        private IFileDinhKemBUS _FileDinhKemBUS;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _host;
        private IOptions<AppSettings> _AppSettings;
        public FileDinhKemController(IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment HostingEnvironment, IOptions<AppSettings> Settings, ILogHelper _LogHelper, ILogger<FileDinhKemController> logger) : base(_LogHelper, logger)
        {
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._host = HostingEnvironment;
            this._AppSettings = Settings;
        }

        [HttpPost]
        [Route("Insert")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public async Task<IActionResult> Insert(IList<IFormFile> files, [FromForm] string FileDinhKemStr)
        {
            try
            {
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                var FileDinhKem = JsonConvert.DeserializeObject<FileDinhKemModel>(FileDinhKemStr);
                var clsCommon = new Commons();
                if (files != null && files.Count > 0 && FileDinhKem != null)
                {
                    FileDinhKem.NguoiCapNhat = CanBoID;
                    if(FileDinhKem.FileType == EnumLoaiFileDinhKem.FileBangDiem.GetHashCode())
                    {
                        FileDinhKem.FolderPath = nameof(EnumLoaiFileDinhKem.FileBangDiem);
                    }
                    List<string> ListFileUrl = new List<string>();
                    foreach (IFormFile source in files)
                    {
                        var file = await clsCommon.InsertFileAsync(source, FileDinhKem, _host, _FileDinhKemBUS);
                        if (file != null && file.Length > 0)
                        {
                            ListFileUrl.Add(file);
                        }
                    }

                    base.Data = ListFileUrl;
                    base.Status = 1;
                    base.Message = "Thêm mới file đính kèm thành công";
                }
                else
                {
                    base.Status = 0;
                    base.Message = "Vui lòng chọn file đính kèm";
                }
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                //base.Message = ConstantLogMessage.API_Error_System;
                base.GetActionResult();
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetByID")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery] int FileDinhKemID)
        {
            try
            {
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                FileDinhKemModel Data = new FileDinhKemModel();
                Data = _FileDinhKemBUS.GetByID(FileDinhKemID);
                if (Data != null && Data.FileID > 0)
                {
                    base.Status = 1;
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext);
                    Data.FileUrl = serverPath + Data.FileUrl;
                }
                else
                {
                    base.Status = 0;
                }
                base.Data = Data;
                return base.GetActionResult();
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [Route("GetByNghiepVuID")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByNghiepVuID([FromQuery] int NghiepVuID, int FileType)
        {
            try
            {
                List<FileDinhKemModel> Data = new List<FileDinhKemModel>();
                Data = _FileDinhKemBUS.GetByNghiepVuID(NghiepVuID, FileType);
                if (Data != null && Data.Count > 0)
                {
                    base.Status = 1;
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext);
                    foreach (var item in Data)
                    {
                        item.FileUrl = serverPath + item.FileUrl;
                    }
                }
                else
                {
                    base.Status = 0;
                }
                base.Data = Data;
                return base.GetActionResult();
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw;
            }
        }

        [HttpPost]
        [Route("Delete")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult Delete(List<FileDinhKemModel> p)
        {
            try
            {
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                var Result = _FileDinhKemBUS.Delete(p);
                base.Status = Result.Status;
                base.Message = Result.Message;
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                //base.Message = ConstantLogMessage.API_Error_System;
                base.Message = ex.Message;
                return base.GetActionResult();
                throw ex;
            }
        }

        //[HttpGet]
        //[Route("Download")]
        //public HttpResponseMessage Download([FromQuery] int? uid)
        //{
        //    try
        //    {
        //        FileDinhKemModel Data = new FileDinhKemModel();
        //        Data = _FileDinhKemBUS.GetByID(uid ?? 0);

        //        //var provider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        //        //var video = provider.GetFileInfo(Data.FileUrl).CreateReadStream();

        //        //string contentType = "image/png";
        //        //StreamReader sr = new StreamReader(provider.Root + Data.FileUrl);
               
        //        //return File(video, contentType, enableRangeProcessing: true);

        //        var stream = new MemoryStream();
        //        // processing the stream.

        //        var result = new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new ByteArrayContent(stream.ToArray());
        //        };
        //        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = Data.FileUrl
        //            //FileName = "CertificationCard.pdf"
        //        };
        //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
