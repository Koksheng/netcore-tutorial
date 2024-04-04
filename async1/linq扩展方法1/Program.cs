namespace linq扩展方法1;

class Program
{
    static void Main(string[] args)
    {
        List<Employee> list = new List<Employee>();
        list.Add(new Employee { Id = 1, Name = "jerry", Age = 28, Gender = true,
            Salary = 5000 });
        list.Add(new Employee
        {
            Id = 2,
            Name = "jim",
            Age = 33,
            Gender = true,
            Salary = 3000
        });
        list.Add(new Employee
        {
            Id = 3,
            Name = "lily",
            Age = 35,
            Gender = false,
            Salary = 9000
        });
        list.Add(new Employee
        {
            Id = 4,
            Name = "lucy",
            Age = 16,
            Gender = false,
            Salary = 2000
        });
        list.Add(new Employee
        {
            Id = 5,
            Name = "kimi",
            Age = 25,
            Gender = true,
            Salary = 1000
        });
        list.Add(new Employee
        {
            Id = 6,
            Name = "nancy",
            Age = 35,
            Gender = false,
            Salary = 8000
        });
        list.Add(new Employee
        {
            Id = 7,
            Name = "zack",
            Age = 35,
            Gender = true,
            Salary = 8500
        });
        list.Add(new Employee
        {
            Id = 8,
            Name = "jack",
            Age = 33,
            Gender = true,
            Salary = 8000
        });

        //IEnumerable<Employee> items1 = list.Where(a => a.Age > 30);
        //foreach(Employee i in items1)
        //{
        //    Console.WriteLine(i);
        //}

        //Console.WriteLine(list.Count());
        //Console.WriteLine(list.Count(a=>a.Age > 20));
        //Console.WriteLine(list.Count(a=>a.Age > 20 && a.Salary > 8000));

        //Console.WriteLine(list.Any(a => a.Age > 20));
        //Console.WriteLine(list.Any(a => a.Salary > 200000));

        //IEnumerable<Employee> item = list.Where(a => a.Name == "jerry");
        //Employee employee = item.Single();
        //Console.WriteLine(employee);

        ////Employee e2 = list.Where(a => a.Name == "yzk").Single();
        ////Console.WriteLine(e2);



        //---------- SIngle, SingleOrDefault, First, FirstOrDefault ----------
        //Employee e3 = list.Single(a => a.Name == "jerry");
        //Console.WriteLine(e3);

        //Employee e4 = list.SingleOrDefault(a => a.Name == "tom");
        //Console.WriteLine(e4==null);

        //int[] num = new int[] { 3, 5, 7 };
        //int i = num.SingleOrDefault(a => a > 10);
        //Console.WriteLine(i);

        //Employee e = list.First(a => a.Age > 30);
        //Console.WriteLine(e);

        //Employee e1 = list.FirstOrDefault(a => a.Age > 300);
        //Console.WriteLine(e1==null);

        //---------- Orderby //----------
        //IEnumerable<Employee> item = list.OrderBy(a=>a.Age);
        //foreach (Employee e in item)
        //{
        //    Console.WriteLine(e);
        //}

        //IEnumerable<Employee> item1 = list.OrderByDescending(a=>a.Salary);
        //foreach (Employee e in item1)
        //{
        //    Console.WriteLine(e);
        //}

        //int[] nums = new int[] { 3, 6, 2, 6, 23, 7 };
        ////IEnumerable<int> n = nums.OrderDescending();
        ////foreach(int a in n)
        ////    Console.WriteLine(a);
        ////}

        //Random random = new Random();
        //IEnumerable<int> n1 = nums.OrderBy(a => random.Next());
        //foreach (int a in n1)
        //{
        //    Console.WriteLine(a);
        //}

        //IEnumerable<Employee> employees = list.OrderBy(a => a.Age).ThenBy(a => a.Salary);
        //foreach(Employee e in employees)
        //{
        //    Console.WriteLine(e);
        //}

        //IEnumerable<Employee> employees = list.Skip(3).Take(2);
        //foreach(Employee e in employees)
        //{
        //    Console.WriteLine(e);
        //}

        //IEnumerable<Employee> employees = list.Where(a => a.Age > 30)
        //    .OrderBy(a => a.Age)
        //    .Skip(1).Take(2);
        //foreach(Employee e in employees)
        //{
        //    Console.WriteLine(e);
        //}


        //---------- Max Average Min SUm //----------
        //int age = list.Max(a => a.Age);
        //Console.WriteLine(age);

        //int age1 = list.Where(a => a.Id > 6).Max(a => a.Salary);
        //Console.WriteLine(age1);

        //string s = list.Max(a => a.Name);
        //Console.WriteLine(s);

        //double n = list.Where(a => a.Age > 30).Average(a => a.Salary);
        //Console.WriteLine(n);


        ////---------- GroupBy //----------
        //IEnumerable<IGrouping<int, Employee>> groupings = list.GroupBy(a => a.Age);
        //foreach(IGrouping<int,Employee>g in groupings)
        //{
        //    Console.WriteLine(g.Key);
        //    IEnumerable<Employee> sortedE = g.OrderByDescending(a => a.Salary);
        //    Console.WriteLine("Max Salary: " + g.Max(a => a.Salary));
        //    foreach(Employee e in sortedE)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    Console.WriteLine("**************");
        //}

        //---------- Select //----------
        //IEnumerable<int> ints = list.Select(a => a.Age);
        //foreach(int i in ints)
        //{
        //    Console.WriteLine(i);
        //}

        //IEnumerable<string> names = list.Select(a => a.Name);
        //foreach (string s in names)
        //{
        //    Console.WriteLine(s);
        //}

        //IEnumerable<string> strings1 = list.Where(a => a.Gender == true).Select(a=>a.Name);
        //IEnumerable<string> strings1 = list.Where(a => a.Salary > 5000).Select(a => a.Gender ? "Boy" : "Girl");
        //foreach (string s in strings1)
        //{
        //    Console.WriteLine(s);
        //}

        //IEnumerable<Dog> dogs = list.Select(a => new Dog() { Nickname = a.Name, Age = a.Age });
        //foreach(Dog d in dogs)
        //{
        //    Console.WriteLine($"{d.Nickname} : {d.Age}");
        //}

        //var p = new { Name = "tom", Id = 1 };
        //Console.WriteLine(p);
        //Console.WriteLine(p.Name);
        //Console.WriteLine(p.Id);

        //var item = list.Select(a => new { Name = a.Name, Age = a.Age,
        //    Gender = a.Gender == true ? "Boy" : "Girl" });
        //foreach(var i in item)
        //{
        //    Console.WriteLine(i.Name + i.Age + i.Gender);
        //}

        //var item = list.GroupBy(a => a.Age).Select(a => new { Age=a.Key,
        //sMax=a.Max(s =>s.Salary), sMin=a.Min(s => s.Salary), headCount=a.Count()});
        //foreach(var i in item)
        //{
        //    Console.WriteLine(i);
        //}

        //IEnumerable<Employee> employees = list.Where(a => a.Id > 2);
        //List<Employee> list2 = employees.ToList();
        //Employee[] employees2 = employees.ToArray();

        //IEnumerable<IGrouping<int,Employee>> employees = list.GroupBy(a => a.Age);
        //foreach(IGrouping<int,Employee> emp in employees)
        //{
        //    Console.WriteLine(emp.Key);
        //    IEnumerable<Employee> item = emp.Where(e => e.Id > 2).OrderBy(e=>e.Age).Take(3);
        //    var result = item.Select(f => new { Age = f.Age, headCount = item.Count()
        //        , averageSalary = item.Average(i => i.Salary) });
        //    Console.WriteLine(result);
        //}

        var items = list.Where(e => e.Id > 2).GroupBy(e => e.Age).OrderBy(g => g.Key).Take(3)
            .Select(g => new { Age =g.Key, HeadCount=g.Count(), AveSalary=g.Average(e=>e.Salary)});
        foreach(var i in items)
        {
            Console.WriteLine(i);
        }
    }
}

