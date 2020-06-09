using AutoMapper;
using Microservice.Core3.AzureAd.Configurations.Exceptions;

namespace Microservice.Core3.AzureAd.Services
{
    public class ValueService
    {
        private readonly IMapper _mapper;

        public ValueService(IMapper mapper) => _mapper = mapper;

        public void CustomExceptionVoid() => throw new CustomException(Types.Conflict);

        public void CustomExceptionMessage() => throw new CustomException(Types.Conflict, "This is a known exception, yei!");
    }
}
