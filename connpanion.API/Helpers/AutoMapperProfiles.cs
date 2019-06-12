using System.Linq;
using AutoMapper;
using connpanion.API.DTOs;
using connpanion.API.Models;

namespace connpanion.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDTOForList>()
                .ForMember(dest => dest.PhotoURL, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMainPhotograph).URL);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserDTOForDetail>()
                .ForMember(dest => dest.PhotoURL, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMainPhotograph).URL);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
                });
            CreateMap<Photograph, PhotographDTOForDetail>();
            CreateMap<UserDTOForUpdate, User>();
            CreateMap<Photograph, PhotographDTOForReturn>();
            CreateMap<PhotographDTOForCreate, Photograph>();
            CreateMap<UserDTOForRegister, User>();
            CreateMap<MessageDTOForCreate, Message>().ReverseMap();
            CreateMap<Message, MessageDTOForReturn>()
                .ForMember(m => m.SenderPhotoURL, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMainPhotograph).URL))
                .ForMember(m => m.ReceiverPhotoURL, opt => opt.MapFrom(u => u.Receiver.Photos.FirstOrDefault(p => p.IsMainPhotograph).URL));
        }
    }
}