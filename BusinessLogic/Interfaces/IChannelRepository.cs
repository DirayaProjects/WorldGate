using WebPortal.BusinessLogic.Exceptions;
using WebPortal.Models;
using WebPortal.ViewModels;

namespace WebPortal.BusinessLogic.Interfaces
{
    public interface IChannelRepository
    {
        Task<Result<List<Channel>>> GetAllChannels();

        Task<Result<List<Code>>> GetAllCategories();

        Task<Result<List<Code>>> GetAllResolutions();

        Task<Result<List<Code>>> GetAllPriorities();

        Task<Result<List<Code>>> GetAllSources();

        Task<Result<Channel>> GetSingleChannel(string id);

        Task<Result<bool>> CreateChannel(ChannelViewModel channel);

        Task<Result<bool>> UpdateChannel(ChannelViewModel channel);

        Task<Result<bool>> DeleteChannel(string id);
    }
}
