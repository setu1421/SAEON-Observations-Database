using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
#if NET472
using System.Collections.Generic;
using System.Web.Mvc;
#endif

namespace SAEON.Observations.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
#if NET472
    public class IsBeforeDateAttribute : ValidationAttribute, IClientValidatable
#else
    public class IsBeforeDateAttribute : ValidationAttribute
#endif
    {
        private readonly string endDateProperty;
        private const string errorMessage = "{0} must be before {1}";

        public IsBeforeDateAttribute(string EndDateProperty) : base(errorMessage)
        {
            if (string.IsNullOrWhiteSpace(EndDateProperty)) throw new ArgumentNullException(nameof(EndDateProperty));
            endDateProperty = EndDateProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, endDateProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!(value is DateTime startDate)) throw new ArgumentException($"{validationContext.MemberName} is not DateTime");
                var property = validationContext.ObjectType.GetProperty(endDateProperty);
                if (property == null) throw new ArgumentException($"{endDateProperty} not found");
                var propertyValue = property.GetValue(validationContext.ObjectInstance);
                if (!(propertyValue is DateTime endDate)) throw new ArgumentException($"{endDateProperty} is not DateTime");
                var atts = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                var properyDisplayName = (atts?[0] as DisplayNameAttribute).DisplayName;
                if (startDate > endDate) return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName, properyDisplayName ?? endDateProperty));
            }
            return ValidationResult.Success;
        }


#if NET472
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            Type type = Type.GetType(metadata.ContainerType.FullName);
            var model = Activator.CreateInstance(type);
            var provider = new DataAnnotationsModelMetadataProvider();
            var endDateMetaData = provider.GetMetadataForProperty(() => model, type, endDateProperty);
            var endDateDisplayName = endDateMetaData.DisplayName;
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = string.Format(ErrorMessageString, metadata.GetDisplayName(), endDateDisplayName ?? endDateProperty),
                ValidationType = "isbeforedate"
            };
            rule.ValidationParameters.Add("enddate", endDateProperty);
            yield return rule;
        }
#endif
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
#if NET472
    public class IsAfterDateAttribute : ValidationAttribute, IClientValidatable
#else
    public class IsAfterDateAttribute : ValidationAttribute
#endif
    {
        private readonly string startDateProperty;
        private const string errorMessage = "{0} must be after {1}";

        public IsAfterDateAttribute(string StartDateName) : base(errorMessage)
        {
            if (string.IsNullOrWhiteSpace(StartDateName)) throw new ArgumentNullException(nameof(StartDateName));
            startDateProperty = StartDateName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, startDateProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!(value is DateTime endDate)) throw new ArgumentException($"{validationContext.MemberName} is not DateTime");
                var property = validationContext.ObjectType.GetProperty(startDateProperty);
                if (property == null) throw new ArgumentException($"{startDateProperty} not found");
                var propertyValue = property.GetValue(validationContext.ObjectInstance);
                if (!(propertyValue is DateTime startDate)) throw new ArgumentException($"{startDateProperty} is not DateTime");
                var atts = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                string properyDisplayName = (atts?[0] as DisplayNameAttribute).DisplayName;
                if (endDate < startDate) return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName, properyDisplayName ?? startDateProperty));
            }
            return ValidationResult.Success;
        }

#if NET472
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            Type type = Type.GetType(metadata.ContainerType.FullName);
            var model = Activator.CreateInstance(type);
            var provider = new DataAnnotationsModelMetadataProvider();
            var startDateMetaData = provider.GetMetadataForProperty(() => model, type, startDateProperty);
            var startDateDisplayName = startDateMetaData.DisplayName;
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = string.Format(ErrorMessageString, metadata.GetDisplayName(), startDateDisplayName ?? startDateProperty),
                ValidationType = "isafterdate"
            };
            rule.ValidationParameters.Add("startdate", startDateProperty);
            yield return rule;
        }
#endif
    }
}
