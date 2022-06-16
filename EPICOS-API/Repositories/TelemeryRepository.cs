using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Helpers;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using Microsoft.EntityFrameworkCore;
using QueryDesignerCore;

namespace EPICOS_API.Repositories
{
    public class TelemeryRepository
    {
        public List<Telemery> TelemeryGetAll(TelemeryFilter filter)
        {
            using (var context = new EpicOSContext())
            {
                var telemeries = context.Telemery
                .Join(context.Workpoint, telemery => telemery.WorkpointID, workpoint => workpoint.ID, (telemery, workpoint) => new { telemery, workpoint })
                .Where(i => i.workpoint.OfficeID == filter.OfficeID && i.workpoint.FloorID == filter.FloorID && i.telemery.DateCreated >= filter.DateStart && i.telemery.DateCreated <= filter.DateEnd)
                .ToList();
                List<Telemery> results = new List<Telemery>();
                foreach(var telemery in telemeries){
                    Telemery result = new Telemery();
                    result.ID = telemery.telemery.ID;
                    result.MAC = telemery.telemery.MAC;
                    result.IPAddress = telemery.telemery.IPAddress;
                    result.DateCreated = telemery.telemery.DateCreated;
                    result.HubID = telemery.telemery.HubID;
                    result.WorkpointID = telemery.telemery.WorkpointID;
                    result.IsActive = telemery.telemery.IsActive;
                    result.IsDeleted = telemery.telemery.IsDeleted;
                    results.Add(result);
                }
                return results;
            }
        }

        public Workpoint WorkPointByMAC(string MAC)
        {
            using (var context = new EpicOSContext())
            {
                return context.Workpoint.Where(i => i.MAC.ToLower().Equals(MAC.ToLower()) && i.IsDeleted.Equals(false) && i.IsActive.Equals(true)).FirstOrDefault();
            }
        }
        public List<RaspWorkPoint> WorkPointByHub(int Id)
        {
            using (var context = new EpicOSContext())
            {
                List<RaspWorkPoint> returnData = new List<RaspWorkPoint>();
                List<Workpoint> results = context.Workpoint.Where(i => i.HubID.Equals(Id) && i.IsDeleted.Equals(false) && i.IsActive.Equals(true)).ToList();
                foreach(Workpoint result in results){
                    RaspWorkPoint data = new RaspWorkPoint();
                    data.ID = result.ID;
                    data.IPaddress = result.IPaddress;
                    data.MAC = result.MAC;
                    data.Name = result.Name;
                    data.Type = 1;
                    returnData.Add(data);
                }
                return returnData;
            }
        }

        public Hub HubByMAC(string MAC)
        {
            using (var context = new EpicOSContext())
            {
                return context.Hub.Where(i => i.MAC.ToLower().Equals(MAC.ToLower()) && i.IsDeleted.Equals(false) && i.IsActive.Equals(true)).FirstOrDefault();
            }
        }

        public async Task<Result> TelemeryInsert(Telemery telemery, Workpoint point){
            Result result = new Result();
            TelemeryFilter filter = new TelemeryFilter();
            filter.DateStart = telemery.DateCreated.AddSeconds(-10);
            filter.DateEnd = telemery.DateCreated.AddSeconds(10);
            filter.FloorID = point.FloorID;
            filter.OfficeID = point.OfficeID;
            List<Telemery> LatestTelemeries = TelemeryGetAll(filter);
            if (LatestTelemeries.Count > 0)
            {
                using (var context = new EpicOSContext())
                {
                    Telemery theOne = context.Telemery.Where(t => t.MAC.ToLower().Equals(telemery.MAC.ToLower())).FirstOrDefault();
                    if (!theOne.IsActive)
                    {
                        result = await this.Insert(telemery);
                    }
                }
    
            }
            else
            {
                Console.WriteLine("test");
                result = await this.Insert(telemery);
            }
            return result;
        }
        
        public async Task<Result> LogInsert(Log log){
            Result result = new Result();
            using (var context = new EpicOSContext())
            {
                try {
                    context.Log.Add(log);
                    await context.SaveChangesAsync();
                    var response = new Result{
                        ID = log.ID,
                        IsSuccess = true,
                    };
                    return response;
                }catch(Exception e){
                    var response = new Result{
                        ExceptionMessage = e.Message
                    };
                    return response;
                }
        
            }
        }
        public async Task<Result> Insert(Telemery telemery){
            
            using (var context = new EpicOSContext())
            {
                try {
                    context.Telemery.Add(telemery);
                    await context.SaveChangesAsync();
                    var response = new Result{
                        ID = telemery.ID,
                        IsSuccess = true,
                    };
                    return response;
                }catch(Exception e){
                    var response = new Result{
                        ExceptionMessage = e.Message
                    };
                    return response;
                }
            }
        }

        public bool IsActive(string MAC, int type = 0)
        {
            bool result = false;
            try
            {
                using (var context = new EpicOSContext())
                {
                    switch (type)
                    {
                        case 1:
                            Workpoint workpoint = context.Workpoint.Where(w => w.MAC.ToLower().Equals(MAC.ToLower()) && w.IsActive.Equals(true)).FirstOrDefault();
                            if(workpoint != null){
                                result = true;
                            }
                            break;
                        case 2:
                            Hub hub = context.Hub.Where(w => w.MAC.ToLower().Equals(MAC.ToLower()) && w.IsActive.Equals(true)).FirstOrDefault();
                            if(hub != null){
                                result = true;
                            }
                            break;
                    }
                }

            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}