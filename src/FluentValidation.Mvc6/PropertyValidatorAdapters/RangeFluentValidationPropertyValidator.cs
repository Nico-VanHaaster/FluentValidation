
namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using Resources;
    using System.Collections.Generic;
    using Validators;


    internal class RangeFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public RangeFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) : base(metaData, rule, validator)
        {
            ShouldValidate = true;
        }

        InclusiveBetweenValidator RangeValidator
        {
            get { return (InclusiveBetweenValidator)Validator; }
        }

        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("From", RangeValidator.From)
                .AppendArgument("To", RangeValidator.To);

            string message = RangeValidator.ErrorMessageSource.GetString();

            if (RangeValidator.ErrorMessageSource.ResourceType == typeof(Messages))
            {
                // If we're using the default resources then the mesage for length errors will have two parts, eg:
                // '{PropertyName}' must be between {From} and {To}. You entered {Value}.
                // We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
                // Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

                message = message.Substring(0, message.IndexOf(".") + 1);
            }

            message = formatter.BuildMessage(message);

            yield return new ModelClientValidationRangeRule(message, RangeValidator.From, RangeValidator.To);
        }

        protected override bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return httpContext != null;
        }
    }
}
