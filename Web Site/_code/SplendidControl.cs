/**********************************************************************************************************************
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
 * IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *********************************************************************************************************************/
using System;
using System.Threading;
using System.Globalization;

using Microsoft.AspNetCore.Http;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidControl.
	/// </summary>
	public class SplendidControl // : System.Web.UI.UserControl
	{
		private IHttpContextAccessor httpContextAccessor;
		private HttpApplicationState Application        = new HttpApplicationState();
		private HttpContext          Context            ;
		public  HttpRequest          Request            ;
		private HttpSessionState     Session            ;
		private Currency             Currency           = new Currency();
		private TimeZone             TimeZone           = new TimeZone();
		private SplendidCache        SplendidCache      ;

		protected bool     bDebug = false;
		protected L10N     L10n;
		protected TimeZone T10n;
		protected Currency C10n;
		protected string   m_sMODULE;  // 04/27/2006 Paul.  Leave null so that we can get an error when not initialized. 
		protected bool     m_bNotPostBack = false;  // 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
		// 11/10/2010 Paul.  The RulesRedirectURL is used by the Rules Engine to allow a redirect after an event. 
		protected string      m_sRulesRedirectURL ;
		protected string      m_sRulesErrorMessage;
		protected bool        m_bRulesIsValid      = true;
		protected string      m_sLayoutListView  ;
		protected string      m_sLayoutEditView  ;
		protected string      m_sLayoutDetailView;

		// 10/10/2015 Paul.  We need a quick way to return stream enabled status. 
		protected bool?       m_bStreamEnabled;
		// 09/26/2017 Paul.  Add Archive access right. 
		protected bool?       m_bArchiveView      ;
		protected bool?       m_bArchiveEnabled   ;
		protected string      m_sVIEW_NAME        ;
		protected bool?       m_bArchiveViewExists;
		// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
		protected bool?       m_bRecordLevelSecurity;
		// 11/23/2017 Paul.  Provide a way to globally disable favorites and following. 
		protected bool?       m_bDisableFavorites;
		protected bool?       m_bDisableFollowing;

		public SplendidControl(IHttpContextAccessor httpContextAccessor, HttpSessionState Session, SplendidCache SplendidCache)
		{
			this.httpContextAccessor = httpContextAccessor ;
			this.Context             = httpContextAccessor.HttpContext;
			this.Session             = Session             ;
			this.L10n                = new L10N(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			if ( this.Context != null )
				this.Request             = this.Context.Request;
			this.SplendidCache       = SplendidCache       ;
		}

		public bool ArchiveView()
		{
			if ( !m_bArchiveView.HasValue )
			{
				this.m_bArchiveView = Sql.ToBoolean(Request.Query["ArchiveView"]);
			}
			return m_bArchiveView.Value;
		}

		public bool ArchiveEnabled()
		{
			if ( !m_bArchiveEnabled.HasValue )
			{
				this.m_bArchiveEnabled = Sql.ToBoolean(Application["Modules." + m_sMODULE + ".ArchiveEnabled"]);
			}
			return m_bArchiveEnabled.Value;
		}

		public bool ArchiveViewEnabled()
		{
			return ArchiveView() && ArchiveEnabled();
		}

		public bool ArchiveViewExists()
		{
			if ( Sql.IsEmptyString(m_sVIEW_NAME) )
				return false;
			if ( !m_bArchiveViewExists.HasValue )
			{
				this.m_bArchiveViewExists = ArchiveView() && SplendidCache.ArchiveViewExists(m_sVIEW_NAME);
			}
			return m_bArchiveViewExists.Value;
		}

		public bool StreamEnabled()
		{
			if ( !m_bStreamEnabled.HasValue )
			{
				// 10/10/2015 Paul.  Allow activity streams to be disabled for performance reasons. 
				this.m_bStreamEnabled = Sql.ToBoolean(Application["Modules." + m_sMODULE + ".StreamEnabled"]) && Sql.ToBoolean(Application["CONFIG.enable_activity_streams"]);
			}
			return m_bStreamEnabled.Value;
		}

		// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
		public bool RecordLevelSecurity()
		{
			if ( !m_bRecordLevelSecurity.HasValue )
			{
				// 10/10/2015 Paul.  Allow activity streams to be disabled for performance reasons. 
				this.m_bRecordLevelSecurity = Sql.ToBoolean(Application["Modules." + m_sMODULE + ".RecordLevelSecurity"]);
			}
			return m_bRecordLevelSecurity.Value;
		}

		// 11/23/2017 Paul.  Provide a way to globally disable favorites and following. 
		public bool DisableFavorites()
		{
			if ( !m_bDisableFavorites.HasValue )
			{
				this.m_bDisableFavorites = Sql.ToBoolean(Application["CONFIG.disable_favorites"]);
			}
			return m_bDisableFavorites.Value;
		}

		public bool DisableFollowing()
		{
			if ( !m_bDisableFollowing.HasValue )
			{
				this.m_bDisableFollowing = Sql.ToBoolean(Application["CONFIG.disable_Following"]);
			}
			return m_bDisableFollowing.Value;
		}

		public string RulesRedirectURL
		{
			get { return m_sRulesRedirectURL; }
			set { m_sRulesRedirectURL = value; }
		}

		public string RulesErrorMessage
		{
			get
			{
				return m_sRulesErrorMessage;
			}
			set
			{
				// 04/28/2011 Paul.  Allow multiple error messages by concatenating. 
				m_sRulesErrorMessage += value;
				m_bRulesIsValid = false;
			}
		}

		public bool RulesIsValid
		{
			get { return m_bRulesIsValid; }
			set { m_bRulesIsValid = value; }
		}

		public string LayoutListView
		{
			get
			{
				return m_sLayoutListView;
			}
			set
			{
				m_sLayoutListView = value;
			}
		}

		public string LayoutEditView
		{
			get
			{
				return m_sLayoutEditView;
			}
			set
			{
				m_sLayoutEditView = value;
			}
		}

		public string LayoutDetailView
		{
			get
			{
				return m_sLayoutDetailView;
			}
			set
			{
				m_sLayoutDetailView = value;
			}
		}

		#region Menu
		public bool IsMobile
		{
			get
			{
				return false;
			}
		}

		public bool PrintView
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool NotPostBack
		{
			get { return m_bNotPostBack; }
			set { m_bNotPostBack = value; }
		}

		protected void SetMenu(string sMODULE)
		{
		}

		// 07/24/2010 Paul.  We need an admin flag for the areas that don't have a record in the Modules table. 
		protected void SetAdminMenu(string sMODULE)
		{
		}

		public void SetPageTitle(string sTitle)
		{
		}
		#endregion

// 11/03/2021 Paul.  ASP.Net components are not needed. 
#if !ReactOnlyUI
		#region Append DetailView
		protected void AppendDetailViewFields(string sDETAIL_NAME, HtmlTable tbl, DataRow rdr)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendDetailViewFields(sDETAIL_NAME, tbl, rdr, GetL10n(), GetT10n(), null);
		}

		protected void AppendDetailViewFields(string sDETAIL_NAME, HtmlTable tbl, DataRow rdr, CommandEventHandler Page_Command)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendDetailViewFields(sDETAIL_NAME, tbl, rdr, GetL10n(), GetT10n(), Page_Command);
		}

		protected void AppendDetailViewRelationships(string sDETAIL_NAME, PlaceHolder plc)
		{
			// 02/26/2010 Paul.  The SubPanel Tabs flag has been moved to the Session so that it would be per-user. 
			bool bGroupTabs = Sql.ToBoolean(Session["USER_SETTINGS/SUBPANEL_TABS"]);
			AppendDetailViewRelationships(sDETAIL_NAME, plc, Guid.Empty, bGroupTabs, false);
		}

		// 07/10/2009 Paul.  We are now allowing relationships to be user-specific. 
		protected void AppendDetailViewRelationships(string sDETAIL_NAME, PlaceHolder plc, Guid gUSER_ID)
		{
			// 03/03/2014 Paul.  User ID was not being passed to method, so Dashboard was global. 
			AppendDetailViewRelationships(sDETAIL_NAME, plc, gUSER_ID, false, false);
		}

		// 01/02/2020 Paul.  Disable update panel on Dashboard. 
		protected void AppendDetailViewRelationships(string sDETAIL_NAME, PlaceHolder plc, Guid gUSER_ID, bool bEnableUpdatePanel)
		{
			// 03/03/2014 Paul.  User ID was not being passed to method, so Dashboard was global. 
			AppendDetailViewRelationships(sDETAIL_NAME, plc, gUSER_ID, false, false, bEnableUpdatePanel);
		}

		protected void tc_ActiveTabChanged(object sender, EventArgs e)
		{
			// 02/27/2010 Paul.  Save the ActiveTabIndex. 
			string sDETAIL_NAME = Sql.ToString(Page.Items["DETAIL_NAME"]);
			AjaxControlToolkit.TabContainer tc = sender as AjaxControlToolkit.TabContainer;
			if ( tc != null )
			{
				Session[sDETAIL_NAME + ".ActiveTabIndex"] = tc.ActiveTabIndex.ToString();
			}
		}

		// 01/02/2020 Paul.  Disable update panel on Dashboard. 
		protected void AppendDetailViewRelationships(string sDETAIL_NAME, PlaceHolder plc, Guid gUSER_ID, bool bGroupTabs, bool bEditView)
		{
			// 09/30/2015 Paul.  Disable update panel on Dashboard. 
			AppendDetailViewRelationships(sDETAIL_NAME, plc, gUSER_ID, bGroupTabs, bEditView, true);
		}

		// 12/03/2009 Paul.  Add support for tabbed subpanels.
		// 01/02/2020 Paul.  Disable update panel on Dashboard. 
		protected void AppendDetailViewRelationships(string sDETAIL_NAME, PlaceHolder plc, Guid gUSER_ID, bool bGroupTabs, bool bEditView, bool bEnableUpdatePanel)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			//int nPlatform = (int) Environment.OSVersion.Platform;
			DataTable dtFields = null;
			if ( Sql.IsEmptyGuid(gUSER_ID) )
				dtFields = SplendidCache.DetailViewRelationships(sDETAIL_NAME);
			else
				dtFields = SplendidCache.UserDashlets(sDETAIL_NAME, gUSER_ID);
			
			// 02/27/2010 Paul.  Save the Detail Name so that it can be used in the 
			Page.Items["DETAIL_NAME"] = sDETAIL_NAME;
			//bGroupTabs = false;
			if ( bGroupTabs )
			{
				GetL10n();
				DataTable dtTabGroups    = SplendidCache.TabGroups();
				DataView  vwModuleGroups = new DataView(SplendidCache.ModuleGroups());

				Literal litBR = new Literal();
				litBR.Text = "<br />";
				plc.Controls.Add(litBR);

				List<String> lstPlacedControls = new List<String>();
				int nControlsInserted = 0;
				AjaxControlToolkit.TabContainer tc = new AjaxControlToolkit.TabContainer();
				tc.TabStripPlacement = TabStripPlacement.Top;
				plc.Controls.Add(tc);
				// 02/27/2010 Paul.  Capture the ActiveTabIndex.  This only works during postback.  
				// Just clicking tab will not cause the event to fire. 
				// It would be nice to always remember the Tab, but it is too much work at this stage. 
				tc.ActiveTabChanged += new EventHandler(tc_ActiveTabChanged);
				AjaxControlToolkit.TabPanel tabOther = null;
				foreach(DataRow rowTabs in dtTabGroups.Rows)
				{
					string sGROUP_NAME = Sql.ToString(rowTabs["NAME" ]);
					string sTITLE      = Sql.ToString(rowTabs["TITLE"]);
					AjaxControlToolkit.TabPanel tab = new AjaxControlToolkit.TabPanel();
					tab.HeaderText = L10n.Term(sTITLE);
					tc.Tabs.Add(tab);
					if ( sGROUP_NAME == "Other" )
						tabOther = tab;
					foreach(DataRow row in dtFields.Rows)
					{
						// 12/03/2009 Paul.  The Title is used for the tabbed subpanels. 
						string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
						string sCONTROL_NAME = Sql.ToString(row["CONTROL_NAME"]);
						// 04/27/2006 Paul.  Only add the control if the user has access. 
						vwModuleGroups.RowFilter = "GROUP_NAME = '" + sGROUP_NAME + "' and MODULE_NAME = '" + sMODULE_NAME + "'";
						if ( Security.GetUserAccess(sMODULE_NAME, "list") >= 0 && vwModuleGroups.Count > 0 )
						{
							try
							{
								Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
								// 07/10/2009 Paul.  If this is a Dashlet, then set the DetailView name. 
								DashletControl ctlDashlet = ctl as DashletControl;
								if ( ctlDashlet != null )
								{
									ctlDashlet.DetailView = sDETAIL_NAME;
								}
								// 01/02/2020 Paul.  Disable update panel on Dashboard. 
								if ( bEnableUpdatePanel )
								{
									UpdatePanel pnl = new UpdatePanel();
									// 05/06/2010 Paul.  Try using the UpdatePanel Conditional mode. 
									pnl.UpdateMode = UpdatePanelUpdateMode.Conditional;
									tab.Controls.Add(pnl);
									pnl.ContentTemplateContainer.Controls.Add(ctl);
								}
								else
								{
									tab.Controls.Add(ctl);
								}
								nControlsInserted++;
								// 01/09/2009 Paul.  Keep a list of controls we have placed so that unplaced items can be added to the Other tab. 
								if ( !lstPlacedControls.Contains(sCONTROL_NAME) )
									lstPlacedControls.Add(sCONTROL_NAME);
								// 02/14/2013 Paul.  Now that the DetailViewRelationships are added in Page_Load, we need to manually fire the DataBind event. 
								if ( !Page.IsPostBack )
									ctl.DataBind();
							}
							catch(Exception ex)
							{
								if ( !Utils.IsOfflineClient )
								{
									Label lblError = new Label();
									lblError.Text            = Utils.ExpandException(ex);
									lblError.ForeColor       = System.Drawing.Color.Red;
									lblError.EnableViewState = false;
									tab.Controls.Add(lblError);
								}
							}
						}
					}
					if ( tab.Controls.Count == 0 )
					{
						tab.Visible = false;
					}
				}
				if ( tabOther != null )
				{
					// 01/09/2009 Paul.  Keep a list of controls we have placed so that unplaced items can be added to the Other tab. 
					foreach(DataRow row in dtFields.Rows)
					{
						string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
						string sCONTROL_NAME = Sql.ToString(row["CONTROL_NAME"]);
						if ( Security.GetUserAccess(sMODULE_NAME, "list") >= 0  && !lstPlacedControls.Contains(sCONTROL_NAME) )
						{
							try
							{
								Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
								// 07/10/2009 Paul.  If this is a Dashlet, then set the DetailView name. 
								DashletControl ctlDashlet = ctl as DashletControl;
								if ( ctlDashlet != null )
								{
									ctlDashlet.DetailView = sDETAIL_NAME;
								}
								// 01/02/2020 Paul.  Disable update panel on Dashboard. 
								if ( bEnableUpdatePanel )
								{
									UpdatePanel pnl = new UpdatePanel();
									// 05/06/2010 Paul.  Try using the UpdatePanel Conditional mode. 
									pnl.UpdateMode = UpdatePanelUpdateMode.Conditional;
									tabOther.Controls.Add(pnl);
									pnl.ContentTemplateContainer.Controls.Add(ctl);
								}
								else
								{
									tabOther.Controls.Add(ctl);
								}
								nControlsInserted++;
								lstPlacedControls.Add(sCONTROL_NAME);
								// 02/14/2013 Paul.  Now that the DetailViewRelationships are added in Page_Load, we need to manually fire the DataBind event. 
								if ( !Page.IsPostBack )
									ctl.DataBind();
							}
							catch(Exception ex)
							{
								if ( !Utils.IsOfflineClient )
								{
									Label lblError = new Label();
									lblError.Text            = Utils.ExpandException(ex);
									lblError.ForeColor       = System.Drawing.Color.Red;
									lblError.EnableViewState = false;
									tabOther.Controls.Add(lblError);
								}
							}
							tabOther.Visible = true;
						}
					}
				}
				if ( !IsPostBack )
				{
					// 02/27/2010 Paul.  Restore the ActiveTabIndex. 
					int nActiveTabIndex = Sql.ToInteger(Session[sDETAIL_NAME + ".ActiveTabIndex"]);
					if ( nActiveTabIndex > 0 )
					{
						tc.ActiveTabIndex = nActiveTabIndex;
					}
					else
					{
						for ( int i=0; i < tc.Tabs.Count; i++ )
						{
							TabPanel tab = tc.Tabs[i];
							if ( tab.Controls.Count > 0 )
							{
								tc.ActiveTabIndex = i;
								break;
							}
						}
					}
				}
				// 12/28/2009 Paul.  If no controls were placed, then this might be an admin area that does not use tabs. 
				// Try again, but without the tabs. 
				if ( dtFields.Rows.Count > 0 && nControlsInserted == 0 )
				{
					tc.Visible = false;
					AppendDetailViewRelationships(sDETAIL_NAME, plc, gUSER_ID, false, bEditView);
				}
			}
			else
			{
				foreach(DataRow row in dtFields.Rows)
				{
					string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
					string sCONTROL_NAME = Sql.ToString(row["CONTROL_NAME"]);
					// 04/27/2006 Paul.  Only add the control if the user has access. 
					if ( Security.GetUserAccess(sMODULE_NAME, "list") >= 0 )
					{
						try
						{
							// 09/21/2008 Paul.  Mono does not fully support AJAX at this time. 
							// 09/22/2008 Paul.  The UpdatePanel is no longer crashing Mono, so resume using it. 
							// 01/27/2010 Paul.  We cannot use an UpdatePanel in EditView mode as the async requests prevent the subpanel data from being submitted. 
							if ( bEditView )
							{
								Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
								SubPanelControl ctlSubPanel = ctl as SubPanelControl;
								if ( ctlSubPanel != null )
								{
									ctlSubPanel.IsEditView = true;
									plc.Controls.Add(ctl);
									// 02/14/2013 Paul.  Now that the DetailViewRelationships are added in Page_Load, we need to manually fire the DataBind event. 
									if ( !Page.IsPostBack )
										ctl.DataBind();
								}
							}
							else
							{
								// 04/24/2008 Paul.  Put an update panel around all sub panels. This will allow in-place pagination and sorting. 
								UpdatePanel pnl = new UpdatePanel();
								// 05/06/2010 Paul.  Try using the UpdatePanel Conditional mode. 
								pnl.UpdateMode = UpdatePanelUpdateMode.Conditional;
								plc.Controls.Add(pnl);
								Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
								// 07/10/2009 Paul.  If this is a Dashlet, then set the DetailView name. 
								DashletControl ctlDashlet = ctl as DashletControl;
								if ( ctlDashlet != null )
								{
									ctlDashlet.DetailView = sDETAIL_NAME;
								}
								pnl.ContentTemplateContainer.Controls.Add(ctl);
								// 02/14/2013 Paul.  Now that the DetailViewRelationships are added in Page_Load, we need to manually fire the DataBind event. 
								if ( !Page.IsPostBack )
									ctl.DataBind();
							}
						}
						catch(Exception ex)
						{
							// 11/29/2009 Paul.  Ignore the missing file on the offline client.  This might be intentional. 
							if ( !Utils.IsOfflineClient )
							{
								Label lblError = new Label();
								// 06/09/2006 Paul.  Catch the error and display a message instead of crashing. 
								// 12/27/2008 Paul.  Don't specify an ID as there can be multiple errors. 
								lblError.Text            = Utils.ExpandException(ex);
								lblError.ForeColor       = System.Drawing.Color.Red;
								lblError.EnableViewState = false;
								plc.Controls.Add(lblError);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Append EditView
		// 04/19/2010 Paul.  New approach to EditView Relationships will distinguish between New Record and Existing Record.
		// 08/14/2020 Paul.  Hide Quick Create hover if nothing to display. 
		protected int AppendEditViewRelationships(string sEDIT_NAME, PlaceHolder plc, bool bNewRecord)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			DataTable dtFields = SplendidCache.EditViewRelationships(sEDIT_NAME, bNewRecord);
			foreach(DataRow row in dtFields.Rows)
			{
				string sMODULE_NAME    = Sql.ToString(row["MODULE_NAME"   ]);
				string sCONTROL_NAME   = Sql.ToString(row["CONTROL_NAME"  ]);
				string sALTERNATE_VIEW = Sql.ToString(row["ALTERNATE_VIEW"]);
				if ( Sql.IsEmptyString(sALTERNATE_VIEW) )
					sALTERNATE_VIEW = "EditView.Inline";
				// 04/19/2010 Paul.  Only add the control if the user has access. 
				if ( Security.GetUserAccess(sMODULE_NAME, "edit") >= 0 )
				{
					try
					{
						Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
						NewRecordControl ctlNewRecord = ctl as NewRecordControl;
						if ( ctlNewRecord != null )
						{
							ctlNewRecord.EditView          = sALTERNATE_VIEW;
							ctlNewRecord.Width             = new Unit("100%");
							ctlNewRecord.ShowHeader        = false;
							ctlNewRecord.ShowInlineHeader  = true ;
							ctlNewRecord.ShowTopButtons    = false;
							ctlNewRecord.ShowBottomButtons = false;
							plc.Controls.Add(ctl);
						}
					}
					catch(Exception ex)
					{
						// 11/29/2009 Paul.  Ignore the missing file on the offline client.  This might be intentional. 
						if ( !Utils.IsOfflineClient )
						{
							Label lblError = new Label();
							// 06/09/2006 Paul.  Catch the error and display a message instead of crashing. 
							// 12/27/2008 Paul.  Don't specify an ID as there can be multiple errors. 
							lblError.Text            = Utils.ExpandException(ex);
							lblError.ForeColor       = System.Drawing.Color.Red;
							lblError.EnableViewState = false;
							plc.Controls.Add(lblError);
						}
					}
				}
			}
			return dtFields.Rows.Count;
		}

		protected void AppendEditViewFields(string sEDIT_NAME, HtmlTable tbl, DataRow rdr, string sSubmitClientID)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendEditViewFields(sEDIT_NAME, tbl, rdr, GetL10n(), GetT10n(), sSubmitClientID);
		}

		protected void AppendEditViewFields(string sEDIT_NAME, HtmlTable tbl, DataRow rdr)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendEditViewFields(sEDIT_NAME, tbl, rdr, GetL10n(), GetT10n());
		}

		protected void ValidateEditViewFields(string sEDIT_NAME)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ValidateEditViewFields(sEDIT_NAME, this);
		}
		#endregion

		#region Apply Rules
		// 11/10/2010 Paul.  Apply Business Rules. 
		protected void ApplyEditViewNewEventRules(string sEDIT_NAME)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyEditViewRules(sEDIT_NAME, this, "NEW_EVENT_XOML", null);
			// 04/28/2011 Paul.  If there was an rules error, then make sure to display it. 
			if ( !this.RulesIsValid )
				throw(new Exception(this.RulesErrorMessage));
		}

		// 11/11/2010 Paul.  Change to Pre Load and Post Load. 
		protected void ApplyEditViewPreLoadEventRules(string sEDIT_NAME, DataRow row)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyEditViewRules(sEDIT_NAME, this, "PRE_LOAD_EVENT_XOML", row);
			// 04/28/2011 Paul.  We don't want to throw an exception here because it would prevent other core logic. 
		}

		protected void ApplyEditViewPostLoadEventRules(string sEDIT_NAME, DataRow row)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyEditViewRules(sEDIT_NAME, this, "POST_LOAD_EVENT_XOML", row);
			// 08/08/2016 Paul.  Apply Business Process Rules. 
			WF4ApprovalActivity.ApplyEditViewPostLoadEventRules(Application, L10n, sEDIT_NAME, this, row);
			// 04/28/2011 Paul.  If there was an rules error, then make sure to display it. 
			if ( !this.RulesIsValid )
				throw(new Exception(this.RulesErrorMessage));
		}

		protected void ApplyEditViewValidationEventRules(string sEDIT_NAME)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyEditViewRules(sEDIT_NAME, this, "VALIDATION_EVENT_XOML", null);
			// 04/28/2011 Paul.  If there was an rules error, then make sure to display it. 
			if ( !this.RulesIsValid )
				throw(new Exception(this.RulesErrorMessage));
		}

		protected void ApplyEditViewPreSaveEventRules(string sEDIT_NAME, DataRow row)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyEditViewRules(sEDIT_NAME, this, "PRE_SAVE_EVENT_XOML", row);
			// 08/08/2016 Paul.  Apply Business Process Rules. 
			WF4ApprovalActivity.ApplyEditViewPreSaveEventRules(Application, L10n, sEDIT_NAME, this, row);
			// 04/28/2011 Paul.  If there was an rules error, then make sure to display it. 
			if ( !this.RulesIsValid )
				throw(new Exception(this.RulesErrorMessage));
		}

		// 12/10/2012 Paul.  Provide access to the item data. 
		protected void ApplyEditViewPostSaveEventRules(string sEDIT_NAME, DataRow row)
		{
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyEditViewRules(sEDIT_NAME, this, "POST_SAVE_EVENT_XOML", row);
			// 04/28/2011 Paul.  Throwing an exception here does not make sense as it would block the redirect after a successful save. 
		}

		// 11/11/2010 Paul.  Change to Pre Load and Post Load. 
		protected void ApplyDetailViewPreLoadEventRules(string sDETAIL_NAME, DataRow row)
		{
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyDetailViewRules(sDETAIL_NAME, this, "PRE_LOAD_EVENT_XOML", row);
			// 04/28/2011 Paul.  We don't want to throw an exception here because it would prevent other core logic. 
		}

		protected void ApplyDetailViewPostLoadEventRules(string sDETAIL_NAME, DataRow row)
		{
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyDetailViewRules(sDETAIL_NAME, this, "POST_LOAD_EVENT_XOML", row);
			// 04/28/2011 Paul.  If there was an rules error, then make sure to display it. 
			if ( !this.RulesIsValid )
				throw(new Exception(this.RulesErrorMessage));
		}

		// 11/22/2010 Paul.  Apply Business Rules. 
		protected void ApplyGridViewRules(string sGRID_NAME, DataTable dt)
		{
			sGRID_NAME = sGRID_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ApplyGridViewRules(sGRID_NAME, this, "PRE_LOAD_EVENT_XOML", "POST_LOAD_EVENT_XOML", dt);
		}
		#endregion

		#region Append Grid
		protected void AppendGridColumns(SplendidGrid grd, string sGRID_NAME)
		{
			AppendGridColumns(grd, sGRID_NAME, null);
		}

		// 02/08/2008 Paul.  We need to build a list of the fields used by the search clause. 
		protected void AppendGridColumns(SplendidGrid grd, string sGRID_NAME, UniqueStringCollection arrSelectFields)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sGRID_NAME = sGRID_NAME + (this.IsMobile ? ".Mobile" : "");
			grd.AppendGridColumns(sGRID_NAME, arrSelectFields, null);
			// 10/11/2017 Paul.  Add Archive relationship view. Remove ARCHIVE_VIEW if this is not enabled. 
			if ( arrSelectFields != null && arrSelectFields.Contains("ARCHIVE_VIEW") )
			{
				if ( !this.ArchiveViewExists() )
					arrSelectFields.Remove("ARCHIVE_VIEW");
			}
		}

		// 03/01/2014 Paul.  Add Preview button. 
		protected void AppendGridColumns(SplendidGrid grd, string sGRID_NAME, UniqueStringCollection arrSelectFields, CommandEventHandler Page_Command)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sGRID_NAME = sGRID_NAME + (this.IsMobile ? ".Mobile" : "");
			grd.AppendGridColumns(sGRID_NAME, arrSelectFields, Page_Command);
			// 10/11/2017 Paul.  Add Archive relationship view. Remove ARCHIVE_VIEW if this is not enabled. 
			if ( arrSelectFields != null && arrSelectFields.Contains("ARCHIVE_VIEW") )
			{
				if ( !this.ArchiveViewExists() )
					arrSelectFields.Remove("ARCHIVE_VIEW");
			}
		}
		#endregion
#endif

		#region Localization
		public TimeZone GetT10n()
		{
			Guid   gTIMEZONE = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			T10n = TimeZone.CreateTimeZone(gTIMEZONE);
			return this.T10n;
		}

		public L10N GetL10n()
		{
			return this.L10n;
		}

		public Currency GetC10n()
		{
			// 05/09/2006 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( C10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				C10n = Context.Items["C10n"] as Currency;
				if ( C10n == null )
				{
					Guid gCURRENCY_ID = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
					// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
					C10n = Currency.CreateCurrency(gCURRENCY_ID);
				}
			}
			return C10n;
		}

		protected void SetC10n(Guid gCURRENCY_ID)
		{
			// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
			C10n = Currency.CreateCurrency(gCURRENCY_ID);
			// 07/28/2006 Paul.  We cannot set the CurrencySymbol directly on Mono as it is read-only.  
			// Just clone the culture and modify the clone. 
			CultureInfo culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
			culture.NumberFormat.CurrencySymbol   = C10n.SYMBOL;
			Thread.CurrentThread.CurrentCulture   = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		protected void SetC10n(Guid gCURRENCY_ID, float fCONVERSION_RATE)
		{
			// 04/30/2016 Paul.  Require the Application so that we can get the base currency. 
			C10n = Currency.CreateCurrency(gCURRENCY_ID, fCONVERSION_RATE);
			// 07/28/2006 Paul.  We cannot set the CurrencySymbol directly on Mono as it is read-only.  
			// Just clone the culture and modify the clone. 
			CultureInfo culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
			culture.NumberFormat.CurrencySymbol   = C10n.SYMBOL;
			Thread.CurrentThread.CurrentCulture   = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}
		#endregion
	}
}

