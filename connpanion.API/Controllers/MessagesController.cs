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
    [Route("api/users/{userID}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IConnpanionRepository _repository;
        private readonly IMapper _mapper;

        public MessagesController(IConnpanionRepository repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet("{messageID}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userID, int messageID)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepository = await _repository.GetMessage(messageID);
            if (messageFromRepository == null)
                return NotFound();
            
            return Ok(messageFromRepository);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userID, [FromQuery]MessageParams messageParams)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserID = userID;
            var messagesFromRepository = await _repository.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageDTOForReturn>>(messagesFromRepository);
            
            Response.AddPagination(messagesFromRepository.CurrentPage, messagesFromRepository.PageSize, 
                messagesFromRepository.TotalCount, messagesFromRepository.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{receiverID}")]
        public async Task<IActionResult> GetMessageThread(int userID, int receiverID)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepository = await _repository.GetMessageThread(userID, receiverID);
            var messageThread = _mapper.Map<IEnumerable<MessageDTOForReturn>>(messageFromRepository);
            
            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userID, MessageDTOForCreate messageDTOForCreate)
        {
            var sender = await _repository.GetUser(userID);
            if (sender.ID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageDTOForCreate.SenderID = userID;

            var receiver = await _repository.GetUser(messageDTOForCreate.ReceiverID);
            if (receiver == null)
                return BadRequest("Could not find the user.");

            var message = _mapper.Map<Message>(messageDTOForCreate);
            _repository.Add(message);
            
            if (await _repository.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageDTOForReturn>(message);
                return CreatedAtRoute("GetMessage", new {messageID = message.ID}, messageToReturn);
            }

            throw new Exception("Error! MessagesController::CreateMessage()");
        }

        [HttpPost("{messageID}")]
        public async Task<IActionResult> DeleteMessage(int messageID, int userID)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepository = await _repository.GetMessage(messageID);
            if (messageFromRepository.SenderID == userID)
                messageFromRepository.SenderDeleted = true;

            if (messageFromRepository.ReceiverID == userID)
                messageFromRepository.ReceiverDeleted = true;

            if (messageFromRepository.SenderDeleted && messageFromRepository.ReceiverDeleted)
                _repository.Delete(messageFromRepository);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception("Error! MessagesController::DeleteMessage()");
        }

        [HttpPost("{messageID}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int messageID, int userID)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _repository.GetMessage(messageID);
            if (message.ReceiverID != userID)
                return Unauthorized();

            message.isRead = true;
            message.DateRead = DateTime.Now;
            await _repository.SaveAll();
            return NoContent();
        }
    }
}