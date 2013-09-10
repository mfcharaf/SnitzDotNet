using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AjaxControlToolkit;

namespace SnitzUI.UserControls
{
    public class CollapsiblePanelEx : CollapsiblePanelExtender
    {
        [DefaultValue("")]
        [ExtenderControlEvent]
        [ClientPropertyName("expanding")]
        public string OnExpand
        {
            get { return (string)(ViewState["OnExpand"] ?? string.Empty); }
            set { ViewState["OnExpand"] = value; }
        }
        [DefaultValue("")]
        [ExtenderControlEvent]
        [ClientPropertyName("expanded")]
        public string OnExpanded
        {
            get { return (string)ViewState["OnExpanded"] ?? string.Empty; }
            set { ViewState["OnExpanded"] = value; }
        }
        [DefaultValue("")]
        [ExtenderControlEvent]
        [ClientPropertyName("expandComplete")]
        public string OnExpandComplete
        {
            get { return (string)ViewState["OnExpandComplete"] ?? string.Empty; }
            set { ViewState["OnExpandComplete"] = value; }
        }
        [DefaultValue("")]
        [ExtenderControlEvent]
        [ClientPropertyName("collapsing")]
        public string OnCollapse
        {
            get { return (string)ViewState["OnCollapse"] ?? string.Empty; }
            set { ViewState["OnCollapse"] = value; }
        }
        [DefaultValue("")]
        [ExtenderControlEvent]
        [ClientPropertyName("collapsed")]
        public string OnCollapsed
        {
            get { return (string)ViewState["OnCollapsed"] ?? string.Empty; }
            set { ViewState["OnCollapsed"] = value; }
        }
        [DefaultValue("")]
        [ExtenderControlEvent]
        [ClientPropertyName("collapseComplete")]
        public string OnCollapseComplete
        {
            get { return (string)ViewState["OnCollapseComplete"] ?? string.Empty; }
            set { ViewState["OnCollapseComplete"] = value; }
        }
    } 
}
