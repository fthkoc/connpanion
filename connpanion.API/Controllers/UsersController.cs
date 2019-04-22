using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using connpanion.API.Data;
using connpanion.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace connpanion.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            return Ok(_mapper.Map<IEnumerable<UserDTOForList>>(await _repository.GetUsers()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(_mapper.Map<UserDTOForDetail>(await _repository.GetUser(id)));
        }
    }
}