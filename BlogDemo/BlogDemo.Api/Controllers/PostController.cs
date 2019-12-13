using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public PostController(
            IPostRepository postRepository , 
            IUnitOfWork unitOfWork,
            ILoggerFactory logger)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("BlogDemo.Api.Controllers.PostController");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var posts = await _postRepository.GetAllPosts();

            _logger.LogError("Get All Posts...");

            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var newPost = new Post
            {
                Author = "admin",
                Body = "123123123123123123123",
                Title = "Title A",
                LastModified = DateTime.Now
            };

            _postRepository.AddPost(newPost);
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        

    }
}