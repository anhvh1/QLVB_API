
using Com.Gosol.QLVB.BUS.FileDinhKem;
using Com.Gosol.QLVB.BUS.KeKhai;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.API.Controllers
{
    public class Commons
    {
        public string passwordForCompress_DecompressFile = "";

        public Commons(string password)
        {
            this.passwordForCompress_DecompressFile = password + "asdfghjkl";
        }

        public Commons()
        {

        }
        /// <summary>
        /// save base64 to file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="TenFileHeThong"></param>
        /// <returns></returns>
        /// 
       
        public string ConvertFileToBase64(string pathFile)
        {
            try
            {
                var at = System.IO.File.GetAttributes(pathFile);

                byte[] fileBit = System.IO.File.ReadAllBytes(pathFile);
                var file = System.IO.Path.Combine(pathFile);

                string AsBase64String = Convert.ToBase64String(fileBit);
                return AsBase64String;
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw ex;
            }
        }

        //tạo file nén từ byte[] đã được mã hóa
        public void CompressAndSaveFile(byte[] byteArrOrigin, string fullPathFile)
        {
            if (byteArrOrigin == null)
                throw new ArgumentNullException("inputData must be non-null");
            using (MemoryStream originalFileStream = new MemoryStream(byteArrOrigin))
            using (FileStream compressedFileStream = File.Create(fullPathFile))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    originalFileStream.CopyTo(compressionStream);
                }
            }
        }

        public byte[] DecompressFile(string PathFileToDecompress)
        {
            byte[] byteArrDecompress;
            using (FileStream originalFileStream = new FileStream(PathFileToDecompress, FileMode.Open))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    using (MemoryStream decompressFile = new MemoryStream())
                    {
                        decompressionStream.CopyTo(decompressFile);
                        byteArrDecompress = decompressFile.ToArray();
                    }
                }
            }
            return byteArrDecompress;
        }

        public void CompressAndSaveFileWithPassword(byte[] byteArrOrigin, string fullPathFile, string TenFileGoc)
        {
            try
            {
                using (FileStream fs = File.Create(fullPathFile))
                using (var outStream = new ZipOutputStream(fs))
                {
                    outStream.Password = passwordForCompress_DecompressFile;
                    outStream.SetLevel(9);
                    outStream.PutNextEntry(new ZipEntry(TenFileGoc));
                    using (MemoryStream StreamFileGoc = new MemoryStream(byteArrOrigin))
                    {
                        StreamFileGoc.CopyTo(outStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExtractZipContentWithPassword(string FileZipPath)
        {
            try
            {
                string base64File = "";
                ICSharpCode.SharpZipLib.Zip.ZipFile file = null;
                FileStream fs = File.OpenRead(FileZipPath);
                file = new ICSharpCode.SharpZipLib.Zip.ZipFile(fs);
                file.Password = passwordForCompress_DecompressFile;
                byte[] fileDecompress;
                foreach (ZipEntry _zipEntry in file)
                {
                    Stream zipStream = file.GetInputStream(_zipEntry);
                    if (zipStream != null)
                    {
                        using (MemoryStream streamFile = new MemoryStream())
                        {
                            zipStream.CopyTo(streamFile);
                            fileDecompress = streamFile.ToArray();
                            base64File = Convert.ToBase64String(fileDecompress);
                        }
                        break;
                    }
                }
                return base64File;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public async Task<string> InsertFileAsync_Encrypt(IFormFile source, FileDinhKemModel FileDinhKem, IHostingEnvironment _host, IFileDinhKemBUS _FileDinhKemBUS)
        //{
        //    try
        //    {
        //        FileDinhKemModel fileInfo = new FileDinhKemModel();
        //        var crCanBoID = FileDinhKem.NguoiCapNhat;
        //        fileInfo.TenFileGoc = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
        //        fileInfo.TenFileGoc = EnsureCorrectFilename(fileInfo.TenFileGoc);
        //        fileInfo.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + fileInfo.TenFileGoc;
        //        fileInfo.NguoiCapNhat = crCanBoID;
        //        fileInfo.FileType = FileDinhKem.FileType;
        //        fileInfo.NghiepVuID = FileDinhKem.NghiepVuID;
        //        fileInfo.NgayCapNhat = DateTime.Now;
        //        fileInfo.FileUrl = GetUrlFile(fileInfo.TenFileHeThong + ".zip", FileDinhKem.FolderPath);
        //        var ResultFile = _FileDinhKemBUS.Insert(fileInfo);

        //        if (ResultFile.Status > 0)
        //        {
        //            //Add file vào thư mục server
        //            try
        //            {
        //                //Edit by ChungNN 22/1/2021
        //                BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
        //                byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
        //                //byte[] byteArrFileEncrypted = Encrypt_Decrypt.EncryptFile_AES(byteArrFile);
        //                CheckAndCreateFolder(_host, FileDinhKem.FolderPath);

        //                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                string fullPathFile = GetSavePathFile(_host, fileInfo.TenFileHeThong + ".zip", FileDinhKem.FolderPath);
        //                CompressAndSaveFileWithPassword(byteArrFile, fullPathFile, fileInfo.TenFileGoc);
        //                //CompressAndSaveFile(byteArrFileEncrypted, fullPathFile);
        //                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //                //using (FileStream output = File.Create(GetSavePathFile(_host, fileInfo.TenFileHeThong, FileDinhKem.FolderPath)))
        //                //    output.Write(byteArrFileEncrypted);
        //                ////await source.CopyToAsync(output);
        //                return fileInfo.FileUrl;
        //            }
        //            catch (Exception ex)
        //            {
        //                int FileDinhKemID = Utils.ConvertToInt32(ResultFile.Data, 0);
        //                if (FileDinhKemID > 0)
        //                {
        //                    List<int> listID = new List<int>();
        //                    listID.Add(FileDinhKemID);
        //                    _FileDinhKemBUS.Delete(listID);
        //                }

        //                return "";
        //                throw ex;
        //            }
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //        throw;
        //    }
        //}

        public async Task<string> InsertFileAsync(IFormFile source, FileDinhKemModel FileDinhKem, IHostingEnvironment _host, IFileDinhKemBUS _FileDinhKemBUS)
        {
            try
            {
                FileDinhKemModel fileInfo = new FileDinhKemModel();
                var crCanBoID = FileDinhKem.NguoiCapNhat;
                fileInfo.TenFileGoc = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                //fileInfo.TenFileGoc = EnsureCorrectFilename(fileInfo.TenFileGoc);
                fileInfo.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + fileInfo.TenFileGoc;
                fileInfo.NguoiCapNhat = crCanBoID;
                fileInfo.FileType = FileDinhKem.FileType;
                fileInfo.NghiepVuID = FileDinhKem.NghiepVuID;
                fileInfo.NgayCapNhat = DateTime.Now;
                fileInfo.FileUrl = GetUrlFile(fileInfo.TenFileHeThong, FileDinhKem.FolderPath);
                string test = source.ContentType;
                var ResultFile = _FileDinhKemBUS.Insert(fileInfo);
                if (ResultFile.Status > 0)
                {
                    //Add file vào thư mục server
                    try
                    {
                        BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
                        byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
                        CheckAndCreateFolder(_host, FileDinhKem.FolderPath);
                        using (FileStream output = File.Create(GetSavePathFile(_host, fileInfo.TenFileHeThong, FileDinhKem.FolderPath)))
                        {
                            output.Write(byteArrFile);
                            //await source.CopyToAsync(output);
                        }
                        return fileInfo.FileUrl;
                    }
                    catch (Exception ex)
                    {
                        int FileDinhKemID = Utils.ConvertToInt32(ResultFile.Data, 0);
                        if (FileDinhKemID > 0)
                        {
                            List<int> listID = new List<int>();
                            listID.Add(FileDinhKemID);
                            _FileDinhKemBUS.Delete(listID);
                        }

                        return "";
                        throw ex;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
                throw;
            }

        }

        public async Task<string> InsertFileHuongDanSuDung(IFormFile source, int crCanBoID, IHostingEnvironment _host)
        {
            try
            {
                string TenFileGoc = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                //fileInfo.TenFileGoc = EnsureCorrectFilename(fileInfo.TenFileGoc);
                string TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + TenFileGoc;

                BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
                byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
                CheckAndCreateFolder(_host, "HuongDanSuDung");

                string path = GetSavePathFile(_host, TenFileHeThong, "HuongDanSuDung");
                string urlFile = GetUrlFile(TenFileHeThong, "HuongDanSuDung");

                using (FileStream output = File.Create(path))
                {
                    output.Write(byteArrFile);
                }
                return urlFile;

            }
            catch (Exception ex)
            {
                return "";
                throw;
            }

        }

        public async Task<string> InsertAnhDaiDienAsync(IFormFile source, IHostingEnvironment _host, int CanBoID, HeThongCanBoModel _HeThongCanBoModel)
        {
            try
            {
                //Add file vào thư mục server
                try
                {
                    string TenFileGoc = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                    string TenFileAnh = CanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + _HeThongCanBoModel.TenCanBo + "_" + TenFileGoc;

                    //_HeThongCanBoModel.TenFileAnh = TenFileAnh;

                    BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
                    byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
                    CheckAndCreateFolder(_host, "AnhDaiDien");
                    string path = GetSavePathFile(_host, TenFileAnh, "AnhDaiDien");
                    string urlFile = GetUrlFile(TenFileAnh, "AnhDaiDien");
                    using (FileStream output = File.Create(path))
                    {
                        output.Write(byteArrFile);
                    }
                    return urlFile;
                }
                catch (Exception ex)
                {
                    return "";
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                return "";
                throw;
            }

        }

        /// <summary>
        /// Thêm nhiều bản kê khai ID với cùg 1 file
        /// Truyền vào list BanKekHaiID
        /// </summary>
        /// <param name="source"></param>
        /// <param name="FileDinhKem"></param>
        /// <param name="_host"></param>
        /// <param name="_FileDinhKemBUS"></param>
        /// <returns></returns>
        //public async Task<string> InsertFileAsync_NhieuBanKeKhaiID(IFormFile source, FileDinhKemModel FileDinhKem, IHostingEnvironment _host, IFileDinhKemBUS _FileDinhKemBUS)
        //{
        //    try
        //    {
        //        FileDinhKemModel fileInfo = new FileDinhKemModel();
        //        var crCanBoID = FileDinhKem.NguoiCapNhat;
        //        fileInfo.TenFileGoc = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
        //        //fileInfo.TenFileGoc = EnsureCorrectFilename(fileInfo.TenFileGoc);
        //        fileInfo.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + fileInfo.TenFileGoc;
        //        fileInfo.NguoiCapNhat = crCanBoID;
        //        fileInfo.FileType = FileDinhKem.FileType;
        //        fileInfo.DanhSachKeKhaiID = FileDinhKem.DanhSachKeKhaiID;
        //        fileInfo.NgayCapNhat = DateTime.Now;
        //        fileInfo.FileUrl = GetUrlFile(fileInfo.TenFileHeThong, FileDinhKem.FolderPath);
        //        string test = source.ContentType;
        //        var ResultFile = _FileDinhKemBUS.Insert_NhieuBanKeKhaiIDCungFile(fileInfo);
        //        if (ResultFile.Status > 0)
        //        {
        //            //Add file vào thư mục server
        //            try
        //            {
        //                BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
        //                byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
        //                CheckAndCreateFolder(_host, FileDinhKem.FolderPath);
        //                using (FileStream output = File.Create(GetSavePathFile(_host, fileInfo.TenFileHeThong, FileDinhKem.FolderPath)))
        //                {
        //                    output.Write(byteArrFile);
        //                    //await source.CopyToAsync(output);
        //                }
        //                return fileInfo.FileUrl;
        //            }
        //            catch (Exception ex)
        //            {
        //                //int FileDinhKemID = Utils.ConvertToInt32(ResultFile.Data, 0);
        //                //if (FileDinhKemID > 0)
        //                //{
        //                //    List<int> listID = new List<int>();
        //                //    listID.Add(FileDinhKemID);
        //                //    _FileDinhKemBUS.Delete(listID);
        //                //}

        //                return "";
        //                throw ex;
        //            }
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //        throw;
        //    }

        //}

        public void CheckAndCreateFolder(IHostingEnvironment _host, string folderPath = "")
        {
            var sysCF = new SystemConfigDAL();
            string path = _host.ContentRootPath + "\\" + sysCF.GetByKey("SavePathFile").ConfigValue + "\\" + folderPath;
            bool isFolder = Directory.Exists(path);
            if (!isFolder)
            {
                Directory.CreateDirectory(path);
            }
        }

        public string GetSavePathFile(IHostingEnvironment _host, string filename, string folderPath = "")
        {
            var sysCF = new SystemConfigDAL();
            return _host.ContentRootPath + "\\" + sysCF.GetByKey("SavePathFile").ConfigValue + "\\" + folderPath + "\\" + filename;
        }

        public string GetUrlFile(string filename, string folderPath = "")
        {
            var sysCF = new SystemConfigDAL();
            var pathfile = sysCF.GetByKey("SavePathFile").ConfigValue;

            return pathfile + "\\" + folderPath + "\\" + filename;
        }

        public string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        public string GetServerPath(HttpContext httpCT)
        {
            return httpCT.Request.Scheme + "://" + httpCT.Request.Host.Value + "\\";
        }

        //public async Task<int> InsertFile(IFormFile source, FileDinhKemModel FileDinhKem, IHostingEnvironment _host, IFileDinhKemBUS _FileDinhKemBUS)
        //{
        //    try
        //    {
        //        FileDinhKemModel fileInfo = new FileDinhKemModel();
        //        var crCanBoID = FileDinhKem.NguoiCapNhat;
        //        fileInfo.TenFileGoc = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
        //        fileInfo.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + fileInfo.TenFileGoc;
        //        fileInfo.NguoiCapNhat = crCanBoID;
        //        fileInfo.FileType = FileDinhKem.FileType;
        //        fileInfo.NghiepVuID = FileDinhKem.NghiepVuID;
        //        fileInfo.NgayCapNhat = DateTime.Now;
        //        fileInfo.FileUrl = GetUrlFile(fileInfo.TenFileHeThong, FileDinhKem.FolderPath);
        //        string test = source.ContentType;
        //        var ResultFile = _FileDinhKemBUS.Insert(fileInfo);
        //        if (ResultFile.Status > 0)
        //        {
        //            //Add file vào thư mục server
        //            try
        //            {
        //                BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
        //                byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
        //                CheckAndCreateFolder(_host, FileDinhKem.FolderPath);
        //                using (FileStream output = File.Create(GetSavePathFile(_host, fileInfo.TenFileHeThong, FileDinhKem.FolderPath)))
        //                {
        //                    output.Write(byteArrFile);
        //                }
        //                return Utils.ConvertToInt32(ResultFile.Data, 0);
        //            }
        //            catch (Exception ex)
        //            {
        //                int FileDinhKemID = Utils.ConvertToInt32(ResultFile.Data, 0);
        //                if (FileDinhKemID > 0)
        //                {
        //                    List<int> listID = new List<int>();
        //                    listID.Add(FileDinhKemID);
        //                    _FileDinhKemBUS.Delete(listID);
        //                }

        //                return 0;
        //                throw ex;
        //            }
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //        throw;
        //    }

        //}
    }
}
