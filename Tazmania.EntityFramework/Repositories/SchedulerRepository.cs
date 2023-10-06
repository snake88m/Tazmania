using Microsoft.EntityFrameworkCore;
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
    public class SchedulerRepository : ISchedulerRepository
    {
        readonly TazmaniaDbContext ctx;

        public SchedulerRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<IList<Scheduler>> Fetchs()
        {
            return await ctx.Schedulers.Where(r => r.Mode != SchedulerMode.Disabled)
                                       .Include(r => r.Items)
                                       .AsNoTracking()
                                       .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
