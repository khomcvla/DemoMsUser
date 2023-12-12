using System.Text.Json;

namespace DemoMsUser.Common.Responses
{
    public class ErrorDetails
    {
        public string Message { get; }
        public List<object> Errors { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public ErrorDetails(string message, List<Tuple<string, string>> errors)
        {
            Message = message;
            Errors = new List<object>(errors);
        }

        public ErrorDetails(string message, List<Tuple<string, string, string>> errors)
        {
            Message = message;
            Errors = new List<object>(errors);
        }

        public ErrorDetails(string message, List<int> errors)
        {
            Message = message;
            Errors = errors.ConvertAll(x => (object)x).ToList();
        }

        public ErrorDetails(string message, List<string> errors)
        {
            Message = message;
            Errors = new List<object>(errors);
        }

        public ErrorDetails(string message, params string[] errors)
        {
            Message = message;
            Errors = new List<object>(errors.ToList());
        }
    }
}
