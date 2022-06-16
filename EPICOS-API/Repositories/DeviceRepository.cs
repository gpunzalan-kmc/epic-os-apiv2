using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Helpers;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using EPICOS_API.Models.Wrappers;
using Microsoft.EntityFrameworkCore;
using QueryDesignerCore;

namespace EPICOS_API.Repositories
{  public class DeviceRepository
    {
        // getter.GetValue(i, null).ToString().Equals(value.ToString())
        public List<Workpoint> WorkPointGetAll(WorkPointFilter filters)
        {
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<WorkPointFilter>.Query(filters);
                IQueryable<Workpoint> query = context.Workpoint.Include(i => i.Hub).Include(i => i.Floor);
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = query.Skip(((filters.Page-1) * filters.Limit)).Take(filters.Limit).ToList();
                return list;
            }
               
        }
        public int PaginationWorkPointCount(WorkPointFilter filters)
        {
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<WorkPointFilter>.Query(filters);
                IQueryable<Workpoint> query = context.Workpoint;
                if (filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = query.Count();
                return list;
            }

        }
        public async Task<List<Hub>> HubGetAll(HubFilter filters)
        {
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<HubFilter>.Query(filters);
                IQueryable<Hub> query = context.Hub.Include(i => i.Floor);
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = await query.Skip(((filters.Page-1) * filters.Limit)).Take(filters.Limit).ToListAsync();
                return list;
            }
        }


        public Hub HubGetID(int Id)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Hub> query = context.Hub;
                var list = query.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                return list;
            }  
        }
        public Workpoint WorkPointValidateMAC(Workpoint workpoint)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Workpoint> query = context.Workpoint;
                var list = query.Where(e => e.MAC.ToLower().Equals(workpoint.MAC.ToLower())).FirstOrDefault();
                return list;
            }  
        }
        public Workpoint WorkPointValidateIP(Workpoint workpoint)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Workpoint> query = context.Workpoint;
                var list = query.Where(e => e.IPaddress.Equals(workpoint.IPaddress)).FirstOrDefault();
                return list;
            }  
        }

        public Hub HubValidateMAC(Hub workpoint)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Hub> query = context.Hub;
                var list = query.Where(e => e.MAC.ToLower().Equals(workpoint.MAC.ToLower())).FirstOrDefault();
                return list;
            }  
        }
        public Hub HubValidateIP(Hub workpoint)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Hub> query = context.Hub;
                var list = query.Where(e => e.IPaddress.Equals(workpoint.IPaddress)).FirstOrDefault();
                return list;
            }  
        }
        public Workpoint WorkpointGetID(int Id)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Workpoint> query = context.Workpoint;
                var list = query.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                return list;
            }
               
        }

        public async Task<Response<Workpoint>> WorkPointCreate(Workpoint workpoints){
            
            using (var context = new EpicOSContext())
            {
                try {
                    context.Workpoint.Add(workpoints);
                    await context.SaveChangesAsync();
                    var response = new Response<Workpoint>{
                        Data = workpoints,
                        Succeeded = true,
                        Message = "Record created"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Workpoint>{
                        Data = workpoints,
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }

            }
        }

        public async Task<Response<Hub>> HubCreate(Hub hub){
            
            using (var context = new EpicOSContext())
            {
                try {
                    context.Hub.Add(hub);
                    await context.SaveChangesAsync();
                    var response = new Response<Hub>{
                        Data = hub,
                        Succeeded = true,
                        Message = "Record created"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Hub>{
                        Data = hub,
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }

            }
        }

        public async Task<Response<Workpoint>> WorkPointUpdate(Workpoint workpoints, int Id){
            
            using (var context = new EpicOSContext())
            {
                workpoints.ID = Id;
                try {
                    context.Workpoint.Update(workpoints);
                    await context.SaveChangesAsync();
                    var response = new Response<Workpoint>{
                        Data = workpoints,
                        Succeeded = true,
                        Message = "Record updated"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Workpoint>{
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }

            }
        }

        public async Task<Response<Hub>> HubUpdate(Hub hub, int Id){
            
            using (var context = new EpicOSContext())
            {
                hub.ID = Id;
                try {
                    context.Hub.Update(hub);
                    await context.SaveChangesAsync();
                    var response = new Response<Hub>{
                        Data = hub,
                        Succeeded = true,
                        Message = "Record updated"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Hub>{
                        Data = hub,
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }

            }
        }

        public async Task<Response<Workpoint>> WorkPointDelete(int Id)
        {
            using (var context = new EpicOSContext())
            {
            
                var checkRecord = context.Workpoint.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                if (checkRecord == null){
                    var response = new Response<Workpoint>{
                        StatusCode = 404,
                        Succeeded = false,
                        Message = "Record not found"
                    };
                    return response;
                }
                context.Entry(checkRecord).State = EntityState.Detached;
                
                try {
                    checkRecord.IsDeleted = true;
                    context.Entry(checkRecord).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    var response = new Response<Workpoint>{
                        Succeeded = true,
                        Message = "Record deleted"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Workpoint>{
                        StatusCode = 500,
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }
            }
        }  
        public async Task<Response<Hub>> HubDelete(int Id)
        {
            using (var context = new EpicOSContext())
            {
            
                var checkRecord = context.Hub.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                if (checkRecord == null){
                    var response = new Response<Hub>{
                        StatusCode = 404,
                        Succeeded = false,
                        Message = "Record not found"
                    };
                    return response;
                }
                context.Entry(checkRecord).State = EntityState.Detached;
                
                try {
                    checkRecord.IsDeleted = true;
                    context.Entry(checkRecord).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    var response = new Response<Hub>{
                        Succeeded = true,
                        Message = "Record deleted"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Hub>{
                        StatusCode = 500,
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }
            }
        }  
    }
}