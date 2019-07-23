using CloudNimble.SimpleMessageBus.Core;

namespace System
{

    /// <summary>
    /// Extension methods to augment <see cref="System.Type"/>.
    /// </summary>
    public static class TypeExtensions
    {

        /// <summary>
        /// Guarantees the creation of an AssemblyQualifiedName that does not contain version or key details. That way when AssemblyVersions are incremented,
        /// the system will still attempt to process the <see cref="IMessage"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A string containing the <paramref name="type"/> name in the format "FullTypeName, SimpleAssemblyName".</returns>
        public static string SimpleAssemblyQualifiedName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }

    }

}