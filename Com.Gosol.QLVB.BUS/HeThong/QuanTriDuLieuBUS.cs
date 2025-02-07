using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.HeThong
{
    public interface IQuanTriDuLieuBUS
    {

        public int BackupData(string fileName, string filePath);
        public int RestoreDatabase(string fileName);
        public List<QuanTriDuLieuModel> GetFileInDerectory();
    }
    public class QuanTriDuLieuBUS : IQuanTriDuLieuBUS
    {
        private IQuanTriDuLieuDAL _QuanTriDuLieuDAL;
        public QuanTriDuLieuBUS(IQuanTriDuLieuDAL QuanTriDuLieuDAL)
        {
            _QuanTriDuLieuDAL = QuanTriDuLieuDAL;
        }
        public int BackupData(string fileName, string filePath)
        {
            return _QuanTriDuLieuDAL.BackupData(fileName, filePath);
        }

        public int RestoreDatabase(string fileName)
        {
            return _QuanTriDuLieuDAL.RestoreData(fileName);
        }

        public List<QuanTriDuLieuModel> GetFileInDerectory()
        {
            return _QuanTriDuLieuDAL.GetFileInDerectory();
        }

    }
}
