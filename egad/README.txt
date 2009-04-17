+---------+
| e g a d |
+---------+

/* egad
 * ----
 * 
 * A simple console Active Directory LDAP Objects browser in C#.
 *  
 *      Website & source:   http://github.com/arth/egad
 *      Windows binary:     http://wrudm.poorcoding.com/art/pub/egad/deploy
 * 
 * COPYRIGHT:
 *      All rights reserved by the author, except:
 *          This program and source is free to use & distribute, 
 *          without restrictions EXCEPT:
 *              - Thou shall not charge money for egad in any way.
 *              - Thou shall not claim mine work as thine own.
 *      
 * AUTHOR: art@poorcoding.com
 *
 */
 

WEBSITE
-------

	Latest binary builds for Windows are available from the deployment URL:
		http://wrudm.poorcoding.com/art/pub/egad/deploy/
		
	Source available at:
		http://github.com/arth/egad

GENERAL
-------

	?, h, help
		Display the contents of the help file.
		
	quit, exit
		Quit egad.
	

CONFIGURATION
-------------

	connect
		Connect to the Directory Object.
		
	disconnect
		Disconnect from the Directory Object.
		
	impersonate
		Toggle user impersonation for authentication.
		
	ldapstring
		DEBUG: Manually set an LDAP connection string.
		
	settings
		Configure Domain Name, Domain Controller, Directory Object path,
		and authentication credentials.
		
		Warning: failure to run through the settings will result in reduced
		functionality and possibly strangeness in general.


INFORMATION
-----------

	c, children
		Display Object's children.
		
	g, guid
		Display Object's GUID.
		
	getpath
		Display Object's LDAP path.
		
	getuser, whoami
		Display current username.
		
	p, properties
		Display Object's properties.


BROWSING OBJECTS
----------------
	cd
		Change to a child object.
		
	cn
		DEBUG: Set path directly.
		
--------------------------------------------------------------------------------
  It is not suggested to use the commands marked DEBUG as they may result in
  unexpected behavior. Use them at your own risk.
--------------------------------------------------------------------------------

EXAMPLE
-------

	A typical session may consist of:
	
		1. Running egad.
		
		2. Configuring the domain, dc, path and user parameters: "settings".
		   You will be connected automatically after configuration.
		   
		3. Browse objects. For example: "children".
