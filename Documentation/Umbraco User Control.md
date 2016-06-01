# Umbraco User Control

## Introduction
Search for user either by email or user name.

If the user exists, their details will be displayed along with the following options:

*	Reset password
*	Lock (unlock) account
*	User permissions

## Note
Umbraco does not apply default permissions to all nodes. A user is assigned to a single User Type, which has a default set of permissions. That default set is returned whenever permissions for a node are requested via the API. Only if the permissions have been altered for a specific node, are they returned instead.

I have removed the need for / use of the local permissions table. Permissions are collected directly from Umbraco when building the tree.

## Reset password
Sends an email to the user, containing a link to a page where they can change their password. The link is valid for 24 hours.

## Lock (unlock) account
Lock or enable the user account.

## User permissions
Displays the current content tree, showing the permissions assigned to the selected user.

Each level of content tree is retrieved when its parent is expanded, hence the icon may change from a document to a folder if there are sub-nodes.

Ticking and clearing the checkbox against each page updates the permissions immediately.

## Sync user permissions
This will obtain the current set of page permissions from Umbraco and replace the page permissions recorded in the local database.

**Note**: This seems unnecessary as this system should always ensure that it is synchronised with the website. The website is the source, so I have changed this application to update its local permissions store on page load. I have also hidden the button, as it should not be needed.

## Copy user permissions
Replace permissions for the current user with those from another user.

## Set editor
What is this for?

**Note**: There does not appear to be a reason for this button / function, so I have hidden it.

## Tools
There is a tools folder (/tools/) with the following pages / options:

### Lookup permissions

Check which pages a user has access to, using either email address or username. Alternatively, you can check which users have permissions for a specific page.

### Lookup pages without web authors

Search the entire site and list pages that do not have any web authors assigned.

## IIS / Server Setup

Ensure Windows Authentication is installed: 

Control Panel -> Programs and Features -> Turn Windows features on and off -> IIS -> WWW Services -> Security -> Windows Authentication

Disable Anonymous Authentication for the website in IIS, enable Windows Authentication.

## Permission levels

Permission levels are assigned to Active Directory groups named in `web.config`.

**WebServices**: Has full permission to the entire application.

**ServiceDesk**: Has permission to lookup a user and initiate a password reset.

## Questions

1.	How will this application be used and protected from unauthorised use?
2.	Is there any format requirements for login ID, e.g. email address?
