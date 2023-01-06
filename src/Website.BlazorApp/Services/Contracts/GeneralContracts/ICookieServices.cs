namespace Website.BlazorApp.Services.Contracts.GeneralContracts;

public interface ICookieServices
{
    void SetCookie(string name, string value, CookieOptions? cookieOptions);
    string GetCookie(string name);
    void RemoveCookie(string name);
}