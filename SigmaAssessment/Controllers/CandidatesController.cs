using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SigmaAssessment.Helpers;
using SigmaAssessment.Models;

namespace SigmaAssessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly CandidateDbCtx _context;
        private readonly ILogger<CandidatesController> _logger;

        public CandidatesController(CandidateDbCtx context, ILogger<CandidatesController> logger)
        {
            _context = context;
            _logger = logger;
            _context.Database.EnsureCreated();
        }

        // POST: api/Candidates
        /// <summary>
        /// Adds a new candidate or updates an existing candidate if the input email exists already
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns>A new or updated candidate information</returns>
        /// <response code="200">Returns the newly created or updated candidate</response>
        /// <response code="400">If the candidate info is invalid, error response is returned</response>
        /// <response code="500">An error occured while processing the request</response>
        [HttpPost]
        public async Task<ActionResult<Candidate>> PostCandidate(Candidate candidate)
        {
            var response = new Response();
            try
            {
                if (!ModelState.IsValid) // invalid data
                {
                    // collect all errors in ModelState and return them
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();

                    var errorResponse = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid candidate information",
                        Errors = errors
                    };

                    _logger.LogError(errorResponse.Message + ": \t" + string.Join(",\n\t\t\t\t\t", errors));

                    return BadRequest(errorResponse);
                }

                if(CandidateExists(candidate.Email))  // if candidate email exists, update the candidate
                {
                    // update existing candidate
                    _context.CandidatesDb.Update(candidate);
                    await _context.SaveChangesAsync();

                    response.Data = candidate;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Candidate updated successfully.";

                    _logger.LogInformation("Candidate {0} with email {1} updated successfully!", 
                        candidate.FirstName + " " + candidate.LastName, candidate.Email);

                    return Ok(response);
                }
                else
                {
                    // create new candidate
                    _context.CandidatesDb.Add(candidate);
                    await _context.SaveChangesAsync();

                    response.Data = candidate;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "New candidate created successfully.";

                    _logger.LogInformation("New candidate {0} with email {1} created successfully!", 
                        candidate.FirstName + " " + candidate.LastName, candidate.Email);

                    return Ok(response);
                }
            }
            catch (Exception ex) {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while processing your request";

                _logger.LogError("Error creating or updating candidate: " + ex.Message);
                return base.StatusCode(StatusCodes.Status500InternalServerError, response);
            }
                
        }

        /// <summary>
        /// Checks the list of candidates to see if a candidate with the given email exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true if email address is found, false if email address not found</returns>
        private bool CandidateExists(string email)
        {
            return (_context.CandidatesDb?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}
