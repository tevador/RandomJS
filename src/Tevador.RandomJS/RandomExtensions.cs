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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tevador.RandomJS.Expressions;

namespace Tevador.RandomJS
{
    static class RandomExtensions
    {
        public static readonly string PrintableChars = "\t\n !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        public static readonly string HexChars = "0123456789abcdef";
        public static readonly string DecimalChars = "0123456789";
        public static readonly string OctalChars = "01234567";
        public static readonly string BinaryChars = "01";

        public static int GenInt(this IRandom rand, int max)
        {
            return (int)(rand.Gen() * max);
        }

        public static int GenInt(this IRandom rand, int min, int max)
        {
            return min + rand.GenInt(max - min);
        }

        public static uint GenUInt(this IRandom rand)
        {
            return (uint)(rand.Gen() * 4294967296.0);
        }

        public static bool FlipCoin(this IRandom rand)
        {
            return rand.FlipCoin(0.5);
        }

        public static bool FlipCoin(this IRandom rand, double chance)
        {
            return rand.Gen() < chance;
        }

        public static void GenString(this IRandom rand, StringBuilder sb, int length, string charset, bool canStartWithZero = true)
        {
            if (!canStartWithZero)
            {
                char c = '\0';
                while (length-- > 0 && (c = charset[rand.GenInt(charset.Length)]) == '0');
                sb.Append(c);
            }
            while (length-- > 0)
            {
                sb.Append(charset[rand.GenInt(charset.Length)]);
            }
        }

        public static string GenStringLiteral(this IRandom rand, int length)
        {
            char quote = rand.FlipCoin() ? '\'' : '"';
            var sb = new StringBuilder(length * 2);
            sb.Append(quote);
            while (sb.Length < length)
            {
                char c = PrintableChars[rand.GenInt(PrintableChars.Length)];
                if (c == '\n')
                {
                    sb.Append("\\n");
                    continue;
                }
                if (c == '\t')
                {
                    sb.Append("\\t");
                    continue;
                }
                if(c == quote || c == '\\')
                {
                    sb.Append('\\');
                }
                sb.Append(c);
            }
            sb.Append(quote);
            return sb.ToString();
        }

        public static void Shuffle<T>(this IRandom rand, IList<T> items)
        {
            for (int i = items.Count - 1; i >= 1; --i)
            {
                int j = rand.GenInt(i + 1);
                //swap
                var temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public static List<T> Choose<T>(this IRandom rand, int count, IList<T> items)
        {
            var selection = new List<T>(count);
            for (int i = 0; i < count; ++i)
            {
                selection[i] = rand.Choose(items);
            }
            return selection;
        }

        public static T Choose<T>(this IRandom rand, IList<T> items)
        {
            return items[rand.GenInt(items.Count)];
        }

        public static T Choose<T>(this IRandom rand, IEnumerable<T> items, int count)
        {
            return items.ElementAt(rand.GenInt(count));
        }

        public static bool Has(this VariableOptions options, VariableOptions vo)
        {
            return (options & vo) != 0;
        }

        public static Variable ChooseVariable(this IRandom rand, IScope scope, VariableOptions options = VariableOptions.None)
        {
            IEnumerable<Variable> candidates;
            if (options.Has(VariableOptions.ForWriting))
            {
                candidates = scope.Variables
                    .Where(
                        var =>
                            (var.IsParameter || !options.Has(VariableOptions.ParametersOnly)) &&
                            !var.IsConstant &&
                            (options.Has(VariableOptions.NonFunctionInitializer) || !(var.Initializer is FunctionExpression)));
            }
            else if (options.Has(VariableOptions.ParametersOnly))
            {
                candidates = scope.Variables.Where(var => var.IsParameter);
            }
            else
            {
                candidates = scope.Variables;
            }
            var count = candidates.Count();
            if (count > 0)
            {
                return rand.Choose(candidates, count);
            }
            return null;
        }
    }
}
