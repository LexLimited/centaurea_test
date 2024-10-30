using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CentaureaTest.Models
{

    public record DataGridValueValidationResult(bool Ok, string Error);

    [Table("values")]
    public abstract class DataGridValue
    {
        [Key]
        public int Id { get; set; }

        public int FieldId { get; set; }

        public int RowIndex { get; set; }

        [NotMapped]
        public DataGridValueType Type { get; set; }

        /// <remarks>Returns validation result, does not throw!!!</remarks>
        public abstract DataGridValueValidationResult Validate(DataGridFieldSignature signature);

        public override string ToString()
        {
            return $"Type: {Type}, Id: {Id}, FieldId: {FieldId}, RowIndex: {RowIndex}";
        }

        protected static DataGridValueValidationResult ValidationOk()
        {
            return new DataGridValueValidationResult(true, string.Empty);
        }

        protected static DataGridValueValidationResult ValidationError(string message)
        {
            return new DataGridValueValidationResult(false, message);
        }

        protected static DataGridValueValidationResult MismatchedTypesValidationError(DataGridValueType actualType, DataGridValueType expectedType)
        {
            return new DataGridValueValidationResult(false, $"{actualType} value does not match expected signature type ({expectedType})");
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

        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
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

        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
        {
            return signature.Type == DataGridValueType.String
                ? DataGridValue.ValidationOk()
                : DataGridValue.MismatchedTypesValidationError(Type, signature.Type);
        }
    }

    public class DataGridEmailValue : DataGridValue
    {
        public static readonly string EMAIL_REGEX = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

        public string Value { get; set; }

        public DataGridEmailValue(string value)
        {
            // if (!Validate(value))
            // {
            //     throw new Exception("DataGridEmailValue passed an invalid email");
            // }
            
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
        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return MismatchedTypesValidationError(Type, signature.Type);
            }

            // NB! Redundant
            if (!Validate(Value))
            {
                return ValidationError("Email value is not a valid email");
            }

            return ValidationOk();
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

        public override string ToString()
        {
            return $"{base.ToString()}, value: {Value}";
        }

        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return MismatchedTypesValidationError(Type, signature.Type);
            }
            else
            {
                if (signature is not DataGridRegexFieldSignature regexSignature)
                {
                    // NB! Terrible case that should never happen
                    // Probably signifies that my design isn't right
                    throw new Exception("Maformed field signature: specified type is 'Regex', but the actual type is not");
                }
                else
                {
                    return Regex.IsMatch(Value, regexSignature.RegexPattern)
                        ? ValidationOk()
                        : ValidationError($"Regex value does not satisfy the regex ({regexSignature.RegexPattern})");
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

        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return MismatchedTypesValidationError(Type, signature.Type);
            }

            // TODO! Validate that the fieldId exists on the grid ?
            return ValidationOk();
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

        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return MismatchedTypesValidationError(Type, signature.Type);
            }

            // TODO! Validate that the optionId exists in single select opnion table
            return ValidationOk();
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

        public override DataGridValueValidationResult Validate(DataGridFieldSignature signature)
        {
            if (Type != signature.Type)
            {
                return MismatchedTypesValidationError(Type, signature.Type);
            }

            // TODO! Validate that the optionIds exist in multi select option table
            return ValidationOk();
        }
    }

}