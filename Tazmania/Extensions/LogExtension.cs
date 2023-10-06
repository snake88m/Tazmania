using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Extensions
{
    public static class LogExtensions
    {
        public static void LogRequest<T>(this ILogger<T> logger, Request request)
        {
            if (request.Type == RequestType.AutomationSetOutput)
            {
                logger.LogInformation($"[REQ] set-output-{request.EntityId} => {request.IsActive.AsString()}");
            }

            if (request.Type == RequestType.HeatingSetTemperature)
            {
                logger.LogInformation($"[REQ] set-temperature-{request.EntityId} => {request.Value}");
            }

            if (request.Type == RequestType.HeatingSetMode)
            {
                logger.LogInformation($"[REQ] set-heating => {request.IsActive.AsString()}");
            }

            if (request.Type == RequestType.SecurityActiveAntiFire)
            {
                logger.LogInformation($"[REQ] set-active-anti-fire");
            }

            if (request.Type == RequestType.SecurityActiveSOS)
            {
                logger.LogInformation($"[REQ] set-active-sos");
            }

            if (request.Type == RequestType.SecurityDeactiveAlarms)
            {
                logger.LogInformation($"[REQ] set-deactive-alarms");
            }

            if (request.Type == RequestType.SecurityNotifyCallSet)
            {
                logger.LogInformation($"[REQ] set-call => {request.IsActive.AsString()}");
            }

            if (request.Type == RequestType.SecurityNotifySirenSet)
            {
                logger.LogInformation($"[REQ] set-siren => {request.IsActive.AsString()}");
            }
        }

        public static void LogHeating<T>(this ILogger<T> logger, bool isActive, Heating heating, HeatingSetting setting, IO? sensor = null)
        {
            var detail = sensor != null
                         ? $" | Temp.sensor: {sensor.ValueCorrected} | Temp.target: {heating.Temperature + setting.Offset}"
                         : string.Empty;

            logger.LogInformation($"[HEAT] set-{heating.Description} => {isActive.AsString()}{detail}");
        }

        public static void LogIrrigation<T>(this ILogger<T> logger, bool isActive, Irrigation irrigation)
        {
            logger.LogInformation($"[IRRIGATION] set-{irrigation.Description} => {isActive.AsString()}");
        }
    }
}
