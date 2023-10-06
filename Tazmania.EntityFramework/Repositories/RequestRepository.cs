using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.EntityFramework.Contexts;
using Tazmania.Interfaces.Repositories;

namespace Tazmania.EntityFramework.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        readonly TazmaniaDbContext ctx;

        public RequestRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task Add(Request request)
        {
            // se la richiesta è già presente in lista non viene inserita
            if (!ctx.Requests.Any(r => r.Type == request.Type && r.Id == request.EntityId))
            {
                await ctx.Requests.AddAsync(request);
            }
        }

        public async Task<IList<Request>> FetchsAndClean()
        {
            var result = new List<Request>();

            if (await ctx.Requests.AnyAsync())
            {
                result = await ctx.Requests.ToListAsync();
                ctx.Requests.RemoveRange(result);
            }

            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
