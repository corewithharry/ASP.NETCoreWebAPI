using AutoMapper;
using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.DataBase;
using BlogDemo.Infrastructure.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public PostController(
            IPostRepository postRepository , 
            IUnitOfWork unitOfWork,
            ILoggerFactory logger , 
            IMapper mapper)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("BlogDemo.Api.Controllers.PostController");
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(PostParameters postParameters)
        {
            var postlist = await _postRepository.GetAllPostsAsync(postParameters);

            var postViewModels = _mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(postlist);

            var meta = new
            {
                Pagesize = postlist.PageSize,
                PageIndex = postlist.PageIndex,
                TotalItemCount = postlist.TotalItemsCount,
                PageCount = postlist.PageCount
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta , new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            return Ok(postViewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var post =await _postRepository.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();
            var postViewModel = _mapper.Map<Post, PostViewModel>(post);
            return Ok(postViewModel);
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