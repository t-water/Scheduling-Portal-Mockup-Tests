using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TEServerTest.Data;

namespace Testing
{
    public static class Utilities
    {
        public static DbContextOptions<TEServerContext> TestDbContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<TEServerContext>()
                .UseSqlServer("TEServerContext")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}