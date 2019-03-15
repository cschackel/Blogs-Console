using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static BloggingContext db = new BloggingContext();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            int menuOption = getMenuOption();

            switch(menuOption)
            {
                case 1:
                    displayBlogs();
                    break;
                case 2:
                    addBlog();
                    break;
            }

            try
            {

                // Create and save a new Blog
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();

                var blog = new Blog { Name = name };

                //var db = new BloggingContext();
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
        public static int getMenuOption()
        {
            int menuChoice = 0;
            bool validMenuChoice=false;
            do
            {
                displayMenu();
                String menuChoiceStr = Console.ReadLine();
                if (int.TryParse(menuChoiceStr, out menuChoice) && (menuChoice == 1 || menuChoice == 2 || menuChoice == 3))
                {
                    validMenuChoice = true;
                }
                else
                {
                    Console.WriteLine("Invalid Menu Input\n\n");
                }
            } while (!validMenuChoice);

            logger.Info("Menu Choice: {menuChoice}",menuChoice);
            return menuChoice;
            
        }

        public static void displayBlogs()
        {
            try
            {
                var query = db.Blogs.OrderBy(b => b.Name);

                Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine(item.Name);
                }
            } catch(Exception e)
            {
                logger.Error("Error Displaying Blogs: " + e.Message + " " + e.StackTrace);
            }
        }

        public static void addBlog()
        {
            try
            {
                bool validBlogName = false;
                String name = "";
                do
                {
                    Console.Write("Enter a name for a new Blog: ");
                    name = Console.ReadLine();

                    var currentBlogNames = db.Blogs.Select(n => n.Name);

                    if(!currentBlogNames.Contains(name))
                    {
                        validBlogName = true;
                    }
                    else
                    {
                        Console.WriteLine("Blog Name Already Taken\n");
                    }
                } while (!validBlogName);

                var blog = new Blog { Name = name };

                //var db = new BloggingContext();
                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            } catch(Exception e)
            {
                logger.Error("Error Adding Blog: {e.Message} {e.StackTrace}",e.Message,e.StackTrace);
            }
        }
        
    }
}