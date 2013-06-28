using System;
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
		public new void Save()
		{
			foreach (GridViewRow row in DirtyRows)
				this.UpdateRow(row.RowIndex, false);

			DirtyRows.Clear();
			OnSaved();
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
	}
}
