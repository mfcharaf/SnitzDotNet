using System;
using System.ComponentModel;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using RealWorld.Grids;

namespace Snitz.ThirdParty
{
	public class BulkEditGridViewEx : BulkEditGridView
	{
        [Browsable(false)]
        public bool BulkEdit
        {
            get
            {
                return (bool)this.ViewState["BulkEdit"];
            }

            set
            {
                this.ViewState["BulkEdit"] = value;
            }
        }

        public BulkEditGridViewEx()
        {
            this.BulkEdit = false;
        }
		
        public new void Save()
		{
			foreach (GridViewRow row in DirtyRows)
				this.UpdateRow(row.RowIndex, false);

			DirtyRows.Clear();
			OnSaved();
		}
        
        protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex,
          DataControlRowType rowType, DataControlRowState rowState)
        {
            GridViewRow row;

            if (this.BulkEdit)
                row = base.CreateRow(rowIndex, dataSourceIndex,
                      rowType, rowState | DataControlRowState.Edit);
            else
                row = base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);

            return row;
        }
		
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			//Attach an event handler to the save button.
			if (false == string.IsNullOrEmpty(this.SaveButtonID))
			{
				Control btn = RecursiveFindControl(this.NamingContainer, this.SaveButtonID);
				if (null != btn)
				{
					if (btn is Button)
					{
						((Button)btn).Click += new EventHandler(SaveClicked);
					}
					else if (btn is LinkButton)
					{
						((LinkButton)btn).Click += new EventHandler(SaveClicked);
					}
					else if (btn is ImageButton)
					{
						((ImageButton)btn).Click += new ImageClickEventHandler(SaveClicked);
					}

					//add more button types here.
				}
			}
		}

		protected virtual void SaveClicked(object sender, EventArgs e)
		{
			this.Save();
		}
        
        public void BulkUpdate()
        {
            foreach (GridViewRow row in this.Rows)
            {
                this.UpdateRow(row.RowIndex, false);
            }
        }
		
        protected Control RecursiveFindControl(Control namingcontainer, string controlName)
		{
			Control control = namingcontainer.FindControl(controlName);
			if (control != null)
			{
				return control;
			}
			if (namingcontainer.NamingContainer != null)
			{
				return this.RecursiveFindControl(namingcontainer.NamingContainer, controlName);
			}
			return null;
		}

		public event EventHandler<EventArgs> Saved;

		protected virtual void OnSaved()
		{
			EventHandler<EventArgs> handler = Saved;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}
		}

        private T ConvertValue<T>(string strValue)
        {
            object value = default(T);

            if (strValue != null)
            {
                if (typeof(T) == typeof(string))
                {
                    value = strValue;
                }
                else if (typeof(T) == typeof(int))
                {
                    value = Convert.ToInt32(strValue);
                }
                else if (typeof(T) == typeof(double))
                {
                    value = Convert.ToDouble(strValue);
                }
                else if (typeof(T) == typeof(bool))
                {
                    if (strValue.ToLower() == "on" || strValue.ToLower() ==
                                "true" || strValue.ToLower() == "1")
                        value = true;
                    else
                        value = false;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    value = Convert.ToDateTime(strValue);
                }
            }

            return (T)value;
        }

        public T GetOldValue<T>(int rowIndex, int cellIndex)
        {
            BoundField bf = this.Columns[cellIndex] as BoundField;

            T retVal = default(T);

            if (bf != null)
            {
                if (bf.ReadOnly)
                {
                    DataControlFieldCell cell =
                      this.Rows[rowIndex].Cells[cellIndex] as DataControlFieldCell;
                    retVal = ConvertValue<T>(cell.Text);
                }
                else
                {
                    Control ctrl = this.Rows[rowIndex].Cells[cellIndex].Controls[0];

                    if (ctrl.GetType() == typeof(TextBox))
                    {
                        retVal = this.ConvertValue<T>(((TextBox)ctrl).Text);
                    }
                    else if (ctrl.GetType() == typeof(CheckBox))
                    {
                        retVal = this.ConvertValue<T>(((CheckBox)ctrl).Checked.ToString());
                    }
                    else if (ctrl.GetType() == typeof(DropDownList))
                    {
                        retVal = this.ConvertValue<T>(((DropDownList)ctrl).SelectedValue);
                    }
                }
            }
            else
            {
                throw new ArgumentException("The cell selected is not a DataBoundControl!");
            }

            return retVal;
        }

        public T GetOldValue<T>(int rowIndex, string controlName)
        {
            Control ctrl = this.Rows[rowIndex].FindControl(controlName);

            T retVal = default(T);

            if (ctrl != null)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    retVal = this.ConvertValue<T>(((TextBox)ctrl).Text);
                }
                else if (ctrl.GetType() == typeof(CheckBox))
                {
                    retVal = this.ConvertValue<T>(((CheckBox)ctrl).Checked.ToString());
                }
                else if (ctrl.GetType() == typeof(DropDownList))
                {
                    retVal = this.ConvertValue<T>(((DropDownList)ctrl).SelectedValue);
                }
            }
            else
            {
                throw new ArgumentException("The controlName not found!");
            }

            return retVal;
        }

        private T GetNewValue<T>(string uniqueID)
        {
            string strValue = this.Page.Request[uniqueID];
            return ConvertValue<T>(strValue);
        }

        public T GetNewValue<T>(int rowIndex, int cellIndex)
        {
            BoundField bf = this.Columns[cellIndex] as BoundField;

            if (bf != null)
            {
                if (bf.ReadOnly)
                {
                    DataControlFieldCell cell =
                      this.Rows[rowIndex].Cells[cellIndex] as DataControlFieldCell;
                    return ConvertValue<T>(cell.Text);
                }
                else
                {
                    string uniqueID = this.Rows[rowIndex].Cells[cellIndex].Controls[0].UniqueID;
                    return this.GetNewValue<T>(uniqueID);
                }
            }
            else
            {
                throw new ArgumentException("The cell selected is not a DataBoundControl!");
            }
        }

        public T GetNewValue<T>(int rowIndex, string controlName)
        {
            string uniqueID = this.Rows[rowIndex].FindControl(controlName).UniqueID;
            return this.GetNewValue<T>(uniqueID);
        }
	}
}
