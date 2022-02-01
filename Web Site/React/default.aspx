<%@ Page language="c#" EnableTheming="true" AutoEventWireup="true" Inherits="SplendidCRM.SplendidPage" %>
<%@ Import Namespace="System.Diagnostics" %>
<script runat="server">
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

override protected bool AuthenticationRequired()
{
	return false;
}

private void Page_PreInit(object sender, EventArgs e)
{
	// 06/18/2015 Paul.  Setting the theme to an empty string should stop the insertion of styles from the Themes folder. 
	// 09/02/2019 Paul.  Going to try themes on React Client. 
	//this.Theme = "";
}

private void Page_Load(object sender, System.EventArgs e)
{
	Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
	if ( !IsPostBack )
	{
		try
		{
			//System.Web.UI.ScriptManager mgrAjax = System.Web.UI.ScriptManager.GetCurrent(this.Page);
			//ChatManager.RegisterScripts(Context, mgrAjax);
		}
		catch(Exception ex)
		{
			SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
		}
	}
}
</script>

<!DOCTYPE HTML>
<html id="htmlRoot" runat="server">

<head runat="server">
	<meta charset="UTF-8" />
	<link rel="shortcut icon" href="<%# Application["imageURL"] %>SplendidCRM_Icon.ico" />
	<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
	<base href="<%# Application["rootURL"] %>React/" />
	<title><%# L10n.Term(".LBL_BROWSER_TITLE") %></title>
</head>

<body>
	<!-- 09/08/2021 Paul.  Change to vh instead of height to allow to grow. -->
	<!-- 09/14/2021 Paul.  Remove height.  It is causing copyright to appear in the middle of a detail view. -->
	<div id="root"></div>
	<script type="text/javascript" src="dist/js/SteviaCRM.js?<%= (bDebug ? (DateTime.Now.ToFileTime().ToString()) : Sql.ToString(Application["SplendidVersion"])) %>"></script>

	<div id="divFooterCopyright" align="center" style="margin-top: 4px" class="copyRight">
		Copyright &copy; 2005-2021 <a id="lnkSplendidCRM" href="http://www.splendidcrm.com" target="_blank" class="copyRightLink">SplendidCRM Software, Inc.</a> All Rights Reserved.<br />
	</div>
</body>
</html>

