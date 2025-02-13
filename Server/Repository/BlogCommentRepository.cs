using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Oqtane.Blogs.Models;

namespace Oqtane.Blogs.Repository
{
    public interface IBlogCommentRepository
    {
        IEnumerable<BlogComment> GetBlogComments(int blogId, int moduleId, bool published);
        BlogComment GetBlogComment(int blogCommentId);
        BlogComment AddBlogComment(BlogComment blogComment);
        BlogComment UpdateBlogComment(BlogComment blogComment);
        void DeleteBlogComment(int blogCommentId);
    }


    public class BlogCommentRepository : IBlogCommentRepository, IService
    {
        private readonly IDbContextFactory<BlogContext> _dbContextFactory;

        public BlogCommentRepository(IDbContextFactory<BlogContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<BlogComment> GetBlogComments(int blogId, int moduleId, bool published)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.BlogComment.AsNoTracking()
                .Include(i => i.Blog)
                .Where(i => i.Blog.ModuleId == moduleId && (blogId == -1 || i.BlogId == blogId) && i.IsPublished == published)
                .ToList();
        }

        public BlogComment GetBlogComment(int blogCommentId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.BlogComment
                .Include(i => i.Blog)
                .FirstOrDefault(i => i.BlogCommentId == blogCommentId);
        }

        public BlogComment AddBlogComment(BlogComment blogComment)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.BlogComment.Add(blogComment);
            db.SaveChanges();
            return blogComment;
        }

        public BlogComment UpdateBlogComment(BlogComment blogComment)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Entry(blogComment).State = EntityState.Modified;
            db.SaveChanges();
            return blogComment;
        }
        public void DeleteBlogComment(int blogCommentId)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var BlogComment = db.BlogComment.Find(blogCommentId);
            db.BlogComment.Remove(BlogComment);
            db.SaveChanges();
        }
    }
}
