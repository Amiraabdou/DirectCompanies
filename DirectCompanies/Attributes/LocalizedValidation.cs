using DirectCompanies.Localization;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using Microsoft.Extensions.Localization;
using System.Net.NetworkInformation;

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

        public class LocalizedRequiredIfSuspendedAttribute : ValidationAttribute
        {
            private readonly string LocalizationKey;

            public LocalizedRequiredIfSuspendedAttribute(string localizationKey)
            {
                LocalizationKey = localizationKey;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var instance = validationContext.ObjectInstance;
              
                var IsSuspendedProperty = instance.GetType().GetProperty("IsTemporarySuspension");
                var IsSuspended = IsSuspendedProperty != null && (bool)IsSuspendedProperty.GetValue(instance);


                if (IsSuspended && (value == null || string.IsNullOrEmpty(value.ToString())))
                {
                    var localizedErrorMessage = Localization.Localizer.GetString(LocalizationKey);
                    return new ValidationResult(localizedErrorMessage, new[] { validationContext.MemberName });

                    return new ValidationResult(string.Format(localizedErrorMessage, validationContext.DisplayName));
                }

                return ValidationResult.Success;
            }
        }
    }
}
