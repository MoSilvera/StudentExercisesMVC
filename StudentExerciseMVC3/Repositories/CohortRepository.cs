using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC3.Models;
using StudentExercisesAPI.Models;

namespace StudentExerciseMVC3.Repositories
{
    public class CohortRepository
    {
        private static IConfiguration _config;

        public static void SetConfig(IConfiguration configuration)
        {
            _config = configuration;
        }

        public static SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public static Student GetStudent(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT s.Id, 
                        s.FirstName, 
                        s.LastName, 
                        s.SlackHandle, 
                        s.CohortId, 
                        c.Designation 
                        FROM Student s Join Cohort c 
                        ON s.CohortId = c.Id
                        Where s.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Designation = reader.GetString(reader.GetOrdinal("Designation")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            }


                        };

                    }
                    reader.Close();
                    return student;
                }
            }
        }
        public static List<Cohort> GetCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id,
                        c.Designation
                        FROM Cohort c;
                        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Designation = reader.GetString(reader.GetOrdinal("Designation"))


                        };

                        cohorts.Add(cohort);
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }

        public static Student CreateStudent(Student Student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstName, @lastName, @slackHandle, @cohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", Student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", Student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", Student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", Student.CohortId));

                    int newId = (int)cmd.ExecuteScalar();
                    Student.Id = newId;

                    return Student;
                }
            }
        }

        public static void UpdateStudent(int id, IFormCollection collection)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Student
                                                SET FirstName = @firstName,
                                                    LastName = @lastName,
                                                    SlackHandle = @slackHandle,
                                                    CohortId = @cohortId
                                                WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@firstName", Convert.ToString(collection["Student.FirstName"])));
                    cmd.Parameters.Add(new SqlParameter("@lastName", Convert.ToString(collection["Student.LastName"])));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", Convert.ToString(collection["Student.SlackHandle"])));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", (Convert.ToInt32(collection["Student.CohortId"]))));

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    

                }
            }
        }

        public static bool DeleteStudent (int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Student WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0) return false;
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        
    }
}
