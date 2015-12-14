using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        /// <summary>
        /// Required Fluent Validation Property Validator
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="rule"></param>
        /// <param name="validator"></param>
        public RequiredFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) :
            base(metaData, rule, validator)
        {
            bool isNonNullableValueType = !TypeAllowsNullValue(metaData.ModelType);
            // bool nullWasSpecified = model == null;


            ShouldValidate = true; //&& nullWasSpecified;
        }

        /// <summary>
        /// Client validation rules handler
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            var message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
            yield return new ModelClientValidationRule("required", message);
        }

        /// <summary>
        /// Should generate client side rules
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return true;
        }

        /// <summary>
        /// Gets the is required flag
        /// </summary>
        public override bool IsRequired
        {
            get { return true; }
        }

    }
}
