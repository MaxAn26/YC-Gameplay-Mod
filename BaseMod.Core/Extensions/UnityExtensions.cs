using System.Diagnostics.CodeAnalysis;

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;

using UnityEngine;

namespace BaseMod.Core.Extensions;
public static class UnityExtensions {
    public static T? GetComponentWithCast<T>(this GameObject gameObject)
        where T : Il2CppObjectBase {

        T? result = gameObject?.GetComponent( Il2CppType.From( typeof( T ) ) )?.TryCast<T>();
        if (result is not null)
            return result;

        result = gameObject?.GetComponentInChildren(Il2CppType.From(typeof(T)))?.TryCast<T>();
        if (result is not null)
            return result;

        return gameObject?.GetComponentInParent(Il2CppType.From(typeof(T)))?.TryCast<T>();
    }

    public static bool TryGetComponentWithCast<T>(this GameObject gameObject, [NotNullWhen(true)] out T? result)
        where T : Il2CppObjectBase {
        result = gameObject.GetComponentWithCast<T>();

        return result is not null;
    }
}