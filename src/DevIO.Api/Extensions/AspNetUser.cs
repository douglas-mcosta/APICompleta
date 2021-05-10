using System;
using System.Collections.Generic;
using System.Security.Claims;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DevIO.Api.Extensions
{
    public class AspNetUser : IUser
    {

        private readonly IHttpContextAccessor _acessor;

        public AspNetUser(IHttpContextAccessor acessor)
        {
            _acessor = acessor;
        }

        public string Name => _acessor.HttpContext.User.Identity.Name;

        public Guid GetUserId()
        {
            return IsAuthenticated() ? Guid.Parse(_acessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }
        public string GetUserEmail()
        {
            return IsAuthenticated() ? _acessor.HttpContext.User.GetUserEmail() : "";
        }
        public bool IsAuthenticated()
        {
            return _acessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsInRole(string role)
        {
            return _acessor.HttpContext.User.IsInRole(role);
        }
        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _acessor.HttpContext.User.Claims;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal){
            
            if(principal == null) throw new ArgumentException(nameof(principal));

            var claims = principal.FindFirst(ClaimTypes.NameIdentifier);

            return  claims?.Value;
        }
        public static string GetUserEmail(this ClaimsPrincipal principal){
            
            if(principal == null) throw new ArgumentException(nameof(principal));

            var claims = principal.FindFirst(ClaimTypes.Email);

            return  claims?.Value;
        }
    }
}