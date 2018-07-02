/*
(c) 2018 tevador <tevador@gmail.com>

This file is part of RandomJS.

RandomJS is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RandomJS is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RandomJS.  If not, see<http://www.gnu.org/licenses/>.
*/

#include "AssignmentOperator.h"
#include "BinaryOperator.h"
#include "UnaryOperator.h"

AssignmentOperator AssignmentOperator::Basic = AssignmentOperator("=");
AssignmentOperator AssignmentOperator::Add = AssignmentOperator("+=", OperatorRequirement::StringLengthLimit);
AssignmentOperator AssignmentOperator::Sub = AssignmentOperator("-=", OperatorRequirement::NumericOnly);
AssignmentOperator AssignmentOperator::Mul = AssignmentOperator("*=", OperatorRequirement::NumericOnly);
AssignmentOperator AssignmentOperator::Div = AssignmentOperator("/=", OperatorRequirement::NumericOnly | OperatorRequirement::RhsNonzero);
AssignmentOperator AssignmentOperator::Mod = AssignmentOperator("%=", OperatorRequirement::NumericOnly | OperatorRequirement::RhsNonzero);
AssignmentOperator AssignmentOperator::PreInc = AssignmentOperator("++", OperatorRequirement::NumericOnly | OperatorRequirement::Prefix | OperatorRequirement::WithoutRhs);
AssignmentOperator AssignmentOperator::PostInc = AssignmentOperator("++", OperatorRequirement::NumericOnly | OperatorRequirement::WithoutRhs);
AssignmentOperator AssignmentOperator::PreDec = AssignmentOperator("--", OperatorRequirement::NumericOnly | OperatorRequirement::Prefix | OperatorRequirement::WithoutRhs);
AssignmentOperator AssignmentOperator::PostDec = AssignmentOperator("--", OperatorRequirement::NumericOnly | OperatorRequirement::WithoutRhs);

BinaryOperator BinaryOperator::Add = BinaryOperator("+", OperatorRequirement::StringLengthLimit);
BinaryOperator BinaryOperator::Comma = BinaryOperator(",");
BinaryOperator BinaryOperator::Sub = BinaryOperator("-", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::Mul = BinaryOperator("*", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::Div = BinaryOperator("/", OperatorRequirement::NumericOnly | OperatorRequirement::RhsNonzero);
BinaryOperator BinaryOperator::Mod = BinaryOperator("%", OperatorRequirement::NumericOnly | OperatorRequirement::RhsNonzero);
BinaryOperator BinaryOperator::Less = BinaryOperator("<");
BinaryOperator BinaryOperator::Greater = BinaryOperator(">");
BinaryOperator BinaryOperator::Equal = BinaryOperator("==");
BinaryOperator BinaryOperator::NotEqual = BinaryOperator("!=");
BinaryOperator BinaryOperator::And = BinaryOperator("&&");
BinaryOperator BinaryOperator::Or = BinaryOperator("||");
BinaryOperator BinaryOperator::BitAnd = BinaryOperator("&", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::BitOr = BinaryOperator("|", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::Xor = BinaryOperator("^", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::ShLeft = BinaryOperator("<<", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::ShRight = BinaryOperator(">>", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::UnShRight = BinaryOperator(">>>", OperatorRequirement::NumericOnly);
BinaryOperator BinaryOperator::Min = BinaryOperator("Math.min", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall);
BinaryOperator BinaryOperator::Max = BinaryOperator("Math.max", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall);

UnaryOperator UnaryOperator::Not = UnaryOperator("!");
UnaryOperator UnaryOperator::Plus = UnaryOperator("+");
UnaryOperator UnaryOperator::Typeof = UnaryOperator("typeof ");
UnaryOperator UnaryOperator::Minus = UnaryOperator("-", OperatorRequirement::NumericOnly);
UnaryOperator UnaryOperator::Sqrt = UnaryOperator("Math.sqrt", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall | OperatorRequirement::RhsNonnegative);
UnaryOperator UnaryOperator::Abs = UnaryOperator("Math.abs", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall);
UnaryOperator UnaryOperator::Ceil = UnaryOperator("Math.ceil", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall);
UnaryOperator UnaryOperator::Floor = UnaryOperator("Math.floor", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall);
UnaryOperator UnaryOperator::Trunc = UnaryOperator("Math.trunc", OperatorRequirement::NumericOnly | OperatorRequirement::FunctionCall);