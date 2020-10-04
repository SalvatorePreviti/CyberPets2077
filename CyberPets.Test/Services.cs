using System;
using CyberPets.API;
using Microsoft.AspNetCore.Hosting;

namespace CyberPetsTest
{
    public class Services
    {
        public static IServiceProvider CreateServiceProvider()
        {
            return Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build().Services;
        }
    }
}