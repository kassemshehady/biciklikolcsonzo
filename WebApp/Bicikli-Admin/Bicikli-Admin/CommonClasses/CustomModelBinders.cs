using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bicikli_Admin.CommonClasses
{
    class DoubleModelBinder : IModelBinder
    {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string numStr = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;
            double res;

            if (!double.TryParse(numStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out res))
            {
                if (bindingContext.ModelType == typeof(double?))
                    return null;
                throw new ArgumentException();
            }

            if (bindingContext.ModelType == typeof(double?))
                return new Nullable<double>(res);
            else
                return res;
        }

        #endregion
    }

    class FloatModelBinder : IModelBinder
    {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string numStr = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;
            float res;

            if (!float.TryParse(numStr, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out res))
            {
                if (bindingContext.ModelType == typeof(float?))
                    return null;
                throw new ArgumentException();
            }

            if (bindingContext.ModelType == typeof(float?))
                return new Nullable<float>(res);
            else
                return res;
        }

        #endregion
    }
}