using AutoMapper;
using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.DataBase;
using BlogDemo.Infrastructure.Extensions;
using BlogDemo.Infrastructure.Services;
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
        private readonly IUrlHelper _urlHelper;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        public PostController(
            IPostRepository postRepository , 
            IUnitOfWork unitOfWork,
            ILoggerFactory logger , 
            IMapper mapper,
            IUrlHelper urlHelper , 
            ITypeHelperService typeHelperService,
            IPropertyMappingContainer propertyMappingContainer)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("BlogDemo.Api.Controllers.PostController");
            _mapper = mapper;
            _urlHelper = urlHelper;
            _typeHelperService = typeHelperService;
            _propertyMappingContainer = propertyMappingContainer;
        }

        [HttpGet(Name ="GetPosts")]
        public async Task<IActionResult> Get(PostParameters postParameters)
        {
            if (!_typeHelperService.TypeHasProperties<PostViewModel>(postParameters.Fields))
                return BadRequest("Fields not exist");
            var postlist = await _postRepository.GetAllPostsAsync(postParameters);

            var postViewModels = _mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(postlist);

            var shapedPostViewModels = postViewModels.ToDynamicIEnumerable(postParameters.Fields);

            var previousPageLink = postlist.HasPrevious ? CreatePostUri(postParameters, PaginationResourceUriType.PerviousPage) : null;
            var nextPageLink = postlist.HasNext ? CreatePostUri(postParameters, PaginationResourceUriType.NextPage): null;

            var meta = new
            {
                postlist.PageSize,
                postlist.PageIndex,
                postlist.TotalItemsCount,
                postlist.PageCount ,
                previousPageLink,
                nextPageLink
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta , new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            return Ok(shapedPostViewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id , string fields)
        {
            var post =await _postRepository.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();
            var postViewModel = _mapper.Map<Post, PostViewModel>(post);

            var shapedPostViewModels = postViewModel.ToDynamic(fields);

            return Ok(shapedPostViewModels);
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

        private string CreatePostUri(PostParameters parameters , PaginationResourceUriType uriType)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PerviousPage:
                    var previousParameters = new
                    {

                        PageIndex = parameters.PageIndex - 1,
                        PageSize = parameters.PageSize,
                        OrderBy = parameters.OrderBy,
                        Fileds = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        PageIndex = parameters.PageIndex + 1,
                        PageSize = parameters.PageSize,
                        OrderBy = parameters.OrderBy,
                        Fileds = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", nextParameters);
                default:
                    var currentParameters = new
                    {
                        PageIndex = parameters.PageIndex,
                        PageSize = parameters.PageSize,
                        OrderBy = parameters.OrderBy,
                        Fileds = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", currentParameters);
            }
        }

    }
}