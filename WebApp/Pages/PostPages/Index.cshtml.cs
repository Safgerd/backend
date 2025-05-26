using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Data.Models;
using WebApp.DTO;
using WebApp.Service.Interface;

namespace WebApp.Pages.PostPages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPostService _postService;

        public IndexModel(IPostService postService)
        {
            _postService = postService;
        }

        public List<PostDto> Posts { get; set; } = new List<PostDto>();

        public int CurrentPage { get; set; } = 0;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10; // Увеличим размер страницы для примера
        public int WindowSize { get; set; } = 2; // Для пагинации

        public async Task OnGetAsync(int? pageNumber) // pageNumber теперь nullable для удобства
        {
            CurrentPage = pageNumber ?? 0; // Если pageNumber не передан, используем 0

            var result = await _postService.GetPaginatedPostsAsync(CurrentPage, PageSize);
            Posts = new List<PostDto>(result.Posts);
            TotalPages = result.TotalPages;
        }
    }
}
