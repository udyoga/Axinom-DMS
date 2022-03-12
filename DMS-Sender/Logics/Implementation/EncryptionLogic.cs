using DMS_Sender.Logics.Interface;
using DMS_Sender.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DMS_Sender.Logics.Implementation
{
    public class EncryptionLogic : IEncryptionLogic
    {
        private readonly IOptionsMonitor<Configs> _config;

        public EncryptionLogic(IOptionsMonitor<Configs> optionsMonitor)
        {
            _config = optionsMonitor;
        }

        public string Encrypt(string clearText)
        {
            try
            {   
                if(string.IsNullOrEmpty(clearText))
                    return null;

                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new
                        Rfc2898DeriveBytes(_config.CurrentValue.EncryptionKey, Encoding.ASCII.GetBytes("AXINOM-TEST"));
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            catch (Exception)
            {
                throw;
            }           
        }
    }
}
