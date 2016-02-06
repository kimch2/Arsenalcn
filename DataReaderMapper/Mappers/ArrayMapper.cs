using System;
using DataReaderMapper.Internal;

namespace DataReaderMapper.Mappers
{
    public class ArrayMapper : EnumerableMapperBase<Array>
    {
        public override bool IsMatch(ResolutionContext context)
        {
            return (context.DestinationType.IsArray) && (PrimitiveExtensions.IsEnumerableType(context.SourceType));
        }

        protected override void ClearEnumerable(Array enumerable)
        {
            // no op
        }

        protected override void SetElementValue(Array destination, object mappedValue, int index)
        {
            destination.SetValue(mappedValue, index);
        }

        protected override Array CreateDestinationObjectBase(Type destElementType, int sourceLength)
        {
            throw new NotImplementedException();
        }

        protected override object GetOrCreateDestinationObject(ResolutionContext context, IMappingEngineRunner mapper,
            Type destElementType, int sourceLength)
        {
            return ObjectCreator.CreateArray(destElementType, sourceLength);
        }
    }
}