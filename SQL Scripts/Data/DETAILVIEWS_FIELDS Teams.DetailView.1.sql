-- 10/15/2006 Paul.  Add support for Teams. 
-- 04/12/2016 Paul.  Add parent team and custom fields. 
-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
-- 11/11/2020 Paul.  React requires full url in format. 
-- delete from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Teams.DetailView'
if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Teams.DetailView' and DELETED = 0) begin -- then
	print 'DETAILVIEWS_FIELDS Teams.DetailView';
	exec dbo.spDETAILVIEWS_InsertOnly          'Teams.DetailView', 'Teams', 'vwTEAMS_Edit', '15%', '35%', null;
	exec dbo.spDETAILVIEWS_FIELDS_InsBound     'Teams.DetailView'    ,  0, 'Teams.LBL_NAME'                  , 'NAME'                             , '{0}'        , null;
	exec dbo.spDETAILVIEWS_FIELDS_InsHyperLink 'Teams.DetailView'    ,  1, 'Teams.LBL_PARENT_NAME'           , 'PARENT_NAME'                      , '{0}'        , 'PARENT_ID' , '~/Administration/Teams/view.aspx?ID={0}', null, null;
	exec dbo.spDETAILVIEWS_FIELDS_InsertOnly   'Teams.DetailView'    ,  2, 'TextBox', 'Teams.LBL_DESCRIPTION', 'DESCRIPTION', null, null, null, null, null, 3, null;
end else begin
	-- 04/12/2016 Paul.  Add parent team and custom fields. 
	-- 04/28/2016 Paul.  Rename parent to PARENT_ID. 
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Teams.DetailView' and DATA_FIELD = 'PARENT_TEAM_NAME' and DELETED = 0) begin -- then
		update DETAILVIEWS_FIELDS
		   set DATA_FIELD        = 'PARENT_NAME'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Teams.DetailView'
		   and DATA_FIELD        = 'PARENT_TEAM_NAME'
		   and DELETED           = 0;
	end -- if;
	if not exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Teams.DetailView' and DATA_FIELD = 'PARENT_NAME' and DELETED = 0) begin -- then
		update DETAILVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Teams.DetailView'
		   and FIELD_INDEX      >= 1
		   and DELETED           = 0;
		update DETAILVIEWS_FIELDS
		   set COLSPAN           = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Teams.DetailView'
		   and DATA_FIELD        = 'NAME'
		   and COLSPAN           = 3
		   and DELETED           = 0;
		exec dbo.spDETAILVIEWS_FIELDS_InsHyperLink 'Teams.DetailView'    ,  1, 'Teams.LBL_PARENT_NAME'           , 'PARENT_NAME'                      , '{0}'        , 'PARENT_ID' , '~/Administration/Teams/view.aspx?ID={0}', null, null;
	end -- if;

	-- 11/11/2020 Paul.  React requires full url in format. 
	if exists(select * from DETAILVIEWS_FIELDS where DETAIL_NAME = 'Teams.DetailView' and DATA_FIELD = 'PARENT_NAME' and URL_FORMAT = 'view.aspx?ID={0}' and DELETED = 0) begin -- then
		update DETAILVIEWS_FIELDS
		   set URL_FORMAT        = '~/Administration/Teams/view.aspx?ID={0}'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where DETAIL_NAME       = 'Teams.DetailView'
		   and DATA_FIELD        = 'PARENT_NAME'
		   and URL_FORMAT        = 'view.aspx?ID={0}'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

