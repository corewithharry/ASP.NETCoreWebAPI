using BlogDemo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogDemo.Infrastructure.DataBase
{
    public class MyContext  :DbContext
    {
        public MyContext(DbContextOptions<MyContext> options): base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }
    }
}
