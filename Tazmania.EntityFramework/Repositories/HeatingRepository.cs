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
    public class HeatingRepository : IHeatingRepository
    {
        readonly TazmaniaDbContext ctx;

        public HeatingRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<IList<Heating>> Fetchs()
        {
            return await ctx.Heatings.AsNoTracking().ToListAsync();
        }

        public async Task<HeatingSetting> FetchSettings()
        {
            return await ctx.HeatingSettings.AsNoTracking().Include(r => r.Times).SingleAsync();
        }

        public async Task SetMode(HeatingMode mode)
        {
           (await ctx.HeatingSettings.SingleAsync()).Mode = mode;
        }

        public async Task SetTemperature(int id, float temperature)
        {
            (await ctx.Heatings.SingleAsync(r => r.Id == id)).Temperature = temperature;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
