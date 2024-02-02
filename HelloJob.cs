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
                TIME(CONVERT_TZ(NOW(), 'UTC', 'Asia/Seoul')) < savedtime 
                ORDER BY 
                    savedtime 
                ASC
                LIMIT 1;

            ", connection);
            DateTime dateTime;
            DateTime now = DateTime.Now;

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dateTime = Convert.ToDateTime(reader["savedtime"]);
                    time1 = dateTime - now;
                    Console.WriteLine("DB에서 받아온 날짜 : {0}", dateTime);
                    Console.WriteLine("현재 날짜 : {0}", now);
                    // Console.WriteLine("현재 날짜ShortDateString: {0}", now.ToShortDateString());
                    // Console.WriteLine("현재 시간: {0}", now.ToShortTimeString());
                    Console.WriteLine("DB 날짜 - 지금 날짜 : {0}", dateTime - now);
                }

            }

            if (time1.Minutes < 1)
            {
                MySqlCommand updatecmd = new(@"
                CALL UpdateCoinPrices();
                CALL UpdateTotal();
            ", connection);
                updatecmd.ExecuteNonQuery();

            }


        }
Console.WriteLine("현재 날짜 : {0}", DateTime.Now);
        // id = Convert.ToInt32(reader["id"]),
        //                     name = reader["name"].ToString(),
        //                     crew = reader["crew"].ToString(),
        //                     type = reader["type"].ToString(),
        //                     balance = Convert.ToInt32(reader["balance"]),
        //                     phonenum = reader["phonenum"].ToString(),
        //                     admin = Convert.ToBoolean(reader["admin"])

        return Task.CompletedTask;
    }
}
