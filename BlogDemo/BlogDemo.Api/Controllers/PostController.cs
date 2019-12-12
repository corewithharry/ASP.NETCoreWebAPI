using BlogDemo.Infrastructure.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly MyContext _myContext;
        public PostController(MyContext myContext)
        {
            _myContext = myContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var posts = await _myContext.Posts.ToListAsync();
            return Ok(posts);
        }

    }
}