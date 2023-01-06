using Website.BlazorApp.Services.Contracts.GeneralContracts;

namespace Website.BlazorApp.Services.Implementations.GeneralServices;

public class CookieServices : ICookieServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieServices(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCookie(string name)
    {
        return _httpContextAccessor.HttpContext.Request.Cookies[name];
    }

    public void RemoveCookie(string name)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(name);
    }

    public void SetCookie(string name, string value, CookieOptions? cookieOptions)
    {
        if (cookieOptions != null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(name, value, cookieOptions);
        }
        else
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(name, value);
        }
    }
}
