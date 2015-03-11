using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Utilities
{

    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        private ParameterExpression _parameterExpression;

        public Expression ReplaceParameter(Expression expr, ParameterExpression parameterExpression)
        {
            _parameterExpression = parameterExpression;

            return Visit(expr);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {

            if (node.NodeType == ExpressionType.Parameter)
            {
                return this._parameterExpression;
            }
            else
            {
                return base.VisitParameter(node);
            }
        }
    }

}
