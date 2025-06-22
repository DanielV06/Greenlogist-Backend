using Greenlogist.Domain;

namespace Greenlogist.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user);
}