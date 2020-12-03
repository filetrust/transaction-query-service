using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Store
{
    public static class TransactionAdaptionEventModelExtensions
    {
        public static TransactionAdaptionEventModel EventOrDefault(this IEnumerable<TransactionAdaptionEventModel> events, EventId eventId)
        {
            return events?.FirstOrDefault(f => int.TryParse(f.PropertyOrDefault("EventId"), out var id) && (EventId)id == eventId);
        }

        public static string PropertyOrDefault(this TransactionAdaptionEventModel @event, string key)
        {
            if (@event?.Properties == null) return null;
            return @event.Properties.TryGetValue(key, out var v) ? v : null;
        }

        public static TEnum? PropertyOrDefault<TEnum>(this TransactionAdaptionEventModel @event, string key) where TEnum : struct
        {
            if (@event?.Properties == null) return null;

            var couldGetValue = @event.Properties.TryGetValue(key, out var v);

            if (!couldGetValue)
                return null;

            if (!Enum.TryParse(v, true, out TEnum val))
                return null;

            return val;
        }
    }
}