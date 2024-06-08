using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SigmaAssessment.Controllers;
using SigmaAssessment.Helpers;
using SigmaAssessment.Models;
using System.Text.RegularExpressions;

namespace SigmaAssessmentTests.Controllers
{
    [TestFixture]
    public class CandidateControllerTests : ControllerBase
    {
        private CandidateDbCtx _context;
        private ILogger<CandidatesController> _logger;
        private CandidatesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CandidateDbCtx>()
                .UseInMemoryDatabase(databaseName: "CandidatesDb")
                .Options;

            _context = new CandidateDbCtx(options);
            _logger = A.Fake<ILogger<CandidatesController>>();
            _controller = new CandidatesController(_context, _logger);
        }

        [Test]
        /// <summary>
        /// Tests for successful creation of a new candidate. All input fields are valid.
        /// </summary>
        public async Task PostCandidate_AddNewCandidate_ReturnsNewCandidate()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+255712345678",
                Email = "jdoe@example.com",
                AvailableStartTime = "08:00",
                AvailableEndTime = "17:00",
                LinkedInProfileUrl = "https://www.linkedin.com/in/johndoe",
                GithubProfileUrl = "https://www.github.com/johndoe",
                Comment = "Good guy"
            };

            // Act
            var result = await _controller.PostCandidate(candidate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as Response;
            response.Should().NotBeNull();
            response.Data.Should().BeOfType<Candidate>();
            response.StatusCode.Should().Be(200);
            response.Message.Should().Be("New candidate created successfully.");
            response.Data.Should().BeEquivalentTo(candidate);
        }

        [Test]
        /// <summary>
        /// Tests for successful update of an existing candidate. All input fields are valid.
        /// Same email as existing candidate is used.
        /// </summary>
        public async Task PostCandidate_UpdateExistingCandidate_ReturnsUpdatedCandidate()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "David",
                PhoneNumber = "+255712345678",
                Email = "jdoe@example.com",
                AvailableStartTime = "08:00",
                AvailableEndTime = "17:00",
                LinkedInProfileUrl = "https://www.linkedin.com/in/johndoe",
                GithubProfileUrl = "https://www.github.com/johndoe",
                Comment = "Good guy"
            };

            // Act
            var result = await _controller.PostCandidate(candidate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as Response;
            response.Should().NotBeNull();
            response.Data.Should().BeOfType<Candidate>();
            response.StatusCode.Should().Be(200);
            response.Message.Should().Be("Candidate updated successfully.");
            response.Data.Should().BeEquivalentTo(candidate);
        }

        [Test]
        /// <summary>
        /// Tests when trying to post data with required values missing.
        /// All required values (5) are missing. Five errors should be returned.
        /// </summary>
        public async Task PostCandidate_MissingRequiredValues_ReturnsBadRequest()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "",
                LastName = "",
                PhoneNumber = null,
                Email = "",
                AvailableStartTime = "",
                AvailableEndTime = "",
                LinkedInProfileUrl = "",
                GithubProfileUrl = "",
                Comment = ""
            };

            _controller.ModelState.AddModelError("FirstName", "First Name is required");
            _controller.ModelState.AddModelError("LastName", "Last Name is required");
            _controller.ModelState.AddModelError("Email", "Email is required");
            _controller.ModelState.AddModelError("Comment", "The comment field is required");

            // Act
            var result = await _controller.PostCandidate(candidate);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            var response = badRequestResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(400);
            response.Message.Should().NotBeNullOrEmpty();
            response.Message.Should().Be("Invalid candidate information");
            response.Errors.Should().NotBeNull();
            response.Errors.Should().HaveCount(4);
        }

        [Test]
        /// <summary>
        /// Tests when trying to post new candidate with missing non required values
        /// A new candidate should be returned successfully
        /// </summary>
        public async Task PostCandidate_MissingNonRequiredValues_ReturnsNewCandidate()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "David",
                PhoneNumber = "+255712345678",
                Email = "jdavid@example.com",
                AvailableStartTime = "",
                AvailableEndTime = "",
                LinkedInProfileUrl = "",
                GithubProfileUrl = "",
                Comment = "Good guy"
            };

            // Act
            var result = await _controller.PostCandidate(candidate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as Response;
            response.Should().NotBeNull();
            response.Data.Should().BeOfType<Candidate>();
            response.StatusCode.Should().Be(200);
            response.Message.Should().Be("New candidate created successfully.");
            response.Data.Should().BeEquivalentTo(candidate);
        }

        [Test]
        /// <summary>
        /// Tests when trying to update an existing candidate with missing non required values
        /// Candidate should be updated successfully.
        /// </summary>
        public async Task PostCandidate_MissingNonRequiredValuesOnUpdate_ReturnsUpdatedCandidate()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "David",
                PhoneNumber = "+255712345678",
                Email = "jdavid@example.com",
                AvailableStartTime = "11:00",
                AvailableEndTime = "14:00",
                LinkedInProfileUrl = "",  // missing or empty
                GithubProfileUrl = "",   // missing or empty
                Comment = "Best Guy"
            };


            // Act
            var result = await _controller.PostCandidate(candidate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as Response;
            response.Should().NotBeNull();
            response.Data.Should().BeOfType<Candidate>();
            response.StatusCode.Should().Be(200);
            response.Message.Should().Be("Candidate updated successfully.");
            response.Data.Should().BeEquivalentTo(candidate);
            response.Data.Comment.Should().Be("Best Guy");
        }

        [Test]
        /// <summary>
        /// Tests when trying to post a candidate with invalid inputs.
        /// Six inputs are invalid. Six errors should be returned.
        /// </summary>
        public async Task PostCandidate_WhenInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "David",
                PhoneNumber = "+267712345678",  // invalid country code in phone number
                Email = "jdavidexample.com",    // invalid email format
                AvailableStartTime = "0800",    // invalid time format
                AvailableEndTime = "30:00",     // exceeding 24 hrs
                LinkedInProfileUrl = "https://www.facebook.com/in/johndoe", // invalid domain
                GithubProfileUrl = "IloveProgramming", // invalid URL
                Comment = "Good guy"
            };

            string phoneRegex = @"^\+255[67]\d{2}\d{3}\d{3}$";
            string emailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            string timeRegex = @"^(?:[01]\d|2[0-3]):[0-5]\d$";
            string linkedinRegex = @"^https:\/\/(www\.)?linkedin\.com\/.*$";
            string githubRegex = @"^https:\/\/(www\.)?github\.com\/.*$";

            if (!Regex.IsMatch(candidate.PhoneNumber, phoneRegex))
            {
                _controller.ModelState.AddModelError("PhoneNumber", "Invalid phone number format");
            }

            if (!Regex.IsMatch(candidate.Email, emailRegex))
            {
                _controller.ModelState.AddModelError("Email", "Invalid email address");
            }

            if (!Regex.IsMatch(candidate.AvailableStartTime, timeRegex))
            {
                _controller.ModelState.AddModelError("AvailableStartTime", "Invalid time format. Time should be in 24-hour format (HH:mm)");
            }

            if (!Regex.IsMatch(candidate.AvailableEndTime, timeRegex))
            {
                _controller.ModelState.AddModelError("AvailableEndTime", "Invalid time format. Time should be in 24-hour format (HH:mm)");
            }

            if (!Regex.IsMatch(candidate.LinkedInProfileUrl, linkedinRegex))
            {
                _controller.ModelState.AddModelError("LinkedInProfileUrl", "The LinkedIn profile URL is not valid.");
            }

            if (!Regex.IsMatch(candidate.GithubProfileUrl, githubRegex))
            {
                _controller.ModelState.AddModelError("GithubProfileUrl", "The GitHub profile URL is not valid.");
            }

            // Act
            var result = await _controller.PostCandidate(candidate);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            var response = badRequestResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(400);
            response.Message.Should().NotBeNullOrEmpty();
            response.Message.Should().Be("Invalid candidate information");
            response.Errors.Should().NotBeNull();
            response.Errors.Should().HaveCount(6);
        }
    }
}
