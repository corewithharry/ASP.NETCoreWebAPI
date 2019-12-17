using BlogDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Infrastructure.Services
{
    public abstract class PropertyMapping<TSource , TDestination> : IPropertyMapping where TDestination: IEntity
    {
        public Dictionary<string , List<MappedProperty>> MappingDictionary { get; }

        protected PropertyMapping(Dictionary<string , List<MappedProperty>> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            mappingDictionary[nameof(IEntity.Id)] = new List<MappedProperty>
            {
                new MappedProperty{Name = nameof(IEntity.Id) , Revert = false }
            };
        }
    }
}
