using System.Threading.Tasks;

namespace DMS_recipient.Logics.Interface
{
    public interface IDatabaseLogic
    {
        Task<int> SaveFileContent(string fileData);
    }
}
