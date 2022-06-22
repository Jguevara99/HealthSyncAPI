using ContosoPizza.Models;

namespace ContosoPizza.Services;

public class JwtAuthenticationService : IJwtAuthenticationService
{
    private readonly string _key;

    public JwtAuthenticationService(string key)
    {
        this._key = key;
    }

    public string Authenticate(string username, string password)
    {
        if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new HttpException("");


        return string.Empty;    
    }
}

public interface IJwtAuthenticationService
{
    string Authenticate(string username, string password);
}