# sitecoreindexupdater
I have created an index update tool for forcing through an index update on an item or an item and its descendants.
For versions: 9.3, 10.0

Install the index updater by installing the Sitecore installation package.
The tool is a Speak application and accessed via the Sitecore start bar under All Applications. 
It can also be accessed from the following URL: https://[yourwebsite]/sitecore/client/your-apps/indexupdate/.

In the interface, select an item from the tree (the source database can be changed from the drop down list at the top). 
Selecting an item will populate the ID box in the main window or this can ben entered manually. 
Choose an index from the drop down list in the main window. 
The recursive check box will update the selected item and its descendants from the items in the source database. 
Click either the update or delete button depending on the operation you want to perform. Results will appear in the text area below.

The index updater can be found in the core database under /sitecore/client/Your Apps/IndexUpdate.
Extra databases can be added to the drop down list by creating a 'Dictionary entry' item under /sitecore/client/Your Apps/IndexUpdate/Databases.
Index names can be added to the drop down list by creating a Dictionary entry item under /sitecore/client/Your Apps/IndexUpdate/Indexes.
