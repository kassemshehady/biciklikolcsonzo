using System;
using System.Globalization;
using System.Web.Mvc;

namespace Bicikli_Admin.CommonClasses
{
    /// <summary>
    /// Model binder for Doubles
    /// </summary>
    public class DoubleModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                actualValue = Convert.ToDouble(valueResult.AttemptedValue, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                modelState.Errors.Add("A következő mező nem valós számot tartalmaz: " + bindingContext.ModelMetadata.DisplayName);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }

    /// <summary>
    /// Model binder for Decimals
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                actualValue = Convert.ToDecimal(valueResult.AttemptedValue, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                modelState.Errors.Add("A következő mező nem valós számot tartalmaz: " + bindingContext.ModelMetadata.DisplayName);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }

    /// <summary>
    /// Universal Double model binder from StackOverflow.com
    /// </summary>
    public class CustomModelBinder : DefaultModelBinder
    {
        public CustomModelBinder()
            : base()
        {
        }

        public override object BindModel(ControllerContext controllerContext,
          ModelBindingContext bindingContext)
        {
            object result = null;
            if (bindingContext.ModelType == typeof(double))
            {

                string modelName = bindingContext.ModelName;
                string attemptedValue = bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

                // Depending on cultureinfo the NumberDecimalSeparator can be "," or "."
                // Both "." and "," should be accepted, but aren't.
                string wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                string alternateSeperator = (wantedSeperator == "," ? "." : ",");

                if (attemptedValue.IndexOf(wantedSeperator) == -1
                  && attemptedValue.IndexOf(alternateSeperator) != -1)
                {
                    attemptedValue = attemptedValue.Replace(alternateSeperator, wantedSeperator);
                }

                try
                {
                    result = double.Parse(attemptedValue, NumberStyles.Any);
                }
                catch (FormatException e)
                {
                    bindingContext.ModelState.AddModelError(modelName, e);
                }

            }
            else
            {
                result = base.BindModel(controllerContext, bindingContext);
            }

            return result;
        }
    }

}