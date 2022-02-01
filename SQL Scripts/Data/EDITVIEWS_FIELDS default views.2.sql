

set nocount on;
GO

if exists(select distinct EDITVIEWS_FIELDS.EDIT_NAME
            from           EDITVIEWS_FIELDS
           left outer join EDITVIEWS_FIELDS EDITVIEWS_FIELDS_DEFAULTS
                        on EDITVIEWS_FIELDS_DEFAULTS.EDIT_NAME    = EDITVIEWS_FIELDS.EDIT_NAME
                       and EDITVIEWS_FIELDS_DEFAULTS.DEFAULT_VIEW = 1
                       and EDITVIEWS_FIELDS_DEFAULTS.DELETED      = 0
           where EDITVIEWS_FIELDS_DEFAULTS.ID is null
             and EDITVIEWS_FIELDS.DELETED = 0) begin -- then

	-- 09/19/2012 Paul.  Add new fields.  Should have done this long ago. 
	insert into EDITVIEWS_FIELDS(ID, DEFAULT_VIEW, CREATED_BY, DATE_ENTERED, MODIFIED_USER_ID, DATE_MODIFIED, DATE_MODIFIED_UTC, EDIT_NAME, FIELD_INDEX, FIELD_TYPE, DATA_LABEL, DATA_FIELD, DATA_FORMAT, DISPLAY_FIELD, CACHE_NAME, DATA_REQUIRED, UI_REQUIRED, ONCLICK_SCRIPT, FORMAT_SCRIPT, FORMAT_TAB_INDEX, FORMAT_MAX_LENGTH, FORMAT_SIZE, FORMAT_ROWS, FORMAT_COLUMNS, COLSPAN, ROWSPAN, FIELD_VALIDATOR_ID, FIELD_VALIDATOR_MESSAGE, MODULE_TYPE, TOOL_TIP, RELATED_SOURCE_MODULE_NAME, RELATED_SOURCE_VIEW_NAME, RELATED_SOURCE_ID_FIELD, RELATED_SOURCE_NAME_FIELD, RELATED_VIEW_NAME, RELATED_ID_FIELD, RELATED_NAME_FIELD, RELATED_JOIN_FIELD, PARENT_FIELD)
	select                  newid(), 1           , CREATED_BY, DATE_ENTERED, MODIFIED_USER_ID, DATE_MODIFIED, DATE_MODIFIED_UTC, EDIT_NAME, FIELD_INDEX, FIELD_TYPE, DATA_LABEL, DATA_FIELD, DATA_FORMAT, DISPLAY_FIELD, CACHE_NAME, DATA_REQUIRED, UI_REQUIRED, ONCLICK_SCRIPT, FORMAT_SCRIPT, FORMAT_TAB_INDEX, FORMAT_MAX_LENGTH, FORMAT_SIZE, FORMAT_ROWS, FORMAT_COLUMNS, COLSPAN, ROWSPAN, FIELD_VALIDATOR_ID, FIELD_VALIDATOR_MESSAGE, MODULE_TYPE, TOOL_TIP, RELATED_SOURCE_MODULE_NAME, RELATED_SOURCE_VIEW_NAME, RELATED_SOURCE_ID_FIELD, RELATED_SOURCE_NAME_FIELD, RELATED_VIEW_NAME, RELATED_ID_FIELD, RELATED_NAME_FIELD, RELATED_JOIN_FIELD, PARENT_FIELD
	  from EDITVIEWS_FIELDS
	 where DELETED = 0
	   and EDIT_NAME in (select distinct EDITVIEWS_FIELDS.EDIT_NAME
	                       from           EDITVIEWS_FIELDS
	                      left outer join EDITVIEWS_FIELDS EDITVIEWS_FIELDS_DEFAULTS
	                                   on EDITVIEWS_FIELDS_DEFAULTS.EDIT_NAME    = EDITVIEWS_FIELDS.EDIT_NAME
	                                  and EDITVIEWS_FIELDS_DEFAULTS.DEFAULT_VIEW = 1
	                                  and EDITVIEWS_FIELDS_DEFAULTS.DELETED      = 0
	                      where EDITVIEWS_FIELDS_DEFAULTS.ID is null
	                        and EDITVIEWS_FIELDS.DELETED = 0);
end -- if;
GO

set nocount off;
GO

/* -- #if Oracle
	EXCEPTION
		WHEN NO_DATA_FOUND THEN
			StoO_selcnt := 0;
		WHEN OTHERS THEN
			RAISE;
	END;
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spEDITVIEWS_FIELDS_DefaultViews()
/

call dbo.spSqlDropProcedure('spEDITVIEWS_FIELDS_DefaultViews')
/

-- #endif IBM_DB2 */

