using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Interfaces;
using DatingApp.API.DTOs;
using AutoMapper;
using System.Security.Claims;
using DatingApp.API.Extensions;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            
            var users = await _userRepository.GetUsersAsync();
            var usertoreturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(usertoreturn);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
           

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Faild to update user"); 
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
           var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
                return _mapper.Map<PhotoDto>(photo);

            return BadRequest("problem adding photo");
        }

        [HttpPut("set-main-photo/{photoid}")]
        public async Task<ActionResult> SetMainPhoto(int photoid)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoid);
            if (photo.IsMain)
                return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null)
                currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync())
                return NoContent();
            return BadRequest();

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            var result =  await _photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error == null)
            {
                user.Photos.Remove(photo);
                if (await _userRepository.SaveAllAsync())
                    return Ok();
            }

            return BadRequest(); 
        }
    }
}
