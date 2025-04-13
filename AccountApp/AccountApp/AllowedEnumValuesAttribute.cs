using System.ComponentModel.DataAnnotations;

namespace AccountApp
{
    public class AllowedEnumValuesAttribute : ValidationAttribute
    {
        private readonly int[] _allowedValues;

        public AllowedEnumValuesAttribute(params object[] allowedValues)
        {
            _allowedValues = allowedValues.Select(v => (int)v).ToArray();
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            var intValue = (int)value;
            return _allowedValues.Contains(intValue);
        }
    }
}