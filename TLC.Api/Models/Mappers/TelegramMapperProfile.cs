using AutoMapper;
using TLC.Api.Configurations.Telegram;
using TLC.Api.Models.Vo;

namespace TLC.Api.Models.Mappers
{
    public class TelegramMapperProfile : Profile
    {
        public TelegramMapperProfile()
        {
            CreateMap<ClientConfiguration, ClientVo>()
                .ForMember(target => target.Id, option => option.MapFrom(source => source.Id))
                .ForMember(target => target.Hash, option => option.MapFrom(source => source.Hash));

            CreateMap<UserConfiguration, UserVo>()
                .ForMember(target => target.Id, option => option.MapFrom(source => source.Id));
            
            CreateMap<TelegramConfiguration, TelegramHelperVo>()
                .ForMember(target => target.Client, opt => opt.MapFrom(source => source.Client))
                .ForMember(target => target.ToUsers, opt => opt.MapFrom(source => source.ToUsers))
                .ForMember(target => target.FromUser, opt => opt.MapFrom(source => source.FromUser));
        }
    }
}
