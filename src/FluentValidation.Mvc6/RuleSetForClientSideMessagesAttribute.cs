using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Filters;

namespace FluentValidation.Mvc6
{
    /// <summary>
    /// Rule set for client side messages attribute
    /// </summary>
    public class RuleSetForClientSideMessagesAttribute : ActionFilterAttribute
    {
        #region cTor
        /// <summary>
        /// Constructur with a rule set
        /// </summary>
        /// <param name="ruleSet"></param>
        public RuleSetForClientSideMessagesAttribute(string ruleSet)
        {
            ruleSets = new[] { ruleSet };
        }

        /// <summary>
        /// Constructor with param rule sets
        /// </summary>
        /// <param name="ruleSets"></param>
        public RuleSetForClientSideMessagesAttribute(params string[] ruleSets)
        {
            this.ruleSets = ruleSets;
        }
        #endregion

        #region Fields
        /// <summary>
        /// fluent validation client side rule set items key
        /// </summary>
        private const string key = "_FV_ClientSideRuleSet";

        /// <summary>
        /// the rules sets
        /// </summary>
        readonly string[] ruleSets;
        #endregion

        #region Methods
        /// <summary>
        /// Overrides the on action executing
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items[key] = ruleSets;
        }

        /// <summary>
        /// Gets the rule set for client validation from the http context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string[] GetRuleSetsForClientValidation(HttpContext context)
        {
            return context.Items[key] as string[] ?? new string[] { null };
        }
        #endregion
    }
}
