using DirectCompanies.Localization;
using System.ComponentModel.DataAnnotations;

namespace DirectCompanies.Attributes
{
    public class LocalizedValidation
    {
        public class CzRequiredAttribute : RequiredAttribute
        {

            public override string FormatErrorMessage(string name)
            {
                var localizedErrorMessage = Localizer.GetString("Required");
                return string.IsNullOrEmpty(localizedErrorMessage) ? base.FormatErrorMessage(name) : localizedErrorMessage;
            }

        }
        public class CzRegularExpressionAttribute : RegularExpressionAttribute
        {
            private readonly string LocalizationKey;

            public CzRegularExpressionAttribute(string pattern, string localizationKey)
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

        public class CzRequiredIfSuspendedAttribute : ValidationAttribute
        {
            private readonly string LocalizationKey;

            public CzRequiredIfSuspendedAttribute(string localizationKey)
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
                }

                return ValidationResult.Success;
            }
        }
    }
}
