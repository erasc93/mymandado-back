// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.services;

public class ClaimsAccessor(IHttpContextAccessor _httpContextAccessor)
{
    //private readonly IHttpContextAccessor _httpContextAccessor;

    //public ClaimsAccessor()
    //{
    //    _httpContextAccessor = accessor;
    //}

    public string GetUsername()
    {
        string? username
        = _httpContextAccessor.HttpContext?.User?.Claims
            ?.FirstOrDefault(c => c.Type == "username")?.Value;
        return username ?? string.Empty;
    }
}
