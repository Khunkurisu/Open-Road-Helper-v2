using System.Drawing;
using System.Text.RegularExpressions;

namespace OpenRoadHelper
{
    public static class Generic
    {
        public static async Task<string> GetJson(string url)
        {
            HttpClient client = new();
            return await client.GetStringAsync(url);
        }

        public async static Task DownloadImage(string url, string filename)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(filename, FileMode.Create);
                await stream.CopyToAsync(fileStream);
                Console.WriteLine("Image downloaded successfully: " + filename);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Failed to download image: " + ex.Message);
            }
        }

        public static string ExpandSkillAbbreviation(string skillAbbreviation)
        {
            return skillAbbreviation switch
            {
                "acr" => "acrobatics",
                "arc" => "arcana",
                "ath" => "athletics",
                "cra" => "crafting",
                "dec" => "deception",
                "dip" => "diplomacy",
                "itm" => "intimidation",
                "med" => "medicine",
                "nat" => "nature",
                "occ" => "occultism",
                "prf" => "performance",
                "rel" => "religion",
                "soc" => "society",
                "ste" => "stealth",
                "sur" => "survival",
                "thievery" => "thievery",
                _ => skillAbbreviation,
            };
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
                suffix = ones switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th",
                };
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
                    height += FeetToCentimeters(resultFeet);
                }
                if (float.TryParse(heightImperial[1], out float resultInches))
                {
                    height += InchesToCentimeters(resultInches);
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

        public static string GuidToBase64(Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Replace("==", "");
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
                    weight += PoundsToKilograms(resultFeet);
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
