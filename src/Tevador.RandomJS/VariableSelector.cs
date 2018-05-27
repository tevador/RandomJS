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
using Tevador.RandomJS.Expressions;

namespace Tevador.RandomJS
{
    class VariableSelector
    {
        IRandom _rand;
        List<Variable> _candidates = new List<Variable>(50);
        int _scopeFactor;

        public VariableSelector(IRandom rand, int scopeFactor)
        {
            _rand = rand;
            _scopeFactor = scopeFactor;
        }

        private void PrepareForWriting(IScope scope, VariableOptions options)
        {
            foreach(Variable v in scope.Variables)
            {
                if (v.IsLoopCounter || v.IsConstant)
                    continue;
                if (options.Has(VariableOptions.NonFunctionInitializer) && (v.Initializer is FunctionExpression))
                    continue;
                for (int i = 0; i <= _scopeFactor * v.Parent.FunctionDepth; ++i)
                    _candidates.Add(v);
            }
        }

        private void PrepareForReading(IScope scope, VariableOptions options)
        {
            foreach (Variable v in scope.Variables)
            {
                for (int i = 0; i <= _scopeFactor * v.Parent.FunctionDepth; ++i)
                    _candidates.Add(v);
            }
        }

        public Variable ChooseVariable(IScope scope, VariableOptions options = VariableOptions.None)
        {
            _candidates.Clear();
            if (options.Has(VariableOptions.ForWriting))
            {
                PrepareForWriting(scope, options);
            }
            else
            {
                PrepareForReading(scope, options);
            }
            var count = _candidates.Count;
            if (count > 0)
            {
                return _rand.Choose(_candidates);
            }
            return null;
        }
    }
}
