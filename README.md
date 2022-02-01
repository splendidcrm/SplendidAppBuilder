# Splendid App Builder
We started with the well tested SplendidCRM and removed all the CRM parts. What was left was a generic enterprise application builder.

## Enterprise Requirements
An enterprise has a much higher set of requirements than your typical organization

### Authentication
Beyond username/password, an enterprise may require Windows Authentication, Active Directory Federation Services or Azure Active Directory.

#### Username/Password
This feature is controlled by IIS. Enable Anonymous Authentication and disable all others.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/IIS_Anonymous_Authentication.gif)

#### Windows Authentication
This feature is controlled by IIS. Enable Windows Authentication and disable all others.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/IIS_Windows_Authentication.gif)

#### Azure Active Directory
First, you must enable Anonymous Authentication in IIS. Then add the single sign on settings to Web.config.
You will need to register an application on your ADFS server to get the ClientId. You will need to get the thumbprin for the server, not the domain name.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Web.config_Azure.SingleSignon.gif)

#### Active Directory Federation Services
First, you must enable Anonymous Authentication in IIS. Then add the single sign on settings to Web.config.
You will need to register an application on Azure to get the AadClientId. The Realm is the APP ID URI in the App Properties panel.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Web.config_ADFS_4.0.SingleSignon.gif)

### Localization
An enterprise needs to support multiple languages, time zones and currencies simultaneously.

#### Language
We have two tables that we use to manage the display of labels in the user's perferred language. A LANGUAGES table and a TERMINOLOGY table.
The LANGUAGES table is simply a list of possible languages that can be enabled.
The TERMINOLOGY table contains all the labels and text for all modules and all supported languages. This table typically has 3000 terms per language. We only ship with english, so you will need to either import any other required language packs or define all the terms yourself for your desired language. Even a list, such as High, Medium, Low, would be represented in the TERMINOLOGY table for each supported language.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Localization_Terminology_Table.gif)

![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Localization_Terminology_Example.gif)

#### Date/Time and Time Zone
Date/time management is complicated. One would thing that a date without a time component would be easier, but it is not. This is because there are timezones and today for you could be yesterday or tomorrow for another enterprise user. This, of course, is due to time zones. And, to make things even more interesting, some countries decide the start and end of day-light saving time each year instead of by formula.
The way we manage date/time is to store values normalized to web server time, which is typically the same as the database server time. Any time a user inputs a date, we convert it from the user's time zone to local web server time. When we display a date, we convert from the local web server time to the user's time zone, then format the value using the user's preferred format for his locale.
Just when you thought you understood how dates are handled, we add to it the conversion to a json date format. We follow Microsoft's example and convert date/times to unix ticks so that we don't have to worry about parsing dates as text. A json date looks like \/Date(1451431683420)\/.
Use FromJsonDate and ToJsonDate to format date values.

> DATA_VALUE = FromJsonDate(DATA_VALUE, Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT());

#### Currency
In addition to having a CURRENCIES table, the way we suggest you manage currencies in data is to store three values. The first is the currency ID being used, the second value is the orignal input value, and the third is the value converted to a field we call USDOLLAR.
For example, the Opportunities table has a CURRENCY_ID, an AMOUNT and an AMOUNT_USDOLLAR field. The USDOLLAR field is just a normalized field. We called it USDOLLAR because SplendidCRM was developed in the United States, but you can treat this field as your primary enterprise currency.
When we display the value in a read-only mode, we start with the USDOLLAR and convert to the user's selected currency based on the conversion factor in the CURRENCIES table. When we edit the record, we use the original AMOUNT field.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Localization_Opportunities_Table.gif)

Sample usage:

> let dConvertedValue = C10n.ToCurrency(Sql.ToDecimal(DATA_VALUE));
> DATA_VALUE   = formatCurrency(dConvertedValue, oNumberFormat);
> DISPLAY_NAME = DATA_FORMAT.replace('{0:c}', DATA_VALUE);


### Access Rights
An enterprise has groups of people that play different roles or are on different teams. Access and permissions can get very complex.

#### ACL Roles
ACL roles are a module level security feature. They apply rules, such as view, edit, delete, import, export, etc. at the module level. Roles do not apply any record level access.
A user can have multiple roles assigned, and the final set that applies is a combination of them all, with the least access for each condition being used. For example, if Role A grants edit access to Acocunts, but Role B denies access, then a user assigned to both will not have access.
Final computed access rights are visible for each user on their profile page.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Access_Rights_ACL_Roles.gif)

#### User Assignment
Records are typically assigned to a user. And, in conjunction with ACL Roles, rules can be put in place that only allow an owner to take some actions. For exmaple, only an owner can edit a record, while all others can simply view the record.

When the require_user_assignment config flag is enabled, it forces the user to specify an owner in order to save the record. But, more importantly, it prevents other users from seeing the record unless those users are part of the management tree above the owner. The management tree is defined in the USERS table with the REPORTS_TO_ID field.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Access_Rights_Assigned_To.gif)

>  where 1 = 1
>   and (ASSIGNED_USER_ID is null or ASSIGNED_USER_ID = '00000000-0000-0000-0000-000000000002')

#### Team Assignment
When team management is enabled using the enable_team_management flag, records can be assigned to a team. Then, any user that is also assigned to that team will be able to see the record. Team assignment does not determine if the record can be edited, it just determines if the user has access to the record.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Access_Rights_Team.gif)

Teams can be configured to be required or optional using the require_team_management config flag. When teams are required, we use an inner join to determine if record is visible.

>        inner join vwTEAM_MEMBERSHIPS
>                on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID
>               and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = '00000000-0000-0000-0000-000000000002'

When team assignement is not required, we use an outer join. This makes the team field informational as it will not restrict access.

>   left outer join vwTEAM_MEMBERSHIPS
>                on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID
>               and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = '00000000-0000-0000-0000-000000000002'

### Runtime Customizations
An enterprise needs to be able to customize at design time as well as run time.

#### Custom Modules
When you create a custom module, a database table is created.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Runtime_Customizations_Module_Builder.gif)

You can later add custom fields to this table.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Runtime_Customizations_Custom_Field.gif)

#### Custom Layouts

There are three types of layouts, EditView, DetailView and ListView. EditView is for editing, DetailView is read-only view that typically includes relationship panels, and ListView is a searchable list.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Runtime_Customizations_Layout_EditView.gif)

#### Custom Code
Code that is loaded at runtime can either come from the REACT_CUSTOM_VIEWS database table, or from a folder called CustomViewsJS.
When using the CustomViewsJS approach, the folder tree is important. The Module name is at the top of the tree, with nodes for DetailViews, EditViews, ListViews and SubPanels.
A quick way to get started is to create the nodes for the module, then copy DetailView.tsx, EditView.tsx or ListView.tsx from the Views folder into the appropriate custom view folder. The imports in the file will be ignored.
![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Runtime_Customizations_Layout_REACT_CUSTOM_VIEWS.gif)

![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Runtime_Customizations_Layout_CustomViewsJS.gif)

### Server and Mobile
An enterprise needs to be able to run the same application/code on the server and on mobile devices.

#### ASP.Net Core
SplendidCRM has been on the ASP.Net platform since version 2.0 more than 15 years ago. This latest release updates the code to use ASP.Net Core. Although this is mostly positive, there are some negatives. For one, Microsoft has not released, nor is there a plan to release, the Report Viewer on ASP.Net Core. This means that generating reports is not an option. Also, Microsoft does not intend to port Windows Workflow Foundation on ASP.Net Core, so workflow and Business Process Management is not an option.
More information is available at ASP.NET documentation

#### React UI
React is a powerful component-based framework based on JavaScript. We prefer to code in TypeScript and have that compiled into JavaScript at build time, and at runtime as necessary. Although there is a React Native platform for mobile devices, we prefer the JavaScript implementation because it allows customizations to be compiled at runtime whereas React Native does not.
More information can be found at Reactjs.org
Being able to compile a React component at runtime is a game changer. It allows mobile applications to be updated without redeployment. As anyone who has published an application to the Apple App Store can attest, updating an app takes time, sometimes even weeks. By allowing runtime updates, that process can be skipped.

#### Cordova
Cordova is the majic that allows an application written in JavaScript to run as an app on mobile devices. It wraps the JavaScript into a native container that can access device features. The most important point is that the JavaScript build file used on the server is the exact same file used by Cordova and run on Android and iOS devices. The ultimate goal of write-once run-anywhere is achieved.
Building an Android app still requires the Java SDK and building an iOS app still requires MacOS running on Apple hardware.
More information can be found at Apache Cordova site.

### Database Management
An enterprise needs to manage more than just tables in a database. They typically need complex logic and a reliable deployment method. They typically have development, QA and production databases and they follow a detailed testing plan for each release.

#### SQL Functions, Stored Procedures and Views
An enterprise application typically needs to deal with lots of data and lost of users. This means that the system needs to be designed to be as fast as possible. There is no faster way than to use SQL views to return as much data as possible in a single request. When updating the database, using a SQL stored procedure is the most effecient approach.

![](http://www.splendidcrm.com/portals/0/SplendidAppBuilder/Database_Management_SQL_Scripts_SplendidApp.gif)

#### Building and Updating the Database
A key to the design of the Splendid system is the way we build our SQL scripts into a single file. The goal is to have a single SQL file that will create an empty database or upgrade an existing database from any previous version to the current version. There is nothing worse than an upgrade path that requires you to apply 7 separate upgrades in sequence to bring the product up to the current level. As we have proven, all it takes is a smart design.
Our solution is to separate the SQL objects by type, then using a file name numbering scheme to combine all the files into a single Build.sql file. The files within each folder are numbered 0 through 9. For example, the following creates a single View.sql file with all the views.

> copy /b Views\*.0.sql + Views\*.1.sql + Views\*.2.sql + Views\*.3.sql + Views\*.4.sql + Views\*.5.sql + Views\*.6.sql + Views\*.7.sql + Views\*.8.sql + Views\*.9.sql Views.sql

Once we have all the files for each seet of SQL objects combined into their respective files, we combine all of those into a single Build.sql

> copy /b ProceduresDDL.sql + BaseTables.sql + Tables.sql + Functions.sql + ViewsDDL.sql + Views.sql + Procedures.sql + Data.sql + Terminology.sql Build.sql

