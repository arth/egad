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
        //"LDAP://lorddenning";
        //LDAP://lorddenning/cn=users,dc=wrudm,dc=poorcoding,dc=com
        //LDAP://lorddenning/cn=art,cn=users,dc=wrudm,dc=poorcoding,dc=com

        private static bool is_running = true;
        private static bool is_connected = false;
        private static DirectoryEntry ent;


        static void Main(string[] args)
        {
            /*SetupConnection();

            Children();

            Properties();

            EndConnection();*/

            Prompt();
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
                if (cmdlow.Equals("quit"))
                {
                    is_running = false;
                }
                else if (cmdlow.Equals("connect"))
                {
                    SetupConnection();
                }
                else if (cmdlow.Equals("disconnect"))
                {
                    EndConnection();
                }
                else if (cmdlow.Equals("properties"))
                {
                    Properties();
                }
                else if (cmdlow.Equals("children"))
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
                else if (cmdlow.Equals("guid"))
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
                else if (cmdlow.Equals("help"))
                {
                    Help();
                }
                else
                {
                    Console.WriteLine("Unrecognized command.");
                }
            }
        }

        private static void Help()
        {
            Console.WriteLine("settings");
            Console.WriteLine("cn");

            Console.WriteLine("connect");
            Console.WriteLine("disconnect");

            Console.WriteLine("children");
            Console.WriteLine("guid");
            Console.WriteLine("properties");

            Console.WriteLine("quit");
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

            BuildLDAPString();

            // Connect to object Y/N

            /*Console.Write("Connect to new LDAP path now? (y/n) : ");
            if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
            {
                Console.WriteLine();

                if (is_connected)
                {
                    EndConnection();
                }

                SetupConnection();
            }
            else
            {
                Console.WriteLine("\nLDAP path changed to: {0}", LDAPString);
            }*/

            Console.WriteLine();

            if (is_connected)
            {
                EndConnection();
            }

            SetupConnection();

            //Console.WriteLine("\nLDAP path: {0}", LDAPString);
        }

        private static void SetCNPath()
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
                EndConnection();
            }

            SetupConnection();
        }

        private static void SetCNPath(string cn_arg)
        {
            if (cn_arg.Length > 0)
            {
                cn_path = cn_arg;

                BuildLDAPString();

            }

            if (is_connected)
            {
                EndConnection();
            }

            SetupConnection();
        }

        private static void Properties()
        {
            if (is_connected)
            {
                try {
                
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

        private static void SetupConnection()
        {
            if (is_connected)
            {
                Console.WriteLine("You are already connected. Disconnect first.");
            }
            else
            {
                try
                {
                    ent = new DirectoryEntry(LDAPString);
                    
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

        private static void EndConnection()
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

        private static string BuildDCString()
        {
            string dc_string = "";

            if (domain_fqdn.Length > 0)
            {
                string[] fqdn = domain_fqdn.Split('.');

                foreach (string f in fqdn)
                {
                    //Console.WriteLine(f);
                    dc_string += "DC="+f+",";
                }

                // remove trailing comma
                dc_string = dc_string.Remove(dc_string.Length - 1, 1);

                //Console.WriteLine(dc_string);
                return dc_string;
            }

            return dc_string;
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
                        EndConnection();
                    }

                    SetupConnection();
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
    }
}
