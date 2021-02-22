using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Core3.AzureAd.Configurations.Exceptions
{
    public class CustomException : Exception
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public new string Message { get; set; }
        public DateTimeOffset Time { get; set; }
        public string User { get; set; }
        public string Method { get; set; }
        public string Request { get; set; }
        public string Body { get; set; }

        public CustomException(Types type, string message = null) => Fill(type, message);

        public CustomException(int statusCode, string message = null)
        {
            bool valid = Enum.IsDefined(typeof(Types), statusCode);
            Types type = valid ? (Types)statusCode : Types.InternalServerError;
            Fill(type, message);
        }

        public override string ToString() => JsonConvert.SerializeObject(new { Title, Detail, Message, Source, Time, User, Method, Request, Body });

        public async Task AddRequest(HttpRequest request)
        {
            Time = DateTimeOffset.UtcNow;
            Method = request.Method;
            Request = request.Path;

            string[] bodyMethods = { "POST", "PUT", "PATCH" };
            if (!bodyMethods.Contains(request.Method)) return;

            request.Body.Position = 0;
            using StreamReader reader = new StreamReader(request.Body);

            Body = await reader.ReadToEndAsync();
        }

        public async Task AddUser(HttpContext context)
        {
            string jwt = await context.GetTokenAsync("access_token");

            if (jwt == null) return;

            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            User = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        }

        private void Fill(Types type, string message)
        {
            string details = StatusDetails.Get(type);
            IEnumerable<string> chars = type.ToString().Select(c => char.IsUpper(c) ? " " + c : c.ToString());

            Code = (int)type;
            Title = string.Concat(chars);
            Detail = details;
            Message = message ?? details;
        }
    }
}
