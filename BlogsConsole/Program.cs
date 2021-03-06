﻿using NLog;
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
                        displayPosts();
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
            Console.WriteLine("1: Display Blogs\n2: Add New Blog\n3: Create Post\n4: View Posts\n5: Exit");
        }

        //Prompts User for the Menu Option
        public static int getMenuOption()
        {
            int menuChoice = 0;
            bool validMenuChoice = false;
            do
            {
                displayMenu();
                String menuChoiceStr = Console.ReadLine();
                if (int.TryParse(menuChoiceStr, out menuChoice) && (menuChoice == 1 || menuChoice == 2 || menuChoice == 3 || menuChoice == 4 || menuChoice == 5))
                {
                    validMenuChoice = true;
                }
                else
                {
                    Console.WriteLine("Invalid Menu Input\n\n");
                }
            } while (!validMenuChoice);

            logger.Info("Menu Choice: {menuChoice}", menuChoice);
            return menuChoice;

        }

        //Get's Blog ID of Blog to Target
        public static int getTargetBlog()
        {
            bool validBlogID = false;
            int blogID = -1;
            var blogIDs = db.Blogs.Select(r => r.BlogId);
            do
            {
                displayBlogs();
                Console.WriteLine("Select Blog ID: ");
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
            if (db.Blogs.Count() == 0)
            {
                //logger.Warn("Cannot Display if no blogs are present.");
                Console.WriteLine("\nNo Blogs To Display\n");
            }
            else
            {
                try
                {
                    var query = db.Blogs.OrderBy(b => b.BlogId);

                    Console.WriteLine($"Blogs Returned: {query.Count()}");
                    //Console.WriteLine("\nAll blogs in the database:");
                    foreach (var item in query)
                    {
                        Console.WriteLine($"\nID: {item.BlogId} -- Name: {item.Name}\n");
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error Displaying Blogs: " + e.Message + " " + e.StackTrace);
                }
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
                    Console.Write("\nEnter a name for a new Blog: ");
                    name = Console.ReadLine();

                    var currentBlogNames = db.Blogs.Select(n => n.Name);

                    if (!currentBlogNames.Contains(name) && !name.Equals(""))
                    {
                        validBlogName = true;
                    }
                    else if(name.Equals(""))
                    {
                        Console.WriteLine("Blog Name Cannot Be Blank\n");
                    }
                    else
                    {
                        Console.WriteLine("Blog Name Already Taken\n");
                    }
                } while (!validBlogName);

                var blog = new Blog { Name = name };

                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            }
            catch (Exception e)
            {
                logger.Error("Error Adding Blog: {e.Message} {e.StackTrace}", e.Message, e.StackTrace);
            }
        }

        //Adds Post To Posts Table
        public static void addPost()
        {
            if(db.Blogs.Count()==0)
            {
                logger.Warn("Cannot add posts if no blogs are present.");
                Console.WriteLine("\nNo Blogs To Post To\n");
            }
            else
            {
                try
                {
                    Post newPost = new Post { BlogId = getTargetBlog() };
                    String tempTitle = "";
                    do
                    {
                        Console.WriteLine("\nPost Title: ");
                        tempTitle = Console.ReadLine();
                        if (tempTitle.Equals(""))
                        {
                            Console.WriteLine("Cannot Leave Title Blank");
                        }
                    } while (tempTitle.Equals(""));
                    newPost.Title = tempTitle;

                    Console.WriteLine("Post Content: ");
                    newPost.Content = Console.ReadLine();

                    db.AddPost(newPost);

                    logger.Info("Post Added to Blog ID {BlogID}: Post ID: {PostID}", newPost.BlogId, newPost.PostId);
                }
                catch (Exception e)
                {
                    logger.Error("Error Adding Post {message} {stackTrace}", e.Message, e.StackTrace);
                }
            }
        }

        //Displays Posts From A Given Blog
        public static void displayPosts()
        {
            if (db.Blogs.Count() == 0)
            {
                logger.Warn("Cannot view posts if no blogs are present.");
                Console.WriteLine("\nNo Blogs To Gather Posts From\n");
            }
            else
            {
                int menuChoice = 0;
                bool validMenuChoice = false;
                do
                {
                    Console.WriteLine("\n1: Display All Posts\n2: Display Posts From Blog\n");
                    String menuChoiceStr = Console.ReadLine();
                    if (int.TryParse(menuChoiceStr, out menuChoice) && (menuChoice == 1 || menuChoice == 2))
                    {
                        validMenuChoice = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Menu Input\n\n");
                    }
                } while (!validMenuChoice);
                switch (menuChoice)
                {
                    case 1:
                        displayAllPosts();
                        break;
                    case 2:
                        displayPostsFromBlog();
                        break;
                    default:
                        logger.Error("Unexpected Switch Property in Display Posts");
                        break;
                }
            }

        }

        //Displays Posts From All Blogs
        public static void displayAllPosts()
        {
            Console.WriteLine("All Posts:");
            var posts = db.Posts;
            Console.WriteLine($"\n{posts.Count()} Posts Returned\n");
            foreach (Post p in posts)
            {
                Console.WriteLine($"\nBlog:\n{p.Blog.Name}\nTitle:\n{p.Title}\nContent:\n{p.Content}\n");
            }
        }

        //Displays Posts From A Single Blog
        public static void displayPostsFromBlog()
        {
            int blogID = getTargetBlog();
            //var posts = db.Blogs.Where(r=>r.BlogId==blogID).Select(r=>r.Posts);
            var posts = db.Posts.Where(r => r.BlogId == blogID);
            Console.WriteLine($"\n{posts.Count()} Posts Returned\n");
            foreach (Post p in posts)
            {
                Console.WriteLine($"\nTitle:\n{ p.Title}\nContent:\n{ p.Content}\n");
            }
        }

    }
}