﻿namespace MicroBlog.Web.Models.ApiDto
{
    public class BlogPostApiDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ApplicationUserId { get; set; }
    }
}