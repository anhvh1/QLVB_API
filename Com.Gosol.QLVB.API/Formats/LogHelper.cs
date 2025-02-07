using Com.Gosol.QLVB.BUS.HeThong;
using Com.Gosol.QLVB.Models.HeThong;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;


namespace Com.Gosol.QLVB.API.Formats
{
    public interface ILogHelper
    {
        void Log(int CanBoID, String logIngo, int logType);
        void Log(int CanBoID, String logInfo, int logType, DateTime logTime);
        ClaimsPrincipal getCurrentUser();
    }
    public class LogHelper : ILogHelper
    {
        private ISystemLogBUS _SystemLogBUS;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public LogHelper(ISystemLogBUS SystemLogBUS, IHttpContextAccessor HttpContextAcess)
        {
            _SystemLogBUS = SystemLogBUS;
            _HttpContextAccessor = HttpContextAcess;
        }
        public void Log(int CanBoID, String logInfo, int logType)
        {
            SystemLogModel systemLogInfo = new SystemLogModel();
            systemLogInfo.CanBoID = CanBoID;
            systemLogInfo.LogInfo = logInfo;
            systemLogInfo.LogTime = DateTime.Now;
            systemLogInfo.LogType = logType;

            try
            {
                _SystemLogBUS.Insert(systemLogInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Log(int CanBoID, String logInfo, int logType, DateTime logTime)
        {
            SystemLogModel systemLogInfo = new SystemLogModel();
            systemLogInfo.CanBoID = CanBoID;
            systemLogInfo.LogInfo = logInfo;
            systemLogInfo.LogTime = logTime;
            systemLogInfo.LogType = logType;

            try
            {
                _SystemLogBUS.Insert(systemLogInfo);
            }
            catch
            {
            }
        }

        public  ClaimsPrincipal getCurrentUser()
        {
            return _HttpContextAccessor.HttpContext.User;
        }
    }
}