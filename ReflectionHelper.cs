using sorceryFight.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace sorceryFight
{
    [Obsolete($"This is kept for {nameof(GeneralParticleHandler)}. Consider use ILoadable or ModType If possible.")]
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetEveryModsTypes()
        {
            return ModLoader.Mods.SelectMany(mod => AssemblyManager.GetLoadableTypes(mod.Code));
        }

        public static bool IsSubclass(Type baseType, Type type, bool includeBaseType)
        {
            return type.IsSubclassOf(baseType) && !type.IsAbstract && (!includeBaseType && type != baseType);
        }

        // TODO: Consider replacing with ModType?
        public static void IterateEveryModsTypes<T>(Action<Type> action, bool includeBaseType = false)
        {
            // WHY????
            if (action is null)
                return;

            Type baseType = typeof(T);
            var types = GetEveryModsTypes().Where(t => IsSubclass(baseType, t, includeBaseType));
            foreach (var type in types)
            {
                action.Invoke(type);
            }
        }
    }
}
