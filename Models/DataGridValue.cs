using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CentaureaTest.Models
{

    [Table("values")]
    public abstract class DataGridValue
    {
        [Key]
        public int Id { get; set; }

        public int FieldId { get; set; }

        public int RowIndex { get; set; }

        public DataGridValueType Type;

        public abstract (bool ok, string error) Validate(DataGridFieldSignature signature);

        protected static (bool ok, string error) ValidationOk()
        {
            return (true, string.Empty);
        }

        protected static (bool ok, string error) MismatchedTypeValidationError(DataGridValueType actualType, DataGridValueType expectedType)
        {
            return (false, $"{actualType} value does not match expected signature type ({expectedType})");
        }
    }

    public class DataGridNumericValue : DataGridValue
    {
        public decimal Value { get; set; }

        public DataGridNumericValue(Decimal value)
        {
            Value = value;
            Type = DataGridValueType.Numeric;
        }

        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            return DataGridValue.ValidationOk(); 
        }
    }

    public class DataGridStringValue : DataGridValue
    {
        public string Value { get; set; }
    
        public DataGridStringValue(string value)
        {
            Value = value;
            Type = DataGridValueType.String;
        }

        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            return signature.Type == DataGridValueType.String
                ? DataGridValue.ValidationOk()
                : DataGridValue.MismatchedTypeValidationError(Type, signature.Type);
        }
    }

    public class DataGridEmailValue : DataGridValue
    {
        public static readonly string EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public string Value { get; set; }

        public DataGridEmailValue(string value)
        {
            if (!Validate(value))
            {
                throw new Exception("DataGridEmailValue passed an invalid email");
            }
            
            Value = value;
            Type = DataGridValueType.Email;
        }

        /// <summary>
        /// Validate whether "value" is a valid email address
        /// </summary>
        static bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return Regex.IsMatch(value, EMAIL_REGEX);
        }

        /// <remarks>
        /// Email value with an invalid email cannot be constructed
        /// </remarks>
        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return DataGridValue.MismatchedTypeValidationError(Type, signature.Type);
            }

            // NB: Redundant
            if (!Validate(Value))
            {
                return (false, "Email value is not a valid email");
            }

            return DataGridValue.ValidationOk();
        }
    }

    public class DataGridRegexValue : DataGridValue
    {
        public string Value { get; set; }
    
        public DataGridRegexValue(string value)
        {
            Value = value;
            Type = DataGridValueType.Regex;
        }

        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return DataGridValue.MismatchedTypeValidationError(Type, signature.Type);
            }
            else
            {
                if (signature is not DataGridRegexFieldSignature regexSignature)
                {
                    // NB: Terrible case that should never happen
                    // Probably signifies that my design isn't right
                    throw new Exception("Maformed field signature: specified type is 'Regex', but the actual type is not");
                }
                else
                {
                    var ok = Regex.IsMatch(Value, regexSignature.RegexPattern);
                    return (ok, ok ? string.Empty : $"Regex value does not satisfy the regex ({regexSignature.RegexPattern})");
                }
            }
        }
    }

    public class DataGridRefValue : DataGridValue
    {
        public int ReferencedFieldId { get; set; }

        public DataGridRefValue(int referencedFieldId)
        {
            ReferencedFieldId = referencedFieldId;
            Type = DataGridValueType.Ref;
        }

        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return DataGridValue.MismatchedTypeValidationError(Type, signature.Type);
            }

            // TODO! Validate that the fieldId exists on the grid ?
            return (true, string.Empty);
        }
    }

    public class DataGridSingleSelectValue : DataGridValue
    {
        public int OptionId { get; set; }

        public DataGridSingleSelectValue(int optionId)
        {
            OptionId = optionId;
            Type = DataGridValueType.SingleSelect;
        }

        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return DataGridValue.MismatchedTypeValidationError(Type, signature.Type);
            }

            // TODO! Validate that the optionId exists in single select opnion table
            return (true, string.Empty);
        }
    }

    public class DataGridMultiSelectValue : DataGridValue
    {
        public List<int> OptionIds { get; set; }

        public DataGridMultiSelectValue(List<int> optionIds)
        {
            OptionIds = optionIds;
            Type = DataGridValueType.MultiSelect;
        }

        public override (bool ok, string error) Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return DataGridValue.MismatchedTypeValidationError(Type, signature.Type);
            }

            // TODO! Validate that the optionIds exist in multi select option table
            return (true, string.Empty);
        }
    }

}