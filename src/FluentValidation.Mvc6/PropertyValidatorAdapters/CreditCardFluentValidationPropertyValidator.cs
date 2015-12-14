

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    using Internal;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Validators;

    /// <summary>
    /// Credit card fluent validation property validator
    /// </summary>
    internal class CreditCardFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public CreditCardFluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator) 
            : base(metaData, rule, validator)
        {
            ShouldValidate = false;
        }

    }
}
