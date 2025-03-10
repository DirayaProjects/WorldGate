using FireSharp.Interfaces;
using FireSharp.Response;
using WebPortal.BusinessLogic.Exceptions;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.Utilities;
using WebPortal.ViewModels;

namespace WebPortal.BusinessLogic.Repositories
{
    public class ChannelRepository : IChannelRepository
    {

        private readonly IFirebaseClient _client;
        private readonly FirebaseStorageUtility _storageUtility;

        public ChannelRepository()
        {
            FirebaseUtility firebaseUtility = new FirebaseUtility();
            _client = firebaseUtility.GetClient();
            _storageUtility = new FirebaseStorageUtility();
        }

        public async Task<Result<List<Channel>>> GetAllChannels()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Channels/");

                if (response.Body == "null")
                {
                    return Result<List<Channel>>.Failure("No channels found.");
                }

                Dictionary<string, Channel> channelsDict = response.ResultAs<Dictionary<string, Channel>>();
                List<Channel> channels = channelsDict
                    .Where(u => u.Value.IsDeleted == 0)
                    .Select(u =>
                    {
                        u.Value.Id = u.Key;
                        return u.Value;
                    }).ToList();

                if (channels == null || !channels.Any())
                {
                    return Result<List<Channel>>.Failure("No channels found.");
                }

                return Result<List<Channel>>.Success(channels); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<Channel>>.Failure("An error occurred while fetching channels.");
            }
        }

        public async Task<Result<Channel>> GetSingleChannel(string id)
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync($"Channels/{id}");
                if (response.Body == "null")
                {
                    return Result<Channel>.Failure("Channel not found");
                }

                Channel channel = response.ResultAs<Channel>();
                if (channel.IsDeleted == 1)
                {
                    return Result<Channel>.Failure("Channel is deleted");
                }

                return Result<Channel>.Success(channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<Channel>.Failure("An error occurred while fetching channel.");
            }
        }


        public async Task<Result<bool>> CreateChannel(ChannelViewModel channelViewModel)
        {
            try
            {

                if (channelViewModel == null)
                {
                    return Result<bool>.Failure("Invalid channel data.");
                }

                string? iconUrl;
                if (channelViewModel.Icon != null)
                {
                    iconUrl = await _storageUtility.UploadFile(channelViewModel.Icon);
                }
                else
                {
                    iconUrl = null;
                }

                var channel = new Channel
                {
                    Name = channelViewModel.Name,
                    Category = channelViewModel.Category,
                    Icon = iconUrl,
                    Links = channelViewModel.Links
                };


                PushResponse response = await _client.PushAsync("Channels/", channel);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    channel.Id = response.Result.name;
                    FirebaseResponse updateResponse = await _client.UpdateAsync($"Channels/{channel.Id}", channel);

                    if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Result<bool>.Success(true);
                    }
                }

                return Result<bool>.Failure("Failed to create channel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while creating channel.");
            }
        }

        public async Task<Result<bool>> UpdateChannel(ChannelViewModel channel)
        {
            try
            {
                if (channel == null || string.IsNullOrEmpty(channel.Id))
                {
                    return Result<bool>.Failure("Invalid user data.");
                }


                FirebaseResponse existingChannelResponse = await _client.GetAsync($"Channels/{channel.Id}");
                if (existingChannelResponse.Body == "null")
                {
                    return Result<bool>.Failure("Channel not found");
                }

                Channel existingChannel = existingChannelResponse.ResultAs<Channel>();

                if (existingChannel.IsDeleted == 1)
                {
                    return Result<bool>.Failure("Cannot update a deleted channel.");
                }


                existingChannel.Name = channel.Name;
                existingChannel.Category = channel.Category;

                string? iconUrl;
                if (channel.Icon != null)
                {
                    iconUrl = await _storageUtility.UploadFile(channel.Icon);
                    existingChannel.Icon = iconUrl;
                }


                List<Link> existingLinks = existingChannel.Links ?? new List<Link>();

                List<Link> updatedLinks = new List<Link>();

                if (channel.Links != null)
                {
                    foreach (var link in channel.Links)
                    {

                        if (string.IsNullOrWhiteSpace(link.Id))
                        {
                            link.Id = Guid.NewGuid().ToString();
                        }

                        updatedLinks.Add(link);
                    }
                }

                existingChannel.Links = updatedLinks;

                FirebaseResponse response = await _client.UpdateAsync($"Channels/{channel.Id}", existingChannel);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Failed to update channel.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while updating the channel.");
            }
        }


        public async Task<Result<bool>> DeleteChannel(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Result<bool>.Failure("Invalid channel ID.");
                }

                FirebaseResponse response = await _client.GetAsync($"Channels/{id}");
                if (response.Body == "null")
                {
                    return Result<bool>.Failure("Channel not found.");
                }

                Channel channel = response.ResultAs<Channel>();

                if (channel.IsDeleted == 1)
                {
                    return Result<bool>.Failure("Channel is already deleted.");
                }

                channel.IsDeleted = 1;

                FirebaseResponse updateResponse = await _client.UpdateAsync($"Channels/{id}", channel);

                if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Failed to delete channel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while deleting the channel.");
            }
        }


        public async Task<Result<List<Code>>> GetAllCategories()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Codes/");

                if (response.Body == "null")
                {
                    return Result<List<Code>>.Failure("No codes found.");
                }

                Dictionary<string, Code> categoriesDict = response.ResultAs<Dictionary<string, Code>>();
                List<Code> categories = categoriesDict.Values.Where(c => c.Kind == 1 && c.IsDeleted == 0).ToList();

                if (categories == null || !categories.Any())
                {
                    return Result<List<Code>>.Failure("No categories found.");
                }

                return Result<List<Code>>.Success(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<Code>>.Failure("An error occurred while fetching categories.");
            }
        }

        public async Task<Result<List<Code>>> GetAllResolutions()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Codes/");

                if (response.Body == "null")
                {
                    return Result<List<Code>>.Failure("No codes found.");
                }

                Dictionary<string, Code> resolutionsDict = response.ResultAs<Dictionary<string, Code>>();
                List<Code> resolutions = resolutionsDict.Values.Where(c => c.Kind == 2 && c.IsDeleted == 0).ToList();

                if (resolutions == null || !resolutions.Any())
                {
                    return Result<List<Code>>.Failure("No resolutions found.");
                }

                return Result<List<Code>>.Success(resolutions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<Code>>.Failure("An error occurred while fetching resolutions.");
            }
        }

        public async Task<Result<List<Code>>> GetAllSources()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Codes/");

                if (response.Body == "null")
                {
                    return Result<List<Code>>.Failure("No codes found.");
                }

                Dictionary<string, Code> sourcesDict = response.ResultAs<Dictionary<string, Code>>();
                List<Code> sources = sourcesDict.Values.Where(c => c.Kind == 3 && c.IsDeleted == 0).ToList();

                if (sources == null || !sources.Any())
                {
                    return Result<List<Code>>.Failure("No sources found.");
                }

                return Result<List<Code>>.Success(sources);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<Code>>.Failure("An error occurred while fetching sources.");
            }
        }

        public async Task<Result<List<Code>>> GetAllPriorities()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Codes/");

                if (response.Body == "null")
                {
                    return Result<List<Code>>.Failure("No codes found.");
                }

                Dictionary<string, Code> prioritiesDict = response.ResultAs<Dictionary<string, Code>>();
                List<Code> priorities = prioritiesDict.Values.Where(c => c.Kind == 4 && c.IsDeleted == 0).ToList();

                if (priorities == null || !priorities.Any())
                {
                    return Result<List<Code>>.Failure("No priorities found.");
                }

                return Result<List<Code>>.Success(priorities);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<Code>>.Failure("An error occurred while fetching priorities.");
            }
        }


    }
}
