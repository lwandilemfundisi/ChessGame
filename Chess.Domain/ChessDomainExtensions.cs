using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain
{
    public static class ChessDomainExtensions
    {
        public static Assembly Assembly { get; } = typeof(ChessDomainExtensions).Assembly;

        public static IDomainContainer ConfigureChessDomain(
            this IServiceCollection services)
        {
            return DomainContainer.New(services)
                .AddDefaults(Assembly);
        }
    }
}
