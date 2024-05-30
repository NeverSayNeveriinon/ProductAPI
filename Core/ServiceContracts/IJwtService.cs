using Core.Domain;
using Core.DTO;

namespace Core.ServiceContracts;

public interface IJwtService
{
    AuthenticationResponse CreateJwtToken(ApplicationUser user);
}