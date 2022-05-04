using CodeGen.Application.Configurations.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Infrastructure.Configurations
{

    public class ConnectionsConfigurations : IConnectionConfigurations
    {
        public static IConfiguration _config;

        public ConnectionsConfigurations(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string DbConnectionString()
        {
            return _config["ConnectionStrings:Server"];
        }
    }
}
