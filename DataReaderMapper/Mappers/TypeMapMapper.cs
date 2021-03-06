using System.Collections.Generic;
using System.Linq;

namespace DataReaderMapper.Mappers
{
    public class TypeMapMapper : IObjectMapper
    {
        private readonly IEnumerable<ITypeMapObjectMapper> _mappers;

        public TypeMapMapper(IEnumerable<ITypeMapObjectMapper> mappers)
        {
            _mappers = mappers;
        }

        public object Map(ResolutionContext context, IMappingEngineRunner mapper)
        {
            context.TypeMap.Seal();

            var mapperToUse = _mappers.First(objectMapper => objectMapper.IsMatch(context, mapper));

            // check whether the context passes conditions before attempting to map the value (depth check)
            var mappedObject = !context.TypeMap.ShouldAssignValue(context) ? null : mapperToUse.Map(context, mapper);

            return mappedObject;
        }

        public bool IsMatch(ResolutionContext context)
        {
            return context.TypeMap != null;
        }
    }
}