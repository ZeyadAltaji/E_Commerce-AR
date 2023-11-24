using System.Text.Json;

namespace E_CommerceAR.Errors
{
    public class ErrorsAPIs
    {
        // Global Var Error 
        public int Error_Code { get; set; }
        public string Error_Messages { get; set; }
        public string Errors_Details { get; set; }
        public ErrorsAPIs() { }

        public ErrorsAPIs(int Error_Code, string Error_Messages, string Errors_Details = null)
        {
            this.Errors_Details = Errors_Details;
            this.Error_Code = Error_Code;
            this.Error_Messages = Error_Messages;

        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
