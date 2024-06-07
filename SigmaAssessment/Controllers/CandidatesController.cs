using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
        public async Task<ActionResult<Candidate>> PostCandidate(Candidate candidate)
        {
            throw new NotImplementedException("Does nothing");
        }

        private bool CandidateExists(string email)
        {
            return (_context.CandidatesDb?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}
