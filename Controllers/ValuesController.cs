using EventManagementSystem.Data;
using EventManagementSystem.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ValuesController : ControllerBase
    {
        AppDbContext _context;
        // GET: api/<ValuesController>
         public ValuesController (AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("{name}")]
        public IEnumerable<string> Get(string name)
        {
            return new string[] { "value1", name };
        }

        // GET api/<ValuesController>/5
        [HttpPost("name/{id}")]
        public IActionResult PostWithId(int id)
        {     
            
            return Ok($"Hello id: {id}") ;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] Role value)
        { 
           // Console.WriteLine(value.EndDate);
           _context.Roles.Add(value);

        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
