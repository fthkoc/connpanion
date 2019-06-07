using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using connpanion.API.Data;
using connpanion.API.DTOs;
using connpanion.API.Helpers;
using connpanion.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace connpanion.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IMapper _mapper;
        private readonly IConnpanionRepository _repository;
        private Cloudinary _cloudinary;

        public PhotosController(IConnpanionRepository repository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repository = repository;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepository = await _repository.GetPhoto(id);

            var photo = _mapper.Map<PhotographDTOForReturn>(photoFromRepository);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userID, [FromForm]PhotographDTOForCreate photographDTOForCreate) 
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepository = await _repository.GetUser(userID);
            var file = photographDTOForCreate.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream()) 
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photographDTOForCreate.URL = uploadResult.Uri.ToString();
            photographDTOForCreate.PublicID = uploadResult.PublicId;
            
            var photo = _mapper.Map<Photograph>(photographDTOForCreate);
            if (!userFromRepository.Photos.Any(u => u.IsMainPhotograph)) {
                photo.IsMainPhotograph = true;
            }

            userFromRepository.Photos.Add(photo);

            if (await _repository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotographDTOForReturn>(photo);                
                return CreatedAtRoute("GetPhoto", new { id = photo.ID}, photoToReturn);
            }
            else
                return BadRequest("Couldn't add the photo");
        }

        [HttpPost("{photoID}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userID, int photoID)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repository.GetUser(userID);
            if (!user.Photos.Any(p => p.ID == photoID))
                return Unauthorized();

            var photoFromRepository = await _repository.GetPhoto(photoID);
            if (photoFromRepository.IsMainPhotograph)
                return BadRequest("This is already the main photograph.");

            var currentMainPhoto = await _repository.GetMainPhotoForUser(userID);
            currentMainPhoto.IsMainPhotograph = false;
            photoFromRepository.IsMainPhotograph = true;

            if (await _repository.SaveAll())
                return NoContent();
            else
                return BadRequest("Could not set as a main photograph.");
        }

        [HttpDelete("{photoID}")]
        public async Task<IActionResult> DeletePhoto(int userID, int photoID)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repository.GetUser(userID);
            if (!user.Photos.Any(p => p.ID == photoID))
                return Unauthorized();

            var photoFromRepository = await _repository.GetPhoto(photoID);
            if (photoFromRepository.IsMainPhotograph)
                return BadRequest("You can not delete your main photograph, change it first.");

            if (photoFromRepository.PublicID == null)
                _repository.Delete(photoFromRepository);
            else {
                var deleteParameters = new DeletionParams(photoFromRepository.PublicID);
                var result = _cloudinary.Destroy(deleteParameters);
                if (result.Result.Equals("ok"))
                    _repository.Delete(photoFromRepository);
            }

            if (await _repository.SaveAll())
                return Ok();
            else
                return BadRequest("Failed to delete the photograph.");
        }
    }
}