using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class MongoObjectIdAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            // Check if value is a valid MongoDB ObjectId
            return ObjectId.TryParse(value.ToString(), out _);
        }


        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? "Not Found";
        }
    }
}
