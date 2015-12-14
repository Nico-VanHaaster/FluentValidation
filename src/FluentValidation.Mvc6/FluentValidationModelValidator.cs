

namespace FluentValidation.Mvc6
{
    using Internal;
    using Results;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.AspNet.Mvc.ModelBinding.Validation;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
	/// ModelValidator implementation that uses FluentValidation.
	/// </summary>
	internal class FluentValidationModelValidator : IModelValidator
    {
        readonly IValidator validator;
        //readonly CustomizeValidatorAttribute customizations;

        /// <summary>
        /// Get the is required flag
        /// </summary>
        public bool IsRequired
        {
            get { return false; }
        }

        public FluentValidationModelValidator(ModelMetadata metadata, IValidator validator)
        {
            this.validator = validator;
            //customizations = null;// CustomizeValidatorAttribute.GetFromControllerContext(actionContext) ?? new CustomizeValidatorAttribute();
        }

        public virtual IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if (context.Model != null)
            {
                // var selector = customizations.ToValidatorSelector();
                //var interceptor = customizations.GetInterceptor() ?? (validator as IValidatorInterceptor);
                var validationContext = new ValidationContext(context.Model, new PropertyChain(), null); // selector);

                //if (interceptor != null)
                //{
                //    // Allow the user to provide a customized context
                //    // However, if they return null then just use the original context.
                //    var newContext = interceptor.BeforeMvcValidation(actionContext, validationContext);
                //    if (newContext != null)
                //        validationContext = newContext;
                //}

                var result = validator.Validate(context.Model);

                //if (interceptor != null)
                //{
                //    // allow the user to provice a custom collection of failures, which could be empty.
                //    // However, if they return null then use the original collection of failures. 
                //    result = interceptor.AfterMvcValidation(actionContext, validationContext, result) ?? result;
                //}

                if (!result.IsValid)
                {
                    return ConvertValidationResultToModelValidationResults(result);
                }
            }
            return Enumerable.Empty<ModelValidationResult>();
        }

        protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result)
        {
            return result.Errors.Select(x => new ModelValidationResult(x.PropertyName, x.ErrorMessage));
        }
    }
}
