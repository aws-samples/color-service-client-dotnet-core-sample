using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using WebUIApp.Models;
using WebUIApp.Utility;

namespace WebUIApp.BizLogic
{
    public class ServiceAClient
    {
        private readonly RestClient restClient;

        public ServiceAClient(IOptionsSnapshot<Settings> settings)
        {
            this.restClient = new RestClient(settings.Value.ServiceBaseUrl);
        }

        public async Task<Color> GetColor()
        {
            string colorString;
            try
            {
                var restRequest = new RestRequest("color");
                colorString = await this.restClient.GetAsync<string>(restRequest);
                if (string.IsNullOrWhiteSpace(colorString))
                    colorString = "Blank Color returned by Color service.";
            }
            catch (Exception e)
            {
                colorString = e.Innermost().Message;
            }

            var color = new Color { Value = colorString };
            return color;
        }
    }
}
