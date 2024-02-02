using Quartz;
using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;

public class HelloJob : IJob
{
    /// <summary>
    /// Called by the <see cref="IScheduler" /> when a
    /// <see cref="ITrigger" /> fires that is associated with
    /// the <see cref="IJob" />.
    /// </summary>

    public MySqlConnection connection = new MySqlConnection(new MySqlConnectionStringBuilder
    {
        Server = "memo.cxywos9kigxi.ap-northeast-2.rds.amazonaws.com",
        Database = "MAP",
        UserID = "admin",
        Password = "memomemo!",
        Port = 3306,
        ConnectionTimeout = 60,
        AllowZeroDateTime = true
    }.ConnectionString);
    public virtual Task Execute(IJobExecutionContext context)
    {

DateTime dateTime = new DateTime();
        using (connection)
        {
            TimeSpan time1 = new TimeSpan();
            connection.Open();
            MySqlCommand cmd = new(@"
                SELECT 
                    *
                FROM 
                    `Time` t
                WHERE 
                NOW() < savedtime 
                ORDER BY 
                    savedtime 
                DESC
                LIMIT 1;

            ", connection);
            
            DateTime now = DateTime.Now;

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dateTime = Convert.ToDateTime(reader["savedtime"]);
                    time1 = dateTime - now;
                 
                }

            }
            Console.WriteLine(time1);
            Console.WriteLine(dateTime);
            Console.WriteLine(now);
            if (dateTime.Minute == DateTime.Now.Minute+1)
            {
                MySqlCommand updatecmd = new(@"
                CALL UpdateCoinPrices();
                CALL UpdateTotal();
            ", connection);
                updatecmd.ExecuteNonQuery();
                Console.WriteLine(dateTime);
                

            }


        }
        return Task.CompletedTask;
    }
}
