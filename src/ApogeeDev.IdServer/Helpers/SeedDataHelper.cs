using System.Linq;
using System.Threading.Tasks;
using ApogeeDev.IdServer.Core.Config.Default;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApogeeDev.IdServer.Helpers
{
    public class SeedDataHelper
    {
        public SeedDataHelper()
        {

        }
        public void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                AddOrUpdateClients(context);

                AddOrUpdateIdentityResources(context);

                AddOrUpdateApiResources(context);
            }
        }
        private void AddOrUpdateApiResources(ConfigurationDbContext context)
        {
            foreach (var api in ApplicationData.Apis)
            {
                var existing = context.ApiResources.Where(i => i.Name.Equals(api.Name))
                    .FirstOrDefault();
                var newEntity = api.ToEntity();
                if (existing.IsNull())
                {
                    context.ApiResources.Add(newEntity);
                }
                else
                {
                    newEntity.Id = existing.Id;

                    context.Entry(existing).CurrentValues.SetValues(newEntity);
                }
            }
            context.SaveChanges();
        }
        private void AddOrUpdateIdentityResources(ConfigurationDbContext context)
        {
            foreach (var ident in ApplicationData.Identities)
            {
                var existing = context.IdentityResources.Where(i => i.Name.Equals(ident.Name))
                    .FirstOrDefault();
                var newEntity = ident.ToEntity();
                if (existing.IsNull())
                {
                    context.IdentityResources.Add(newEntity);
                }
                else
                {
                    newEntity.Id = existing.Id;

                    context.Entry(existing).CurrentValues.SetValues(newEntity);
                }
            }
            context.SaveChanges();
        }
        private void AddOrUpdateClients(ConfigurationDbContext context)
        {
            foreach (var client in ApplicationData.Clients)
            {
                var existing = context.Clients.Where(c => c.ClientName.Equals(client.ClientName))
                    .FirstOrDefault();
                var newEntity = client.ToEntity();
                if (existing.IsNull())
                {
                    context.Clients.Add(newEntity);
                }
                else
                {
                    newEntity.Id = existing.Id;

                    context.Entry(existing).CurrentValues.SetValues(newEntity);
                }
            }
            context.SaveChanges();
        }
    }
}