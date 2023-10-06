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
    public class IrrigationRepository : IIrrigationRepository
    {
        readonly TazmaniaDbContext ctx;

        public IrrigationRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<IList<Irrigation>> Fetchs()
        {
            return await ctx.Irrigations.AsNoTracking().ToListAsync();
        }

        public async Task<IrrigationSetting> FetchSettings()
        {
            return await ctx.IrrigationSettings.AsNoTracking().SingleAsync();
        }

        public async Task SetExecution()
        {
            (await ctx.IrrigationSettings.SingleAsync()).LastExecution = DateTime.Now.Date;
        }

        public async Task SetMode(IrrigationMode mode)
        {
            (await ctx.IrrigationSettings.SingleAsync()).Mode = mode;
        }

        public async Task SetWeekDays(DayOfWeek dayOfWeek, bool isActive)
        {
            var settings = await ctx.IrrigationSettings.SingleAsync();

            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: settings.Sunday = isActive; break;
                case DayOfWeek.Monday: settings.Monday = isActive; break;
                case DayOfWeek.Tuesday: settings.Tuesday = isActive; break;
                case DayOfWeek.Wednesday: settings.Wednesday = isActive; break;
                case DayOfWeek.Thursday: settings.Thursday = isActive; break;
                case DayOfWeek.Friday: settings.Friday = isActive; break;
                case DayOfWeek.Saturday: settings.Saturday = isActive; break;
            }
        }

        public async Task SetWatering(int irrigationId, bool isActive)
        {
            (await ctx.Irrigations.SingleAsync(r => r.Id == irrigationId)).WateringEnd = (isActive ? DateTime.Now.AddMinutes(10) : DateTime.MinValue);
        }

        public async Task SetTimer(int irrigationId, int minutes)
        {
            (await ctx.Irrigations.SingleAsync(r => r.Id == irrigationId)).Minutes = minutes;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
