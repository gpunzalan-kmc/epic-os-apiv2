using System.Net.Http;
using System.Threading.Tasks;
using EPICOS_API.Models.Wrappers;

namespace EPICOS_API.Managers
{
    public interface IHttpCallManager
    {
        Task<ExternalPaginationResponse<T>> GetURI<T>(string url);
        Task<ExternalPaginationResponse<T>> GetErpURI<T>(string url);

        Task<T> GetSingleErpURI<T>(string url);
        Task<bool> PostAuthURI(string u, HttpContent c);

        Task<ExternalPaginationResponse<T>> QueryURI<S, T>(string url, S query);
        Task<ExternalPaginationResponse<T>> QueryErpURI<S, T>(string url, S query);
    }
}