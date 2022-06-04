using Microsoft.AspNetCore.Components.Forms;

namespace QLESS.BlazorServerApp.Administrator.Shared.Components
{
    public class InputSelectNumber<T> : InputSelect<T>
    {
        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            if (typeof(T) == typeof(int))
            {
                return Validate(int.TryParse(value, out var resultInt), resultInt, out result, out validationErrorMessage);

            }
            else if (typeof(T) == typeof(long))
            {
                return Validate(long.TryParse(value, out var resultLong), resultLong, out result, out validationErrorMessage);
            }
            else
            {
                return base.TryParseValueFromString(value, out result, out validationErrorMessage);
            }
        }
        private bool Validate(bool tryParseValue, object tryParseResult, out T result, out string validationErrorMessage)
        {
            if (tryParseValue)
            {
                result = (T)tryParseResult;
                validationErrorMessage = null;
            }
            else
            {
                result = default;
                validationErrorMessage = $"The chosen value is not a valid {typeof(T).Name}.";
            }

            return tryParseValue;
        }
    }
}
