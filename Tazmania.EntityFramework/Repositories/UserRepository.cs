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
    public class UserRepository : IUserRepository
    {
        readonly TazmaniaDbContext ctx;

        public UserRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<User?> Fetch(string mail, string passwordHash)
        {
            return await ctx.Users.AsNoTracking().SingleOrDefaultAsync(r => r.Mail == mail && r.PasswordHash == passwordHash);
        }

        public async Task<IList<User>> Fetchs()
        {
            return await ctx.Users.AsNoTracking().ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
