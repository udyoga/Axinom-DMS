using DMS_Sender.Logics.Interface;
using DMS_Sender.Models;
using DMS_Sender.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace DMS_Sender.Logics.Implementation
{
    public class ExtractLogic : IExtractLogic
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEncryptionLogic _encryptionLogic;
        private readonly IRecipientAPILogic _recipientAPILogic;

        public ExtractLogic(IHostingEnvironment hostingEnvironment,
            IEncryptionLogic encryptionLogic,
            IRecipientAPILogic recipientAPILogic)
        {
            _hostingEnvironment = hostingEnvironment;
            _encryptionLogic = encryptionLogic;
            _recipientAPILogic = recipientAPILogic;
        }

        public async Task<List<ProcessStatus>> UnzipAndProcessFile(IFormFile postedFile, AuthModel authModel)
        {
            List<ProcessStatus> processStatusList = new List<ProcessStatus>();
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");
            Guid tempId = Guid.NewGuid();
            string fileName = Path.GetFileName(postedFile.FileName);
            string jsonStructure = null;
            string encryptedJsonStructure = null;
            string folderPath = Path.Combine(path, tempId.ToString(), Path.GetFileNameWithoutExtension(fileName));

            #region -- Upload File
            ProcessStatus processStatus = SaveUploadFile(path, fileName, postedFile);
            processStatusList.Add(processStatus);
            #endregion

            #region -- Unzip File
            processStatus = Unzip(path, fileName, tempId);
            processStatusList.Add(processStatus);
            #endregion

            #region -- Read folder structure
            if (processStatus.Status)
            {

                try
                {
                    processStatus = new ProcessStatus()
                    {
                        Action = "Read folder structure",
                        Status = true
                    };
                    jsonStructure = ReadTreeStructure(Path.GetFileNameWithoutExtension(fileName), folderPath);
                }
                catch (Exception ex)
                {
                    processStatus.Exception = ex;
                    processStatus.Status = false;
                }
                processStatusList.Add(processStatus);
            }
            #endregion

            #region -- Encrypt json object
            if (jsonStructure != null)
            {
                try
                {
                    processStatus = new ProcessStatus()
                    {
                        Action = "Encrypt json object",
                        Status = true
                    };
                    encryptedJsonStructure = _encryptionLogic.Encrypt(jsonStructure);
                }
                catch (Exception ex)
                {
                    processStatus.Exception = ex;
                    processStatus.Status = false;
                }
                processStatusList.Add(processStatus);
            }
            #endregion

            #region -- Send json file
            if (encryptedJsonStructure != null)
            {
                try
                {
                    processStatus = new ProcessStatus()
                    {
                        Action = "Send json file"
                    };

                    HttpResponseMessage httpResponseMessage = await _recipientAPILogic.SendEncyptedFile(encryptedJsonStructure, authModel);

                    if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        processStatus.Status = true;                       
                    }
                    else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        processStatus.Status = false;
                        processStatus.Action = "Authentication failed";
                        processStatus.Exception = httpResponseMessage.StatusCode;
                    }
                    else
                    {
                        processStatus.Status = false;
                        processStatus.Action = "Send json file";
                        processStatus.Exception = httpResponseMessage.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    processStatus.Exception = ex;
                    processStatus.Status = false;
                }
                processStatusList.Add(processStatus);
            }
            #endregion

            return processStatusList;
        }

        private ProcessStatus SaveUploadFile(string path, string fileName, IFormFile postedFile)
        {
            ProcessStatus processStatus = new ProcessStatus()
            {
                Action = "Upload File",
                Status = true
            };
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                processStatus.Exception = ex;
                processStatus.Status = false;
            }
            return processStatus;
        }

        private ProcessStatus Unzip(string zipPath,string fileName, Guid tempId)
        {
            ProcessStatus processStatus = new ProcessStatus()
            {
                Action = "Unzip File",
                Status = true
            };

            try
            {
                ZipFile.ExtractToDirectory(Path.Combine(zipPath, fileName), Path.Combine(zipPath, tempId.ToString(), Path.GetFileNameWithoutExtension(fileName)));
               
            }
            catch (Exception ex)
            {
                processStatus.Exception = ex;
                processStatus.Status = false;
            }            
            return processStatus;
        }

        private string ReadTreeStructure(string rootFolderName, string folderPath)
        {
            try
            {
                FolderStructure folderStructure = new FolderStructure()
                {
                    subFolder = new List<FolderStructure>()
                };

                folderStructure.name = rootFolderName;
                folderStructure.type = "folder";

                folderStructure.subFolder = DirectorySearch(new DirectoryInfo(folderPath), folderStructure.subFolder);

                string jsonTreeView = JsonConvert.SerializeObject(folderStructure, Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });

                return jsonTreeView;
            }
            catch (Exception)
            {
                throw;
            }           
        }

        private static List<FolderStructure> DirectorySearch(DirectoryInfo dir, List<FolderStructure> folderStructures)
        {
            try
            {
                foreach (FileInfo f in dir.GetFiles())
                {
                    folderStructures.Add(new FolderStructure()
                    {
                        name = f.Name,
                        type = enFileStructureType.file.ToString(),
                    });
                }

                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    folderStructures.Add(new FolderStructure()
                    {
                        name = d.Name,
                        type = enFileStructureType.folder.ToString(),
                        subFolder = DirectorySearch(d, new List<FolderStructure>())
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return folderStructures.Count > 0 ? folderStructures : null;
        }
    }
}
