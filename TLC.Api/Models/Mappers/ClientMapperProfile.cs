using AutoMapper;
using TLC.Api.Models.Responses;

namespace TLC.Api.Models.Mappers
{
    public class ClientMapperProfile : Profile
    {
        public ClientMapperProfile()
        {
            CreateMap<TelegramCodeResponse, ClientResponse>()
                .ForMember(target => target.PhoneCodeHash, option => option.MapFrom(source => source.PhoneHashCode));
        }
    }
}
