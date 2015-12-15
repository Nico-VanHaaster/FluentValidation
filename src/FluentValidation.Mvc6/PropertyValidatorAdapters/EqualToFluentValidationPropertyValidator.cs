namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using System.Collections.Generic;
    using System.Reflection;
    using Validators;

    //using System.ComponentModel.DataAnnotations;

    internal class EqualToFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public EqualToFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator)
            : base(metaData, rule, validator)
        {
            ShouldValidate = false;
        }

        EqualValidator EqualValidator
        {
            get { return (EqualValidator)Validator; }
        }

        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;
            if (propertyToCompare != null)
            {
                // If propertyToCompare is not null then we're comparing to another property.
                // If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
                // We only care about property comparisons in this case.

                var comparisonDisplayName =
                    ValidatorOptions.DisplayNameResolver(Rule.TypeToValidate, propertyToCompare, null)
                    ?? propertyToCompare.Name.SplitPascalCase();

                var formatter = new MessageFormatter()
                    .AppendPropertyName(Rule.GetDisplayName())
                    .AppendArgument("ComparisonValue", comparisonDisplayName);

                string message = formatter.BuildMessage(EqualValidator.ErrorMessageSource.GetString());

                var rule = new ModelClientValidationRule("equalto", message);
                rule.ValidationParameters.Add("other", string.Format("*.{0}", propertyToCompare.Name));

                yield return rule;
            }
        }

        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return httpContext != null;
        }


    }
}
