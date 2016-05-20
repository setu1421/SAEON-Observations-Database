using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.Observations.Data;
using Ext.Net;
using SubSonic;
using System.Text;
using System.Web.Security;

/// <summary>
/// Summary description for Roles
/// </summary>
public partial class _Roles : System.Web.UI.Page
{
	protected void Store1_RefreshData(object sender, StoreRefreshDataEventArgs e)
	{
		this.GridPanel1.GetStore().DataSource = RoleRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
	}

	protected void ValidateField(object sender, RemoteValidationEventArgs e)
	{

		//AspnetRoleCollection roles = new AspnetRoleCollection();

		//string checkColumn = String.Empty,
		//       errorMessage = String.Empty;

		//if (e.ID == "tfRoleName")
		//{
		//    roles = new AspnetRoleCollection().Where(AspnetRole.Columns.RoleName.ToLower(), tfRoleName.Text.ToLower()).Load();
		//    if (roles.Count != 0)
		//    {
		//        errorMessage = "The specified role name already exists";
		//    }

		//}

		//if (errorMessage != "")
		//{
		//    e.Success = false;
		//    e.ErrorMessage = errorMessage;
		//}
		//else
		//    e.Success = true;

	}

	protected void ShowDetails(object sender, DirectEventArgs e)
	{
		lblInfo.Hidden = true;
		lblInfo.Text = "";
		if (e != null)
		{

			string id = e.ExtraParams["id"];
			if (id != null)
			{
				AspnetRole role = new AspnetRole(id);

				tfID.Text = id;
				tfRoleName.Text = role.RoleName;
				tfComment.Text = role.Description;
			}
		}

		this.Window1.Show();
	}

	protected void NewRole(object sender, DirectEventArgs e)
	{
		tfID.Text = "";
		tfRoleName.Text = "";
		tfComment.Text = "";
		ShowDetails(null, null);

	}


	protected void DoDelete(object sender, DirectEventArgs e)
	{
		string ActionType = e.ExtraParams["type"];
		string recordID = e.ExtraParams["id"];

		if (ActionType == "Delete")
		{
			AspnetUsersInRoleCollection UiRCol = new AspnetUsersInRoleCollection().Where(AspnetUsersInRole.Columns.RoleId, SubSonic.Comparison.Equals, recordID).Load();

			if (UiRCol.Count != 0)
			{
				X.Msg.Show(new MessageBoxConfig
				{
					Title = "Error",
					Message = "Roles can't be deleted when they are connected to users.",
					Buttons = MessageBox.Button.OK,
					Icon = MessageBox.Icon.ERROR
				});
			}
			else
			{
				//AspnetRole.Delete(recordID);
				AspnetRole role = new AspnetRole(recordID);
				System.Web.Security.Roles.DeleteRole(role.RoleName);
			}

			GridPanel1.DataBind();

		}

		if (ActionType == "Edit")
		{
			ShowDetails(sender, e);
		}

		else if (ActionType == "RemoveModules")
		{
			RoleModuleCollection RMCol = new RoleModuleCollection().Where("RoleId", e.ExtraParams["RoleID"]).Where("ModuleId", recordID).Load();

			if (RMCol.Count == 1)
			{
				ModuleX module = new ModuleX(recordID);

				///////bad query	// (ModuleX.IdColumn, RoleModule.ModuleIDColumn)
				//SqlQuery q = new Select().From(RoleModule.Schema);
				//q.InnerJoin( ("ModuleX", "ID", "RoleModule", "ModuleID");
				//q.Where(RoleModule.Columns.RoleId).IsEqualTo(e.ExtraParams["RoleID"]);
				//q.And(ModuleX.Columns.ModuleID).IsEqualTo(module.ModuleID);
				//q.ExecuteAsCollection<RoleModuleCollection>();	
				//RoleModuleCollection RMColAllInRole = 
				VModuleRoleModuleCollection RMColAllInRole = new VModuleRoleModuleCollection()
					.Where(VModuleRoleModule.Columns.RoleId, SubSonic.Comparison.Equals, e.ExtraParams["RoleID"])
					.Where(VModuleRoleModule.Columns.BaseModuleID, SubSonic.Comparison.Equals, module.ModuleID)
					.Load();


				if (RMColAllInRole.Count == 1)
				{
					RoleModuleCollection rm = new RoleModuleCollection().Where("RoleId", e.ExtraParams["RoleID"]).Where("ModuleId", RMColAllInRole[0].BaseModuleID).Load();
					RoleModule.Delete(rm[0].Id);
					//RoleModule.Delete(RMColAllInRole[1].Id);

				}

				RoleModule.Delete(RMCol[0].Id);

				Store4.DataBind();
			}
			else
			{

				X.Msg.Show(new MessageBoxConfig
				{
					Title = "Error",
					Message = "Entry not found",
					Buttons = MessageBox.Button.OK,
					Icon = MessageBox.Icon.ERROR
				});
			}

		}
	}

	protected void SaveRole(object sender, DirectEventArgs e)
	{
		if (tfID.Text != "")
		{
			string id = tfID.Text;

			if (id != null)
			{

				AspnetRole role = new AspnetRole(id);
				//role.RoleName = tfRoleName.Text;
				//role.Description = tfComment.Text;
				//role.Save();
				if (!Roles.RoleExists(tfRoleName.Text))
				{
					string[] users = Roles.GetUsersInRole(role.RoleName);
					Roles.CreateRole(tfRoleName.Text);
					Roles.AddUsersToRole(users, tfRoleName.Text);
					Roles.RemoveUsersFromRole(users, role.RoleName);
					Roles.DeleteRole(role.RoleName);
				}

				role = new AspnetRole("RoleName", tfRoleName.Text);
				role.Description = tfComment.Text;
				role.Save();

				GridPanel1.DataBind();

				Window1.Hide();
			}
		}
		else  //new user
		{

			try
			{
				AspnetApplicationCollection aspApp = new AspnetApplicationCollection().Where(AspnetApplication.Columns.ApplicationName, Membership.ApplicationName).Load();
				AspnetRoleCollection roles = new AspnetRoleCollection().Where(AspnetRole.Columns.RoleName.ToLower(), tfRoleName.Text.ToLower()).Load();

				if ((aspApp.Count == 1) && (roles.Count == 0))
				{
					if (!Roles.RoleExists(tfRoleName.Text))
					{
						Roles.CreateRole(tfRoleName.Text);

						AspnetRole role = new AspnetRole("RoleName", tfRoleName.Text);
						role.Description = tfComment.Text;
						role.Save();
					}


					GridPanel1.DataBind();

					Window1.Hide();

					GridPanel1.DataBind();
				}
				else
				{
					lblInfo.Text = "Duplicate role names are not allowed.";
					lblInfo.Hidden = false;
				}

			}
			catch (Exception ex)
			{
				lblInfo.Text = ex.Message;
				lblInfo.Hidden = false;
				//throw;
			}
		}
	}

	protected void AcceptMod_Click(object sender, DirectEventArgs e)
	{
		StringBuilder result = new StringBuilder();

		RowSelectionModel sm = this.AModsGrid.SelectionModel.Primary as RowSelectionModel;
		RowSelectionModel RoleRow = this.GridPanel1.SelectionModel.Primary as RowSelectionModel;

		string RoleID = RoleRow.SelectedRecordID;

		foreach (SelectedRow row in sm.SelectedRows)
		{
			AspnetRole role = new AspnetRole(AspnetRole.Columns.RoleId, RoleID);
			ModuleX module = new ModuleX(row.RecordID);

			if (module.ModuleID != null)		//auto add the parent module to the RoleModule table
			{

				if (new RoleModuleCollection().Where(RoleModule.Columns.ModuleID, SubSonic.Comparison.Equals, module.ModuleID).Where(RoleModule.Columns.RoleId, SubSonic.Comparison.Equals, role.RoleId).Load().Load().Count() == 0)
				{
					RoleModule roleModBase = new RoleModule();
					roleModBase.RoleId = role.RoleId;
					roleModBase.ModuleID = new Guid(module.ModuleID.ToString());
					roleModBase.Save();
				}
			}

			RoleModule roleMod = new RoleModule();
			roleMod.RoleId = role.RoleId;
			roleMod.ModuleID = module.Id;

			roleMod.Save();
		}

		Store4.DataBind();
		AvailableModsWindow.Hide();
	}

	protected void AMods_RefreshData(object sender, StoreRefreshDataEventArgs e)
	{
		if (e.Parameters["RoleID"] != null && e.Parameters["RoleID"].ToString() != "-1")
		{
			Guid Id = Guid.Parse(e.Parameters["RoleID"].ToString());

			ModuleXCollection ModavailCol = new Select()
					.From(ModuleX.Schema)
					.Where(ModuleX.IdColumn).NotIn(new Select(new String[] { RoleModule.Columns.ModuleID })
																	.From(RoleModule.Schema)
																	.Where(RoleModule.RoleIdColumn).IsEqualTo(Id))
					.And(ModuleX.Columns.ModuleID).IsNotNull()
					.OrderAsc(ModuleX.Columns.ModuleID, ModuleX.Columns.Description)
					.ExecuteAsCollection<ModuleXCollection>();

			Store3.DataSource = ModavailCol.ToList();
			Store3.DataBind();

		}
	}

	protected void ModsGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
	{

		if (e.Parameters["RoleID"] != null && e.Parameters["RoleID"].ToString() != "-1")
		{

			Guid Id = Guid.Parse(e.Parameters["RoleID"].ToString());

			ModuleXCollection ModCol = new Select()
					  .From(ModuleX.Schema).InnerJoin(RoleModule.ModuleIDColumn, ModuleX.IdColumn)
					  .Where(RoleModule.Columns.RoleId).IsEqualTo(Id).OrderAsc(ModuleX.Columns.ModuleID, ModuleX.Columns.Description)
					  .And(ModuleX.Columns.ModuleID).IsNotNull()
					  .ExecuteAsCollection<ModuleXCollection>();

			this.ModsGrid.GetStore().DataSource = ModCol;
			this.ModsGrid.GetStore().DataBind();
		}
	}
}