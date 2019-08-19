using AutoMapper;
using TLC.Api.Models.Enums;
using TLC.Api.Models.Responses;

namespace TLC.Api.Models.Mappers
{
    public class ContactMapperProfile : Profile
    {
        public ContactMapperProfile()
        {
            CreateMap<TelegramContactResponse, ContactResponse>()
                .ForMember(target => target.Id, option => option.MapFrom(source => source.Id))
                .ForMember(target => target.Name, option => option.MapFrom(source => source.Name))
                .ForMember(target => target.Type, option => option.MapFrom(source => source.Type == ContactType.Contact ? "Contact" : "Channel"));
        }
    }
}
