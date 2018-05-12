/*
    (c) 2018 tevador <tevador@gmail.com>

    This file is part of Tevador.RandomJS.

    Tevador.RandomJS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Tevador.RandomJS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Tevador.RandomJS.  If not, see<http://www.gnu.org/licenses/>.
*/

using System.Text;

namespace Tevador.RandomJS.Expressions
{
    class NumericLiteral : Literal
    {
        public static new NumericLiteral Generate(IRandom rand, IScope scope)
        {
            return Generate(rand, scope.Options.NumericLiterals.ChooseRandom(rand));
        }

        public static NumericLiteral Generate(IRandom rand, NumericLiteralType type)
        {
            StringBuilder sb = new StringBuilder(35);
            switch (type)
            {
                case NumericLiteralType.Boolean:
                    if (rand.FlipCoin())
                        sb.Append("true");
                    else
                        sb.Append("false");
                    break;

                case NumericLiteralType.SmallInteger:
                    sb.Append(rand.GenInt(-40, 40));
                    break;

                case NumericLiteralType.BinaryInteger:
                    if (rand.FlipCoin()) sb.Append('-');
                    sb.Append("0b");
                    rand.GenString(sb, 32, RandomExtensions.BinaryChars);
                    break;

                case NumericLiteralType.OctalInteger:
                    if (rand.FlipCoin()) sb.Append('-');
                    sb.Append("0o");
                    rand.GenString(sb, 10, RandomExtensions.OctalChars);
                    break;

                case NumericLiteralType.HexInteger:
                    if (rand.FlipCoin()) sb.Append('-');
                    sb.Append("0x");
                    rand.GenString(sb, 8, RandomExtensions.HexChars);
                    break;

                case NumericLiteralType.FixedFloat:
                    if (rand.FlipCoin()) sb.Append('-');
                    rand.GenString(sb, 5, RandomExtensions.DecimalChars, false);
                    sb.Append('.');
                    rand.GenString(sb, 5, RandomExtensions.DecimalChars, true);
                    break;

                case NumericLiteralType.ExpFloat:
                    if (rand.FlipCoin()) sb.Append('-');
                    sb.Append(rand.GenInt(0, 10));
                    sb.Append('.');
                    rand.GenString(sb, 5, RandomExtensions.DecimalChars, true);
                    sb.Append('e');
                    sb.Append(rand.GenInt(-40, 40));
                    break;

                default:
                    if (rand.FlipCoin()) sb.Append('-');
                    rand.GenString(sb, 9, RandomExtensions.DecimalChars);
                    break;
            }
            return new NumericLiteral { Value = sb.ToString() };
        }
    }
}
