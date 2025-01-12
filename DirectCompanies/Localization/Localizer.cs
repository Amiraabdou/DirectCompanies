using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text.Json;

namespace DirectCompanies.Localization
{
    public class Localizer
    {
        private static List<LocalizationRecord>? _dic;
        public static IReadOnlyList<LocalizationRecord> Dictionary => _dic;

        public static async Task LoadStrings(string url)
        {
            try
            {
                var response = await new HttpClient().GetAsync(url);
                response.EnsureSuccessStatusCode();
                string? result;
                await using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(responseStream))
                {
                    result = await streamReader.ReadToEndAsync();
                }

                _dic = JsonSerializer.Deserialize<List<LocalizationRecord>>(result);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static void LoadStrings(List<LocalizationRecord> localizationList)
        {
            _dic = localizationList;
        }

        public static MarkupString GetUi(string name, string? defaultString = null, params object?[] arguments)
        {
            return new MarkupString(GetString(name, arguments));
        }

        public static string GetString(string name, params object?[] arguments)
        {
            var rec = _dic?.FirstOrDefault(c => c.Key?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
            if (rec != null)
            {
                var text = IsArabic() ? rec.Arabic : rec.English;
                if (text != null)
                {
                    var value = arguments?.Any() == true ? string.Format(text, arguments) : text;
                    return value;
                }
            }

            return $"[{name}]";
        }

        public static bool IsArabic()
        {
            return CultureInfo.CurrentCulture.Name == "ar-EG";
        }

        public static string GetLocalizedProperty(string propertyPerfix)
        {
            return $"{propertyPerfix}{(IsArabic() ? "Ar" : "En")}";
        }
        public static string GetLocalizedProperty()
        {
            return $"{(IsArabic() ? "Ar" : "En")}";
        }
        public class LocalizationRecord
        {
            public string? Key { get; set; }
            public string? Arabic { get; set; }
            public string? English { get; set; }
        }
    }
}
