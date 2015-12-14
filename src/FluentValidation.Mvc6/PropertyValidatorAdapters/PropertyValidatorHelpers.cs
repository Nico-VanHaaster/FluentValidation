using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentValidation.Mvc6.PropertyValidatorAdapters
{
    internal static class PropertyValidatorHelpers
    {
        readonly static Lazy<Type> _messagesLazyType = new Lazy<Type>(() => Type.GetType("FluentValidation.Resources.Messages, FluentValidation"));

        public static Type MessagesType { get { return _messagesLazyType.Value; } }

    }
}
