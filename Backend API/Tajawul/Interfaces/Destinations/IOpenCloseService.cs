using Tajawul.Models.Domain.Destinations;

namespace Tajawul.Interfaces.Destinations
{
    public interface IOpenCloseService
    {
        Task<DestinationOpenClose> UpdateOpenCloseTimesAsync(string destinationId, TimeOnly openAt, TimeOnly closeAt);

    }
}
