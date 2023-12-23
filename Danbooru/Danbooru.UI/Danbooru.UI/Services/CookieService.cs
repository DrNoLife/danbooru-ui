using Microsoft.AspNetCore.Http;
using Danbooru.UI.Interfaces;

namespace Danbooru.UI.Services;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext.Request.Cookies[key];
    }

    public void SetCookie(string key, string value, int? expireTime)
    {
        CookieOptions option = new CookieOptions();

        if (expireTime.HasValue)
            option.Expires = DateTime.Now.AddDays(expireTime.Value);
        else
            option.Expires = DateTime.Now.AddMinutes(2);

        _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
    }
}
