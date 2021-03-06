using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
   
    public class UsersController : BaseApiController
    {
        
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersForList = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersForList);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userForDetailed = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userForDetailed);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {

            // if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on saved");

        }

    }
}