using System;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation.Mvc6
{
    /// <summary>
    /// Static extensions class for the fluent validation model validator provider
    /// </summary>
    public static class FluentValidationModelValidatorProviderExtensions
    {
        /// <summary>
        /// Adds the fluent validation options to the MvcBuilder pipline.
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="configurationExpression"></param>
        /// <returns></returns>
        public static IMvcBuilder AddFluentValidation(this IMvcBuilder mvcBuilder, Action<FluentValidationModelValidatorProvider> configurationExpression = null)
        {
            configurationExpression = configurationExpression ?? delegate { };

            var provider = new FluentValidationModelValidatorProvider();
            configurationExpression(provider);

            mvcBuilder.AddDataAnnotationsLocalization();
            mvcBuilder.AddViewLocalization();

            //register the mvc optios
            mvcBuilder.AddMvcOptions(o =>
            {
                o.ModelValidatorProviders.Add(provider);
                
            });

            //register the mvc view options
            mvcBuilder.AddViewOptions(o =>
            {
                o.ClientModelValidatorProviders.Add(provider);
            });

            return mvcBuilder;
        }
    }
}
