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
            bool endProgram = false;  //Flag For End Program
            do  //Do While Program Is Running
            {
                int menuOption = getMenuOption();  //Gets Menu Option

                switch (menuOption)
                {
                    case 1:  //Display
                        displayBlogs();
                        break;
                    case 2:  //Add Blog
                        addBlog();
                        break;
                    case 3:  //Add Post
                        addPost();
                        break;
                    case 4: //Display Posts From Blog

                        break;
                    case 5:  //End Program
                        endProgram = true;
                        break;
                }
            } while (!endProgram);

            Console.WriteLine("");
            logger.Info("Program ended");
        }


        //Display's Menu
        public static void displayMenu()
        {
            Console.WriteLine("1: Display Blogs\n2: Add New Blog\n3: Create Post\n");
        }

        //Prompts User for the Menu Option
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

        //Get's Blog ID of Blog to Target
        public static int getTargetBlog()
        {
            bool validBlogID=false;
            int blogID=-1;
            var blogIDs = db.Blogs.Select(r=>r.BlogId);
            do
            {
                displayBlogs();
                Console.WriteLine("Post To Blog ID: ");
                String blogStr = Console.ReadLine();
                if (int.TryParse(blogStr, out blogID) && blogIDs.Contains(blogID))
                {
                    validBlogID = true;
                }
                else
                {
                    Console.WriteLine("Invalid Blog ID\n");
                }
            } while (!validBlogID);
            return blogID;
        }

        //Displays All Blogs
        public static void displayBlogs()
        {
            try
            {
                var query = db.Blogs.OrderBy(b => b.Name);

                Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine($"ID: {item.BlogId} -- Name: {item.Name}");
                }
            } catch(Exception e)
            {
                logger.Error("Error Displaying Blogs: " + e.Message + " " + e.StackTrace);
            }
        }

        //Adds a New Blog
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

                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            } catch(Exception e)
            {
                logger.Error("Error Adding Blog: {e.Message} {e.StackTrace}",e.Message,e.StackTrace);
            }
        }

        //Adds Post To Posts Table
        public static void addPost()
        {

            try
            {
                Post newPost = new Post { BlogId = getTargetBlog() };
                Console.WriteLine("Post Title: ");
                newPost.Title = Console.ReadLine();
                Console.WriteLine("Post Content: ");
                newPost.Content = Console.ReadLine();

                db.AddPost(newPost);

                logger.Info("Post Added to Blog ID {BlogID}: Post ID: {PostID}",newPost.BlogId,newPost.PostId);
            }
            catch(Exception e)
            {
                logger.Error("Error Adding Post {message} {stackTrace}",e.Message,e.StackTrace);
            }
        }
        
    }
}