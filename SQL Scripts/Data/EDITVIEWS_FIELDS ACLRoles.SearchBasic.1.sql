set nocount on;
GO

-- 03/30/2021 Paul.  Roles needed for React client. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ACLRoles.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ACLRoles.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'ACLRoles.SearchBasic'        , 'ACLRoles', 'vwACL_ROLES', '15%', '85%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'ACLRoles.SearchBasic'        ,  0, 'ACLRoles.LBL_NAME'                     , 'NAME'                        , 0, null, 100, 25, null;
end -- if;
GO

-- 06/02/2021 Paul.  Roles needed for React client. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'ACLRoles.SearchByUser';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ACLRoles.SearchByUser' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ACLRoles.SearchByUser';
	exec dbo.spEDITVIEWS_InsertOnly             'ACLRoles.SearchByUser'       , 'ACLRoles', 'vwACL_ROLES', '15%', '85%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'ACLRoles.SearchByUser'       ,  0, '.LBL_ASSIGNED_TO'                      , 'ID'                         , 0, null, 'AssignedUser' , null, null;
end -- if;
GO

-- 08/01/2016 Paul.  Roles needed for BPMN. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ACLRoles.SearchPopup' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ACLRoles.SearchPopup';
	exec dbo.spEDITVIEWS_InsertOnly             'ACLRoles.SearchPopup'        , 'ACLRoles', 'vwACL_ROLES', '15%', '85%', 1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'ACLRoles.SearchPopup'        ,  0, 'ACLRoles.LBL_NAME'                     , 'NAME'                        , 0, null, 100, 25, null;
end -- if;
GO

set nocount off;
GO


