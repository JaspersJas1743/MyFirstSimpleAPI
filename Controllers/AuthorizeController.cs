using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly UserContext _context;

        public AuthorizeController(UserContext context, ILogger<AuthorizeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetTodoItems() => await _context.TodoItems.Select(x => UserToDTO(x)).ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(long id)
        {
            var user = await _context.TodoItems.FindAsync(id);
            if (user is null) return NotFound();
            _logger.LogInformation($"Токен для {user.Name} {user.Surname}: {user.Token}");

            return UserToDTO(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserDTO userDTO)
        {
            if (id != userDTO.Id) return BadRequest();

            var user = await _context.TodoItems.FindAsync(id);
            if (user is null) return NotFound();

            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (UserExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO user)
        {
            var item = new User { Name = user.Name, Surname = user.Surname, Token = Utility.Generator.GenerateSimpleToken() };
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.Id }, UserToDTO(item));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.TodoItems.FindAsync(id);
            if (user is null) return NotFound();
            _context.TodoItems.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserExists(long id) => (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        
        private static UserDTO UserToDTO(User user) => new UserDTO { Id = user.Id, Name = user.Name, Surname = user.Surname };
    }
}
