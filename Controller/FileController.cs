using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnimeNowApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("請選擇文件");
            }

            // 限制文件類型
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                return BadRequest("只允許上傳 JPEG, PNG 或 GIF 格式的圖片");
            }

            // 限制文件大小 (例如 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("文件大小不能超過 5MB");
            }

            // 生成唯一文件名
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            // 確保目錄存在
            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "uploads"));

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 返回文件 URL
            var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            return Ok(new { url });
        }
    }
}