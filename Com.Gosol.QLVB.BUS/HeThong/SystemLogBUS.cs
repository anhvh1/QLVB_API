using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.DAL.HeThong;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Com.Gosol.QLVB.BUS.HeThong
{
    public interface ISystemLogBUS
    {
        int Insert(SystemLogModel systemLogInfo);
        List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow);
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay);
        public string NhatKyHeThong_ExportExcel(string rootPath, string pathFile, List<SystemLogPartialModel> data);
    }
    public class SystemLogBUS : ISystemLogBUS
    {
        private ISystemLogDAL _SystemLogDAL;
        public SystemLogBUS(ISystemLogDAL systemLogDAL)
        {
            _SystemLogDAL = systemLogDAL;
        }
        public int Insert(SystemLogModel systemLogInfo)
        {
            return _SystemLogDAL.Insert(systemLogInfo);
        }
        public List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _SystemLogDAL.GetPagingBySearch(p, ref TotalRow);
        }

        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow)
        {
            return _SystemLogDAL.GetPagingByQuanTriDuLieu(p, ref TotalRow);
        }
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay)
        {
            return _SystemLogDAL.CreateLogFile(SystemLogPath,TuNgay,DenNgay);
        }
        public string NhatKyHeThong_ExportExcel(string rootPath, string pathFile, List<SystemLogPartialModel> data)
        {
            return _SystemLogDAL.NhatKyHeThong_ExportExcel(rootPath, pathFile, data);
        }
    }
}
