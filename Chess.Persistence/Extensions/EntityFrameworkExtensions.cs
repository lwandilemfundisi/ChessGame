using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Extensions;
using Microservice.Framework.Persistence;
using Microservice.Framework.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chess.Persistence.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static IDomainContainer ConfigureChessPersistence(
            this IDomainContainer domainContainer)
        {
            return domainContainer
                .ConfigureEntityFramework(EntityFrameworkConfiguration.New)
                .AddDbContextProvider<ChessContext, ChessContextProvider>()
                .RegisterServices(sr =>
                {
                    sr.AddTransient<IPersistenceFactory, EntityFrameworkPersistenceFactory<ChessContext>>();
                });
        }
        
        public static IDomainContainer ConfigureEntityFramework(
            this IDomainContainer domainContainer,
            IEntityFrameworkConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            configuration.Apply(domainContainer.ServiceCollection);
            return domainContainer;
        }

        public static IDomainContainer AddDbContextProvider<TDbContext, TContextProvider>(
            this IDomainContainer domainContainer)
            where TContextProvider : class, IDbContextProvider<TDbContext>
            where TDbContext : DbContext
        {
            domainContainer
                .ServiceCollection
                .AddTransient<IDbContextProvider<TDbContext>, TContextProvider>();

            return domainContainer;
        }
    }
}
