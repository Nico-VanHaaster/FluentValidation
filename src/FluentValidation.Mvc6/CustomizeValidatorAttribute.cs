

namespace FluentValidation.Mvc6
{
    //using System;
    //using System.Collections.Generic;
    //using System.Reflection;
    //using System.Web.Mvc;
    //using Internal;
    //using Microsoft.AspNet.Mvc.ModelBinding;
    //using Microsoft.AspNet.Mvc;
    //using System.Threading.Tasks;

    //public class CustomizeValidatorAttribute : CustomModelBinderAttribute, IModelBinder
    //{
    //    public string RuleSet { get; set; }
    //    public string Properties { get; set; }
    //    public Type Interceptor { get; set; }

    //    private const string key = "_FV_CustomizeValidator";

    //    public IModelBinder GetBinder()
    //    {
    //        return this;
    //    }

    //    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        // Originally I thought about storing this inside ModelMetadata.AdditionalValues.
    //        // Unfortunately, DefaultModelBinder overwrites this property internally.
    //        // So anything added to AdditionalValues will not be passed to the ValidatorProvider.
    //        // This is a very poor design decision. 
    //        // The only piece of information that is passed all the way down to the validator is the controller context.
    //        // So we resort to storing the attribute in HttpContext.Items. 
    //        // Horrible, horrible, horrible hack. Horrible.
    //        controllerContext.HttpContext.Items[key] = this;

    //        var innerBinder = ModelBinders.Binders.GetBinder(bindingContext.ModelType);
    //        var result = innerBinder.BindModel(controllerContext, bindingContext);

    //        controllerContext.HttpContext.Items.Remove(key);

    //        return result;
    //    }

    //    public static CustomizeValidatorAttribute GetFromControllerContext(ActionContext context)
    //    {
    //        return context.HttpContext.Items[key] as CustomizeValidatorAttribute;
    //    }

    //    /// <summary>
    //    /// Builds a validator selector from the options specified in the attribute's properties.
    //    /// </summary>
    //    public IValidatorSelector ToValidatorSelector()
    //    {
    //        IValidatorSelector selector;

    //        if (!string.IsNullOrEmpty(RuleSet))
    //        {
    //            var rulesets = RuleSet.Split(',', ';');
    //            selector = new RulesetValidatorSelector(rulesets);
    //        }
    //        else if (!string.IsNullOrEmpty(Properties))
    //        {
    //            var properties = Properties.Split(',', ';');
    //            selector = new MemberNameValidatorSelector(properties);
    //        }
    //        else
    //        {
    //            selector = new DefaultValidatorSelector();
    //        }

    //        return selector;

    //    }

    //    public IValidatorInterceptor GetInterceptor()
    //    {
    //        if (Interceptor == null) return null;

    //        if (!typeof(IValidatorInterceptor).IsAssignableFrom(Interceptor))
    //        {
    //            throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
    //        }

    //        var instance = Activator.CreateInstance(Interceptor) as IValidatorInterceptor;

    //        if (instance == null)
    //        {
    //            throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
    //        }

    //        return instance;
    //    }

       
    //}

}
