using DMS_recipient.Logics.Interface;
using DMS_recipient.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DMS_recipient.Logics.Implementation
{
    public class EncryptionLogic : IEncryptionLogic
    {
        private readonly IOptionsMonitor<Configs> _config;

        public EncryptionLogic(IOptionsMonitor<Configs> optionsMonitor)
        {
            _config = optionsMonitor;
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                string plainText = "";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new
                        Rfc2898DeriveBytes(_config.CurrentValue.EncryptionKey, Encoding.ASCII.GetBytes("AXINOM-TEST"));
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        plainText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return plainText;
            }
            catch (Exception)
            {

                throw;
            }            
        }

        public string DecryptJsonObject(string jsonString)
        {
            try
            {
                FolderStructure folderStructure = JsonConvert.DeserializeObject<FolderStructure>(jsonString);
                folderStructure.name = Decrypt(folderStructure.name);
                folderStructure.subFolder = DecryptSubfoldersNames(folderStructure.subFolder);

                return JsonConvert.SerializeObject(folderStructure, Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
            }
            catch (Exception)
            {
                return null;
            }
           
        }

        private List<FolderStructure> DecryptSubfoldersNames(List<FolderStructure> subFolder)
        {
            if(subFolder==null)
                return null;

            foreach (var item in subFolder)
            {
                item.name = Decrypt(item.name);
                item.subFolder = DecryptSubfoldersNames(item.subFolder);
            }
            return subFolder;
        }
    }
}
