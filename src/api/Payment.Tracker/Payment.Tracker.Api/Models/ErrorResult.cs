using System.Collections.Generic;
using Newtonsoft.Json;

namespace Payment.Tracker.Api.Models
{
    public class ErrorResult
    {
        private readonly IEnumerable<string> _messages;

        public ErrorResult(params string[] messages) => _messages = messages;

        public ErrorResult(IEnumerable<string> messages) => _messages = messages;

        public object ToJson() =>
            _messages == null
                ? null
                : new
                {
                    errors = _messages
                };

        public override string ToString() => JsonConvert.SerializeObject(ToJson());
    }
}