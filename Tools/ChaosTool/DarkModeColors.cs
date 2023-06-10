using System.Collections.Immutable;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis.Classification;
using RoslynPad.Editor;

namespace ChaosTool;

internal sealed class DarkModeColors : IClassificationHighlightColors
{
    private readonly HighlightingColor BraceHighlightBrush;
    private readonly ImmutableDictionary<string, HighlightingColor> Colors;
    private readonly HighlightingColor CommentBrush;
    private readonly HighlightingColor EscapedStringBrush;
    private readonly HighlightingColor KeywordBrush;
    private readonly HighlightingColor MemberBrush;
    private readonly HighlightingColor MethodBrush;
    private readonly HighlightingColor NumberBrush;
    private readonly HighlightingColor PunctuationBrush;
    private readonly HighlightingColor ReferenceTypeBrush;
    private readonly HighlightingColor StringBrush;
    private readonly HighlightingColor UnknownBrush;

    /// <inheritdoc />
    public HighlightingColor DefaultBrush { get; }

    internal DarkModeColors()
    {
        DefaultBrush = GetHighlightingColor("#FF86DBFD");
        StringBrush = GetHighlightingColor("#FFD59C74");
        KeywordBrush = GetHighlightingColor("#FF569CD6");
        ReferenceTypeBrush = GetHighlightingColor("#FF4EC9B0");
        PunctuationBrush = GetHighlightingColor("#FFFFFFFF");
        CommentBrush = GetHighlightingColor("#FF4DC883");
        UnknownBrush = GetHighlightingColor("#FFFC141F");
        MethodBrush = GetHighlightingColor("#FFDCDCAA");
        NumberBrush = GetHighlightingColor("#FFB5CEA8");
        MemberBrush = GetHighlightingColor("#FFFFFFFF");
        EscapedStringBrush = GetHighlightingColor("#FFDF7902");
        BraceHighlightBrush = GetHighlightingColor("#FF0E4583");

        var colors = new Dictionary<string, HighlightingColor>
        {
            [ClassificationTypeNames.Comment] = CommentBrush,
            [ClassificationTypeNames.ExcludedCode] = UnknownBrush,
            [ClassificationTypeNames.Identifier] = MemberBrush,
            [ClassificationTypeNames.Keyword] = KeywordBrush,
            [ClassificationTypeNames.ControlKeyword] = KeywordBrush,
            [ClassificationTypeNames.NumericLiteral] = NumberBrush,
            [ClassificationTypeNames.Operator] = PunctuationBrush,
            [ClassificationTypeNames.OperatorOverloaded] = PunctuationBrush,
            [ClassificationTypeNames.PreprocessorKeyword] = UnknownBrush,
            [ClassificationTypeNames.StringLiteral] = StringBrush,
            [ClassificationTypeNames.WhiteSpace] = UnknownBrush,
            [ClassificationTypeNames.Text] = StringBrush,
            [ClassificationTypeNames.StaticSymbol] = MemberBrush,
            [ClassificationTypeNames.PreprocessorText] = UnknownBrush,
            [ClassificationTypeNames.Punctuation] = PunctuationBrush,
            [ClassificationTypeNames.VerbatimStringLiteral] = StringBrush,
            [ClassificationTypeNames.StringEscapeCharacter] = EscapedStringBrush,
            [ClassificationTypeNames.ClassName] = ReferenceTypeBrush,
            [ClassificationTypeNames.RecordClassName] = ReferenceTypeBrush,
            [ClassificationTypeNames.DelegateName] = ReferenceTypeBrush,
            [ClassificationTypeNames.EnumName] = NumberBrush,
            [ClassificationTypeNames.InterfaceName] = NumberBrush,
            [ClassificationTypeNames.ModuleName] = UnknownBrush,
            [ClassificationTypeNames.StructName] = KeywordBrush,
            [ClassificationTypeNames.RecordStructName] = KeywordBrush,
            [ClassificationTypeNames.TypeParameterName] = ReferenceTypeBrush,
            [ClassificationTypeNames.FieldName] = MemberBrush,
            [ClassificationTypeNames.EnumMemberName] = MemberBrush,
            [ClassificationTypeNames.ConstantName] = MemberBrush,
            [ClassificationTypeNames.LocalName] = DefaultBrush,
            [ClassificationTypeNames.ParameterName] = DefaultBrush,
            [ClassificationTypeNames.MethodName] = MethodBrush,
            [ClassificationTypeNames.ExtensionMethodName] = MethodBrush,
            [ClassificationTypeNames.PropertyName] = MemberBrush,
            [ClassificationTypeNames.EventName] = MemberBrush,
            [ClassificationTypeNames.NamespaceName] = MemberBrush,
            [ClassificationTypeNames.LabelName] = MemberBrush,
            [BraceMatcherHighlightRenderer.BracketHighlight] = BraceHighlightBrush,
            [ClassificationTypeNames.XmlDocCommentAttributeName] = MemberBrush,
            [ClassificationTypeNames.XmlDocCommentAttributeQuotes] = StringBrush,
            [ClassificationTypeNames.XmlDocCommentAttributeValue] = StringBrush,
            [ClassificationTypeNames.XmlDocCommentCDataSection] = CommentBrush,
            [ClassificationTypeNames.XmlDocCommentComment] = CommentBrush,
            [ClassificationTypeNames.XmlDocCommentDelimiter] = PunctuationBrush,
            [ClassificationTypeNames.XmlDocCommentEntityReference] = ReferenceTypeBrush,
            [ClassificationTypeNames.XmlDocCommentName] = MemberBrush,
            [ClassificationTypeNames.XmlDocCommentProcessingInstruction] = CommentBrush,
            [ClassificationTypeNames.XmlDocCommentText] = StringBrush,
            [ClassificationTypeNames.XmlLiteralAttributeName] = MemberBrush,
            [ClassificationTypeNames.XmlLiteralAttributeQuotes] = StringBrush,
            [ClassificationTypeNames.XmlLiteralAttributeValue] = StringBrush,
            [ClassificationTypeNames.XmlLiteralCDataSection] = CommentBrush,
            [ClassificationTypeNames.XmlLiteralComment] = CommentBrush,
            [ClassificationTypeNames.XmlLiteralDelimiter] = PunctuationBrush,
            [ClassificationTypeNames.XmlLiteralEmbeddedExpression] = CommentBrush,
            [ClassificationTypeNames.XmlLiteralEntityReference] = ReferenceTypeBrush,
            [ClassificationTypeNames.XmlLiteralName] = MemberBrush

            /*
                         XmlDocCommentAttributeName,
            XmlDocCommentAttributeQuotes,
            XmlDocCommentAttributeValue,
            XmlDocCommentCDataSection,
            XmlDocCommentComment,
            XmlDocCommentDelimiter,
            XmlDocCommentEntityReference,
            XmlDocCommentName,
            XmlDocCommentProcessingInstruction,
            XmlDocCommentText,
            XmlLiteralAttributeName,
            XmlLiteralAttributeQuotes,
            XmlLiteralAttributeValue,
            XmlLiteralCDataSection,
            XmlLiteralComment,
            XmlLiteralDelimiter,
            XmlLiteralEmbeddedExpression,
            XmlLiteralEntityReference,
            XmlLiteralName,
            XmlLiteralProcessingInstruction,
            XmlLiteralText,
            RegexComment,
            RegexCharacterClass,
            RegexAnchor,
            RegexQuantifier,
            RegexGrouping,
            RegexAlternation,
            RegexText,
            RegexSelfEscapedCharacter,
            RegexOtherEscape,
            JsonComment,
            JsonNumber,
            JsonString,
            JsonKeyword,
            JsonText,
            JsonOperator,
            JsonPunctuation,
            JsonArray,
            JsonObject,
            JsonPropertyName,
            JsonConstructorName
             */
        };

        Colors = colors.ToImmutableDictionary();
    }

    /// <inheritdoc />
    public HighlightingColor GetBrush(string classificationTypeName)
    {
        if (Colors.TryGetValue(classificationTypeName, out var color))
            return color;

        return DefaultBrush;
    }

    private static Color GetColor(string str) => (Color)ColorConverter.ConvertFromString(str);

    private static HighlightingColor GetHighlightingColor(string str)
    {
        var color = new HighlightingColor
        {
            Foreground = new SimpleHighlightingBrush(GetColor(str))
        };

        if (!color.IsFrozen)
            color.Freeze();

        return color;
    }
}