using API.Infrastructure.Common;
using DAL.ModelView.Safaricom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace API.Infrastructure.Application.SafaricomManager
{
    internal partial class PhoneInsuranceManager
    {
        public async Task<string> UploadFileAsync(FileDetails request,string customername)
        {

            string FileType = ".jpg,.png,.jpeg,.pdf,.docx";
           
            
                if (request.extension == null || !FileType.Contains(request.extension.ToLower()))
                    throw new InvalidOperationException("File Format Not Supported.");
                if (request.name is null)
                    throw new InvalidOperationException("Name is required.");
            
           

            string base64Data = Regex.Match(request.data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            switch (request.extension.ToLower())
            {
                case ".pdf":
                    base64Data= Regex.Match(request.data, "data:application/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    break;
                case ".docx":
                    base64Data = Regex.Match(request.data, "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    break;
                default:
                    base64Data = Regex.Match(request.data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    break;
            }

            var streamData = new MemoryStream(Convert.FromBase64String(base64Data));
            if (streamData.Length > 0)
            {
                string folder = "Claims";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    folder = folder.Replace(@"\", "/");
                }

                string folderName = Path.Combine("Files", "Others", folder);
                //    supportedFileType switch
                //{
                //    FileType.Image => Path.Combine("Files", "Images", folder),
                //    _ => Path.Combine("Files", "Others", folder),
                //};
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Directory.CreateDirectory(pathToSave);

                string fileName = request.name.Trim('"');
                fileName = RemoveSpecialCharacters(fileName);
                fileName = fileName.ReplaceWhitespace("-") + "_" + customername + "_" + DateTime.Now.ToString("ddMMyyHHmmss");
                fileName += request.extension.Trim();
                string fullPath = Path.Combine(pathToSave, fileName);
                string dbPath = Path.Combine(folderName, fileName);
                if (File.Exists(dbPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }

                using var stream = new FileStream(fullPath, FileMode.Create);
                await streamData.CopyToAsync(stream);
                return fileName;// dbPath.Replace("\\", "/");
            }
            else
            {
                return string.Empty;
            }


        }
        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
        }

        public void Remove(string? path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private const string NumberPattern = "-{0}";

        private static string NextAvailableFilename(string path)
        {
            if (!File.Exists(path))
            {
                return path;
            }

            if (Path.HasExtension(path))
            {
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path), StringComparison.Ordinal), NumberPattern));
            }

            return GetNextFilename(path + NumberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);

            if (!File.Exists(tmp))
            {
                return tmp;
            }

            int min = 1, max = 2;

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                {
                    min = pivot;
                }
                else
                {
                    max = pivot;
                }
            }

            return string.Format(pattern, max);
        }

        public  string GetDescription(Enum enumValue)
        {
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())!
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attr.Length > 0)
                return ((DescriptionAttribute)attr[0]).Description;
            string result = enumValue.ToString();
            result = Regex.Replace(result, "([a-z])([A-Z])", "$1 $2");
            result = Regex.Replace(result, "([A-Za-z])([0-9])", "$1 $2");
            result = Regex.Replace(result, "([0-9])([A-Za-z])", "$1 $2");
            result = Regex.Replace(result, "(?<!^)(?<! )([A-Z][a-z])", " $1");
            return result;
        }

        public  List<string> GetDescriptionList(Enum enumValue)
        {
            string result = enumValue.GetDescription();
            return result.Split(',').ToList();
        }
    }
}
