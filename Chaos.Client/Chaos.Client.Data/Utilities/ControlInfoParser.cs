using System.Text;
using Chaos.Client.Data.Models;
using Chaos.Extensions.Common;
using Chaos.Geometry;

namespace Chaos.Client.Data.Utilities;

public sealed class ControlInfoParser
{
    private TokenType CurrentToken;

    public UserControlInfo Parse(string name, Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.Default, true);
        var userControlInfo = new UserControlInfo(name);
        ControlInfo? currentControl = null;

        while (reader.ReadLine() is { } line)
        {
            var token = ReadToken(line, out var value);

            if ((currentControl == null) && (token != TokenType.Control))
                throw new InvalidOperationException("Invalid control file");

            //if the token was parsed by itself with no value
            if (string.IsNullOrEmpty(value))
            {
                switch (token)
                {
                    case TokenType.Control:
                    {
                        currentControl = new ControlInfo();

                        break;
                    }
                    case TokenType.EndControl:
                    {
                        userControlInfo.Add(currentControl!);
                        currentControl = null;

                        break;
                    }
                    case TokenType.Color:
                        break;
                    case TokenType.Value:
                        break;
                    case TokenType.Image:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                continue;
            }

            //if the currentControl is null, something is wrong
            if (currentControl == null)
                throw new InvalidOperationException("Invalid control file");

            //if the current token was parsed with a value, set that value on the current control
            switch (token)
            {
                case TokenType.Name:
                {
                    currentControl.Name = value;

                    break;
                }
                case TokenType.Type:
                {
                    currentControl.Type = (ControlType)int.Parse(value);

                    break;
                }
                case TokenType.Rect:
                {
                    var parts = value.Split(' ');

                    var left = int.Parse(parts[0]);
                    var top = int.Parse(parts[1]);
                    var right = int.Parse(parts[2]);
                    var bottom = int.Parse(parts[3]);

                    currentControl.Rect = new Rectangle(
                        left,
                        top,
                        right - left,
                        bottom - top);

                    break;
                }

                //current means no token was parsed, and any values should be considered as part of the CurrentToken
                case TokenType.Current:
                    switch (CurrentToken)
                    {
                        case TokenType.Color:
                        {
                            currentControl.ColorIndexes.Add(int.Parse(value));

                            break;
                        }
                        case TokenType.Value:
                        {
                            currentControl.ButtonResultValue = int.Parse(value);

                            break;
                        }
                        case TokenType.Image:
                        {
                            var parts = value.Split(' ');

                            var imageName = parts[0]
                                .Trim('"');
                            var paletteNum = int.Parse(parts[1]);

                            currentControl.Images.Add((imageName, paletteNum));

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (token != TokenType.Current)
                CurrentToken = token;
        }

        return userControlInfo;
    }

    private TokenType ReadToken(string line, out string? value)
    {
        value = null;
        line = line.Trim();

        if (line.StartsWithI("<CONTROL>"))
            return TokenType.Control;

        if (line.StartsWithI("<ENDCONTROL>"))
            return TokenType.EndControl;

        if (line.StartsWithI("<NAME>"))
        {
            value = line[8..^1];

            return TokenType.Name;
        }

        if (line.StartsWithI("<TYPE>"))
        {
            value = line[7..];

            return TokenType.Type;
        }

        if (line.StartsWithI("<RECT>"))
        {
            value = line[7..];

            return TokenType.Rect;
        }

        if (line.StartsWithI("<COLOR>"))
            return TokenType.Color;

        if (line.StartsWithI("<VALUE>"))
            return TokenType.Value;

        if (line.StartsWithI("<IMAGE>"))
            return TokenType.Image;

        value = line;

        return TokenType.Current;
    }

    private enum TokenType
    {
        Current = 0,
        Control = 1,
        EndControl = 2,
        Name = 3,
        Type = 4,
        Rect = 5,
        Color = 6,
        Value = 7,
        Image = 8
    }
}