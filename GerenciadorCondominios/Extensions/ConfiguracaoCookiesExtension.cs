using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Protocol;

namespace GerenciadorCondominios.Extensions
{
    public static class ConfiguracaoCookiesExtension
    {
        public static void  ConfigurarCookies(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(opcoes =>
            {
                opcoes.Cookie.Name = "IdentityCookie";
                opcoes.Cookie.HttpOnly = true;
                opcoes.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                opcoes.LoginPath= "/Usuarios/Login";
            });
        }
    }
}
