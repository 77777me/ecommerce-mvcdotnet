using eTickets.Data.Base;
using eTickets.Data.ViewModels;
using eTickets.Models;
using Microsoft.EntityFrameworkCore;

namespace eTickets.Data.Services
{
    public class MovieService:EntityBaseRepository<Movie>, IMovieService
    {
        private readonly AppDbContext _context;
        public MovieService(AppDbContext context):base(context)
        {
            _context=context;
        }

        public async Task AddNewMovieAsync(NewMovieVM data)
        {
            var newmovie = new Movie()
            {
                Name = data.Name,
                Description = data.Description,
                Price = data.Price,
                ImageURL = data.ImageURL,
                CinemaId = data.CinemaId,
                StartDate = DateTime.SpecifyKind(data.StartDate, DateTimeKind.Utc), // Ensure UTC kind
                EndDate = DateTime.SpecifyKind(data.EndDate, DateTimeKind.Utc), // Ensure UTC kind
                MyMovieCaProperty = data.MyMovieCaProperty,
                ProducerId = data.ProducerId
            };
            await _context.Movies.AddAsync(newmovie);
            await _context.SaveChangesAsync();

            //add Movie_actors
            foreach (var actor in data.actorIds)
            {
                var actorMovie = new Actor_Movie()
                {
                    MovieId = newmovie.Id,
                    ActorId = actor
                };
                await _context.Actors_Movie.AddAsync(actorMovie);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            var movieDetails=await _context.Movies
                .Include(c =>c.Cinema)
                .Include(p => p.producer)
                .Include(am =>am.Actor_Movies).ThenInclude(a=>a.Actor)
                .FirstOrDefaultAsync(n=>n.Id==id);

            return movieDetails;
        }

        public async Task<NewMovieDropdownVM> GetNewMovieDropdownValues()
        {
            var response = new NewMovieDropdownVM()
            {
                Actors = await _context.Actors.OrderBy(n => n.FullName).ToListAsync(),
                Cinemas = await _context.Cinemas.OrderBy(n => n.Name).ToListAsync(),
                Producers = await _context.Producers.OrderBy(n => n.fullName).ToListAsync()

            };
            return response;
        }

        public async Task UpdateNewMovieAsync(NewMovieVM data)
        {
            var dbMovie=await _context.Movies.FirstOrDefaultAsync(n=>n.Id == data.Id);

            if (dbMovie != null)
            {
                dbMovie.Name = data.Name;
                dbMovie.Description = data.Description;
                dbMovie.Price = data.Price;
                dbMovie.ImageURL = data.ImageURL;
                dbMovie.CinemaId = data.CinemaId;
                dbMovie.StartDate = DateTime.SpecifyKind(data.StartDate, DateTimeKind.Utc); // Ensure UTC kind
                dbMovie.EndDate = DateTime.SpecifyKind(data.EndDate, DateTimeKind.Utc); // Ensure UTC kind
                dbMovie.MyMovieCaProperty = data.MyMovieCaProperty;
                dbMovie.ProducerId = data.ProducerId;
                
                await _context.SaveChangesAsync();

            }
            //remove existing actors
            var existingActorsDb=_context.Actors_Movie.Where(n=>n.MovieId == data.Id).ToList();
            _context.Actors_Movie.RemoveRange(existingActorsDb);
            await _context.SaveChangesAsync();

            //add Movie_actors
            foreach (var actor in data.actorIds)
            {
                var actorMovie = new Actor_Movie()
                {
                    MovieId = data.Id,
                    ActorId = actor
                };
                await _context.Actors_Movie.AddAsync(actorMovie);
            }
            await _context.SaveChangesAsync();
        }
    }
}
