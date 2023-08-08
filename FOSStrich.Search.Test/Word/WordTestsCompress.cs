﻿namespace FOSStrich.Search;

[TestClass]
public class WordTestsCompress
{
    [TestMethod]
    public void Compressible()
    {
        var word = new Word(0);
        word.IsCompressible.Should().BeTrue();
        word.Compress();
        word.Raw.Should().Be(0x80000001);
        word.IsCompressed.Should().BeTrue();
        word.FillBit.Should().BeFalse();
        word.FillCount.Should().Be(1);

        word = new Word(Word.COMPRESSIBLEMASK);
        word.IsCompressible.Should().BeTrue();
        word.Compress();
        word.Raw.Should().Be(0xC0000001);
        word.IsCompressed.Should().BeTrue();
        word.FillBit.Should().BeTrue();
        word.FillCount.Should().Be(1);
    }

    [TestMethod]
    public void NotCompressible()
    {
        foreach (uint wordValue in new uint[] { 0x00000001u, 0x40000000u, 0x7FFFFFFEu, 0x3FFFFFFFu, 0x12345678u, 0x7FEDCBA9u })
        {
            var word = new Word(wordValue);
            word.Compress();
            word.Raw.Should().Be(wordValue);
            word.IsCompressible.Should().BeFalse();
            word.IsCompressed.Should().BeFalse();
        }
    }

    [TestMethod]
    public void NotCompressibleFullCoverage()
    {
        for (uint i = 1; i < 0x7FFFFFFF; i += WordExtensions.LARGEPRIME)
        {
            var word = new Word(i);
            word.Compress();
            word.Raw.Should().Be(i);
            word.IsCompressible.Should().BeFalse();
            word.IsCompressed.Should().BeFalse();
        }
    }

    [TestMethod]
    public void CompressedFullCoverage()
    {
        for (uint i = 0x80000000; i > 0x80000000 && i <= 0xFFFFFFFF; i += WordExtensions.LARGEPRIME)
        {
            var word = new Word(i);
            word.Compress();
            word.Raw.Should().Be(i, because: word.ToString());
            word.IsCompressed.Should().BeTrue(because: word.ToString());
        }
    }

    [TestMethod]
    public void Pack()
    {
        var word = new Word(true, 1);

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsFalse(word.HasPackedWord);

        word.Pack(new Word(1));

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsTrue(word.HasPackedWord);
        word.PackedPosition.Should().Be(30);
        word.PackedWord.Raw.Should().Be((uint)1);

        word = new Word(true, 1);

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsFalse(word.HasPackedWord);

        word.Pack(new Word(1 << 30));

        Assert.IsTrue(word.IsCompressed);
        Assert.IsTrue(word.FillBit);
        word.FillCount.Should().Be(1);
        Assert.IsTrue(word.HasPackedWord);
        word.PackedPosition.Should().Be(0);
        word.PackedWord.Raw.Should().Be((uint)1 << 30);
    }

    [TestMethod]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var word = new Word(0);
            int packedPositions = word.PackedPosition;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackedPositionNotSupported");

        act = () =>
        {
            var word = new Word(0);
            Word packedWord = word.PackedWord;
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackedWordNotSupported");

        act = () =>
        {
            var word = new Word(0);
            word.Pack(new Word(1));
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackNotSupported1");

        act = () =>
        {
            var word = new Word(true, 1);
            word.Pack(new Word(1));
        };
        act.Should().NotThrow(because: "PackNotSupported2OK");

        act = () =>
        {
            var word = new Word(true, 1);
            word.Pack(new Word(1));
            word.Pack(new Word(1));
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackNotSupported2");

        act = () =>
        {
            var word = new Word(true, 1);
            word.Pack(new Word(true, 1));
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackNotSupported3");

        act = () =>
        {
            var word = new Word(true, 1);
            word.Pack(new Word(0));
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackNotSupported4_1");

        act = () =>
        {
            var word = new Word(true, 1);
            word.Pack(new Word(3));
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "PackNotSupported4_2");
    }
}