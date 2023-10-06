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
    public class SecurityRepository : ISecurityRepository
    {
        readonly TazmaniaDbContext ctx;

        public SecurityRepository(DbContextOptionsBuilder<TazmaniaDbContext> optionsBuilder)
        {
            ctx = new TazmaniaDbContext(optionsBuilder.Options);
        }

        public async Task<SecuritySetting> FetchSettings()
        {
            return await ctx.SecuritySettings.AsNoTracking().SingleAsync();
        }

        public async Task SetAntitheftMode(SecurityAntitheftMode mode)
        {
            (await ctx.SecuritySettings.SingleAsync()).AntitheftMode = mode;
        }

        public async Task ActiveAntiFire(string detail)
        {
            if (detail == null)
            {
                throw new ArgumentNullException(nameof(detail));
            }

            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.AntiFireActivationDateTime = DateTime.Now;
            setting.AntiFireDetail = detail;
        }

        public async Task ActiveSOS(string detail)
        {
            if (detail == null)
            {
                throw new ArgumentNullException(nameof(detail));
            }

            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.SOSActivationDateTime = DateTime.Now;
            setting.SOSDetail = detail;
        }

        public async Task ActiveAntitheft(string detail)
        {
            if (detail == null)
            {
                throw new ArgumentNullException(nameof(detail));
            }

            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.AntitheftActivationDateTime = DateTime.Now;
            setting.AntitheftDetail = detail;
        }

        public async Task DeactiveSOS()
        {
            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.SOSActivationDateTime = DateTime.MinValue;
            setting.SOSDetail = string.Empty;
        }

        public async Task DeactiveAntiFire()
        {
            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.AntiFireActivationDateTime = DateTime.MinValue;
            setting.AntiFireDetail = string.Empty;
        }

        public async Task DeactiveAntitheft()
        {
            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.AntitheftActivationDateTime = DateTime.MinValue;
            setting.AntitheftDetail = string.Empty;
        }

        public async Task DeactiveAlarms()
        {
            var setting = await ctx.SecuritySettings.SingleAsync();
            ctx.Entry(setting).Reload();
            setting.SOSActivationDateTime = DateTime.MinValue;
            setting.SOSDetail = string.Empty;
            setting.AntiFireActivationDateTime = DateTime.MinValue;
            setting.AntiFireDetail = string.Empty;
            setting.AntitheftActivationDateTime = DateTime.MinValue;
            setting.AntitheftDetail = string.Empty;
        }

        public async Task SetNotifySiren(bool isActive)
        {
            (await ctx.SecuritySettings.SingleAsync()).NotifySiren = isActive;
        }

        public async Task SetNotifyCall(bool isActive)
        {
            (await ctx.SecuritySettings.SingleAsync()).NotifyCall = isActive;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }
    }
}
