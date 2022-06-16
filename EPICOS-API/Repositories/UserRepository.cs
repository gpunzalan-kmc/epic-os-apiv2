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
{
    public class UserRepository
    {

        public List<User> UserGetAll(UserFilter filters)
        {
            // IEnumerable enumerable = filters as IEnumerable;
            using (var context = new EpicOSContext())
            {
                FilterContainer filter = QueryDesigner<UserFilter>.Query(filters);
                IQueryable<User> query = context.User;
                if(filter.Where.Operands.Count > 0)
                    query = query.Request(filter);
                var list = query.Skip(((filters.Page-1) * filters.Limit)).Take(filters.Limit).ToList();
                return list;
            }
               
        }

        public User UserGetID(int Id)
        {
            using (var context = new EpicOSContext())
            {
                var result = context.User.Where(e => e.ID.Equals(Id)).FirstOrDefault();
                return result;
            }
               
        }
        
        public async Task<Response<User>> Create(User parameter)
        {
            using (var context = new EpicOSContext())
            {
                var users = context.User.Where(e => e.UserName.Equals(parameter.UserName) && e.IsDeleted.Equals(false)).FirstOrDefault();
                if (users == null){
                    try {
                        context.User.Add(parameter);
                        await context.SaveChangesAsync();
                        var response = new Response<User>{
                            Data = parameter,
                            Succeeded = true,
                            Message = "Record created"
                        };
                        return response;
                    }catch(Exception e){
                        var response = new Response<User>{
                            StatusCode = 500,
                            Succeeded = false,
                            Message = e.Message
                        };
                        return response;
                    }
                }else {
                    var response = new Response<User>{
                        StatusCode = 403,
                        Succeeded = false,
                        Message = "Duplicate username"
                    };
                    return response;
                }

            }
               
        }

        public User ValidateUsername(string username)
        {
            using (var context = new EpicOSContext())
            {
                var result = context.User.Where(e => e.UserName.Equals(username)).FirstOrDefault();
                return result;
            }
      
        }

        public async Task<Response<User>> Update(User parameter, int Id)
        {
            using (var context = new EpicOSContext())
            {
                
                var checkRecord = context.User.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                if (checkRecord == null){
                    var response = new Response<User>{
                        StatusCode = 404,
                        Succeeded = false,
                        Message = "Record not found"
                    };
                    return response;
                }
                var users = context.User.Where(e => e.UserName.Equals(parameter.UserName) && e.IsDeleted.Equals(false) && !e.ID.Equals(Id)).FirstOrDefault();
                if (users == null){
                    context.Entry(checkRecord).State = EntityState.Detached;
                    
                    try {
                        parameter.ID = Id;
                        context.Entry(parameter).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                        var response = new Response<User>{
                            Data = parameter,
                            Succeeded = true,
                            Message = "Record updated"
                        };
                        return response;
                    }catch(Exception e){
                        var response = new Response<User>{
                            StatusCode = 500,
                            Succeeded = false,
                            Message = e.Message
                        };
                        return response;
                    }
                }else {
                    var response = new Response<User>{
                        Succeeded = false,
                        Message = "Duplicate username"
                    };
                    return response;
                }


            }  
        }

        public async Task<Response<User>> Delete(int Id)
        {
            using (var context = new EpicOSContext())
            {
            
                var checkRecord = context.User.Where(e => e.ID.Equals(Id) && e.IsDeleted.Equals(false)).FirstOrDefault();
                if (checkRecord == null){
                    var response = new Response<User>{
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
                    var response = new Response<User>{
                        Succeeded = true,
                        Message = "Record deleted"
                    };
                    return response;
                }catch(Exception e){
                    var response = new Response<User>{
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