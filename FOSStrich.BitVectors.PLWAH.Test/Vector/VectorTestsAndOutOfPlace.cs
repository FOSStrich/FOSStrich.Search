﻿#if POSITIONLISTENABLED
namespace FOSStrich.BitVectors.PLWAH;
#else
namespace FOSStrich.BitVectors.WAH;
#endif

public class VectorTestsAndOutOfPlace
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    public void AndOutOfPlaceRandom(bool leftIsCompressed, bool rightIsCompressed, bool resultIsCompressed)
    {
        const int randomSeed = 22;

        VectorTestsRandom.LogicOutOfPlaceBase(randomSeed, (Word.SIZE - 1) * 10 + 1,
            leftIsCompressed, rightIsCompressed,
            (left, right) => left.AndOutOfPlace(right, resultIsCompressed), Enumerable.Intersect);
    }
}