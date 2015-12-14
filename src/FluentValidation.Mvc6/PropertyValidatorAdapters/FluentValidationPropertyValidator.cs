using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    /// <summary>
    /// Basic fluent validation property validator
    /// </summary>
    public class FluentValidationPropertyValidator : IModelValidator, IClientModelValidator
    {
        #region cTor
        /// <summary>
        /// Base Fluent Property validator
        /// </summary>
        /// <param name="modelValidationContext"></param>
        /// <param name="rule"></param>
        /// <param name="validator"></param>
        public FluentValidationPropertyValidator(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator)
        {
            Validator = validator;

            // Build a new rule instead of the one passed in.
            // We do this as the rule passed in will not have the correct properties defined for standalone validation.
            // We also want to ensure we copy across the CustomPropertyName and RuleSet, if specified. 
            Rule = new PropertyRule(null, x => metaData.PropertyGetter, null, null, metaData.ModelType, null)
            {
                PropertyName = metaData.PropertyName,
                DisplayName = rule == null ? null : rule.DisplayName,
                RuleSet = rule == null ? null : rule.RuleSet
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the is required flag
        /// </summary>
        public virtual bool IsRequired { get { return false; } }


        /// <summary>
        /// Gets the Property validator
        /// </summary>
        public IPropertyValidator Validator { get; private set; }

        /// <summary>
        /// Gets the property rule to validate
        /// </summary>
        public PropertyRule Rule { get; private set; }

        /*
		 This might seem a bit strange, but we do *not* want to do any work in these validators.
		 They should only be used for metadata purposes.
		 This is so that the validation can be left to the actual FluentValidationModelValidator.
		 The exception to this is the Required validator - these *do* need to run standalone
		 in order to bypass MVC's "A value is required" message which cannot be turned off.
		 Basically, this is all just to bypass the bad design in ASP.NET MVC. Boo, hiss. 
		*/
        protected bool ShouldValidate { get; set; }

        
        #endregion

        #region Methods
        /// <summary>
        /// Gets the client validation rules
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context)
        {
            var x = context.RequestServices.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            if (x == null || x.HttpContext == null || !ShouldGenerateClientSideRules(context, x.HttpContext))
                return Enumerable.Empty<ModelClientValidationRule>();

            return GetClientValidationRules(context, x.HttpContext);

        }

        /// <summary>
        /// Gets the client validation rules
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return Enumerable.Empty<ModelClientValidationRule>();
        }

        /// <summary>
        /// Validates the model (Server side)
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext validationContext)
        {
            if (ShouldValidate)
            {
                var fakeRule = new PropertyRule(null, x => validationContext.Model, null, null, validationContext.Metadata.ModelType, null)
                {
                    PropertyName = validationContext.Metadata.PropertyName,
                    DisplayName = Rule == null ? null : Rule.DisplayName,
                };

                var fakeParentContext = new ValidationContext(validationContext.Container);
                var context = new PropertyValidatorContext(fakeParentContext, fakeRule, validationContext.Metadata.PropertyName);
                var result = Validator.Validate(context);

                foreach (var failure in result)
                {
                    yield return new ModelValidationResult(validationContext.Metadata.PropertyName, failure.ErrorMessage);
                }
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method called before client side rules are generated.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldGenerateClientSideRules(ClientModelValidationContext context, HttpContext httpContext)
        {
            return false;
        }

        /// <summary>
        /// Helper to determine if type allows for null values
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool TypeAllowsNullValue(Type type)
        {
            return (!type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null);
        }
        #endregion
    }
}
