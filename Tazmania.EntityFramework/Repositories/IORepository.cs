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
    public class IORepository : IIORepository
    {
        readonly TazmaniaDbContext ctx;

        public IORepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<IList<IO>> Fetchs(params IOMajor[] majors)
        {
            return await ctx.IOs.Include(r => r.IOGroup)
                                .Where(r => majors.Any() ? majors.Contains(r.Major) : true)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task SetActive(int id, bool isActive)
        {
            (await ctx.IOs.SingleAsync(r => r.Id == id)).IsActive = isActive;
        }

        public async Task SetValue(int id, float value)
        {
            (await ctx.IOs.SingleAsync(r => r.Id == id)).Value = value;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
