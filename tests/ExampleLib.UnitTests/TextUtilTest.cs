using Xunit;

namespace ExampleLib.UnitTests;

public class TextUtilTest
{
    [Fact]
    public void Can_extract_russian_words()
    {
        const string text = """
                            Играют волны — ветер свищет,
                            И мачта гнётся и скрыпит…
                            Увы! он счастия не ищет
                            И не от счастия бежит!
                            """;
        List<string> expected =
        [
            "Играют",
            "волны",
            "ветер",
            "свищет",
            "И",
            "мачта",
            "гнётся",
            "и",
            "скрыпит",
            "Увы",
            "он",
            "счастия",
            "не",
            "ищет",
            "И",
            "не",
            "от",
            "счастия",
            "бежит",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_hyphens()
    {
        const string text = "Что-нибудь да как-нибудь, и +/- что- то ещё";
        List<string> expected =
        [
            "Что-нибудь",
            "да",
            "как-нибудь",
            "и",
            "что",
            "то",
            "ещё",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_apostrophes()
    {
        const string text = "Children's toys and three cats' toys";
        List<string> expected =
        [
            "Children's",
            "toys",
            "and",
            "three",
            "cats'",
            "toys",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Can_extract_words_with_grave_accent()
    {
        const string text = "Children`s toys and three cats` toys, all of''them are green";
        List<string> expected =
        [
            "Children`s",
            "toys",
            "and",
            "three",
            "cats`",
            "toys",
            "all",
            "of'",
            "them",
            "are",
            "green",
        ];

        List<string> actual = TextUtil.ExtractWords(text);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, byte, byte, byte> ValidHexColors => new TheoryData<string, byte, byte, byte>
    {
        { "#FF0000", 255, 0, 0 },
        { "#00FF00", 0, 255, 0 },
        { "#0000FF", 0, 0, 255 },
        { "#FFFFFF", 255, 255, 255 },
        { "#000000", 0, 0, 0 },
    };

    [Theory]
    [MemberData(nameof(ValidHexColors))]
    public void ParseCssRbgColor_ValidInput_ReturnsExpectedRgb(string hex, byte expectedR, byte expectedG, byte expectedB)
    {
        TextUtil.RgbColor color = TextUtil.ParseCssRbgColor(hex);
        Assert.Equal(expectedR, color.R);
        Assert.Equal(expectedG, color.G);
        Assert.Equal(expectedB, color.B);
    }

    public static TheoryData<string> InvalidHexColors => new TheoryData<string>
    {
        "#ZZZZZZ",
        "#12345",
        "FF0000",
        "#1234567",
        ""
    };

    [Theory]
    [MemberData(nameof(InvalidHexColors))]
    public void ParseCssRbgColor_InvalidInput_ThrowsFormatException(string hex)
    {
        Assert.Throws<FormatException>(() => TextUtil.ParseCssRbgColor(hex));
    }
}