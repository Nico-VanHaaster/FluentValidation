
namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using FluentValidation.Validators;
    using Internal;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using System.Collections.Generic;

    internal abstract class AbstractComparisonFluentValidationPropertyValidator<TValidator> : FluentValidationPropertyValidator
        where TValidator : AbstractComparisonValidator
    {
        public AbstractComparisonFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) :
            base(metaData, rule, validator)
        {
            ShouldValidate = false;
        }

        protected abstract object MinValue { get; }
        protected abstract object MaxValue { get; }

        /// <summary>
        /// Gets the abstract comparison validator
        /// </summary>
        protected TValidator AbstractComparisonValidator
        {
            get { return (TValidator)Validator; }
        }

        /// <summary>
        /// Gets the client validation rules
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            var formatter = new MessageFormatter()
               .AppendPropertyName(Rule.GetDisplayName())
               .AppendArgument("ComparisonValue", AbstractComparisonValidator.ValueToCompare);
            var message = formatter.BuildMessage(AbstractComparisonValidator.ErrorMessageSource.GetString());
            yield return new ModelClientValidationRangeRule(message, MinValue, MaxValue);
        }
    }
}
