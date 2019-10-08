﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestDepLib.Persistence;
using GestDepLib.BusinessLogic;
using GestDepLib.Entities;
using GestDepLib.BusinessLogic.Services;

namespace BusinessLogicTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IGestDepService service = new GestDepService(new EntityFrameworkDAL(new GestDepDbContext()));

                new Program(service);
            }
            catch (Exception e)
            {
                printError(e);
            }

        }

        private IGestDepService service;

        Program(IGestDepService service)
        {
            this.service = service;

            service.removeAllData();

                // Adding Pool and Lanes
                addPoolAndLanes();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                // Adding Courses
                addCourses();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();


                // Adding Monitor
                addMonitor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();


                // Adding Users and Enrollments
                addUsers();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                // Adding payments
                addPayments();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                // Testing Free Lanes
                testingFreeLanes();
                Console.Out.WriteLine("\nPress any key to finish.");
                Console.In.ReadLine();

        }


        static void printError(Exception e)
        {
            while (e != null)
            {
                Console.WriteLine("ERROR: " + e.Message);
                e = e.InnerException;
            }
        }

        void addPoolAndLanes()
        {
            Console.WriteLine();
            Console.WriteLine("ADDING POOL AND LANES...");

            try
            {
                // Pool(int id, DateTime OpeningHour, DateTime ClosingHour, int ZipCode, int discountLocal, int discountRetired, double freeSwimPrice)
                // Id is not autogenerated
                Pool pool = new Pool(1, Convert.ToDateTime("08:00:00"), Convert.ToDateTime("14:00:00"), 46122, 5, 5, 2.00);
                for (int i = 1; i < 7; i++)
                {
                    pool.addLane(new Lane(i));
                }
                service.addPool(pool);

                foreach(Pool p in service.getAllPools())
                {
                    Console.WriteLine(" Pool " + p.Id);
                    foreach (Lane l in p.Lanes)
                        Console.WriteLine("   Lane " + l.Number);
                }

            }
            catch(Exception e)
            {
                printError(e);
            }
        }

        void addCourses()
        {
            Console.WriteLine();
            Console.WriteLine("ADDING COURSES AND ASSIGNING LANES...");

            try
            {
                // Course(String description, DateTime startDate, DateTime finishDate, DateTime startHour, TimeSpan duration, Days courseDays, int minimunEnrollments, int maximunEnrollments, bool cancelled, double price)
                Course c = new Course("Learning with M. Phelps", new DateTime(2017, 9, 4),
                    new DateTime(2018, 6, 29), Convert.ToDateTime("09:30:00"), 
                    new TimeSpan(0, 45, 0),
                                  Days.Monday | Days.Wednesday | Days.Friday,
                                  6, 20, false, 100);
                //añadido
                Console.WriteLine("creado -->");
                service.addCourse(c);
                Console.WriteLine("añadido -->");

                Console.WriteLine("  Course days: " + c.CourseDays);
                if ((c.CourseDays & Days.Friday) == Days.Friday)
                    Console.WriteLine("   Course on Friday");
                else
                    Console.WriteLine("   Course is NOT on Friday");

                // Adding lanes for a Course
                Pool p = service.findPoolById(1); // ¡¡¡¡¡¡¡ Assuming that Id is not autogenerated !!!!!!

                
                c.addLane(p.findLane(3));
                c.addLane(p.findLane(4));
                c.addLane(p.findLane(5));
                service.saveChanges();
               
              
                // Testing Lanes assigned
                Console.WriteLine("\n  Lanes assigned");
                foreach (Lane la in c.Lanes)
                    Console.WriteLine("   " + la.Number + " assigned");

                // Adding another Course
                c = new Course("201 - Swimming", new DateTime(2018, 1, 6), new DateTime(2018, 3, 30), Convert.ToDateTime("09:30:00"), new TimeSpan(0, 45, 0),
                  Days.Monday | Days.Wednesday | Days.Friday,
                  6, 20, false, 100);
                service.addCourse(c);

                // Adding lanes for a Course
                p = service.findPoolById(1); // ¡¡¡¡¡¡¡ Assuming that Id is not autogenerated !!!!!!
                c.addLane(p.findLane(1));
                c.addLane(p.findLane(6));
                service.saveChanges();

            }
            catch (Exception e)
            {
                printError(e);
            }

        }

        void addMonitor()
        {
            Console.WriteLine();
            Console.WriteLine("ADDING MONITOR...");

            try
            {
                // Monitor(string id, string Name, string Address, int ZipCode, string IBAN, string Ssn)
                Monitor m = new Monitor("X-00000001", "Michael Phelps", " Michael Phelps'address", 46001, "ES891234121234567890", "SSN01010101");
                service.addMonitor(m);

                Course c = service.findCourseByName("Learning with M. Phelps");
                // añadido
                Console.WriteLine(c.Description);
                c.setMonitor(m);
                service.saveChanges();
                Console.WriteLine("   " + c.Monitor.Name + " assigned to " + c.Description + " course");

                // Add the same monitor to another course with collision dates
                // Must fails by collision dates
                c = service.findCourseByName("201 - Swimming");
                c.setMonitor(m);
                service.saveChanges();
                Console.WriteLine("   " + c.Monitor.Name + " assigned to " + c.Description + " course");


            }
            catch (Exception e)
            {
                printError(e);
            }
        }

        void addUsers()
        {
            Console.WriteLine();
            Console.WriteLine("ADDING USERS...");

            try
            {
                Course c = service.findCourseByName("Learning with M. Phelps");
                //añadido
                Console.WriteLine(c.Description);
                // User(string id, string name, string address, int zipCode, string IBAN, DateTime birthDate, bool retired)
                User u = new User("1234567890", "Ona Carbonell", "Ona Carbonell's address", 46002, "ES891234121234567890", new DateTime(1990, 6, 5), false);
                service.enrollUserToCourse(new DateTime(2017, 08, 16), u, c);

                u = new User("2345678901", "Gemma Mengual", "Gemma Mengual's address", 46002, "ES891234121234567890", new DateTime(1977, 4, 12), false);
                service.enrollUserToCourse(new DateTime(2017, 07, 26), u, c);

                u = new User("3456789012", "Mireia Belmonte", "Mireia Belmonte's address", 46003, "ES891234121234567890", new DateTime(1990, 11, 10), false);
                service.enrollUserToCourse(new DateTime(2017, 08, 28), u, c);

                u = new User("4567890123", "Rigoberto", "Rigoberto's address", 46122, "ES891234121234567890", new DateTime(1995, 2, 28), false);
                service.enrollUserToCourse(new DateTime(2017, 08, 28), u, c);

                u = new User("5678901234", "Lázaro", "Lázaro's address", 46122, "ES891234121234567890", new DateTime(1900, 1, 1), true);
                service.enrollUserToCourse(new DateTime(2017, 08, 29), u, c);

                // Checking Users enrolled
                Console.WriteLine("  Users enrolled in course with monitor " + c.Monitor.Name);
                foreach (Enrollment en in c.Enrollments)
                    Console.WriteLine("   " + en.User.Name + " enrolled");

            }
            catch (Exception e)
            {
                printError(e);
            }
        }

        void addPayments()
        {
            Console.WriteLine();
            Console.WriteLine("ADDING PAYMENTS...");

            try
            {
                
                service.addFreeSwimPayment(new DateTime(2017, 8, 10, 18, 12, 5));

                service.addFreeSwimPayment(new DateTime(2017, 8, 20, 18, 12, 5));

                service.addFreeSwimPayment(new DateTime(2017, 8, 20, 18, 13, 5));

                // Adding Payments
                Course c = service.findCourseByName("Learning with M. Phelps");

                Enrollment e = c.findEnrollment("1234567890");
                service.addPayment(e, new DateTime(2017, 8, 16, 12, 30, 0));
                service.addPayment(e, new DateTime(2017, 8, 17, 13, 30, 1));


                e = c.findEnrollment("5678901234");
                service.addPayment(e, new DateTime(2017, 09, 29));



                // Testing Payments
                foreach (Enrollment en in service.getAllEnrollments())
                {
                    Console.WriteLine("\n  Payments attached to " + en.User.Name);
                    foreach (Payment moO in en.Payments)
                        Console.WriteLine("   " + moO.Description + " " + moO.Quantity);
                }

                Console.WriteLine("\n  Free Swim payments");
                foreach (Payment p in service.getAllFreeSwimPayments())
                    Console.WriteLine("   " + p.Quantity + " " + p.Date );

            }
            catch (Exception e)
            {
                printError(e);
            }
        }

        void testingFreeLanes()
        {
            Console.WriteLine();
            Console.WriteLine("TESTING FREE LANES");

            try
            {
                // Test free lanes week 2018, 1, 29
                Pool p = service.findPoolById(1);

                int freeLanes = p.getFreeLanes(new DateTime(2018, 1, 29, 8, 00, 0), Days.Monday);
                Console.WriteLine("   Free lanes at 8:00 - " + freeLanes);

                freeLanes = p.getFreeLanes(new DateTime(2018, 1, 29, 9, 30, 0), Days.Monday);
                Console.WriteLine("   Free lanes at 9:30 - " + freeLanes);

                //añadido
               // freeLanes = p.getFreeLanes(new DateTime(2018, 12, 18, 9, 30, 0), Days.Monday);
               // Console.WriteLine("   Free lanes at 9:30 lunes - " + freeLanes);

            }
            catch (Exception e)
            {
                printError(e);
            }
        }




    }

}


