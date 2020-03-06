using AutoMapper;
using Microservice.Core3.AzureAd.Configurations.Exceptions;

// ReSharper disable SuggestVarOrType_Elsewhere
namespace Microservice.Core3.AzureAd.Configurations
{
    // ReSharper disable once UnusedMember.Global
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var baseCustomExceptionExceptionsResponse = CreateMap<BaseCustomException, ExceptionsResponse>()
                .ForMember(d => d.Title, opt => opt.MapFrom(o => o.Title))
                .ForMember(d => d.Detail, opt => opt.MapFrom(o => o.Detail))
                .ForMember(d => d.Message, opt => opt.MapFrom(o => o.Message));

            baseCustomExceptionExceptionsResponse.ForAllOtherMembers(opt => opt.Ignore());
            baseCustomExceptionExceptionsResponse.ReverseMap().ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
