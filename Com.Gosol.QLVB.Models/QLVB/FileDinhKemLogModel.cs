using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class FileDinhKemLogModel
    {
        public int FileLogID { get; set; }
        public int FileID { get; set; }
        public int FileIDMoi { get; set; }
        public int? NghiepVuID { get; set; }
        public int? DuyetBanKeKhaiID { get; set; }
        public string TenFileGocCu { get; set; }
        public string TenFileGocMoi { get; set; }
        public string TenFileHeThongCu{ get; set; }
        public string TenFileHeThongMoi{ get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public int? KeKhaiID { get; set; }
        public string Base64StringCu { get; set; }
        public string Base64StringMoi { get; set; }
        public string FileType { get; set; }
        public int TrangThai { get; set; }
        public int ThaoTac { get; set; }
        //public DateTime _NgayCapNhat { get; set; }
        public string FolderPath { get; set; }
        public string FileUrl { get; set; }
        public FileDinhKemLogModel()
        {

        }
        public FileDinhKemLogModel(int FileID,  string TenFileGoc, string TenFileHeThong, DateTime NgayCapNhat,
            int NguoiCapNhat, int KeKhaiID, int DuyetBanKeKhaiID, string FileType, string Base64String, int TrangThai)
        {
            this.FileID = FileID;
            this.DuyetBanKeKhaiID = DuyetBanKeKhaiID;
            this.TenFileHeThongCu = TenFileHeThong;
            this.TenFileGocCu = TenFileGoc;
            this.NgayCapNhat = NgayCapNhat;
            this.NguoiCapNhat = NguoiCapNhat;
            this.KeKhaiID = KeKhaiID;
            this.TrangThai = TrangThai;
            this.FileType = FileType;
            this.Base64StringCu = Base64String;
        }
    }
    public class FileLogModel
    {
        public int FileIDCu { get; set; }
        public int FileIDMoi { get; set; }
        public string TenFileCu { get; set; }
        public string TenFileMoi { get; set; }
        public DateTime NgayChinhSua { get; set; }
        public string NguoiCapNhat { get; set; }
        public int ThaoTac { get; set; }
    }
}
