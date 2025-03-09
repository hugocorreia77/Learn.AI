using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Learn.Core.Api.Jwt
{
    public class LearnJwtBearerEvents : JwtBearerEvents
    {
        public override async Task MessageReceived(MessageReceivedContext context)
        {
            // Verifica se a autenticação já encontrou um token no header Authorization
            if (!string.IsNullOrEmpty(context.Token))
            {
                return;
            }

            // Caso não tenha vindo no header, tenta extrair da Query String (para SignalR)
            var accessToken = context.Request.Query["access_token"];

            // Verifica se a requisição é WebSocket (SignalR usa WS/WSS)
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/quizHub"))
            {
                context.Token = accessToken;
            }

            await Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                context.Fail("Token validation failed");
                return;
            }

            // Validar se o token tem o o id do user
            var userIdClaim = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                context.Fail("Invalid user");
                return;
            }
            context.Success();
            await Task.CompletedTask;
        }
    }
}