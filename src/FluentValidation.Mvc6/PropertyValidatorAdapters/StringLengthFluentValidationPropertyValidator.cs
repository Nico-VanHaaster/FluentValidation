namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using Resources;
    using System.Collections.Generic;
    using Validators;

    public class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public StringLengthFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) 
            : base(metaData, rule, validator)
        {
            ShouldValidate = false;
        }

        private ILengthValidator LengthValidator
        {
            get { return (ILengthValidator)Validator; }
        }

        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return httpContext != null;
        }

        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {

            var formatter = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("MinLength", LengthValidator.Min)
                .AppendArgument("MaxLength", LengthValidator.Max);

            string message = LengthValidator.ErrorMessageSource.GetString();

            if (LengthValidator.ErrorMessageSource.ResourceType == PropertyValidatorHelpers.MessagesType)
            {
                // If we're using the default resources then the mesage for length errors will have two parts, eg:
                // '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
                // We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
                // Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.
                message = message.Substring(0, message.IndexOf(".") + 1);
            }

            message = formatter.BuildMessage(message);

            yield return new ModelClientValidationStringLengthRule(message, LengthValidator.Min, LengthValidator.Max);
        }
    }
}
