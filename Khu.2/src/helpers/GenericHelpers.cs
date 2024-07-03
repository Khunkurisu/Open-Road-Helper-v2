using System.Drawing;
using System.Text.RegularExpressions;

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

        public static float GetHeightFromString(string heightString)
        {
            float height = 0f;
            string patternImperial = " ?\"| ?'";
            string patternMetric = " ?cms?";

            Regex regex = new(patternImperial);
            string[] heightImperial = regex.Split(heightString);
            if (regex.IsMatch(patternImperial) && heightImperial.Length > 1)
            {
                if (float.TryParse(heightImperial[0], out float resultFeet))
                {
                    height += GenericHelpers.FeetToCentimeters(resultFeet);
                }
                if (float.TryParse(heightImperial[1], out float resultInches))
                {
                    height += GenericHelpers.InchesToCentimeters(resultInches);
                }
                return height;
            }

            regex = new(patternMetric);
            string[] heightMetric = regex.Split(heightString);
            if (regex.IsMatch(patternMetric) && heightMetric.Length > 0)
            {
                if (float.TryParse(heightMetric[0], out float result))
                {
                    height += result;
                    return height;
                }
            }

            return height;
        }

        public static int[] HexStringToRGB(string hexColor)
        {
            Color color = ColorTranslator.FromHtml(hexColor);
            return new int[]
            {
                Convert.ToInt16(color.R),
                Convert.ToInt16(color.G),
                Convert.ToInt16(color.B)
            };
        }

        public static float GetWeightFromString(string weightString)
        {
            float weight = 0f;
            string patternImperial = " ?lbs?";
            string patternMetric = " ?kgs?";

            Regex regex = new(patternImperial);
            string[] weightImperial = regex.Split(weightString);
            if (regex.IsMatch(patternImperial) && weightImperial.Length > 1)
            {
                if (float.TryParse(weightImperial[0], out float resultFeet))
                {
                    weight += GenericHelpers.PoundsToKilograms(resultFeet);
                    return weight;
                }
            }

            regex = new(patternMetric);
            string[] weightMetric = regex.Split(weightString);
            if (regex.IsMatch(patternMetric) && weightMetric.Length > 0)
            {
                if (float.TryParse(weightMetric[0], out float result))
                {
                    weight += result;
                    return weight;
                }
            }
            return weight;
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

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                ""
                    => throw new ArgumentException(
                        $"{nameof(input)} cannot be empty",
                        nameof(input)
                    ),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };
    }
}
