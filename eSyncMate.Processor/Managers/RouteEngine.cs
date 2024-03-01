using eSyncMate.DB;
using eSyncMate.DB.Entities;
using eSyncMate.Processor.Models;
using Hangfire;

namespace eSyncMate.Processor.Managers
{
    public class RouteEngine
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public RouteEngine(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Execute(int routeId)
        {
            Routes route = new Routes();

            try
            {
                route.UseConnection(CommonUtils.ConnectionString);

                route.Id = routeId;
                if (!route.GetObject().IsSuccess)
                {
                    this._logger.LogError($"Invalid Route! [{routeId}]");
                    return;
                }

                if (route.TypeId == Convert.ToInt32(RouteTypesEnum.InventoryFeed))
                {
                    InventoryFeedRoute.Execute(_config, _logger, route);
                }
                else if (route.TypeId == Convert.ToInt32(RouteTypesEnum.GetOrders))
                {
                    GetOrdersRoute.Execute(_config, _logger, route);
                }
                else if (route.TypeId == Convert.ToInt32(RouteTypesEnum.CreateOrder))
                {
                    CreateOrderRoute.Execute(_config, _logger, route);
                }
                else if (route.TypeId == Convert.ToInt32(RouteTypesEnum.GetOrderStatus))
                {
                    GetOrderStatusRoute.Execute(_config, _logger, route);
                }
                else if (route.TypeId == Convert.ToInt32(RouteTypesEnum.ASN))
                {
                    ASNRoute.Execute(_config, _logger, route);
                }
                else if (route.TypeId == Convert.ToInt32(RouteTypesEnum.CreateInvoice))
                {
                    CreateInvoiceRoute.Execute(_config, _logger, route);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);
            }
        }

        public void Schedule(int routeId)
        {
            Routes route = new Routes();

            try
            {
                route.UseConnection(CommonUtils.ConnectionString);

                route.Id = routeId;
                if (!route.GetObject().IsSuccess)
                {
                    this._logger.LogError($"Invalid Route! [{routeId}]");
                    return;
                }

                this.SetupRouteJob(route);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);
            }
        }

        private void SetupRouteJob(Routes route)
        {
            RouteEngine l_Engine = this;

            if (route.FrequencyType == "Minutely")
            {
                RecurringJob.AddOrUpdate($"Route [{route.Id}]", () => l_Engine.Execute(route.Id), Cron.MinuteInterval(route.RepeatCount));
            }
            else if (route.FrequencyType == "Hourly")
            {
                RecurringJob.AddOrUpdate($"Route [{route.Id}]", () => l_Engine.Execute(route.Id), Cron.HourInterval(route.RepeatCount));
            }
            else if (route.FrequencyType == "Daily")
            {
                if (string.IsNullOrEmpty(route.ExecutionTime))
                {
                    RecurringJob.AddOrUpdate($"Route [{route.Id}]", () => l_Engine.Execute(route.Id), Cron.Daily);
                }
                else
                {
                    foreach (string time in route.ExecutionTime.Split(","))
                    {
                        string[] split = time.Split(":");
                        RecurringJob.AddOrUpdate($"Route [{route.Id}] at [{time}]", () => l_Engine.Execute(route.Id), Cron.Daily(Convert.ToInt32(split[0]), Convert.ToInt32(split[1])));
                    }
                }
            }
            else if (route.FrequencyType == "Weekly")
            {
                foreach (string weekday in route.WeekDays.Split(","))
                {
                    if (string.IsNullOrEmpty(route.ExecutionTime))
                    {
                        RecurringJob.AddOrUpdate($"Route [{route.Id}] on [{weekday}]", () => l_Engine.Execute(route.Id), Cron.Weekly(Enum.Parse<DayOfWeek>(weekday)));
                    }
                    else
                    {
                        foreach (string time in route.ExecutionTime.Split(","))
                        {
                            string[] split = time.Split(":");
                            RecurringJob.AddOrUpdate($"Route [{route.Id}] on [{weekday}] at [{time}]", () => l_Engine.Execute(route.Id), Cron.Weekly(Enum.Parse<DayOfWeek>(weekday), Convert.ToInt32(split[0]), Convert.ToInt32(split[1])));
                        }
                    }
                }
            }
            else if (route.FrequencyType == "Monthly")
            {
                foreach (string day in route.OnDay.Split(","))
                {
                    if (string.IsNullOrEmpty(route.ExecutionTime))
                    {
                        RecurringJob.AddOrUpdate($"Route [{route.Id}] on [{day}]", () => l_Engine.Execute(route.Id), Cron.Monthly(Convert.ToInt32(day)));
                    }
                    else
                    {
                        foreach (string time in route.ExecutionTime.Split(","))
                        {
                            string[] split = time.Split(":");
                            RecurringJob.AddOrUpdate($"Route [{route.Id}] on [{day}] at [{time}]", () => l_Engine.Execute(route.Id), Cron.Monthly(Convert.ToInt32(day), Convert.ToInt32(split[0]), Convert.ToInt32(split[1])));
                        }
                    }
                }
            }
        }

        public void RemoveRouteJob(Routes route)
        {
            if (route.FrequencyType == "Minutely")
            {
                RecurringJob.RemoveIfExists($"Route [{route.Id}]");
            }
            else if (route.FrequencyType == "Hourly")
            {
                RecurringJob.RemoveIfExists($"Route [{route.Id}]");
            }
            else if (route.FrequencyType == "Daily")
            {
                if (string.IsNullOrEmpty(route.ExecutionTime))
                {
                    RecurringJob.RemoveIfExists($"Route [{route.Id}]");
                }
                else
                {
                    foreach (string time in route.ExecutionTime.Split(","))
                    {
                        RecurringJob.RemoveIfExists($"Route [{route.Id}] at [{time}]");
                    }
                }
            }
            else if (route.FrequencyType == "Weekly")
            {
                foreach (string weekday in route.WeekDays.Split(","))
                {
                    if (string.IsNullOrEmpty(route.ExecutionTime))
                    {
                        RecurringJob.RemoveIfExists($"Route [{route.Id}] on [{weekday}]");
                    }
                    else
                    {
                        foreach (string time in route.ExecutionTime.Split(","))
                        {
                            RecurringJob.RemoveIfExists($"Route [{route.Id}] on [{weekday}] at [{time}]");
                        }
                    }
                }
            }
            else if (route.FrequencyType == "Monthly")
            {
                foreach (string day in route.OnDay.Split(","))
                {
                    if (string.IsNullOrEmpty(route.ExecutionTime))
                    {
                        RecurringJob.RemoveIfExists($"Route [{route.Id}] on [{day}]");
                    }
                    else
                    {
                        foreach (string time in route.ExecutionTime.Split(","))
                        {
                            RecurringJob.RemoveIfExists($"Route [{route.Id}] on [{day}] at [{time}]");
                        }
                    }
                }
            }
        }
    }
}
