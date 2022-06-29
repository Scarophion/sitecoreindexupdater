using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using System.Web.Mvc;

namespace SitecoreTools.IndexUpdate
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IndexUpdateController));
        }
    }
}
