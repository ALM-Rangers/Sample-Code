
namespace Microsoft.VisualStudio.TestTools.UITest.Sample.WordExtension
{
    using System;
    using System.Text;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.TestTools.UITest.Common;

    /// <summary>
    /// Word specific action filter.
    /// </summary>
    internal class WordActionFilter : UITestActionFilter
    {
        #region Simple Properties

        /// <summary>
        /// Gets whether to apply timeout to this filter or not.
        /// The filter time out is applied only for the Binary filters to determine if the filter
        /// should be executed or not. If the time between the last two actions exceeds
        /// the user configured timeout value then the filter is not executed.
        /// </summary>
        public override bool ApplyTimeout
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the category of this filter. Each category of filters are grouped together
        /// and these different groups are ordered as per the priority of that category. 
        /// </summary>
        public override UITestActionFilterCategory Category
        {
            get { return UITestActionFilterCategory.PostSimpleToCompoundActionConversion; }
        }

        /// <summary>
        /// Gets whether this filter is enabled or not.
        /// </summary>
        public override bool Enabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the type of this filter.
        /// </summary>
        public override UITestActionFilterType FilterType
        {
            get { return UITestActionFilterType.Binary; }
        }

        /// <summary>
        /// Gets the name of the group to which this filter belongs to.
        /// A group can be enabled/disabled using configuration file.
        /// </summary>
        public override string Group
        {
            get { return "WordActionFilters"; }
        }

        /// <summary>
        /// Gets the name of this filter.
        /// This is also used in logs to provide meaningful message.
        /// </summary>
        public override string Name
        {
            get { return "WordActionFilter"; }
        }

        #endregion

        /// <summary>
        /// Processes the rule for this filter. 
        /// The action stack can be modified by the filter if it matches it's filtering criteria.
        /// </summary>
        /// <param name="actionStack">The action stack.</param>
        /// <returns>True if no more filtering should be done in this cycle. False if the 
        /// filtering should continue with action filters that follow.</returns>
        public override bool ProcessRule(IUITestActionStack actionStack)
        {
            // Remove the mouse click preceeding on send keys on the same Excel cell.
            SendKeysAction lastAction = actionStack.Peek() as SendKeysAction;
            MouseAction secondLastAction = actionStack.Peek(1) as MouseAction;

            if (IsLeftClick(secondLastAction) &&
                AreActionsOnSameWordSelection(lastAction, secondLastAction))
            {
                // This is left click on a cell preceding a typing on the same cell.
                // Remove the left click action.
                // (0 means top-most action and 1 means 2nd action & so on.) 
                actionStack.Pop(1);
            }

            return false;
        }

        // Checks if this is a left click action or not.
        private static bool IsLeftClick(MouseAction mouseAction)
        {
            return mouseAction != null &&
                   mouseAction.ActionType == MouseActionType.Click &&
                   mouseAction.MouseButton == MouseButtons.Left &&
                   mouseAction.ModifierKeys == System.Windows.Input.ModifierKeys.None;
        }

        // Checks if two actions are on same selection or not.
        private static bool AreActionsOnSameWordSelection(SendKeysAction lastAction, MouseAction secondLastAction)
        {
            return lastAction != null && secondLastAction != null &&
                   lastAction.UIElement is WordSelectionRange &&
                   secondLastAction.UIElement is WordSelectionRange &&
                   object.Equals(lastAction.UIElement, secondLastAction.UIElement);
        }
    }
}
