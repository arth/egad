using System;
using System.DirectoryServices;

namespace egad
{
    class egad
    {
        private static string LDAPString = "LDAP://lorddenning/cn=users";
        //LDAP://lorddenning/cn=users,dc=wrudm,dc=poorcoding,dc=com

        private static bool is_running = true;

        private static DirectoryEntry ent;

        static void Main(string[] args)
        {
            /*SetupConnection();

            Children();

            Properties();

            EndConnection();*/

            Prompt();
        }

        private static void Properties()
        {
            try
            {
                PropertyCollection props = ent.Properties;

                foreach (string prop_name in props.PropertyNames)
                    Console.WriteLine("property: {0}", prop_name);

                Console.ReadKey(true);
            }
            catch (DirectoryServicesCOMException e)
            {
                Console.WriteLine("DirectoryServicesCOMException: {0}", e.ToString());
            }
        }

        private static void SetupConnection()
        {
            ent = new DirectoryEntry(LDAPString);
            Console.WriteLine("LDAP path: {0}", ent.Path);
        }

        private static void EndConnection()
        {
            ent.Close();
        }

        private static void Children()
        {
            try
            {
                foreach (DirectoryEntry child in ent.Children)
                    Console.WriteLine(child.Name);

                Console.ReadKey(true);
            }
            catch (DirectoryServicesCOMException e)
            {
                Console.WriteLine("DirectoryServicesCOMException: {0}", e.ToString());
            }
        }

        private static void Prompt()
        {
            while (is_running)
            {
                Console.Write("> ");
                string cmd = Console.ReadLine();
                string cmdlow = cmd.ToLower();
                if (cmdlow.Equals("quit"))
                {
                    is_running = false;
                }
                else if (cmdlow.Equals("connect"))
                {
                }
                else
                {
                    Console.WriteLine("Unrecognized command.");
                }
            }
        }
    }
}
