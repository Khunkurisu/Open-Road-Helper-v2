namespace Bot.Helpers
{
    public static class GenericHelpers
    {
        public static async Task<string> GetJson(string url)
        {
            HttpClient client = new();
            Console.WriteLine(url);
            return await client.GetStringAsync(url);
        }

        public static string ExpandSkillAbbreviation(string skillAbbreviation)
        {
            switch (skillAbbreviation)
            {
                case "acr":
                    return "acrobatics";
                case "arc":
                    return "arcana";
                case "ath":
                    return "athletics";
                case "cra":
                    return "crafting";
                case "dec":
                    return "deception";
                case "dip":
                    return "diplomacy";
                case "itm":
                    return "intimidation";
                case "med":
                    return "medicine";
                case "nat":
                    return "nature";
                case "occ":
                    return "occultism";
                case "prf":
                    return "performance";
                case "rel":
                    return "religion";
                case "soc":
                    return "society";
                case "ste":
                    return "stealth";
                case "sur":
                    return "survival";
                case "thievery":
                    return "thievery";
                default:
                    return "";
            }
        }

        public static string Ordinal(float num)
        {
            float ones = num % 10;
            float tens = MathF.Floor(num / 10) % 10;
            string suffix;
            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;
                    case 2:
                        suffix = "nd";
                        break;
                    case 3:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
            }
            return $"{num}{suffix}";
        }

        public static DateTime DateTimeFromUnixSeconds(uint unixSeconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixSeconds);
        }

        public static float FeetToCentimeters(float feet)
        {
            return 30.48f * feet;
        }

        public static float InchesToCentimeters(float inches)
        {
            return 2.54f * inches;
        }

        public static float PoundsToKilograms(float lbs)
        {
            return 0.45359237f * lbs;
        }
    }
}
