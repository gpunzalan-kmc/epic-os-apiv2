using System.Collections.Generic;
using System.Linq;
using EPICOS_API.Helpers;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using QueryDesignerCore;

namespace EPICOS_API.Repositories
{
    public class RaspberryRepository
    {
        public List<Log> LogGetAll(LogFilter filters)
        {
            // IEnumerable enumerable = filters as IEnumerable;
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<LogFilter>.Query(filters);
                IQueryable<Log> query = context.Log;
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = query.OrderByDescending(d => d.DateCreated).Skip(((filters.Page-1) * filters.Limit)).Take(filters.Limit).ToList();
                return list;
            }
               
        }

        public int LogCount(LogFilter filters)
        {
            // IEnumerable enumerable = filters as IEnumerable;
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<LogFilter>.Query(filters);
                IQueryable<Log> query = context.Log;
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = query.Count();
                return list;
            }
               
        }
    }
}