using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using BookLibraryCompulsoryAssignment;

namespace BookTCPServerCompulsoryAssignment
{
    class Program
    {
        public static List<Book> ListOfBooks = new List<Book>();


        static void Main(string[] args)
        {

            Book b4 = new Book("Book", "Paul S.", 500, "12233556");
            Book b2 = new Book("ThisBook", "Nohely G.", 60, "0987654321");
            Book b3 = new Book("Big Book", "Amethyst.", 100, "67890563");


            ListOfBooks.Add(b4);
            ListOfBooks.Add(b2);
            ListOfBooks.Add(b3);


            Console.WriteLine("Server");

            TcpListener listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 4646);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();

                Task.Run(() => { HandleRequests(socket); });
            }

        }


        public static void HandleRequests(TcpClient socket)
        {
                NetworkStream ns = socket.GetStream();
                StreamReader reader = new StreamReader(ns);
                StreamWriter writer = new StreamWriter(ns);

                string message1 = reader.ReadLine();
                string message2 = reader.ReadLine();

                if (message1 == "GetAll")
                {
                    writer.WriteLine(GetAll()); 
                }

                if (message1 == "Get")
                {
                    writer.WriteLine(Get(message2));
                }

                if (message1 == "Save")
                {
                    Save(message2);
                }

                Console.WriteLine("Client says:" + message1 + message2);

                writer.Flush();
                socket.Close();

        }
        

        public static string GetAll()
        {
            var results = ListOfBooks;
            string SerializedListBooks = JsonSerializer.Serialize(results);

            return SerializedListBooks;
        }


        public static string Get(string isbn13)
        {
            Book book = ListOfBooks.Find(b => b.ISBN13.Equals(isbn13));
            string SerializedBook = JsonSerializer.Serialize(book);

            return SerializedBook;
        }


        public static void Save(string message)
        {
            Book book = JsonSerializer.Deserialize<Book>(message);
            ListOfBooks.Add(book);
        }


        //{"Title": "UML", "Author": "Larman", "numberOfPages": 654, "ISBN13": "9780133594140"}
        //{"Title": "Twilight", "Author": "Stephenie Meyer", "numberOfPages": 501, "ISBN13": "9780316015844"}
        //{"Title": "To Kill a Mockingbird", "Author": "Harper Lee", "numberOfPages": 324, "ISBN13": "9780446310789"}
        //{"Title": "Dune", "Author": "Frank Herbert", "numberOfPages": 688, "ISBN13": "9780593099322"}

    }
}
