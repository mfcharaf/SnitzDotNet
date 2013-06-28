using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.ComponentModel;


namespace GroupDropDownList
{
    /// <summary>
    /// GroupDropDownList for forum jumpto dropdown.
    /// </summary>
    [ToolboxData("<{0}:GroupDropDownList runat=server></{0}:GroupDropDownList>")]
    public class GroupDropDownList : System.Web.UI.WebControls.DropDownList
    {
        /// <summary>
        /// The field in the datasource which provides values for groups
        /// </summary>
        [DefaultValue(""), Category("Data")]
        public virtual string DataGroupField
        {
            get
            {
                object obj = this.ViewState["DataGroupField"];
                if (obj != null)
                {
                    return (string)obj;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["DataGroupField"] = value;
            }
        }
        /// <summary>
        /// if a group doesn't have any enabled items,there is no need
        /// to render the group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private bool IsGroupHasEnabledItems(string groupName)
        {
            ListItemCollection items = this.Items;
            ListItem item;
            for (int i = 0; i < items.Count; i++)
            {
                item = items[i];
                if (item.Attributes["DataGroupField"].Equals(groupName) && item.Enabled)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Render this control to the output parameter specified.
        /// Based on the source code of the original DropDownList method
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            ListItemCollection items = this.Items;
            int itemCount = this.Items.Count;
            string curGroup = String.Empty;
            string itemGroup;
            bool bSelected = false;

            if (itemCount <= 0)
            {
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                ListItem item = items[i];
                itemGroup = (string)item.Attributes["DataGroupField"];
                if (itemGroup != null && itemGroup != curGroup && IsGroupHasEnabledItems(itemGroup))
                {
                    if (curGroup != String.Empty)
                    {
                        writer.WriteEndTag("optgroup");
                        writer.WriteLine();
                    }

                    curGroup = itemGroup;
                    writer.WriteBeginTag("optgroup");
                    writer.WriteAttribute("label", curGroup, true);
                    writer.Write('>');
                    writer.WriteLine();
                }
                // we don't want to render disabled items
                if (item.Enabled)
                {
                    writer.WriteBeginTag("option");
                    if (item.Selected)
                    {
                        if (bSelected)
                        {
                            throw new HttpException("Cant_Multiselect_In_DropDownList");
                        }
                        bSelected = true;
                        writer.WriteAttribute("selected", "selected", false);
                    }

                    writer.WriteAttribute("value", item.Value, true);
                    writer.Write('>');
                    HttpUtility.HtmlEncode(item.Text, writer);
                    writer.WriteEndTag("option");
                    writer.WriteLine();
                }
            }
            if (curGroup != String.Empty)
            {
                writer.WriteEndTag("optgroup");
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Perform data binding logic that is associated with the control
        /// </summary>
        /// <param name="e">An EventArgs object that contains the event data</param>
        protected override void OnDataBinding(EventArgs e)
        {
            // Call base method to bind data
            base.OnDataBinding(e);

            if (this.DataGroupField == String.Empty)
            {
                return;
            }
            // For each Item add the attribute "DataGroupField" with value from the datasource
            IEnumerable dataSource = GetResolvedDataSource(this.DataSource, this.DataMember);
            if (dataSource != null)
            {
                ListItemCollection items = this.Items;
                int i = 0;

                string groupField = this.DataGroupField;
                foreach (object obj in dataSource)
                {
                    string groupFieldValue = DataBinder.GetPropertyValue(obj, groupField, null);
                    ListItem item = items[i];
                    item.Attributes.Add("DataGroupField", groupFieldValue);
                    i++;
                }
            }

        }

        /// <summary>
        /// This is a copy of the internal ListControl method
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="dataMember"></param>
        /// <returns></returns>
        private IEnumerable GetResolvedDataSource(object dataSource, string dataMember)
        {
            if (dataSource != null)
            {
                IListSource source1 = dataSource as IListSource;
                if (source1 != null)
                {
                    IList list1 = source1.GetList();
                    if (!source1.ContainsListCollection)
                    {
                        return list1;
                    }
                    if ((list1 != null) && (list1 is ITypedList))
                    {
                        ITypedList list2 = (ITypedList)list1;
                        PropertyDescriptorCollection collection1 = list2.GetItemProperties(new PropertyDescriptor[0]);
                        if ((collection1 == null) || (collection1.Count == 0))
                        {
                            throw new HttpException("ListSource_Without_DataMembers");
                        }
                        PropertyDescriptor descriptor1 = null;
                        if ((dataMember == null) || (dataMember.Length == 0))
                        {
                            descriptor1 = collection1[0];
                        }
                        else
                        {
                            descriptor1 = collection1.Find(dataMember, true);
                        }
                        if (descriptor1 != null)
                        {
                            object obj1 = list1[0];
                            object obj2 = descriptor1.GetValue(obj1);
                            if ((obj2 != null) && (obj2 is IEnumerable))
                            {
                                return (IEnumerable)obj2;
                            }
                        }
                        throw new HttpException("ListSource_Missing_DataMember");
                    }
                }
                if (dataSource is IEnumerable)
                {
                    return (IEnumerable)dataSource;
                }
            }
            return null;
        }
        
        #region Internal behaviour
        ///<summary>
        /// Saves the state of the view.
        /// </summary>
        protected override object SaveViewState()
        {
            // Create an object array with one element for the CheckBoxList's
            // ViewState contents, and one element for each ListItem in skmCheckBoxList
            object[] state = new object[this.Items.Count + 1];

            object baseState = base.SaveViewState();
            state[0] = baseState;

            // Now, see if we even need to save the view state
            bool itemHasAttributes = false;
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Attributes.Count > 0)
                {
                    itemHasAttributes = true;

                    // Create an array of the item's Attribute's keys and values
                    object[] attribKV = new object[this.Items[i].Attributes.Count * 2];
                    int k = 0;
                    foreach (string key in this.Items[i].Attributes.Keys)
                    {
                        attribKV[k++] = key;
                        attribKV[k++] = this.Items[i].Attributes[key];
                    }

                    state[i + 1] = attribKV;
                }
            }

            // return either baseState or state, depending on whether or not
            // any ListItems had attributes
            if (itemHasAttributes)
                return state;
            else
                return baseState;
        }

        /// <summary>
        /// Loads the state of the view.
        /// </summary>
        /// <param name="savedState">State of the saved.</param>
        protected override void LoadViewState(object savedState)
        {
            if (savedState == null) return;

            // see if savedState is an object or object array
            if (savedState is object[])
            {
                // we have an array of items with attributes
                object[] state = (object[])savedState;
                base.LoadViewState(state[0]);   // load the base state

                for (int i = 1; i < state.Length; i++)
                {
                    if (state[i] != null)
                    {
                        // Load back in the attributes
                        object[] attribKV = (object[])state[i];
                        for (int k = 0; k < attribKV.Length; k += 2)
                            this.Items[i - 1].Attributes.Add(attribKV[k].ToString(),
                                                           attribKV[k + 1].ToString());
                    }
                }
            }
            else
                // we have just the base state
                base.LoadViewState(savedState);
        }
        #endregion
    }
}
