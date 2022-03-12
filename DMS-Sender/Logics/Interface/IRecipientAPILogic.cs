using DMS_Sender.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace DMS_Sender.Logics.Interface
{
    public interface IRecipientAPILogic
    {
        Task<HttpResponseMessage> SendEncyptedFile(string jsonFile, AuthModel authModel);
    }
}
