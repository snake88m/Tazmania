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
    public class NotifyRepository : INotifyRepository
    {
        readonly TazmaniaDbContext ctx;

        public NotifyRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<NotifySetting> FetchSettings()
        {
            return await ctx.NotifySettings.AsNoTracking().SingleAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
