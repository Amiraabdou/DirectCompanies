using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DirectCompanies.Security
{
    public class ApiKeyAuthAttribute() : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IConfiguration configuration = (IConfiguration)ServiceProviderHelper.ServiceProvider.GetService(typeof(IConfiguration));
           var apiKey= configuration.GetValue<string>("ApiKey");
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var
            extractedApiKey) ||
            !string.Equals(extractedApiKey, apiKey))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}

