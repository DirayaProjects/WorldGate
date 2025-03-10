using FireSharp.Interfaces;
using FireSharp.Response;
using WebPortal.Models;

namespace WebPortal.Utilities
{
	public class PlanExpirationChecker : BackgroundService
	{
		private readonly IFirebaseClient _firebaseClient;
		private readonly ILogger<PlanExpirationChecker> _logger;

		public PlanExpirationChecker(ILogger<PlanExpirationChecker> logger, FirebaseUtility firebaseUtility)
		{
			_firebaseClient = firebaseUtility?.GetClient() ?? throw new ArgumentNullException(nameof(firebaseUtility));
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await CheckAndUpdateExpiredPlans();
				await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
			}
		}

		private async Task CheckAndUpdateExpiredPlans()
		{
			try
			{
				FirebaseResponse response = await _firebaseClient.GetAsync("Users/");
				if (response.Body == "null" || string.IsNullOrWhiteSpace(response.Body))
				{
					_logger.LogInformation("No users found in Firebase.");
					return;
				}

				Dictionary<string, User> usersDict = response.ResultAs<Dictionary<string, User>>();
				List<User> users = usersDict
					.Where(u => u.Value.IsDeleted == 0)
					.Select(u =>
					{
						u.Value.Id = u.Key;
						return u.Value;
					}).ToList();

				if (users == null || users.Count == 0)
				{
					_logger.LogInformation("Users list is empty.");
					return;
				}

				//_logger.LogInformation($"Fetched Users: {JsonConvert.SerializeObject(users)}");

				int updatedCount = 0;
				DateTimeOffset today = DateTimeOffset.Now;
				today = today.AddTicks(-(today.Ticks % TimeSpan.TicksPerSecond));

				_logger.LogInformation($"Today: {today}");

				foreach (var user in users)
				{
					if (user.UserPlan?.EndDate != null)
					{
						user.UserPlan.EndDate = user.UserPlan.EndDate;
						user.UserPlan.EndDate = user.UserPlan.EndDate.AddTicks(-(user.UserPlan.EndDate.Ticks % TimeSpan.TicksPerSecond));
					}

					_logger.LogInformation($"{user.FirstName} - EndDate: {user.UserPlan?.EndDate}");

					if (user.UserPlan?.EndDate <= today && user.IsActive == 1)
					{
						user.PlanId = null;
						user.IsActive = 0;
						user.UserPlan = null;
						await _firebaseClient.SetAsync($"Users/{user.Id}", user);
						updatedCount++;
						_logger.LogInformation($"Deactivated: {user.FirstName}");
					}
				}

				_logger.LogInformation($"Deactivated {updatedCount} expired users.");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error updating expired plans: {ex}");
			}
		}
	}

}