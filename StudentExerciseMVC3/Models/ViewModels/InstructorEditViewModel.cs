using Microsoft.AspNetCore.Mvc.Rendering;
using StudentExercisesAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC3.Models.ViewModels
{
    public class InstructorEditViewModel
    {
        public Instructor instructor { get; set; } = new Instructor();

        public List<SelectListItem> Cohorts { get; set; } = new List<SelectListItem>();

        public SqlConnection Connection;

        public InstructorEditViewModel()
        {

        }
        public InstructorEditViewModel(SqlConnection connection, int id)
        {
            Connection = connection;
            GetAllCohorts(id);
        }

        public void GetAllCohorts(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select c.Id, c.Designation from Cohort c;";

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

                    Cohorts = cohorts.Select(li => new SelectListItem
                        {
                            Text = li.Designation,
                            Value = li.Id.ToString()

                        }).ToList();
                    
                    Cohorts.Insert(0, new SelectListItem
                    {
                        Text = "Choose cohort ...",
                        Value = "0"
                    });
                    reader.Close();
                }
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT i.Id, 
                        i.FirstName, 
                        i.LastName, 
                        i.SlackHandle, 
                        i.CohortId,
                        i.Specialty,
                        c.Designation 
                        FROM Instructor i Join Cohort c 
                        ON i.CohortId = c.Id
                        Where i.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {


                        instructor.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        instructor.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        instructor.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        instructor.SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"));
                        instructor.CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        instructor.Specialty = reader.GetString(reader.GetOrdinal("Specialty"));
                        instructor.Cohort = new Cohort
                        {
                            Designation = reader.GetString(reader.GetOrdinal("Designation")),
                            Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        }


                    ;

                    }
                    reader.Close();

                }
            }
        }
    }
}
