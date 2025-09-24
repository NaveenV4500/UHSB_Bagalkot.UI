using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_MasterService.Common
{
    public static class CommonHelper
    {

        public static string UploadFilesInwardFTP(IFormFile image1, string filedate, string path)
        {
            string filelength = "filelength";
            string wrongfileformat = "wrongfileformat";

            if (image1 != null && image1.Length > 0)
            {
                var Maxvalue = 2097152;

                string fileName = image1.FileName;
                string fileContentType = image1.ContentType.ToLower();

                byte[] tempFileBytes;
                using (var ms = new MemoryStream())
                {
                    image1.CopyTo(ms);
                    tempFileBytes = ms.ToArray();
                }

                var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif", ".txt", ".csv", ".zip" };
                var ext = Path.GetExtension(image1.FileName);

                if (allowedExtensions.Contains(ext))
                {
                    var types = FileUploadCheck.GetFileType(ext);
                    var result = FileUploadCheck.isValidFile(tempFileBytes, types, fileContentType);

                    if (true) // same as old code
                    {
                        if (image1.Length <= Maxvalue)
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string myfile = Guid.NewGuid() + "_" + 1 + ext;
                            myfile = myfile.Replace("&", "");

                            var finalPath = Path.Combine(path, myfile);
                            try
                            {
                                using (var fs = new FileStream(finalPath, FileMode.Create))
                                {
                                    image1.CopyTo(fs);
                                }
                                return myfile;
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        else
                        {
                            return filelength;
                        }
                    }
                    else
                    {
                        return wrongfileformat;
                    }
                }
                else
                {
                    return wrongfileformat;
                }
            }
            return null;
        }

        public static List<string> GetFilePathFTP(string fileName, string originalfilename, string serverPath, string subName, IHttpContextAccessor httpContextAccessor)
        {
            var fileName1 = Path.GetFileName(originalfilename);
            FileInfo file = new FileInfo(originalfilename);
            string Fromfile = Path.Combine(serverPath, fileName);

            var BranchId = 0; 
            string exactfilename = "";
            string yearvalue = DateTime.Now.ToString("yyyyMMdd");
            int Year = Convert.ToInt32(yearvalue.Substring(0, 4));
            int Month = Convert.ToInt32(yearvalue.Substring(4, 2));

            exactfilename = subName + GetUniqueString() + file.Extension.ToLower();
            exactfilename = exactfilename.Replace("&", "").Replace("/", "_");

            string Tofile = Path.Combine(serverPath, exactfilename);

            if (File.Exists(Tofile))
            {
                File.Delete(Tofile);
            }

            if (!File.Exists(Tofile) && File.Exists(Fromfile))
            {
                File.Move(Fromfile, Tofile);
            }

            string MonthName = EnumHelper<CommonEnum.MonthsName>.GetValueFromName(Enum.GetName(typeof(CommonEnum.MonthsName), Month)).Trim();

            string pathdet = Path.Combine(Year.ToString(), MonthName, exactfilename);

            List<string> a = new List<string>();
            a.Insert(0, pathdet);
            a.Insert(1, exactfilename);
            return a;
        }

        public static void SaveFileFTP(string tempFileName, string newfilePath, string serverPath, string tempPath, string Type, IHttpContextAccessor httpContextAccessor)
        {
            string getyearvalue = DateTime.Now.ToString("yyyyMMdd");
            int getYear = Convert.ToInt32(getyearvalue.Substring(0, 4));
            int getMonth = Convert.ToInt32(getyearvalue.Substring(4, 2));

            string getMonthName = EnumHelper<CommonEnum.MonthsName>.GetValueFromName(Enum.GetName(typeof(CommonEnum.MonthsName), getMonth)).Trim();

            var BranchId = 0;// Convert.ToInt32(httpContextAccessor.HttpContext.Session.GetInt32("BranchId"));

            string pathdetail = Path.Combine(serverPath, getYear.ToString(), getMonthName);

            if (!Directory.Exists(pathdetail))
            {
                Directory.CreateDirectory(pathdetail);
            }

            string ToPathfile = Path.Combine(pathdetail, tempFileName);

            if (File.Exists(ToPathfile))
            {
                File.Delete(ToPathfile);
            }

            string fromPath = Path.Combine(tempPath, tempFileName);
            string toPath = Path.Combine(pathdetail, tempFileName);

            if (File.Exists(fromPath))
            {
                File.Move(fromPath, toPath);
            }
        }

        public static string GetUniqueString()
        {
            var uniqueCode = Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssffffff")) + 1;
            return uniqueCode.ToString();
        }

        public static void DownloadFileInwardFTP(string fileName, ref string filePath, ref string ExtName, string serverPath)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string ext = Path.GetExtension(fileName).ToLowerInvariant();

                var mimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            // Documents
            { ".pdf",  "application/pdf" },
            { ".doc",  "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls",  "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".ppt",  "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".txt",  "text/plain" },
            { ".csv",  "text/csv" },
            { ".rtf",  "application/rtf" },

            // Images
            { ".jpg",  "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png",  "image/png" },
            { ".gif",  "image/gif" },
            { ".bmp",  "image/bmp" },
            { ".tiff", "image/tiff" },
            { ".svg",  "image/svg+xml" },
            { ".webp", "image/webp" },

            // Archives
            { ".zip",  "application/zip" },
            { ".rar",  "application/x-rar-compressed" },
            { ".tar",  "application/x-tar" },
            { ".gz",   "application/gzip" },
            { ".7z",   "application/x-7z-compressed" }
        };

                if (!mimeTypes.TryGetValue(ext, out string mime))
                {
                    mime = "application/octet-stream";
                }

                ExtName = mime;

                // File path resolution
                string toFile = Path.Combine(serverPath, fileName);

                if (System.IO.File.Exists(toFile))
                {
                    filePath = toFile;
                }
            }
        }

    }
}
