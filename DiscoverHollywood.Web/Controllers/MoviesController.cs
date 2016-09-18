using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DiscoverHollywood.Web.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        // GET: api/movies
        [HttpGet]
        public IEnumerable<Models.Movie> Get(string q, int? year, string genres, int page = 0)
        {
            return Models.Job.Movies(q, year, genres, page);
        }

        // GET api/movies/5
        [HttpGet("{id}")]
        public Models.Movie Get(int id)
        {
            return Models.Job.Movie(id);
        }

        // GET api/movies/5/ratings
        [HttpGet("{id}/ratings")]
        public IEnumerable<Models.RatingSummary> Ratings(int id)
        {
            return Models.Job.MovieRatings(id);
        }

        // GET api/movies/5/tags
        [HttpGet("{id}/tags")]
        public IEnumerable<Models.Tag>Tags(int id)
        {
            return Models.Job.MovieTags(id, 0, 50);
        }

    }
}
