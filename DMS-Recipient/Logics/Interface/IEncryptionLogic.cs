namespace DMS_recipient.Logics.Interface
{
    public interface IEncryptionLogic
    {
        string Decrypt(string cipherText);
        string DecryptJsonObject(string jsonString);
    }
}
