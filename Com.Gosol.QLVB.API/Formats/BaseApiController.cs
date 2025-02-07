using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.QLVB.API.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.QLVB.API.Formats
{
    public class BaseApiController : ControllerBase
    {
        private string _Message;
        protected string Message { get { return string.IsNullOrEmpty(_Message) ? Constant.GetUserMessage(Status) : _Message; } set { _Message = value; } }
        protected object Data { get; set; }
        protected int Status { get; set; }
        protected int TotalRow { get; set; }
        protected int CanBoID { get; }
        protected int NguoidungID { get; }
        protected int CoQuanID { get; }

        private ILogHelper _LogHelper;
        private readonly ILogger _BugLogger;
        public BaseApiController(ILogHelper logHelper)
        {
            _LogHelper = logHelper;
            //_BugLogger = this._BugLogger;
            ClaimsPrincipal User = _LogHelper.getCurrentUser();
            if (User.Claims.Any(t => t.Type == "CanBoID")) 
                CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "CanBoID").Value, 0);
            else CanBoID = 0; // trường hợp không đăng nhập - test
            if (User.Claims.Any(t => t.Type == "NguoidungID")) NguoidungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "NguoidungID").Value, 0);
            var _BugLogger = this._BugLogger;
        }

        public BaseApiController(ILogHelper logHelper, ILogger bugLogger)
        {
            _LogHelper = logHelper;
            //_BugLogger = this._BugLogger;
            ClaimsPrincipal User = _LogHelper.getCurrentUser();
            if (User.Claims.Any(t => t.Type == "CanBoID")) CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "CanBoID").Value, 0);
            else CanBoID = 0; // trường hợp không đăng nhập - test
            if (User.Claims.Any(t => t.Type == "NguoidungID")) NguoidungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "NguoidungID").Value, 0);
            _BugLogger = bugLogger;
        }
        protected IActionResult GetActionResult()
        {
            return Ok(new
            {
                Status = Status,
                Message = Message,
                Data = Data,
                TotalRow = TotalRow,
            });
        }
        protected IActionResult GetActionResult(int status, string message, object data = null, int totalRow = 0)
        {
            if (data == null)
                return Ok(new
                {
                    Status = status,
                    Message = message,
                });
            else
                return Ok(new
                {
                    Status = status,
                    Message = message,
                    Data = data,
                    TotalRow = totalRow,
                });
        }
        protected IActionResult CreateActionResult(string LogString, EnumLogType Action, Func<IActionResult> funct)
        {
            try
            {
                WriteLog(LogString, (int)Action);
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, (int)EnumLogType.Error);
                if (_BugLogger != null)
                    _BugLogger.LogInformation(ex.Message, LogString);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ConstantLogMessage.API_Error_System,
                });
            }
        }

        protected IActionResult CreateActionResultAsync(string LogString, EnumLogType Action, Func<IActionResult> funct)
        {
            try
            {
                WriteLog(LogString, (int)Action);
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, (int)EnumLogType.Error);
                if (_BugLogger != null)
                    _BugLogger.LogInformation(ex.Message, LogString);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ConstantLogMessage.API_Error_System,
                });
            }
        }

        protected IActionResult CreateActionResultNew(int Status, string LogString, EnumLogType Action, Func<IActionResult> funct)
        {
            try
            {
                if (Status == 1)
                {
                    WriteLog(LogString, (int)Action);
                }
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, (int)EnumLogType.Error);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ConstantLogMessage.API_Error_System,
                });
            }
        }
        //protected IActionResult ExportFileResult(string LogString, Func<IActionResult> funct)
        //{

        //    try
        //    {
        //        WriteLog(LogString, (int)LogType.Action);
        //        return funct.Invoke();
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex.Message, (int)LogType.Error);
        //        return Ok(new
        //        {
        //            Status = -1,
        //            Message = ex.Message,
        //        });
        //    }
        //}

        protected async Task WriteLog(string message, int logType)
        {
            _LogHelper.Log(CanBoID, message, logType);
        }
        protected IActionResult GetActionResultErrorAPI()
        {
            return Ok(new
            {
                Status = -1,
                Message = ConstantLogMessage.API_Error_System,
                Data = Data,
                TotalRow = 0,
            });
        }
    }
}
