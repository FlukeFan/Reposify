using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace Reposify.NHibernate
{
    public static class NhHelper
    {
        public static HbmMapping CreateConventionalMappings<TId>(Type rootEntityType, ConventionModelMapper mapper = null)
        {
            mapper = mapper ?? new ConventionModelMapper();

            var baseEntities =
                rootEntityType.Assembly.GetTypes()
                    .Where(t => t.BaseType == rootEntityType)
                    .OrderBy(t => t.FullName);

            var allEntities =
                rootEntityType.Assembly.GetTypes()
                    .Where(t => rootEntityType.IsAssignableFrom(t) && t != rootEntityType)
                    .OrderBy(t => t.FullName);

            var entitiesWithoutHierarchy =
                baseEntities
                    .Where(b => allEntities.Where(e => e.BaseType == b).Count() == 0)
                    .ToList();

            var entitiesWithHierarchy =
                allEntities
                    .Where(e => !entitiesWithoutHierarchy.Contains(e))
                    .ToList();

            mapper.IsEntity((t, declared) => allEntities.Contains(t));
            mapper.IsRootEntity((t, declared) => baseEntities.Contains(t));
            mapper.IsTablePerClassHierarchy((t, declared) => entitiesWithHierarchy.Contains(t));

            mapper.Class<IEntity<TId>>(m =>
            {
                m.Id(e => e.Id, im => im.Generator(Generators.Native));
            });

            return mapper.CompileMappingFor(allEntities);
        }

        public static Configuration CreateConfig<TId>(Type rootEntityType, Action<Configuration> configure)
        {
            var mappings = CreateConventionalMappings<TId>(rootEntityType);
            return CreateConfig(mappings, configure);
        }

        public static Configuration CreateConfig(HbmMapping mappings, Action<Configuration> configure)
        {
            var config = new Configuration();
            configure(config);

            config.AddDeserializedMapping(mappings, "DomainMapping");

            return config;
        }
    }
}
