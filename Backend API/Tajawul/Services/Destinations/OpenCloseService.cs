using Tajawul.Interfaces.Destinations;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Repositories.Destinations;

namespace Tajawul.Services.Destinations
{
    public class OpenCloseService : IOpenCloseService 
    {
            private readonly OpenCloseRepository _openCloseRepository;

            public OpenCloseService(OpenCloseRepository openCloseRepository)
            {

                _openCloseRepository = openCloseRepository;

            }
            public async Task<DestinationOpenClose> UpdateOpenCloseTimesAsync(string destinationId, TimeOnly openAt, TimeOnly closeAt)
            {
                return await _openCloseRepository.UpdateOpenCloseTimesAsync(destinationId, openAt, closeAt);
            }
    }
}