using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Veterinarian.Domain.ValueObjects
{
    public record Text
    {
        public string Value { get; set; }

        public Text(string value)
        {
            Validate(value);
            Value = value;
        }
        private void Validate(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Text cannot be empty");
            }
            if (value.Length > 400)
            {
                throw new ArgumentException("Text cannot be longer than 400 characters");
            }
        }
        public static implicit operator Text(string value) { return new Text(value); }
    }
}
