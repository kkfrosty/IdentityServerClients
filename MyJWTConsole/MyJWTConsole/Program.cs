using MyJWTConsole.WcfJwtSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace MyJWTConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var _response = GetClientToken();

            CallApi(_response);

            _response = GetUserToken();
            CallApi(_response);
            CallWcf(_response);
        }

        static void CallWcf(TokenResponse token)
        {
            var _httpRequestProperty = new HttpRequestMessageProperty();

            _httpRequestProperty.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", token.AccessToken);
            //_httpRequestProperty.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", _token.AccessToken);
            var _wcfClient = new Service1Client();

            var _context = new OperationContext(_wcfClient.InnerChannel);

            using (new OperationContextScope(_context))
            {
                _context.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = _httpRequestProperty;

                var _response = _wcfClient.GetData(10);

                Console.WriteLine(string.Format("WCF GetData() Call: {0}", _response));

            }
        }

        static void CallApi(TokenResponse response)
        {
            var client = new HttpClient();
            client.SetBearerToken(response.AccessToken);

            Console.WriteLine(client.GetStringAsync("http://localhost/IdentityWebApis/test").Result);
        }

        static TokenResponse GetClientToken()
        {
            var client = new OAuth2Client(
                new Uri("https://localhost/IdSvr/core/connect/token"),
                "silicon",
                "F621F470-9731-4A25-80EF-67A6F7C5F4B8");

            return client.RequestClientCredentialsAsync("api1").Result;
        }

        static TokenResponse GetUserToken()
        {
            var client = new OAuth2Client(
              //  new Uri("https://localhost/IdSvr/core/connect/token")
                new Uri("https://localhost/IdSvr/core/connect/token"),
                "carbon",
                "21B5F798-BE55-42BC-8AA8-0025B903DC3B"
                );


            return client.RequestResourceOwnerPasswordAsync("bob", "secret", "api1").Result;
        }
    }
}
