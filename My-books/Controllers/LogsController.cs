using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My_books.Controllers
{
    //[Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private LogsService _logsService;
        public LogsController(LogsService logsService)
        {
            _logsService = logsService;
        }

        [HttpGet("get-all-logs-from-db")]
        public async Task<ActionResult<List<LogEntry>>> GetAllLogsFromDB()
        {
            try
            {
                var logs = await _logsService.GetAllLogsAsync();
                return Ok(logs);
            }
            catch (Exception)
            {
                return BadRequest("Could not load logs from the database");
            }
        }
    }
}
