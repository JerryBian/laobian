using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laobian.Share.Command
{
    public interface ICommandClient
    {
        Task<string> RunAsync(string command);
    }
}