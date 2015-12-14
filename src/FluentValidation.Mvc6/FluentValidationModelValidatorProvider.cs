using FluentValidation.Attributes;
using FluentValidation.Internal;
using FluentValidation.Mvc6.PropertyValidatorAdapters;
using FluentValidation.Validators;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentValidation.Mvc6
{
    public delegate IModelValidator FluentValidationModelValidationFactory(ModelMetadata metaData, PropertyRule rule, IPropertyValidator validator);

    public class FluentValidationModelValidatorProvider : IClientModelValidatorProvider, IModelValidatorProvider
    {


        #region Static Fields
        private Dictionary<Type, FluentValidationModelValidationFactory> validatorFactories = new Dictionary<Type, FluentValidationModelValidationFactory>() {
            { typeof(INotNullValidator), (metadata, rule, validator) => new RequiredFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(INotEmptyValidator), (metadata, rule, validator) => new RequiredFluentValidationPropertyValidator(metadata, rule, validator) },
            // email must come before regex.
            { typeof(IEmailValidator), (metadata, rule, validator) => new EmailFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(IRegularExpressionValidator), (metadata, rule, validator) => new RegularExpressionFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(ILengthValidator), (metadata, rule, validator) => new StringLengthFluentValidationPropertyValidator(metadata, rule, validator)},
            { typeof(InclusiveBetweenValidator), (metadata, rule, validator) => new RangeFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(GreaterThanOrEqualValidator), (metadata, rule, validator) => new MinFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(LessThanOrEqualValidator), (metadata, rule, validator) => new MaxFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(EqualValidator), (metadata, rule, validator) => new EqualToFluentValidationPropertyValidator(metadata, rule, validator) },
            { typeof(CreditCardValidator), (metadata, description, validator) => new CreditCardFluentValidationPropertyValidator(metadata, description, validator) }
        };
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the add implicated required validator
        /// </summary>
        public bool AddImplicitRequiredValidator { get; set; }

        /// <summary>
        /// gets or sets the validator factory
        /// </summary>
        public IValidatorFactory ValidatorFactory { get; set; }

        #endregion

        #region cTor
        /// <summary>
        /// Fluent validation model validator provider vNext
        /// </summary>
        /// <param name="validatorFactory"></param>
        public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory = null)
        {
            AddImplicitRequiredValidator = true;
            ValidatorFactory = validatorFactory ?? new AttributedValidatorFactory();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <param name="validatorType"></param>
        /// <param name="factory"></param>
        public void Add(Type validatorType, FluentValidationModelValidationFactory factory)
        {
            if (validatorType == null) throw new ArgumentNullException("validatorType");
            if (factory == null) throw new ArgumentNullException("factory");

            validatorFactories[validatorType] = factory;
        }


        /// <summary>
        /// Gets the validators (Server side)
        /// </summary>
        /// <param name="context"></param>
        public void GetValidators(ModelValidatorProviderContext context)
        {
            foreach (var validator in GetValidators(context.ModelMetadata))
                context.Validators.Add(validator);
        }

        /// <summary>
        /// Gets the validators (Client Side)
        /// </summary>
        /// <param name="context"></param>
        public void GetValidators(ClientValidatorProviderContext context)
        {
            foreach (var validator in GetValidators(context.ModelMetadata).OfType<IClientModelValidator>())
            {
                context.Validators.Add(validator);
            }
        }

        /// <summary>
        /// Gets the validators
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IModelValidator> GetValidators(ModelMetadata metadata)
        {
            IValidator validator = CreateValidator(metadata);
            List<IModelValidator> validators = new List<IModelValidator>();
            if (IsValidatingProperty(metadata))
            {
                var propertyValidators = GetValidatorsForProperty(metadata, validator);
                validators.AddRange(propertyValidators);
                return validators;
            }
            var modelValidators = GetValidatorsForModel(metadata, validator);
            validators.AddRange(modelValidators);
            return validators;
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Creates a new validator
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected virtual IValidator CreateValidator(ModelMetadata metadata)
        {
            if (IsValidatingProperty(metadata))
            {
                return ValidatorFactory.GetValidator(metadata.ContainerType);
            }
            return ValidatorFactory.GetValidator(metadata.ModelType);
        }

        /// <summary>
        /// Determines if fluent should be validating the property
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected virtual bool IsValidatingProperty(ModelMetadata metadata)
        {
            return IsValidatingProperty(metadata.ContainerType, metadata.PropertyName);
        }

        /// <summary>
        /// Determines if fluent should be validating the property
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected virtual bool IsValidatingProperty(Type containerType, string propertyName)
        {
            return containerType != null && !string.IsNullOrEmpty(propertyName);
        }
        #endregion

        #region Server Side Validation

        /// <summary>
        /// Gets the validators for a property
        /// </summary>
        /// <param name="modelValidationContext"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        IEnumerable<IModelValidator> GetValidatorsForProperty(ModelMetadata metadata, IValidator validator)
        {
            var modelValidators = new List<IModelValidator>();

            if (validator != null)
            {
                var descriptor = validator.CreateDescriptor();



                var validatorsWithRules = from rule in descriptor.GetRulesForMember(metadata.PropertyName)
                                          let propertyRule = (PropertyRule)rule
                                          let validators = rule.Validators
                                          where validators.Any()
                                          from propertyValidator in validators
                                          let modelValidatorForProperty = GetModelValidator(metadata, propertyRule, propertyValidator)
                                          where modelValidatorForProperty != null
                                          select modelValidatorForProperty;

                modelValidators.AddRange(validatorsWithRules);
            }

            if (validator != null && metadata.IsRequired && AddImplicitRequiredValidator)
            {
                bool hasRequiredValidators = modelValidators.Any(x => x.IsRequired);

                //If the model is 'Required' then we assume it must have a NotNullValidator. 
                //This is consistent with the behaviour of the DataAnnotationsModelValidatorProvider
                //which silently adds a RequiredAttribute

                if (!hasRequiredValidators)
                {
                    modelValidators.Add(CreateNotNullValidatorForProperty(metadata));
                }
            }

            return modelValidators;
        }

        /// <summary>
        /// Gets the validators for the model
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        IEnumerable<IModelValidator> GetValidatorsForModel(ModelMetadata metadata, IValidator validator)
        {
            if (validator != null)
            {
                yield return new FluentValidationModelValidator(metadata, validator);
            }
        }

        /// <summary>
        /// Gets the model validator
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="rule"></param>
        /// <param name="propertyValidator"></param>
        /// <returns></returns>
        private IModelValidator GetModelValidator(ModelMetadata metadata, PropertyRule rule, IPropertyValidator propertyValidator)
        {
            var type = propertyValidator.GetType();

            var factory = validatorFactories.Where(x => x.Key.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                                            .Select(x => x.Value)
                                            .FirstOrDefault() ?? ((m, r, p) => new FluentValidationPropertyValidator(m, r, p));

            return factory(metadata, rule, propertyValidator);
        }

        /// <summary>
        /// Creates a generic not null validator for a property
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        IModelValidator CreateNotNullValidatorForProperty(ModelMetadata metadata)
        {
            return new RequiredFluentValidationPropertyValidator(metadata, null, new NotNullValidator());
        }

        #endregion

    }
}