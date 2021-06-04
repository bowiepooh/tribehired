using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using tribehired.Models;

namespace tribehired.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public static string requestPostUrl = "https://jsonplaceholder.typicode.com/posts";
        public static string requestCommentUrl = "https://jsonplaceholder.typicode.com/comments";
        [HttpGet]
        public object TopPost() //Question 1
        {
            List<PostModel> resultPost = RequestByJson<List<PostModel>>(requestPostUrl);
            List<CommentModel> resultComment = RequestByJson<List<CommentModel>>(requestCommentUrl);
            List<ApiResponseModel> listApi = new List<ApiResponseModel>();
            if (resultPost.Count() > 0)
            {
                foreach (var item in resultPost)
                {
                    var resultfilterComment = resultComment.Where(p => p.PostId == item.Id).Select(p => new { Name = p.Name.ToString(), Email = p.Email.ToString(), Comment = p.Body.ToString() }).ToList();
                    ApiResponseModel model = new ApiResponseModel()
                    {
                        post_body = item.Body,
                        post_id = item.Id,
                        post_title = item.Title,
                        total_number_of_comments = resultComment.Count() > 0 ? resultComment.Where(p => p.PostId == item.Id).Count() : 0,
                        comments = resultfilterComment
                    };
                    listApi.Add(model);
                }
                listApi = listApi.OrderBy(p => p.total_number_of_comments).ToList();
                return listApi;
            }
            return "error";
        }

        [HttpGet]
        public object SearchApi(string postTitle = null, string postBody = null,  string commentName = null, string email = null, string comment = null, int userId = 0)
        {
            List<PostModel> resultPost = RequestByJson<List<PostModel>>(requestPostUrl);
            List<CommentModel> resultComment = RequestByJson<List<CommentModel>>(requestCommentUrl);
            List<PostModel> filterPost = new List<PostModel>();
            List<CommentModel> filterComment = new List<CommentModel>();
            if (resultComment.Count() > 0)
            {
                if (!string.IsNullOrEmpty(postTitle))
                {
                   var result =  resultPost.Where(p => p.Title.Contains(postTitle));
                    filterPost.AddRange(result);
                }
                if (!string.IsNullOrEmpty(postBody))
                {
                    var result = resultPost.Where(p => p.Body.Contains(postBody));
                    filterPost.AddRange(result);
                }
                if (!string.IsNullOrEmpty(commentName))
                {
                    var result = resultComment.Where(p => p.Name.Contains(commentName));
                    filterComment.AddRange(result);
                }
                if(!string.IsNullOrEmpty(email))
                {
                    var result = resultComment.Where(p => p.Email.Contains(email));
                    filterComment.AddRange(result);
                }
                if (!string.IsNullOrEmpty(comment))
                {
                    var result = resultComment.Where(p => p.Body.Contains(comment));
                    filterComment.AddRange(result);
                }
            }
            List<ApiResponseModel> listApi = new List<ApiResponseModel>();
            if (filterPost.Count() > 0)
            {
                foreach (var item in filterPost)
                {
                    var resultfilterComment = resultComment.Where(p => p.PostId == item.Id).Select(p => new { Name = p.Name.ToString(), Email = p.Email.ToString(), Comment = p.Body.ToString() }).ToList();
                    ApiResponseModel model = new ApiResponseModel()
                    {
                        post_body                = item.Body,
                        post_id                  = item.Id,
                        post_title               = item.Title,
                        total_number_of_comments = resultComment.Count() > 0 ? resultComment.Where(p => p.PostId == item.Id).Count() : 0,
                        comments                 = resultfilterComment
                    };
                    listApi.Add(model);
                }
                listApi = listApi.OrderBy(p => p.total_number_of_comments).ToList();
                return listApi;
            }

            if (filterComment.Count() > 0)
            {
                foreach (var item in filterComment)
                {
                    string singleRequestPortUrl = requestPostUrl + "/" + item.PostId;
                    PostModel post = RequestByJson<PostModel>(singleRequestPortUrl);
                    ApiResponseModel model = new ApiResponseModel()
                    {
                        post_body                = post.Body,
                        post_id                  = item.Id,
                        post_title               = post.Title,
                        total_number_of_comments = filterComment.Count(),
                        comments                 = item
                    };
                    listApi.Add(model);
                }
                listApi = listApi.OrderBy(p => p.total_number_of_comments).ToList();
                return listApi;
            }

            return "error";
        }


        private T RequestByJson<T>(string requestUrl, string param = "", bool isGet = true) where T : new()
        {
            var client = new RestClient(requestUrl);
            var request = (isGet) ? new RestRequest(Method.GET) : new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            if (!isGet)
                request.AddParameter("undefined", param, ParameterType.RequestBody);
            IRestResponse<T> response = client.Execute<T>(request);
            Console.WriteLine(response);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
