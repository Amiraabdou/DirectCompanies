using DirectCompanies.Localization;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;

namespace DirectCompanies.Attributes
{
    public class LocalizedValidation
    {
        public class LocalizedRequiredAttribute : RequiredAttribute
        {
            private readonly string LocalizationKey;

            public LocalizedRequiredAttribute(string localizationKey)
            {
                LocalizationKey = localizationKey;
            }

            public override string FormatErrorMessage(string name)
            {
                var localizedErrorMessage = Localizer.GetString(LocalizationKey);
                return string.IsNullOrEmpty(localizedErrorMessage) ? base.FormatErrorMessage(name) : localizedErrorMessage;
            }

        }
        public class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
        {
            private readonly string LocalizationKey;

            public LocalizedRegularExpressionAttribute(string pattern, string localizationKey)
                : base(pattern)
            {
                LocalizationKey = localizationKey;
            }

            public override string FormatErrorMessage(string name)
            {
                var localizedErrorMessage = Localization.Localizer.GetString(LocalizationKey);
                return string.Format(localizedErrorMessage, name);
            }
        }
    }
}
