using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;

class Program()
{
    public static SqlConnection CheckIfDataBaseExists(string dataBaseName, string serverName)
    {
        string connectionString = $"Server={serverName};Integrated Security=True;";
        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();

            SqlCommand checkDB = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{dataBaseName}'", connection);
            int count = (int)checkDB.ExecuteScalar();

            if (count == 0)
            {
                SqlCommand createDB = new SqlCommand($"CREATE DATABASE {dataBaseName}", connection);
                createDB.ExecuteNonQuery();
                Console.WriteLine($"Created database with the name {dataBaseName}!");

                string newConnectionString = $"{connectionString}Initial Catalog={dataBaseName}";
                SqlConnection newConnection = new SqlConnection(newConnectionString);
                newConnection.Open();
                return newConnection;
            }
            else
            {
                Console.WriteLine($"DataBase with the name {dataBaseName} already exists!");
                return connection;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: " + ex.Message);
            return null;
        }
    }

    public static void CreateTables(SqlConnection connection)
    {
        try
        {
            SqlCommand table1Creation = new SqlCommand("CREATE TABLE [subjects]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [subject] NVARCHAR(100) NOT NULL,\r\n    [level] NVARCHAR(10) CHECK([level] = 'easy' OR [level] = 'medium' OR [level] = 'hard') NOT NULL\r\n);", connection);
            table1Creation.ExecuteNonQuery();
            Console.WriteLine("Table 1 is created!");
            SqlCommand table2Creation = new SqlCommand("CREATE TABLE [teachers]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [teacher_code] INT UNIQUE NOT NULL,\r\n    [full_name] NVARCHAR(100) NOT NULL,\r\n    [gender] CHAR(1) CHECK([gender] = 'м' OR [gender] = 'ж') NOT NULL,\r\n    [date_of_birth] DATE NOT NULL,\r\n    [email] NVARCHAR(100) UNIQUE NOT NULL,\r\n    [phone] VARCHAR(10) NOT NULL,\r\n    [working_days] INT NOT NULL\r\n);", connection);
            table2Creation.ExecuteNonQuery();
            Console.WriteLine("Table 2 is created!");
            SqlCommand table3Creation = new SqlCommand("CREATE TABLE [classrooms]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [floor] INT CHECK([floor] >= 1 AND [floor] <= 3) NOT NULL,\r\n    [capacity] INT CHECK([capacity] <= 30) NOT NULL,\r\n    [description] NVARCHAR(200) NOT NULL\r\n);", connection);
            table3Creation.ExecuteNonQuery();
            Console.WriteLine("Table 3 is created!");
            SqlCommand table4Creation = new SqlCommand("CREATE TABLE [parents]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [parent_code] INT UNIQUE NOT NULL,\r\n    [full_name] NVARCHAR(100) NOT NULL,\r\n    [email] NVARCHAR(100) UNIQUE NOT NULL,\r\n    [phone] VARCHAR(10) NOT NULL\r\n);", connection);
            table4Creation.ExecuteNonQuery();
            Console.WriteLine("Table 4 is created!");
            SqlCommand table5Creation = new SqlCommand("CREATE TABLE [teachers_subjects]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [teacher_id] INT FOREIGN KEY REFERENCES teachers(id) NOT NULL,\r\n    [subject_id] INT FOREIGN KEY REFERENCES subjects(id) NOT NULL\r\n);", connection);
            table5Creation.ExecuteNonQuery();
            Console.WriteLine("Table 5 is created!");
            SqlCommand table6Creation = new SqlCommand("CREATE TABLE [classes]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [class_number] INT CHECK([class_number] >= 1 AND [class_number] <= 12) NOT NULL,\r\n    [class_letter] CHAR(1) NOT NULL,\r\n    [class_teacher_id] INT FOREIGN KEY REFERENCES teachers(id),\r\n    [classroom_id] INT FOREIGN KEY REFERENCES classrooms(id)\r\n);", connection);
            table6Creation.ExecuteNonQuery();
            Console.WriteLine("Table 6 is created!");
            SqlCommand table7Creation = new SqlCommand("CREATE TABLE [students]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [student_code] INT UNIQUE NOT NULL,\r\n    [full_name] NVARCHAR(100) NOT NULL,\r\n    [gender] CHAR(1) CHECK([gender] = 'м' OR [gender] = 'ж') NOT NULL,\r\n    [date_of_birth] DATE NOT NULL,\r\n    [email] NVARCHAR(100) UNIQUE NOT NULL,\r\n    [phone] VARCHAR(10) NOT NULL,\r\n    [class_id] INT FOREIGN KEY REFERENCES classes(id),\r\n    [is_active] BIT NOT NULL DEFAULT 1\r\n);", connection);
            table7Creation.ExecuteNonQuery();
            Console.WriteLine("Table 7 is created!");
            SqlCommand table8Creation = new SqlCommand("CREATE TABLE [students_parents]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [student_id] INT FOREIGN KEY REFERENCES students(id),\r\n    [parent_id] INT FOREIGN KEY REFERENCES parents(id)\r\n);", connection);
            table8Creation.ExecuteNonQuery();
            Console.WriteLine("Table 8 is created!");
            SqlCommand table9Creation = new SqlCommand("CREATE TABLE [classes_subject]\r\n(\r\n    [id] INT PRIMARY KEY IDENTITY(1,1),\r\n    [class_id] INT FOREIGN KEY REFERENCES classes(id),\r\n    [subject_id] INT FOREIGN KEY REFERENCES subjects(id)\r\n);", connection);
            table9Creation.ExecuteNonQuery();
            Console.WriteLine("Table 9 is created!");
            Console.WriteLine("-------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}.");
        }
    }

    public static void InsertInfoInTheTables(SqlConnection connection)
    {
        try
        {
            SqlCommand insertInTable1 = new SqlCommand("INSERT INTO subjects (subject, level)\r\nVALUES\r\n(N'Математика', N'easy'),\r\n(N'Български език', N'medium'),\r\n(N'Химия', N'hard'),\r\n(N'Физика', N'hard'),\r\n(N'История', N'medium'),\r\n(N'География', N'medium');", connection);
            insertInTable1.ExecuteNonQuery();
            Console.WriteLine("Table 1 filled with information!");
            SqlCommand insertInTable2 = new SqlCommand("INSERT INTO teachers (teacher_code, full_name, gender, date_of_birth, email, phone, working_days)\r\nVALUES\r\n(1001, N'Анна Георгиева', N'ж', '1982-03-12', 'anna.georgieva@example.com', '3216549870', 5),\r\n(1002, N'Иван Петров', N'м', '1975-08-22', 'ivan.petrov@example.com', '8765432109', 4),\r\n(1003, N'Лиляна Стефанова', N'ж', '1986-11-05', 'lilyana.stefanova@example.com', '6543219870', 3),\r\n(1004, N'Георги Димитров', N'м', '1983-01-17', 'georgi.dimitrov@example.com', '1239874560', 5),\r\n(1005, N'Маргарита Иванова', N'ж', '1990-02-14', 'margarita.ivanova@example.com', '5432167890', 2);", connection);
            insertInTable2.ExecuteNonQuery();
            Console.WriteLine("Table 2 filled with information!");
            SqlCommand insertInTable3 = new SqlCommand("INSERT INTO classrooms ([floor], capacity, [description])\r\nVALUES\r\n(1, 30, N'Голяма класна стая с проектор'),\r\n(2, 25, N'Класна стая с компютри и мултимедия'),\r\n(3, 20, N'Стая за специализирани предмети');", connection);
            insertInTable3.ExecuteNonQuery();
            Console.WriteLine("Table 3 filled with information!");
            SqlCommand insertInTable4 = new SqlCommand("INSERT INTO parents (parent_code, full_name, email, phone)\r\nVALUES\r\n(2001, N'Петър Иванов', 'petar.ivanov@example.com', '2345678901'),\r\n(2002, N'Мария Георгиева', 'maria.georgieva@example.com', '3456789012'),\r\n(2003, N'Иванка Стоянова', 'ivanka.stoyanova@example.com', '4567890123'),\r\n(2004, N'Красимир Димитров', 'krasimir.dimitrov@example.com', '5678901234'),\r\n(2005, N'Елена Петрова', 'elena.petrov@example.com', '6789012345');", connection);
            insertInTable4.ExecuteNonQuery();
            Console.WriteLine("Table 4 filled with information!");
            SqlCommand insertInTable5 = new SqlCommand("INSERT INTO teachers_subjects (teacher_id, subject_id)\r\nVALUES\r\n(1, 1),  \r\n(2, 2),  \r\n(3, 3),  \r\n(4, 4),  \r\n(5, 5);", connection);
            insertInTable5.ExecuteNonQuery();
            Console.WriteLine("Table 5 filled with information!");
            SqlCommand insertInTable6 = new SqlCommand("INSERT INTO classes (class_number, class_letter, class_teacher_id, classroom_id)\r\nVALUES\r\n(10, N'А', 1, 1),  \r\n(11, N'Б', 2, 2),  \r\n(9, N'А', 3, 3),   \r\n(12, N'Б', 4, 1),  \r\n(10, N'В', 5, 2);", connection);
            insertInTable6.ExecuteNonQuery();
            Console.WriteLine("Table 6 filled with information!");
            SqlCommand insertInTable7 = new SqlCommand("INSERT INTO students (student_code, full_name, gender, date_of_birth, email, phone, class_id)\r\nVALUES\r\n(3001, N'Иван Иванов', N'м', '2007-09-15', 'ivan.ivanov@example.com', '1234567890', 1),\r\n(3002, N'Елена Георгиева', N'ж', '2008-03-10', 'elena.georgieva@example.com', '2345678901', 2),\r\n(3003, N'Михаил Петров', N'м', '2007-05-22', 'mihail.petrov@example.com', '3456789012', 3),\r\n(3004, N'Габриела Иванова', N'ж', '2008-07-15', 'gabriela.ivanova@example.com', '4567890123', 4),\r\n(3005, N'Стефан Димитров', N'м', '2007-10-05', 'stefan.dimitrov@example.com', '7778889999', 5);", connection);
            insertInTable7.ExecuteNonQuery();
            Console.WriteLine("Table 7 filled with information!");
            SqlCommand insertInTable8 = new SqlCommand("INSERT INTO students_parents (student_id, parent_id)\r\nVALUES\r\n(1, 1),  \r\n(2, 2),  \r\n(3, 3),  \r\n(4, 4),  \r\n(5, 5);", connection);
            insertInTable8.ExecuteNonQuery();
            Console.WriteLine("Table 8 filled with information!");
            SqlCommand insertInTable9 = new SqlCommand("INSERT INTO classes_subject (class_id, subject_id)\r\nVALUES\r\n(1, 1),  \r\n(2, 2),  \r\n(3, 3),  \r\n(4, 4),  \r\n(5, 5);", connection);
            insertInTable9.ExecuteNonQuery();
            Console.WriteLine("Table 9 filled with information!");
            Console.WriteLine("-------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}.");
        }
    }

    public static void Query1(SqlConnection connection)
    {
        connection.Open();
        SqlCommand command = new SqlCommand("SELECT students.full_name FROM students\r\nJOIN classes ON classes.id = students.class_id\r\nWHERE(classes.class_number = 11 AND classes.class_letter = 'Б')", connection);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Students' Full Name:");
            Console.WriteLine($"{reader[0]}, {reader[1]}");
        }
        connection.Close();
    }

    public static void Main()
    {
        SqlConnection connection =  CheckIfDataBaseExists("school", "(localdb)\\ProjectModels");
        if (connection != null)
        {
            CreateTables(connection);
            InsertInfoInTheTables(connection);
        }
        else
        {
            Console.WriteLine("Failed to create or connect to the database.");
        }
        string[] input = Console.ReadLine().Split(' ').ToArray();
        while (input[0] != "end")
        {
            switch (input[0])
            {
                case "Query1":
                    Query1(connection);
                    break;
                default:
                    break;
            }
            input = Console.ReadLine().Split(' ').ToArray();
        }
    }
}



//SELECT students.full_name FROM students
//JOIN classes ON classes.id = students.class_id
//WHERE(classes.class_number = 11 AND classes.class_letter = 'Б')

//SELECT teachers.full_name, subjects.[subject] FROM teachers
//JOIN teachers_subjects ON teachers_subjects.teacher_id = teachers.id 
//JOIN subjects ON teachers_subjects.subject_id = subjects.id 
//GROUP BY teachers.full_name, subjects.[subject]

//SELECT classes.class_number, classes.class_letter, teachers.full_name FROM classes
//JOIN teachers ON teachers.id = classes.class_teacher_id

//SELECT subjects.[subject], COUNT(teachers_subjects.teacher_id) FROM subjects
//JOIN teachers_subjects ON teachers_subjects.subject_id = subjects.id
//GROUP BY(subjects.[subject])

//SELECT classrooms.id, classrooms.capacity FROM classrooms
//WHERE(classrooms.capacity > 26)
//ORDER BY(classrooms.[floor])

//SELECT students.full_name, classes.class_number, classes.class_letter FROM students
//JOIN classes ON classes.id = students.class_id
//GROUP BY classes.class_number, students.full_name, classes.class_letter
//ORDER BY(classes.class_number) ASC

//SELECT students.full_name