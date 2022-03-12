using DMS_recipient.Logics.Interface;
using DMS_recipient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DMS_recipient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivingController : ControllerBase
    {
        private readonly IEncryptionLogic _encryptionLogic;
        private readonly IDatabaseLogic _databaseLogic;

        public ReceivingController(IEncryptionLogic encryptionLogic,
            IDatabaseLogic databaseLogic)
        {
            _encryptionLogic = encryptionLogic;
            _databaseLogic = databaseLogic;
        }

        [HttpPost("SendJsonFile")]
        public async Task<IActionResult> SendJsonFile(PostObject postObject)
        {
            try
            {
                string jsonValue = _encryptionLogic.Decrypt(postObject.jsoncipherText);

                if (!string.IsNullOrEmpty(jsonValue)) {
                    return Ok(await _databaseLogic.SaveFileContent(jsonValue));
                }
                return BadRequest();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
    }
}
