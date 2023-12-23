namespace Danbooru.UI.Interfaces;

public interface ICookieService
{
    public void SetCookie(string key, string value, int? expireTime);
    public string GetCookie(string key);
}
