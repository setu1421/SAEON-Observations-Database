using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Posts")]
    public class PostsController : Controller
    {
        BlogDbContext db = null;

        public PostsController(BlogDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IEnumerable<Post> GetAll()
        {
            return db.Posts.Include(i => i.PostTags).ToList();
        }
    }

    [Produces("application/json")]
    [Route("api/Tags")]
    public class TagsController : Controller
    {
        BlogDbContext db = null;

        public TagsController(BlogDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IEnumerable<Tag> GetAll()
        {
            return db.Tags.Include(i => i.PostTags).ToList();
        }
    }

    [Produces("application/json")]
    [Route("api/PostTags")]
    public class PostTagsController : Controller
    {
        BlogDbContext db = null;

        public PostTagsController(BlogDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IEnumerable<PostTag> GetAll()
        {
            return db.PostTags.Include(i => i.Post).Include(i => i.Tag).ToList();
        }
    }

}