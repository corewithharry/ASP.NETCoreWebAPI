using BlogDemo.Core.Entities;
using BlogDemo.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Infrastructure.ViewModel
{
    public class PostPropertyMapping : PropertyMapping<PostViewModel , Post>
    {
        public PostPropertyMapping():base(new Dictionary<string, List<MappedProperty>>(StringComparer.OrdinalIgnoreCase) { 
            [nameof(PostViewModel.Title)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(Post.Title) , Revert = false}
            },
            [nameof(PostViewModel.Body)] = new List<MappedProperty> {
                new MappedProperty{ Name = nameof(Post.Body) , Revert = false}
            },
            [nameof(PostViewModel.Author)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(Post.Author) , Revert = false}
            }
        })
        {
            
        }
    }
}
