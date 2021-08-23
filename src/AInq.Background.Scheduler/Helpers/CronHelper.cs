// Copyright 2020-2021 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Cronos;
using System;

namespace AInq.Background.Helpers
{

/// <summary> Cron expression parsing utility </summary>
public static class CronHelper
{
    /// <summary> Parse cron string with format auto detection </summary>
    /// <param name="cronExpression"> Cron string </param>
    /// <returns> <see cref="CronExpression" /> instance </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static CronExpression ParseCron(this string cronExpression)
    {
        try
        {
            return cronExpression.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length switch
            {
                5 => CronExpression.Parse(cronExpression, CronFormat.Standard),
                6 => CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds),
                _ => throw new CronFormatException("Unknown format")
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Syntax error in cron expression", ex);
        }
    }
}

}
