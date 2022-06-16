using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Managers;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using EPICOS_API.Models.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace EPICOS_API.Repositories
{
    public class OfficeRepository
    {
        private readonly IHttpCallManager _httpCallManager;

        public OfficeRepository(IHttpCallManager httpCallManager)
        {
            _httpCallManager = httpCallManager;
        }
        public async Task<ExternalPaginationResponse<Site>> OfficeGetall(OfficeFilter filters)
        {   
            string url = "/buildings";
            ExternalPaginationResponse<Site> response = await _httpCallManager.QueryErpURI<OfficeFilter, Site>(url,filters);
            return response;
        }

        public async Task<Site> OfficeGetID(int Id)
        {
            string url = "/buildings/" + Id;
            Site response = await _httpCallManager.GetSingleErpURI<Site>(url);
            return response;
        }

        public async Task<int> WorkPointCount(int OfficeID)
        {
            using (var context = new EpicOSContext())
            {
                var list = await context.Workpoint.Where(i => i.OfficeID == OfficeID && i.IsDeleted == false).CountAsync();
                return list;
            }
        }

        public async Task<int> FloorCount(int OfficeID)
        {
            using (var context = new EpicOSContext())
            {
                var list = await context.Floor.Where(i => i.OfficeID == OfficeID && i.IsDeleted == false).CountAsync();
                return list;
            }
        }
        public async Task<int> HubCount(int OfficeID)
        {
            using (var context = new EpicOSContext())
            {
          var list = await context.Hub.Where(i => i.OfficeID == OfficeID && i.IsDeleted == false).CountAsync();
                return list;
            }
        }

    }


}