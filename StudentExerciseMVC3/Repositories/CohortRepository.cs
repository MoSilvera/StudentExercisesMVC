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
        public static Cohort GetCohort(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        $@"
                        SELECT
                        c.Id,
                        c.Designation
                        FROM Cohort c
                        Where c.id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Designation = reader.GetString(reader.GetOrdinal("Designation"))

                        };

                    }
                    reader.Close();
                    return cohort;

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
