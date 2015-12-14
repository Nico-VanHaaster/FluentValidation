

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using System.Collections.Generic;
    using Validators;

    /// <summary>
    /// Regular expression fluent validation property validator
    /// </summary>
    internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        #region cTor
        /// <summary>
        /// regular expression fluent property validator
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="rule"></param>
        /// <param name="validator"></param>
        public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, PropertyRule rule, IPropertyValidator validator)
            : base(metadata, rule, validator)
        {
            ShouldValidate = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the regex core validator
        /// </summary>
        IRegularExpressionValidator RegexValidator
        {
            get { return (IRegularExpressionValidator)Validator; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get client validation rules
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            string message = formatter.BuildMessage(RegexValidator.ErrorMessageSource.GetString());
            yield return new ModelClientValidationRegexRule(message, RegexValidator.Expression);
        }

        /// <summary>
        /// Should validate client side rules
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
