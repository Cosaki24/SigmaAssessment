using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SigmaAssessment.Models
{
    [SwaggerSchemaFilter(typeof(CandidateSchemaFilter))]
    public class Candidate
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^\+255[67]\d{2}\d{3}\d{3}$", ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Invalid time format. Time should be in 24-hour format (HH:mm)")]
        public string? AvailableStartTime { get; set; }

        [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Invalid time format. Time should be in 24-hour format (HH:mm)")]
        public string? AvailableEndTime { get; set; }

        [RegularExpression(@"^https:\/\/(www\.)?linkedin\.com\/.*$", ErrorMessage = "The LinkedIn profile URL is not valid.")]
        public string? LinkedInProfileUrl { get; set; }

        [RegularExpression(@"^https:\/\/(www\.)?github\.com\/.*$", ErrorMessage = "The GitHub profile URL is not valid.")]
        public string? GithubProfileUrl { get; set; }

        [Required(ErrorMessage = "The comment field is required")]
        public string Comment { get; set; }
    }

    public class CandidateSchemaFilter : ISchemaFilter 
    {
        /// <summary>
        /// Provides an example for the Candidate Model in the Swagger UI
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="ctx"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext ctx)
        {
            schema.Example = new OpenApiObject
            {
                ["FirstName"] = new OpenApiString("John"),
                ["LastName"] = new OpenApiString("Doe"),
                ["PhoneNumber"] = new OpenApiString("+255712345678"),
                ["Email"] = new OpenApiString("candidate@example.com"),
                ["AvailableStartTime"] = new OpenApiString("08:00"),
                ["AvailableEndTime"] = new OpenApiString("17:00"),
                ["LinkedInProfileUrl"] = new OpenApiString("https://www.linkedin.com/in/johndoe"),
                ["GithubProfileUrl"] = new OpenApiString("https://www.github.com/johndoe"),
                ["Comment"] = new OpenApiString("Write your comment here")
            };
        }
    }
}
