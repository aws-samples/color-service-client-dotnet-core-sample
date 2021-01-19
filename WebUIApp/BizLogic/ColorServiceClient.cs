using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using WebUIColorClient.Models;
using WebUIColorClient.Utility;

namespace WebUIColorClient.BizLogic
{
    public class ColorServiceClient
    {
        private readonly RestClient restClient;

        public ColorServiceClient(IOptionsSnapshot<Settings> settings)
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
