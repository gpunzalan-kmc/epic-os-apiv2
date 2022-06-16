using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Helpers;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Parameters;
using EPICOS_API.Models.Wrappers;
using Microsoft.EntityFrameworkCore;
using QueryDesignerCore;

namespace EPICOS_API.Repositories
{
    public class FloorRepository
    {
        public PageResponse<List<FloorResponse>> FloorGetAll(FloorFilter filters)
        {
            // IEnumerable enumerable = filters as IEnumerable;
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<FloorFilter>.Query(filters);
                IQueryable<Floor> query = context.Floor;
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = query.Skip(((filters.Page-1) * filters.Limit)).Take(filters.Limit).ToList();
                var results = new List<FloorResponse>();
                foreach(Floor row in list){
                    FloorResponse floor = new FloorResponse();
                    floor.ID = row.ID;
                    floor.Name = row.Name;
                    floor.Filename = row.Filename;
                    floor.OfficeID = row.OfficeID;
                    floor.IsActive = row.IsActive;
                    floor.IsDeleted = row.IsDeleted;
                    floor.WorkpointCount = context.Workpoint.Where(i => i.FloorID == row.ID && i.IsDeleted == false).Count();
                    floor.HubCount = context.Hub.Where(i => i.FloorID == row.ID && i.IsDeleted == false).Count();
                    results.Add(floor);
                }
                var response = new PageResponse<List<FloorResponse>>(results, filters.Page, filters.Limit);
                response.TotalRecords = FloorGetAllCount(filters);
                return response;
            }
               
        }

        public int FloorGetAllCount(FloorFilter filters)
        {
            // IEnumerable enumerable = filters as IEnumerable;
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<FloorFilter>.Query(filters);
                IQueryable<Floor> query = context.Floor;
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var count = query.Count();
                return count;
            }
               
        }

        public Floor FloorGetID(int Id)
        {
            using (var context = new EpicOSContext())
            {
                IQueryable<Floor> query = context.Floor;
                var list = query.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                return list;
            }
        }


        public async Task<Response<Floor>> CreateFloor(Floor floor){
            
            using (var context = new EpicOSContext())
            {
                try {
                    context.Floor.Add(floor);
                    await context.SaveChangesAsync();
                    var response = new Response<Floor>{
                        Data = floor,
                        Succeeded = true,
                        Message = "Record created"
                    };
                    return response;
                }catch(DbUpdateException e){
                    var response = new Response<Floor>{
                        Data = floor,
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }

            }
        }

        public async Task<Response<Floor>> UpdateFloor(Floor floor, int Id){
            
            using (var context = new EpicOSContext())
            {
                floor.ID = Id;
                try {
                    context.Floor.Update(floor);
                    await context.SaveChangesAsync();
                    var response = new Response<Floor>{
                        Data = floor,
                        Succeeded = true,
                        Message = "Record updated"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Floor>{
                        Succeeded = false,
                        Message = e.Message
                    };
                    return response;
                }

            }
        }
        public async Task<Response<Floor>> FloorDelete(int Id)
        {
            using (var context = new EpicOSContext())
            {
            
                var checkRecord = context.Floor.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                if (checkRecord == null){
                    var response = new Response<Floor>{
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
                    var response = new Response<Floor>{
                        Succeeded = true,
                        Message = "Record deleted"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<Floor>{
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