using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Data.Models;
using WebApp.DTO;
using WebApp.Service.Interface;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, UpdatePostDto updatePostDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _postService.UpdatePostAsync(id, updatePostDto, userId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }


        // POST: api/Posts
        [HttpPost]
        public async Task<ActionResult<PostDto>> PostPost(CreatePostDto createPostDto) // Принимаем DTO
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var createdPost = await _postService.CreatePostAsync(createPostDto, userId);
            if (createdPost == null)
            {
                return BadRequest("Could not create post.");
            }

            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _postService.DeletePostAsync(id, userId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
