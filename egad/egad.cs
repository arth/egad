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

using System;
using System.DirectoryServices;

namespace egad
{
    class egad
    {
        private static string preprompt = "";
        private static string dc_server = "";       // Domain Controller name or IP.
        private static string domain_fqdn = "";     // FQDN domain name (i.e., not the short name!)
        private static string domain_ldap = "";     // DC=fqdn LDAP formatted domain string
        private static string cn_path = "";         // CN=objects LDAP formatted string
        private static string LDAPString = "";      // Complete LDAP Path string

        private static bool is_running = true;
        private static bool is_connected = false;
        private static bool impersonate = false;

        private static string username = "";
        private static string password = "";

        private static DirectoryEntry ent;

        private static string[] cn_tree = new string[] { };

        static void Main(string[] args)
        {
            Console.Title = "egad";
            Settings();
            Prompt();
        }

        private static string BuildDCString()
        {
            string dc_string = "";

            if (domain_fqdn.Length > 0)
            {
                string[] fqdn = domain_fqdn.Split('.');

                foreach (string f in fqdn)
                {
                    //Console.WriteLine(f);
                    dc_string += "DC=" + f + ",";
                }

                // remove trailing comma
                dc_string = dc_string.Remove(dc_string.Length - 1, 1);

                //Console.WriteLine(dc_string);
                return dc_string;
            }

            return dc_string;
        }

        private static void BuildLDAPString()
        {
            // Build LDAPString
            if (dc_server.Length > 0)
            {
                LDAPString = "LDAP://" + dc_server;

                if (cn_path.Length > 0 && !cn_path.Equals('/'))
                {
                    LDAPString = LDAPString + '/' + cn_path;

                    if (domain_fqdn.Length > 0)
                    {
                        LDAPString = LDAPString + ',' + domain_ldap;
                    }
                }
            }
        }

        private static void ChangeCN()
        {
            if (is_connected)
            {
                if (dc_server.Length < 1 || domain_fqdn.Length < 1)
                {
                    Console.WriteLine("You can't change paths until you provide domain and DC information in \"settings\"");
                }
                else
                {
                    Console.WriteLine("Current path: {0}\n{1}", cn_path, LDAPString);
                    Console.WriteLine("\nCurrent path children:");
                    Children();
                    Console.Write("Change to child (ENTER for none, .. to go to parent): ");
                    string cmd = Console.ReadLine().Trim();

                    if (cmd.Length < 1)
                    {
                        Console.WriteLine("NO CHANGE");
                    }
                    else if (cmd.Equals(".."))
                    {
                        try
                        {
                            //Console.WriteLine("PARENT");
                            if (cn_path.Length > 0)
                            {
                                ent = ent.Parent;
                                preprompt = ent.Name.ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionOutput(e);
                        }
                    }
                    else
                    {
                        // Check that the cmd exists as a child of current cn_path

                        if (ent.Children.Find(cmd) != null)
                        {
                            Console.WriteLine("Found child {0} in this Object", cmd);
                        }

                        if (cn_path.Length > 0)
                        {
                            cn_path = cmd + "," + cn_path;
                        }
                        else
                        {
                            cn_path = cmd;
                        }

                        preprompt = ent.Name.ToString();

                        BuildLDAPString();

                        if (is_connected)
                        {
                            ConnectionEnd();
                        }

                        ConnectionStart();
                    }
                }
            }
        }

        private static void Children()
        {
            if (is_connected)
            {
                try
                {
                    foreach (DirectoryEntry child in ent.Children)
                        Console.WriteLine(child.Name);

                    //Console.ReadKey(true);
                }
                catch (Exception e)
                {
                    ExceptionOutput(e);
                }
            }
            else
            {
                Console.WriteLine("You are not connected.");
            }
        }

        private static void ConnectionStart()
        {
            if (is_connected)
            {
                Console.WriteLine("You are already connected. Disconnect first.");
            }
            else
            {
                try
                {
                    if (impersonate)
                    {

                        // if no username or password, then don't do anything.
                        if (username.Length < 1 || password.Length < 1)
                        {
                            Console.WriteLine("Error: no username and/or password set. Use the \"settings\" command when \"impersonate\" has been enabled.");
                            return;
                        }
                        else
                        {
                            ent = new DirectoryEntry(LDAPString, username, password);
                        }
                    }
                    else
                    {
                        ent = new DirectoryEntry(LDAPString);
                    }

                    preprompt = ent.Name.ToString();

                    Console.WriteLine("LDAP path: {0} ({1})", ent.Path, preprompt);

                    is_connected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caught: {0}", e.Message.ToString());
                    is_connected = false;
                }
            }
        }

        private static void ConnectionEnd()
        {
            if (is_connected == false)
            {
                Console.WriteLine("Nothing to disconnect from.");
            }
            else
            {
                try
                {
                    ent.Close();
                    is_connected = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caught: {0}", e.Message.ToString());
                }
            }
        }

        private static void ExceptionOutput(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Exception: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e.Message.ToString());
            Console.ResetColor();
        }

        private static void GetGuid()
        {
            if (is_connected)
            {
                try
                {
                    Console.WriteLine("GUID: {0}", ent.Guid.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.Message.ToString());
                }
            }
            else
            {
                Console.WriteLine("Not connected to any AD object");
            }
        }

        private static void GetPath()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nLDAP path: {0}\n", ent.Path.ToString());
                Console.ResetColor();
            }
            catch (Exception e)
            {
                ExceptionOutput(e);
            }
        }

        private static void GetUser()
        {
            if (impersonate)
            {
                Console.WriteLine("Impersonating: {0} ({1})", ent.Username.ToString(), ent.AuthenticationType.ToString());
            }
            else
            {
                Console.WriteLine("Environment defaults: {0} ({1})", Environment.UserName.ToString(), ent.AuthenticationType.ToString());
            }
        }

        private static void Help()
        {

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n  EGAD HELP  \n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("GENERAL");
            Console.WriteLine("-------");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("?, h, help");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Display the contents of the help file.");
            Console.WriteLine("quit, exit");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Quit egad.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("CONFIGURATION");
            Console.WriteLine("-------------");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("connect");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Connect to the Directory Object.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("disconnect");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Disconnect from the Directory Object.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("impersonate");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Toggle user impersonation for authentication.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("settings");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Configure Domain Name, Domain Controller, Directory Object path,");
            Console.WriteLine("  and authentication credentials.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Warning: failure to run through the settings will result in reduced");
            Console.WriteLine("functionality and possibly strangeness in general.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("INFORMATION");
            Console.WriteLine("-----------");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("c, children");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Display Object's children.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("g, guid");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Display Object's GUID.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("getpath");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Display Object's LDAP path.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("getuser, whoami");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Display current username.");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("p, properties");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Display Object's properties.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("BROWSING OBJECTS");
            Console.WriteLine("----------------");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("cd");
            Console.ForegroundColor = ConsoleColor.DarkYellow; Console.WriteLine("  Change to a child object.");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nNote: Until you provide a domain controller and FQDN domain name");
            Console.WriteLine("in the \"settings\", egad will use the default domain, default domain");
            Console.WriteLine("controller and default username; you will be unable to change Objects");
            Console.WriteLine("or impersonate.\n");
            
            Console.ResetColor();

        }

        private static void ImpersonateToggle()
        {
            if (impersonate)
            {
                impersonate = false;
                Console.WriteLine("Impersonation is off.");
            }
            else
            {
                impersonate = true;
                Console.WriteLine("Impersonation is on.");
            }
        }

        private static void Prompt()
        {
            while (is_running)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("{0}> ", preprompt);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                string cmd = Console.ReadLine().Trim();
                Console.ResetColor();

                string cmdlow = cmd.ToLower();
                if (cmdlow.Equals("quit") || cmdlow.Equals("exit"))
                {
                    is_running = false;
                }
                else if (cmdlow.Equals("connect"))
                {
                    ConnectionStart();
                }
                else if (cmdlow.Equals("disconnect"))
                {
                    ConnectionEnd();
                }
                else if (cmdlow.Equals("properties") || cmdlow.Equals("p"))
                {
                    Properties();
                }
                else if (cmdlow.Equals("children") || cmdlow.Equals("c"))
                {
                    Children();
                }
                else if (cmdlow.Equals("ldapstring"))
                {
                    SetLDAPString();
                }
                else if (cmdlow.Equals("settings"))
                {
                    Settings();
                }
                else if (cmdlow.Equals("guid") || cmdlow.Equals("g"))
                {
                    GetGuid();
                }
                else if (cmdlow.Equals("cn"))
                {
                    SetCNPath();
                }
                else if (cmdlow.StartsWith("cn "))
                {
                    string cn_arg = cmd.Substring(3);
                    SetCNPath(cn_arg);
                }
                else if (cmdlow.Equals("help") || cmdlow.Equals("h") || cmdlow.Equals("?"))
                {
                    Help();
                }
                else if (cmdlow.Equals("cd"))
                {
                    ChangeCN();
                }
                else if (cmdlow.Equals("impersonate"))
                {
                    ImpersonateToggle();
                }
                else if (cmdlow.Equals("getpath"))
                {
                    GetPath();
                }
                else if (cmdlow.Equals("getuser") || cmdlow.Equals("whoami"))
                {
                    GetUser();
                }
                else
                {
                    Console.WriteLine("Unrecognized command.");
                }
            }
        }

        private static void Properties()
        {
            if (is_connected)
            {
                try
                {

                    PropertyCollection props = ent.Properties;

                    foreach (string prop_name in props.PropertyNames)
                        Console.WriteLine("{0} = {1}", prop_name, props[prop_name].Value.ToString());

                    //Console.ReadKey(true);
                }
                catch (DirectoryServicesCOMException e)
                {
                    Console.WriteLine("DirectoryServicesCOMException: {0}", e.Message.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.Message.ToString());
                }
            }
            else
            {
                Console.WriteLine("You are not connected.");
            }
        }

        private static void Settings()
        {
            // Display and set domain controller

            string dc;
            if (dc_server.Equals(""))
                dc = "<default domain controller>";
            else
                dc = dc_server;

            Console.WriteLine("\nDomain Controller: {0}", dc);

            Console.Write("New DC (ENTER for no change): ");
            string cmd = Console.ReadLine();

            if (cmd.Length > 0)
            {
                dc_server = cmd;
                Console.WriteLine("DC changed to \"{0}\"", dc_server);
            }
            else
            {
                Console.WriteLine("DC unchanged.");
            }

            // Display and set domain FQDN
            string domain;
            if (domain_fqdn.Equals(""))
                domain = "<default domain>";
            else
                domain = domain_fqdn;

            Console.WriteLine("\nDomain: {0}", domain);

            Console.Write("New Domain (ENTER for no change): ");
            cmd = Console.ReadLine();

            if (cmd.Length > 0)
            {
                domain_fqdn = cmd;
                domain_ldap = BuildDCString();
                Console.WriteLine("Domain changed to \"{0}\" (\"{1}\")", domain_fqdn, domain_ldap);
            }
            else
            {
                Console.WriteLine("Domain unchanged.");
            }

            // Display and set CN path
            if (dc_server.Length > 0 && domain_fqdn.Length > 0)
            {
                Console.WriteLine("\nCurrent CN Path: {0}", cn_path);

                Console.Write("CN Path (ENTER for no change, / for none): ");
                cmd = Console.ReadLine();

                if (cmd.Trim().Equals("/"))
                {
                    cn_path = "";
                    Console.WriteLine("CN set to none.");
                }
                else if (cmd.Length > 0)
                {
                    cn_path = cmd;
                    Console.WriteLine("CN changed to \"{0}\"", cn_path);
                }
                else
                {
                    Console.WriteLine("CN unchanged.");
                }
                Console.WriteLine("");
            }

            // Get Username and Password, if impersonate is on.
            if (impersonate && dc_server.Length > 0 && domain_fqdn.Length > 0)
            {
                Console.Write("Username (ENTER for no change): ");
                cmd = Console.ReadLine().Trim();
                if (cmd.Length < 1)
                {
                    Console.WriteLine("Username unchanged.");
                }
                else { 
                    username = cmd; 
                }

                Console.Write("Password: ");    // TODO: Blank output.
                cmd = Console.ReadLine().Trim();
                if (cmd.Length < 1)
                {
                    Console.WriteLine("Password unchanged.");
                }
                else
                {
                    password = cmd;
                }

                
                Console.WriteLine(username, password);
            }

            // Build new LDAP string to object.
            BuildLDAPString();
            Console.WriteLine();

            if (is_connected)
            {
                ConnectionEnd();
            }

            ConnectionStart();

            //Console.WriteLine("\nLDAP path: {0}", LDAPString);
        }

        private static void SetCNPath()
        {
            if (dc_server.Length < 1 || domain_fqdn.Length < 1)
            {
                Console.WriteLine("You can't change paths until you provide domain and DC information in \"settings\"");
            }
            else
            {
                // Display and set CN path
                Console.WriteLine("\nCurrent CN Path: {0}", cn_path);

                Console.Write("CN Path (ENTER for no change, / for none): ");
                string cmd = Console.ReadLine();

                if (cmd.Trim().Equals("/"))
                {
                    cn_path = "";
                    Console.WriteLine("CN set to none.");
                }
                else if (cmd.Length > 0)
                {
                    cn_path = cmd;
                    Console.WriteLine("CN changed to \"{0}\"", cn_path);
                }
                else
                {
                    Console.WriteLine("CN unchanged.");
                }

                BuildLDAPString();

                if (is_connected)
                {
                    ConnectionEnd();
                }

                ConnectionStart();
            }
        }

        private static void SetCNPath(string cn_arg)
        {
            if (dc_server.Length < 1 || domain_fqdn.Length < 1)
            {
                Console.WriteLine("You can't change paths until you provide domain and DC information in \"settings\"");
            }
            else
            {
                if (cn_arg.Length > 0)
                {
                    cn_path = cn_arg;

                    BuildLDAPString();

                }

                if (is_connected)
                {
                    ConnectionEnd();
                }

                ConnectionStart();
            }
        }

        private static void SetLDAPString()
        {
            Console.WriteLine("Current LDAP path: {0}", LDAPString);
            Console.Write("New path: ");

            string new_path = Console.ReadLine();

            if (new_path.Length > 0 && new_path != null)
            {
                LDAPString = new_path;

                //ent = new DirectoryEntry(LDAPString);

                Console.Write("Connect to new LDAP path now? (y/n) : ");
                if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
                {
                    Console.WriteLine();

                    if (is_connected)
                    {
                        ConnectionEnd();
                    }

                    ConnectionStart();
                }
                else
                {
                    Console.WriteLine("\nLDAP path changed to: {0}", LDAPString);
                }
            }
            else
            {
                Console.WriteLine("LDAP path unchanged.");
            }
        }

    }
}
