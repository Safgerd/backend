using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.DTO;
using WebApp.Service.Interface;

namespace WebApp.Controllers
{
    [Route("api/posts/{postId}/comments")]
    [Route("api/comments")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: api/posts/1/comments
        [HttpGet] // Будет соответствовать api/posts/{postId}/comments
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost(int postId)
        {
            var comments = await _commentService.GetCommentsForPostAsync(postId);
            return Ok(comments);
        }

        // GET: api/comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }


        // POST: api/posts/1/comments (или просто api/comments, если PostId в теле)
        [HttpPost] // Будет соответствовать api/posts/{postId}/comments
        public async Task<ActionResult<CommentDto>> PostComment(int postId, CreateCommentDto createCommentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            createCommentDto.PostId = postId;

            var createdComment = await _commentService.CreateCommentAsync(createCommentDto, userId);

            if (createdComment == null)
            {
                return BadRequest("Could not create comment. Post might not exist.");
            }

            return CreatedAtAction(nameof(GetComment), new { id = createdComment.Id }, createdComment);
        }

        // DELETE: api/comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _commentService.DeleteCommentAsync(id, userId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
