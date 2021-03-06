﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mhotivo.Implement.Utils
{
    public static class Extensions
    {
        public static string GetEnumDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }
            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }

        public static string ToAmPm(this TimeSpan timespan)
        {
            var timespan2 = new TimeSpan(timespan.Days, timespan.Hours > 12 ? timespan.Hours % 12 : timespan.Hours, timespan.Minutes, timespan.Seconds);
            var time = timespan2.ToString(@"hh\:mm");
            time += timespan.Hours >= 12 ? " PM" : " AM";
            return time;
        }
    }
}
