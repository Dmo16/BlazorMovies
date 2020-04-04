using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController :ControllerBase
    {
        private readonly ApplicationDbContext context;
        public PeopleController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get()
        {
            return await context.People.ToListAsync();
        }

        [HttpGet("id")]
        public async Task<ActionResult<Person>> Get(int id)
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null) { return NotFound(); }
            return NoContent();
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<List<Person>>> FilterByName(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) { return new List<Person>();  }
            return await context.People
                .Where(x => x.Name.Contains(searchText))
                    .Take(5)
                        .ToListAsync();

        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Person person)
        {
            context.Add(person);
            await context.SaveChangesAsync();
            return person.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Person person)
        {
            context.Attach(person).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
