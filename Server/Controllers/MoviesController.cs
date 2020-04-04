using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMovies.Shared.DTOs;
using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public MoviesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetailsMovieDTO>> GetMovieDetailsDTO(int id)
        {
            var movie = await context.Movies
                .Where(x => x.Id == id)
                    .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                    .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                    .FirstOrDefaultAsync();

            if (movie == null) { return NotFound(); }

            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();
            var model = new DetailsMovieDTO();
            model.Movie = movie;
            model.Genres = movie.MoviesGenres.Select(x => x.Genre).ToList();
            model.Actors = movie.MoviesActors.Select(x =>
                new Person
                {
                    Name = x.Person.Name,
                    Picture = x.Person.Picture,
                    Character = x.Character,
                    Id = x.PersonId
                }).ToList();
            return model;            
        }

        [HttpGet]
        public async Task<ActionResult<IndexPageDTO>> GetMovieInTheater()
        {
            var limit = 6;
            var moviesInTheaters = await context.Movies
                .Where(x => x.InTheaters).Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();

            var todaysDate = DateTime.Today;

            var upcomingReleases = await context.Movies
                .Where(x => x.ReleaseDate > todaysDate).Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();

            var response = new IndexPageDTO();
            response.InTheaters = moviesInTheaters;
            response.UpcomingReleases = upcomingReleases;

            return response;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }
    }
}
