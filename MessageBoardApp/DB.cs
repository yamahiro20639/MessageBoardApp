using MySql.Data.MySqlClient;

namespace MySQLConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var con = new MySqlConnection("server=localhost;user=root;password=Malaysia4649;database=message_information;"))
            {
                con.Open();
                var command = new MySqlCommand("select id, message from messages;", con);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("-----------------------");
                    Console.WriteLine("| {0} | {1} |", reader["id"], reader["message"]);
                }
            }
            Console.WriteLine("-----------------------");
            Console.ReadLine();
        }
    }
}