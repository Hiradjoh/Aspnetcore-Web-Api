using Microsoft.EntityFrameworkCore;
using My_books.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My_books.Data.Services
{
    public class LogsService
    {
        private ProjectDbContext _context;
        public LogsService(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<LogEntry>> GetAllLogsAsync()
        {
            return await _context.Logs
                                 .OrderByDescending(l => l.TimeStamp)
                                 .ToListAsync();
        }
    }
}
