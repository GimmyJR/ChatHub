using ChatHub.Dto;
using ChatHub.Models;
using ChatHub.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ChatHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IRegisterRepository registerRepository;

        public AccountController(IRegisterRepository registerRepository)
        {
            this.registerRepository = registerRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]RegisterDto register,CancellationToken cancellationToken)
        {
            bool isNameExist=await registerRepository.IsNameExist(register.Name);
            if (isNameExist)
            {
                return BadRequest(new { Nessage = "This User Name Is Already exist" });
            }
            string avatarFilePath = string.Empty;
            if (register.Avatar != null && register.Avatar.Length > 0)
            {
                var uploadsfolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
                if (!Directory.Exists(uploadsfolder))
                {
                    Directory.CreateDirectory(uploadsfolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(register.Avatar.FileName);
                avatarFilePath = Path.Combine(uploadsfolder, uniqueFileName);
                using (var fileStream = new FileStream(avatarFilePath, FileMode.Create))
                {
                    await register.Avatar.CopyToAsync(fileStream);
                }
                avatarFilePath = $"/avatars/{uniqueFileName}";
            }

            User user = new()
            {
                Name = register.Name,
                Avatar = avatarFilePath
            };
            registerRepository.AddToDb(user,cancellationToken);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
        {
            User user = await registerRepository.IsUserExist(loginDto.name, cancellationToken);
            if(user is null) 
            {
                return BadRequest(new { Message = "username is wrong" });
            }
            return Ok(user);
        }
        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> Logout(Guid userId)
        {
            await registerRepository.Logout(userId);
            return Ok();
        }
    }
}
