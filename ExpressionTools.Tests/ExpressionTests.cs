using System;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTools.Tests
{
    [TestClass]
    public class ExpressionTests
    {
        private Expression<Func<int,bool>> GetExpression()
        {            
            return (i) => i == 1;
        }
        
        [TestMethod]
        public void Find_Equals_ReturnsEqualsBinaryExpression()
        {
            Expression<Func<int,bool>> exp = GetExpression();
            BinaryExpression binaryExpression =  exp.Find<BinaryExpression>(be => be.NodeType == ExpressionType.Equal).FirstOrDefault();
            Assert.IsNotNull(binaryExpression);
        }
        
        [TestMethod]
        public void Find_Equals_ReturnsExpression()
        {
            Expression<Func<int,bool>> exp = GetExpression();
            Expression expression = exp.Find<Expression>(e => e.NodeType == ExpressionType.Equal).FirstOrDefault();
            Assert.IsNotNull(expression);
        }

        [TestMethod]
        public void Contains_Equals_ReturnsTrue()
        {
            Expression<Func<int,bool>> exp = GetExpression();
            bool result = exp.Contains<BinaryExpression>(be => true);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_NotEquals_ReturnsFalse()
        {
            Expression<Func<int,bool>> exp = GetExpression();
            bool result = exp.Contains<BinaryExpression>(be => be.NodeType == ExpressionType.NotEqual);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Replace_Equals_ReturnsExpressionWithNotEquals()
        {
            Expression<Func<int,bool>> exp = GetExpression();
            Expression result = exp.Replace<BinaryExpression>(be => true, be => Expression.NotEqual(be.Left, be.Right));
            Assert.IsFalse(result.Contains<BinaryExpression>(be => be.NodeType == ExpressionType.Equal));
        }

        [TestMethod]
        public void And_TwoExpression_ReturnsMergedExpression()
        {
            Expression<Func<int, bool>> exp1 = GetExpression();
            Expression<Func<int, bool>> exp2 = GetExpression();
            Expression<Func<int, bool>> result = exp1.And(exp2);
            Assert.IsTrue(result.Find<BinaryExpression>(be => be.NodeType == ExpressionType.AndAlso).Count() == 1);
        }

        [TestMethod]
        public void Or_TwoExpression_ReturnsMergedExpression()
        {
            Expression<Func<int, bool>> exp1 = GetExpression();
            Expression<Func<int, bool>> exp2 = GetExpression();
            Expression<Func<int, bool>> result = exp1.Or(exp2);
            Assert.IsTrue(result.Find<BinaryExpression>(be => be.NodeType == ExpressionType.OrElse).Count() == 1);
        }

        [TestMethod]
        public void And_TwoExpression_ReturnsCompilableExpression()
        {
            Expression<Func<int, bool>> exp1 = GetExpression();
            Expression<Func<int, bool>> exp2 = GetExpression();
            Expression<Func<int, bool>> expression = exp1.And(exp2);
            Func<int, bool> result = expression.Compile();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Or_TwoExpression_ReturnsCompilableExpression()
        {
            Expression<Func<int, bool>> exp1 = GetExpression();
            Expression<Func<int, bool>> exp2 = GetExpression();
            Expression<Func<int, bool>> expression = exp1.Or(exp2);
            Func<int, bool> result = expression.Compile();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Merge_TwoExpressions_ReplacesOnlyParametersFoundInFirstExpression()
        {
            Expression<Func<int, bool>> exp1 = GetExpression();
            Expression<Func<int, bool>> exp2 = (i) => i.ToString().Any(c => c == '1');
            Expression<Func<int, bool>> result =  exp1.Merge(exp2,Expression.AndAlso);
            Assert.IsTrue(result.Contains<ParameterExpression>(pe => pe.Name == "c" && pe.Type == typeof(char)));

        }
    }
}
