using AutoMapper;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTOs;
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
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;


        public PeopleController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            return await queryable.Paginate(paginationDTO).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> Get(int id)
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null) { return NotFound(); }
            return person;
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
            var personDB = await context.People.FirstOrDefaultAsync(x => x.Id == person.Id);

            if (personDB == null) { return NotFound(); }

            personDB = mapper.Map(person, personDB);

            if (!string.IsNullOrWhiteSpace(person.Picture))
            {
                //update picture
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var genre = await context.People.FirstOrDefaultAsync(x => x.Id == Id);
            if (genre == null) { return NotFound(); }
            context.Remove(genre);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
