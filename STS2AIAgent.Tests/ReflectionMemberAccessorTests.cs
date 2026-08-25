using STS2AIAgent.Game;

namespace STS2AIAgent.Tests;

internal static class ReflectionMemberAccessorTests
{
    public static void ReadsPrivateBaseFieldFromDerivedInstance()
    {
        var instance = new DerivedFixture();

        var value = ReflectionMemberAccessor.TryGetValue(
            instance, "_baseField", out var declaringType);

        Assert.Equal("base-field", value as string);
        Assert.Equal(typeof(BaseFixture), declaringType);
    }

    public static void ReadsPrivateBasePropertyFromDerivedInstance()
    {
        var instance = new DerivedFixture();

        var value = ReflectionMemberAccessor.TryGetValue(
            instance, "BaseProperty", out var declaringType);

        Assert.Equal("base-property", value as string);
        Assert.Equal(typeof(BaseFixture), declaringType);
    }

    public static void PrefersDerivedMemberWithSameName()
    {
        var instance = new DerivedFixture();

        var value = ReflectionMemberAccessor.TryGetValue(
            instance, "_shadowed", out var declaringType);

        Assert.Equal("derived", value as string);
        Assert.Equal(typeof(DerivedFixture), declaringType);
    }

    public static void DoesNotFallBackWhenDerivedGetterThrows()
    {
        var instance = new DerivedFixture();

        var value = ReflectionMemberAccessor.TryGetValue(
            instance, "_throwing", out var declaringType);

        Assert.Null(value);
        Assert.Equal(typeof(DerivedFixture), declaringType);
    }

    private class BaseFixture
    {
        private readonly string _baseField = "base-field";
        private readonly string _shadowed = "base";
        private readonly string _throwing = "base";
        private string BaseProperty => "base-property";

        public string KeepFieldsReferenced() => _baseField + _shadowed + _throwing;
    }

    private sealed class DerivedFixture : BaseFixture
    {
        private readonly string _shadowed = "derived";
        private string _throwing => throw new InvalidOperationException("fixture getter failure");

        public string KeepFieldReferenced() => _shadowed;
    }
}
