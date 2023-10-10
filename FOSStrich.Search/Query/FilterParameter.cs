﻿namespace FOSStrich.Search;

using FOSStrich.BitVectors;
using System.Collections;

public static class FilterParameter
{
    public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, TKey exact)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, exact);

    public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, IEnumerable<TKey> enumerable)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, enumerable);

    public static FilterParameter<TKey> Create<TKey>(ICatalogHandle<TKey> catalog, TKey rangeMin, TKey rangeMax)
            where TKey : IEquatable<TKey>, IComparable<TKey> =>
        new(catalog, rangeMin, rangeMax);

    internal static IFilterParameter Create<TBitVector, TItem, TPrimaryKey>(Engine<TBitVector, TItem, TPrimaryKey> engine, string catalogName, object exact)
            where TBitVector : IBitVector<TBitVector> =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(exact));

    internal static IFilterParameter Create<TBitVector, TItem, TPrimaryKey>(Engine<TBitVector, TItem, TPrimaryKey> engine, string catalogName, IEnumerable enumerable)
            where TBitVector : IBitVector<TBitVector> =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(enumerable));

    internal static IFilterParameter Create<TBitVector, TItem, TPrimaryKey>(Engine<TBitVector, TItem, TPrimaryKey> engine, string catalogName, object rangeMin, object rangeMax)
            where TBitVector : IBitVector<TBitVector> =>
        ParameterHelper.CreateLooselyTyped(engine, catalogName, catalog => catalog.CreateFilterParameter(rangeMin, rangeMax));
}

public sealed class FilterParameter<TKey> : FilterClause, IFilterParameter
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    internal FilterParameter(ICatalogHandle<TKey> catalog, TKey exact)
        : this(catalog, FilterParameterType.Exact, exact: exact)
    { }

    internal FilterParameter(ICatalogHandle<TKey> catalog, IEnumerable<TKey> enumerable)
        : this(catalog, FilterParameterType.Enumerable, enumerable: enumerable)
    { }

    internal FilterParameter(ICatalogHandle<TKey> catalog, TKey rangeMin, TKey rangeMax)
        : this(catalog, FilterParameterType.Range, rangeMin: rangeMin, rangeMax: rangeMax)
    { }

    private FilterParameter(ICatalogHandle<TKey> catalog, FilterParameterType parameterType,
        TKey exact = default, IEnumerable<TKey> enumerable = null, TKey rangeMin = default, TKey rangeMax = default)
    {
        if (parameterType == FilterParameterType.Range)
        {
            if (rangeMin == null && rangeMax == null)
                throw new ArgumentNullException(nameof(rangeMin), "Either rangeMin or rangeMax must be non-null.");

            if (rangeMin != null && rangeMax != null && rangeMin.CompareTo(rangeMax) > 0)
                throw new ArgumentOutOfRangeException(nameof(rangeMin), "rangeMin must be <= rangeMax.");
        }

        Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        ParameterType = parameterType;
        Exact = exact;
        Enumerable = enumerable;
        RangeMin = rangeMin;
        RangeMax = rangeMax;
    }

    public ICatalogHandle Catalog { get; }
    public FilterParameterType ParameterType { get; }
    public TKey Exact { get; }
    public IEnumerable<TKey> Enumerable { get; }
    public TKey RangeMin { get; }
    public TKey RangeMax { get; }

    #region IFilterParameter

    object IFilterParameter.Exact => Exact;
    IEnumerable IFilterParameter.Enumerable => Enumerable;
    object IFilterParameter.RangeMin => RangeMin;
    object IFilterParameter.RangeMax => RangeMax;

    #endregion
}

public interface IFilterParameter : IParameter
{
    FilterParameterType ParameterType { get; }
    object Exact { get; }
    IEnumerable Enumerable { get; }
    object RangeMin { get; }
    object RangeMax { get; }
}

public enum FilterParameterType
{
    Exact,
    Enumerable,
    Range
}