using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class FileDinhKemModel
    {
        public int FileID { get; set; }
        public int? NghiepVuID { get; set; }
        public int? DuyetBanKeKhaiID { get; set; }
        public string TenFileGoc { get; set; }
        public string TenFileHeThong { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public int? KeKhaiID { get; set; }
        public string Base64String { get; set; }
        public int FileType { get; set; }
        public int TrangThai { get; set; }
        public string FolderPath { get; set; }
        public string FileUrl { get; set; }
        public string TenCanBo { get; set; }
        public Byte[] TepByte { get; set; }
        public List<int> DanhSachKeKhaiID { get; set; }
        public FileDinhKemModel()
        {

        }
        public FileDinhKemModel(int FileID, int DuyetBanKeKhaiID, string TenFileHeThong, string TenFileGoc, DateTime NgayCapNhat,
            int NguoiCapNhat, int KeKhaiID,int TrangThai)
        {
            this.FileID = FileID;
            this.DuyetBanKeKhaiID = DuyetBanKeKhaiID;
            this.TenFileHeThong = TenFileHeThong;
            this.TenFileGoc = TenFileGoc;
            this.NgayCapNhat = NgayCapNhat;
            this.NguoiCapNhat = NguoiCapNhat;
            this.KeKhaiID = KeKhaiID;
            this.TrangThai = TrangThai;



        }
    }
    public class FileDinhKemModelPar : FileDinhKemModel
    {
        public string TenCanBo { get; set; }
        public FileDinhKemModelPar()
        {

        }
        public FileDinhKemModelPar(string TenCanBo)
        {
            this.TenCanBo = TenCanBo;
        }
    }
    public class PostFile
    {
        public int KeKhaiID { get; set; }
        public List<FileModel> ListBase64 { get; set; }
    }

    public class FileModel
    {
        public string TenFile { get; set; }
        public string Base64 { get; set; }
    }
}
