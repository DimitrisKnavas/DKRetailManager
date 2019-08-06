using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}