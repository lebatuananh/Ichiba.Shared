using AutoMapper;

namespace Shared.Extensions
{
    public static class MapperExtension
    {
        private static IMapper _mapper;

        public static IMapper RegisterMap(this IMapper mapper)
        {
            _mapper = mapper;
            return mapper;
        }

        public static T To<T>(this object source)
        {
            return _mapper.Map<T>(source);
        }

        public static T To<T>(this object source, T dest)
        {
            return _mapper.Map(source, dest);
        }
    }
}