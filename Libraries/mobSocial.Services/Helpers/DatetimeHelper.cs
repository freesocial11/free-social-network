using System;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;

namespace mobSocial.Services.Helpers
{
    public class DateTimeHelper
    {
        /// <summary>
        /// Gets a particular date in the target time zone
        /// </summary>
        /// <param name="sourceDate"></param>
        /// <param name="sourceTimeZoneInfo"></param>
        /// <param name="destinationTimeZoneInfo"></param>
        /// <returns></returns>
        public static DateTime GetDateInTimeZone(DateTime sourceDate, TimeZoneInfo sourceTimeZoneInfo, TimeZoneInfo destinationTimeZoneInfo)
        {
            return TimeZoneInfo.ConvertTime(sourceDate, sourceTimeZoneInfo, destinationTimeZoneInfo);
        }

        /// <summary>
        /// Gets a particular date in target timezone
        /// </summary>
        /// <param name="sourcedate"></param>
        /// <param name="destinationTimeZoneInfo"></param>
        /// <returns></returns>
        public static DateTime GetDateInTimeZone(DateTime sourcedate, TimeZoneInfo destinationTimeZoneInfo)
        {
            return TimeZoneInfo.ConvertTime(sourcedate, destinationTimeZoneInfo);
        }
        /// <summary>
        /// Gets a partiuclar date in UTC
        /// </summary>
        /// <param name="sourceDate"></param>
        /// <returns></returns>
        public static DateTime GetDateInUtc(DateTime sourceDate)
        {
            return sourceDate.ToUniversalTime();
        }

        public static DateTime GetDateInUserTimeZone(DateTime sourceDate, DateTimeKind sourceDateTimeKind, User user)
        {
            sourceDate = DateTime.SpecifyKind(sourceDate, sourceDateTimeKind);
            //get the timezone of mentioned user
            var userTimezoneId = user.GetPropertyValueAs<string>(PropertyNames.DefaultTimeZoneId);
            if (string.IsNullOrEmpty(userTimezoneId))
            {
                //get default timezone
                var generalSettings = mobSocialEngine.ActiveEngine.Resolve<GeneralSettings>();
                userTimezoneId = generalSettings.DefaultTimeZoneId;
            }
            //let's find the timezone
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userTimezoneId);
            }
            catch
            {
                //in case of error let's default to local timezone
                timeZoneInfo = TimeZoneInfo.Local;
            }
            //get the timezone
            return GetDateInTimeZone(sourceDate, timeZoneInfo);
        }
    }
}