using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            String menuOption="";
            do
            {
                Console.WriteLine("");
                try
                {

                } catch(Exception e)
                {

                }
            } while ();

            try
            {

                // Create and save a new Blog
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();

                var blog = new Blog { Name = name };

                var db = new BloggingContext();
                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);

                // Display all Blogs from the database
                var query = db.Blogs.OrderBy(b => b.Name);

                Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine(item.Name);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            Console.WriteLine("");
            logger.Info("Program ended");
        }

        public static void displayMenu()
        {
            Console.WriteLine("1: Display Blogs\n2: Add New Blog\n3: Create Post\n");
        }
        public static void getMenuOption()
        {
            int menuChoice = 0;
            String menuChoiceStr;
            do
            {
                displayMenu();
                menuChoiceStr = Console.ReadLine();
            } while (!int.TryParse(menuChoiceStr, out menuChoice) || !(menuChoice == 1 || menuChoice == 2 || menuChoice == 3))
            
        }
        
    }
}