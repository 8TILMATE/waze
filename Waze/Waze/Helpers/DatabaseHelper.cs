using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
namespace Waze.Helpers
{
    
    class DatabaseHelper
    {
        public const string _connectionstring = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\rafxg\\source\\repos\\Waze\\Waze\\Helpers\\Db.mdf;Integrated Security=True";
        public static string _hartipath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.Length-11,10) + "Resurse\\harti.txt");
        public static string _masuratoripath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.Length - 11, 10) + "Resurse\\masurari.txt");
        public static void HartiToDB(SqlConnection con)
        {
            using(StreamReader rdr=new StreamReader(_hartipath)){
                while (rdr.Peek()>=0)
                {
                        var line = rdr.ReadLine().Split('#');
                        string cmdText = "Insert into Harti(Nume,Fisier) values (@Nume,@Fisier)";
                        using (SqlCommand cmd = new SqlCommand(cmdText, con))
                        {
                        cmd.Parameters.AddWithValue("@Nume", line[0].TrimEnd());
                        cmd.Parameters.AddWithValue("@Fisier", line[1].TrimEnd());
                        cmd.ExecuteNonQuery();
                        }
                    }
                }
        }
        public static void ClearDB(SqlConnection con)
        {
            string cmdText = "Delete from Harti";
            using(SqlCommand cmd= new SqlCommand(cmdText, con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("DBCC CHECKIDENT('Harti', RESEED, 0)", con))
            {
                cmd.ExecuteNonQuery();
            }
            try
            {
                using (SqlCommand cmd = new SqlCommand("DBCC CHECKIDENT('Masurare', RESEED, 0)", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch { 
            }
            }
        public static void Init()
        {
            using(SqlConnection con = new SqlConnection(_connectionstring))
            {
                con.Open();
                ClearDB(con);
                HartiToDB(con);
                MasuraritoDB(con);
            }
        }
        public static void MasuraritoDB(SqlConnection con)
        {
            int k = 1;
            using (StreamReader rdr = new StreamReader(_masuratoripath))
            {
                int id;
                while (rdr.Peek() >= 1)
                {
                    var line = rdr.ReadLine().Split('#');
                    string cmdText = "Insert into Masurare(IdHarta,PosX,PosY,Val,Data) values (@id1,@px,@py,@v,@d)";
                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    { 
                        cmd.Parameters.AddWithValue("id1", return_id(line[0], con));
                        cmd.Parameters.AddWithValue("px", line[1]);
                        cmd.Parameters.AddWithValue("py", line[2]);
                        cmd.Parameters.AddWithValue("v", line[3]);
                        cmd.Parameters.AddWithValue("d", line[4]);
                        cmd.ExecuteNonQuery();
                        k++;
                    }
                }
            }
        }
        public static int return_id(string nume,SqlConnection con)
        {
            string cmdText = "Select id from Harti where Harti.Nume = @nume";
            using (SqlCommand cmd = new SqlCommand(cmdText, con))
            {
                cmd.Parameters.AddWithValue("nume", nume);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return Int32.Parse(rdr[0].ToString());
                }
            }
        }
    }
}
