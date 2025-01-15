using Microsoft.EntityFrameworkCore;

namespace PostgresConnect1
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=password");
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            // добавление данных
            using (ApplicationContext db = new ApplicationContext())
            {
                // создаем два объекта User
                User user1 = new User { Name = "Tom", Age = 33 };
                User user2 = new User { Name = "Alice", Age = 26 };

                // добавляем их в бд
                db.Users.AddRange(user1, user2);
                db.SaveChanges();
            }
            // получение данных
            using (ApplicationContext db = new ApplicationContext())
            {
                // обновляем только объекты, у которых имя Tom
                db.Users.Where(u => u.Name == "Tom")
                    .ExecuteUpdate(s => s
                            .SetProperty(u => u.Age, u => u.Age + 1)    // Age = Age + 1
                            .SetProperty(u => u.Name, u => "Tomas"));      // Name = "Tomas
                
                // получаем объекты из бд и выводим на консоль
                var users = db.Users.ToList();
                Console.WriteLine("Users list before:");
                foreach (User u in users)
                {
                    Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                }

                foreach (User u in users)
                {
                    if (u.Age>30)
                        db.Users.Remove(u);
                }

                db.SaveChanges();
                users = db.Users.ToList();

                Console.WriteLine("Users list after:");
                foreach (User u in users)
                {
                    Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");                    
                }
                //  await db.Users.Where(u => u.Name == "Bob").ExecuteDeleteAsync();
            }
            Console.ReadLine();
        }
    }
}
