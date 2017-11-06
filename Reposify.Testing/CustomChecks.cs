using System;
using System.Collections.Generic;

namespace Reposify.Testing
{
    public class CustomChecks
    {
        private static CustomChecks _customChecks = new CustomChecks();

        public static void Add<T>(Action<ConstraintChecker, T> checker)
        {
            _customChecks.AddChecker(checker);
        }

        public static void Check(object entity, ConstraintChecker checker)
        {
            _customChecks.CheckEntity(entity, checker);
        }

        private class CheckerTree : Dictionary<Type, CheckerTree>
        {
            public Action<ConstraintChecker, object> Check;
        }

        private CheckerTree _checkerTree = new CheckerTree();

        public void AddChecker<T>(Action<ConstraintChecker, T> checker)
        {
            var node = FindOrCreateNode(typeof(T));
            node.Check = (v, e) => checker(v, (T)e);
        }

        public void CheckEntity(object entity)
        {
            CheckEntity(entity, new ConstraintChecker());
        }

        public void CheckEntity(object entity, ConstraintChecker checker)
        {
            var type = entity.GetType();
            CheckEntity(type, entity, checker);
        }

        private CheckerTree CheckEntity(Type type, object entity, ConstraintChecker checker)
        {
            if (type == null)
                return _checkerTree;

            var node = CheckEntity(type.BaseType, entity, checker);

            if (node == null || !node.ContainsKey(type))
                return null;

            var child = node[type];

            if (child.Check != null)
                child.Check(checker, entity);

            return child;
        }

        private CheckerTree FindOrCreateNode(Type type)
        {
            if (type == null)
                return _checkerTree;

            var node = FindOrCreateNode(type.BaseType);

            if (!node.ContainsKey(type))
                node.Add(type, new CheckerTree());

            return node[type];
        }
    }
}
