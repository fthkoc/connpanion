using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using connpanion.API.Data;
using connpanion.API.DTOs;
using connpanion.API.Helpers;
using connpanion.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace connpanion.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConnpanionRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IConnpanionRepository repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepository = await _repository.GetUser(currentUserID);
            userParams.UserID = currentUserID;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepository.Gender == "male" ? "female" : "male";
            }
            var users = await _repository.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserDTOForList>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(_mapper.Map<UserDTOForDetail>(await _repository.GetUser(id)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTOForUpdate userDTOForUpdate) 
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepository = await _repository.GetUser(id);
            _mapper.Map(userDTOForUpdate, userFromRepository);

            if (await _repository.SaveAll())
                return NoContent();
            else
                throw new Exception($"Updating user {id} failed on save!");
        }

        [HttpPost("{from}/like/{to}")]
        public async Task<IActionResult> LikeUser(int from, int to)
        {
            if (from != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repository.GetLike(from, to);
            if (like != null)
                return BadRequest("You already like this user");

            if (await _repository.GetUser(to) == null)
                return NotFound();

            like = new Like{ LikerID = from, LikeeID = to };
            _repository.Add<Like>(like);
            
            if (await _repository.SaveAll())
                return Ok();

            return BadRequest("MC Hammer says you can't touch this!");
        }
    }
}