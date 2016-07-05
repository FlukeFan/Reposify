using System;
using System.Collections.Generic;

namespace Reposify.Testing
{
    public class CustomInspections
    {
        private static CustomInspections _customInspections = new CustomInspections();

        public static void Add<T>(Action<ConsistencyInspector, T> inspection)
        {
            _customInspections.AddInspection(inspection);
        }

        public static void Inspect(object entity, ConsistencyInspector validator)
        {
            _customInspections.InspectEntity(entity, validator);
        }

        private class InspectorTree : Dictionary<Type, InspectorTree>
        {
            public Action<ConsistencyInspector, object> Inspection;
        }

        private InspectorTree _inspectorTree = new InspectorTree();

        public void AddInspection<T>(Action<ConsistencyInspector, T> inspection)
        {
            var node = FindOrCreateNode(typeof(T));
            node.Inspection = (v, e) => inspection(v, (T)e);
        }

        public void InspectEntity(object entity)
        {
            InspectEntity(entity, new ConsistencyInspector());
        }

        public void InspectEntity(object entity, ConsistencyInspector inspector)
        {
            var type = entity.GetType();
            InspectEntity(type, entity, inspector);
        }

        private InspectorTree InspectEntity(Type type, object entity, ConsistencyInspector inspector)
        {
            if (type == null)
                return _inspectorTree;

            var node = InspectEntity(type.BaseType, entity, inspector);

            if (node == null || !node.ContainsKey(type))
                return null;

            var child = node[type];

            if (child.Inspection != null)
                child.Inspection(inspector, entity);

            return child;
        }

        private InspectorTree FindOrCreateNode(Type type)
        {
            if (type == null)
                return _inspectorTree;

            var node = FindOrCreateNode(type.BaseType);

            if (!node.ContainsKey(type))
                node.Add(type, new InspectorTree());

            return node[type];
        }
    }
}
