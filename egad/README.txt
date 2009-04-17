+---------+
| e g a d |
+---------+


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
	g, guid
	getpath
	getuser, whoami
	p, properties


BROWSING OBJECTS
----------------
	cd
		Change to a child object.
		
	cn
		DEBUG: Set path directly.


EXAMPLE
-------

	A typical session may consist of:
	
		1. Running egad.
		
		2. Configuring the domain, dc, path and user parameters: "settings".
		   You will be connected automatically after configuration.
		   
		3. Browse objects. For example: "children".
