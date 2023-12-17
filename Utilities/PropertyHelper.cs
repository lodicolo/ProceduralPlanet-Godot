namespace ProceduralPlanet.Utilities;

public static class PropertyHelper
{
    public static bool SetIfChanged<T>(ref T backingField, T value) => SetIfChanged(ref backingField, value, out _);

    public static bool SetIfChanged<T>(ref T backingField, T value, out T? lastValue)
    {
        lastValue = backingField;

        if (Equals(backingField, value))
        {
            return false;
        }

        backingField = value;
        return true;
    }

    public static bool SetIfChanged<T>(ref T backingField, T value, ref bool dirty) =>
        SetIfChanged(ref backingField, value, ref dirty, out _);

    public static bool SetIfChanged<T>(ref T backingField, T value, ref bool dirty, out T? lastValue)
    {
        dirty = SetIfChanged(ref backingField, value, out lastValue);
        return dirty;
    }
}