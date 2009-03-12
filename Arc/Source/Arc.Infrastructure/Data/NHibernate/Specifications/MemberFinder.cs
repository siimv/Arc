#region License

// Copyright (c) 2008-2009 Marek Tihkan (marektihkan@gmail.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Marek Tihkan nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Arc.Infrastructure.Data.NHibernate.Specifications
{
    internal static class MemberFinder
    {
        public static string FindFromExpression(Expression expression)
        {
            var memberExpression = FindMemberExpression(expression);

            if (memberExpression == null)
                throw new ArgumentException("Could not determine member from " + expression, "expression");

            return BuildMemberName(memberExpression);
        }

        private static string BuildMemberName(MemberExpression memberExpression)
        {
            var result = memberExpression.Member.Name;

            while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = (MemberExpression)memberExpression.Expression;
                result = memberExpression.Member.Name + "." + result;
            }

            return result;
        }

        private static MemberExpression FindMemberExpression(Expression expression)
        {
            if (AliasFinder.IsAliasExpression(expression))
                return expression as MemberExpression;

            if (expression is MemberExpression)
                return expression as MemberExpression;
            
            if (expression is UnaryExpression)
                return FindFromUnaryExpression(expression as UnaryExpression);
            
            return null;
        }

        private static MemberExpression FindFromUnaryExpression(UnaryExpression expression)
        {
            if (expression.NodeType != ExpressionType.Convert)
                throw new ArgumentException("Cannot interpret member from " + expression, "expression");

            return (MemberExpression)expression.Operand;
        }


        public static bool IsPropertyExpression(Expression expression)
        {
            return IsProperty(expression) || IsCapsulatedProperty(expression) || IsAlias(expression);
        }

        private static bool IsAlias(Expression expression)
        {
            return AliasFinder.IsAliasExpression(expression);
        }

        private static bool IsCapsulatedProperty(Expression expression)
        {
            if (!(expression is UnaryExpression && expression.NodeType == ExpressionType.Convert))
                return false;

            var unaryExpression = expression as UnaryExpression;
            return unaryExpression.Operand is MemberExpression;
        }

        private static bool IsProperty(Expression expression)
        {
            if (!(expression is MemberExpression))
                return false;

            var memberExpression = expression as MemberExpression;
            return memberExpression.Expression is ParameterExpression && memberExpression.Member is PropertyInfo;
        }
    }
}