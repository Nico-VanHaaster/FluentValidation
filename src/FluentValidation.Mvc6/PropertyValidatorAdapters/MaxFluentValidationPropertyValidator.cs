
namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using System.Collections.Generic;
    using FluentValidation.Validators;
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;

    internal class MaxFluentValidationPropertyValidator : AbstractComparisonFluentValidationPropertyValidator<LessThanOrEqualValidator>
    {
        public MaxFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) 
            : base(metaData, rule, validator)
        {
            ShouldValidate = false;
        }

        #region Properties
        /// <summary>
        /// Gets the minimum value
        /// </summary>
        protected override object MinValue
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the maximum value
        /// </summary>
        protected override object MaxValue
        {
            get { return AbstractComparisonValidator.ValueToCompare; }
        }
        #endregion

        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            var message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());

            var rule = new ModelClientValidationRule("range", message);
            rule.ValidationParameters.Add("max", MaxValue);
            yield return rule;
        }

        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return httpContext != null;
        }
    }
}
