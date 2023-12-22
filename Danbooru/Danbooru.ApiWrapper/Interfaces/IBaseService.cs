namespace Danbooru.ApiWrapper.Interfaces;

public interface IBaseService
{
    Task<T?> GetFromApiAsync<T>(string uri);
}
