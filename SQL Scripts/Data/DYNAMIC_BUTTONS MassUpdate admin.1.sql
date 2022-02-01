print 'DYNAMIC_BUTTONS MassUpdate admin';

set nocount on;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'ACLRoles.MassUpdate' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS ACLRoles MassUpdate';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'ACLRoles.MassUpdate'      , 0, 'ACLRoles'      , 'delete', null, null, 'MassDelete', null, '.LBL_DELETE'         , '.LBL_DELETE'         , null, 'if ( !ValidateOne() ) return false;', null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Terminology.MassUpdate' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Terminology MassUpdate';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Terminology.MassUpdate'   , 0, 'Terminology'   , 'delete', null, null, 'MassDelete', null, '.LBL_DELETE'         , '.LBL_DELETE'         , null, 'if ( !ValidateOne() ) return false;', null;
end -- if;
GO

-- 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons.
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = '.MassUpdate' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS  MassUpdate';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    '.MassUpdate'      , 0, null, null, null, null, 'MassUpdate', null, '.LBL_UPDATE'         , '.LBL_UPDATE'         , null, 'if ( !ValidateOne() ) return false;', null;
end -- if;
GO

-- 05/01/2016 Paul.  We are going to prepopulate the currency table so that we can be sure to get the supported ISO values correct. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Currencies.MassUpdate' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Currencies MassUpdate';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Currencies.MassUpdate'      , 0, 'Currencies'      , 'edit'  , null, null, 'MassUpdate', null, '.LBL_UPDATE'         , '.LBL_UPDATE'         , null, 'if ( !ValidateOne() ) return false;', null;
end -- if;
GO

if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'Teams.MassUpdate' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS Teams MassUpdate';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'Teams.MassUpdate'         , 0, 'Teams'         , 'delete', null, null, 'MassDelete', null, '.LBL_DELETE'         , '.LBL_DELETE'         , null, 'if ( !ValidateOne() ) return false;', null;
end -- if;
GO


set nocount off;
GO


