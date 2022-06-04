using Microsoft.AspNetCore.Components.Forms;
using System;

namespace QLESS.BlazorServerApp.Administrator.Shared.Components
{
    public class InputSelectGuid<T> : InputSelect<T>
    {
        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            var isGuid = typeof(T) == typeof(Guid);
            var isNullable = Nullable.GetUnderlyingType(typeof(T)) != null;

            if (isGuid || isNullable)
            {
                if (Guid.TryParse(value, out var guid))
                {
                    result = (T)(object)guid;
                    validationErrorMessage = null;
                    return true;
                }
                else
                {
                    result = default;
                    validationErrorMessage = "The chosen value is not a valid GUID.";
                    return false;
                }
            }
            else
            {
                return base.TryParseValueFromString(value, out result, out validationErrorMessage);
            }
        }
    }
}
