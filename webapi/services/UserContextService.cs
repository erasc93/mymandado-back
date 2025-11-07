// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_mandado.services;

public class UserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public string? GetUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims
            ?.FirstOrDefault(c => c.Type == "username")?.Value;
    }
}
