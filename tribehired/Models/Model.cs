using System;
using System.Collections.Generic;

namespace tribehired.Models
{
    public class PostModel
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
    public class CommentModel
    {
        public int PostId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }
    public class ApiResponseModel
    {
        public int post_id { get; set; }
        public string post_title { get; set; }
        public string post_body { get; set; }
        public int total_number_of_comments { get; set; }
        public object comments { get; set; }
    }
}
