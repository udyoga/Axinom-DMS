using DMS_Sender.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS_Sender.Logics.Interface
{
    public interface IExtractLogic
    {
        Task<List<ProcessStatus>> UnzipAndProcessFile(IFormFile postedFile, AuthModel authModel);
    }
}
