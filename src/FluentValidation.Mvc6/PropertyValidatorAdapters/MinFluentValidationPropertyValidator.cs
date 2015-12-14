

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using System.Collections.Generic;
    using Validators;

    internal class MinFluentValidationPropertyValidator : AbstractComparisonFluentValidationPropertyValidator<GreaterThanOrEqualValidator>
    {
        #region cTor
        /// <summary>
        /// Min Fluient validation property validator
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="propertyDescription"></param>
        /// <param name="validator"></param>
        public MinFluentValidationPropertyValidator(ModelMetadata metadata, PropertyRule propertyDescription, IPropertyValidator validator)
            : base(metadata, propertyDescription, validator)
        {
            ShouldValidate = false;
        }
        #endregion

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

        #region Methods
        /// <summary>
        /// Gets the client validation rules
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            var message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());

            var rule = new ModelClientValidationRule("range", message);
            rule.ValidationParameters.Add("min", MaxValue);
            yield return rule;
        }

        /// <summary>
        /// Should generate client side rules
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return httpContext != null;
        }
        #endregion
    }
}
