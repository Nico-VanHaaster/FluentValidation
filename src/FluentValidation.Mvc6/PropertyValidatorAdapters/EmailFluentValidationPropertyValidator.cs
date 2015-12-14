using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    internal class EmailFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        #region Properties
        /// <summary>
        /// Gets the email validator
        /// </summary>
        private IEmailValidator EmailValidator
        {
            get { return (IEmailValidator)Validator; }
        }
        #endregion

        public EmailFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) 
            : base(metaData, rule, validator)
        {
            ShouldValidate = false;
        }

        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            string message = formatter.BuildMessage(EmailValidator.ErrorMessageSource.GetString());

            yield return new ModelClientValidationRule("email", message);
        }

        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return httpContext != null;
        }

    }
}
