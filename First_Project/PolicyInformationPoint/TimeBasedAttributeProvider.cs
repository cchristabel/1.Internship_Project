using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

public class TimeLimits
{
    [PolicyAttributeValue(PolicyAttributeCategories.Environment, "CurrentDay")]
    public string CurrentDay { get; set; }

    [PolicyAttributeValue(PolicyAttributeCategories.Environment, "CurrentTime")]
    public string currentTime { get; set; }
}

public class TimeBasedAttributeProvider : RecordAttributeValueProvider<TimeLimits>
{
    private static readonly PolicyAttribute Role =
             new PolicyAttribute("role",
                        PolicyValueType.String,
                        PolicyAttributeCategories.Subject);
    protected override Task<TimeLimits> GetRecordValue(IAttributeResolver attributeResolver, CancellationToken ct)
    {
        // Retrieve the current day and time in UTC format
        TimeZoneInfo swedishZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, swedishZone);

        // Get the current day as a string (e.g., "Monday")
        string currentDay = now.ToString("dddd");

        // Get the current time as a TimeSpan
        string currentTime = now.ToLocalTime().ToString("HH:mm:ss");

        // Print the current day and time for debugging (optional)
        Console.WriteLine($"Current Day: {currentDay}, Current Time: {currentTime}");

        return Task.FromResult(new TimeLimits
        {
            CurrentDay = currentDay,
            currentTime = currentTime
        });
    }

}
