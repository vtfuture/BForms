using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Utilities
{
    public class ParameterVisitor : ExpressionVisitor
    {

        List<ParameterExpression> _parameterExpressions;

        public IEnumerable<ParameterExpression> GetParameters(Expression expr)
        {
            _parameterExpressions = new List<ParameterExpression>();
            Visit(expr);
            return _parameterExpressions;
        }

        protected override Expression VisitParameter(System.Linq.Expressions.ParameterExpression p)
        {

            if (!_parameterExpressions.Contains(p))
                _parameterExpressions.Add(p);

            return base.VisitParameter(p);
        }
    }
}
