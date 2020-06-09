using AutoMapper;

namespace Microservice.Core3.AzureAd.Configurations
{
    // ReSharper disable once UnusedMember.Global
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Disable mapping by same name (You will have to specify each mapping manually) 
            DefaultMemberConfig.MemberMappers.Clear();
            DefaultMemberConfig.NameMapper.NamedMappers.Clear();


        }
    }
}
